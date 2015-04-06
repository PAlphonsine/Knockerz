using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EventsNewspaperButtonScript : MonoBehaviour
{
	// Panel du journal d'évènements
	[SerializeField]
	GameObject eventsNewspaperButtonPanel;
	// Composant texte contenant les différents évènements survenus
	[SerializeField]
	Text newsFlowText;
	// Gestion des phases
	[SerializeField]
	PhasesManager phasesManager;
	// Gestion de la population
	[SerializeField]
	PopManager popManager;
	// Gestion d'envoi et de retour des Survivants
	[SerializeField]
	ReturningSurvivors returningSurvivors;
	// Booléen de controle d'affichage des évènements
	private bool newsShown;
	// Booléen de controle d'envoi des Survivants
	private bool popSent;
	
	// Use this for initialization
	void Start ()
	{
		this.newsShown = false;
		this.popSent = false;
	}
	
	void FixedUpdate ()
	{
		// Si la partie a commencé
		if(this.phasesManager.startgame == true)
		{
			// Si l'on est en phase de réflexion
			if (this.phasesManager.startAction == false)
			{
				// Le journal affiche des évènements relatifs à la phase de réflexion
				this.IntoMinding();
			}
			// Sinon, si l'on est en phase d'action
			else
			{
				// Le journal affiche des évènements relatifs à la phase d'action
				this.IntoAction();
			}
		}
	}
	
	// Méthode d'affichage des évènements de la phase de réflexion
	private void IntoMinding()
	{
		// Si les évènements n'ont pas encore été affichés
		if(this.newsShown == false)
		{
			// On informe le joueur du début de la phase de réflexion
			this.newsFlowText.text += "La phase de réflexion débute. Pensez à tout !\n";
			
			if(this.popManager.PopToSendNextPhaseMaterials + this.popManager.PopToSendNextPhaseWeapons == 0)
			{
				// Si de la population a été envoyée
				if(this.popSent == true)
				{
					// On affiche les rescapés de retour à la phase de réflexion
					this.newsFlowText.text += "Rescapés matériaux : " + this.returningSurvivors.CountMaterials + "\n";
					this.newsFlowText.text += "Rescapés armes : " + this.returningSurvivors.CountWeapons + "\n";
				}
				else
				{
					// Sinon, on affiche qu'aucune population n'a été envoyée
					this.newsFlowText.text += "Aucune population envoyée.\n";
				}
			}
		}
		// Les évènements ont été affichés pour la phase de réflexion
		this.newsShown = true;
	}
	
	// Méthode d'affichage des évènements de la phase d'action
	private void IntoAction()
	{
		// Si les évènements de la phsae de réflexion ont été affichés
		if(this.newsShown == true)
		{
			// On informe le joueur du début de la phase d'action
			this.newsFlowText.text += "Les Zombies arrivent ... Défendez-vous !\n";
			// Si aucun Survivant n'a été envoyé
			if(this.popManager.PopToSendNextPhaseMaterials + this.popManager.PopToSendNextPhaseWeapons == 0)
			{
				// On affiche qu'aucun Survivant n'a été envoyé
				this.newsFlowText.text += "Aucun population envoyée.\n";
			}
			// Sinon, si un ou plusieurs Survivants ont été envoyés
			else
			{
				// On affiche le nombre de Survivants envoyés pour chaque ressource
				this.newsFlowText.text += "Population envoyée matériaux : " + this.popManager.PopToSendNextPhaseMaterials + "\n";
				this.newsFlowText.text += "Population envoyée armes : " + this.popManager.PopToSendNextPhaseWeapons + "\n";
				// La population a été envoyée
				this.popSent = true;
			}
		}
		// Les évènements de la phase d'action ont été affichés, les nouveaux évènements pas encore
		this.newsShown = false;
	}
	
	// Méthode d'activation et de désactivation du panel du journal d'évènements
	public void EventsNewspaperButtonPanelEnabled()
	{
		this.eventsNewspaperButtonPanel.SetActive(!this.eventsNewspaperButtonPanel.activeSelf);
	}
	
	// Accesseurs
	public bool NewsShown
	{
		get { return this.newsShown; }
	}
}
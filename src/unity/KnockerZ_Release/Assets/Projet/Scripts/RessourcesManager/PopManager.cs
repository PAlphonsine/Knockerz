using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopManager : MonoBehaviour
{
	//http://docs.unity3d.com/ScriptReference/Editor.html
	
	// Collection contenant tous les Survivants possédés par le joueur dans sa base
	[SerializeField] SentSurvivorScript[] allSurvivorsJ1;
	[SerializeField] SentSurvivorScript[] allSurvivorsJ2;
	// Journal d'évènements
	[SerializeField] EventsNewspaperScript eventsNewspaperScript;
	// Textes de l'interface
	[SerializeField] Text popText;
	[SerializeField] Text popDeclaredTextMaterials;
	[SerializeField] Text popDeclaredTextWeapons;
	// Boutons de controle du nombre de Survivants et d'envoi des Survivants
	[SerializeField] Button moreSurvivorsMaterials;
	[SerializeField] Button lessSurvivorsMaterials;
	[SerializeField] Button more10SurvivorsMaterials;
	[SerializeField] Button less10SurvivorsMaterials;
	[SerializeField] Button sendButtonMaterials;
	[SerializeField] Button moreSurvivorsWeapons;
	[SerializeField] Button more10SurvivorsWeapons;
	[SerializeField] Button lessSurvivorsWeapons;
	[SerializeField] Button less10SurvivorsWeapons;
	[SerializeField] Button sendButtonWeapons;
	// Gestion des phases
	[SerializeField] PhasesManager phasesManager;
	// Script d'envoi des Survivants
	[SerializeField] ReturningSurvivors returningSurvivors;
	// Variables de décisions et de confirmation d'envoi de la population
	private int popDeclaredMaterials;
	private int popDeclaredWeapons;
	//private int popToSendNextPhase;
	private int popToSendNextPhaseMaterials;
	private int popToSendNextPhaseWeapons;
	// Booléen d'envoi définitif des Survivants
	private bool sending;
	
	// Use this for initialization
	void Start ()
	{
		// Affichage de la population totale
		this.popText.text = "Pop : " + GameStats.Instance.Population;
		//Debug.Log("PM/Mise à jour du texte : " + GameStats.Instance.Population);
		// La population planifiée et envoyée est à zéro au début
		this.popDeclaredMaterials = 0;
		this.popDeclaredWeapons = 0;
		//this.popToSendNextPhase = 0;
		this.popToSendNextPhaseMaterials = 0;
		this.popToSendNextPhaseWeapons = 0;
		// Affichage de la population planifiée dans la fenetre d'envoi
		this.popDeclaredTextMaterials.text = "Population à envoyer : " + popDeclaredMaterials.ToString();
		this.popDeclaredTextWeapons.text = "Population à envoyer : " + popDeclaredWeapons.ToString();
		// Les Survivants ne partent qu'en phase d'action
		this.sending = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Si la partie a commencé
		if(this.phasesManager.startgame == true)
		{
			// La population totale se met à jour
			this.popText.text = "Pop : " + GameStats.Instance.Population;
			//Debug.Log("PM/Mise à jour du texte : " + GameStats.Instance.Population);
			// La population planifiée se met à jour
			this.popDeclaredTextMaterials.text = "Population à envoyer : " + popDeclaredMaterials.ToString();
			this.popDeclaredTextWeapons.text = "Population à envoyer : " + popDeclaredWeapons.ToString();
			// Si on passe en phase d'action et que personne n'a encore été envoyé
			if (this.phasesManager.startAction == true && sending == false)
			{
				//Debug.Log("Envoyés Matériaux : " + this.popToSendNextPhaseMaterials);
				//Debug.Log("Envoyés Armes : " + this.popToSendNextPhaseWeapons);
				// On instancie le tableau de Survivants envoyés selon la population planifiée en phase de réflexion
				returningSurvivors.sentSurvivorsMaterials = new SentSurvivorScript[popToSendNextPhaseMaterials];
				returningSurvivors.sentSurvivorsWeapons = new SentSurvivorScript[popToSendNextPhaseWeapons];
				// Pour le joueur 1
				if (_STATICS._networkPlayer[0] == Network.player)
				{
					// Pour chaque Survivant planifié
					for (int i = 0; i < popToSendNextPhaseMaterials; i++)
					{
						// On ajoute des Survivants depuis notre population
						returningSurvivors.sentSurvivorsMaterials[i] = this.allSurvivorsJ1[i];
					}

					for (int i = popToSendNextPhaseMaterials; i < popToSendNextPhaseWeapons + popToSendNextPhaseMaterials; i++)
					{
						// On ajoute des Survivants depuis notre population
						returningSurvivors.sentSurvivorsWeapons[i - popToSendNextPhaseMaterials] = this.allSurvivorsJ1[i - popToSendNextPhaseMaterials];
					}
				}
				// Sinon, pour le joueur 2
				else if (_STATICS._networkPlayer[1] == Network.player)
				{
					// Pour chaque Survivant planifié
					for (int i = 0; i < popToSendNextPhaseMaterials; i++)
					{
						// On ajoute des Survivants depuis notre population
						returningSurvivors.sentSurvivorsMaterials[i] = this.allSurvivorsJ2[i];
					}
					
					for (int i = popToSendNextPhaseMaterials; i < popToSendNextPhaseWeapons + popToSendNextPhaseMaterials; i++)
					{
						// On ajoute des Survivants depuis notre population
						returningSurvivors.sentSurvivorsWeapons[i - popToSendNextPhaseMaterials] = this.allSurvivorsJ2[i - popToSendNextPhaseMaterials];
					}
				}
				// Les Survivants ont été envoyés
				sending = true;
			}
			// Lorsque l'on revient en phase de réflexion, on permet de nouveau l'envoi de Survivants
			if(this.phasesManager.startAction == false && sending == true)
			{
				sending = false;
				// On rétablit la population envoyée aux ressources à 0
				this.popToSendNextPhaseMaterials = 0;
				this.popToSendNextPhaseWeapons = 0;
			}
		}
	}
	
	void FixedUpdate()
	{
		// Si la partie a commencé
		if(this.phasesManager.startgame == true)
		{
			// Si on est en phase de réflexion
			if(this.phasesManager.startAction == false)
			{
				if (this.popDeclaredMaterials <= 0)
				{
					this.sendButtonMaterials.interactable = false;
				}
				else
				{
					this.sendButtonMaterials.interactable = true;
				}

				if (this.popDeclaredMaterials < 10)
				{
					this.less10SurvivorsMaterials.interactable = false;
					if (this.popDeclaredMaterials <= 0)
					{
						this.lessSurvivorsMaterials.interactable = false;
					}
					else
					{
						this.lessSurvivorsMaterials.interactable = true;
					}
				}
				else
				{
					this.less10SurvivorsMaterials.interactable = true;
					this.lessSurvivorsMaterials.interactable = true;
				}
				
				if (GameStats.Instance.Population - this.popDeclaredMaterials < 10)
				{
					this.more10SurvivorsMaterials.interactable = false;
					if (GameStats.Instance.Population - this.popDeclaredMaterials <= 0)
					{
						this.moreSurvivorsMaterials.interactable = false;
					}
					else
					{
						this.moreSurvivorsMaterials.interactable = true;
					}
				}
				else
				{
					this.more10SurvivorsMaterials.interactable = true;
					this.moreSurvivorsMaterials.interactable = true;
				}

				if (this.popDeclaredWeapons <= 0)
				{
					this.sendButtonWeapons.interactable = false;
				}
				else
				{
					this.sendButtonWeapons.interactable = true;
				}
				
				if (this.popDeclaredWeapons < 10)
				{
					this.less10SurvivorsWeapons.interactable = false;
					if (this.popDeclaredWeapons <= 0)
					{
						this.lessSurvivorsWeapons.interactable = false;
					}
					else
					{
						this.lessSurvivorsWeapons.interactable = true;
					}
				}
				else
				{
					this.less10SurvivorsWeapons.interactable = true;
					this.lessSurvivorsWeapons.interactable = true;
				}
				
				if (GameStats.Instance.Population - this.popDeclaredWeapons < 10)
				{
					this.more10SurvivorsWeapons.interactable = false;
					if (GameStats.Instance.Population - this.popDeclaredWeapons <= 0)
					{
						this.moreSurvivorsWeapons.interactable = false;
					}
					else
					{
						this.moreSurvivorsWeapons.interactable = true;
					}
				}
				else
				{
					this.more10SurvivorsWeapons.interactable = true;
					this.moreSurvivorsWeapons.interactable = true;
				}
			}
			// Si on est en phase d'action
			else
			{
				// Il est impossible d'envoyer des Survivants - Tous les boutons sont désactivés
				this.moreSurvivorsMaterials.interactable = false;
				this.more10SurvivorsMaterials.interactable = false;
				this.lessSurvivorsMaterials.interactable = false;
				this.less10SurvivorsMaterials.interactable = false;
				this.sendButtonMaterials.interactable = false;
				this.moreSurvivorsWeapons.interactable = false;
				this.more10SurvivorsWeapons.interactable = false;
				this.lessSurvivorsWeapons.interactable = false;
				this.less10SurvivorsWeapons.interactable = false;
				this.sendButtonWeapons.interactable = false;
			}
			
			// On met à jour en temps réel l'affichage de la population planifiée et de la population totale
			this.popText.text = "Pop : " + GameStats.Instance.Population;
			//Debug.Log("PM/Mise à jour du texte : " + GameStats.Instance.Population);
			this.popDeclaredTextMaterials.text = "Population à envoyer : " + popDeclaredMaterials.ToString();
			this.popDeclaredTextWeapons.text = "Population à envoyer : " + popDeclaredWeapons.ToString();
		}
	}
	
	// Méthode d'ajout d'un Survivant à la recherche de matériaux
	public void PopToSendMaterials(int pop)
	{
		popDeclaredMaterials += pop;
	}

	// Méthode de retrait d'un Survivant à la recherche de matériaux
	public void PopToKeepMaterials(int pop)
	{
		popDeclaredMaterials -= pop;
	}
	
	// Méthode d'ajout d'un Survivant à la recherche d'armes
	public void PopToSendWeapons(int pop)
	{
		popDeclaredWeapons += pop;
	}
	
	// Méthode de retrait d'un Survivant à la recherche d'armes
	public void PopToKeepWeapons(int pop)
	{
		popDeclaredWeapons -= pop;
	}
	
	// Méthode de validation d'envoi des Survivants planifiés aux matériaux
	public void PopPlanifyMaterials()
	{
		GameStats.Instance.Population -= popDeclaredMaterials;
		//Debug.Log ("PM/Retrait de Survivants : recherche matériaux - " + GameStats.Instance.Population);
		this.popToSendNextPhaseMaterials += popDeclaredMaterials;
		this.eventsNewspaperScript.MaterialsSurvivorsGo = this.popToSendNextPhaseMaterials;
		this.ResetMaterials ();
	}
	
	// Méthode de validation d'envoi des Survivants planifiés aux armes
	public void PopPlanifyWeapons()
	{
		GameStats.Instance.Population -= popDeclaredWeapons;
		//Debug.Log ("PM/Retrait de Survivants : recherche armes - " + GameStats.Instance.Population);
		this.popToSendNextPhaseWeapons += popDeclaredWeapons;
		this.eventsNewspaperScript.WeaponsSurvivorsGo = this.popToSendNextPhaseWeapons;
		this.ResetWeapons ();
	}

	public void ResetMaterials()
	{
		this.popDeclaredMaterials = 0;
	}

	public void ResetWeapons()
	{
		this.popDeclaredWeapons = 0;
	}
	
	// Accesseurs
	public int PopDeclaredMaterials
	{
		get { return this.popDeclaredMaterials; }
		set { this.popDeclaredMaterials = value; }
	}
	
	public int PopDeclaredWeapons
	{
		get { return this.popDeclaredWeapons; }
		set { this.popDeclaredWeapons = value; }
	}
	
	public int PopToSendNextPhaseMaterials
	{
		get { return this.popToSendNextPhaseMaterials; }
		set { this.popToSendNextPhaseMaterials = value; }
	}
	
	public int PopToSendNextPhaseWeapons
	{
		get { return this.popToSendNextPhaseWeapons; }
		set { this.popToSendNextPhaseWeapons = value; }
	}
	
	public bool Sending
	{
		get { return this.sending; }
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RessourcesButtonScript : MonoBehaviour
{
	// Gestion des phases
	[SerializeField]
	PhasesManager phasesManager;
	// Panel du bouton Ressources
	[SerializeField]
	GameObject ressourcesButtonPanel;
	// Panel de planification de recherche de matériaux
	[SerializeField]
	GameObject sendPopMaterialsPanel;
	// Panel de planification de recherche d'armes
	[SerializeField]
	GameObject sendPopWeaponsPanel;
	// Bouton du menu Ressources
	[SerializeField]
	Button ressourcesButton;

	void Update ()
	{
		if (this.phasesManager.startAction == false)
		{
			this.ressourcesButton.interactable = true;
		}
		else
		{
			this.ressourcesButton.interactable = false;
		}
	}

	// Méthode d'activation et de désactivation du panel du bouton Ressources
	public void RessourcesButtonPanelEnabled()
	{
		// Si le panel est désactivé ...
		if (this.ressourcesButtonPanel.activeSelf == false)
		{
			// ... on l'active
			this.ressourcesButtonPanel.gameObject.SetActive(true);
		}
		// Sinon, si le panel est activé ...
		else
		{
			// ... on le désactive
			this.ressourcesButtonPanel.gameObject.SetActive(false);
		}
	}
	
	// Méthode d'activation et de désactivation du panel de planification de recherche de matériaux
	public void SendPopMaterialsPanelEnabled()
	{
		// Si le panel est désactivé ...
		if (this.sendPopMaterialsPanel.activeSelf == false)
		{
			// ... on l'active
			this.sendPopMaterialsPanel.SetActive(true);
		}
		// Sinon, si le panel est activé ...
		else
		{
			// ... on le désactive
			this.sendPopMaterialsPanel.SetActive(false);
		}
	}
	
	// Méthode d'activation et de désactivation du panel de planification de recherche d'armes
	public void SendPopWeaponsPanelEnabled()
	{
		// Si le panel est désactivé ...
		if (this.sendPopWeaponsPanel.activeSelf == false)
		{
			// ... on l'active
			this.sendPopWeaponsPanel.SetActive(true);
		}
		// Sinon, si le panel est activé ...
		else
		{
			// ... on le désactive
			this.sendPopWeaponsPanel.SetActive(false);
		}
	}
}
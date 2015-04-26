using UnityEngine;
using System.Collections;

public class BadActsButtonScript : MonoBehaviour
{
	// Panel de choix entre boutique et inventaire de coups fourrés
	[SerializeField]
	GameObject badActsButtonPanel;
	// Panel de la boutique de coups fourrés
	[SerializeField]
	GameObject badActsShopPanel;
	// Panel de l'inventaire de coups fourrés
	[SerializeField]
	GameObject badActsInventoryPanel;
	
	// Méthode d'activation et de désactivation du panel du bouton de coups fourrés
	public void BadActsButtonPanelEnabled()
	{
		if(this.badActsButtonPanel.activeSelf == false)
		{
			this.badActsButtonPanel.SetActive(true);
		}
		else
		{
			this.badActsButtonPanel.SetActive(false);
		}
	}
	
	// Méthode d'activation ou désactivation du panel de la boutique de coups fourrés
	public void BadActsShopPanelEnabled()
	{
		this.badActsShopPanel.SetActive(!this.badActsShopPanel.activeSelf);
	}
	
	// Méthode d'activation et de désactivation du panel de l'inventaire de coups fourrés
	public void BadActsInventoryPanelEnabled()
	{
		this.badActsInventoryPanel.SetActive(!this.badActsInventoryPanel.activeSelf);
	}
}
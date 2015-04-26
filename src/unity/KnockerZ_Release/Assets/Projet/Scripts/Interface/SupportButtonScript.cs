using UnityEngine;
using System.Collections;

public class SupportButtonScript : MonoBehaviour
{
	// Panel du bouton Soutien
	[SerializeField]
	GameObject supportButtonPanel;
	// Panel de la boutique de soutiens
	[SerializeField]
	GameObject supportShopPanel;
	
	// Méthode d'activation et de désactivation du panel du bouton Soutien
	public void SupportButtonPanelEnabled()
	{
		this.supportButtonPanel.SetActive (!this.supportButtonPanel.activeSelf);
	}
	
	// Méthode d'activation et de désactivation du panel de la boutique de soutiens
	public void SupportShopPanelEnabled()
	{
		this.supportShopPanel.SetActive(!this.supportShopPanel.activeSelf);
	}
}
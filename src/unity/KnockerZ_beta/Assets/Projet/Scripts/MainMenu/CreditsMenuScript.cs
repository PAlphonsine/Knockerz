using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreditsMenuScript : MonoBehaviour
{
	// Panel contenant le menu des crédits
	[SerializeField]
	GameObject creditsMenuPanel;
	
	// Méthode d'activation/désactivation du menu de crédits
	public void CreditsMenuPanelEnabled()
	{
		// Si le panel est activé, on le désactive et inversement
		this.creditsMenuPanel.SetActive (!this.creditsMenuPanel.activeSelf);
	}
}
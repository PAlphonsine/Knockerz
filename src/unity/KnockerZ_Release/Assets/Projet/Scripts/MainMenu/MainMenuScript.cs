using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
	// Panel du menu principal
	[SerializeField] GameObject mainMenuPanel;

	// Méthode appellée pour quitter l'application
	public void Exit()
	{
		// Le jeu se ferme
		Application.Quit ();
	}
	
	// Méthode d'activation/désactivation du menu principal
	public void MainMenuPanelEnabled()
	{
		this.mainMenuPanel.SetActive(!this.mainMenuPanel.activeSelf);
	}
}
using UnityEngine;
using System.Collections;

public class LevelsMenuScript : MonoBehaviour
{
	// Panel du menu de choix de la carte
	[SerializeField] GameObject levelsMenuPanel;
	// Panel de chargement
	[SerializeField] GameObject loadingPanel;

	// Méthode de chargement de la carte désirée
	public void LoadMap(string mapName)
	{
		Application.LoadLevel (mapName);
	}

	// Méthode d'activation/désactivation du menu de choix de la carte
	public void LevelsMenuPanelEnabled()
	{
		// Si le panel est activé, on le désactive et inversement
		this.levelsMenuPanel.SetActive (!this.levelsMenuPanel.activeSelf);
	}

	// Méthode d'activation/désactivation de l'écran de chargement
	public void LoadingPanelEnabled()
	{
		// Si le panel est activé, on le désactive et inversement
		this.loadingPanel.SetActive (!this.loadingPanel.activeSelf);
	}
}
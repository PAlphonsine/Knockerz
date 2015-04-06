using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour
{
	// Panel du menu des options
	[SerializeField]
	GameObject settingsMenuPanel;
	// Toggle du mode grand écran
	[SerializeField]
	Toggle fullscreenToggle;
	// Script de gestion de résolutions
	[SerializeField]
	ResolutionsManager resolutionsManager;
	
	// Use this for initialization
	void Start ()
	{
		if (Screen.fullScreen == true)
		{
			this.fullscreenToggle.isOn = true;
		}
	}

	// Méthode de définition du mode grand écran
	public void FullScreenSet()
	{
		if (this.fullscreenToggle.isOn == true)
		{
			Screen.fullScreen = true;
		}
		else
		{
			Screen.fullScreen = false;
		}
	}

	public void ResolutionSet(int i)
	{
		Screen.SetResolution (Screen.resolutions [i].width, Screen.resolutions [i].height, Screen.fullScreen);
	}
	
	// Méthode d'activation/désactivation du menu d'options
	public void SettingsMenuPanelEnabled()
	{
		// Si le panel est activé, on le désactive et inversement
		this.settingsMenuPanel.SetActive (!this.settingsMenuPanel.activeSelf);
	}
}
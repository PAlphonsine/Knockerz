using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsMenuScript : MonoBehaviour
{
	// Panel du menu des options
	[SerializeField] GameObject settingsMenuPanel;
	// Toggle du mode grand écran
	[SerializeField] Toggle fullscreenToggle;
	// Boutons de réglage de la qualité graphique
	[SerializeField] Button ultraQualityButton;
	[SerializeField] Button beautifulQualityButton;
	[SerializeField] Button simpleQualityButton;
	[SerializeField] Button fastestQualityButton;
	// Script de gestion de résolutions
	[SerializeField] ResolutionsManager resolutionsManager;
	
	// Use this for initialization
	void Start ()
	{
		// Si le joueur a coché le mode grand écran dans la fenetre de démarrage, le Toggle du menu des options est coché
		if (Screen.fullScreen == true)
		{
			this.fullscreenToggle.isOn = true;
		}

		// Au lancement du jeu, on récupère la qualité désirée (celle par défaut ou celle choisie dans la fenetre
		// précédant le lancement du jeu) et on surligne le bouton de qualité graphique correspondant
		if (QualitySettings.GetQualityLevel() < 2)
		{
			QualitySettings.SetQualityLevel (0, true);
			ColorBlock buttonColors = this.fastestQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 0f, 0f, 1f);
			buttonColors.highlightedColor = new Color (1f, 0f, 0f, 1f);
			this.fastestQualityButton.colors = buttonColors;
		}
		else if (QualitySettings.GetQualityLevel() < 4)
		{
			QualitySettings.SetQualityLevel (2, true);
			ColorBlock buttonColors = this.simpleQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 0f, 0f, 1f);
			buttonColors.highlightedColor = new Color (1f, 0f, 0f, 1f);
			this.simpleQualityButton.colors = buttonColors;
		}
		else if (QualitySettings.GetQualityLevel() < 6)
		{
			QualitySettings.SetQualityLevel (4, true);
			ColorBlock buttonColors = this.beautifulQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 0f, 0f, 1f);
			buttonColors.highlightedColor = new Color (1f, 0f, 0f, 1f);
			this.beautifulQualityButton.colors = buttonColors;
		}
		else if (QualitySettings.GetQualityLevel() == 6)
		{
			QualitySettings.SetQualityLevel (6, true);
			ColorBlock buttonColors = this.ultraQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 0f, 0f, 1f);
			buttonColors.highlightedColor = new Color (1f, 0f, 0f, 1f);
			this.ultraQualityButton.colors = buttonColors;
		}
	}

	// Méthode de définition de la qualité graphique
	public void SetQuality(int i)
	{
		// On applique la qualité désirée selon le bouton sur lequel le joueur a cliqué
		QualitySettings.SetQualityLevel (i, true);

		// On change la Normal Color et la Highlited Color du bouton dont la qualité a été sélectionnée
		if (i == 0)
		{
			ColorBlock buttonColors = this.fastestQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 0f, 0f, 1f);
			buttonColors.highlightedColor = new Color (1f, 0f, 0f, 1f);
			this.fastestQualityButton.colors = buttonColors;
		}
		// Sinon, on réapplique au bouton ses couleurs par défaut
		else
		{
			ColorBlock buttonColors = this.fastestQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 1f, 1f, 1f);
			buttonColors.highlightedColor = new Color (1f, 1f, 1f, 1f);
			this.fastestQualityButton.colors = buttonColors;
		}
		
		if (i == 2)
		{
			ColorBlock buttonColors = this.simpleQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 0f, 0f, 1f);
			buttonColors.highlightedColor = new Color (1f, 0f, 0f, 1f);
			this.simpleQualityButton.colors = buttonColors;
		}
		else
		{
			ColorBlock buttonColors = this.simpleQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 1f, 1f, 1f);
			buttonColors.highlightedColor = new Color (1f, 1f, 1f, 1f);
			this.simpleQualityButton.colors = buttonColors;
		}
		
		if (i == 4)
		{
			ColorBlock buttonColors = this.beautifulQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 0f, 0f, 1f);
			buttonColors.highlightedColor = new Color (1f, 0f, 0f, 1f);
			this.beautifulQualityButton.colors = buttonColors;
		}
		else
		{
			ColorBlock buttonColors = this.beautifulQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 1f, 1f, 1f);
			buttonColors.highlightedColor = new Color (1f, 1f, 1f, 1f);
			this.beautifulQualityButton.colors = buttonColors;
		}
		
		if (i == 6)
		{
			ColorBlock buttonColors = this.ultraQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 0f, 0f, 1f);
			buttonColors.highlightedColor = new Color (1f, 0f, 0f, 1f);
			this.ultraQualityButton.colors = buttonColors;
		}
		else
		{
			ColorBlock buttonColors = this.ultraQualityButton.colors;
			buttonColors.normalColor = new Color (1f, 1f, 1f, 1f);
			buttonColors.highlightedColor = new Color (1f, 1f, 1f, 1f);
			this.ultraQualityButton.colors = buttonColors;
		}
	}

	// Méthode de définition du mode grand écran
	public void SetFullScreen()
	{
		// Si le Toggle est coché
		if (this.fullscreenToggle.isOn == true)
		{
			// On passe en grand écran
			Screen.fullScreen = true;
		}
		else
		{
			// Sinon on passe en fenetré
			Screen.fullScreen = false;
		}
	}

	// Méthode de définition de la résolution
	public void SetResolution(int i)
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
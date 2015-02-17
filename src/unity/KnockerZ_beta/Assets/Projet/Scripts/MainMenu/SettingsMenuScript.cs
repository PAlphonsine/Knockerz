using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour {
	// Tous les composants du menu d'options
	[SerializeField]
	Text settingsMenuTitle;
	[SerializeField]
	Toggle fullscreenToggle;
	[SerializeField]
	Button validateButton;
	[SerializeField]
	Button backButton;

	// Booléens de vérification de changement d'options et de validation des changements
	//private bool settingsChanged;
	//private bool validated;
	
	// Use this for initialization
	void Start () {
		//this.settingsChanged = false;
		//this.validated = false;
		if (Screen.fullScreen == true)
		{
			this.fullscreenToggle.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {

	}

	public void FullScreenSet()
	{
		Screen.fullScreen = !Screen.fullScreen;
	}

	// Méthode d'activation/désactivation du menu d'options
	public void SettingsMenuEnabled(bool b)
	{
		if (b == true)
		{
			this.settingsMenuTitle.enabled = true;
			this.fullscreenToggle.gameObject.SetActive(true);
			this.validateButton.gameObject.SetActive(true);
			this.backButton.gameObject.SetActive(true);
		}
		else
		{
			this.settingsMenuTitle.enabled = false;
			this.fullscreenToggle.gameObject.SetActive(false);
			this.validateButton.gameObject.SetActive(false);
			this.backButton.gameObject.SetActive(false);
		}
	}
}
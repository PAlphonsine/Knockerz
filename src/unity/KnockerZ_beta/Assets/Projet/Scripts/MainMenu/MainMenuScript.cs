using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {
	// Tous les composants du menu principal
	[SerializeField]
	Text mainMenuTitle;
	[SerializeField]
	Button playButton;
	[SerializeField]
	Button settingsButton;
	[SerializeField]
	Button creditsButton;
	[SerializeField]
	Button exitButton;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {

	}

	// Méthode d'activation/désactivation du menu principal
	public void MainMenuEnabled(bool b)
	{
		if (b == true)
		{
			this.mainMenuTitle.enabled = true;
			this.playButton.gameObject.SetActive(true);
			this.settingsButton.gameObject.SetActive(true);
			this.creditsButton.gameObject.SetActive(true);
			this.exitButton.gameObject.SetActive(true);
		}
		else
		{
			this.mainMenuTitle.enabled = false;
			this.playButton.gameObject.SetActive(false);
			this.settingsButton.gameObject.SetActive(false);
			this.creditsButton.gameObject.SetActive(false);
			this.exitButton.gameObject.SetActive(false);
		}
	}

	// Méthode appellant la scène (pour le moment, ammènera à un menu abouti dans le futur)
	public void Play()
	{
		Application.LoadLevel ("MainScene");
	}

	// Méthode appellée pour quitter l'application
	public void Exit()
	{
		Application.Quit ();
	}
}
















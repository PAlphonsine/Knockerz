using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreditsMenuScript : MonoBehaviour {
	// Tous les composants du menu de crédits
	[SerializeField]
	Text creditsMenuTitle;
	[SerializeField]
	Text creditsText;
	[SerializeField]
	Button backButton;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {

	}
	
	// Méthode d'activation/désactivation du menu de crédits
	public void CreditsMenuEnabled(bool b)
	{
		if (b == true)
		{
			this.creditsMenuTitle.enabled = true;
			this.creditsText.enabled = true;
			this.backButton.gameObject.SetActive(true);
		}
		else
		{
			this.creditsMenuTitle.enabled = false;
			this.creditsText.enabled = false;
			this.backButton.gameObject.SetActive(false);
		}
	}
}

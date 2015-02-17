using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameMenuButtonScript : MonoBehaviour {

	[SerializeField]
	GameObject inGameMenuPanel;

	[SerializeField]
	GameObject inGameAvatarPanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {

	}

	public void InGameMenuPanelEnabled()
	{
		if (this.inGameMenuPanel.activeSelf == false)
		{
			this.inGameMenuPanel.gameObject.SetActive(true);
		}
		else
		{
			this.inGameMenuPanel.gameObject.SetActive(false);
		}
	}

	public void BackToMainMenu()
	{
		Application.LoadLevel ("MenuScene");
		Network.Disconnect ();
	}

	public void InGameAvatarPanel()
	{
		if (this.inGameAvatarPanel.activeSelf == false)
		{
			this.inGameAvatarPanel.gameObject.SetActive(true);
		}
		else
		{
			this.inGameAvatarPanel.gameObject.SetActive(false);
		}
	}
}

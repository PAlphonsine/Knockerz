using UnityEngine;
using System.Collections;

public class SupportButtonScript : MonoBehaviour {
	[SerializeField]
	GameObject supportButtonPanel;
	[SerializeField]
	GameObject supportShopPanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		
	}

	public void SupportButtonPanelEnabled()
	{
		if (this.supportButtonPanel.activeSelf == false)
		{
			this.supportButtonPanel.SetActive(true);
		}
		else
		{
			this.supportButtonPanel.SetActive(false);
		}
	}

	public void SupportShopPanelEnabled()
	{
		if (this.supportShopPanel.activeSelf == false)
		{
			this.supportShopPanel.SetActive(true);
		}
		else
		{
			this.supportShopPanel.SetActive(false);
		}
	}
}

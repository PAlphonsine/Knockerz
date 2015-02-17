using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RessourcesButtonScript : MonoBehaviour {
	[SerializeField]
	GameObject ressourcesButtonPanel;
	[SerializeField]
	GameObject sendPopMaterialsPanel;
	[SerializeField]
	GameObject sendPopWeaponsPanel;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {
		
	}

	public void RessourcesButtonPanelEnabled()
	{
		if (this.ressourcesButtonPanel.activeSelf == false)
		{
			this.ressourcesButtonPanel.gameObject.SetActive(true);
		}
		else
		{
			this.ressourcesButtonPanel.gameObject.SetActive(false);
		}
	}

	public void SendPopMaterialsPanelEnabled()
	{
		if (this.sendPopMaterialsPanel.activeSelf == false)
		{
			this.sendPopMaterialsPanel.SetActive(true);
		}
		else
		{
			this.sendPopMaterialsPanel.SetActive(false);
		}
	}
	
	public void SendPopWeaponsPanelEnabled()
	{
		if (this.sendPopWeaponsPanel.activeSelf == false)
		{
			this.sendPopWeaponsPanel.SetActive(true);
		}
		else
		{
			this.sendPopWeaponsPanel.SetActive(false);
		}
	}
}

using UnityEngine;
using System.Collections;

public class MissionsButtonScript : MonoBehaviour {
	[SerializeField]
	GameObject missionsButtonPanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {

	}

	public void MissionsButtonPanelEnabled()
	{
		if(this.missionsButtonPanel.activeSelf == false)
		{
			this.missionsButtonPanel.SetActive(true);
		}
		else
		{
			this.missionsButtonPanel.SetActive(false);
		}
	}
}

using UnityEngine;
using System.Collections;

public class BadActsButtonScript : MonoBehaviour {
	[SerializeField]
	GameObject badActsButtonPanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		
	}

	public void BadActsButtonPanelEnabled()
	{
		if(this.badActsButtonPanel.activeSelf == false)
		{
			this.badActsButtonPanel.SetActive(true);
		}
		else
		{
			this.badActsButtonPanel.SetActive(false);
		}
	}
}

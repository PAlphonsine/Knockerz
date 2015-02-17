using UnityEngine;
using System.Collections;

public class DebugLogNetwork : MonoBehaviour {

	public PhasesManager _phasesManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (_phasesManager.startgame) {
			gameObject.renderer.material.color = Color.red;
		}
	}
}

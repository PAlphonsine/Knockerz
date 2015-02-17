using UnityEngine;
using System.Collections;

public class TurretMenuSet : MonoBehaviour {
	
	public GameObject[] tabs;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ActiveMenu(){
		foreach (GameObject n in tabs) {
			n.SetActive(true);
		}
	}

	public void DesactiveMenu(){
		foreach (GameObject n in tabs) {
			n.SetActive(false);
		}
	}
}

using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour {

	public DoorsManager _doorsManager;

	private int pv;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider collider){
		if(collider.tag == "Zombie" || collider.tag == "Survivor"){
			_doorsManager.CantClose = true;
		}
	}

	void OnTriggerExit(Collider collider){
		if(collider.tag == "Zombie" || collider.tag == "Survivor")
			_doorsManager.CantClose = false;
	}

	public int Pv
	{
		get { return _doorsManager.Pv; }
		set {	_doorsManager.Pv = value;}
	}
}

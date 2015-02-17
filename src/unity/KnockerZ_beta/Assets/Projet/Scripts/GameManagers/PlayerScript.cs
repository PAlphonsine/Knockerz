using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public bool IsStarted = false;
	public NetworkPlayer _networkPlayer;
	public GameObject PlayerCamera;
	public BaseScript EnemyBase;
	public GameObject StartButton;

	// Use this for initialization
	void Start () {
	}
	
	void Update () {
		if (Input.GetKey("escape"))
			Application.Quit();
	}
}

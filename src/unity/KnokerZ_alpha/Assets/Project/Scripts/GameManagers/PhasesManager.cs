using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhasesManager : MonoBehaviour {

	public Text time;
	public float vtime;
	public bool startgame = false;
	public bool startAction = false;
	public float vtimeA = 6f;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (startgame == true) {
			if(!startAction){
				if (vtime > 0.1) {
					vtime -= Time.deltaTime;
					//vtime = (int)vtime;
					time.text = "Phase de reflection" + ((int)vtime).ToString ();
				} else {
					time.text = "TimeOut";
					startAction = true;
					vtimeA = 6;
				}
			}else{
				if (vtimeA > 0.1) {
					vtimeA -= Time.deltaTime;
					//vtime = (int)vtime;
					time.text = "Phase d'action" + ((int)vtimeA).ToString ();
				} else {
					time.text = "TimeOut";
					startAction = false;
					vtime = 6;
				}
			}
		}
	}
}

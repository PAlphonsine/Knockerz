using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhasesManager : MonoBehaviour {

	public Text time;
	public float vtime;
	public bool startgame = false;
	public bool startAction = false;
	public float vtimeA;
	public bool switchPhase=false;
	float timeAchose;
	float timeRchose;

	void OnSerializeNetworkView(BitStream stream){
		//Debug.Log ("ok");
		//stream.Serialize(ref i);
		stream.Serialize(ref  vtime);
		stream.Serialize(ref  vtimeA);
		stream.Serialize(ref  switchPhase);
		stream.Serialize(ref  timeAchose);
		stream.Serialize(ref  timeRchose);
	}

	// Use this for initialization
	void Start () {
		timeAchose = vtimeA;
		timeRchose = vtime;
	}

	// Update is called once per frame
	void Update () {
		if (startgame == true) {
			if(!startAction){
				if (vtime > 0.1) {
					vtime -= Time.deltaTime;
					//vtime = (int)vtime;
					time.text = "Reflex : " + ((int)vtime).ToString ();
					switchPhase = false;
				} else {
					time.text = "TimeOut";
					startAction = true;
					vtimeA = timeAchose;
					switchPhase = true;
				}
			}else{
				if (vtimeA > 0.1) {
					vtimeA -= Time.deltaTime;
					//vtime = (int)vtime;
					time.text = "Action : " + ((int)vtimeA).ToString ();
					switchPhase = false;
				} else {
					time.text = "TimeOut";
					startAction = false;
					vtime = timeRchose;
					switchPhase = true;
				}
			}
		}
	}
}

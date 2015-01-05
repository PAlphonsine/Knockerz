using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BaseScript : MonoBehaviour {
	
	public GameObject prefab;

	public Camera _camera;

	public GameObject[] Zombies;
	private int EnCours=0;

	public PhasesManager _phasesManager;

	public Text ressourcesText;
	public Text pvText;

	public Text scores;

	public GameObject[] Survivants;
	//private int SEnCours=0;

	public bool isTheEnemyBase = false;

	void Start(){

	}

	void OnTriggerEnter(Collider collider)
	{
		//Debug.Log ("ok");
		if (collider.gameObject.tag.Equals("Zombie")) 
		{
			if(!isTheEnemyBase)
				GameStats.Instance.Pv-=10;
			Destroy(collider.gameObject);
		}
		if(!isTheEnemyBase){
			if (GameStats.Instance.Pv <= 0) {Debug.Log("GameOver");
				if (Network.isClient) {
					networkView.RPC ("GameOverFunction", RPCMode.AllBuffered);
				}
			}

			if (collider.gameObject.tag.Equals ("Survivant")) 
			{
				GameStats.Instance.Population++;
			}
		}
	}

	[RPC]
	void GameOverFunction(){
		if (Network.isClient) {
			networkView.RPC ("ReturnGameStats", RPCMode.Others, Network.player, GameStats.Instance.Pv, GameStats.Instance.Population);
			//Application.LoadLevel(1);
		}
	}

	[RPC]
	void ReturnGameStats(NetworkPlayer player, int pv, int pop){
		if (Network.isClient) {
			if (player == _STATICS._networkPlayer [1]) {
				scores.text = "Score joueur 1 (vous) : PV = " + GameStats.Instance.Pv + " et POP = " + GameStats.Instance.Population + "\nScore joueur 2 : PV = " + pv + " et POP = " + pop;
			}
			if (player == _STATICS._networkPlayer [0]) {
				scores.text = "Score joueur 1 : PV = " + pv + " et POP = " + pop + "\nScore joueur 2 (vous) : PV = " + GameStats.Instance.Pv + " et POP = " + GameStats.Instance.Population;
			}
		}
	}

	void Update(){
		//DetecterObjet ();
		ressourcesText.text = "Ressources : " + GameStats.Instance.Ressources.ToString();
		pvText.text = "PV : " + GameStats.Instance.Pv.ToString();
		SpawnZombie ();
	}

	void SpawnZombie(){
		if (EnCours < Zombies.Length-1 && _phasesManager.vtime <= 0.1) {
			if (Zombies [EnCours] == null) {
				EnCours++;
				Zombies [EnCours].SetActive (true);
			}
		}
	}

}

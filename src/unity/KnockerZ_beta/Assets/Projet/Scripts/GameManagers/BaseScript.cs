using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BaseScript : MonoBehaviour {

	//Variables pour la gestions des phases
	public StockZombies stock;
	public int phase = 1;
	public float timez1= 3;
	public float timez2= 3;
	public float timez3= 3;
	public float times= 3;
	public int nbzombies1=2;
	public int nbzombies2=0;
	public int nbzombies3=0;
	public int nbsurvivor=0;
	private int iz1=0;
	private int iz2=0;
	private int iz3=0;
	private int is1=0;
	
	public PhasesManager _phasesManager;

	public Text ressourcesMatText;
	public Text ressourcesWeapText;
	public Text pvText;
	public Text scores;
	public Text goldText;

	public GameObject[] Survivants;
	//private int SEnCours=0;

	//Variable pour savoir si la base est celle du joueur ou de l'aversaire
	public bool isTheEnemyBase = false;

	void Start(){
		//Pause
		//Time.timeScale = 0.5f;
	}

	//Quand un gameobject rentre en collision
	void OnTriggerEnter(Collider collider)
	{
		//Si c'est un zombie
		if (collider.gameObject.tag.Equals("Zombie")) 
		{
			//Si c'est la base du joueur, on lui enlève des pv et on reset le zombie
			if(!isTheEnemyBase)
				GameStats.Instance.Pv-=1;
			ResetGameobject(collider.gameObject);
			GameStats.Instance.Exp += 1;
		}else{
			//Si c'est un survivant
			if(collider.gameObject.tag.Equals("Survivor")){
				//Si c'est la base du joueur, on incrémente la population et on reset le zombie
				if(!isTheEnemyBase)
					GameStats.Instance.Population+=1;
				ResetGameobject(collider.gameObject);
			}
		}

		//Si un joueur n'a plus de point de vie, il déclanche le gameOver
		if(!isTheEnemyBase){
			if (GameStats.Instance.Pv <= 0) {Debug.Log("GameOver");
				if (Network.isClient) {
					networkView.RPC ("GameOverFunction", RPCMode.AllBuffered);
					networkView.RPC ("SendEndGameExp", RPCMode.AllBuffered);
				}
			}
		}
	}
	//Tous les clients qui recoivent le gameOver renvoyent leurs stats
	[RPC]
	void GameOverFunction(){
		if (Network.isClient) {
			Debug.Log("send");
			networkView.RPC ("ReturnGameStats", RPCMode.Others, Network.player, GameStats.Instance.Pv, GameStats.Instance.Population);
			//Application.LoadLevel(1);
		}
	}
	//Gestion du gameOver par les clients: affichage des stats
	[RPC]
	void ReturnGameStats(NetworkPlayer player, int pv, int pop){
		if (Network.isClient) {
			if (player == _STATICS._networkPlayer [1]) {
				Debug.Log("receive");
				scores.text = "Score joueur 1 (vous) : PV = " + GameStats.Instance.Pv + " et POP = " + GameStats.Instance.Population + "\nScore joueur 2 : PV = " + pv + " et POP = " + pop;
			}
			if (player == _STATICS._networkPlayer [0]) {
				Debug.Log("receive");
				scores.text = "Score joueur 1 : PV = " + pv + " et POP = " + pop + "\nScore joueur 2 (vous) : PV = " + GameStats.Instance.Pv + " et POP = " + GameStats.Instance.Population;
			}
		}
	}

	[RPC]
	void SendEndGameExp(){
		if (Network.isClient) {
			if (Network.player == _STATICS._networkPlayer [0])
				networkView.RPC ("EndGameExp", RPCMode.Server, _STATICS._playersInGame[0], GameStats.Instance.Exp);
			if (Network.player == _STATICS._networkPlayer [1])
				networkView.RPC ("EndGameExp", RPCMode.Server, _STATICS._playersInGame[1], GameStats.Instance.Exp);
		}
	}
	[RPC]
	void EndGameExp(string playerLogin, int xp){
		if(PlayerPrefs.HasKey(playerLogin+"XP")){
			int nxp = xp + PlayerPrefs.GetInt (playerLogin+"XP");
			PlayerPrefs.SetInt (playerLogin+"XP", nxp);
		}else
			PlayerPrefs.SetInt (playerLogin+"XP", xp);
	}

	void Update(){
		ressourcesMatText.text = "Mat : " + GameStats.Instance.RessourcesMat.ToString();
		ressourcesWeapText.text = "Weap : " + GameStats.Instance.RessourcesWeap.ToString();
		pvText.text = "PV : " + GameStats.Instance.Pv.ToString();
		goldText.text = "Or : " + GameStats.Instance.Gold.ToString ();
		SpawnZombie ();
	}

	//Gestions des vagues de zombies
	void SpawnZombie(){
		if (phase <=2){
			if(phase == 1){
				//if (EnCours < Zombies.Length-1 && _phasesManager.vtime <= 0.1)
				if (_phasesManager.startAction) {
//					if (Zombies [EnCours] == null) {
//						EnCours++;
//						Zombies [EnCours].SetActive (true);
//					}
					if(iz1<nbzombies1){
						if (timez1 > 1.5) {
							timez1 -= Time.deltaTime;

							//vtime = (int)vtime;
						}else{
							stock.zombiesType1[iz1].SetActive(true);
							timez1 = 3;
							iz1++;
						}
					}
					if(iz2<nbzombies2){
						if (timez2 > 0.5) {
							timez2 -= Time.deltaTime;
							
							//vtime = (int)vtime;
						}else{
							stock.zombiesType2[iz2].SetActive(true);
							timez2 = 3;
							iz2++;
						}
					}
					if(iz3<nbzombies3){
						if (timez3 > 0.25) {
							timez3 -= Time.deltaTime;
							
							//vtime = (int)vtime;
						}else{
							stock.zombiesType3[iz3].SetActive(true);
							timez3 = 3;
							iz3++;
						}
					}
					if(is1<nbsurvivor){
						if (times > 2.8) {
							times -= Time.deltaTime;
							
							//vtime = (int)vtime;
						}else{
							stock.SurvivorsType1[is1].SetActive(true);
							times = 3;
							is1++;
						}
					}
				}else{
					if(_phasesManager.switchPhase == true){
						iz1=0;
						iz2=0;
						iz3=0;
						is1=0;
						foreach(GameObject zombie in stock.zombiesType1){
							ResetGameobject(zombie.gameObject);
						}
						foreach(GameObject zombie in stock.zombiesType2){
							ResetGameobject(zombie.gameObject);
						}
						foreach(GameObject zombie in stock.zombiesType3){
							ResetGameobject(zombie.gameObject);
						}
						foreach(GameObject surivor in stock.SurvivorsType1){
							ResetGameobject(surivor.gameObject);
						}
						phase ++;
						nbzombies1+=2;
						nbzombies2+=1;
					}
				}
			}
			if(phase == 2){
				phase++;
			}
		}else
			phase = 1;
	}

	void ResetGameobject(GameObject something){
		if (something.tag == "Zombie")
			StartCoroutine(something.GetComponent<ZombieScript>().Reset());
		if (something.tag == "Survivor")
			StartCoroutine(something.GetComponent<SurvivorScript>().Reset());
	}

}

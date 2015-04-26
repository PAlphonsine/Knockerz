using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BaseScript : MonoBehaviour 
{
	// Stock de Zombies
	[SerializeField] StockZombies stock;
	// Numéro de la phase dans les courbes de gestion du nombre de Zombies
	int phaseZ1 = 0;
	int phaseZ2 = 0;
	int phaseZ3 = 0;
	// Temps entre chaque apparition de Zombie (en secondes)
	float timeZ1 = 3;
	float timeZ2 = 3;
	float timeZ3 = 3;
	// Temps entre chaque apparition de Survivant (en secondes)
	float timeS = 4;
	// Courbes d'évolution du nombre de Zombies de chaque type
	[SerializeField] AnimationCurve nbzombies1Change;
	[SerializeField] AnimationCurve nbzombies2Change;
	[SerializeField] AnimationCurve nbzombies3Change;
	// Nombre de Zombies de chaque type au départ
	public int nbzombies1Start = 2;
	public int nbzombies2Start = 0;
	public int nbzombies3Start = 0;
	// Nombre de Zombies de chaque type
	int nbzombies1 = 2;
	int nbzombies2 = 0;
	int nbzombies3 = 0;
	// Nombre de Survivant
	[SerializeField] int nbsurvivor = 0;
	// Compteur de parcours du tableau de Zombies pour chaque type de Zombie
	private int iz1 = 0;
	private int iz2 = 0;
	private int iz3 = 0;
	// Compteur de parcours du tableau de Survivant
	private int is1 = 0;
	// Gestion des phases
	[SerializeField] PhasesManager _phasesManager;
	// Texte des matériaux
	[SerializeField] Text ressourcesMatText;
	// Texte des armes
	[SerializeField] Text ressourcesWeapText;
	// Texte de l'or
	[SerializeField] Text goldText;
	// Texte des points de vie (population civile)
	[SerializeField] Text pvText;
	// Objet score
	[SerializeField] GameObject scores;
	// Texte des scoresText
	[SerializeField] Text scoresText;
	// Booléen de controle pour vérifier si la base est celle du joueur ou de l'aversaire
	[SerializeField] bool isTheEnemyBase = false;
	// Script du menu ingame
	[SerializeField] InGameMenuButtonScript _inGameMenuButtonScript;
	// Text d'affichage de la population
	[SerializeField] Text popText;
	// Login des joueurs
	[SerializeField] Text[] LoginsTexts;
	// Animation pour le gagnant
	[SerializeField] GameObject winnerObject;
	// Animation pour le perdant
	[SerializeField] GameObject loserObject;
	// Zombies et surivants activés en secours
	List<GameObject> zombiesAndSurvivorsSupp;
	
	void Start()
	{
		// Le nombre de Zombies de chaque type est égal au nombre de Zombies définit par défaut au départ
		nbzombies1 = nbzombies1Start;
		nbzombies2 = nbzombies2Start;
		nbzombies3 = nbzombies3Start;
		zombiesAndSurvivorsSupp = new List<GameObject>();
	}
	
	void Update ()
	{
		// Le montant des matériaux, armes , pop et or est constamment mis à jour
		ressourcesMatText.text = GameStats.Instance.RessourcesMat.ToString();
		ressourcesWeapText.text = GameStats.Instance.RessourcesWeap.ToString();
		popText.text = GameStats.Instance.Population.ToString();
		goldText.text = GameStats.Instance.Gold.ToString ();
		// Le montant des points de vie également
		pvText.text = GameStats.Instance.Pv.ToString();
		// Les Zombies spawnent
		SpawnZombie ();
		// Si la partie à commencée, le client font remonter leurs statistiques
		if(_phasesManager.startgame && Network.isClient)
			StartCoroutine (SendStats ());
	}

	// Envoie des stats au server
	IEnumerator SendStats(){
		// Les joueurs envoient les infos du gamestats au server
		networkView.RPC ("ReceiveStats", RPCMode.Server, Network.player, GameStats.Instance.Pv, GameStats.Instance.RessourcesMat, GameStats.Instance.RessourcesWeap, GameStats.Instance.Gold, GameStats.Instance.Population, GameStats.Instance.Exp);
		// Toutes les secondes
		yield return new WaitForSeconds (1f);
	}

	// Le server reçoit les infos
	[RPC]
	void ReceiveStats(NetworkPlayer player, int pv, int rm, int rw, int gold, int pop, int xp){
		// On fonction du joueur qui les lui a envoyés
		if (player == _STATICS._networkPlayer [0]) {
			// Il stocke les stats
			_STATICS._statsJ1 [0] = pv;
			_STATICS._statsJ1 [1] = rm;
			_STATICS._statsJ1 [2] = rw;
			_STATICS._statsJ1 [3] = gold;
			_STATICS._statsJ1 [4] = pop;
			_STATICS._statsJ1 [5] = xp;
		} else {
			_STATICS._statsJ2[0]=pv;
			_STATICS._statsJ2[1]=rm;
			_STATICS._statsJ2[2]=rw;
			_STATICS._statsJ2[3]=gold;
			_STATICS._statsJ2[4]=pop;
			_STATICS._statsJ2[5]=xp;
		}
	}
	
	// Quand un gameobject rentre en collision avec la base
	void OnTriggerEnter (Collider collider)
	{
		// Si c'est un Zombie
		if (collider.gameObject.tag.Equals("Zombie")) 
		{
			// Si c'est la base du joueur, on lui enlève des points de vie, ...
			if (!isTheEnemyBase)
				GameStats.Instance.Pv -= 1;
			// ... on reset le Zombie
			ResetGameobject(collider.gameObject);
		}
		else
		{
			// Si c'est un Survivant
			if (collider.gameObject.tag.Equals("Survivor"))
			{
				// Si c'est la base du joueur, on incrémente la population ...
				if (!isTheEnemyBase)
					GameStats.Instance.Population += 1;
				// ... et on reset le Zombie
				ResetGameobject(collider.gameObject);
			}
		}
		
		// Si un joueur n'a plus de point de vie ...
		if (!isTheEnemyBase)
		{
			// ... il déclenche le GameOver
			if (GameStats.Instance.Pv <= 0)
			{
				if (Network.isClient)
				{
					if (_phasesManager.startgame)
					{
						// On demande d'envoyer les données de gameOver pour tout le monde
						networkView.RPC ("GameOverFunction", RPCMode.AllBuffered);
						// Chaque joueur gagne l'expérience enmagazinée durant la partie
						networkView.RPC ("SendEndGameExp", RPCMode.AllBuffered);
					}
				}
			}
		}
	}
	
	// Tous les clients qui recoivent le GameOver renvoient leurs stats au serveur
	[RPC]
	void GameOverFunction ()
	{
		if (Network.isClient)
		{
			networkView.RPC ("ReturnGameStats", RPCMode.Others, Network.player, GameStats.Instance.Pv, GameStats.Instance.Population, GameStats.Instance.Exp);
		}
		// Arret du jeu
		_phasesManager.startgame = false;
	}
	
	// Gestion du GameOver par les clients : affichage des stats
	[RPC]
	void ReturnGameStats (NetworkPlayer player, int pv, int pop, int xp)
	{
		if (Network.isClient)
		{
			// Affichage des scoresText en fonction des joueurs
			if (player == _STATICS._networkPlayer[1])
			{
				// Activation du panel score
				scores.SetActive(true);
				// Affichage de différents paramètres
				scoresText.text = "Scores " + LoginsTexts[0].text + " (vous)\nPV: " + GameStats.Instance.Pv + "| POP: " + GameStats.Instance.Population + "| XP: " + GameStats.Instance.Exp + "\n\nScores " + LoginsTexts[1].text + "\nPV: " + pv + "| POP: " + pop + "| XP: " + xp;
				// Affichage de l'animation de fin de partie en fonction du résultat
				if(pv<=0)
					winnerObject.SetActive(true);
				else
					loserObject.SetActive(true);
				// Arret du jeu
				_phasesManager.startgame = false;
			}
			if (player == _STATICS._networkPlayer[0])
			{
				// Activation du panel score
				scores.SetActive(true);
				// Affichage de différents paramètres
				scoresText.text = "Scores " + LoginsTexts[0].text + "\nPV: " + pv + " | POP: " + pop + "| XP: " + xp + "\n\nScores " + LoginsTexts[1].text + " (vous)\nPV: " + GameStats.Instance.Pv + " | POP: " + GameStats.Instance.Population + "| XP: " + GameStats.Instance.Exp;
				// Affichage de l'animation de fin de partie en fonction du résultat
				if(pv<=0)
					winnerObject.SetActive(true);
				else
					loserObject.SetActive(true);
				// Arret du jeu
				_phasesManager.startgame = false;
			}
		}
	}
	
	// Gestion de la distribution d'expérience
	[RPC]
	void SendEndGameExp ()
	{
		if (Network.isClient)
		{
			// Envoi de l'expérience au serveur en fonction du joueur
			if (Network.player == _STATICS._networkPlayer[0])
				networkView.RPC ("EndGameExp", RPCMode.Server, _STATICS._playersInGame[0], GameStats.Instance.Exp);
			if (Network.player == _STATICS._networkPlayer[1])
				networkView.RPC ("EndGameExp", RPCMode.Server, _STATICS._playersInGame[1], GameStats.Instance.Exp);
		}
	}
	
	// Gestion du calcul et de la sauvegarde de l'expérience
	[RPC]
	void EndGameExp (string playerLogin, int xp)
	{
		if (PlayerPrefs.HasKey(playerLogin + "XP"))
		{
			int nxp = xp + PlayerPrefs.GetInt (playerLogin + "XP");
			PlayerPrefs.SetInt (playerLogin + "XP", nxp);
		}
		else
			PlayerPrefs.SetInt (playerLogin + "XP", xp);
	}
	
	// Gestion des vagues de Zombies
	void SpawnZombie ()
	{
		//if (EnCours < Zombies.Length-1 && _phasesManager.vtime <= 0.1)
		// Si l'on est en phase d'action
		if (_phasesManager.startAction)
		{
			// Si le compteur de Zombies de type 1 n'est pas à son maximum
			if (iz1 < nbzombies1)
			{
				// Si le temps entre chaque Zombie de type 1 est supérieur à 1.5 ...
				if (timeZ1 > 0)
				{
					// ... on le baisse
					timeZ1 -= Time.deltaTime;
					//vtime = (int)vtime;
				}
				else
				{
					// Sinon, on active le Zombie à la position du compteur, ...
					if(!stock.zombiesType1[iz1].activeSelf)
						stock.zombiesType1[iz1].SetActive(true);
					else{
						// Ou on cherche un Zombie desactivé
						foreach(GameObject go in stock.zombiesType1)
						{
							if(!go.activeSelf){
								go.SetActive(true);
								// Que l'on ajoute dans la liste des elements supplémentaires
								zombiesAndSurvivorsSupp.Add(go);
								break;
							}
						}
					}
					// ... on réinitialise le temps entre chaque Zombie ...
					timeZ1 = 2;
					// ... et on incrémente le compteur
					iz1++;
				}
			}
			// Si le compteur de Zombies de type 2 n'est pas à son maximum
			if (iz2 < nbzombies2)
			{
				// Si le temps entre chaque Zombie de type 2 est supérieur à 0.5 ...
				if (timeZ2 > 0)
				{
					// ... on le baisse
					timeZ2 -= Time.deltaTime;
					//vtime = (int)vtime;
				}
				else
				{
					// Sinon, on active le Zombie à la position du compteur, ...
					if(!stock.zombiesType2[iz2].activeSelf)
						stock.zombiesType2[iz2].SetActive(true);
					else{
						// Ou on cherche un Zombie desactivé
						foreach(GameObject go in stock.zombiesType2)
						{
							if(!go.activeSelf){
								go.SetActive(true);
								// Que l'on ajoute dans la liste des elements supplémentaires
								zombiesAndSurvivorsSupp.Add(go);
								break;
							}
						}
					}
					// ... on réinitialise le temps entre chaque Zombie ...
					timeZ2 = 3;
					// ... et on incrémente le compteur
					iz2++;
				}
			}
			// Si le compteur de Zombies de type 3 n'est pas à son maximum
			if (iz3 < nbzombies3)
			{
				// Si le temps entre chaque Zombie de type 3 est supérieur à 0.25 ...
				if (timeZ3 > 0)
				{
					// ... on le baisse
					timeZ3 -= Time.deltaTime;
					//vtime = (int)vtime;
				}
				else
				{
					// Sinon, on active le Zombie à la position du compteur, ...
					if(!stock.zombiesType3[iz3].activeSelf)
						stock.zombiesType3[iz3].SetActive(true);
					else{
						// Ou on cherche un Zombie desactivé
						foreach(GameObject go in stock.zombiesType3)
						{
							if(!go.activeSelf){
								go.SetActive(true);
								// Que l'on ajoute dans la liste des elements supplémentaires
								zombiesAndSurvivorsSupp.Add(go);
								break;
							}
						}
					}
					// ... on réinitialise le temps entre chaque Zombie ...
					timeZ3 = 4;
					// ... et on incrémente le compteur
					iz3++;
				}
			}
			// Si le compteur de Survivants n'est pas à son maximum
			if (is1 < nbsurvivor)
			{
				// Si le temps entre chaque Survivant est supérieur à 2.8 ...
				if (timeS > 0)
				{
					// ... on le baisse
					timeS -= Time.deltaTime;
					//vtime = (int)vtime;
				}
				else
				{
					// Sinon, on active le Survivant à la position du compteur, ...
					if(!stock.SurvivorsType1[is1].activeSelf)
						stock.SurvivorsType1[is1].SetActive(true);
					else{
						// Ou on cherche un Zombie desactivé
						foreach(GameObject go in stock.SurvivorsType1)
						{
							if(!go.activeSelf){
								go.SetActive(true);
								// Que l'on ajoute dans la liste des elements supplémentaires
								zombiesAndSurvivorsSupp.Add(go);
								break;
							}
						}
					}
					// ... on réinitialise le temps entre chaque Survivant ...
					timeS = 4;
					// ... et on incrémente le compteur
					is1++;
				}
			}
		}
		// Sinon, si l'on est en phase de réflexion
		else
		{
			// Si le changement de phase à bien été opéré
			if (_phasesManager.switchPhase == true)
			{
				if(!isTheEnemyBase)
					// Accroissement par défaut de la population à chaque tour
					GameStats.Instance.Population += 5;

				// Pour chaque Zombie dans le tableau de Zombies de type 1 ...
				for(int i=0;i<iz1;i++)
				{
					// ... on reset le Zombie
					ResetGameobject(stock.zombiesType1[i].gameObject);
				}
				// Pour chaque Zombie dans le tableau de Zombies de type 2 ...
				for(int i=0;i<iz2;i++)
				{
					// ... on reset le Zombie
					ResetGameobject(stock.zombiesType2[i].gameObject);
				}
				// Pour chaque Zombie dans le tableau de Zombies de type 3 ...
				for(int i=0;i<iz3;i++)
				{
					// ... on reset le Zombie
					ResetGameobject(stock.zombiesType3[i].gameObject);
				}
				// Pour chaque Survivant das le tableau de Survivants ...
				for(int i=0;i<is1;i++)
				{
					// ... on reset le Survivant
					ResetGameobject(stock.SurvivorsType1[i].gameObject);
				}
				foreach(GameObject go in zombiesAndSurvivorsSupp)
					ResetGameobject(go);
				zombiesAndSurvivorsSupp.Clear();

				// On réinitialise tous les compteurs des Zombies et des Survivants
				iz1 = 0;
				iz2 = 0;
				iz3 = 0;
				is1 = 0;
				
				// On incrémente le numéro de la phase pour les Zombies de type 1
				phaseZ1++;
				// Si l'on est arrivé au bout de la courbe d'évolution du nombre de Zombies de type 1 ...
				if (phaseZ1 >= nbzombies1Change.keys.Length)
				{
					// ... on réinitialise
					phaseZ1 = 0;
				}

				// On incrémente le numéro de la phase pour les Zombies de type 1
				phaseZ2++;
				// Si l'on est arrivé au bout de la courbe d'évolution du nombre de Zombies de type 2 ...
				if (phaseZ2 >= nbzombies2Change.keys.Length)
				{
					// ... on réinitialise
					phaseZ2 = 0;
				}
				
				// On incrémente le numéro de la phase pour les Zombies de type 1
				phaseZ3++;
				// Si l'on est arrivé au bout de la courbe d'évolution du nombre de Zombies de type 3 ...
				if (phaseZ3 >= nbzombies3Change.keys.Length)
				{
					// ... on réinitialise
					phaseZ3 = 0;
				}
				
				// Le nombre de Zombies est incrémenté par le nombre de Zombies correspondant au point de la courbe où l'on se trouve
				nbzombies1 += Mathf.RoundToInt(nbzombies1Change.keys[phaseZ1].value);
				//Random.Range()
				nbzombies2 += Mathf.RoundToInt(nbzombies2Change.keys[phaseZ2].value);
				nbzombies3 += Mathf.RoundToInt(nbzombies3Change.keys[phaseZ3].value);
				
				// Le nombre de Zombies de chaque type est limité entre 0 et 20
				nbzombies1 = Mathf.Clamp(nbzombies1, 0, 20);
				nbzombies2 = Mathf.Clamp(nbzombies2, 0, 20);
				nbzombies3 = Mathf.Clamp(nbzombies3, 0, 20);
			}
		}
	}

	// Méthode de réinitialisation des Zombies et des Survivants
	void ResetGameobject (GameObject something)
	{
		if (something.tag == "Zombie")
			StartCoroutine(something.GetComponent<ZombieScript>().Reset());
		if (something.tag == "Survivor")
			StartCoroutine(something.GetComponent<SurvivorScript>().Reset());
	}

	// Synchronisation des éléments du BaseScript
	void OnSerializeNetworkView (BitStream stream)
	{
		if (stream.isWriting && Network.isServer) {
			stream.Serialize (ref  phaseZ1);
			stream.Serialize (ref  phaseZ2);
			stream.Serialize (ref  phaseZ3);
			stream.Serialize (ref  nbzombies1);
			stream.Serialize (ref  nbzombies2);
			stream.Serialize (ref  nbzombies3);
			stream.Serialize (ref  iz1);
			stream.Serialize (ref  iz2);
			stream.Serialize (ref  iz3);
		} else if (stream.isReading && Network.isClient) {
			stream.Serialize (ref  phaseZ1);
			stream.Serialize (ref  phaseZ2);
			stream.Serialize (ref  phaseZ3);
			stream.Serialize (ref  nbzombies1);
			stream.Serialize (ref  nbzombies2);
			stream.Serialize (ref  nbzombies3);
			stream.Serialize (ref  iz1);
			stream.Serialize (ref  iz2);
			stream.Serialize (ref  iz3);
		}
	}
	
	public bool IsTheEnemyBase {
		get {
			return isTheEnemyBase;
		}
		set {
			isTheEnemyBase = value;
		}
	}
}
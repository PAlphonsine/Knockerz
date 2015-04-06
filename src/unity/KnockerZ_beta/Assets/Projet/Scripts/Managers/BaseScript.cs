using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BaseScript : MonoBehaviour 
{
	// Stock de Zombies
	[SerializeField]
	StockZombies stock;
	// Numéro de la phase dans les courbes de gestion du nombre de Zombies
	int phaseZ1 = 0;
	int phaseZ2 = 0;
	int phaseZ3 = 0;
	// Temps entre chaque apparition de Zombie (en secondes)
	float timeZ1 = 3;
	float timeZ2 = 3;
	float timeZ3 = 3;
	// Temps entre chaque apparition de Survivant (en secondes)
	float timeS = 3;
	// Courbes d'évolution du nombre de Zombies de chaque type
	[SerializeField]
	AnimationCurve nbzombies1Change;
	[SerializeField]
	AnimationCurve nbzombies2Change;
	[SerializeField]
	AnimationCurve nbzombies3Change;
	// Nombre de Zombies de chaque type au départ
	public int nbzombies1Start = 2;
	public int nbzombies2Start = 0;
	public int nbzombies3Start = 0;
	// Nombre de Zombies de chaque type
	int nbzombies1 = 2;
	int nbzombies2 = 0;
	int nbzombies3 = 0;
	// Nombre de Survivant
	[SerializeField]
	int nbsurvivor = 0;
	// Compteur de parcours du tableau de Zombies pour chaque type de Zombie
	private int iz1 = 0;
	private int iz2 = 0;
	private int iz3 = 0;
	// Compteur de parcours du tableau de Survivant
	private int is1 = 0;
	// Gestion des phases
	[SerializeField]
	PhasesManager _phasesManager;
	// Texte des matériaux
	[SerializeField]
	Text ressourcesMatText;
	// Texte des armes
	[SerializeField]
	Text ressourcesWeapText;
	// Texte de l'or
	[SerializeField]
	Text goldText;
	// Texte des points de vie (population civile)
	[SerializeField]
	Text pvText;
	// Objet score
	[SerializeField]
	GameObject scores;
	// Texte des scoresText
	[SerializeField]
	Text scoresText;
	// Booléen de controle pour vérifier si la base est celle du joueur ou de l'aversaire
	[SerializeField]
	bool isTheEnemyBase = false;
	// Script du menu ingame
	[SerializeField]
	InGameMenuButtonScript _inGameMenuButtonScript;
	// Text d'affichage de la population
	[SerializeField]
	Text popText;
	// Login des joueurs
	[SerializeField]
	Text[] LoginsTexts;
	// Animation pour le gagnant
	[SerializeField]
	GameObject winnerObject;
	// Animation pour le perdant
	[SerializeField]
	GameObject loserObject;
	
	void Start()
	{
		// Le nombre de Zombies de chaque type est égal au nombre de Zombies définit par défaut au départ
		nbzombies1 = nbzombies1Start;
		nbzombies2 = nbzombies2Start;
		nbzombies3 = nbzombies3Start;
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
	}
	
	// Quand un gameobject rentre en collision avec la base
	void OnTriggerEnter (Collider collider)
	{
		// Si c'est un Zombie
		if (collider.gameObject.tag.Equals("Zombie")) 
		{
			// Si c'est la base du joueur, on lui enlève des points de vie, ...
			if (!isTheEnemyBase)
				GameStats.Instance.Pv-=1;
			// ... on reset le Zombie ...
			ResetGameobject(collider.gameObject);
			// ... et le joueur gagne de l'expérience
			GameStats.Instance.Exp += 1;
		}
		else
		{
			// Si c'est un Survivant
			if (collider.gameObject.tag.Equals("Survivor"))
			{
				// Si c'est la base du joueur, on incrémente la population ...
				if (!isTheEnemyBase)
					GameStats.Instance.Population+=1;
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
						// Le jeu s'arrete pour tout le monde
						_inGameMenuButtonScript.PauseAll();
					}
					// Le GameOver est définitif
					networkView.RPC ("GameOverFunction", RPCMode.AllBuffered);
					// Chaque joueur gagne l'expérience enmagazinée durant la partie
					networkView.RPC ("SendEndGameExp", RPCMode.AllBuffered);
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
			//Application.LoadLevel(1);
		}
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
				scoresText.text = "scoresText " + LoginsTexts[0] + " (vous)      PV: " + GameStats.Instance.Pv + "| POP: " + GameStats.Instance.Population + "| XP: " + GameStats.Instance.Exp + "\nscoresText " + LoginsTexts[1] + "      PV: " + pv + "| POP: " + pop + "| XP: " + xp;
				// Pause de la partie
				Time.timeScale = 0.0f;
				// Arret du jeu
				_phasesManager.startgame = false;
				// Affichage de l'animation de fin de partie en fonction du résultat
				if(pv>0)
					winnerObject.SetActive(true);
				else
					loserObject.SetActive(false);
			}
			if (player == _STATICS._networkPlayer[0])
			{
				// Activation du panel score
				scores.SetActive(true);
				// Affichage de différents paramètres
				scoresText.text = "scoresText " + LoginsTexts[0] + "      PV: " + pv + " et POP: " + pop + "| XP: " + xp + "\nscoresText joueur 2 (vous)      PV: " + GameStats.Instance.Pv + " et POP: " + LoginsTexts[1] + "| XP: " + GameStats.Instance.Exp;
				// Pause de la partie
				Time.timeScale = 0.0f;
				// Arret du jeu
				_phasesManager.startgame = false;
				// Affichage de l'animation de fin de partie en fonction du résultat
				if(pv>0)
					winnerObject.SetActive(true);
				else
					loserObject.SetActive(false);
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
					stock.zombiesType1[iz1].SetActive(true);
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
					stock.zombiesType2[iz2].SetActive(true);
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
					stock.zombiesType3[iz3].SetActive(true);
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
					stock.SurvivorsType1[is1].SetActive(true);
					// ... on réinitialise le temps entre chaque Survivant ...
					timeS = 3;
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

				// On réinitialise tous les compteurs des Zombies et des Survivants
				iz1 = 0;
				iz2 = 0;
				iz3 = 0;
				is1 = 0;
				
				// Pour chaque Zombie dans le tableau de Zombies de type 1 ...
				foreach(GameObject zombie in stock.zombiesType1)
				{
					// ... on reset le Zombie
					ResetGameobject(zombie.gameObject);
				}
				// Pour chaque Zombie dans le tableau de Zombies de type 2 ...
				foreach(GameObject zombie in stock.zombiesType2)
				{
					// ... on reset le Zombie
					ResetGameobject(zombie.gameObject);
				}
				// Pour chaque Zombie dans le tableau de Zombies de type 3 ...
				foreach(GameObject zombie in stock.zombiesType3)
				{
					// ... on reset le Zombie
					ResetGameobject(zombie.gameObject);
				}
				// Pour chaque Survivant das le tableau de Survivants ...
				foreach(GameObject surivor in stock.SurvivorsType1)
				{
					// ... on reset le Survivant
					ResetGameobject(surivor.gameObject);
				}
				
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
	
	public bool IsTheEnemyBase {
		get {
			return isTheEnemyBase;
		}
		set {
			isTheEnemyBase = value;
		}
	}
}
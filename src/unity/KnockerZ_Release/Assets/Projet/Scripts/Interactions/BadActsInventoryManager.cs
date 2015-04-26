using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BadActsInventoryManager : MonoBehaviour
{
	// Gestion des achats de coup(s) fourré(s)
	[SerializeField] BadActsShopManager badActsShopManager;
	// Nom, image, description et caractéristiques des coups fourrés
	[SerializeField] Text nameText;
	[SerializeField] Text descriptionText;
	[SerializeField] Text featuresText;
	// Gestion des phases
	[SerializeField] PhasesManager phasesManager;
	// Gestion du stock des Zombies
	[SerializeField] StockZombies stockZombiesJ1;
	[SerializeField] StockZombies stockZombiesJ2;
	// Fenetre de l'inventaire
	[SerializeField] GameObject inventoryPanel;
	// Textes et boutons des coups fourrés
	[SerializeField] Button fogButton;
	[SerializeField] Text fogButtonText;
	[SerializeField] Button zombieBaitButton;
	[SerializeField] Text zombieBaitButtonText;
	// Grenades fumigènes
	[SerializeField] GameObject[] smokeGrenades;
	// Nombre de coups fourrés dans l'inventaire
	// Grenades fumigènes
	private int fogsNumber;
	// Appats pour Zombies
	private int zombieBaitsNumber;
	// Tableau des Zombies aléatoires envoyés à l'adversaire
	private List<ZombieScript> zombiesBaited;
	// Tableau des Zombies aléatoires sur le terrain
	private List<ZombieScript> zombiesInGame;
	// Booléen de controle de lancement de grenades fumigènes
	private bool smokingGround;
	// Tag de l'object sur lequel on applique le soutien (pour l'instant "PathJ2", soit le chemin des zombies de l'adversaire)
	private string objectTag;
	// Type du coup fourré sélectionné
	private string badActType;
	// Portée du RayCast
	private float limitDetection = 250.0f;
	// Point d'impact du Raycast
	RaycastHit hit;
	// Randoms correspondant aux types de zombies
	private int randomType1;
	private int randomType2;
	private int randomType3;
	private int randomType4;
	private int randomType5;
	// Pour lancer une fois le reset
	private bool wasReset;

	// Compteur de Zombies de chaque type pour chaque joueur
	int nbZt1J1 = 1;
	int nbZt2J1 = 1;
	int nbZt3J1 = 1;
	int nbZt1J2 = 1;
	int nbZt2J2 = 1;
	int nbZt3J2 = 1;

	void Start ()
	{
		// Rien n'a été envoyé à l'adversaire dès le début
		this.fogsNumber = 0;
		this.zombieBaitsNumber = 0;
		this.zombiesBaited = new List<ZombieScript> ();
		this.zombiesInGame = new List<ZombieScript> ();
		this.smokingGround = false;
		this.badActType = "";
		this.wasReset = false;
	}
	
	void FixedUpdate ()
	{
		// Si la partie a commencé
		if (this.phasesManager.startgame == true)
		{
			// Si le panel de l'inventaire est actif
			if (this.inventoryPanel.activeSelf == true)
			{
				// Les textes des boutons deviennent le nombre de chaque coup fourré disponible
				this.fogButtonText.text = this.fogsNumber.ToString();
				this.zombieBaitButtonText.text = this.zombieBaitsNumber.ToString();
			}
			
			// Si l'on est en phase de réflexion
			if (this.phasesManager.startAction == false)
			{
				// Si l'on a pas encore reset les variables
				if(!wasReset)
				{
					// On ne peut pas lancer de coups fourrés, les boutons sont donc désactivés
					this.fogButton.interactable = false;
					this.zombieBaitButton.interactable = false;
					
					// Si les grenades fumigènes lancées à la phase précédente font encore effet
					if (this.smokingGround == true)
					{
						// Pour chaque grenades du tableau de grenades fumigènes, ...
						foreach (GameObject grenade in this.smokeGrenades)
						{
							// ... si la grenade est active, ...
							if (grenade.activeSelf == true)
							{
								// ... elle cesse d'emettre de la fumée
								grenade.GetComponentInChildren<ParticleSystem>().Stop();
							}
						}
						// On dissipe le nuage de fumée
						StartCoroutine(this.Unsmoke());
						// Il n'y a plus de fumée
						this.smokingGround = false;
					}
					
					// Les compteurs restent tels qu'ils ont été initialisés
					this.nbZt1J1 = 1;
					this.nbZt2J1 = 1;
					this.nbZt3J1 = 1;
					this.nbZt1J2 = 1;
					this.nbZt2J2 = 1;
					this.nbZt3J2 = 1;

					// Pour tous les zombies envoyés avec des appats, on les reset
					foreach(ZombieScript zs in zombiesInGame)
					{
						if(zs.gameObject.activeSelf)
							StartCoroutine(zs.Reset());
					}
					// On vide le tableau
					zombiesInGame.Clear();
					// On desactive l'interupteur
					wasReset = true;
				}
			}
			// Sinon, si l'on est en phase d'action
			else
			{
				// On permet de reset les variables
				wasReset = false;
				// Si le joueur n'a pas acheté de grenades fumigènes
				if (this.fogsNumber <= 0)
					// On ne peut en utiliser
					this.fogButton.interactable = false;
				else
					//Sinon, on peut en utiliser
					this.fogButton.interactable = true;
				
				// Si le joueur n'a pas acheté d'appats pour Zombies
				if (this.zombieBaitsNumber <= 0)
					// On ne peut en utiliser
					this.zombieBaitButton.interactable = false;
				else
					// Sinon, on peut en utiliser
					this.zombieBaitButton.interactable = true;
				
				// Si le mode de coup fourré est le mode "Brouillard"
				if (this.badActType == "Fog")
				{
					// Lors du clic du joueur ...
					if(Input.GetMouseButtonDown(0))
					{
						// ... on trace un rayon partant de la caméra à la position de la souris
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						// On initialise le point d'impact
						RaycastHit hit;
						// Si le rayon touche un chemin
						if (Physics.Raycast(ray, out hit, limitDetection) && hit.transform.CompareTag(this.objectTag))
						{
							networkView.RPC("PutSmokeGrenade", RPCMode.AllBuffered, hit.point.x, hit.point.y, hit.point.z);
							// Le nombre de grenades restantes est diminué
							this.fogsNumber--;
						}
						// Le mode de coup fourré est réinitialisé
						this.badActType = "";
					}
				}
				
				// Si le mode de coup fourré est le mode "Appat pour Zombies"
				if (this.badActType == "ZombieBait")
				{
					// Lors du clic du joueur ...
					if(Input.GetMouseButtonDown(0))
					{
						// ... on trace un rayon partant de la caméra à la position de la souris
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						// Si le rayon touche un chemin
						if(Physics.Raycast(ray, out hit, limitDetection) && hit.transform.CompareTag(this.objectTag))
						{
							this.randomType1 = Random.Range(1, 4);
							this.randomType2 = Random.Range(1, 4);
							this.randomType3 = Random.Range(1, 4);
							this.randomType4 = Random.Range(1, 4);
							this.randomType5 = Random.Range(1, 4);
							networkView.RPC("DefineZombiesType", RPCMode.AllBuffered, Network.player, this.randomType1, this.randomType2, this.randomType3, this.randomType4, this.randomType5); 
							//networkView.RPC("ActivateSentZombies", RPCMode.AllBuffered);
							this.zombieBaitsNumber--;
						}
						// Le mode de coup fourré est réinitialisé
						this.badActType = "";
					}
				}
			}
		}
	}
	
	// Méthode de définition du coup fourré sélectionné dans la boutique
	public void SetBadActType(string type)
	{
		// Si le joueur clique sur la grenade fumigène
		if (type == "Fog")
		{
			// On change le type de coup fourré sélectionné
			this.badActType = type;
			// On change toute la partie description du coup fourré
			this.nameText.text = "Grenades Fumigènes";
			//this.descriptionText.text = "Vos survivants lancent des grenades fumigènes dans la base adverse pour l'empecher de voir une partie de sa carte. En espérant qu'il ait une bonne audition ...";
			//this.featuresText.text = "Durée : 30 secondes";
			this.descriptionText.text = "Vos survivants lancent une grenade fumigène dans la base adverse.";
			this.featuresText.text = "Cache une partie de la carte de l'adversaire pendant 20 secondes";
		}
		// Si le joueur clique sur l'appat pour Zombie
		if (type == "ZombieBait")
		{
			// On change le type de coup fourré sélectionné
			this.badActType = type;
			// On change toute la partie description du coup fourré
			this.nameText.text = "Appats pour zombies";
			//this.descriptionText.text = "Les appats pour zombies de la marque KillerZ ! A utiliser en cas d'apocalypse zombies, ne pas laisser à la portée des enfants (sauf des enfants-zombies).";
			this.descriptionText.text = "Vos survivants placent des appats pour Zombies dans la base adverse.";
			this.featuresText.text = "Envoie 5 zombies de type aléatoire à l'adversaire";
		}
	}

	// RPC d'envoi de grenade fumigène
	[RPC]
	void PutSmokeGrenade(float x, float y, float z)
	{
		// On parcourt le tableau de grenades fumigènes
		for (int i = 0; i < this.smokeGrenades.Length; i++)
		{
			// Si la grenade n'est pas activée
			if (this.smokeGrenades[i].gameObject.activeSelf == false)
			{
				// Une grenade fumigène est lancée à l'emplacement visé
				this.smokeGrenades[i].transform.position = new Vector3(x, y, z);
				// La grenade est activée
				this.smokeGrenades[i].gameObject.SetActive(true);
				// La fumée est activée
				this.smokingGround = true;
				// On arrete le parcours
				break;
			}
		}
	}

	// RPC de définition des Zombies envoyés
	[RPC]
	void DefineZombiesType (NetworkPlayer player, int zt1, int zt2, int zt3, int zt4, int zt5)
	{
		// On l'ordre selon chaque type de Zombie
		switch (zt1)
		{
			case 1:
				this.SendZombies(player, zt1);
				break;
			case 2:
				this.SendZombies(player, zt1);
				break;
			case 3:
				this.SendZombies(player, zt1);
				break;
			default:
				break;
		}

		switch (zt2)
		{
			case 1:
				this.SendZombies(player, zt2);
				break;
			case 2:
				this.SendZombies(player, zt2);
				break;
			case 3:
				this.SendZombies(player, zt2);
				break;
			default:
				break;
		}
		
		switch (zt3)
		{
			case 1:
				this.SendZombies(player, zt3);
				break;
			case 2:
				this.SendZombies(player, zt3);
				break;
			case 3:
				this.SendZombies(player, zt3);
				break;
			default:
				break;
		}
		
		switch (zt4)
		{
			case 1:
				this.SendZombies(player, zt4);
				break;
			case 2:
				this.SendZombies(player, zt4);
				break;
			case 3:
				this.SendZombies(player, zt4);
				break;
			default:
				break;
		}
		
		switch (zt5)
		{
			case 1:
				this.SendZombies(player, zt5);
				break;
			case 2:
				this.SendZombies(player, zt5);
				break;
			case 3:
				this.SendZombies(player, zt5);
				break;
			default:
				break;
		}
	}

	// Méthode d'envoi de Zombies
	public void SendZombies(NetworkPlayer player, int type)
	{
		// Booléen de controle de vérification de la position du joueur
		bool isFirstPlayer = true;

		// Si le joueur est le joueur 1
		if (player == _STATICS._networkPlayer[0])
		{
			// On le confirme comme joueur 2
			isFirstPlayer = true;
		}
		// Sinon, si le joueur est le joueur 2
		else if (player == _STATICS._networkPlayer[1])
		{
			// On ne le confirme pas comme joueur 1
			isFirstPlayer = false;
		}

		// Si le joueur est le premier joueur ainsi que celui qui envoie l'ordre ou celui qui le recoie
		if ((Network.player == player && isFirstPlayer == true) || (Network.player != player && isFirstPlayer == true))
		{
			// Si le Zombie est de type 1 ...
			if (type == 1)
			{
				// Si le Zombie défini n'est pas activé
				if (this.stockZombiesJ2.zombiesType1[this.stockZombiesJ2.zombiesType1.Length-nbZt1J1].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 1 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ2.zombiesType1[this.stockZombiesJ2.zombiesType1.Length-nbZt1J1].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 1 attiré
					this.nbZt1J1++;
				}
			}
			// Sinon, si le Zombie est de type 2 ...
			else if (type == 2)
			{
				// Si le Zombie défini n'est pas activé
				if (this.stockZombiesJ2.zombiesType2[this.stockZombiesJ2.zombiesType2.Length-nbZt2J1].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 2 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ2.zombiesType2[this.stockZombiesJ2.zombiesType2.Length-nbZt2J1].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 2 attiré
					this.nbZt2J1++;
				}
			}
			// Sinon, si le Zombie est de type 3 ...
			else if (type == 3)
			{
				// Si le Zombie défini n'est pas activé
				if (this.stockZombiesJ2.zombiesType3[this.stockZombiesJ2.zombiesType3.Length-nbZt3J1].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 3 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ2.zombiesType3[this.stockZombiesJ2.zombiesType3.Length-nbZt3J1].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 3 attiré
					this.nbZt3J1++;
				}
			}
		}
		// Sinon, si le joueur est le deuxième joueur et qu'il envoie ou reçoit l'ordre
		else
		{
			// Si le Zombie est de type 1 ...
			if (type == 1)
			{
				// Si le Zombie défini n'est pas activé
				if (this.stockZombiesJ1.zombiesType1[this.stockZombiesJ1.zombiesType1.Length-nbZt1J2].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 1 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ1.zombiesType1[this.stockZombiesJ1.zombiesType1.Length-nbZt1J2].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 1 attiré
					this.nbZt1J2++;
				}
			}
			// Sinon, si le Zombie est de type 2 ...
			else if (type == 2)
			{
				// Si le Zombie défini n'est pas activé
				if (this.stockZombiesJ1.zombiesType2[this.stockZombiesJ1.zombiesType2.Length-nbZt2J2].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 2 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ1.zombiesType2[this.stockZombiesJ1.zombiesType2.Length-nbZt2J2].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 2 attiré
					this.nbZt2J2++;
				}
			}
			// Sinon, si le Zombie est de type 3 ...
			else if (type == 3)
			{
				// Si le Zombie défini n'est pas activé
				if (this.stockZombiesJ1.zombiesType3[this.stockZombiesJ1.zombiesType3.Length-nbZt3J2].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 3 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ1.zombiesType3[this.stockZombiesJ1.zombiesType3.Length-nbZt3J2].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 3 attiré
					this.nbZt3J2++;
				}
			}
		}
		// On active les Zombies envoyés
		ActivateSentZombies ();
	}

	// Méthode d'activation des Zombies envoyés
	public void ActivateSentZombies()
	{
		// Pour chaque Zombie attiré
		foreach (ZombieScript zs in this.zombiesBaited)
		{
			// On active le Zombie
			zs.gameObject.SetActive(true);
			// On le place dans la liste des zombies activés
			zombiesInGame.Add(zs);
		}
		// On vide la liste des Zombies envoyés
		this.zombiesBaited.Clear ();
	}
	
	// Fonction Coroutine de dissipation de la fumée
	public IEnumerator Unsmoke()
	{
		// On attend 5 secondes
		yield return new WaitForSeconds (5f);
		// Pour chaque grenades du tableau de grenades fumigènes ...
		foreach (GameObject grenade in this.smokeGrenades)
		{
			// ... on désactive la grenade
			grenade.SetActive(false);
		}
	}
	
	// Accesseurs
	public int FogsNumber
	{
		get { return this.fogsNumber; }
		set { this.fogsNumber = value; }
	}
	
	public int ZombieBaitsNumber
	{
		get { return this.zombieBaitsNumber; }
		set { this.zombieBaitsNumber = value; }
	}
	
	public bool SmokingGround
	{
		get { return this.smokingGround; }
		set { this.smokingGround = value; }
	}
	
	public string BadActsType
	{
		get { return this.badActType; }
		set { this.badActType = value; }
	}

	public string ObjectTag
	{
		get { return this.objectTag; }
		set { this.objectTag = value; }
	}
}
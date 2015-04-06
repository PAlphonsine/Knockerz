using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BadActsInventoryManager : MonoBehaviour
{
	// Gestion des achats de coup(s) fourré(s)
	[SerializeField]
	BadActsShopManager badActsShopManager;
	// Gestion des phases
	[SerializeField]
	PhasesManager phasesManager;
	// Gestion du stock des Zombies
	[SerializeField]
	StockZombies stockZombiesJ1;
	[SerializeField]
	StockZombies stockZombiesJ2;
	// Fenetre de l'inventaire
	[SerializeField]
	GameObject inventoryPanel;
	// Textes et boutons des coups fourrés
	[SerializeField]
	Button fogButton;
	[SerializeField]
	Text fogButtonText;
	[SerializeField]
	Button zombieBaitButton;
	[SerializeField]
	Text zombieBaitButtonText;
	// Grenades fumigènes
	[SerializeField]
	ParticleSystem[] smokeGrenades;
	// Nombre de coups fourrés dans l'inventaire
	// Grenades fumigènes
	private int fogsNumber;
	// Appats pour Zombies
	private int zombieBaitsNumber;
	// Tableau des Zombies aléatoires envoyés à l'adversaire
	private List<ZombieScript> zombiesBaited;
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

	// Compteur de Zombies de chaque type
	int nbZt1 = 1;
	int nbZt2 = 1;
	int nbZt3 = 1;

	void Start ()
	{
		this.fogsNumber = 0;
		this.zombieBaitsNumber = 0;
		this.zombiesBaited = new List<ZombieScript> ();
		this.smokingGround = false;
		this.badActType = "";
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
				this.fogButtonText.text = "Grenade(s)\n" + this.fogsNumber.ToString();
				this.zombieBaitButtonText.text = "Appats\n" + this.zombieBaitsNumber.ToString();
			}
			
			// Si l'on est en phase de réflexion
			if (this.phasesManager.startAction == false)
			{
				// On ne peut pas lancer de coups fourrés, les boutons sont donc désactivés
				this.fogButton.interactable = false;
				this.zombieBaitButton.interactable = false;
				
				// Si les grenades fumigènes lancées à la phase précédente font encore effet
				if (this.smokingGround == true)
				{
					// Pour chaque grenades du tableau de grenades fumigènes, ...
					foreach (ParticleSystem ps in this.smokeGrenades)
					{
						// ... si la grenade est active, ...
						if (ps.gameObject.activeSelf == true)
						{
							// ... elle cesse d'emettre de la fumée
							ps.Stop();
						}
					}
					// On dissipe le nuage de fumée
					StartCoroutine(this.Unsmoke());
					// Il n'y a plus de fumée
					this.smokingGround = false;
					// Pour chaque grenades du tableau de grenades fumigènes ...
					foreach (ParticleSystem ps in this.smokeGrenades)
					{
						// ... on désactive la grenade
						ps.gameObject.SetActive(false);
					}
				}
				
				// Les compteurs restent tels qu'ils ont été initialisés
				this.nbZt1 = 1;
				this.nbZt2 = 1;
				this.nbZt3 = 1;
			}
			// Sinon, si l'on est en phase d'action
			else
			{
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
							// La fumée est activée
							this.smokingGround = true;
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

	[RPC]
	void PutSmokeGrenade(float x, float y, float z)
	{
		for (int i = 0; i < this.smokeGrenades.Length; i++)
		{
			if (this.smokeGrenades[i].gameObject.activeSelf == false)
			{
				// Une grenade fumigène est lancée à l'emplacement visé
				this.smokeGrenades[i].transform.position = new Vector3(x, y, z);
				// La grenade est activée
				this.smokeGrenades[i].gameObject.SetActive(true);
				break;
			}
		}
	}

	[RPC]
	void DefineZombiesType (NetworkPlayer player, int zt1, int zt2, int zt3, int zt4, int zt5)
	{
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

	public void SendZombies(NetworkPlayer player, int type)
	{
		bool isFirstPlayer = true;
		
		if (player == _STATICS._networkPlayer[0])
		{
			isFirstPlayer = true;
		}
		else if (player == _STATICS._networkPlayer[1])
		{
			isFirstPlayer = false;
		}
		
		if ((Network.player == player && isFirstPlayer == true) || (Network.player != player && isFirstPlayer == true))
		{
			// Si le Zombie est de type 1 ...
			if (type == 1)
			{
				if (this.stockZombiesJ2.zombiesType1[this.stockZombiesJ2.zombiesType1.Length-nbZt1].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 1 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ2.zombiesType1[this.stockZombiesJ2.zombiesType1.Length-nbZt1].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 1 attiré
					this.nbZt1++;
				}
			}
			// Sinon, si le Zombie est de type 2 ...
			else if (type == 2)
			{
				if (this.stockZombiesJ2.zombiesType2[this.stockZombiesJ2.zombiesType2.Length-nbZt2].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 2 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ2.zombiesType2[this.stockZombiesJ2.zombiesType2.Length-nbZt2].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 2 attiré
					this.nbZt2++;
				}
			}
			// Sinon, si le Zombie est de type 3 ...
			else if (type == 3)
			{
				if (this.stockZombiesJ2.zombiesType3[this.stockZombiesJ2.zombiesType3.Length-nbZt3].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 3 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ2.zombiesType3[this.stockZombiesJ2.zombiesType3.Length-nbZt3].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 3 attiré
					this.nbZt3++;
				}
			}
		}
		else/* if ((Network.player == player && isFirstPlayer == false) || (Network.player != player && isFirstPlayer == false))*/
		{
			// Si le Zombie est de type 1 ...
			if (type == 1)
			{
				if (this.stockZombiesJ1.zombiesType1[this.stockZombiesJ1.zombiesType1.Length-nbZt1].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 1 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ1.zombiesType1[this.stockZombiesJ1.zombiesType1.Length-nbZt1].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 1 attiré
					this.nbZt1++;
				}
			}
			// Sinon, si le Zombie est de type 2 ...
			else if (type == 2)
			{
				if (this.stockZombiesJ1.zombiesType2[this.stockZombiesJ1.zombiesType2.Length-nbZt2].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 2 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ1.zombiesType2[this.stockZombiesJ1.zombiesType2.Length-nbZt2].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 2 attiré
					this.nbZt2++;
				}
			}
			// Sinon, si le Zombie est de type 3 ...
			else if (type == 3)
			{
				if (this.stockZombiesJ1.zombiesType3[this.stockZombiesJ1.zombiesType3.Length-nbZt3].gameObject.activeSelf == false)
				{
					// On ajoute au tableau de Zombies attirés un Zombie de type 3 en partant de la fin du stock du joueur 2
					this.zombiesBaited.Add(this.stockZombiesJ1.zombiesType3[this.stockZombiesJ1.zombiesType3.Length-nbZt3].GetComponent<ZombieScript>());
					// On incrémente le compteur de nombre de Zombie de type 3 attiré
					this.nbZt3++;
				}
			}
		}
		ActivateSentZombies ();
	}
	
	public void ActivateSentZombies()
	{
		// Pour chaque Zombie attiré
		foreach (ZombieScript zs in this.zombiesBaited)
		{
			// On active le Zombie
			zs.gameObject.SetActive(true);
		}
		this.zombiesBaited.Clear ();
	}
	
	// Fonction Coroutine de dissipation de la fumée
	public IEnumerator Unsmoke()
	{
		yield return new WaitForSeconds (3f);
	}
	
	// Méthode de définition du mode de coup fourré
	public void SetBadActType(string type)
	{
		this.badActType = type;
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
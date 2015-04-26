using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ZombieScript : MonoBehaviour
{
	// Position de départ du Zombie
	[SerializeField] Transform start;
	// Destination du Zombie
	[SerializeField] Transform destination;
	// Agent de navigation
	NavMeshAgent agent;
	// Points de vie du Zombie
	[SerializeField] private int pv = 500;
	// Cible variable du zombie
	private FighterScript fighter;
	private MeatScript meat;
	private SurvivorScript survivor;
	// Bombes aériennes
	AirBombScript airBomb;
	// Dégats par seconde du Zombie
	[SerializeField] private int dps = 1;
	// Booléen de déclenchement de combat
	private bool isFighting;
	// Booléen de déclenchement de dégustation
	private bool isEating;
	// Booléen de déclenchement de poursuite de survivant
	private bool isChasing;
	// Booléen de controle de changement de destination
	private bool destinationChanged;
	// Booléen de controle de la position du Zombie devant la porte
	private bool isKnocking;
	// Booléen de controle de la postion du Zombie dans la base
	private bool isInBase;
	// Porte
	private DoorScript door;
	// Barre de vie du Zombie
	public GameObject lifeSprite;
	// Points de vie de départ du Zombie
	private int startPv;
	// Type de Zombie
	[SerializeField] private int type;
	// Liste des zombies en collision avec celui-ci
	private List<ZombieScript> zombiesInCollision;
	// Si le zombie est en collision avec la porte
	private bool connectedToDoor;
	// Si le zombie est en collision avec un zombie connecté à la porte
	private bool connected;
	// Variables pour sérialiser les paramètres
	private Vector3 fighterPosition;
	private Vector3 zombiePosition;
	private Quaternion zombieQuaternion;
	private Vector3 zombieDestination;
	private int pvZombie;
	[SerializeField] private GameObject deadZombie;
	
	// Use this for initialization
	void Start ()
	{
		this.agent = this.GetComponent<NavMeshAgent> ();
		this.startPv = this.pv;
		this.isFighting = false;
		this.isEating = false;
		this.isChasing = false;
		this.isKnocking = false;
		this.isInBase = false;
		this.destinationChanged = false;
		this.connectedToDoor = false;
		this.connected = false;
		// Les listes sont initialisés vides
		zombiesInCollision = new List<ZombieScript>();
	}
	
	// Update is called once per frame
	public virtual void Update ()
	{
		// La barre de vie voit son échelle dépendre des points de vie du Zombie
		lifeSprite.transform.localScale = new Vector3((float)pv/startPv,
		                                              lifeSprite.transform.localScale.y,
		                                              lifeSprite.transform.localScale.z);
		
		// Si le zombie ne touche pas la porte
		if(!connectedToDoor)
		{
			// Pour tous les zombies en collision
			foreach (ZombieScript zbs in zombiesInCollision) 
			{
				// S'il est connecté à la porte
				if((zbs.ConnectedToDoor || zbs.Connected))
				{
					// Ce zombie devient connecté
					connected = true;
					// Il se stoppe
					isKnocking = true;
					// Il attend un petit peu avec de se deconnecter de la porte
					StartCoroutine (WaitBeforePop());
					break;
				}
				else
				{
					// Le zombie reprend son chemin
					isKnocking = false;
					// Il ne se soucie plus de la porte
					this.door = null;
					// Il reprend son chemin vers sa destination par défaut
					this.agent.Resume();
					this.destinationChanged = false;
					// Il n'est plus connecté
					connected = false;
				}
			}
		}
	}
	
	// Attente avant de se deconnecter, le temps que les autres zombies detectent son état
	IEnumerator WaitBeforePop()
	{
		yield return new WaitForSeconds (0.1f);
		// Le zombie n'est plus connecté
		connected = false;
	}
	
	void FixedUpdate ()
	{
		// Si la destination du Zombie n'a pas changé
		if (this.destinationChanged == false)
		{
			// Son path est calculé selon son point de destination
			this.agent.SetDestination (this.destination.position);
		}
		
		// Si le Zombie est vivant
		if (this.pv > 0)
		{
			// Si le Zombie est en train de manger
			if (this.isEating == true)
			{
				// Sa destination devient le morceau de viande qu'il mange
				this.agent.SetDestination(this.meat.transform.position);
				// Si la distance entre le Zombie et le morceau de viande est inférieure à 2
				if(Vector3.Distance(this.transform.position, this.meat.transform.position) < 2)
				{
					// Le Zombie s'arrete ...
					this.agent.Stop();
					// ... et commence à manger la viande
					this.meat.Durability -= this.dps;
				}
				
				// Si le morceau de viande a été mangé
				if (this.meat.Durability <= 0 || !this.meat.gameObject.activeSelf)
				{
					// Le Zombie reprend son chemin vers sa destination par défaut
					this.destinationChanged = false;
					this.agent.Resume();
				}
			}
			// Sinon, si le Zombie est en train de se battre
			else if (this.isFighting == true)
			{
				// Si la distance entre le Zombie et le Fighter avec lequel il se bat est inférieure à 2
				if(Vector3.Distance(this.transform.position, this.fighter.transform.position) < 2)
				{
					// Le Zombie s'arrete ...
					this.agent.Stop();
					// ... et il attaque le Fighter
					// Calcul de la chance d'esquive du fighter
					if(Random.Range(this.fighter.Dodge,1f)>this.fighter.Dodge)
						// Le fighter perd des points de vie normalement
						this.fighter.Pv -= this.dps;
				}
				else
				{
					// Sinon, le Zombie continue d'avancer vers le Fighter
					this.agent.SetDestination(this.fighter.transform.position);
				}
				
				// Si le Fighter est mort
				if (this.fighter.Pv <= 0)
				{
					// Le Zombie ne se bat plus
					this.isFighting = false;
					// Il reprend son chemin vers sa destination par défaut
					this.destinationChanged = false;
					this.agent.Resume();
				}
			}
			// Sinon, si le Zombie est en train de poursuivre un Survivant
			else if (this.isChasing == true)
			{
				if(!this.isKnocking)
					// Sa destination devient le Survivant
					this.agent.SetDestination(this.survivor.transform.position);
				else
					// Le Zombie s'arrete
					this.agent.Stop();
				// Si la distance entre le Zombie et le Survivant qu'il poursuit est inférieure à 2
				if (Vector3.Distance(this.transform.position, this.survivor.transform.position) < 2)
				{
					// Le Zombie attaque le Survivant
					this.survivor.Pv -= this.dps;
				}
			}
			// Sinon si le Zombie est devant la porte et pas dans la base
			else if (this.isKnocking && !isInBase)
			{
				// Le Zombie s'arrete ...
				this.agent.Stop();
				// ... et frappe la porte
				if (this.door != null)
					this.door.Pv -= this.dps;
			}
		}
		// Sinon, si le Zombie est mort
		else
		{
			if(GetComponentInParent<StockZombies>() != null)
				if((GetComponentInParent<StockZombies>().Stock1 && Network.player == _STATICS._networkPlayer[0])
				  || (!GetComponentInParent<StockZombies>().Stock1 && Network.player == _STATICS._networkPlayer[1])){
				// Le joueur gagne de l'or pour avoir tué le Zombie ... 
				GameStats.Instance.Gold += 10;
				// ... et gagne de l'expérience
				GameStats.Instance.Exp += 1;
			}
			// On le reset
			StartCoroutine(this.Reset());
		}
	}
	
	// Méthode de synchronisation des paramètres du Zombie
	void OnSerializeNetworkView(BitStream stream)
	{
		// Quand le serveur écrit dans le stream
		if (stream.isWriting && Network.isServer)
		{
			this.pvZombie = this.pv;
			stream.Serialize (ref this.pvZombie);
			this.zombiePosition = transform.position;
			this.zombieQuaternion = transform.rotation;
			this.zombieDestination = this.destination.position;
			// Sérialisation de ces propriétés
			stream.Serialize (ref this.fighterPosition);
			stream.Serialize (ref this.zombiePosition);
			stream.Serialize (ref this.zombieQuaternion);
			stream.Serialize (ref this.zombieDestination);
			// Quand le client lit dans le stream
		} 
		else if (stream.isReading && Network.isClient)
		{
			// On sérialize les propriétés du zombie
			stream.Serialize (ref this.pvZombie);
			this.pv = this.pvZombie;
			stream.Serialize (ref this.fighterPosition);
			stream.Serialize (ref this.zombiePosition);
			stream.Serialize (ref this.zombieQuaternion);
			stream.Serialize (ref this.zombieDestination);
			// On les applique à l'état actuel
			transform.position = this.zombiePosition;
			transform.rotation = this.zombieQuaternion;
			// Recalcule le chemin du zombie après avoir reçu un nouvelle position de la part du serveur
			if(!isFighting && !isEating && !isChasing && !isInBase && !isKnocking)
				this.agent.SetDestination (this.destination.position);
		}
	}
	
	// Le Zombie peut colisionner avec : Fighter, Survivor, Meat, AirBomb
	void OnTriggerEnter(Collider collider)
	{
		// Si le Zombie rencontre un morceau de viande
		if (collider.tag == "Meat" && type != 4)
		{
			// Si il n'est pas déjà en train de manger
			if (this.isEating != true)
			{
				// Il s'arrete de poursuivre les Survivants
				this.isChasing = false;
				// Il se met à manger
				this.isEating = true;
				// On lui définit le morceau de viande par lequel il va etre attiré
				this.meat = collider.transform.GetComponent<MeatScript>();
				// Il va vers le morceau de viande
				this.agent.SetDestination(this.meat.transform.position);
				// Sa destination a changé
				this.destinationChanged = true;
			}
		}
		
		// Si le Zombie rencontre un Fighter, qu'il n'est pas en train de manger et qu'il n'est pas de type 2
		if (collider.tag == "Fighter" && !isEating && type != 2)
		{
			// Il s'arrete de poursuivre les Survivants
			this.isChasing = false;
			// On lui définit le Fighter qu'il va attaquer
			this.fighter = collider.transform.GetComponent<FighterScript>();
			// Si le Fighter n'est pas en train de se battre
			if (this.fighter.IsFighting != true)
			{
				// Lz Zombie se bat
				this.isFighting = true;
				// Le Zombie va vers le Fighter
				this.agent.SetDestination(this.fighter.transform.position);
				// Sa destination a changé
				this.destinationChanged = true;
			}
		}
		
		// Si le Zombie rencontre un Survivant et qu'il n'est ni en train de manger, ni en train de se battre
		if (collider.tag == "Survivor" && !isEating && !isFighting && type != 4)
		{
			// Il poursuit le Survivant
			this.isChasing = true;
			// On lui définit le Survivant qu'il poursuit
			this.survivor = collider.transform.GetComponent<SurvivorScript>();
			// Sa destination devient la position du Survivant
			this.agent.SetDestination(this.survivor.transform.position);
			// Sa destination a changé
			this.destinationChanged = true;
		}
		
		// Si le Zombie rencontre une bombe aérienne
		if (collider.tag == "AirBomb")
		{
			// La bombe qu'il rencontre ...
			this.airBomb = collider.transform.GetComponent<AirBombScript>();
			// ... lui fait des dommages
			this.pv -= this.airBomb.Damage;
		}
		
		// Si le Zombie rencontre la base
		if(collider.tag == "InBase")
		{
			// Le Zombie est dans la base
			this.isInBase = true;
			// Il n'est pas devant la porte
			this.isKnocking = false;
			// Il ne se soucie plus de la porte
			this.door = null;
			// Il reprend son chemin vers sa destination par défaut
			this.agent.Resume();
			// Il reprend sa route
			this.destinationChanged = true;
			// Il est déconnecté de tout
			if (this.zombiesInCollision != null)
				this.zombiesInCollision.Clear();
			connected = false;
			connectedToDoor = false;
		}
		
		// Si le Zombie rencontre la porte
		if (collider.tag == "Door")
		{
			// Il est devant la porte
			this.isKnocking = true;
			// On lui définit la porte qu'il va attaquer
			this.door = collider.GetComponent<DoorScript>();
			// Sa destination est stopée
			this.destinationChanged = true;
			// Il est connecté à la porte
			connectedToDoor = true;
		}
		
		// Si c'est un Zombie
		if (collider.tag == "Zombie")
		{
			// On l'ajoute à la liste des zombies en collision s'il n'y ai pas déja
			if(zombiesInCollision != null)
			if(!zombiesInCollision.Contains(collider.GetComponent<ZombieScript>()))
				zombiesInCollision.Add(collider.GetComponent<ZombieScript>());
		}
	}
	
	// Lorsqu'un objet sort du collider du Zombie
	void OnTriggerExit(Collider collider)
	{
		// Si c'est un morceau de viande
		if (collider.tag == "Meat" && type != 4)
		{
			// Le Zombie ne mange plus
			this.isEating = false;
			// Sa destination redevient celle par défaut
			this.destinationChanged = false;
			// Il n'a plus de morceau de viande à manger
			this.meat = null;
			// On fait "clignoter" son collider pour que le Zombie redevienne "attentif" à son environnement
			StartCoroutine(this.TriggerTwinkle());
			// Il redemare
			this.isKnocking = false;
		}
		
		// Si c'est un Fighter et que le Zombie n'est pas de type 2
		if (collider.tag == "Fighter" && type != 2)
		{
			// Le Zombie ne se bat plus
			this.isFighting = false;
			// Il n'a plus de Fighter à combattre
			this.fighter = null;
		}
		
		// Si c'est un Survivant
		if (collider.tag == "Survivor" && type != 4)
		{
			// Le Zombie ne le poursuit plus
			this.isChasing = false;
			// Sa destination redevient celle par défaut
			this.destinationChanged = false;
			// Il n'a plus de Survivant à poursuivre
			this.survivor = null;
		}
		
		// Si c'est la base
		if(collider.tag == "InBase")
		{
			// Il n'est plus dans la base
			this.isInBase = false;
		}
		
		// Si c'est la porte
		if (collider.tag == "Door")
		{
			// Il n'est plus devant la porte
			this.isKnocking = false;
			// Il n'a plus de porte à frapper
			this.door = null;
			// Il reprend son chemin vers sa destination par défaut
			this.agent.Resume();
			// Il reprend sa route
			this.destinationChanged = false;
			// Il est déconnecté de la porte
			connectedToDoor = false;
		}
		
		// Si c'est un Zombie
		if (collider.tag == "Zombie")
		{
			// Si il est contenu dans la liste des zombies en collision
			if(zombiesInCollision.Contains(collider.GetComponent<ZombieScript>()))
				// On le retire
				zombiesInCollision.Remove(collider.GetComponent<ZombieScript>());
			if(zombiesInCollision.Count<=0)
				connected = false;
		}
	}
	
	// Fonction Coroutine permettant le "clignotement" du collider du Zombie
	public IEnumerator TriggerTwinkle()
	{
		// Le collider du Zombie est désactivé
		this.collider.enabled = false;
		// On attend le prochain FixedUpdate()
		yield return new WaitForFixedUpdate ();
		// Le collider est activé
		this.collider.enabled = true;
	}
	
	// Fonction Coroutine de réinitialisation
	public IEnumerator Reset()
	{
		this.deadZombie.SetActive (false);
		// Le composant Survivant mort se place à l'endroit où le Survivant est mort
		this.deadZombie.transform.position = this.transform.position;
		// Le Zombie est placé sur sa position de départ
		this.transform.position = this.start.position;
		if (this.gameObject.activeSelf)
			// Le Survivant se transforme en Survivant mort
			this.deadZombie.SetActive (true);
		// On attend le prochain FixedUpdate()
		yield return new WaitForFixedUpdate ();
		// Son collider est activé
		this.collider.enabled = true;
		// Le Zombie est désactivé
		this.gameObject.SetActive (false);
		// On réinitialise tous ses paramètres à leur valeur par défaut
		this.pv = startPv;
		this.fighter = null;
		this.meat = null;
		this.survivor = null;
		this.isFighting = false;
		this.isEating = false;
		this.isChasing = false;
		this.isInBase = false;
		this.isKnocking = false;
		this.connected = false;
		this.connectedToDoor = false;
		this.destinationChanged = false;
		// Les listes sont réinitialisés
		if (this.zombiesInCollision != null)
			this.zombiesInCollision.Clear();
	}
	
	// Accesseurs
	public FighterScript Fighter
	{
		get { return this.fighter; }
		set { this.fighter = value; }
	}
	
	public int Pv
	{
		get { return this.pv; }
		set { this.pv = value; }
	}
	
	public int Dps
	{
		get { return this.dps; }
		set { this.dps = value; }
	}
	
	public bool IsFighting
	{
		get { return this.isFighting; }
		set { this.isFighting = value; }
	}
	
	public bool IsEating
	{
		get { return this.isEating; }
		set { this.isEating = value; }
	}
	
	public bool IsChasing
	{
		get { return this.isChasing; }
		set { this.isChasing = value; }
	}
	
	public bool DestinationChanged
	{
		get { return this.destinationChanged; }
		set { this.destinationChanged = value; }
	}
	
	public bool IsKnocking
	{
		get { return this.isKnocking; }
		set { this.isKnocking = value; }
	}
	
	public bool IsInBase
	{
		get { return this.isInBase; }
		set { this.isInBase = value; }
	}
	
	public int Type
	{
		get { return this.type; }
		set { this.type = value; }
	}
	
	public Transform Destination
	{
		get { return this.destination; }
		set { this.destination = value; }
	}
	
	public bool ConnectedToDoor
	{
		get { return connectedToDoor; }
		set { connectedToDoor = value; }
	}
	
	public bool Connected
	{
		get { return connected; }
		set { connected = value; }
	}
	
	public int StartPv 
	{
		get { return startPv; }
		set { startPv = value; }
	}
}
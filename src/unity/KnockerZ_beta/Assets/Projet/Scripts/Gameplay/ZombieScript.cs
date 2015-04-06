using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieScript : MonoBehaviour
{
	// Position de départ du Zombie
	[SerializeField]
	Transform start;
	// Destination du Zombie
	[SerializeField]
	Transform destination;
	// Agent de navigation
	NavMeshAgent agent;
	// Points de vie du Zombie
	[SerializeField]
	private int pv = 500;
	// Cible variable du zombie
	private FighterScript fighter;
	private MeatScript meat;
	private SurvivorScript survivor;
	// Bombes aériennes
	AirBombScript airBomb;
	// Dégats par seconde du Zombie
	[SerializeField]
	private int dps = 1;
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
	[SerializeField]
	private int type;
	// 
	private bool free;
	
	// Use this for initialization
	void Start ()
	{
		this.agent = this.GetComponent<NavMeshAgent> ();
		//this.pv = 500;
		this.startPv = this.pv;
		//this.dps = 2;
		this.isFighting = false;
		this.isEating = false;
		this.isChasing = false;
		this.isKnocking = false;
		this.isInBase = false;
		this.destinationChanged = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// La barre de vie voit son échelle dépendre des points de vie du Zombie
		lifeSprite.transform.localScale = new Vector3((float)pv/startPv,
		                                              lifeSprite.transform.localScale.y,
		                                              lifeSprite.transform.localScale.z);
	}
	
	void FixedUpdate ()
	{
		// Si la destination du Zombie n'a pas changé
		if (this.destinationChanged == false)
		{
			// Son path est calculé selon son point de destination
			this.agent.SetDestination (this.destination.position);
			//this.destinationChanged = true;
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
				if (this.meat.Durability <= 0)
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
				// Sa destination devient le Survivant
				this.agent.SetDestination(this.survivor.transform.position);
				
				// Si la distance entre le Zombie et le Survivant qu'il poursuit est inférieure à 2
				if (Vector3.Distance(this.transform.position, this.survivor.transform.position) < 2)
				{
					// Le Zombie attaque le Survivant
					this.survivor.Pv -= this.dps;
				}
				
				/*if (this.survivor.Pv <= 0)
				{
					this.isChasing = false;
					this.survivor = null;
					this.destinationChanged = false;
				}*/
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
				// Le joueur gagner de l'or pour avoir tué le Zombi
				GameStats.Instance.Gold += 10;
			}
			// On le reset
			StartCoroutine(this.Reset());
		}
	}

	// Méthode de synchronisation des paramètres du Zombie
	void OnSerializeNetworkView(BitStream stream)
	{
		if (stream.isWriting) {
			stream.Serialize (ref this.pv);
			stream.Serialize (ref this.dps);
			/*stream.Serialize (ref this.isFighting);
			stream.Serialize (ref this.isEating);
			stream.Serialize (ref this.isChasing);
			stream.Serialize (ref this.isInBase);
			stream.Serialize (ref this.isKnocking);*/
		} else {
			stream.Serialize (ref this.pv);
			stream.Serialize (ref this.dps);
			/*stream.Serialize (ref this.isFighting);
			stream.Serialize (ref this.isEating);
			stream.Serialize (ref this.isChasing);
			stream.Serialize (ref this.isInBase);
			*/stream.Serialize (ref this.isKnocking);
			// Recalcule le chemin du zombie après avoir reçu un nouvelle position de la part du serveur
			if(!isFighting && !isEating && !isChasing && !isInBase)
				this.agent.SetDestination (this.destination.position);
		}
	}

	// Le Zombie peut colisionner avec : Fighter, Survivor, Meat, AirBomb
	void OnTriggerEnter(Collider collider)
	{
		// Si le Zombie rencontre un morceau de viande
		if (collider.tag == "Meat")
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
		if (collider.tag == "Survivor" && !isEating && !isFighting)
		{
			// Il poursuit le Survivant
			this.isChasing = true;
			// On lui définit le Survivant qu'il poursuit
			this.survivor = collider.transform.GetComponent<SurvivorScript>();
			// Sa destination devient la position du Survivant
			this.agent.SetDestination(this.survivor.transform.position);
			// Sa destination a changé
			this.destinationChanged = true;
			//Debug.Log("Survivor ENTERING");
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
		}
		
		// Si le Zombie rencontre la porte
		if (collider.tag == "Door")
		{
			// Il est devant la porte
			this.isKnocking = true;
			// On lui définit la porte qu'il va attaquer
			this.door = collider.GetComponent<DoorScript>();
		}

		/*// Si c'est un Zombie
		if (collider.tag == "Zombie")
		{
			// Si le Zombie rencontré est devant la porte et pas dans la base
			if(collider.GetComponent<ZombieScript>().IsKnocking == true && !isInBase)
				// Le Zombie s'arrete
				this.agent.Stop();
			//this.isKnocking = true;
		}*/
	}
	
	// Lorsqu'un objet reste dans le collider du Zombie
	void OnTriggerStay(Collider collider)
	{
		// Si c'est un Zombie
		if (collider.tag == "Zombie")
		{
			// Si le Zombie rencontré est devant la porte et pas dans la base
			if(collider.GetComponent<ZombieScript>().IsKnocking == true && !isInBase)
				// Le Zombie s'arrete
				this.agent.Stop();
			//this.isKnocking = true;
		}
	}
	
	// Lorsqu'un objet sort du collider du Zombie
	void OnTriggerExit(Collider collider)
	{
		// Si c'est un morceau de viande
		if (collider.tag == "Meat")
		{
			// Le Zombie ne mange plus
			this.isEating = false;
			// Sa destination redevient celle par défaut
			this.destinationChanged = false;
			// Il n'a plus de morceau de viande à manger
			this.meat = null;
			// On fait "clignoter" son collider pour que le Zombie redevienne "attentif" à son environnement
			StartCoroutine(this.TriggerTwinkle());
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
		if (collider.tag == "Survivor")
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
		}
		
		/*// Si c'est un Zombie
		if (collider.tag == "Zombie")
		{
			// Si le Zombie rencontré est devant la porte et pas dans la base
			if(collider.GetComponent<ZombieScript>().IsKnocking == true && !isInBase)
				// Le Zombie s'arrete
				this.agent.Resume();
			//this.isKnocking = true;
		}*/
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
		// Le Zombie est placé sur sa position de départ
		this.transform.position = this.start.position;
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
		this.destinationChanged = false;
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
		set { this.destinationChanged = value;}
	}
	
	public bool IsKnocking
	{
		get { return this.isKnocking; }
		set { this.isKnocking = value;}
	}
	
	public bool IsInBase
	{
		get { return this.isInBase; }
		set { this.isInBase = value;}
	}
	
	public int Type
	{
		get { return this.type; }
		set { this.type = value;}
	}

	public Transform Destination
	{
		get { return this.destination; }
		set { this.destination = value;}
	}
}
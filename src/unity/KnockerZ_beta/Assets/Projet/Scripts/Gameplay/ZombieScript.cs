using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieScript : MonoBehaviour {
	// Zone de stock du Zombie
	[SerializeField]
	Transform stock;
	// Destination du Zombie
	[SerializeField]
	Transform destination;
	// Agent de navigation
	NavMeshAgent agent;

	// Points de vie du Zombie
	private int pv;

	// Cible variable du zombie
	private FighterScript fighter;
	private MeatScript meat;
	private SurvivorScript survivor;
	// Bombes aériennes
	AirBombScript airBomb;
	// Dégats par seconde du Zombie
	private int dps;

	// Booléen de déclenchement de combat
	private bool isFighting;
	// Booléen de déclenchement de dégustation
	private bool isEating;
	// Booléen de déclenchement de poursuite de survivant
	private bool isChasing;

	// Booléen de controle de changement de destination
	private bool destinationChanged;

	private bool isKnocking;
	private bool isInBase;
	private DoorScript door;

	public GameObject lifeSprite;

	private int startPv;

	// Use this for initialization
	void Start () {
		this.agent = this.GetComponent<NavMeshAgent> ();
		this.pv = 500;
		this.startPv = this.pv;
		this.dps = 2;
		this.isFighting = false;
		this.isEating = false;
		this.isChasing = false;
		this.isKnocking = false;
		this.isInBase = false;
		this.destinationChanged = false;
	}
	
	// Update is called once per frame
	void Update () {
		lifeSprite.transform.localScale = new Vector3((float)pv/startPv,
		                                              lifeSprite.transform.localScale.y,
		                                              lifeSprite.transform.localScale.z);
	}
	
	void FixedUpdate () {
		if (this.destinationChanged == false)
		{
			this.agent.SetDestination (this.destination.position);
		}
		
		if (this.pv > 0)
		{
			if (this.isEating == true)
			{
				this.agent.SetDestination(this.meat.transform.position);
				if(Vector3.Distance(this.transform.position, this.meat.transform.position) < 2)
				{
					this.agent.Stop();
					this.meat.Durability -= this.dps;
				}
				
				if (this.meat.Durability <= 0)
				{
					this.destinationChanged = false;
					this.agent.Resume();
				}
			}
			else if (this.isFighting == true)
			{
				if(Vector3.Distance(this.transform.position, this.fighter.transform.position) < 2)
				{
					this.agent.Stop();
					this.fighter.Pv -= this.dps;
				}
				else
				{
					this.agent.SetDestination(this.fighter.transform.position);
				}
				
				if (this.fighter.Pv <= 0)
				{
					this.isFighting = false;
					this.destinationChanged = false;
					this.agent.Resume();
				}
			}
			else if (this.isChasing == true)
			{
				this.agent.SetDestination(this.survivor.transform.position);
				
				if (Vector3.Distance(this.transform.position, this.survivor.transform.position) < 2)
				{
					this.survivor.Pv -= this.dps;
				}
				
				/*if (this.survivor.Pv <= 0)
				{
					this.isChasing = false;
					this.survivor = null;
					this.destinationChanged = false;
				}*/
			}else if (this.isKnocking && !isInBase)
			{
				this.agent.Stop();
				this.door.Pv -= this.dps;
			}
		}
		else
		{
			StartCoroutine(this.Reset());
		}
	}

	void OnSerializeNetworkView(BitStream stream)
	{
		stream.Serialize (ref this.pv);
		stream.Serialize (ref this.dps);
		stream.Serialize (ref this.isFighting);
		stream.Serialize (ref this.isEating);
	}

	// Le Zombie peut colisionner avec : Fighter, Survivor, Meat, AirBomb
	void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Meat")
		{
			if (this.isEating != true)
			{
				this.isChasing = false;
				this.isEating = true;
				this.meat = collider.transform.GetComponent<MeatScript>();
				this.agent.SetDestination(this.meat.transform.position);
				this.destinationChanged = true;
				//Debug.Log("Meat ENTERING");
			}
		}
		
		if (collider.tag == "Fighter" && !isEating)
		{
			this.isChasing = false;
			this.fighter = collider.transform.GetComponent<FighterScript>();
			if (this.fighter.IsFighting != true)
			{
				this.isFighting = true;
				this.agent.SetDestination(this.fighter.transform.position);
				this.destinationChanged = true;
			}
			//Debug.Log("Fighter ENTERING");
		}

		if (collider.tag == "Survivor" && !isEating && !isFighting)
		{
			this.isChasing = true;
			this.survivor = collider.transform.GetComponent<SurvivorScript>();
			this.agent.SetDestination(this.survivor.transform.position);
			this.destinationChanged = true;
			//Debug.Log("Survivor ENTERING");
		}

		if (collider.tag == "AirBomb")
		{
			this.airBomb = collider.transform.GetComponent<AirBombScript>();
			this.pv -= this.airBomb.Damage;
		}

		if(collider.tag == "InBase")
		{
			this.isInBase = true;
		}

		if (collider.tag == "Door")
		{
			this.isKnocking = true;
			this.door = collider.GetComponent<DoorScript>();
		}
	}

	void OnTriggerStay(Collider collider){
		if (collider.tag == "Zombie")
		{
			if(collider.GetComponent<ZombieScript>().IsKnocking == true && !isInBase)
				this.agent.Stop();
				//this.isKnocking = true;
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Meat")
		{
			this.isEating = false;
			this.destinationChanged = false;
			this.meat = null;
			//Debug.Log("Meat EXITING");
			StartCoroutine(this.TriggerTwinkle());
		}
		
		if (collider.tag == "Fighter")
		{
			this.isFighting = false;
			this.fighter = null;
			//Debug.Log("Fighter EXITING");
		}
		
		if (collider.tag == "Survivor")
		{
			this.isChasing = false;
			this.destinationChanged = false;
			this.survivor = null;
			//Debug.Log("Survivor EXITING");
		}

		if(collider.tag == "InBase")
		{
			this.isInBase = false;
		}

		if (collider.tag == "Door")
		{
			this.isKnocking = false;
			this.door = null;
			this.agent.Resume();
		}

		//this.destinationChanged = false;
	}

	public IEnumerator TriggerTwinkle()
	{
		this.collider.enabled = false;
		yield return new WaitForFixedUpdate ();
		this.collider.enabled = true;
		//Debug.Log ("Twinkle");
	}

	public IEnumerator Reset()
	{
		GameStats.Instance.Gold += 10;
		this.transform.position = this.stock.position;
		yield return new WaitForFixedUpdate ();
		this.collider.enabled = true;
		this.gameObject.SetActive (false);
		this.pv = 500;
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
}

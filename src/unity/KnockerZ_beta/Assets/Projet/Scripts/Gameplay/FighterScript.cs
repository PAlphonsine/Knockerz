using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FighterScript : MonoBehaviour {
	// Gestion des phases
	[SerializeField]
	PhasesManager phasesManager;

	// Position de départ du Fighter
	[SerializeField]
	private Transform startPosition;
	// Points de vie du Fighter
	private int pv;

	// Cible variable du Fighter
	ZombieScript zombie;
	// Dernier Zombie renconté par le Fighter
	ZombieScript lastZombie;
	// Dégats par seconde du Fighter
	private int dps;

	// Booléen de déclenchement de combat
	private bool isFighting;

	// Collection des cibles potentielles du Fighter
	private List<ZombieScript> canBeAttacked;

	private int startPv;
	public GameObject lifeSprite;

	// Use this for initialization
	void Start () {
		this.pv = 1000;
		this.startPv = this.pv;
		this.dps = 4;
		this.isFighting = false;
		this.canBeAttacked = new List<ZombieScript>();
		this.lastZombie = null;
	}
	
	// Update is called once per frame
	void Update () {
		lifeSprite.transform.localScale = new Vector3((float)pv/startPv, 
		                                              lifeSprite.transform.localScale.y, 
		                                              lifeSprite.transform.localScale.z);
	}

	void FixedUpdate () {
		if (this.phasesManager.startAction == true)
		{
			if (this.pv > 0)
			{
				if (this.isFighting == true)
				{
					this.lastZombie = this.zombie;
					this.zombie.IsFighting = true;
					this.zombie.Fighter = this;
					this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.zombie.transform.position.x, this.zombie.transform.position.y, this.zombie.transform.position.z+1.2f), 0.03f);
					this.zombie.Pv -= this.dps;
					
					if (this.zombie.Pv <= 0)
					{
						this.isFighting = false;
						this.zombie.IsFighting = false;
						this.canBeAttacked.RemoveAt(0);
						
						for (int i = this.canBeAttacked.Count-1; i >= 0; i--)
						{
							if ((Vector3.Distance(this.transform.position, this.canBeAttacked[i].transform.position) > 5) || this.canBeAttacked[i].gameObject.activeSelf == false)
							{
								this.canBeAttacked.RemoveAt(i);
							}
						}
						
						if (this.canBeAttacked.Count != 0)
						{
							this.zombie = this.canBeAttacked[0];
							this.isFighting = true;
						}
					}
				}
				else
				{
					this.Guard();
				}
			}
			else
			{
				StartCoroutine(this.Reset());
			}
		}
		else
		{
			this.lastZombie = null;
			this.Guard();
		}
	}

	void OnSerializeNetworkView(BitStream stream)
	{
		stream.Serialize (ref this.pv);
		stream.Serialize (ref this.dps);
		stream.Serialize (ref this.isFighting);
	}

	// Le Fighter peut colisionner avec : Zombies
	void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Zombie")
		{
			//Debug.Log("Zombie ENTERING");
			this.isFighting = true;
			//this.zombie = collider.transform.GetComponent<ZombieScript>();
			if (this.lastZombie == null || collider.gameObject != this.lastZombie.gameObject)
			{
				this.canBeAttacked.Add(collider.transform.GetComponent<ZombieScript>());
				//Debug.Log("New Zombie Added");
				this.zombie = this.canBeAttacked[0];
			}
			else
			{
				this.zombie = this.canBeAttacked[0];
			}
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Zombie")
		{
			//Debug.Log("Zombie EXITING");
			if (this.canBeAttacked.Count == 0)
			{
				this.isFighting = false;
				this.zombie = null;
			}
			else
			{
				this.isFighting = true;
			}
		}
	}

	public void Guard()
	{
		this.transform.position = Vector3.Lerp(this.transform.position, this.startPosition.position, 0.03f);
	}

	public IEnumerator Reset()
	{
		this.transform.position = new Vector3 (0, 0, -80f);
		yield return new WaitForFixedUpdate ();
		this.gameObject.SetActive (false);
		this.pv = 1000;
		this.zombie = null;
		this.lastZombie = null;
		this.isFighting = false;
	}

	// Accesseurs
	public Transform StartPosition
	{
		get { return this.startPosition; }
		set { this.startPosition = value; }
	}

	public Vector3 StartPositionPosition
	{
		get { return this.startPosition.position; }
		set { this.startPosition.position = value; }
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
}

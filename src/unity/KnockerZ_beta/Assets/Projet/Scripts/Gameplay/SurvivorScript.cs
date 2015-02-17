using UnityEngine;
using System.Collections;

public class SurvivorScript : MonoBehaviour {
	// Zone de stock du Survivant
	[SerializeField]
	Transform stock;
	// Destination du Survivant
	[SerializeField]
	Transform destination;
	// Agent de navigation
	NavMeshAgent agent;

	// Cadavre du Survivant s'il meurt sur le chemin
	[SerializeField]
	Transform deadSurvivor;
	
	// Points de vie du Survivant
	private int pv;
	private int startPv;
	public GameObject lifeSprite;
	
	private bool isKnocking;

	private bool isInBase;

	// Use this for initialization
	void Start () {
		this.agent = this.GetComponent<NavMeshAgent> ();
		this.pv = 100;
		this.startPv = this.pv;
	}
	
	// Update is called once per frame
	void Update () {
		lifeSprite.transform.localScale = new Vector3((float)pv/startPv,
		                                              lifeSprite.transform.localScale.y,
		                                              lifeSprite.transform.localScale.z);
	}

	void FixedUpdate () {
		if (this.pv <= 0)
		{
			//Debug.Log ("MORT");
			this.BeingEaten();
			StartCoroutine(this.Reset());
		}
		if (this.isKnocking && !isInBase)
		{
			this.agent.Stop();
		}else
			this.agent.SetDestination (this.destination.position);
	}

	void OnSerializeNetworkView(BitStream stream)
	{
		stream.Serialize (ref this.pv);
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "InBase")
		{
			this.isInBase = true;
		}

		if (collider.tag == "Door")
		{
			this.isKnocking = true;
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if(collider.tag == "InBase")
		{
			this.isInBase = false;
		}

		if (collider.tag == "Door")
		{

			this.isKnocking = false;
			this.agent.Resume();
		}
	}

	public void BeingEaten()
	{
		this.deadSurvivor.gameObject.SetActive (true);
		this.deadSurvivor.position = this.transform.position;
	}

	public IEnumerator Reset()
	{
		this.transform.position = this.stock.position;
		yield return new WaitForFixedUpdate ();
		this.gameObject.SetActive (false);
		this.isInBase = false;
		this.isKnocking = false;
		this.pv = 100;
	}

	// Accesseurs
	public int Pv
	{
		get { return this.pv; }
		set { this.pv = value; }
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

using UnityEngine;
using System.Collections;

public class AirBombScript : MonoBehaviour {

	// Gestion des phases
	[SerializeField]
	PhasesManager phasesmanager;
	// Booléen de controle de résolution de l'effet
	private bool explosion;
	// Dommages d'une bombe
	private int damage;
	// Gestion de l'inventaire
	[SerializeField]
	SupportInventoryManager supportInventoryManager;
	
	// Use this for initialization
	void Start () {
		this.explosion = false;
		this.damage = 250;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		if (this.phasesmanager.startgame == true)
		{
			if (this.explosion == true)
			{
				StartCoroutine(this.Reset());
			}
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Path")
		{
			this.explosion = true;
			this.supportInventoryManager.HittedTheGround = true;
		}
	}

	public IEnumerator Reset()
	{
		this.transform.position = new Vector3(-20, 0, 40);
		yield return new WaitForFixedUpdate ();
		this.gameObject.SetActive (false);
		this.explosion = false;
	}

	// Accesseurs
	public bool Explosion
	{
		get { return this.explosion; }
		set { this.explosion = value; }
	}

	public int Damage
	{
		get { return this.damage; }
		set { this.damage = value; }
	}
}

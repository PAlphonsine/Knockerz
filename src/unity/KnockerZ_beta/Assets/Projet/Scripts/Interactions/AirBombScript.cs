using UnityEngine;
using System.Collections;

public class AirBombScript : MonoBehaviour
{
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

	void Start ()
	{
		this.explosion = false;
		this.damage = 250;
	}
	
	void FixedUpdate ()
	{
		// Si la partie a commencé
		if (this.phasesmanager.startgame == true)
		{
			// Si la bombe a explosé
			if (this.explosion == true)
			{
				// On la reset
				StartCoroutine(this.Reset());
			}
		}
	}

	// Lorsque la bombe rencontre un objet
	void OnTriggerEnter(Collider collider)
	{
		// Si le tag de l'objet est "Path"
		if (collider.tag == "PathJ1" || collider.tag == "PathJ2")
		{
			// La bombe explose
			this.explosion = true;
			// On active la possibilité d'en envoyer une autre
			this.supportInventoryManager.HittedTheGround = true;
		}
	}

	// Fonction Coroutine de reset
	public IEnumerator Reset()
	{
		// On déplace la bombe
		this.transform.position = new Vector3(-20, 0, 40);
		// On attend le prochain FixedUpdate ()
		yield return new WaitForFixedUpdate ();
		// On désactive la bombe
		this.gameObject.SetActive (false);
		// La bombe n'explose pas
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
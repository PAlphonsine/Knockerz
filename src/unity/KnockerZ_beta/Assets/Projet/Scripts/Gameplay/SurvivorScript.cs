using UnityEngine;
using System.Collections;

public class SurvivorScript : MonoBehaviour
{
	// Position de départ du Survivant
	[SerializeField]
	Transform start;
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
	// Points de vie de départ du Survivant
	private int startPv;
	// Barre de vie du Survivant
	public GameObject lifeSprite;
	// Booléen de controle de position du Survivant devant la porte
	private bool isKnocking;
	// Booléen de controle de position du Survivant dans la base
	private bool isInBase;
	// Tete du Survivant
	[SerializeField]
	private GameObject head;
	// Sac-à-dos du Survivant
	[SerializeField]
	private GameObject backpack;
	// Skin du Survivant
	[SerializeField]
	private GameObject skin1;
	
	// Use this for initialization
	void Start ()
	{
		this.agent = this.GetComponent<NavMeshAgent> ();
		this.pv = 100;
		this.startPv = this.pv;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// La barre de vie voit son échelle dépendre des points de vie du Survivant
		lifeSprite.transform.localScale = new Vector3((float)pv/startPv,
		                                              lifeSprite.transform.localScale.y,
		                                              lifeSprite.transform.localScale.z);
	}
	
	void FixedUpdate ()
	{
		// Si le Survivant est mort
		if (this.pv <= 0)
		{
			// Il se fait manger
			this.BeingEaten();
			// Il est reset
			StartCoroutine(this.Reset());
		}
		// Si le Survivant est devant la porte et n'est pas dans la base
		if (this.isKnocking && !isInBase)
		{
			// Il ne bouge plus
			this.agent.Stop();
		}
		else
		{
			// Sinon, il continue son chemin vers la base
			this.agent.SetDestination (this.destination.position);
		}
	}
	
	// Méthode de synchronisation des paramètres du Survivant
	void OnSerializeNetworkView(BitStream stream)
	{
		stream.Serialize (ref this.pv);
	}
	
	// Lorsqu'un objet rentre en collision avec le Survivant
	void OnTriggerEnter(Collider collider)
	{
		// Si l'objet est l'intérieur de la base
		if(collider.tag == "InBase")
		{
			// Le Survivant est dans la base
			this.isInBase = true;
		}
		
		// Si l'objet est la porte
		if (collider.tag == "Door")
		{
			// Le Survivant est de vant la porte
			this.isKnocking = true;
		}
	}
	
	// Lorsqu'un objet ne rentre plus en collision du Survivant
	void OnTriggerExit(Collider collider)
	{
		// Si l'objet est l'intérieur de la base
		if(collider.tag == "InBase")
		{
			// Le Survivant n'est plus dans la base
			this.isInBase = false;
		}
		
		// Si l'objet est la porte
		if (collider.tag == "Door")
		{
			// Il ne frappe plus
			this.isKnocking = false;
			// Il reprend son chemin vers la base
			this.agent.Resume();
		}
	}
	
	// Méthode de transformation du Survivant lors de sa mort
	public void BeingEaten()
	{
		// Le Survivant se transforme en Survivant mort
		this.deadSurvivor.gameObject.SetActive (true);
		// Le composant Survivant mort se place à l'endroit où le Survivant est mort
		this.deadSurvivor.position = this.transform.position;
	}
	
	// Fonction Coroutine de réinitialisation des paramètres du Survivant
	public IEnumerator Reset()
	{
		// Le Survivant est placé sur sa position de départ
		this.transform.position = this.start.position;
		// On attend le prochain FixedUpdate()
		yield return new WaitForFixedUpdate ();
		// On désactive le Survivant
		this.gameObject.SetActive (false);
		// Le Survivant n'est pas dans la base
		this.isInBase = false;
		// Le Survivant n'est pas devant la porte
		this.isKnocking = false;
		// Les points de vie du Survivant sont réinitialisés
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
	
	public GameObject Head
	{
		get { return this.head; }
		set { this.head = value;}
	}
	
	public GameObject Hat
	{
		get { return this.skin1; }
		set { this.skin1 = value;}
	}

	public GameObject Backpack {
		get {
			return backpack;
		}
		set {
			backpack = value;
		}
	}

	public Transform Destination {
		get {
			return destination;
		}
		set {
			destination = value;
		}
	}
}
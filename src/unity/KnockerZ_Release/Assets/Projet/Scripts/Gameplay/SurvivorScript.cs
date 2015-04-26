using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurvivorScript : MonoBehaviour
{
	// Position de départ du Survivant
	[SerializeField] Transform start;
	// Destination du Survivant
	[SerializeField] Transform destination;
	// Agent de navigation
	NavMeshAgent agent;
	// Cadavre du Survivant s'il meurt sur le chemin
	[SerializeField] Transform deadSurvivor;
	// Points de vie du Survivant
	[SerializeField] private int pv;
	// Points de vie de départ du Survivant
	private int startPv;
	// Barre de vie du Survivant
	public GameObject lifeSprite;
	// Booléen de controle de position du Survivant devant la porte
	private bool isKnocking;
	// Booléen de controle de position du Survivant dans la base
	private bool isInBase;
	// Tete du Survivant
	[SerializeField] private GameObject head;
	// Sac-à-dos du Survivant
	[SerializeField] private GameObject backpack;
	// Skin du Survivant
	[SerializeField] private GameObject skin1;
	// Liste des survivants en collision avec celui-ci
	private List<SurvivorScript> survivorsInCollision;
	// Si le survivant est en collision avec la porte
	[SerializeField] private bool connectedToDoor;
	// Si le survivant est en collision avec un zombie connecté à la porte
	[SerializeField] private bool connected;
	// Variables pour sérialiser les paramètres
	private Vector3 survivorPosition;
	private Quaternion survivorQuaternion;
	private int pvValue;
	
	// Use this for initialization
	void Start ()
	{
		// Définitions de base
		this.agent = this.GetComponent<NavMeshAgent> ();
		this.pv = 100;
		this.startPv = this.pv;
		this.connectedToDoor = false;
		this.connected = false;
		// Les listes sont initialisés vides
		survivorsInCollision = new List<SurvivorScript>();
	}

	void Update ()
	{
		// La barre de vie voit son échelle dépendre des points de vie du Survivant
		lifeSprite.transform.localScale = new Vector3((float)pv/startPv,
		                                              lifeSprite.transform.localScale.y,
		                                              lifeSprite.transform.localScale.z);

		// Si le survivant ne touche pas la porte
		if(!connectedToDoor)
		{
			// Pour tous les survivants en collision
			foreach (SurvivorScript zbs in survivorsInCollision) 
			{
				// S'il est connecté à la porte
				if((zbs.ConnectedToDoor || zbs.Connected))
				{
					// Ce survivant devient connecté
					connected = true;
					// Il se stoppe
					isKnocking = true;
					// Il attend un petit peu avec de se deconnecter de la porte
					StartCoroutine (WaitBeforePop());
					break;
				}
				else
				{
					// Le survivant reprend son chemin
					isKnocking = false;
					// Il reprend son chemin vers sa destination par défaut
					this.agent.Resume();
					// Il n'est plus connecté
					connected = false;
				}
			}
		}
	}

	// Attente avant de se deconnecter, le temps que les autres survivants detectent son état
	IEnumerator WaitBeforePop()
	{
		yield return new WaitForSeconds (0.1f);
		// Le survivant n'est plus connecté
		connected = false;
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
		// Quand le serveur écrit dans le stream
		if (stream.isWriting && Network.isServer)
		{
			this.pvValue = this.pv;
			stream.Serialize (ref this.pvValue);
			this.survivorPosition = transform.position;
			this.survivorQuaternion = transform.rotation;
			// Sérialisation de ces propriétés
			stream.Serialize (ref this.survivorPosition);
			stream.Serialize (ref this.survivorQuaternion);
			// Quand le client lit dans le stream
		}
		else if (stream.isReading && Network.isClient)
		{
			// On sérialize les propriétés du zombie
			stream.Serialize (ref this.pvValue);
			this.pv = this.pvValue;
			stream.Serialize (ref this.survivorPosition);
			stream.Serialize (ref this.survivorQuaternion);
			// On les applique à l'état actuel
			transform.position = this.survivorPosition;
			transform.rotation = this.survivorQuaternion;
			// Recalcule le chemin du survivant après avoir reçu un nouvelle position de la part du serveur
			if(!isInBase && !isKnocking)
				this.agent.SetDestination (this.destination.position);
		}
	}
	
	// Lorsqu'un objet rentre en collision avec le Survivant
	void OnTriggerEnter(Collider collider)
	{
		// Si l'objet est l'intérieur de la base
		if(collider.tag == "InBase")
		{
			// Le Survivant est dans la base
			this.isInBase = true;
			// Il est déconnecté de tout
			if (this.survivorsInCollision != null)
				this.survivorsInCollision.Clear();
			connected = false;
			connectedToDoor = false;
		}
		
		// Si l'objet est la porte
		if (collider.tag == "Door")
		{
			// Le Survivant est devant la porte
			this.isKnocking = true;
			// Il est connecté de la porte
			connectedToDoor = true;
		}

		// Si c'est un Zombie
		if (collider.tag == "Survivor")
		{
			// On l'ajoute à la liste des survivants en collision s'il n'y ai pas déja
			if(!survivorsInCollision.Contains(collider.GetComponent<SurvivorScript>()))
				survivorsInCollision.Add(collider.GetComponent<SurvivorScript>());
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
			// Il est déconnecté de la porte
			connectedToDoor = false;
		}

		// Si c'est un Zombie
		if (collider.tag == "Survivor")
		{
			// Si il est contenu dans la liste des survivants en collision
			if(survivorsInCollision.Contains(collider.GetComponent<SurvivorScript>()))
				survivorsInCollision.Remove(collider.GetComponent<SurvivorScript>());
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
		// Les listes sont réinitialisés
		if (this.survivorsInCollision != null)
			this.survivorsInCollision.Clear();
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

	public GameObject Backpack
	{
		get { return backpack; }
		set { backpack = value; }
	}

	public Transform Destination
	{
		get { return destination; }
		set { destination = value; }
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
}
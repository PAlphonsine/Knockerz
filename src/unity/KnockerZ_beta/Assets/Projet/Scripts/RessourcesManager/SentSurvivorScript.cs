using UnityEngine;
using System.Collections;

public class SentSurvivorScript : MonoBehaviour
{
	// Destination du Survivant hors de portée de vision de la caméra
	[SerializeField]
	GameObject destination;
	// Détection des phases
	[SerializeField]
	PhasesManager phasesManager;
	// Position de départ des Survivants (la base)
	[SerializeField]
	GameObject startPosition;
	public AnimationCurve curve;
	private float progression;
	// Booléen activant le départ de la base (fin phase réflexion/début phase action)
	private bool goSearch;
	// Booléen déterminant du retour ou non du Survivant
	private bool comeBack;
	// Booléen activant la possibilité de retour physique du Survivant (fin phase action/début phase réflexion)
	private bool returning;
	// Position du Survivant
	Vector3 survivorPosition;
	// Le Survivant va soit chercher des matériaux, soit des armes
	private bool materials;
	private bool weapons;
	
	// Use this for initialization
	void Start ()
	{
		this.progression = 0f;
		// Le Survivant ne va pas encore chercher de ressources, il n'a donc pas encore de chance ou d'ordre de revenir
		this.goSearch = false;
		this.comeBack = false;
		this.returning = false;
		this.survivorPosition = transform.position;
		this.materials = false;
		this.weapons = false;
	}
	
	void FixedUpdate()
	{
		//transform.position = survivorPosition;
		// Lorsque le Survivant part chercher des ressources
		if (goSearch == true)
		{
			// On active le Survivant
			this.gameObject.SetActive(true);
			// Animation du mouvement
			this.transform.position = Vector3.Lerp (this.transform.position, this.destination.transform.position, 0.1f);
			//transform.rotation = Quaternion.Lerp (transform.rotation, this.destination.transform.rotation, 0.05f);
			
			// Si le Survivant est arrivé à destination, il s'arrete
			if (this.transform.position == this.destination.transform.position)
			{
				goSearch = false;
			}
			
			// Il ne va plus chercher de ressources jusqu'à la prochaine phase d'action
			//goSearch = false;
		}
		
		if (phasesManager.startAction == false)
		{
			// Quand on est en phase de réflexion et que le Survivant a survécu
			if (this.comeBack == true)
			{
				// Il revient à sa position de départ
				this.transform.position = Vector3.Lerp (this.transform.position, this.startPosition.transform.position, 0.1f);
				//transform.rotation = Quaternion.Lerp (transform.rotation, this.startPosition.transform.rotation, 0.05f);
				// On le désactive
				this.gameObject.SetActive(false);
			}
		}
		//survivorPosition = transform.position;
	}
	
	void OnSerializeNetworkView(BitStream stream)
	{
		//stream.Serialize(ref i);
		stream.Serialize(ref  progression);
		stream.Serialize(ref  survivorPosition);
	}
	
	// Accesseurs
	public bool GoSearch
	{
		get { return this.goSearch; }
		set { this.goSearch = value; }
	}
	
	public bool ComeBack
	{
		get { return this.comeBack; }
		set { this.comeBack = value; }
	}
	
	public bool Returning
	{
		get { return this.returning; }
		set { this.returning = value; }
	}
	
	public bool Materials
	{
		get { return this.materials; }
		set
		{
			this.weapons = false;
			this.materials = value;
		}
	}
	
	public bool Weapons
	{
		get { return this.weapons; }
		set
		{
			this.materials = false;
			this.weapons = value;
		}
	}
}

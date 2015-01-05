using UnityEngine;
using System.Collections;

public class SentSurvivorScript : MonoBehaviour {
	// Destination du survivant hors de portée de vision de la caméra
	[SerializeField]
	GameObject destination;
	// Copie du timer du jeu
	[SerializeField]
	PhasesManager phasesManager;
	// Position de départ des survivants (la base)
	[SerializeField]
	GameObject startPosition;
	public AnimationCurve curve;
	private float progression;
	// Booléen activant le départ de la base (fin phase réflexion/début phase action)
	private bool goSearch;
	// Booléen déterminant du retour ou non du survivant
	private bool comeBack;
	// Booléen activant la possibilité de retour physique du survivant (fin phase action/début phase réflexion)
	private bool returning;
	// Position du survivant
	Vector3 survivorPosition;

	// Use this for initialization
	void Start () {
		this.startPosition.transform.position = this.transform.position;
		this.progression = 0f;
		this.goSearch = false;
		this.comeBack = false;
		this.returning = false;
		this.survivorPosition = transform.position;
	}

	void OnSerializeNetworkView(BitStream stream){
		//Debug.Log ("ok");
		//stream.Serialize(ref i);
		stream.Serialize(ref  progression);
		stream.Serialize(ref  survivorPosition);
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate() {
		transform.position = survivorPosition;

		if (goSearch == true)
		{
			transform.position = Vector3.Lerp (transform.position, this.destination.transform.position, curve.Evaluate(progression));
			transform.rotation = Quaternion.Lerp (transform.rotation, this.destination.transform.rotation, curve.Evaluate (progression));
			progression += Time.deltaTime * 0.25f;
			
			if (phasesManager.startAction == false && this.comeBack == true)
			{
				transform.position = Vector3.Lerp (transform.position, this.startPosition.transform.position, curve.Evaluate (progression));
				transform.rotation = Quaternion.Lerp (transform.rotation, this.startPosition.transform.rotation, curve.Evaluate (progression));
				progression += Time.deltaTime * 0.25f;
			}
			else
			{
				transform.position = destination.transform.position;
			}
			goSearch = false;
		}

		survivorPosition = transform.position;
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
}

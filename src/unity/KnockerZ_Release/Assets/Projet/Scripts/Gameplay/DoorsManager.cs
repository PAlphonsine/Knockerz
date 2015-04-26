using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DoorsManager : MonoBehaviour
{
	// Porte
	public GameObject door;
	// Position de départ de la porte
	public Transform _startPosition;
	// Destination de la porte
	public Transform _destination;
	// Seconde porte
	public GameObject otherDoor;
	// Position de départ de la seconde porte
	public Transform _otherstartPosition;
	// Destination de la seconde porte
	public Transform _otherdestination;
	// Portée du Raycast
	float limiteDetection = 250.0f ;
	// Caméra
	public Camera _camera;
	// Gestion des phases
	public PhasesManager _phasesManager;
	// Booléen de controle d'ouverture de la porte
	[SerializeField] private bool opening = false;
	// Booléen de controle d'impossibilité de fermeture de la porte
	private bool cantClose = false;
	// Booléen de controle d'état fermé de la porte
	private bool isClosed = false;
	// Booléen de controle d'état ouvert de la porte
	private bool isOpened = false;
	// Points de vie de la porte
	[SerializeField] private int pv = 7500;
	// Barre de vie de la porte
	public GameObject lifeSprite;
	// Points de vie de départ de la porte
	private int startPv;
	// Zone déterminant qu'un objet est rentrée dans la base
	public GameObject inBase;
	//Pour savoir à qui appartient la porte
	[SerializeField] Transform separator;
	// Permet de gérer le temps de réaparition
	float timeReset = 0f;
	// Pour gérer l'evolution du chrono
	bool timeToResetDoor;
	// Permet de replacer le Inbase à sa position de départ
	[SerializeField] Transform posInBase;
	// Canvas d'affichage du texte de réaparition
	[SerializeField] GameObject timeCanvas;
	// Affiche le temps réstant avant la réaparion
	[SerializeField] Text timeText;
	// Pvs synchronisés
	int spv = 0;
	// Position des battants de la porte
	Vector3 posDoor;
	Vector3 posOtherDoor;
	// Cannaux de diffusion de sons
	[SerializeField] AudioSource soundOpening;
	[SerializeField] AudioSource soundClosing;

	void Start ()
	{
		// Pvs de départ pour la jauge de vie
		startPv = pv;
	}

	void FixedUpdate()
	{
		// Si la porte s'ouvre
		if (opening)
		{
			// On déplace la porte de sa position à sa destination
			door.transform.position = Vector3.Lerp(door.transform.position, _destination.position, 0.02f);
			// On déplace la seconde porte de sa position à sa destination
			otherDoor.transform.position = Vector3.Lerp(otherDoor.transform.position, _otherdestination.position, 0.02f);
		}
		// Sinon, si la porte peut se fermer
		else if (!cantClose)
		{
			// On déplace la porte de sa position à sa position de départ
			door.transform.position = Vector3.Lerp(door.transform.position, _startPosition.position, 0.02f);
			// On déplace la seconde porte de sa position à sa position de départ
			otherDoor.transform.position = Vector3.Lerp(otherDoor.transform.position, _otherstartPosition.position, 0.02f);
		}
	}

	void Update ()
	{
		// On détecte chaque objet sous le pointeur de la souris
		DetecterObjet ();
		
		// Si la distance entre la porte et sa destination et inférieure à 0.3
		if (Vector3.Distance (door.transform.position, _destination.position) < 0.3f)
			// La porte est considérée comme ouverte
			isOpened = true;
		else
			// Sinon, elle est considérée comme non ouverte
			isOpened = false;
		// Si la distance entre la porte et sa position de départ est inférieure ou égale à 0.3
		if (Vector3.Distance(door.transform.position, _startPosition.position) < 0.3f)
			// La porte est considérée comme fermée
			isClosed = true;
		else
			// Sinon, elle est considérée comme non fermée
			isClosed = false;
		
		// La barre de vie voit son échelle dépendre des points de vie de la porte
		lifeSprite.transform.localScale = new Vector3((float)pv/startPv,
		                                              lifeSprite.transform.localScale.y,
		                                              lifeSprite.transform.localScale.z);
		// Si les points de vie de la porte atteignent 0
		if (pv <= 0)
		{
			// La barre de vie est désactivée
			lifeSprite.SetActive(false);
			// La porte est désactivée
			door.SetActive(false);
			// La seconde porte est désactivée
			otherDoor.SetActive(false);
			// La zone de détectione des entités entrées dans la base se place à l'emplacement de la porte
			inBase.transform.position = _startPosition.position;
			// On déclanche le chrono de remise à zero
			timeToResetDoor = true;
		}

		// Si la porte doit etre remise à zero
		if (timeToResetDoor) {
			// On affiche le panneau du temps
			timeCanvas.SetActive(true);
			// On recalcul le temps restant
			timeReset+=Time.deltaTime;
			// On affiche le temps restant
			timeText.text = (180-(int)timeReset).ToString();
			// Quand on est arrivé à la fin du temps
			if(timeReset>180f)
				ResetDoor();
		}
	}

	// Méthode de réinitialisation de la porte
	void ResetDoor()
	{
		timeCanvas.SetActive(false);
		timeToResetDoor = false;
		timeReset = 0f;
		pv = 7500;
		startPv = 7500;
		lifeSprite.SetActive(true);
		// La porte est désactivée
		door.SetActive(true);
		// La seconde porte est désactivée
		otherDoor.SetActive(true);
		// La zone de détectione des entités entrées dans la base se place à l'emplacement de la porte
		inBase.transform.position = posInBase.position;
	}
	
	// Méthode de détectione d'objet
	void DetecterObjet()
	{
		// Lorsque le joueur clique et que l'on est en phase d'action
		if (Input.GetMouseButtonUp(0) && _phasesManager.startAction)
		{
			// On trace un rayon partant de la camera à la position du curseur
			Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
			// On instancie un point d'impact
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, limiteDetection))
			{
				// Si le rayon touche la première ou la seconde porte
				if (hit.collider.gameObject == door.gameObject || hit.collider.gameObject == otherDoor.gameObject)
				{
					// Si le joueur est le joueur 1 et qu'il a cliqué de son coté
					// ou si le joueur est le joueur 2 et qu'il a cliqué de son coté
					// ou si c'est le serveur
					if ((Network.player == _STATICS._networkPlayer[0] && hit.transform.position.x < separator.position.x) 
					    || (Network.player == _STATICS._networkPlayer[1] && hit.transform.position.x > separator.position.x)
					    || Network.isServer)
					{
						// Si la porte s'ouvre ou est ouverte
						if (opening && isOpened)
						{
							// La porte ne s'ouvre plus
							//opening = false;
							// On synchronise son état
							networkView.RPC("SynchroStates", RPCMode.AllBuffered, false);
						}
						// Sinon, si la porte ne peut pas se fermer
						// ou qu'elle ne s'ouvre pas et qu'elle est fermée
						else if (cantClose || (!opening && isClosed))
						{
							// La porte s'ouvre
							//opening = true;
							// On synchronise son état
							networkView.RPC("SynchroStates", RPCMode.AllBuffered, true);
						}
					}
				}
			}
		}
	}
	
	// RPC de controle de l'état de la porte
	[RPC]
	void SynchroStates(bool _opening)
	{
		opening = _opening;
		if(opening)
		// On joue un son
			soundOpening.Play();
		else
			soundClosing.Play();
	}
	
	// Méthode de synchronisation des paramètres de la porte
	void OnSerializeNetworkView(BitStream stream)
	{
		if (stream.isWriting && Network.isServer)
		{
			this.spv = this.pv;
			stream.Serialize (ref this.spv);
			this.posDoor = this.door.transform.position;
			stream.Serialize(ref this.posDoor);
			this.posOtherDoor = this.otherDoor.transform.position;
			stream.Serialize(ref this.posOtherDoor);
		}
		else if (stream.isReading && Network.isClient)
		{
			stream.Serialize (ref this.spv);
			this.pv = this.spv;
			stream.Serialize(ref this.posDoor);
			this.door.transform.position = this.posDoor;
			stream.Serialize(ref this.posOtherDoor);
			this.otherDoor.transform.position = this.posOtherDoor;
		}
	}
	
	// Accesseurs
	public int Pv
	{
		get { return this.pv; }
		set { this.pv = value; }
	}
	
	public bool CantClose
	{
		get { return this.cantClose; }
		set { this.cantClose = value; }
	}
}
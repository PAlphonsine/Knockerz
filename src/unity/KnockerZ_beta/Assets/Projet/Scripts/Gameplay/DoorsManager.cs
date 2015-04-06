using UnityEngine;
using System.Collections;

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
	private bool opening = false;
	// Booléen de controle d'impossibilité de fermeture de la porte
	private bool cantClose = false;
	// Booléen de controle d'état fermé de la porte
	private bool isClosed = false;
	// Booléen de controle d'état ouvert de la porte
	private bool isOpened = false;
	// Points de vie de la porte
	private int pv = 5000;
	// Barre de vie de la porte
	public GameObject lifeSprite;
	// Points de vie de départ de la porte
	private int startPv;
	// Zone déterminant qu'un objet est rentrée dans la base
	public GameObject inBase;
	//Pour savoir à qui appartient la porte
	[SerializeField]
	Transform separator;

	void Start ()
	{
		//Pvs de départ pour la jauge de vie
		startPv = pv;
	}

	void Update ()
	{
		// On détecte chaque objet sous le pointeur de la souris
		DetecterObjet ();
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
		}
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
							opening = false;
							// On synchronise son état
							networkView.RPC("SynchroStates", RPCMode.OthersBuffered, false);
						}
						// Sinon, si la porte ne peut pas se fermer
						// ou qu'elle ne s'ouvre pas et qu'elle est fermée
						else if (cantClose || (!opening && isClosed))
						{
							// La porte s'ouvre
							opening = true;
							// On synchronise son état
							networkView.RPC("SynchroStates", RPCMode.OthersBuffered, true);
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
	}
	
	// Méthode de synchronisation des paramètres de la porte
	void OnSerializeNetworkView(BitStream stream)
	{
		//int pv;
		//if (stream.isWriting) {
		//}
		stream.Serialize (ref this.pv);
		stream.Serialize (ref this.opening);
		stream.Serialize (ref this.cantClose);
		stream.Serialize (ref this.isOpened);
		stream.Serialize (ref this.isClosed);
	}
	
	// Accesseurs
	public int Pv
	{
		get { return this.pv; }
		set { this.pv = value;}
	}
	
	public bool CantClose
	{
		get { return this.cantClose; }
		set { this.cantClose = value;}
	}
}
using UnityEngine;
using System.Collections;

public class TutoDoorScript : MonoBehaviour 
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
	// Zone déterminant qu'un objet est rentrée dans la base
	public GameObject inBase;
	
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
				if(hit.collider.gameObject == door.gameObject || hit.collider.gameObject == otherDoor.gameObject)
				{
					if (opening && isOpened){
						opening = false;
					}else 
						if(cantClose || (!opening && isClosed)){
							opening = true;
					}
				}
			}
		}
	}

	public bool CantClose
	{
		get { return this.cantClose; }
		set { this.cantClose = value;}
	}
}
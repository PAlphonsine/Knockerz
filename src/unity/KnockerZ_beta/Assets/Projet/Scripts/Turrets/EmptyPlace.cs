using UnityEngine;
using System.Collections;

public class EmptyPlace : MonoBehaviour
{
	// Tourelles à distance et au corps-à-corps
	public GameObject _turretD;
	public GameObject _turretHtoH;
	public TurretAttackD _turretAttackD;
	public TurretAttackHtoH _turretAttackHtoH;
	// Menu de la tourelle
	public TurretMenuSet _turretMenuSet;
	// Booléen de gestion d'activation et de désactivation des menus
	bool hasClicked = false;
	bool canClickOut = false;
	//string tagObjet = "place" ; // Vérifiez bien que le gameObject possède le tag	//Plus obligatoire
	// Portée du Raycast
	float limiteDetection = 250.0f ; // Définir la limite de distance au delà de laquelle le clic n’est plus pris en compte
	// Caméra
	public Camera _camera;
	// Tourelle détectée
	Transform tmp; //transform de la tourelle détectée
	// Gestion des phases
	public PhasesManager _phasesManager;
	//Pour savoir à qui appartient la tourelle
	[SerializeField]
	Transform separator;


	void Update()
	{
		// On détecte en permanence si le joueur clique sur une tourelle
		DetecterObjet ();
	}
	
	// Méthode de détection de l'objet sur lequel le joueur clique
	void DetecterObjet()
	{
		// Si le joueur clique ailleurs que sur la tourelle
		if (Input.GetMouseButtonUp (0) && canClickOut == true)
		{
			// Le menu de la tourelle se désactive
			_turretMenuSet.DesactiveMenu ();
			// Les booléens de gestion du menu sont réinitialisés
			canClickOut = false;
			hasClicked = false;
		}
		// Sinon, si le joueur clique sur une tourelle
		else
		{
			// Si le joueur clique pendant la phase de réflexion
			if (Input.GetMouseButtonUp(0) && !_phasesManager.startAction)
			{
				// On trace un rayon qui passe par le centre de la caméra et la positon de la souris
				Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, limiteDetection))
				{ 
					//Le rayon est lancé. Sa taille sera égale à  limiteDetection. Les objets en contact avec le rayon "ray" sont stockés dans la variable hit.
					//Si le joueur a cliqué sur cette tourelle
					if (hit.collider.gameObject == this.gameObject)
					{
						// Si le joueur est bien le joueur qui possède la tourelle sur laquelle il a cliqué
						if ((Network.player == _STATICS._networkPlayer[0] && hit.transform.position.x < separator.position.x) 
						    || (Network.player == _STATICS._networkPlayer[1] && hit.transform.position.x > separator.position.x))
						{
							// Si le joueur n'a pas cliqué sur la tourelle
							if (hasClicked == false)
							{
								// Le menu de la tourelle s'active
								_turretMenuSet.ActiveMenu ();
								// Le joueur peut cliquer ailleurs
								canClickOut = true;
								// Le joueur a cliqué sur la tourelle
								hasClicked = true;
							}
							else
							{
								hasClicked = false;
							}
							// La tourelle dont le menu s'active est la tourelle sur laquelle le joueur a cliqué
							tmp = hit.transform;
						}
					}
				}
			}
		}
		// Si le joueur clique ailleurs que sur la tourelle
		if (Input.GetMouseButtonUp(0) && hasClicked == false && canClickOut == false)
		{
			// Si il y avait une tourelle dont le menu était ouvert auparavant
			if (tmp != null)
				// Le menu de cette tourelle se désactive
				_turretMenuSet.DesactiveMenu();
			// Le joueur ne peut plus cliquer ailleurs pour fermer le menu car aucun menu n'est ouvert
			canClickOut = false;
		}
	}
	
	// RPC de synchronisation du menu
	[RPC]
	public void DoSynchro(int mode)
	{
		// On active le menu de la tourelle
		_turretMenuSet.ActiveMenu ();
		// On vérifie si le joueur décide d'acheter une tourelle ou de l'améliorer
		_turretMenuSet.menus[0].GetComponent<TurretMenu>().ClientWantToBuy(mode);
	}
}
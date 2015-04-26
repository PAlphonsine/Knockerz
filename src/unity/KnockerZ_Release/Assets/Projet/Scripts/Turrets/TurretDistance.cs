using UnityEngine;
using System.Collections;

public class TurretDistance: MonoBehaviour 
{
	// Script de gestion du type de mission
	[SerializeField]
	MissionsButtonScript missionButtonScript;
	// Panel du menu
	[SerializeField]
	GameObject sabotageCanvas;
	// Booléen de controle d'envoi d'un sabotage précédemment
	private bool alreadyPlanned;
	// Menu de la tourelle
	public TurretMenuSet _turretMenuSet;
	// Booléen de gestion d'activation et de désactivation des menus
	bool hasClicked = false;
	bool canClickOut = false;
	//string tagObjet = "place" ; // Vérifiez bien que le gameObject possède le tag	//Plus obligatoire
	// Portée du Raycast
	float limiteDetection = 250.0f ; // Définir la limite de distance au delà de laquelle le clic n’est plus prit en compte
	// Caméra
	public Camera _camera;
	// Tourelle détectée
	Transform tmp; //transform de la tourelle détécté
	// Niveau de la tourelle
	[SerializeField]
	int nivTurret = 1;
	// Gestion des phases
	public PhasesManager _phasesManager;
	//Pour savoir à qui appartient la tourelle
	[SerializeField]
	Transform separator;
	// Matériaux dépensés
	public int matSpent;
	// Armes dépensées
	public int weapSpent;
	// Pop dépensés
	public int popSpent;
	// La spécialisation de la tourelle
	public int spec = 0;
	// Système de particules activé lors de l'amélioration de la tourelle
	[SerializeField] ParticleSystem ameliorationParticles;
	
	void Update()
	{
		// On détecte en permanence si le joueur clique sur la tourelle
		DetecterObjet ();
		if (this._phasesManager.startAction == true)
		{
			this.alreadyPlanned = false;
		}
	}
	
	// Méthode de détection de l'objet sur lequel le joueur clique
	void DetecterObjet()
	{
		// Si le joueur clique ailleurs que sur la tourelle
		if (Input.GetMouseButtonUp (0) && canClickOut == true)
		{
			// Le menu de la tourelle se désactive
			_turretMenuSet.DesactiveMenu ();
			_turretMenuSet.DesactiveSpe ();
			// Les booléens de gestion du menu sont réinitialisés
			canClickOut = false;
			hasClicked = false;
		}
		// Sinon, si le joueur clique sur une tourelle
		else
		{
			// Si le joueur clique pendant la phase de réflexion et qu'il n'y a pas de mode de mission sélectionné
			if(Input.GetMouseButtonUp(0) && !_phasesManager.startAction && this.missionButtonScript.MissionMode == "")
			{
				// On trace un rayon qui passe par le centre de la caméra et la positon de la souris
				Ray ray = _camera.ScreenPointToRay(Input.mousePosition); // Droite (rayon) qui passe par le centre de la caméra et la positon de la souris
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
								if (nivTurret == 4)
									_turretMenuSet.ActiveSpe ();
								else
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
			if (tmp != null){
				// Le menu de cette tourelle se désactive
				_turretMenuSet.DesactiveMenu();
				_turretMenuSet.DesactiveSpe();
			}
			// Le joueur ne peut plus cliquer ailleurs pour fermer le menu car aucun menu n'est ouvert
			canClickOut = false;
		}
	}
	
	// Méthode d'augmentation du niveau de la tourelle
	public void LevelUpTurret()
	{
		// On incrémente le niveau de la tourelle
		nivTurret = nivTurret + 1;
	}
	
	// Méthode d'ouverture du menu de sabotage d'une tourelle
	public void OpenSabotageMenu()
	{
		if (this.alreadyPlanned == false)
		{
			// Si le joueur clique sur une tourelle de l'adversaire
			if((Network.player == _STATICS._networkPlayer[0] && this.transform.position.x > separator.position.x) || (Network.player == _STATICS._networkPlayer[1] && this.transform.position.x < separator.position.x))
			{
				// Si le mode de mission précedemment sélectionné dans le menu de Missions est le mode Sabotage
				if (this.missionButtonScript.MissionMode == "Sabotage")
				{
					// Le menu sabotage de la tourelle est activé
					this.sabotageCanvas.gameObject.SetActive(true);
				}
			}
		}
		// Le mode de mission est réinitialisé à aucun
		this.missionButtonScript.MissionMode = "";
	}

	// Méthode RPC de confirmation de sabotage d'une tourelle
	[RPC]
	void ConfirmSabotage(float deathRate, float successRate)
	{
		this.sabotageCanvas.SetActive (true);
		this.sabotageCanvas.GetComponent<SabotageMissionsScript> ().AddSabotage (deathRate, successRate);
	}
	
	// RPC de synchronisation des demandes de vente ou d'achat
	// Les fonctions RPC ne prennent pas de paramètre facultatif
	[RPC]
	public void DoSynchroD(int mode, int spe)
	{
		// Si la tourelle est en mode achat
		if (mode == 1)
		{
			// Si on est passé à une des deux spécialisation
			if (spe == 1 || spe == 2)
			{
				// On active le menu de spécialisation
				_turretMenuSet.ActiveSpe();
				// On engage la procedure d'achat de la tourelle
				_turretMenuSet.spes[spe].GetComponent<TurretMenuD>().ClientWantToBuy(spe);
			}
			else 
			{
				// Sinon on active le menu de la tourelle
				_turretMenuSet.ActiveMenu();
				// On engage la procedure d'achat de la tourelle
				_turretMenuSet.menus[0].GetComponent<TurretMenuD>().ClientWantToBuy(spe);
			}
		}
		// Si la tourelle est en mode vente
		if(mode == 2)
		{
			// On active le menu de la tourelle
			_turretMenuSet.ActiveMenu ();
			// On engage la procedure de vente de la tourelle
			_turretMenuSet.menus[0].GetComponent<TurretMenuD>().ClientWantToSell();
		}
	}

	//Accesseurs

	public int NivTurret {
		get {
			return nivTurret;
		}
		set {
			nivTurret = value;
		}
	}

	public ParticleSystem AmeliorationParticles
	{
		get { return this.ameliorationParticles; }
		set { this.ameliorationParticles = value; }
	}
	
	public bool AlreadyPlanned
	{
		get { return this.alreadyPlanned; }
		set { this.alreadyPlanned = value; }
	}
}
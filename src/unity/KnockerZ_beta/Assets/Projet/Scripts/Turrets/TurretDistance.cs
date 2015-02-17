using UnityEngine;
using System.Collections;

public class TurretDistance: MonoBehaviour {

	//Pour acceder au menu de la tourelle
	public TurretMenuSet _turretMenuSet;
	
	//Pour pouvoir activer les menus
	bool hasClicked = false;
	bool canClickOut = false;
	
	//Detection par raycast
	//string tagObjet = "place" ; // Vérifiez bien que le gameObject possède le tag	//Plus obligatoire
	float limiteDetection = 250.0f ; // Définir la limite de distance au delà de laquelle le clic n’est plus prit en compte
	public Camera _camera;
	Transform tmp; //transform de la tourelle détécté

	public int NivTurret = 1;

	public PhasesManager _phasesManager;

	public void LevelUpTurret(){
		NivTurret =  (NivTurret+1) %3;
	}
	
	void Update(){
		DetecterObjet ();
	}
	
	void DetecterObjet(){
		//Si on clique autre part que sur la tourelle
		if (Input.GetMouseButtonUp (0) && canClickOut == true) {
			_turretMenuSet.DesactiveMenu ();
			canClickOut = false;
			hasClicked = false;
		}else{
			//Sinon on regarde si on a cliqué sur la tourelle
			if(Input.GetMouseButtonUp(0) && !_phasesManager.startAction){
				Ray ray = _camera.ScreenPointToRay(Input.mousePosition); // Droite (rayon) qui passe par le centre de la caméra et la positon de la souris
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, limiteDetection)){ //Le rayon est lancé. Sa taille sera égale à  limiteDetection. Les objets en contact avec le rayon "ray" sont stockés dans la variable hit.
					//Si on a cliqué sur cette tourelle
					if(/*important*/hit.collider.gameObject == this.gameObject){
						//Si on est bien le joueur qui possède la tourelle
						if((Network.player == _STATICS._networkPlayer[0] && hit.transform.position.x < 0) 
						   || (Network.player == _STATICS._networkPlayer[1] && hit.transform.position.x > 0)){
							if (hasClicked == false) {
								_turretMenuSet.ActiveMenu ();
								canClickOut = true;
								hasClicked = true;
							}
							else {
								hasClicked = false;
							}
							tmp = hit.transform;
						}
					}
				}
			}
		}
		if (Input.GetMouseButtonUp(0) && hasClicked == false && canClickOut == false) {
			if(tmp!=null)
				_turretMenuSet.DesactiveMenu();
			canClickOut = false;
		}
	}

	[RPC]
	public void DoSynchro(int mode){
		if(mode ==1){
			_turretMenuSet.ActiveMenu ();
			_turretMenuSet.tabs[0].GetComponent<TurretMenuD>().ClientWantToBuy();
		}
		if(mode == 2){
			_turretMenuSet.ActiveMenu ();
			_turretMenuSet.tabs[0].GetComponent<TurretMenuD>().ClientWantToSell();
		}
	}
}
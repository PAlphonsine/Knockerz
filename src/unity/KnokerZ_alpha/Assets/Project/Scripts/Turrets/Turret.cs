using UnityEngine;
using System.Collections;

public class Turret: MonoBehaviour {

	string tagObjet = "cube" ; // Vérifiez bien que le gameObject possède le tag
	float limiteDetection = 250.0f ; // Définir la limite de distance au delà de laquelle le clic n’est plus prit en compte
	Transform tmp;
	bool hasClicked = false;
	bool canClickOut = false;

	public GameObject fighter;

	void Update(){
		DetecterObjet ();
	}

	void DetecterObjet(){
		if (Input.GetMouseButtonUp (0) && canClickOut == true) {
			tmp.GetComponent<TurretMenuSet> ().DesactiveMenu ();
			canClickOut = false;
			hasClicked = false;
		}
		else {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); // Droite (rayon) qui passe par le centre de la caméra et la positon de la souris
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, limiteDetection)) { 
				//Le rayon est lancé. Sa taille sera égale à  limiteDetection. Les objets en contact avec le rayon "ray" sont stockés dans la variable hit.
				if (hit.transform.CompareTag (tagObjet) && Input.GetMouseButtonUp (0)) {
					if((Network.player == _STATICS._networkPlayer[0] && hit.transform.position.x < 0) 
					   || (Network.player == _STATICS._networkPlayer[1] && hit.transform.position.x > 0)){
						//Si le tag correspond, faites ce que vous voulez
						//Debug.Log("Coordonnées de la souris sur l’objet = " + hit.point ) ; // La variable “hit.point” (Vector3) contient  les coordonnés
						Debug.Log (hit.transform.name);
						//hit.transform.position = new Vector3(0,0,0);
						if (hasClicked == false) {
							//hit.transform.GetComponent<TurretMenuSet>().enabled = true;
							hit.transform.GetComponent<TurretMenuSet> ().ActiveMenu ();
							canClickOut = true;
							//Debug.Log("ooo");
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
		
		if (Input.GetMouseButtonUp(0) && hasClicked == false && canClickOut == false) {
			//Debug.Log("kkkkk");
			if(tmp!=null)
			tmp.GetComponent<TurretMenuSet>().DesactiveMenu();
			canClickOut = false;
			//hit.transform.GetComponent<TurretMenuSet>().enabled = false;
		}
	}
}

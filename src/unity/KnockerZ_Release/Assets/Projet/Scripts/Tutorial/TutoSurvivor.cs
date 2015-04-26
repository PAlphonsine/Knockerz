using UnityEngine;
using System.Collections;

public class TutoSurvivor : MonoBehaviour 
{
	// NavMesh
	public NavMeshAgent agent;
	// Destination de son NavMesh
	public Transform destination;
	// Indique que le survivant est arrivé à destination
	public bool tutoFini;
	
	void Start () {
		// Application de la position de départ
		agent.SetDestination (destination.position);
	}

	void Update () {
		// Calcul de sa distance à l'arrivée
		if (Vector3.Distance (transform.position, agent.destination) < 1f) {
			tutoFini = true;
			// Réduction de sa vitesse en vu de la prochaine partie du tuto
			agent.speed = 2;
		}
	}

	// Le surivant s'arrete s'il collisionne avec une porte
	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Door")
			agent.Stop();
	}

	// Il reprend sa route quand le joueur ouvre la porte (ou le survivant touche un zombie)
	void OnTriggerExit(Collider collider){
			agent.Resume();
	}
}

using UnityEngine;
using System.Collections;

public class ChangeDestination : MonoBehaviour
{
	// Transform de la nouvelle destination
	[SerializeField] Transform newDestination;
	// Booléen de décision de changement de direction
	[SerializeField] bool canChangeDirection = true;

	// Méthode déclenchée lorsqu'un collider déclenche le trigger de l'objet
	void OnTriggerEnter(Collider collider)
	{
		// Si le changement de direction est possible
		if (canChangeDirection)
		{
			// Si le float aléatoire calculé est supérieur ou égal à 0.5
			if (Random.Range(0.0f, 1.0f) >= 0.5f)
			{
				// Si c'est un Zombie
				if (collider.tag == "Zombie")
					// On lui applique la nouvelle destination
					collider.GetComponent<ZombieScript> ().Destination = newDestination;
				// Si c'est un Survivant
				if (collider.tag == "Survivor")
					// On lui applique la nouvelle destination
					collider.GetComponent<SurvivorScript> ().Destination = newDestination;
			}
		}
		// Sinon, si le changement de direction n'est pas possible
		else
		{
			// Si c'est un Zombie
			if (collider.tag == "Zombie")
				// Il continue vers la base
				collider.GetComponent<ZombieScript> ().Destination = newDestination;
			// Si c'est un Survivant
			if (collider.tag == "Survivor")
				// Il continue vers la base
				collider.GetComponent<SurvivorScript> ().Destination = newDestination;
		}
	}
}
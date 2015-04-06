using UnityEngine;
using System.Collections;

public class ChangeDestination : MonoBehaviour
{
	[SerializeField] PhasesManager PhasesManager;
	[SerializeField] Transform newDestination;
	[SerializeField] bool canChangeDirection = true;
	private bool meeting = true;
	
	void OnTriggerEnter(Collider collider)
	{
		if (canChangeDirection)
		{
			if (meeting == true)
			{
				meeting = false;
				if (collider.tag == "Zombie")
					collider.GetComponent<ZombieScript> ().Destination = newDestination;
				if (collider.tag == "Survivor")
					collider.GetComponent<SurvivorScript> ().Destination = newDestination;
			}
			else
			{
				meeting = true;
			}
		}
		else
		{
			if (collider.tag == "Zombie")
				collider.GetComponent<ZombieScript> ().Destination = newDestination;
			if (collider.tag == "Survivor")
				collider.GetComponent<SurvivorScript> ().Destination = newDestination;
		}
	}
}

/*public class ChangeDestination : MonoBehaviour
{
	[SerializeField] Transform newDestination;
	[SerializeField] Transform baseDestination;

	void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Zombie")
		{
			if (collider.GetComponent<ZombieScript> ().ChangeDirection == false)
			{
				if (Random.Range (0.0f, 1.0f) < 0.5f)
				{
					collider.GetComponent<ZombieScript> ().Destination = newDestination;
					collider.GetComponent<ZombieScript> ().ChangeDirection = true;
				}
			}
			else
			{
				collider.GetComponent<ZombieScript> ().Destination = baseDestination;
				collider.GetComponent<ZombieScript> ().ChangeDirection = false;
			}
		}

		if (collider.tag == "Survivor")
		{
			if (collider.GetComponent<SurvivorScript> ().ChangeDirection == false)
			{
				if (Random.Range (0.0f, 1.0f) < 0.5f)
				{
					collider.GetComponent<SurvivorScript> ().Destination = newDestination;
					collider.GetComponent<SurvivorScript> ().ChangeDirection = true;
				}
			}
			else
			{
				collider.GetComponent<SurvivorScript> ().Destination = baseDestination;
				collider.GetComponent<SurvivorScript> ().ChangeDirection = false;
			}
		}
	}
}
*/
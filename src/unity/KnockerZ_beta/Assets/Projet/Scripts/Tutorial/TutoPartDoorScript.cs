﻿using UnityEngine;
using System.Collections;

public class TutoPartDoorScript : MonoBehaviour
{
	// Manager des portes
	[SerializeField]
	private TutoDoorScript _doorsManager;
	// Points de vie des portes
	private int pv;
	
	// Lorsqu'un objet entre dans le collider de la porte
	void OnTriggerEnter(Collider collider)
	{
		// Si c'est un Zombie ou un Survivant
		if (collider.tag == "Zombie" || collider.tag == "Survivor")
		{
			// La porte ne peut pas se fermer
			_doorsManager.CantClose = true;
		}
	}
	
	// Lorsqu'un objet quitte le collider de la porte
	void OnTriggerExit(Collider collider)
	{
		// Si c'est un Zombie ou un Survivant
		if (collider.tag == "Zombie" || collider.tag == "Survivor")
		{
			// La porte peut se fermer
			_doorsManager.CantClose = false;
		}
	}
}
using UnityEngine;
using System.Collections;

public class TurretMenuSet : MonoBehaviour
{
	// Boutons du menu
	public GameObject[] menus;
	// Boutons de specialisation
	public GameObject[] spes;
	
	// Méthode d'activation du menu
	public void ActiveMenu()
	{
		// Pour chaque bouton du menu
		foreach (GameObject n in menus)
		{
			// On active le bouton
			n.SetActive(true);
		}
	}

	// Méthode d'activation des spes
	public void ActiveSpe()
	{
		// Pour chaque bouton des spes
		foreach (GameObject n in spes)
		{
			// On active le bouton
			n.SetActive(true);
		}
	}
	
	// Méthode de désactivation du menu
	public void DesactiveMenu()
	{
		// Pour chaque bouton du menu
		foreach (GameObject n in menus)
		{
			// On désactive le bouton
			n.SetActive(false);
		}
	}

	// Méthode de désactivation des spes
	public void DesactiveSpe()
	{
		// Pour chaque bouton des spes
		foreach (GameObject n in spes)
		{
			// On désactive le bouton
			n.SetActive(false);
		}
	}
}

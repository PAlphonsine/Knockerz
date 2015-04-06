using UnityEngine;
using System.Collections;

public class TurretAttackHtoH : MonoBehaviour
{
	// Script de gestion du fighter
	[SerializeField]
	FighterScript _fighterScript;
	// Objet fighter
	[SerializeField]
	GameObject fighter;
	// Variable pour lancer la coroutine une seule fois
	bool mustWait = false;

	// Mise à jour des caractéristiques du fighter
	public void UpdateFighter(int dam, int pv)
	{
		// Pour les dommages
		_fighterScript.Dps = dam;
		// Pour les points de vie courants
		_fighterScript.Pv = pv;
		// Pour les points de vie actuels
		_fighterScript.InitPv = pv;
		// Ppour la barre de vie
		_fighterScript.StartPv = pv;
	}

	// Mise à jour de la stat d'esquive
	public void UpdateDodgeFighter(float dodge)
	{
		_fighterScript.Dodge = dodge;
	}

	// mIse à jour de la stat de coups critiques
	public void UpdateCriticalHitsFighter(float critic)
	{
		_fighterScript.CriticalHits = critic;
	}

	void Update()
	{
		// Si le fighter est activé et que l'on peut lancer la coroutine
		if (!fighter.activeSelf && !mustWait){
			// On lance la coroutine d'attente
			StartCoroutine(WaitAndReset());
			// On ne peut pas relancer la fonction
			mustWait = true;
		}
	}

	// coroutine d'attente
	IEnumerator WaitAndReset()
	{
		// On rend la main à Unity quelques secondes
		yield return new WaitForSeconds (8f);
		// A la on active le fighter
		fighter.SetActive (true);
		// On peut relancer la fonction
		mustWait = false;
	}

	// Accesseurs

	public void EnableFighter()
	{
		fighter.SetActive (true);
	}
	
	public bool MustWait 
	{
		get {
			return mustWait;
		}
		set {
			mustWait = value;
		}
	}
}

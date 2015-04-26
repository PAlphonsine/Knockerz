using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinalSabotageScript : MonoBehaviour
{
	// Gestion des phases
	[SerializeField] PhasesManager phasesManager;
	// Tableau des sabotages prévus
	private List<SabotageMissionsScript> sabotages = new List<SabotageMissionsScript> ();
	// Compteur de sabotage
	private int sabotageCount;
	// Tableau de saboteurs en mission durant la vague en cours
	[SerializeField] SabotageManScript[] sabotageMen;
	// Compteur de saboteurs
	private int sabotageMenCount;
	// Booléen de controle d'envoi des saboteurs
	private bool sabotageMenSent;
	// Booléen de déclenchemet de l'envoi du saboteur
	private bool sabotaging;
	// Booléen d'état : le saboteur est mort ou pas
	private bool dead;
	// Booléen d'état : le saboteur a réussi sa mission ou pas
	private bool success;
	// Chance de survie ou de réussite du saboteur
	private int chance;
	// Booléen de controle de reset
	private bool hasBeenReseted;
	
	// Use this for initialization
	void Start ()
	{
		this.sabotageMenCount = -1;
		this.sabotageMenSent = false;
		this.sabotaging = false;
		this.dead = false;
		this.success = false;
		this.chance = 0;
		this.hasBeenReseted = true;
	}
	
	void FixedUpdate ()
	{
		// Si l'on est en phase d'action
		if (this.phasesManager.startAction == true)
		{
			// Si les saboteurs n'ont pas encore été envoyés
			if (this.sabotageMenSent == false && this.sabotageMenCount != -1)
			{
				// Pour chaque sabotage planifié durant la phase de réflexion précédente
				foreach (SabotageMissionsScript sabotage in this.sabotages)
				{
					// On active le saboteur
					this.sabotageMen[sabotageMenCount].gameObject.SetActive(true);
					// On calcule son risque de mourir
					this.sabotageMen[sabotageMenCount].Dead = this.DeathCalculate(sabotage);
					// On calcule ses chances de réussir
					this.sabotageMen[sabotageMenCount].Success = this.SuccessCalculate(sabotage);
					// On lui assigne comme destination la tourelle ciblée
					this.sabotageMen[sabotageMenCount].Destination.transform.position = sabotage.TargetedTurret.transform.position;
					// On lui assigne la tourelle ciblée
					this.sabotageMen[sabotageMenCount].TargetedTurret = sabotage.TargetedTurret;
					// Le saboteur peut aller saboter
					this.sabotageMen[sabotageMenCount].CanGoSabotage = true;
					//Debug.Log("FSS/Réussite de la mission : " + this.sabotageMen[sabotageMenCount].Success);
					//Debug.Log("FSS/Mort du Saboteur : " + this.sabotageMen[sabotageMenCount].Dead);
					// Le saboteur n'a pas été réinitialisé
					sabotage.HasBeenReseted = false;
					// On passe au saboteur suivant
					this.sabotageMenCount++;
				}			
				this.sabotageMenSent = true;
			}

			// Le script de planification des sabotages n'a pas été réinitialisé
			this.hasBeenReseted = false;
		}
		// Sinon, si l'on est en phase de réflexion
		else
		{
			// Si le script de planification des sabotages n'a pas été réinitialisé
			if (this.hasBeenReseted == false)
			{
				// On le réinitialise
				this.Reset();
			}
		}
	}
	
	// Méthode de calcul des chances de survie du saboteur
	public bool DeathCalculate(SabotageMissionsScript sabotage)
	{
		// On génère un nombre aléatoire entre 0 et 100
		this.chance = Random.Range (0, 100);

		// Si ce nombre est inférieur au pourcentage de danger du sabotage
		if (this.chance <= sabotage.DeathRate)
		{
			// Le saboteur meurt
			this.dead = true;
		}
		else
		{
			// Sinon, le saboteur reste en vie
			this.dead = false;
		}

		// On renvoie la valeur
		return this.dead;
	}
	
	// Méthode de calcul des chances de réussite du saboteur
	public bool SuccessCalculate(SabotageMissionsScript sabotage)
	{
		// On génère un nombre aléatoire entre 0 et 100
		this.chance = Random.Range (0, 100);
		
		// Si ce nombre est inférieur au pourcentage de réussite du sabotage
		if (this.chance <= sabotage.SuccessRate)
		{
			// Le saboteur réussit son sabotage
			this.success = true;
		}
		else
		{
			// Sinon, le saboteur échoue
			this.success = false;
		}
		
		// On renvoie la valeur
		return this.success;
	}
	
	// Méthode de réinitialisation des paramètres
	public void Reset()
	{
		this.sabotageMenCount = -1;
		this.sabotageMenSent = false;
		this.sabotaging = false;
		this.dead = false;
		this.success = false;
		this.chance = 0;
		this.sabotages = new List<SabotageMissionsScript> ();
		this.hasBeenReseted = true;
	}
	
	// Accesseurs
	public List<SabotageMissionsScript> Sabotages
	{
		get { return this.sabotages; }
		set { this.sabotages = value; }
	}
	
	public int SabotageCount
	{
		get { return this.sabotageCount; }
		set { this.sabotageCount = value; }
	}
	
	public int SabotageMenCount
	{
		get { return this.sabotageMenCount; }
		set { this.sabotageMenCount = value; }
	}
	
	public bool SabotageMenSent
	{
		get { return this.sabotageMenSent; }
		set { this.sabotageMenSent = value; }
	}
	
	public bool Sabotaging
	{
		get { return this.sabotaging; }
		set { this.sabotaging = value; }
	}
	
	public bool Dead
	{
		get { return this.dead; }
		set { this.success = value; }
	}
	
	public bool Success
	{
		get { return this.success; }
		set { this.success = value; }
	}
	
	public int Chance
	{
		get { return this.chance; }
		set { this.chance = value; }
	}
	
	public bool HasBeenReseted
	{
		get { return this.hasBeenReseted; }
		set { this.hasBeenReseted = value; }
	}
}
using UnityEngine;
using System.Collections;

public class ReturningSurvivors : MonoBehaviour
{
	// Journal d'évènements
	[SerializeField]
	EventsNewspaperScript eventsNewspaperScript;
	// Liste des Survivants envoyés aux deux ressources
	[SerializeField] public SentSurvivorScript[] sentSurvivorsMaterials;
	[SerializeField] public SentSurvivorScript[] sentSurvivorsWeapons;
	// Ressources trouvées
	private int foundRessources;
	// "Pourcentage" influant sur le nombre de ressources rapportées par Survivants
	private float ressourceChance;
	// "Pourcentage" influant sur les chances de retour selon le nombre de Survivants envoyés
	private float chanceMaterials;
	private float chanceWeapons;
	// Booléen disant au premier Survivant de toujours revenir
	private bool firstOneAlwaysComeBackForMat;
	private bool firstOneAlwaysComeBackForWeap;
	// Booléen de controle du calcul déjà fait ou non
	private bool calculated;
	// Détection des phases
	[SerializeField]
	PhasesManager phasesManager;
	#region Tests
	bool survivorsReturning; // retour des Survivants
	private int countMaterials = 0; // compteur de Survivants revenus pour les matériaux
	private int countWeapons = 0; // compteur de Survivants revenus pour les armes
	private bool survivorsSent = false;
	#endregion
	
	// Use this for initialization
	void Start ()
	{
		// Les Survivants ramènent minimum 15 de ressources
		this.foundRessources = 15;
		// Le pourcentage de chance influant sur le gain de ressources est de 10%
		this.ressourceChance = 0.1f;
		// Le pourcentage de chance influant sur le retour des Survivants est de 10%
		this.chanceMaterials = 0.05f;
		this.chanceWeapons = 0.05f;
		// Il y a toujours minimum 1 Survivant qui revient
		this.firstOneAlwaysComeBackForMat = true;
		this.firstOneAlwaysComeBackForWeap = true;
		// Détermine si le calcul des chances de retour et de ressources a été fait
		this.calculated = true;
		this.survivorsSent = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Quand on est en phase d'action
		if (phasesManager.startAction == true)
		{
			// Les chances de retour varient selon le nombre de Survivants envoyés
			this.chanceMaterials = 0.05f * this.sentSurvivorsMaterials.Length;
			this.chanceWeapons = 0.05f * this.sentSurvivorsWeapons.Length;
			if (this.survivorsSent == false)
			{
				// Pour chaque Survivant prévu
				foreach (SentSurvivorScript survivor in this.sentSurvivorsMaterials)
				{
					// On déclenche son animation de sortie
					survivor.GoSearch = true;
					if (survivor.GoSearch == true)
					{
						this.survivorsSent = true;
					}
				}
				// Pour chaque Survivant prévu
				foreach (SentSurvivorScript survivor in this.sentSurvivorsWeapons)
				{
					// On déclenche son animation de sortie
					survivor.GoSearch = true;
					if (survivor.GoSearch == true)
					{
						this.survivorsSent = true;
					}
				}
			}
			// Ses chances de retour et son nombre de ressources n'a pas encore été calculé
			calculated = false;
		}

		// Quand on revient en phase de réflexion et que rien n'a été calculé
		if (calculated == false && phasesManager.startAction == false)
		{
			// Minimum un revenant
			this.firstOneAlwaysComeBackForMat = true;
			this.firstOneAlwaysComeBackForWeap = true;
			// Compteur de Survivants revenus
			countMaterials = 0;
			countWeapons = 0;
			// Pour chaque Survivant envoyé aux matériaux
			foreach (SentSurvivorScript survivor in this.sentSurvivorsMaterials)
			{
				// Si c'est le premier
				if (this.firstOneAlwaysComeBackForMat == true)
				{
					// Il revient toujours
					survivor.ComeBack = true;
					this.firstOneAlwaysComeBackForMat = false;
					countMaterials++;
					// Calcul des ressources qu'il rammène
					GameStats.Instance.RessourcesMat += HowManyToCarry();
					// Ajout à la population
					GameStats.Instance.Population++;
					//Debug.Log ("RS/Ajout de Survivants : retour matériaux - " + GameStats.Instance.Population);
					continue;
				}
				// Détermination de la survie du Survivant ou non
				survivor.ComeBack = KilledOrNotKilled ("Materials");
				// S'il survit
				if (survivor.ComeBack == true)
				{
					countMaterials++;
					// Calcul des ressources qu'il rammène
					GameStats.Instance.RessourcesMat += HowManyToCarry();
					// Ajout à la population
					GameStats.Instance.Population++;
					//Debug.Log ("RS/Ajout de Survivants : retour matériaux - " + GameStats.Instance.Population);
				}
				this.eventsNewspaperScript.MaterialsSurvivorsBack = this.countMaterials;
			}

			// Pour chaque Survivant envoyé aux armes
			foreach (SentSurvivorScript survivor in this.sentSurvivorsWeapons)
			{
				// Si c'est le premier
				if (this.firstOneAlwaysComeBackForWeap == true)
				{
					// Il revient toujours
					survivor.ComeBack = true;
					this.firstOneAlwaysComeBackForWeap = false;
					countWeapons++;
					// Calcul des ressources qu'il rammène
					GameStats.Instance.RessourcesWeap += HowManyToCarry();
					// Ajout à la population
					GameStats.Instance.Population++;
					//Debug.Log ("RS/Ajout de Survivants : retour armes - " + GameStats.Instance.Population);
					continue;
				}
				// Détermination de la survie du Survivant ou non
				survivor.ComeBack = KilledOrNotKilled ("Weapons");
				// S'il survit
				if (survivor.ComeBack == true)
				{
					countWeapons++;
					// Calcul des ressources qu'il rammène
					GameStats.Instance.RessourcesWeap += HowManyToCarry();
					// Ajout à la population
					GameStats.Instance.Population++;
					//Debug.Log ("RS/Ajout de Survivants : retour armes - " + GameStats.Instance.Population);
				}
				this.eventsNewspaperScript.WeaponsSurvivorsBack = this.countWeapons;
			}
			//Debug.Log ("Revenus matériaux : " + countMaterials);
			//Debug.Log ("Revenus armes : " + countWeapons);
			//Debug.Log(this.countMaterials);
			//Debug.Log(this.countWeapons);
			countMaterials = 0;
			countWeapons = 0;
			// Tout a été calculé pour tout le monde
			calculated = true;
		}
	}
	
	private int HowManyToCarry()
	{
		// Nombre aléatoire entre 5 et 15 multiplié par les chance de trouver des ressources
		float richOrNot = Random.Range (5, 15) * ressourceChance;
		// Multiplication par 30 (résultat entre 15 et 45)
		this.foundRessources = (int)(30 * richOrNot);
		
		return foundRessources;
	}
	
	private bool KilledOrNotKilled(string type)
	{
		if (type == "Materials")
		{
			// Chiffre aléatoire entre 0.0 et 1.0 + le pourcentage de chance (de 0.1*nbSurvivantsEnvoyés)
			float returningOrNot = Random.Range (0f, 1f) + chanceMaterials;

			// Si le résultat est supérieur à 1.2
			if (returningOrNot > 1.2f)
			{
				// Le Survivant revient
				return true;
			}
			else
			{
				// Sinon non
				return false;
			}
		}

		if (type == "Weapons")
		{
			// Chiffre aléatoire entre 0.0 et 1.0 + le pourcentage de chance (de 0.1*nbSurvivantsEnvoyés)
			float returningOrNot = Random.Range (0f, 1f) + chanceWeapons;

			// Si le résultat est supérieur à 1.2
			if (returningOrNot > 1.2f)
			{
				// Le Survivant revient
				return true;
			}
			else
			{
				// Sinon non
				return false;
			}
		}

		return false;
	}
	
	// Accesseurs
	public int FoundRessources
	{
		get { return this.foundRessources; }
		set { this.foundRessources = value; }
	}
	
	public float RessourceChance
	{
		get { return this.ressourceChance; }
		set { this.ressourceChance = value; }
	}
	
	public float ChanceMaterials
	{
		get { return this.chanceMaterials; }
		set { this.chanceMaterials = value; }
	}
	
	public float ChanceWeapons
	{
		get { return this.chanceWeapons; }
		set { this.chanceWeapons = value; }
	}
	
	public bool FirstOneAlwaysComeBackForMat
	{
		get { return this.firstOneAlwaysComeBackForMat; }
		set { this.firstOneAlwaysComeBackForMat = value; }
	}
	
	public bool FirstOneAlwaysComeBackForWeap
	{
		get { return this.firstOneAlwaysComeBackForWeap; }
		set { this.firstOneAlwaysComeBackForWeap = value; }
	}
	
	public bool Calculated
	{
		get { return this.calculated; }
	}
	
	public int CountMaterials
	{
		get { return this.countMaterials; }
	}
	
	public int CountWeapons
	{
		get { return this.countWeapons; }
	}
}

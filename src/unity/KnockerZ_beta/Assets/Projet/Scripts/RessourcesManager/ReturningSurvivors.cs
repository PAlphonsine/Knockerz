using UnityEngine;
using System.Collections;

public class ReturningSurvivors : MonoBehaviour {
	// Liste des survivants envoyés
	[SerializeField]
	public SentSurvivorScript[] sentSurvivors;
	// Ressources trouvées
	private int foundRessources;
	// "Pourcentage" influant sur le nombre de ressources rapportées par survivants
	private float ressourceChance;
	// "Pourcentage" influant sur les chances de retour selon le nombre de survivants envoyés
	private float chance;
	// Booléen disant au premier survivant de toujours revenir
	private bool firstOneAlwaysComeBackForMat;
	private bool firstOneAlwaysComeBackForWeap;
	// Booléen de controle du calcul déjà fait ou non
	private bool calculated;
	// Détection des phases
	[SerializeField]
	PhasesManager phasesManager;
	#region Tests
	bool survivorsReturning; // retour des survivants
	private int countMaterials = 0; // compteur de survivants revenus pour les matériaux
	private int countWeapons = 0; // compteur de survivants revenus pour les armes
	private bool survivorsSent = false;
	#endregion

	// Use this for initialization
	void Start () {
		// Les survivants ramènent minimum 15 de ressources
		this.foundRessources = 15;
		// Le pourcentage de chance influant sur le gain de ressources est de 10%
		this.ressourceChance = 0.1f;
		// Le pourcentage de chance influant sur le retour des survivants est de 10%
		this.chance = 0.1f;
		// Il y a toujours minimum 1 survivant qui revient
		this.firstOneAlwaysComeBackForMat = true;
		this.firstOneAlwaysComeBackForWeap = true;
		// Détermine si le calcul des chances de retour et de ressources a été fait
		this.calculated = true;
		this.survivorsSent = false;
	}
	
	// Update is called once per frame
	void Update () {
		// Quand on est en phase d'action
		if(phasesManager.startAction == true)
		{
			// Les chances de retour varient selon le nombre de survivants envoyés
			this.chance = 0.1f * this.sentSurvivors.Length;
			if (this.survivorsSent == false)
			{
				// Pour chaque survivant prévu
				foreach (SentSurvivorScript survivor in this.sentSurvivors)
				{
					Debug.Log("Survivors Sent");
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
			// Compteur de survivants revenus
			countMaterials = 0;
			// Pour chaque survivant envoyé
			foreach (SentSurvivorScript survivor in this.sentSurvivors)
			{
				if(survivor.Materials == true)
				{
					// Si c'est le premier
					if (this.firstOneAlwaysComeBackForMat == true)
					{
						Debug.Log("Matériaux précédents : " + GameStats.Instance.RessourcesMat);
						// Il revient toujours
						survivor.ComeBack = true;
						this.firstOneAlwaysComeBackForMat = false;
						countMaterials++;
						// Calcul des ressources qu'il rammène
						GameStats.Instance.RessourcesMat += HowManyToCarry();
						Debug.Log("Matériaux : " + GameStats.Instance.RessourcesMat);
						// Ajout à la population
						GameStats.Instance.Population++;
						continue;
					}
					// Détermination de la survie du survivant ou non
					survivor.ComeBack = KilledOrNotKilled ();
					// S'il survit
					if (survivor.ComeBack == true)
					{
						countMaterials++;
						// Calcul des ressources qu'il rammène
						GameStats.Instance.RessourcesMat += HowManyToCarry();
						Debug.Log("Matériaux : " + GameStats.Instance.RessourcesMat);
						// Ajout à la population
						GameStats.Instance.Population++;
					}
				}
				else
				{
					// Si c'est le premier
					if (this.firstOneAlwaysComeBackForWeap == true)
					{
						Debug.Log("Armes précédentes : " + GameStats.Instance.RessourcesWeap);
						// Il revient toujours
						survivor.ComeBack = true;
						this.firstOneAlwaysComeBackForWeap = false;
						countWeapons++;
						// Calcul des ressources qu'il rammène
						GameStats.Instance.RessourcesWeap += HowManyToCarry();
						Debug.Log("Armes : " + GameStats.Instance.RessourcesWeap);
						// Ajout à la population
						GameStats.Instance.Population++;
						continue;
					}
					// Détermination de la survie du survivant ou non
					survivor.ComeBack = KilledOrNotKilled ();
					// S'il survit
					if (survivor.ComeBack == true)
					{
						countWeapons++;
						// Calcul des ressources qu'il rammène
						GameStats.Instance.RessourcesWeap += HowManyToCarry();
						Debug.Log("Armes : " + GameStats.Instance.RessourcesWeap);
						// Ajout à la population
						GameStats.Instance.Population++;
					}
				}
			}
			Debug.Log ("Rescapés matériaux : " + countMaterials);
			Debug.Log ("Rescapés armes : " + countWeapons);
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

	private bool KilledOrNotKilled()
	{
		// Chiffre aléatoire entre 0.0 et 1.0 + le pourcentage de chance (de 0.1*nbSurvivantsEnvoyés)
		float returningOrNot = Random.Range (0f, 1f) + chance;
		// Si le résultat est supérieur à 1.2
		if (returningOrNot > 1.2f)
		{
			// Le survivant revient
			return true;
		}
		else
		{
			// Sinon non
			return false;
		}
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

	public float Chance
	{
		get { return this.chance; }
		set { this.chance = value; }
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

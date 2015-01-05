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
	private bool firstOneAlwaysComeBack;
	// Booléen de controle du calcul déjà fait ou non
	private bool calculated;
	// Copie du timer du jeu
	[SerializeField]
	PhasesManager phasesManager;
	private float time;
	#region Tests
	[SerializeField]
	bool survivorsReturning; // retour des survivants
	int count = 0; // compteur de survivants revenus
	#endregion

	// Use this for initialization
	void Start () {
		this.foundRessources = 30;
		this.ressourceChance = 0.1f;
		this.chance = 0.1f;
		this.firstOneAlwaysComeBack = true;
		this.calculated = true;
		this.time = phasesManager.vtime; // test
	}
	
	// Update is called once per frame
	void Update () {
		this.time = phasesManager.vtime;

		if(phasesManager.startAction == true)
		{
			this.chance = 0.1f * this.sentSurvivors.Length;
			foreach (SentSurvivorScript survivor in this.sentSurvivors)
			{
				survivor.GoSearch = true;
			}
			calculated = false;
		}
		if (calculated == false && phasesManager.startAction == false)
		{
			this.firstOneAlwaysComeBack = true;
			count = 0;
			foreach (SentSurvivorScript survivor in this.sentSurvivors)
			{
				if (this.firstOneAlwaysComeBack == true)
				{
					Debug.Log("Ressources : " + GameStats.Instance.Ressources);
					survivor.ComeBack = true;
					this.firstOneAlwaysComeBack = false;
					count++;
					GameStats.Instance.Ressources += HowManyToCarry();
					Debug.Log("Ressources : " + GameStats.Instance.Ressources);
					GameStats.Instance.Population++;
					continue;
				}
				
				survivor.ComeBack = KilledOrNotKilled ();
				if (survivor.ComeBack == true)
				{
					count++;
					GameStats.Instance.Ressources += HowManyToCarry();
					Debug.Log("Ressources : " + GameStats.Instance.Ressources);
					GameStats.Instance.Population++;
				}
			}
			Debug.Log ("Rescapés : " + count);
			calculated = true;
		}
	}

	private int HowManyToCarry()
	{
		float richOrNot = Random.Range (5, 15) * ressourceChance;
		this.foundRessources = (int)(30 * richOrNot);

		return foundRessources;
	}

	private bool KilledOrNotKilled()
	{
		float returningOrNot = Random.Range (0f, 1f) + chance;

		if (returningOrNot > 1.2f)
		{
			return true;
		}
		else
		{
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

	public bool FirstOneAlwaysComeBack
	{
		get { return this.firstOneAlwaysComeBack; }
		set { this.firstOneAlwaysComeBack = value; }
	}

	public bool Calculated
	{
		get { return this.calculated; }
	}

	public float Time
	{
		get { return this.time; }
		set { this.time = value; }
	}
}

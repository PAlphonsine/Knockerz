using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopManager : MonoBehaviour {

	//http://docs.unity3d.com/ScriptReference/Editor.html

	// Collection contenant tous les survivants possédés par le joueur dans sa base
	[SerializeField]
	SentSurvivorScript[] allSurvivorsJ1;
	[SerializeField]
	SentSurvivorScript[] allSurvivorsJ2;
	// Textes de l'interface
	[SerializeField]
	Text popText;
	[SerializeField]
	Text popDeclaredTextMaterials;
	[SerializeField]
	Text popDeclaredTextWeapons;
	// Boutons de controle
	[SerializeField]
	Button moreSurvivorsMaterials;
	[SerializeField]
	Button lessSurvivorsMaterials;
	[SerializeField]
	Button sendButtonMaterials;
	[SerializeField]
	Button moreSurvivorsWeapons;
	[SerializeField]
	Button lessSurvivorsWeapons;
	[SerializeField]
	Button sendButtonWeapons;
	
	// Gestion des phases
	[SerializeField]
	PhasesManager phasesManager;
	// Script d'envoi des survivants
	[SerializeField]
	ReturningSurvivors returningSurvivors;
	// Variables de décisions et de confirmation d'envoi de la population
	private int popDeclaredMaterials;
	private int popDeclaredWeapons;
	//private int popToSendNextPhase;
	private int popToSendNextPhaseMaterials;
	private int popToSendNextPhaseWeapons;
	// Booléen d'envoi définitif des survivants
	private bool sending;

	// Use this for initialization
	void Start () {
		// Affichage de la population totale
		this.popText.text = GameStats.Instance.Population.ToString();
		// La population planifiée et envoyée est à zéro au début
		this.popDeclaredMaterials = 0;
		this.popDeclaredWeapons = 0;
		//this.popToSendNextPhase = 0;
		this.popToSendNextPhaseMaterials = 0;
		this.popToSendNextPhaseWeapons = 0;
		// Affichage de la population planifiée dans la fenetre d'envoi
		this.popDeclaredTextMaterials.text = "Population planifiée : " + popDeclaredMaterials.ToString();
		this.popDeclaredTextWeapons.text = "Population planifiée : " + popDeclaredWeapons.ToString();
		// Les survivants ne partent qu'en phase d'action
		this.sending = false;
	}
	
	// Update is called once per frame
	void Update () {
		// Si la partie a commencé
		if(this.phasesManager.startgame == true)
		{
			// La population totale se met à jour
			this.popText.text = "Pop : " + GameStats.Instance.Population.ToString();
			// La population planifiée se met à jour
			this.popDeclaredTextMaterials.text = "Population planifiée : " + popDeclaredMaterials.ToString();
			this.popDeclaredTextWeapons.text = "Population planifiée : " + popDeclaredWeapons.ToString();
			// Si on passe en phase d'action et que personne n'a encore été envoyé
			if (this.phasesManager.startAction == true && sending == false)
			{
				// On instancie le tableau de survivants envoyés selon la population planifiée en phase de réflexion
				returningSurvivors.sentSurvivors = new SentSurvivorScript[popToSendNextPhaseMaterials+popToSendNextPhaseWeapons];
				// Pour le joueur 1
				if(_STATICS._networkPlayer[0] == Network.player){
					int count = 0;
					for(count = 0; count < popToSendNextPhaseMaterials; count++)
					{
						this.allSurvivorsJ1[count].Materials = true;
					}
					for (int i = 0 + count; i < popToSendNextPhaseWeapons + count; i++)
					{
						this.allSurvivorsJ1[i].Weapons = true;
					}
					count = 0;

					// Pour chaque survivants planifié
					for (int i = 0; i < popToSendNextPhaseMaterials + popToSendNextPhaseWeapons; i++)
					{
						if(this.allSurvivorsJ1[i].Materials == true)
						{
							// On ajoute des survivants depuis notre population
							returningSurvivors.sentSurvivors[i] = this.allSurvivorsJ1[i];
						}
						else
						{
							// On ajoute des survivants depuis notre population
							returningSurvivors.sentSurvivors[i] = this.allSurvivorsJ1[i];
						}
					}
				}else{
					// Pour le joueur 2
					if(_STATICS._networkPlayer[1] == Network.player){
						int count = 0;
						for(count = 0; count < popToSendNextPhaseMaterials; count++)
						{
							this.allSurvivorsJ2[count].Materials = true;
						}
						for (int i = 0 + count; i < popToSendNextPhaseWeapons + count; i++)
						{
							this.allSurvivorsJ2[i].Weapons = true;
						}
						count = 0;

						// Pour chaque survivants planifié
						for (int i = 0; i < popToSendNextPhaseMaterials + popToSendNextPhaseWeapons; i++)
						{
							if(this.allSurvivorsJ2[i].Materials == true)
							{
								// On ajoute des survivants depuis notre population
								returningSurvivors.sentSurvivors[i] = this.allSurvivorsJ2[i];
							}
							else
							{
								// On ajoute des survivants depuis notre population
								returningSurvivors.sentSurvivors[i] = this.allSurvivorsJ2[i];
							}
						}
					}
				}
				Debug.Log ("Pop envoyée matériaux : " + popToSendNextPhaseMaterials);
				Debug.Log ("Pop envoyée armes : " + popToSendNextPhaseWeapons);
				// Les survivants ont été envoyés
				sending = true;
			}
			// Lorsque l'on revient en phase de réflexion, on permet de nouveau l'envoi de survivants
			if(this.phasesManager.startAction == false)
				sending = false;
		}
	}

	void FixedUpdate()
	{
		// Si la partie a commencé
		if(this.phasesManager.startgame == true)
		{
			// Si on est en phase de réflexion
			if(this.phasesManager.startAction == false)
			{
				// Si aucune population n'est planifiée
				if (this.popDeclaredMaterials == 0)
				{
					// On ne peut pas cliquer sur - ou sur Envoyer ...
					this.lessSurvivorsMaterials.interactable = false;
					this.sendButtonMaterials.interactable = false;
					// ... mais on peut cliquer sur +
					this.moreSurvivorsMaterials.interactable = true;
				}
				// Si la population planifiée est égale à la population totale
				else if	(this.popDeclaredMaterials == GameStats.Instance.Population)
				{
					// On ne peut plus cliquer sur +
					this.moreSurvivorsMaterials.interactable = false;
				}
				else
				{
					// Tous les boutons sont actifs dans les autres cas
					this.moreSurvivorsMaterials.interactable = true;
					this.lessSurvivorsMaterials.interactable = true;
					this.sendButtonMaterials.interactable = true;
				}

				// Si aucune population n'est planifiée
				if (this.popDeclaredWeapons == 0)
				{
					// On ne peut pas cliquer sur - ou sur Envoyer ...
					this.lessSurvivorsWeapons.interactable = false;
					this.sendButtonWeapons.interactable = false;
					// ... mais on peut cliquer sur +
					this.moreSurvivorsWeapons.interactable = true;
				}
				// Si la population planifiée est égale à la population totale
				else if	(this.popDeclaredWeapons == GameStats.Instance.Population)
				{
					// On ne peut plus cliquer sur +
					this.moreSurvivorsWeapons.interactable = false;
				}
				else
				{
					// Tous les boutons sont actifs dans les autres cas
					this.moreSurvivorsWeapons.interactable = true;
					this.lessSurvivorsWeapons.interactable = true;
					this.sendButtonWeapons.interactable = true;
				}
			}
			// Si on est en phase d'action
			else
			{
				// Il est impossible d'envoyer des survivants - Tous les boutons sont désactivés
				this.lessSurvivorsMaterials.interactable = false;
				this.moreSurvivorsMaterials.interactable = false;
				this.sendButtonMaterials.interactable = false;
				this.lessSurvivorsWeapons.interactable = false;
				this.moreSurvivorsWeapons.interactable = false;
				this.sendButtonWeapons.interactable = false;
			}

			// On met à jour en temps réel l'affichage de la population planifiée et de la population totale
			this.popText.text = GameStats.Instance.Population.ToString();
			this.popDeclaredTextMaterials.text = "Population planifiée : " + popDeclaredMaterials.ToString();
			this.popDeclaredTextWeapons.text = "Population planifiée : " + popDeclaredWeapons.ToString();
		}
	}

	public void PopToSendMaterials()
	{
		if (popDeclaredMaterials < GameStats.Instance.Population)
		{
			popDeclaredMaterials++;
		}
	}

	public void PopToKeepMaterials()
	{
		if (popDeclaredMaterials > 0)
		{
			popDeclaredMaterials--;
		}
	}
	
	public void PopToSendWeapons()
	{
		if (popDeclaredWeapons < GameStats.Instance.Population)
		{
			popDeclaredWeapons++;
		}
	}
	
	public void PopToKeepWeapons()
	{
		if (popDeclaredWeapons > 0)
		{
			popDeclaredWeapons--;
		}
	}

	/*public void PopPlanify()
	{
		GameStats.Instance.Population -= popDeclared;
		this.popToSendNextPhase += popDeclared;
		this.popDeclared = 0;
	}*/
	
	public void PopPlanifyMaterials()
	{
		GameStats.Instance.Population -= popDeclaredMaterials;
		this.popToSendNextPhaseMaterials += popDeclaredMaterials;
		this.popDeclaredMaterials = 0;
	}
	
	public void PopPlanifyWeapons()
	{
		GameStats.Instance.Population -= popDeclaredWeapons;
		this.popToSendNextPhaseWeapons += popDeclaredWeapons;
		this.popDeclaredWeapons = 0;
	}

	// Accesseurs
	public int PopDeclaredMaterials
	{
		get { return this.popDeclaredMaterials; }
		set { this.popDeclaredMaterials = value; }
	}

	public int PopDeclaredWeapons
	{
		get { return this.popDeclaredWeapons; }
		set { this.popDeclaredWeapons = value; }
	}

	public int PopToSendNextPhaseMaterials
	{
		get { return this.popToSendNextPhaseMaterials; }
		set { this.popToSendNextPhaseMaterials = value; }
	}
	
	public int PopToSendNextPhaseWeapons
	{
		get { return this.popToSendNextPhaseWeapons; }
		set { this.popToSendNextPhaseWeapons = value; }
	}

	public bool Sending
	{
		get { return this.sending; }
	}

	/*public void PopSend(int pop){
		if (GameStats.Instance.Population - pop >= 0)
		{
			GameStats.Instance.Population -= pop;
			popText.text = "Population : " + GameStats.Instance.Population.ToString ();
			GameStats.Instance.PopulationSend ++;
		}
	}

	public void PopRevert(int pop){
		if (GameStats.Instance.PopulationSend - pop >= 0) {
			Debug.Log("ok");
			GameStats.Instance.Population += pop;
			popText.text = "Population : " + GameStats.Instance.Population.ToString ();
			GameStats.Instance.PopulationSend --;
		}
	}*/
}

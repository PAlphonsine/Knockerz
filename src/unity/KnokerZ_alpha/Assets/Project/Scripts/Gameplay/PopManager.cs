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
	Text popDeclaredText;
	// Boutons de controle
	[SerializeField]
	Button moreSurvivors;
	[SerializeField]
	Button lessSurvivors;
	[SerializeField]
	Button sendButton;
	
	// Copie du timer du jeu
	[SerializeField]
	PhasesManager phasesManager;
	private float time;
	// Script d'envoi des survivants
	[SerializeField]
	ReturningSurvivors returningSurvivors;
	// Variables de décisions et de confirmation d'envoi de la population
	private int popDeclared;
	private int popToSendNextPhase;
	// Booléen d'envoi définitif des survivants
	private bool sending;

	// Use this for initialization
	void Start () {
		this.popText.text = "Population totale : " + GameStats.Instance.Population.ToString();
		this.popDeclared = 0;
		this.popToSendNextPhase = 0;
		this.popDeclaredText.text = "Population planifiée : " + popDeclared.ToString();
		this.time = phasesManager.vtime;
		this.sending = false;
	}
	
	// Update is called once per frame
	void Update () {
		this.popText.text = "Population totale : " + GameStats.Instance.Population.ToString();
		this.popDeclaredText.text = "Population planifiée : " + popDeclared.ToString();
		this.time = phasesManager.vtime;
		if (this.time <= 0.1f && sending == false)
		{
			returningSurvivors.sentSurvivors = new SentSurvivorScript[popToSendNextPhase];
			if(_STATICS._networkPlayer[0] == Network.player){
				for (int i = 0; i < popToSendNextPhase; i++)
				{
					returningSurvivors.sentSurvivors[i] = this.allSurvivorsJ1[i];
				}
			}else{
				if(_STATICS._networkPlayer[1] == Network.player){
					for (int i = 0; i < popToSendNextPhase; i++)
					{
						returningSurvivors.sentSurvivors[i] = this.allSurvivorsJ2[i];
					}
				}
			}
			Debug.Log ("Pop envoyée : " + popToSendNextPhase);
			sending = true;
		}
	}

	void FixedUpdate()
	{
		if (this.popDeclared == 0)
		{
			this.lessSurvivors.interactable = false;
			this.sendButton.interactable = false;
		}
		else if	(this.popDeclared == GameStats.Instance.Population)
		{
			this.moreSurvivors.interactable = false;
		}
		else
		{
			this.moreSurvivors.interactable = true;
			this.lessSurvivors.interactable = true;
			this.sendButton.interactable = true;
		}

		this.popText.text = "Population totale : " + GameStats.Instance.Population.ToString();
		this.popDeclaredText.text = "Population planifiée : " + popDeclared.ToString();
	}

	public void PopToSend()
	{
		if (popDeclared < GameStats.Instance.Population)
		{
			popDeclared++;
		}
	}

	public void PopToKeep()
	{
		if (popDeclared > 0)
		{
			popDeclared--;
		}
	}

	public void PopPlanify()
	{
		GameStats.Instance.Population -= popDeclared;
		this.popToSendNextPhase += popDeclared;
		this.popDeclared = 0;
	}

	// Accesseurs
	public int PopDeclared
	{
		get { return this.popDeclared; }
		set { this.popDeclared = value; }
	}

	public int PopToSendNextPhase
	{
		get { return this.popToSendNextPhase; }
		set { this.popToSendNextPhase = value; }
	}

	public float Time
	{
		get { return this.time; }
		set { this.time = value; }
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

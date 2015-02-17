using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EventsNewspaperButtonScript : MonoBehaviour {
	[SerializeField]
	GameObject eventsNewspaperButtonPanel;
	[SerializeField]
	Text newsFlowText;
	[SerializeField]
	PhasesManager phasesManager;
	[SerializeField]
	PopManager popManager;
	[SerializeField]
	ReturningSurvivors returningSurvivors;

	private bool newsShown;
	private bool popSent;

	// Use this for initialization
	void Start () {
		this.newsShown = false;
		this.popSent = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		if(this.phasesManager.startgame == true)
		{			
			if (this.phasesManager.startAction == false)
			{
				this.IntoMinding();
			}
			else
			{
				this.IntoAction();
			}
		}
	}

	public void EventsNewspaperButtonPanelEnabled()
	{
		if (this.eventsNewspaperButtonPanel.activeSelf == false)
		{
			this.eventsNewspaperButtonPanel.SetActive(true);
		}
		else
		{
			this.eventsNewspaperButtonPanel.SetActive(false);
		}
	}

	private void IntoMinding()
	{
		if(this.newsShown == false)
		{
			this.newsFlowText.text += "La phase de réflexion débute. Pensez à tout !\n";
			if(this.popManager.PopToSendNextPhaseMaterials + this.popManager.PopToSendNextPhaseWeapons == 0)
			{
				if(this.popSent == true)
				{
					this.newsFlowText.text += "Rescapés matériaux : " + this.returningSurvivors.CountMaterials + "\n";
					this.newsFlowText.text += "Rescapés armes : " + this.returningSurvivors.CountWeapons + "\n";
				}
				else
				{
					this.newsFlowText.text += "Aucune population envoyée.\n";
				}
			}
		}
		this.newsShown = true;
	}

	private void IntoAction()
	{
		if(this.newsShown == true)
		{
			this.newsFlowText.text += "Les Zombies arrivent ... Défendez-vous !\n";
			if(this.popManager.PopToSendNextPhaseMaterials + this.popManager.PopToSendNextPhaseWeapons == 0)
			{
				this.newsFlowText.text += "Aucun population envoyée.\n";
			}
			else
			{
				this.newsFlowText.text += "Population envoyée matériaux : " + this.popManager.PopToSendNextPhaseMaterials + "\n";
				this.newsFlowText.text += "Population envoyée armes : " + this.popManager.PopToSendNextPhaseWeapons + "\n";
				this.popManager.PopToSendNextPhaseMaterials = 0;
				this.popManager.PopToSendNextPhaseWeapons = 0;
				this.popSent = true;
			}
		}
		this.newsShown = false;
	}

	// Accesseurs
	public bool NewsShown
	{
		get { return this.newsShown; }
	}
}
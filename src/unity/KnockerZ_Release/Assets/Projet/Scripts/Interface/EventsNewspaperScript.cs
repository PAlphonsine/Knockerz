using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EventsNewspaperScript : MonoBehaviour
{
	// Gestion des phases
	[SerializeField] PhasesManager phasesManager;
	// Panel du journal d'évènements
	[SerializeField] GameObject eventsNewspaperPanel;
	// Bouton d'activation du journal d'évènements
	[SerializeField] Button eventsNewspaperButton;
	// Texte du journal d'évènements
	[SerializeField] Text newsFlowText;
	// Zone de défilement et barre de défilement du journal d'évènements
	[SerializeField] Scrollbar eventsScrollbar;
	[SerializeField] GameObject eventsHandle;
	// Booléen de controle d'ajout d'une ligne
	private bool lineAdded;
	// Booléen de controle d'affichage pour la première vague des évènements
	private bool firstPhase;
	// Booléen de controle d'affichage des évènements selon les phases
	private bool newsShownMinding;
	private bool newsShownAction;
	// Nombre de Survivants envoyés à la recherche de ressources
	private int materialsSurvivorsGo;
	private int weaponsSurvivorsGo;
	// Nombre de Survivants revenus de la recherche de ressources
	private int materialsSurvivorsBack;
	private int weaponsSurvivorsBack;
	// Booléen de controle d'arrivée des machines boss
	private bool bossWave;
	// Caractéristiques des boss des deux joueurs
	private int boss1Pv;
	private float boss1Speed;
	private int boss1Damages;
	private float boss1ZombiesSpawnCooldown;
	private int boss2Pv;
	private float boss2Speed;
	private int boss2Damages;
	private float boss2ZombiesSpawnCooldown;

	// Use this for initialization
	void Start ()
	{
		// Aucune ligne n'est ajoutée au départ
		this.lineAdded = false;
		// La partie commence par la première phase
		this.firstPhase = true;
		// Aucune information n'est à afficher pour le moment
		this.newsShownMinding = false;
		this.newsShownAction = false;
		// La première vague n'est pas la vague de boss ...
		this.bossWave = false;
		// ... et toutes ses caractéristiques sont à zéro
		this.boss1Pv = 3000;
		this.boss1Speed = 0.5f;
		this.boss1Damages = 6;
		this.boss1ZombiesSpawnCooldown = 5;
		this.boss2Pv = 3000;
		this.boss2Speed = 0.5f;
		this.boss2Damages = 6;
		this.boss2ZombiesSpawnCooldown = 5;
	}

	void Update ()
	{
		// Si la partie a commencé
		if (this.phasesManager.startgame == true)
		{
			// Si l'on est en phase de réflexion
			if (this.phasesManager.startAction == false)
			{
				if (this.firstPhase == true)
				{
					this.newsFlowText.text += "LES ZOMBIES SONT EN TRAIN D'ARRIVER ! PREPAREZ VOS DEFENSES DE TOUTE URGENCE !\n";
					this.lineAdded = true;
					this.firstPhase = false;
				}
				else
				{
					if (this.newsShownMinding == false)
					{
						if (this.lineAdded == true && this.newsFlowText.cachedTextGenerator.lineCount >= 10)
						{
							this.newsFlowText.rectTransform.sizeDelta = new Vector2(this.newsFlowText.rectTransform.sizeDelta.x, this.newsFlowText.rectTransform.sizeDelta.y+50f);
							this.newsFlowText.rectTransform.transform.position = new Vector3(149.3f, 402.2f, 0f);
							this.lineAdded = false;
						}

						if (this.materialsSurvivorsBack == 0)
						{
							this.newsFlowText.text += "";
							this.lineAdded = true;
						}
						else if (this.materialsSurvivorsBack == 1)
						{
							this.newsFlowText.text += "Un seul survivant est revenu vivant de la recherche de matériaux.\n";
							this.lineAdded = true;
						}
						else
						{
							this.newsFlowText.text += this.materialsSurvivorsBack + " survivants sont revenus vivants de la recherche de matériaux.\n";
							this.lineAdded = true;
						}
						
						if (this.weaponsSurvivorsBack == 0)
						{
							this.newsFlowText.text += "";
							this.lineAdded = true;
						}
						else if (this.weaponsSurvivorsBack == 1)
						{
							this.newsFlowText.text += "Un seul survivant est revenu vivant de la recherche d'armes.\n";
							this.lineAdded = true;
						}
						else
						{
							this.newsFlowText.text += this.weaponsSurvivorsBack + " survivants sont revenus vivants de la recherche d'armes.\n";
							this.lineAdded = true;
						}

						this.newsFlowText.text += "\n";
						this.lineAdded = true;
						this.materialsSurvivorsBack = 0;
						this.weaponsSurvivorsBack = 0;
						this.newsShownMinding = true;
						this.newsShownAction = false;
					}
				}
			}
			// Sinon, si l'on est en phase d'action
			else
			{
				if (this.newsShownAction == false)
				{
					if (this.lineAdded == true && this.newsFlowText.cachedTextGenerator.lineCount >= 10)
					{
						this.newsFlowText.rectTransform.sizeDelta = new Vector2(this.newsFlowText.rectTransform.sizeDelta.x, this.newsFlowText.rectTransform.sizeDelta.y+50f);
						this.newsFlowText.rectTransform.transform.position = new Vector3(149.3f, 402.2f, 0f);
						this.lineAdded = false;
					}

					if (this.materialsSurvivorsGo == 0)
					{
						this.newsFlowText.text += "Aucun survivant n'a été envoyé à la recherche de matériaux.\n";
						this.lineAdded = true;
					}
					else if (this.materialsSurvivorsGo == 1)
					{
						this.newsFlowText.text += "Un seul survivant a été envoyé à la recherche de matériaux.\n";
						this.lineAdded = true;
					}
					else
					{
						this.newsFlowText.text += this.materialsSurvivorsGo + " survivants ont été envoyés à la recherche de matériaux.\n";
						this.lineAdded = true;
					}

					if (this.weaponsSurvivorsGo == 0)
					{
						this.newsFlowText.text += "Aucun survivant n'a été envoyé à la recherche d'armes.\n";
						this.lineAdded = true;
					}
					else if (this.weaponsSurvivorsGo == 1)
					{
						this.newsFlowText.text += "Un seul survivant a été envoyé à la recherche d'armes.\n";
						this.lineAdded = true;
					}
					else
					{
						this.newsFlowText.text += this.weaponsSurvivorsGo + " survivants ont été envoyés à la recherche d'armes.\n";
						this.lineAdded = true;
					}

					if (this.bossWave == true)
					{
						this.newsFlowText.text += "\nALERTE ! UNE MACHINE ADVERSE ARRIVE !!\n";
						if (Network.player == _STATICS._networkPlayer[0])
						{
							this.newsFlowText.text += "Machine de l'ennemi : " + this.boss2Pv + "PV - " + this.boss2Speed * 10f + "-" + this.boss2Speed/2f * 10f + "km/h - " + this.boss2Damages + " d'ATQ - 1Z/" + System.Math.Round (this.boss2ZombiesSpawnCooldown, 2) + "sec\n";
							this.newsFlowText.text += "Votre machine : " + this.boss1Pv + "PV - " + this.boss1Speed * 10f + "-" + this.boss1Speed/2f * 10f + "km/h - " + this.boss1Damages + " d'ATQ - 1Z/" + System.Math.Round (this.boss1ZombiesSpawnCooldown, 2) + "sec\n";
							this.lineAdded = true;
						}
						else if (Network.player == _STATICS._networkPlayer[1])
						{
							this.newsFlowText.text += "Machine de l'ennemi : " + this.boss1Pv + "PV - " + this.boss1Speed * 10f + "-" + this.boss1Speed/2f * 10f + "km/h - " + this.boss1Damages + " d'ATQ - 1Z/" + System.Math.Round (this.boss1ZombiesSpawnCooldown, 2) + "sec\n";
							this.newsFlowText.text += "Votre machine : " + this.boss2Pv + "PV - " + this.boss2Speed * 10f + "-" + this.boss2Speed/2f * 10f + "km/h - " + this.boss2Damages + " d'ATQ - Un 1Z/" + System.Math.Round (this.boss2ZombiesSpawnCooldown, 2) + "sec\n";
							this.lineAdded = true;
						}

						this.bossWave = false;
						this.boss1Pv = 3000;
						this.boss1Speed = 0.5f;
						this.boss1Damages = 6;
						this.boss1ZombiesSpawnCooldown = 5;
						this.boss2Pv = 3000;
						this.boss2Speed = 0.5f;
						this.boss2Damages = 6;
						this.boss2ZombiesSpawnCooldown = 5;
					}

					this.newsFlowText.text += "\n";
					this.lineAdded = true;
					this.materialsSurvivorsGo = 0;
					this.weaponsSurvivorsGo = 0;
					this.newsShownMinding = false;
					this.newsShownAction = true;
				}
			}
		}
	}
	
	// Méthode d'activation et de désactivation du panel du journal d'évènements
	public void EventsNewspaperButtonPanelEnabled()
	{
		this.eventsNewspaperPanel.SetActive(!this.eventsNewspaperPanel.activeSelf);
	}

	// Accesseurs
	public bool LineAdded
	{
		get { return this.lineAdded; }
		set { this.lineAdded = value; }
	}

	public bool FirstPhase
	{
		get { return this.firstPhase; }
		set { this.firstPhase = value; }
	}

	public bool NewsShownMinding
	{
		get { return this.newsShownMinding; }
		set { this.newsShownMinding = value; }
	}

	public bool NewsShownAction
	{
		get { return this.newsShownAction; }
		set { this.newsShownAction = value; }
	}

	public int MaterialsSurvivorsGo
	{
		get { return this.materialsSurvivorsGo; }
		set { this.materialsSurvivorsGo = value; }
	}

	public int WeaponsSurvivorsGo
	{
		get { return this.weaponsSurvivorsGo; }
		set { this.weaponsSurvivorsGo = value; }
	}

	public int MaterialsSurvivorsBack
	{
		get { return this.materialsSurvivorsBack; }
		set { this.materialsSurvivorsBack = value; }
	}

	public int WeaponsSurvivorsBack
	{
		get { return this.weaponsSurvivorsBack; }
		set { this.weaponsSurvivorsBack = value; }
	}

	public bool BossWave
	{
		get { return this.bossWave; }
		set { this.bossWave = value; }
	}

	public int Boss1Pv
	{
		get { return this.boss1Pv; }
		set { this.boss1Pv = value; }
	}
	
	public float Boss1Speed
	{
		get { return this.boss1Speed; }
		set { this.boss1Speed = value; }
	}
	
	public int Boss1Damages
	{
		get { return this.boss1Damages; }
		set { this.boss1Damages = value; }
	}
	
	public float Boss1ZombiesSpawnCooldown
	{
		get { return this.boss1ZombiesSpawnCooldown; }
		set { this.boss1ZombiesSpawnCooldown = value; }
	}
	
	public int Boss2Pv
	{
		get { return this.boss1Pv; }
		set { this.boss1Pv = value; }
	}
	
	public float Boss2Speed
	{
		get { return this.boss1Speed; }
		set { this.boss1Speed = value; }
	}
	
	public int Boss2Damages
	{
		get { return this.boss1Damages; }
		set { this.boss1Damages = value; }
	}
	
	public float Boss2ZombiesSpawnCooldown
	{
		get { return this.boss1ZombiesSpawnCooldown; }
		set { this.boss1ZombiesSpawnCooldown = value; }
	}
}
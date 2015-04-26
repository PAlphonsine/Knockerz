using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MachinePreparationScript : MonoBehaviour
{
	// Journal d'évènements
	[SerializeField] EventsNewspaperScript eventsNewspaperScript;
	// Gestion des phases
	[SerializeField] PhasesManager phasesManager;
	// Panel d'envoi à la fabrication de la machine
	[SerializeField] GameObject machinePanel;
	// Composants Text
	[SerializeField] Text popText;
	[SerializeField] Text materialsText;
	[SerializeField] Text weaponsText;
	[SerializeField] Text lifePointsText;
	[SerializeField] Text speedText;
	[SerializeField] Text damagesText;
	[SerializeField] Text zombiesSpawnCooldownText;
	[SerializeField] Text playerPopText;
	[SerializeField] Text playerMaterialsText;
	[SerializeField] Text playerWeaponsText;
	// Composants InputField
	[SerializeField] InputField popInputField;
	[SerializeField] InputField materialsInputField;
	[SerializeField] InputField weaponsInputField;
	// Boutons de validation et d'annulation
	[SerializeField] Button validateButton;
	[SerializeField] Button cancelButton;

	// Boss dans le jeu
	[SerializeField] GameObject[] bosses;
	// Scripts des caractéristiques zombie des boss
	[SerializeField] ZombieScript[] zombieScriptBosses;
	// Scripts des boss 
	[SerializeField] BossScript[] bossesScript;

	// Caractéristiques de base du boss
	private int lifePointsDefault;
	private float speedDefault;
	private int damagesDefault;
	private float zombiesSpawnCooldownDefault;

	// Caractéristiques finales du boss
	private int newLifePoints;
	private float newSpeed;
	private int newDamages;
	private float newZombiesSpawnCooldown;

	// Apport à la fabrication en cours de planification
	private int popPlanned;
	private int materialsPlanned;
	private int weaponsPlanned;

	// Apport définitif à la fabrication
	private int popWorking;
	private int materialsUsed;
	private int weaponsUsed;

	// Coefficient modificateur des caractéristiques selon l'apport
	private float lifePointsCoeff;
	private float speedCoeff;
	private float damagesCoeff;
	private float zombiesSpawnCooldownCoeff;

	void Start ()
	{
		// On définit les caractéristiques de base du boss
		this.lifePointsDefault = 3000;
		this.speedDefault = 0.5f;
		this.damagesDefault = 6;
		this.zombiesSpawnCooldownDefault = 5f;

		// Toutes les valeurs sont à 0
		this.newLifePoints = 0;
		this.newSpeed = 0;
		this.newDamages = 0;
		this.newZombiesSpawnCooldown = 0f;

		this.popPlanned = 0;
		this.materialsPlanned = 0;
		this.weaponsPlanned = 0;

		this.popWorking = 0;
		this.materialsUsed = 0;
		this.weaponsUsed = 0;

		this.lifePointsCoeff = 0f;
		this.speedCoeff = 0f;
		this.damagesCoeff = 0f;
		this.zombiesSpawnCooldownCoeff = 0f;
	}

	void Update ()
	{
		// Si la partie a commencé
		if (this.phasesManager.startgame == true)
		{
			// Si l'on est en phase de réflexion
			if (this.phasesManager.startAction == false)
			{
				// On effectue les vérification nécessaire pour permettre ou non la validation au joueur selon les informations rentrées
				if (this.popPlanned <= 0 && this.materialsPlanned <= 0 && this.weaponsPlanned <= 0)
				{
					this.validateButton.interactable = false;
					this.cancelButton.interactable = true;
				}
				else
				{
					this.validateButton.interactable = true;
					this.cancelButton.interactable = true;
				}

				if (this.popPlanned > GameStats.Instance.Population || this.materialsPlanned > GameStats.Instance.RessourcesMat || this.weaponsPlanned > GameStats.Instance.RessourcesWeap)
				{
					this.validateButton.interactable = false;
					this.cancelButton.interactable = true;
				}
				else
				{
					this.validateButton.interactable = true;
					this.cancelButton.interactable = true;
				}
			}
			// Sinon, si l'on est en phase d'action
			else
			{
				// Le joueur ne peut plus envoyer à la fabrication
				this.validateButton.interactable = false;
				this.cancelButton.interactable = false;
			}
		}
	}

	// Coroutine de clignotement des textes
	public IEnumerator AmmountExceeded(string ressource)
	{
		// Selon la ressource dont le montant planifié est supérieur au montant possible du joueur
		if (ressource == "Population")
		{
			// On fait clignoter le texte de cette ressource pour l'avertir
			while (this.popPlanned > GameStats.Instance.Population)
			{
				this.playerPopText.color = new Color(255, 0, 0);
				yield return new WaitForSeconds (0.5f);
				this.playerPopText.color = new Color(0.195f, 0.195f, 0.195f, 1.000f);
				yield return new WaitForSeconds (0.5f);
			}
		}

		if (ressource == "Matériaux")
		{
			while (this.materialsPlanned > GameStats.Instance.RessourcesMat)
			{
				this.playerMaterialsText.color = new Color(255, 0, 0);
				yield return new WaitForSeconds (0.5f);
				this.playerMaterialsText.color = new Color(0.195f, 0.195f, 0.195f, 1.000f);
				yield return new WaitForSeconds (0.5f);
			}
		}

		if (ressource == "Armes")
		{
			while (this.weaponsPlanned > GameStats.Instance.RessourcesWeap)
			{
				this.playerWeaponsText.color = new Color(255, 0, 0);
				yield return new WaitForSeconds (0.5f);
				this.playerWeaponsText.color = new Color(0.195f, 0.195f, 0.195f, 1.000f);
				yield return new WaitForSeconds (0.5f);
			}
		}
	}

	// Méthode de planification de la population
	public void PopPlannedChange()
	{
		// Si un montant est rentré
		if (this.popInputField.text != "")
		{
			// On convertit ce montant en int
			this.popPlanned = int.Parse(this.popInputField.text);
			// On calcule les modifications que ce montant apporte
			this.LifePointsCalculate();
			this.SpeedCalculate();
			this.DamagesCalculate();
			this.ZombiesSpawnRateCalculate();
		}
		else
		{
			// Sinon, on considère que le montant est de 0
			this.popPlanned = 0;
		}

		// Le joueur ne peut pas envoyer plus de population qu'il n'en a
		if (this.popPlanned > GameStats.Instance.Population)
		{
			StartCoroutine(this.AmmountExceeded("Population"));
		}
		else
		{
			this.playerPopText.color = new Color(0.195f, 0.195f, 0.195f, 1.000f);
		}
	}
	
	// Méthode de planification des matériaux
	public void MaterialsPlannedChange()
	{
		// Si un montant est rentré
		if (this.materialsInputField.text != "")
		{
			// On convertit ce montant en int
			this.materialsPlanned = int.Parse(this.materialsInputField.text);
			// On calcule les modifications que ce montant apporte
			this.LifePointsCalculate();
		}
		else
		{
			// Sinon, on considère que le montant est de 0
			this.materialsPlanned = 0;
		}
		
		// Le joueur ne peut pas envoyer plus de matériaux qu'il n'en a
		if (this.materialsPlanned > GameStats.Instance.RessourcesMat)
		{
			StartCoroutine(this.AmmountExceeded("Matériaux"));
		}
		else
		{
			this.playerMaterialsText.color = new Color(0.195f, 0.195f, 0.195f, 1.000f);
		}
	}
	
	// Méthode de planification des armes
	public void WeaponsPlannedChange()
	{
		// Si un montant est rentré
		if (this.weaponsInputField.text != "")
		{
			// On convertit ce montant en int
			this.weaponsPlanned = int.Parse(this.weaponsInputField.text);
			// On calcule les modifications que ce montant apporte
			this.DamagesCalculate();
		}
		else
		{
			// Sinon, on considère que le montant est de 0
			this.weaponsPlanned = 0;
		}
		
		// Le joueur ne peut pas envoyer plus d'armes qu'il n'en a
		if (this.weaponsPlanned > GameStats.Instance.RessourcesWeap)
		{
			StartCoroutine(this.AmmountExceeded("Armes"));
		}
		else
		{
			this.playerWeaponsText.color = new Color(0.195f, 0.195f, 0.195f, 1.000f);
		}
	}

	// Méthode de calcul des points de vie ajoutés au boss
	public void LifePointsCalculate()
	{
		// On affiche ce que la modification ajoute au boss
		this.lifePointsText.text = "+" + System.Math.Round ((((this.popPlanned + this.popWorking) * 30 + (this.materialsPlanned + this.materialsUsed) * 0.80) / this.lifePointsDefault), 2) * 100f + "%";
	}
	
	// Méthode de calcul de la vitesse ajoutée au boss
	public void SpeedCalculate()
	{
		// On affiche ce que la modification ajoute au boss
		this.speedText.text = "+" + System.Math.Round ((((this.popPlanned + this.popWorking) * 0.1) / this.speedDefault), 2) * 100f + "%";
	}
	
	// Méthode de calcul des dommages ajoutés au boss
	public void DamagesCalculate()
	{
		// On affiche ce que la modification ajoute au boss
		this.damagesText.text = "+" + System.Math.Round ((((this.popPlanned + this.popWorking) * 0.1 + (this.weaponsPlanned + this.weaponsUsed) * 0.005) / this.damagesDefault), 2) * 100f + "%";
	}
	
	// Méthode de calcul des points de la cadence d'apparition des Zombies
	public void ZombiesSpawnRateCalculate()
	{
		// On affiche ce que la modification ajoute au boss
		this.zombiesSpawnCooldownText.text = "-" + System.Math.Round ((((this.popPlanned + this.popWorking) * 0.05) / this.zombiesSpawnCooldownDefault), 2) * 100f + "%";
	}

	// Méthode de validation de l'envoi à la fabrication
	public void Validate()
	{
		// On ajoute à l'apport définitif l'apport planifié
		this.popWorking += this.popPlanned;
		this.materialsUsed += this.materialsPlanned;
		this.weaponsUsed += this.materialsPlanned;

		// On calcule les coefficients
		this.lifePointsCoeff = (float)System.Math.Round (((this.popWorking * 30 + this.materialsUsed * 0.80) / this.lifePointsDefault), 2);
		this.speedCoeff = (float)System.Math.Round ((((this.popPlanned + this.popWorking) * 0.1) / this.speedDefault), 2);
		this.damagesCoeff = (float)System.Math.Round (((this.popWorking * 0.1 + this.weaponsUsed * 0.005) / this.damagesDefault), 2);
		this.zombiesSpawnCooldownCoeff = (float)System.Math.Round ((((this.popPlanned + this.popWorking) * 0.05) / this.zombiesSpawnCooldownDefault), 2);

		// On calcule les caractéristiques finales du boss
		this.newLifePoints = System.Convert.ToInt32(this.lifePointsDefault * (1 + this.lifePointsCoeff));
		this.newSpeed = Mathf.Clamp(this.speedDefault * (1 + this.speedCoeff), 1f, 2f);
		this.newDamages = System.Convert.ToInt32(this.damagesDefault * (1 + this.damagesCoeff));
		this.newZombiesSpawnCooldown = this.zombiesSpawnCooldownDefault * (1 - this.zombiesSpawnCooldownCoeff);

		// On envoie à tout le monde les caractéristiques du boss
		networkView.RPC("SendBossInfos", RPCMode.AllBuffered, Network.player, this.newLifePoints, this.newSpeed, this.newDamages, this.newZombiesSpawnCooldown);

		// On retire toutes les ressources planifiées
		GameStats.Instance.Population -= this.popPlanned;
		GameStats.Instance.RessourcesMat -= this.materialsPlanned;
		GameStats.Instance.RessourcesWeap -= this.weaponsPlanned;

		// On met à jour l'affichage de l'apport
		this.popText.text = this.popWorking.ToString();
		this.materialsText.text = this.materialsUsed.ToString();
		this.weaponsText.text = this.weaponsUsed.ToString();

		// On réinitialise
		this.Reset ();
	}

	// Méthode d'annulation
	public void Cancel()
	{
		// On réinitialise
		this.Reset ();
	}

	// Méthode de réinitialisation
	public void Reset()
	{
		// On réinitialise tous les affichages qui ne sont pas persistants
		this.popPlanned = 0;
		this.materialsPlanned = 0;
		this.weaponsPlanned = 0;
		
		this.popInputField.text = "";
		this.materialsInputField.text = "";
		this.weaponsInputField.text = "";
	}

	// Méthode de réinitialisation totale de la machine
	public void RefabricateMachine()
	{
		// Toutes les caractéristiques de la machine sont remises à leur valeur d'origine
		this.lifePointsDefault = 3000;
		this.speedDefault = 0.5f;
		this.damagesDefault = 6;
		this.zombiesSpawnCooldownDefault = 5f;

		// Toutes les informations et les caractéristiques apportées sont remises à zéro
		this.newLifePoints = 0;
		this.newSpeed = 0;
		this.newDamages = 0;
		this.newZombiesSpawnCooldown = 0f;
		
		this.popPlanned = 0;
		this.materialsPlanned = 0;
		this.weaponsPlanned = 0;
		
		this.popWorking = 0;
		this.materialsUsed = 0;
		this.weaponsUsed = 0;
		
		this.lifePointsCoeff = 0f;
		this.speedCoeff = 0f;
		this.damagesCoeff = 0f;
		this.zombiesSpawnCooldownCoeff = 0f;

		this.popText.text = "0";
		this.materialsText.text = "0";
		this.weaponsText.text = "0";

		this.lifePointsText.text = "+0%";
		this.speedText.text = "+0%";
		this.damagesText.text = "+0%";
		this.zombiesSpawnCooldownText.text = "+0%";
	}

	// RPC d'envoi des informations du boss
	[RPC]
	void SendBossInfos(NetworkPlayer player, int pv, float speed, int dps, float cooldown)
	{
		// Si le joueur est le joueur 1
		if (player == _STATICS._networkPlayer[0])
		{
			// On applique les caractéristiques au boss
			zombieScriptBosses[0].Pv = pv;
			// Si les caractéristiques dépassent un certain montant
			if (pv >= lifePointsDefault + 1000)
				// Des composants graphiques peuvent etre activés
				bossesScript[0].ObjectPv.SetActive(true);
			else
				bossesScript[0].ObjectPv.SetActive(false);
			zombieScriptBosses[0].StartPv = pv;

			zombieScriptBosses[0].Dps = dps;
			if (dps >= damagesDefault + 2)
				bossesScript[0].ObjectDps.SetActive(true);
			else
				bossesScript[0].ObjectDps.SetActive(false);

			bossesScript[0].Speed = speed;
			if (speed >= speedDefault + 0.5)
				bossesScript[0].ObjectSpeed.SetActive(true);
			else
				bossesScript[0].ObjectSpeed.SetActive(false);

			bossesScript[0].CooldowndZombiesSpawn = cooldown;
			if (cooldown <= zombiesSpawnCooldownDefault - 2)
				bossesScript[0].ObjectCooldown.SetActive(true);
			else
				bossesScript[0].ObjectCooldown.SetActive(false);

			// On envoie les informations du boss au journal d'évènements
			this.eventsNewspaperScript.Boss1Pv = pv;
			this.eventsNewspaperScript.Boss1Speed = speed;
			this.eventsNewspaperScript.Boss1Damages = dps;
			this.eventsNewspaperScript.Boss1ZombiesSpawnCooldown = cooldown;
		}
		// Et on fait de meme pour le joueur 2
		else
		{
			zombieScriptBosses[1].Pv = pv;
			if (pv >= lifePointsDefault + 1000)
				bossesScript[1].ObjectPv.SetActive(true);
			else
				bossesScript[1].ObjectPv.SetActive(false);
			zombieScriptBosses[1].StartPv = pv;

			zombieScriptBosses[1].Dps = dps;
			if (dps >= damagesDefault + 2)
				bossesScript[1].ObjectDps.SetActive(true);
			else
				bossesScript[1].ObjectDps.SetActive(false);

			bossesScript[1].Speed = speed;
			if (speed >= speedDefault + 0.5)
				bossesScript[1].ObjectSpeed.SetActive(true);
			else
				bossesScript[1].ObjectSpeed.SetActive(false);

			bossesScript[1].CooldowndZombiesSpawn = cooldown;
			if (cooldown <= zombiesSpawnCooldownDefault - 2)
				bossesScript[1].ObjectCooldown.SetActive(true);
			else
				bossesScript[1].ObjectCooldown.SetActive(false);
			
			this.eventsNewspaperScript.Boss2Pv = pv;
			this.eventsNewspaperScript.Boss2Speed = speed;
			this.eventsNewspaperScript.Boss2Damages = dps;
			this.eventsNewspaperScript.Boss2ZombiesSpawnCooldown = cooldown;
		}
	}

	// Méthode d'activation et de désactivation du panel de planification de la machine
	public void MachinePanelEnabled()
	{
		this.machinePanel.SetActive(!this.machinePanel.activeSelf);
	}

	//Accesseurs
	public BossScript[] BossesScript
	{
		get { return bossesScript; }
		set { bossesScript = value; }
	}
	
	public ZombieScript[] ZombieScriptBosses
	{
		get { return zombieScriptBosses; }
		set { zombieScriptBosses = value; }
	}

	public GameObject[] Bosses
	{
		get { return bosses; }
		set { bosses = value; }
	}
}
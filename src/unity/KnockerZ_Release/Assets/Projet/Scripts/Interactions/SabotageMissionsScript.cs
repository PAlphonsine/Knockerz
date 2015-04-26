using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SabotageMissionsScript : MonoBehaviour
{
	// Gestion des phases
	[SerializeField] PhasesManager phasesManager;
	// Gestion des sabotages lors des phases d'actions
	[SerializeField] FinalSabotageScript finalSabotageScript;
	// Texte informant le joueur du montant de son compte d'or
	[SerializeField] Text goldAmmountText;
	// Tourelle ciblée par le sabotage
	private GameObject targetedTurret;
	// Composant InputField recevant l'investissement en or du joueur
	[SerializeField] InputField goldInvestmentInputField;
	// Bouton valider
	[SerializeField] Button validateButton;
	// Composant Text du pourcentage de dangerosité de la mission
	[SerializeField] Text deathRateText;
	// Composant Text du pourcentage de réussite de la mission
	[SerializeField] Text successRateText;
	// Tourelle à laquelle le canvas est attaché (distance et corps-à-corps)
	[SerializeField] TurretDistance turretDistance;
	[SerializeField] TurretHtoH turretHToH;
	// Niveau de la tourelle
	private int turretLevel;
	// Or investi dans la mission
	private int goldInvestment;
	// Taux de dangerosité de la mission
	private float deathRate;
	// Taux de réussite de la mission
	private float successRate;
	// Booléen de controle de reset
	private bool hasBeenReseted;
	// Booléen de controle d'envoi d'un sabotage précédemment
	private bool alreadyPlanned;
	
	// Use this for initialization
	void Start ()
	{
		// Le curseur est dans l'InputField
		this.goldInvestmentInputField.Select ();
		this.goldInvestmentInputField.ActivateInputField ();
		// L'affichage contient les informations par défaut
		this.deathRateText.text = "100%";
		this.successRateText.text = "0%";
		this.goldInvestment = 0;
		this.deathRate = 100;
		this.successRate = 0;
		this.hasBeenReseted = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Si l'o est en phase de réflexion
		if (this.phasesManager.startAction == false)
		{
			// Si le panneau de sabotage n'a pas été réinitialisé ...
			if (this.hasBeenReseted == false)
			{
				// ... on le réinitialise
				this.Reset();
			}
			
			// Si aucun la mission n'est pas financée
			// ou que l'or investi dépasse le montant d'or possédé par le joueur
			// ou que le joueur n'a pas de population disponible
			if (this.goldInvestment == 0 || this.goldInvestment > GameStats.Instance.Gold || GameStats.Instance.Population == 0)
			{
				// La mission ne peut pas etre validée
				this.validateButton.interactable = false;
			}
			else
			{
				// Sinon, la mission peut etre validée
				this.validateButton.interactable = true;
			}
			
			// Si aucun financement n'est apporté à la mission
			if (this.goldInvestmentInputField.text == "")
			{
				// Les taux de succès est de danger prennent leur valeur par défaut
				this.successRateText.text = "0%";
				this.deathRateText.text = "100%";
			}
		}
		// Sinon, si l'on est en phase d'action
		else
		{
			// Le panneau de planification de sabotage de tourelle est désactivé
			this.gameObject.SetActive(false);
		}
	}
	
	// Méthode de calcul du pourcentage de danger du sabotage
	public void DeathCalculation()
	{
		// Initialisation du coefficient qui varie les pourcentages
		float coef = 0f;
		
		// Si c'est une tourelle à distance
		if (this.turretHToH.gameObject.activeSelf == false)
		{
			// On récupère le niveau de la tourelle ...
			this.turretLevel = this.turretDistance.NivTurret;
			// ... qui influence le coefficient
			coef = this.turretLevel * 30f;
		}
		// Sinon, si c'est une tourelle au corps-à-corps
		else if (this.turretDistance.gameObject.activeSelf == false)
		{
			// On récupère le niveau de la tourelle ...
			this.turretLevel = this.turretHToH.NivTurret;
			// ... qui influence le coefficient
			coef = this.turretLevel * 30f;
		}
		
		// Le pourcentage de danger est calculé selon le niveau de la tourelle, l'investissement en or et le coefficient
		// Le résultat est arrondi à l'unité
		this.deathRate = Mathf.Round((((this.turretLevel * 100f) / (this.goldInvestment / coef)) / 100f) * 100f);
		
		// Si le pourcentage de danger dépasse 100%
		if (this.deathRate >= 100f)
		{
			// On affiche 100%
			this.deathRateText.text = "100%";
		}
		// Sinon, si le pourcentage de danger est en dessous de 0%
		else if (this.deathRate <= 0f)
		{
			// On affiche 0%
			this.deathRateText.text = "0%";
		}
		else
		{
			// Sinon, on affiche le pourcentage de danger calculé
			this.deathRateText.text = this.deathRate.ToString() + "%";
		}
	}
	
	// Méthode de calcul du pourcentage de réussite du sabotage
	public void SuccessCalculation()
	{
		// Initialisation du coefficient qui varie les pourcentages
		float coef = 0f;
		
		// Si c'est une tourelle à distance
		if (this.turretHToH.gameObject.activeSelf == false)
		{
			// On récupère le niveau de la tourelle ...
			this.turretLevel = this.turretDistance.NivTurret;
			// ... et on définit le coefficient
			coef = 0.5f;
		}
		// Sinon, si c'est une tourelle au corps-à-corps
		else if (this.turretDistance.gameObject.activeSelf == false)
		{
			// On récupère le niveau de la tourelle ...
			this.turretLevel = this.turretHToH.NivTurret;
			// ... et on définit le coefficient
			coef = 0.7f;
		}
		
		// Le pourcentage de danger est calculé selon le niveau de la tourelle, l'investissement en or et le coefficient
		// Le résultat est arrondi à l'unité
		this.successRate = Mathf.Round((((this.goldInvestment * coef) / this.turretLevel) / 100f) * 100f);
		
		// Si le pourcentage de succès dépasse 100%
		if (this.successRate >= 100f)
		{
			// On affiche 100%
			this.successRateText.text = "100%";
		}
		// Sinon, si le pourcentage de succès est en dessous de 0%
		else if (this.successRate <= 0f)
		{
			// On affiche 0%
			this.successRateText.text = "0%";
		}
		else
		{
			// Sinon, on affiche le pourcentage de succès calculé
			this.successRateText.text = this.successRate.ToString() + "%";
		}
	}
	
	// Méthode de déclenchement des calculs des pourcentages de dangers et de réussite
	public void GoldInvestmentChanged()
	{
		// Si l'investissement en or est de 0
		if (this.goldInvestmentInputField.text != "")
		{
			// L'investissement en or est correctement définit à 0
			this.goldInvestment = int.Parse(this.goldInvestmentInputField.text);
		}
		
		// Si l'investissement en or dépasse les fonds du joueurs
		if (this.goldInvestment > GameStats.Instance.Gold)
		{
			// On fait clignoter son montant d'or pour le prévenir
			StartCoroutine(this.GoldAmmountExceeded());
		}
		else
		{
			// Sinon, le texte de l'or garde sa couleur d'origine
			this.goldAmmountText.color = new Color(0.195f, 0.195f, 0.195f, 1.000f);
		}
		
		// Lancement du calcul des pourcentages de danger et de succès
		this.DeathCalculation ();
		this.SuccessCalculation ();
	}
	
	// Coroutine permettant d'alerter le joueur lorsque son investissement dépasse son or en banque
	public IEnumerator GoldAmmountExceeded()
	{
		// Tant que l'or investi dépasse l'or en possession du joueur
		while (this.goldInvestment > GameStats.Instance.Gold)
		{
			// Le texte devient rouge ...
			this.goldAmmountText.color = new Color(255, 0, 0);
			// ... pendant 0.5 seconde ...
			yield return new WaitForSeconds (0.5f);
			// ... puis revient à sa couleur d'origine ...
			this.goldAmmountText.color = new Color(0.195f, 0.195f, 0.195f, 1.000f);
			// ... pendant 0.5 seconde
			yield return new WaitForSeconds (0.5f);
		}
	}
	
	public void AddSabotage(float deathRate, float successRate)
	{
		this.deathRate = deathRate;
		this.successRate = successRate;
		// Si c'est une tourelle à distance ...
		if (this.turretHToH.gameObject.activeSelf == false)
		{
			// ... elle devient la tourelle ciblée
			this.targetedTurret = this.turretDistance.gameObject;
		}
		// Sinon, si c'est une tourelle au corps-à-corps ...
		else if (this.turretDistance.gameObject.activeSelf == false)
		{
			// ... elle devient la tourelle ciblée
			this.targetedTurret = this.turretHToH.gameObject;
		}
		// La mission est planifiée et est ajoutée à la liste des sabotages prévus
		this.finalSabotageScript.Sabotages.Add (this);
		this.finalSabotageScript.SabotageMenCount = 0;
		// On ferme le panneau de planification de sabotage
		this.CanvasClose ();
	}
	
	// Méthode de validation d'une mission de sabotage
	public void ValidateMission()
	{
		// Si une mission de sabotage n'a pas déjà été planifiée auparavant sur la tourelle désignée
		if (this.alreadyPlanned == false)
		{
			// Si c'est une tourelle à distance ...
			if (this.turretHToH.gameObject.activeSelf == false)
			{
				this.turretDistance.networkView.RPC("ConfirmSabotage", RPCMode.AllBuffered, this.deathRate, this.successRate);
				this.turretDistance.AlreadyPlanned = true;
			}
			// Sinon, si c'est une tourelle au corps-à-corps ...
			else if (this.turretDistance.gameObject.activeSelf == false)
			{
				this.turretHToH.networkView.RPC("ConfirmSabotage", RPCMode.AllBuffered, this.deathRate, this.successRate);
				this.turretHToH.AlreadyPlanned = true;
			}

			// On décrémente la population du joueur
			GameStats.Instance.Population--;
			// Le joueur paye son investissement en or
			GameStats.Instance.Gold -= this.goldInvestment;
			// Un sabotage est planifié sur cette tourelle
			this.alreadyPlanned = true;
		}
		else
		{
			this.Cancel();
		}
		
		// On ferme le panneau de planification de sabotage
		this.CanvasClose ();
	}
	
	// Méthode d'annulation de la planification d'une mission de sabotage
	public void Cancel()
	{
		// On ferme le panneau de planification de sabotage
		this.CanvasClose ();
	}

	// Méthode de fermeture du menu sabotage
	public void CanvasClose ()
	{
		// Le canvas est désactivé
		this.gameObject.SetActive (false);
	}
	
	// Méthode de réinitialisation des paramètres du menu sabotage
	public void Reset()
	{
		// On rétablit tous les paramètres à leur valeur par défaut
		this.deathRateText.text = "100%";
		this.successRateText.text = "0%";
		this.goldInvestment = 0;
		this.deathRate = 100;
		this.successRate = 0;
		this.goldInvestmentInputField.text = "";
		// Le panneau a bien été réinitialisé
		this.hasBeenReseted = true;
		this.alreadyPlanned = false;
	}
	
	// Accesseurs
	public GameObject TargetedTurret
	{
		get { return this.targetedTurret; }
		set { this.targetedTurret = value; }
	}
	
	public int GoldInvestment
	{
		get { return this.goldInvestment; }
		set { this.goldInvestment = value; }
	}
	
	public float DeathRate
	{
		get { return this.deathRate; }
		set { this.deathRate = value; }
	}
	
	public float SuccessRate
	{
		get { return this.successRate; }
		set { this.successRate = value; }
	}
	
	public bool HasBeenReseted
	{
		get { return this.hasBeenReseted; }
		set { this.hasBeenReseted = value; }
	}

	public bool AlreadyPlanned
	{
		get { return this.alreadyPlanned; }
		set { this.alreadyPlanned = value; }
	}
}
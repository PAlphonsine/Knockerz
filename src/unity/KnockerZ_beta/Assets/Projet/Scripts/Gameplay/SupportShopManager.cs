using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SupportShopManager : MonoBehaviour {
	// Texte de planification du nombre de soutiens aériens à acheter
	[SerializeField]
	Text airSupportToBuyText;
	// Texte de planification du nombre d'appats à acheter
	[SerializeField]
	Text meatSupportToBuyText;
	// Boutons de controle du nombre d'achats de soutiens aériens
	[SerializeField]
	Button moreAirSupportToBuyButton;
	[SerializeField]
	Button lessAirSupportToBuyButton;
	// Boutons de controle du nombre d'achats d'appats
	[SerializeField]
	Button moreMeatSupportToBuyButton;
	[SerializeField]
	Button lessMeatSupportToBuyButton;
	// Cout actuel
	[SerializeField]
	Text currentCostText;
	// Bouton de validation d'achat
	[SerializeField]
	Button buyButton;
	// Gestion des phases
	[SerializeField]
	PhasesManager phasesManager;
	// Gestion de l'inventaire
	[SerializeField]
	SupportInventoryManager supportInventoryManager;
	// Emplacement texte de l'Or
	[SerializeField]
	Text goldText;
	// Prix des soutiens disponibles à l'achat
	private int airSupportPrice;
	private int meatSupportPrice;
	// Nombre de soutiens planifiés pour l'achat
	private int airSupportToBuy;
	private int meatSupportToBuy;
	// Nombre maximum de soutiens possibles d'acheter selon l'or du joueur
	private int maxAirSupportToBuy;
	private int maxMeatSupportToBuy;
	// Prix d'achat actuel
	private int currentCost;

	// Use this for initialization
	void Start () {
		this.goldText.text = GameStats.Instance.Gold.ToString();
		this.airSupportPrice = 200;
		this.airSupportToBuy = 0;
		this.maxAirSupportToBuy = GameStats.Instance.Gold / this.airSupportPrice;
		this.meatSupportPrice = 50;
		this.meatSupportToBuy = 0;
		this.maxMeatSupportToBuy = GameStats.Instance.Gold / this.meatSupportPrice;
		this.currentCost = 0;
		this.currentCostText.text += this.currentCost;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate () {
		if (this.phasesManager.startgame == true)
		{
			this.airSupportToBuyText.text = this.airSupportToBuy.ToString();
			this.meatSupportToBuyText.text = this.meatSupportToBuy.ToString();

			if (this.phasesManager.startAction == false)
			{
				if (this.airSupportToBuy == 0)
				{
					this.moreAirSupportToBuyButton.interactable = true;
					this.lessAirSupportToBuyButton.interactable = false;
				}
				else if (this.airSupportToBuy >= this.maxAirSupportToBuy || this.currentCost + this.airSupportPrice > GameStats.Instance.Gold)
				{
					this.moreAirSupportToBuyButton.interactable = false;
					this.lessAirSupportToBuyButton.interactable = true;
					this.buyButton.interactable = true;
				}
				else
				{
					this.moreAirSupportToBuyButton.interactable = true;
					this.lessAirSupportToBuyButton.interactable = true;
					this.buyButton.interactable = true;
				}

				if (this.meatSupportToBuy == 0)
				{
					this.moreMeatSupportToBuyButton.interactable = true;
					this.lessMeatSupportToBuyButton.interactable = false;
				}
				else if (this.meatSupportToBuy >= this.maxMeatSupportToBuy || this.currentCost + this.meatSupportPrice > GameStats.Instance.Gold)
				{
					this.moreMeatSupportToBuyButton.interactable = false;
					this.lessMeatSupportToBuyButton.interactable = true;
					this.buyButton.interactable = true;
				}
				else
				{
					this.moreMeatSupportToBuyButton.interactable = true;
					this.lessMeatSupportToBuyButton.interactable = true;
					this.buyButton.interactable = true;
				}

				if (this.airSupportToBuy == 0 && this.meatSupportToBuy == 0)
				{
					this.buyButton.interactable = false;
				}

				if (this.currentCost >= GameStats.Instance.Gold)
				{
					this.moreAirSupportToBuyButton.interactable = false;
					this.moreMeatSupportToBuyButton.interactable = false;
					if (this.currentCost > GameStats.Instance.Gold)
					{
						this.buyButton.interactable = false;
					}
				}
			}
			else
			{
				this.moreAirSupportToBuyButton.interactable = false;
				this.lessAirSupportToBuyButton.interactable = false;
				this.moreMeatSupportToBuyButton.interactable = false;
				this.lessMeatSupportToBuyButton.interactable = false;
				this.buyButton.interactable = false;
			}
		}
	}

	public void MoreAirSupportToBuy()
	{
		this.airSupportToBuy++;
		this.currentCost += this.airSupportPrice;
		this.currentCostText.text = "Cout actuel : " + this.currentCost;
		if (this.airSupportToBuy > this.maxAirSupportToBuy)
		{
			this.airSupportToBuy--;
		}
	}

	public void LessAirSupportToBuy()
	{
		this.airSupportToBuy--;
		this.currentCost -= this.airSupportPrice;
		this.currentCostText.text = "Cout actuel : " + this.currentCost;
		if (this.airSupportToBuy * this.airSupportPrice < 0)
		{
			this.airSupportToBuy++;
		}
	}

	public void MoreMeatSupportToBuy()
	{
		this.meatSupportToBuy++;
		this.currentCost += this.meatSupportPrice;
		this.currentCostText.text = "Cout actuel : " + this.currentCost;
		if (this.meatSupportToBuy > this.maxMeatSupportToBuy)
		{
			this.meatSupportToBuy--;
		}
	}

	public void LessMeatSupportToBuy()
	{
		this.meatSupportToBuy--;
		this.currentCost -= this.meatSupportPrice;
		this.currentCostText.text = "Cout actuel : " + this.currentCost;
		if (this.meatSupportToBuy * this.meatSupportPrice < 0)
		{
			this.meatSupportToBuy++;
		}
	}

	public void Buy()
	{
		GameStats.Instance.Gold -= this.currentCost;
		this.goldText.text = GameStats.Instance.Gold.ToString();

		this.supportInventoryManager.AirSupportNumber += this.airSupportToBuy;
		this.supportInventoryManager.MeatSupportNumber += this.meatSupportToBuy;

		this.airSupportToBuy = 0;
		this.meatSupportToBuy = 0;
		this.currentCost = 0;
		this.currentCostText.text = "0";
	}

	public void Cancel()
	{
		this.airSupportToBuy = 0;
		this.meatSupportToBuy = 0;
		this.currentCost = 0;
		this.currentCostText.text = "0";
	}

	// Accesseurs
	public int AirSupportPrice
	{
		get { return this.airSupportPrice; }
		set { this.airSupportPrice = value; }
	}

	public int AirSupportToBuy
	{
		get { return this.airSupportToBuy; }
		set { this.airSupportToBuy = value; }
	}
	
	public int MaxAirSupportToBuy
	{
		get { return this.maxAirSupportToBuy; }
		set { this.maxAirSupportToBuy = value; }
	}
	
	public int MeatSupportPrice
	{
		get { return this.meatSupportPrice; }
		set { this.meatSupportPrice = value; }
	}
	
	public int MeatSupportToBuy
	{
		get { return this.meatSupportToBuy; }
		set { this.meatSupportToBuy = value; }
	}
	
	public int MaxMeatSupportToBuy
	{
		get { return this.maxMeatSupportToBuy; }
		set { this.maxMeatSupportToBuy = value; }
	}

	public int CurrentCost
	{
		get { return this.currentCost; }
	}
}

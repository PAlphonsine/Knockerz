using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SupportShopManager : MonoBehaviour
{
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
	// Prix d'achat actuel
	private int currentCost;
	
	// Use this for initialization
	void Start ()
	{
		this.goldText.text = GameStats.Instance.Gold.ToString();
		this.airSupportPrice = 200;
		this.airSupportToBuy = 0;
		this.meatSupportPrice = 50;
		this.meatSupportToBuy = 0;
		this.currentCost = 0;
		this.currentCostText.text += this.currentCost;
	}
	
	void FixedUpdate ()
	{
		// Si la partie a commencé
		if (this.phasesManager.startgame == true)
		{
			// Les textes du nombre de supports dans le panier correspondent au nombre de supports que le joueur envisage d'acheter
			this.airSupportToBuyText.text = this.airSupportToBuy.ToString();
			this.meatSupportToBuyText.text = this.meatSupportToBuy.ToString();
			
			// Si l'on est en phase de réflexion
			if (this.phasesManager.startAction == false)
			{
				// Si le joueur ne prévoit pas d'acheter de support aérien
				if (this.airSupportToBuy == 0 && this.currentCost + this.airSupportPrice < GameStats.Instance.Gold)
				{
					// Il peut en ajouter à son panier quand il le souhaite
					this.moreAirSupportToBuyButton.interactable = true;
					// Il ne peut pas en retirer car il en a 0
					this.lessAirSupportToBuyButton.interactable = false;
				}
				// Sinon, si le joueur prévoit d'acheter un nombre trop important de supports aériens
				// ou si le montant de ses achats additionné à un éventuel achat supplémentaire dépasse l'or qu'il possède
				else if (this.currentCost + this.airSupportPrice > GameStats.Instance.Gold && this.currentCost > 0 && this.airSupportToBuy != 0)
				{
					// Il ne peut pas en rajouter à son panier ...
					this.moreAirSupportToBuyButton.interactable = false;
					// ... mais il peut en retirer ...
					this.lessAirSupportToBuyButton.interactable = true;
					// ... ou acheter les articles de son panier
					this.buyButton.interactable = true;
				}
				else if (this.airSupportToBuy == 0)
				{
					this.lessAirSupportToBuyButton.interactable = false;
				}
				else
				{
					// Sinon, le joueur peut rajouter ou enlever des articles de son panier ...
					this.moreAirSupportToBuyButton.interactable = true;
					this.lessAirSupportToBuyButton.interactable = true;
					// ... et valider ses achats
					this.buyButton.interactable = true;
				}

				// Si le joueur ne prévoit pas d'acheter de morceau de viande
				if (this.meatSupportToBuy == 0 && this.currentCost + this.meatSupportPrice < GameStats.Instance.Gold)
				{
					// Il peut en ajouter à son panier quand il le souhaite
					this.moreMeatSupportToBuyButton.interactable = true;
					// Il ne peut pas en retirer car il en a 0
					this.lessMeatSupportToBuyButton.interactable = false;
				}
				// Sinon, si le joueur prévoit d'acheter un nombre trop important de morceaux de viande
				// ou si le montant de ses achats additionné à un éventuel achat supplémentaire dépasse l'or qu'il possède
				else if (this.currentCost + this.meatSupportPrice > GameStats.Instance.Gold && this.currentCost > 0 && this.meatSupportToBuy != 0)
				{
					// Il ne peut pas en rajouter à son panier ...
					this.moreMeatSupportToBuyButton.interactable = false;
					// ... mais il peut en retirer ...
					this.lessMeatSupportToBuyButton.interactable = true;
					// ... ou acheter les articles de son panier
					this.buyButton.interactable = true;
				}
				else if (this.meatSupportToBuy == 0)
				{
					this.lessMeatSupportToBuyButton.interactable = false;
				}
				else
				{
					// Sinon, le joueur peut rajouter ou enlever des articles de son panier ...
					this.moreMeatSupportToBuyButton.interactable = true;
					this.lessMeatSupportToBuyButton.interactable = true;
					// ... et valider ses achats
					this.buyButton.interactable = true;
				}
				
				// Si le joueur n'a ajouté aucun article à son panier
				if (this.airSupportToBuy <= 0 && this.meatSupportToBuy <= 0)
				{
					// Il ne peut rien acheter
					this.buyButton.interactable = false;
					this.lessAirSupportToBuyButton.interactable = false;
					this.lessMeatSupportToBuyButton.interactable = false;
				}
				
				// Si le prix du panier du joueur est supérieur ou égal au montant d'or qu'il possède
				if (this.currentCost >= GameStats.Instance.Gold)
				{
					// Il ne peut ni rajouter d'article à son panier ...
					this.moreAirSupportToBuyButton.interactable = false;
					// ... ni en enlever
					this.moreMeatSupportToBuyButton.interactable = false;
					// Si le prix du panier du joueur est strictement supérieur au montant d'or qu'il possède
					if (this.currentCost > GameStats.Instance.Gold)
					{
						// Le joueur ne peut pas acheter
						this.buyButton.interactable = false;
					}
				}
			}
			// Sinon, si l'on est en phase d'action
			else
			{
				// Le joueur ne peut pas interagir avec la boutique
				this.moreAirSupportToBuyButton.interactable = false;
				this.lessAirSupportToBuyButton.interactable = false;
				this.moreMeatSupportToBuyButton.interactable = false;
				this.lessMeatSupportToBuyButton.interactable = false;
				this.buyButton.interactable = false;
			}
		}
	}
	
	// Méthode d'ajout au panier d'un soutien aérien
	public void MoreAirSupportToBuy()
	{
		// On incrémente le nombre de soutiens aériens dont l'achat est planifié
		this.airSupportToBuy++;
		// Le prix du panier augmente du prix d'un soutien aérien
		this.currentCost += this.airSupportPrice;
		// Le texte informant le joueur du prix de son panier change en conséquence
		this.currentCostText.text = "Cout actuel : " + this.currentCost;
	}
	
	// Méthode de retrait du panier d'un soutien aérien
	public void LessAirSupportToBuy()
	{
		// On décrémente le nombre de soutiens aériens dont l'achat est planifié
		this.airSupportToBuy--;
		// Le prix du panier baisse du prix d'un soutien aérien
		this.currentCost -= this.airSupportPrice;
		// Le texte informant le joueur du prix de son panier change en conséquence
		this.currentCostText.text = "Cout actuel : " + this.currentCost;
		// Si le nombre de soutiens aériens que le joueur souhaite acheter multiplié par le prix d'un soutien aérien est inférieur à 0
		if (this.airSupportToBuy * this.airSupportPrice < 0)
		{
			// On ajoute un soutien aérien de son panier
			this.airSupportToBuy++;
		}
	}
	
	// Méthode d'ajout au panier d'un morceau de viande
	public void MoreMeatSupportToBuy()
	{
		// On incrémente le nombre de morceaux de viande dont l'achat est planifié
		this.meatSupportToBuy++;
		// Le prix du panier augmente du prix d'un morceau de viande
		this.currentCost += this.meatSupportPrice;
		// Le texte informant le joueur du prix de son panier change en conséquence
		this.currentCostText.text = "Cout actuel : " + this.currentCost;
	}
	
	// Méthode de retrait au panier d'un morceau de viande
	public void LessMeatSupportToBuy()
	{
		// On décrémente le nombre de morceaux de viande dont l'achat est planifié
		this.meatSupportToBuy--;
		// Le prix du panier baisse du prix d'un morceau de viande
		this.currentCost -= this.meatSupportPrice;
		// Le texte informant le joueur du prix de son panier change en conséquence
		this.currentCostText.text = "Cout actuel : " + this.currentCost;
		// Si le nombre de morceaux de viande que le joueur souhaite acheter multiplié par le prix d'un morceau de viande est inférieur à 0
		if (this.meatSupportToBuy * this.meatSupportPrice < 0)
		{
			// On ajoute un morceau de viande de son panier
			this.meatSupportToBuy++;
		}
	}
	
	// Méthode de validation d'achat
	public void Buy()
	{
		// On retire du montant d'or du joueur le prix de son panier
		GameStats.Instance.Gold -= this.currentCost;
		// Le texte de l'or du joueur correspond au nouveau montant d'or que possède le joueur
		this.goldText.text = GameStats.Instance.Gold.ToString();
		// Les soutiens achetés s'ajoutent à l'inventaire du joueur
		this.supportInventoryManager.AirSupportNumber += this.airSupportToBuy;
		this.supportInventoryManager.MeatSupportNumber += this.meatSupportToBuy;
		// Les paramètres d'achat et de cout du panier sont réinitialisés
		this.airSupportToBuy = 0;
		this.meatSupportToBuy = 0;
		this.currentCost = 0;
		this.currentCostText.text = "Cout actuel : 0";
		// Les textes du nombre de supports dans le panier reviennent à 0
		this.airSupportToBuyText.text = "0";
		this.meatSupportToBuyText.text = "0";
		// Les boutons d'achat et de retrait sont désactivés
		this.lessAirSupportToBuyButton.interactable = false;
		this.lessMeatSupportToBuyButton.interactable = false;
		this.buyButton.interactable = false;
	}
	
	// Méthode d'annulation d'achat
	public void Cancel()
	{
		// Les paramètres d'achat et de cout du panier sont réinitialisés
		this.airSupportToBuy = 0;
		this.meatSupportToBuy = 0;
		this.currentCost = 0;
		this.currentCostText.text = "Cout actuel : 0";
		// Les textes du nombre de supports dans le panier reviennent à 0
		this.airSupportToBuyText.text = "0";
		this.meatSupportToBuyText.text = "0";
		// Les boutons d'achat et de retrait sont désactivés
		this.lessAirSupportToBuyButton.interactable = false;
		this.lessMeatSupportToBuyButton.interactable = false;
		this.buyButton.interactable = false;
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
	
	public int CurrentCost
	{
		get { return this.currentCost; }
	}
}
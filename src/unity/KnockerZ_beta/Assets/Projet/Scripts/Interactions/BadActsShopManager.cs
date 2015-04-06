using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BadActsShopManager : MonoBehaviour
{
	// Gestion des phases
	[SerializeField]
	PhasesManager phaseManager;
	// Gestion de l'inventaire
	[SerializeField]
	BadActsInventoryManager badActsInventoryManager;
	// Nom, image, description et caractéristiques des coups fourrés
	[SerializeField]
	Text nameText;
	[SerializeField]
	Text descriptionText;
	[SerializeField]
	Text featuresText;
	// Boutons des différents coups fourrés
	[SerializeField]
	Button fogButton;
	[SerializeField]
	Button zombieBaitButton;
	// Bouton d'achat et de vente
	[SerializeField]
	Button buyButton;
	[SerializeField]
	Button sellButton;
	// Prix des coups fourrés disponibles à l'achat
	private int fogPrice;
	private int zombieBaitPrice;
	// Type de coup fourré sélectionné
	private string badActType;
	
	// Use this for initialization
	void Start ()
	{
		this.fogPrice = 100;
		this.zombieBaitPrice = 250;
		this.badActType = "";
	}
	
	void FixedUpdate ()
	{
		// Si la partie a commencé
		if (this.phaseManager.startgame == true)
		{
			// Si l'on est en phase de réflexion
			if (this.phaseManager.startAction == false)
			{
				// Si aucun coup fourré n'est sélectionné
				if (this.badActType == "")
				{
					// On ne peut acheter ni vendre de coups fourrés
					this.buyButton.interactable = false;
					this.sellButton.interactable = false;
				}
				// Sinon, si un coup fourré est sélectionné
				else
				{
					// Si c'est la grenade fumigène
					if (this.badActType == "Fog")
					{
						// Si le prix de la grenade est plus grand que l'or du joueur ...
						if (this.fogPrice > GameStats.Instance.Gold)
						{
							// ... il ne peut pas acheter
							this.buyButton.interactable = false;
						}
						else
						{
							// Sinon il peut
							this.buyButton.interactable = true;
						}
						
						// Si le joueur possède déjà des grenades ...
						if (this.badActsInventoryManager.FogsNumber > 0)
						{
							// ... il peut en vendre
							this.sellButton.interactable = true;
						}
						else
						{
							// Sinon il ne peut pas
							this.sellButton.interactable = false;
						}
					}
					// Sinon, si le coup fourré sélectionné est l'appat pour Zombie
					else if (this.badActType == "ZombieBait")
					{
						// Si le prix de l'appat est plus grand que l'or du joueur ...
						if (this.zombieBaitPrice > GameStats.Instance.Gold)
						{
							// ... il ne peut pas acheter
							this.buyButton.interactable = false;
						}
						else
						{
							// Sinon il peut
							this.buyButton.interactable = true;
						}
						
						// Si le joueur possède déjà des appats ...
						if (this.badActsInventoryManager.ZombieBaitsNumber > 0)
						{
							// ... il peut en vendre
							this.sellButton.interactable = true;
						}
						else
						{
							// Sinon il ne peut pas
							this.sellButton.interactable = false;
						}
					}
					else
					{
						this.buyButton.interactable = true;
					}
				}
			}
			// Sinon, si l'on est en phase d'action
			else
			{
				// Le joueur ne peut ni acheter ...
				this.buyButton.interactable = false;
				// ... ni vendre
				this.sellButton.interactable = false;
			}
		}
	}
	
	// Méthode de définition du coup fourré sélectionné dans la boutique
	public void SetBadActType(string type)
	{
		// Si le joueur clique sur la grenade fumigène
		if (type == "Fog")
		{
			// On change le type de coup fourré sélectionné
			this.badActType = type;
			// On change toute la partie description du coup fourré
			this.nameText.text = "Grenades Fumigènes";
			//this.descriptionText.text = "Vos survivants lancent des grenades fumigènes dans la base adverse pour l'empecher de voir une partie de sa carte. En espérant qu'il ait une bonne audition ...";
			//this.featuresText.text = "Durée : 30 secondes";
			this.descriptionText.text = "Vos survivants lancent des grenades fumigènes dans la base adverse.";
			this.featuresText.text = "Cache une partie de la carte de l'adversaire pendant 30 secondes";
			// Le texte du bouton d'achat devient le prix d'achat de la grenade
			this.buyButton.GetComponentInChildren<Text>().text = this.fogPrice.ToString();
			// Le texte du bouton de vente devient le prix de vente de la grenade
			this.sellButton.GetComponentInChildren<Text>().text = ((int)(this.fogPrice * 0.80f)).ToString();
		}
		// Si le joueur clique sur l'appat pour Zombie
		if (type == "ZombieBait")
		{
			// On change le type de coup fourré sélectionné
			this.badActType = type;
			// On change toute la partie description du coup fourré
			this.nameText.text = "Appats pour zombies";
			//this.descriptionText.text = "Les appats pour zombies de la marque KillerZ ! A utiliser en cas d'apocalypse zombies, ne pas laisser à la portée des enfants (sauf des enfants-zombies).";
			this.descriptionText.text = "Vos survivants placent des appats pour Zombies dans la base adverse.";
			this.featuresText.text = "Envoie 5 zombies de type aléatoire (sauf boss) à l'adversaire";
			// Le texte du bouton d'achat devient le prix d'achat de l'appat
			this.buyButton.GetComponentInChildren<Text>().text = this.zombieBaitPrice.ToString();
			// Le texte du bouton de vente devient le prix de vente de l'appat
			this.sellButton.GetComponentInChildren<Text>().text = ((int)(this.zombieBaitPrice * 0.80f)).ToString();
		}
	}
	
	// Méthode d'achat d'un coup fourré
	public void Buy()
	{
		// Si le coup fourré sélectionné est la grenade fumigène
		if (this.badActType == "Fog")
		{
			// On retire de l'or du joueur le prix de la grenade
			GameStats.Instance.Gold -= this.fogPrice;
			// On ajoute la grenade à son inventaire
			this.badActsInventoryManager.FogsNumber++;
		}
		// Si le coup fourré sélectionné est l'appat pour Zombie
		if (this.badActType == "ZombieBait")
		{
			// On retire de l'or du joueur le prix de l'appat
			GameStats.Instance.Gold -= this.zombieBaitPrice;
			// On ajoute l'appat à son inventaire
			this.badActsInventoryManager.ZombieBaitsNumber++;
		}
	}
	
	// Méthode de vente d'un coup fourré
	public void Sell()
	{
		// Si le coup fourré sélectionné est la grenade fumigène
		if (this.badActType == "Fog")
		{
			// On ajoute à l'or du joueur le prix de vente de la grenade
			GameStats.Instance.Gold += (int)(this.fogPrice * 0.80f);
			// On retire la grenade à son inventaire
			this.badActsInventoryManager.FogsNumber--;
		}
		// Si le coup fourré sélectionné est l'appat pour Zombie
		if (this.badActType == "ZombieBait")
		{
			// On ajoute à l'or du joueur le prix de vente de l'appat
			GameStats.Instance.Gold += (int)(this.zombieBaitPrice * 0.80f);
			// On retire l'appat à son inventaire
			this.badActsInventoryManager.ZombieBaitsNumber--;
		}
	}
	
	// Accesseurs
	public int FogPrice
	{
		get { return this.fogPrice; }
		set { this.fogPrice = value; }
	}
	
	public int ZombieBaitPrice
	{
		get { return this.zombieBaitPrice; }
		set { this.zombieBaitPrice = value; }
	}
	
	public string BadActType
	{
		get { return this.badActType; }
		set { this.badActType = value; }
	}
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurretMenu : MonoBehaviour
{
	// Type du menu de la tourelle
	public int TurretMenuType = 0;
	// Emplacement de tourelle parent du menu de la tourelle
	public EmptyPlace _parent;
	// Portée du RayCast
	float limiteDetection = 250.0f ;
	// Booléen de controle de paiement du joueur
	bool _mustPay = false;
	// Type de la tourelle à acheter
	int typeToBuy;
	// Lien avec le script référence pour afficher et récuperer les caractéristiques des tourelles
	[SerializeField] TurretsComponentsBook _turretsComponentsBook;
	
	void Update ()
	{
		// Si le joueur clique
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
			RaycastHit hit;
			// Si le joueur clique sur un menu
			if (Physics.Raycast(ray, out hit, limiteDetection) && hit.collider.gameObject == this.gameObject)
			{
				// Si le joueur achète une tourelle à distance
				if (TurretMenuType == 0)
				{
					// Si la soustraction entre les ressources du joueur et le prix d'une tourelle à distance est supérieure à 0
					if (GameStats.Instance.RessourcesMat - _turretsComponentsBook.CostTDM[0] >= 0 
					    && GameStats.Instance.RessourcesWeap - _turretsComponentsBook.CostTDA[0] >= 0 
					    && GameStats.Instance.Population - _turretsComponentsBook.CostTDP[0] >= 0)
					{
						// On effectue les retraits de ressource
						GameStats.Instance.RessourcesMat -= _turretsComponentsBook.CostTDM[0];
						GameStats.Instance.RessourcesWeap -= _turretsComponentsBook.CostTDA[0];
						GameStats.Instance.Population -= _turretsComponentsBook.CostTDP[0];
						// On synchronise l'achat de la tourelle
						_parent.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 1);
					}
				}
				// Sinon, si le joueur achète une tourelle corps-à-corps
				else
				{
					// Si la soustraction entre les ressources du joueur et le prix d'une tourelle corps-à-corps est supérieure à 0
					if (GameStats.Instance.RessourcesMat - _turretsComponentsBook.CostTHtoHM[0] >= 0 
					    && GameStats.Instance.RessourcesWeap - _turretsComponentsBook.CostTHtoHA[0] >= 0 
					    && GameStats.Instance.Population - _turretsComponentsBook.CostTHtoHP[0] >= 0)
					{
						// On effectue les retraits de ressource
						GameStats.Instance.RessourcesMat -= _turretsComponentsBook.CostTHtoHM[0];
						GameStats.Instance.RessourcesWeap -= _turretsComponentsBook.CostTHtoHA[0];
						GameStats.Instance.Population -= _turretsComponentsBook.CostTHtoHP[0];
						// On synchronise l'achat de la tourelle
						_parent.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 2);
					}
				}
				// On cache les infos sur la tourelle
				_turretsComponentsBook.HideInfos ();
			}
		}
	}
	
	void FixedUpdate()
	{
		// Si le joueur doit payer
		if (_mustPay)
		{
			// Il est en train de payer et n'aura plus à le faire après
			_mustPay = false;
			// Si c'est une tourelle à distance
			if(typeToBuy == 1)
			{
				// On active la tourelle à distance à partir de l'emplacement de tourelle
				_parent._turretD.SetActive(true);
				// Valeurs initials des caractéristiques de la tourelle
				_parent._turretAttackD.UpgradeBullet (_turretsComponentsBook.ValueTDDamages [0], _turretsComponentsBook.ValueTDRoF [0]);
				_parent._turretAttackD.UpgradeRange (_turretsComponentsBook.ValueTDRange [0]);
				// On désactive le menu
				_parent._turretD.GetComponent<TurretMenuSet>().DesactiveMenu();
			}
			else
			{
				// On active la tourelle corps-à-corps à partir de l'emplacement de tourelle
				_parent._turretHtoH.SetActive(true);
				// Valeurs initials des caractéristiques du fighter
				_parent._turretAttackHtoH.UpdateFighter(_turretsComponentsBook.ValueTHtoHDamages [0], _turretsComponentsBook.ValueTHtoHPv [0]);
				_parent._turretAttackHtoH.UpdateCriticalHitsFighter (0);
				_parent._turretAttackHtoH.UpdateDodgeFighter (0);
				// On désactive le menu
				_parent._turretHtoH.GetComponent<TurretMenuSet>().DesactiveMenu();
			}

			_parent.ConstructionParticles.Play();

			// On désactive l'emplacement de tourelle
			_parent.gameObject.SetActive (false);
		}
	}

	// Lorsque le joueur passe sa souris sur un menu
	void OnMouseEnter()
	{
		//display = true;
		// Si le menu est attaché à une tourelle à distance
		if (TurretMenuType == 0)
		{
			// On demande au script référence pour les caractéristiques des tourelles d'afficher les infos sur la tourelle Distance
			_turretsComponentsBook.ShowInfosTD();
		}
		// Sinon, si le menu est attaché à une tourelle corps-à-corps
		else
		{
			// On demande au script référence pour les caractéristiques des tourelles d'afficher les infos sur la tourelle CaC
			_turretsComponentsBook.ShowInfosTHtoH();
		}
	}
	
	// Lorsque la souris du joueur n'est plus sur un menu
	void OnMouseExit()
	{
		// On cache les infos sur les tourelles
		_turretsComponentsBook.HideInfos ();
	}
	
	// Méthode de validation de l'achat du joueur
	public void ClientWantToBuy(int type)
	{
		// On récupère le type de tourelle acheté
		this.typeToBuy = type;
		// Le joueur passe désormais au payment
		_mustPay = true;
	}
}

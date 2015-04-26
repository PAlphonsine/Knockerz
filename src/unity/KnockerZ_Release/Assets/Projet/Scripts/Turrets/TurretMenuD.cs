using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurretMenuD : MonoBehaviour
{
	// Type du menu de la tourelle
	public int TurretMenuSetype = 0;
	// Référence pour les caractéristiques de la tourelle
	[SerializeField]
	TurretsComponentsBook _turretsComponentsBook;
	// Mesh de base de la tourelle
	public Mesh basemesh;
	// Nouveau Mesh de la tourelle selon son type
	public Mesh newmeshType0;
	public Mesh newmeshType1;
	public Mesh newmeshType2;
	// Parent de la tourelle
	public GameObject _parent;
	// Portée du Raycast
	float limiteDetection = 250.0f;
	// Tourelle de base
	public GameObject _turret;
	// Gestion de la tourelle
	public TurretDistance _turretDistance;
	// Gestion de l'attaque de la tourelle
	public TurretAttackD _turretAttackD;

	 void Update ()
	{
		// TODO : Mettre des tableaux pour les prix et faire des fonctions pour les changements de mesh
		// Si le joueur clique
		if (Input.GetMouseButtonDown(0))
		{
			// On trace un rayon depuis la caméra à la position de la souris vers la scène
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			// Si le joueur a cliqué sur un menu
			if (Physics.Raycast(ray, out hit, limiteDetection) && hit.collider.gameObject == this.gameObject)
			{
				// Si le joueur a cliqué sur le menu de type 0
				if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 0)
				{
					// On verifie la possibilité de montée en niveau de la tourelle
					CheckLevelUpTurret();
				}
				// Si le joueur a cliqué sur le menu de type 1
				else if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 1)
				{
					// On défini la spécialité de la tourelle
					_turretDistance.spec = 1;
					// On verifie la possibilité de montée en niveau de la tourelle
					CheckLevelUpTurret();
				}
				// Si le joueur a cliqué sur le menu de type 2
				else if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 2)
				{
					// On défini la spécialité de la tourelle
					_turretDistance.spec = 2;
					// On verifie la possibilité de montée en niveau de la tourelle
					CheckLevelUpTurret();
				}
				// Si le joueur a cliqué sur le menu de type 4
				else if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 4)
				{
					// La tourelle est vendu
					SellTurret();
				}
			}
		}
		
	}

	// Lorsque le joueur passe sa souris sur un menu
	void OnMouseEnter()
	{
		// Si le joueur passe sa souris sur le menu de type 0
		if (TurretMenuSetype == 0)
		{
			// On demande au script référence pour les caractéristiques des tourelles d'afficher les infos sur la tourelle Distance
			_turretsComponentsBook.ShowInfosTD(_turretDistance.NivTurret, _turretDistance.spec);
		}
		else if (TurretMenuSetype == 1)
		{
			// On demande au script référence pour les caractéristiques des tourelles d'afficher les infos sur la tourelle Distance
			_turretsComponentsBook.ShowInfosTD(_turretDistance.NivTurret, 1);
		}
		else if (TurretMenuSetype == 2)
		{
			// On demande au script référence pour les caractéristiques des tourelles d'afficher les infos sur la tourelle Distance
			_turretsComponentsBook.ShowInfosTD(_turretDistance.NivTurret, 2);
		}
		else if (TurretMenuSetype == 4)
		{
			// Affichage l'écriteau de vente
			_turretsComponentsBook.ShowInfosSell();
		}
	}
	
	// Lorsque la souris du joueur n'est plus sur un menu
	void OnMouseExit()
	{
		// On cache les infos sur les tourelles
		_turretsComponentsBook.HideInfos ();
	}

	// Fonction demande de montée en niveau de la tourelle
	void CheckLevelUpTurret()
	{
		// On récupère le niveau de la tourelle
		int level = _turretDistance.NivTurret;
		// Si la tourelle n'a aucune spécialisation
		if (_turretDistance.spec == 0) {
			// Si la soustraction entre les matériaux du joueur et le cout de base est supérieure à 0
			if (GameStats.Instance.RessourcesMat - _turretsComponentsBook.CostTDM [level] >= 0 
				&& GameStats.Instance.RessourcesWeap - _turretsComponentsBook.CostTDA [level] >= 0 
				&& GameStats.Instance.Population - _turretsComponentsBook.CostTDP [level] >= 0) {
				// On effectue les retraits
				GameStats.Instance.RessourcesMat -= _turretsComponentsBook.CostTDM [level];
				GameStats.Instance.RessourcesWeap -= _turretsComponentsBook.CostTDA [level];
				GameStats.Instance.Population -= _turretsComponentsBook.CostTDP [level];
				// On incrémente le montant des dépenses en ressources
				_turretDistance.matSpent += _turretsComponentsBook.CostTDM[level];
				_turretDistance.weapSpent += _turretsComponentsBook.CostTDA[level];
				_turretDistance.popSpent +=  _turretsComponentsBook.CostTDP[level];
				// On synchronise la demande d'amélioration de la tourelle
				_turret.networkView.RPC ("DoSynchroD", RPCMode.AllBuffered, 1, 0);
			}
		} 
		// Si on est dans le cas d'une tourelle de spécialité 1 ou bien on le demande 
		else if (_turretDistance.spec == 1) 
		{
			// Si la soustraction entre les matériaux du joueur et le cout de la spécialisation 1 est supérieure à 0
			if (GameStats.Instance.RessourcesMat - _turretsComponentsBook.CostTDRifM(level) >= 0 
			    && GameStats.Instance.RessourcesWeap - _turretsComponentsBook.CostTDRifA(level) >= 0 
			    && GameStats.Instance.Population - _turretsComponentsBook.CostTDRifP(level) >= 0) {
				// On effectue les retraits
				GameStats.Instance.RessourcesMat -= _turretsComponentsBook.CostTDRifM(level);
				GameStats.Instance.RessourcesWeap -= _turretsComponentsBook.CostTDRifA(level);
				GameStats.Instance.Population -= _turretsComponentsBook.CostTDRifP(level);
				// On incrémente le montant des dépenses en ressources en fonction du cout de la spécialisation
				_turretDistance.matSpent += _turretsComponentsBook.CostTDRifM(level);
				_turretDistance.weapSpent += _turretsComponentsBook.CostTDRifA(level);
				_turretDistance.popSpent +=  _turretsComponentsBook.CostTDRifP(level);
				// On synchronise la demande d'amélioration de la tourelle
				_turret.networkView.RPC ("DoSynchroD", RPCMode.AllBuffered, 1, 1);
			}
		} 
		// Si on est dans le cas d'une tourelle de spécialité 2 ou bien on le demande 
		else if (_turretDistance.spec == 2) 
		{
			// Si la soustraction entre les matériaux du joueur et le cout de la spécialisation 2 est supérieure à 0
			if (GameStats.Instance.RessourcesMat - _turretsComponentsBook.CostTDSnipM(level) >= 0 
			    && GameStats.Instance.RessourcesWeap - _turretsComponentsBook.CostTDSnipA(level) >= 0 
			    && GameStats.Instance.Population - _turretsComponentsBook.CostTDSnipP(level) >= 0) {
				// On effectue les retraits
				GameStats.Instance.RessourcesMat -= _turretsComponentsBook.CostTDSnipM(level);
				GameStats.Instance.RessourcesWeap -= _turretsComponentsBook.CostTDSnipA(level);
				GameStats.Instance.Population -= _turretsComponentsBook.CostTDSnipP(level);
				// On incrémente le montant des dépenses en ressources en fonction du cout de la spécialisation
				_turretDistance.matSpent += _turretsComponentsBook.CostTDSnipM(level);
				_turretDistance.weapSpent += _turretsComponentsBook.CostTDSnipA(level);
				_turretDistance.popSpent +=  _turretsComponentsBook.CostTDSnipP(level);
				// On synchronise la demande d'amélioration de la tourelle
				_turret.networkView.RPC ("DoSynchroD", RPCMode.AllBuffered, 1, 2);
			}
		}
		// On cache les infos sur la tourelle
		_turretsComponentsBook.HideInfos ();
	}

	// Méthode de validation d'achat du joueur
	public void ClientWantToBuy(int spec)
	{
		// On appelle la fonction de montée en niveau
		LevelUpTurret (spec);
	}

	// Fonction de montée en niveau de la tourelle
	public void LevelUpTurret(int spec){
		// Si la tourelle n'a pas de spécialisation
		if (spec == 0) {
			// On change le Mesh de la tourelle
			_turret.GetComponent<MeshFilter> ().mesh = newmeshType0;
			// On augmente le niveau de la tourelle
			_turretDistance.LevelUpTurret ();
			// On améliore les projectiles de la tourelle en fonction du niveau de la tourelle et des données du book
			_turretAttackD.UpgradeBullet (_turretsComponentsBook.ValueTDDamages[_turretDistance.NivTurret], _turretsComponentsBook.ValueTDRoF[_turretDistance.NivTurret]);
			// On fait evoluer la portée de la tourelle
			_turretAttackD.UpgradeRange(_turretsComponentsBook.ValueTDRange[_turretDistance.NivTurret]);
			// On désactive le menu
			transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
			// On cache les infos sur la tourelle
			_turretsComponentsBook.HideInfos ();
		} 
		// Si la tourelle a la spécialisation 1
		else if (spec == 1) 
		{
			// On change le Mesh de la tourelle
			_turret.GetComponent<MeshFilter> ().mesh = newmeshType1;
			// On augmente le niveau de la tourelle
			_turretDistance.LevelUpTurret ();
			// On améliore les projectiles de la tourelle en fonction du niveau de la tourelle et des données du book
			_turretAttackD.UpgradeBullet (_turretsComponentsBook.ValueTDRifDam(_turretDistance.NivTurret), _turretsComponentsBook.ValueTDRifRoF(_turretDistance.NivTurret));
			// On fait evoluer la portée de la tourelle
			_turretAttackD.UpgradeRange (_turretsComponentsBook.ValueTDRifRange(_turretDistance.NivTurret));
			// On désactive le menu
			transform.GetComponentInParent<TurretMenuSet> ().DesactiveSpe();
			// On cache les infos sur la tourelle
			_turretsComponentsBook.HideInfos ();
		} 
		// Si la tourelle a la spécialisation 2
		else if (spec == 2) 
		{
			// On change le Mesh de la tourelle
			_turret.GetComponent<MeshFilter> ().mesh = newmeshType2;
			// On augmente le niveau de la tourelle
			_turretDistance.LevelUpTurret ();
			// On améliore les projectiles de la tourelle en fonction du niveau de la tourelle et des données du book
			_turretAttackD.UpgradeBullet (_turretsComponentsBook.ValueTDSnipDam(_turretDistance.NivTurret), _turretsComponentsBook.ValueTDSnipRoF(_turretDistance.NivTurret));
			// On fait evoluer la portée de la tourelle
			_turretAttackD.UpgradeRange (_turretsComponentsBook.ValueTDSnipRange(_turretDistance.NivTurret));
			// On désactive le menu
			transform.GetComponentInParent<TurretMenuSet> ().DesactiveSpe();
			// On cache les infos sur la tourelle
			_turretsComponentsBook.HideInfos ();
		}
		// On joue le feedback d'amélioration
		_turretDistance.AmeliorationParticles.Play ();
	}

	// Fonction de vente de la tourelle
	void SellTurret()
	{
		// On crédite son montant de matériaux de la moitié des matériaux qu'il a dépense dans l'amélioration de la tourelle vendue
		GameStats.Instance.RessourcesMat += _turretDistance.matSpent / 2 + _turretsComponentsBook.CostTDM[0];
		GameStats.Instance.RessourcesWeap += _turretDistance.weapSpent / 2 + _turretsComponentsBook.CostTDA[0];
		GameStats.Instance.Population += _turretDistance.popSpent + _turretsComponentsBook.CostTDP[0];
		// Remise à zero des ressources dépensées pour cette tourelle
		_turretDistance.matSpent = 0;
		_turretDistance.weapSpent = 0;
		_turretDistance.popSpent = 0;
		// On synchronise la vente
		_turret.networkView.RPC("DoSynchroD", RPCMode.AllBuffered, 2, 0);
		// On cache les infos
		_turretsComponentsBook.HideInfos ();
	}

	// Méthode de validation de vente du joueur
	public void ClientWantToSell()
	{
		// Le joueur vend sa tourelle
		Sell ();
	}
	
	// Méthode de vente d'une tourelle
	public void Sell()
	{
		// Le niveau de la tourelle redevient 1
		_turretDistance.NivTurret = 1;
		// Elle redevient une tourelle sans spécialité
		_turretDistance.spec = 0;
		// On remet les valeurs initials des caractéristiques de la tourelle
		_turretAttackD.UpgradeBullet (_turretsComponentsBook.ValueTDDamages [0], _turretsComponentsBook.ValueTDRoF [0]);
		_turretAttackD.UpgradeRange (_turretsComponentsBook.ValueTDRange[0]);
		// La tourelle retrouve son Mesh de base
		_turret.GetComponent<MeshFilter> ().mesh = basemesh;
		// La description de la tourelle est désactivée
		_turretsComponentsBook.HideInfos ();
		// On active le parent de la tourelle
		_parent.SetActive(true);
		// On désactive le menu de la tourelle
		_parent.GetComponent<TurretMenuSet>().DesactiveMenu();
		// On désactive le menu de la tourelle
		_turret.GetComponent<TurretMenuSet>().DesactiveMenu();
		// La tourelle est désactivée
		_turret.gameObject.SetActive (false);
	}
}

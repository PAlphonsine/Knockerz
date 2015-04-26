using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurretMenuH : MonoBehaviour
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
	// Tourelle	de base
	public GameObject _turret;
	// Gestion de la tourelle
	public TurretHtoH _turretHtoH;
	// Gestion de l'attaque du fighter
	public TurretAttackHtoH _turretAttackHtoH;

	void Update ()
	{
		// && Network.isClient
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
				if (hit.collider.gameObject.GetComponent<TurretMenuH>().TurretMenuSetype == 0)
				{
					// On verifie la possibilité de montée en niveau de la tourelle
					CheckLevelUpTurret();
				}
				// Si le joueur a cliqué sur le menu de type 1
				else if (hit.collider.gameObject.GetComponent<TurretMenuH>().TurretMenuSetype == 1)
				{
					// On défini la spécialité de la tourelle
					_turretHtoH.spec = 1;
					// On verifie la possibilité de montée en niveau de la tourelle
					CheckLevelUpTurret();
				}
				// Si le joueur a cliqué sur le menu de type 2
				else if (hit.collider.gameObject.GetComponent<TurretMenuH>().TurretMenuSetype == 2)
				{
					// On défini la spécialité de la tourelle
					_turretHtoH.spec = 2;
					// On verifie la possibilité de montée en niveau de la tourelle
					CheckLevelUpTurret();
				}
				// Si le joueur a cliqué sur le menu de type 3
				else if (hit.collider.gameObject.GetComponent<TurretMenuH>().TurretMenuSetype == 3)
				{
					// On donne la possibilité de changer la position du fighter
					ChangePosFighter();
				}
				// Si le joueur a cliqué sur le menu de type 4
				else if (hit.collider.gameObject.GetComponent<TurretMenuH>().TurretMenuSetype == 4)
				{
					// La tourelle est vendu
					SellTurret();
				}
			}
		}
		
	}

	// Changer la position du fighter
	void ChangePosFighter(){
		transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
		// On débloque le changement de position du fighter
		_turretHtoH.changePosFighter=true;
		// On cache les infos sur la tourelle
		_turretsComponentsBook.HideInfos ();
	}

	// Lorsque le joueur passe sa souris sur un menu
	void OnMouseEnter()
	{
		// Si le joueur passe sa souris sur le menu de type 0
		if (TurretMenuSetype == 0) 
		{
			// On demande au script référence pour les caractéristiques des tourelles d'afficher les infos sur la tourelle Distance
			_turretsComponentsBook.ShowInfosTHtoH (_turretHtoH.NivTurret, _turretHtoH.spec);
		} else if (TurretMenuSetype == 1) 
		{
			// On demande au script référence pour les caractéristiques des tourelles d'afficher les infos sur la tourelle Distance
			_turretsComponentsBook.ShowInfosTHtoH (_turretHtoH.NivTurret, 1);
		} else if (TurretMenuSetype == 2) 
		{
			// On demande au script référence pour les caractéristiques des tourelles d'afficher les infos sur la tourelle Distance
			_turretsComponentsBook.ShowInfosTHtoH (_turretHtoH.NivTurret, 2);
		} else if (TurretMenuSetype == 3) 
		{
			// Affichage l'écriteau du point de ralliement
			_turretsComponentsBook.ShowInfosPath ();
		} else if (TurretMenuSetype == 4) 
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
		int level = _turretHtoH.NivTurret;
		// Si la tourelle n'a aucune spécialisation
		if (_turretHtoH.spec == 0) {
			// Si la soustraction entre les matériaux du joueur et le cout de base est supérieure à 0
			if (GameStats.Instance.RessourcesMat - _turretsComponentsBook.CostTHtoHM [level] >= 0 
			    && GameStats.Instance.RessourcesWeap - _turretsComponentsBook.CostTHtoHA [level] >= 0 
			    && GameStats.Instance.Population - _turretsComponentsBook.CostTHtoHP [level] >= 0) {
				// On effectue les retraits
				GameStats.Instance.RessourcesMat -= _turretsComponentsBook.CostTHtoHM [level];
				GameStats.Instance.RessourcesWeap -= _turretsComponentsBook.CostTHtoHA [level];
				GameStats.Instance.Population -= _turretsComponentsBook.CostTHtoHP [level];
				// On incrémente le montant des dépenses en ressources
				_turretHtoH.matSpent += _turretsComponentsBook.CostTHtoHM[level];
				_turretHtoH.weapSpent += _turretsComponentsBook.CostTHtoHA[level];
				_turretHtoH.popSpent +=  _turretsComponentsBook.CostTHtoHP[level];
				// On synchronise la demande d'amélioration de la tourelle
				_turret.networkView.RPC ("DoSynchroHtoH", RPCMode.AllBuffered, 1, 0);
			}
		} 
		// Si on est dans le cas d'une tourelle de spécialité 1 ou bien on le demande 
		else if (_turretHtoH.spec == 1) 
		{
			// Si la soustraction entre les matériaux du joueur et le cout de la spécialisation 1 est supérieure à 0
			if (GameStats.Instance.RessourcesMat - _turretsComponentsBook.CostTHtoHCriticM(level) >= 0 
			    && GameStats.Instance.RessourcesWeap - _turretsComponentsBook.CostTHtoHCriticA(level) >= 0 
			    && GameStats.Instance.Population - _turretsComponentsBook.CostTHtoHCriticP(level) >= 0) {
				// On effectue les retraits
				GameStats.Instance.RessourcesMat -= _turretsComponentsBook.CostTHtoHCriticM(level);
				GameStats.Instance.RessourcesWeap -= _turretsComponentsBook.CostTHtoHCriticA(level);
				GameStats.Instance.Population -= _turretsComponentsBook.CostTHtoHCriticP(level);
				// On incrémente le montant des dépenses en ressources en fonction du cout de la spécialisation
				_turretHtoH.matSpent += _turretsComponentsBook.CostTHtoHCriticM(level);
				_turretHtoH.weapSpent += _turretsComponentsBook.CostTHtoHCriticA(level);
				_turretHtoH.popSpent +=  _turretsComponentsBook.CostTHtoHCriticP(level);
				// On synchronise la demande d'amélioration de la tourelle
				_turret.networkView.RPC ("DoSynchroHtoH", RPCMode.AllBuffered, 1, 1);
			}
		} 
		// Si on est dans le cas d'une tourelle de spécialité 2 ou bien on le demande 
		else if (_turretHtoH.spec == 2) 
		{
			// Si la soustraction entre les matériaux du joueur et le cout de la spécialisation 2 est supérieure à 0
			if (GameStats.Instance.RessourcesMat - _turretsComponentsBook.CostTHtoHDodgeM(level) >= 0 
			    && GameStats.Instance.RessourcesWeap - _turretsComponentsBook.CostTHtoHDodgeA(level) >= 0 
			    && GameStats.Instance.Population - _turretsComponentsBook.CostTHtoHDodgeP(level) >= 0) {
				// On effectue les retraits
				GameStats.Instance.RessourcesMat -= _turretsComponentsBook.CostTHtoHDodgeM(level);
				GameStats.Instance.RessourcesWeap -= _turretsComponentsBook.CostTHtoHDodgeA(level);
				GameStats.Instance.Population -= _turretsComponentsBook.CostTHtoHDodgeP(level);
				// On incrémente le montant des dépenses en ressources en fonction du cout de la spécialisation
				_turretHtoH.matSpent += _turretsComponentsBook.CostTHtoHDodgeM(level);
				_turretHtoH.weapSpent += _turretsComponentsBook.CostTHtoHDodgeA(level);
				_turretHtoH.popSpent +=  _turretsComponentsBook.CostTHtoHDodgeP(level);
				// On synchronise la demande d'amélioration de la tourelle
				_turret.networkView.RPC ("DoSynchroHtoH", RPCMode.AllBuffered, 1, 2);
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
			_turretHtoH.LevelUpTurret ();
			// On améliore les projectiles de la tourelle en fonction du niveau de la tourelle et des données du book
			_turretAttackHtoH.UpdateFighter (_turretsComponentsBook.ValueTHtoHDamages[_turretHtoH.NivTurret], _turretsComponentsBook.ValueTHtoHPv[_turretHtoH.NivTurret]);
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
			_turretHtoH.LevelUpTurret ();
			// On améliore les projectiles de la tourelle en fonction du niveau de la tourelle et des données du book
			_turretAttackHtoH.UpdateFighter (_turretsComponentsBook.ValueTHtoHCriticDam(_turretHtoH.NivTurret), _turretsComponentsBook.ValueTHtoHCriticPv(_turretHtoH.NivTurret));
			_turretAttackHtoH.UpdateCriticalHitsFighter (_turretsComponentsBook.ValueTHtoHCriticalHits (_turretHtoH.NivTurret));
			_turretAttackHtoH.UpdateDodgeFighter (_turretsComponentsBook.ValueTHtoHDodge (_turretHtoH.NivTurret));
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
			_turretHtoH.LevelUpTurret ();
			// On améliore les projectiles de la tourelle en fonction du niveau de la tourelle et des données du book
			_turretAttackHtoH.UpdateFighter (_turretsComponentsBook.ValueTHtoHDodgeDam(_turretHtoH.NivTurret), _turretsComponentsBook.ValueTHtoHDodgePv(_turretHtoH.NivTurret));
			_turretAttackHtoH.UpdateCriticalHitsFighter (_turretsComponentsBook.ValueTHtoHCriticalHits (_turretHtoH.NivTurret));
			_turretAttackHtoH.UpdateDodgeFighter (_turretsComponentsBook.ValueTHtoHDodge (_turretHtoH.NivTurret));
			// On désactive le menu
			transform.GetComponentInParent<TurretMenuSet> ().DesactiveSpe();
			// On cache les infos sur la tourelle
			_turretsComponentsBook.HideInfos ();
		}
		// On joue le feedback d'amélioration
		_turretHtoH.AmeliorationParticles.Play ();
	}
	
	// Fonction de vente de la tourelle
	void SellTurret()
	{
		// On crédite son montant de matériaux de la moitié des matériaux qu'il a dépense dans l'amélioration de la tourelle vendue
		GameStats.Instance.RessourcesMat += _turretHtoH.matSpent / 2 + _turretsComponentsBook.CostTHtoHM[0];
		GameStats.Instance.RessourcesWeap += _turretHtoH.weapSpent / 2 + _turretsComponentsBook.CostTHtoHA[0];
		GameStats.Instance.Population += _turretHtoH.popSpent + _turretsComponentsBook.CostTHtoHP[0];
		// Remise à zero des ressources dépensées pour cette tourelle
		_turretHtoH.matSpent = 0;
		_turretHtoH.weapSpent = 0;
		_turretHtoH.popSpent = 0;
		// On synchronise la vente
		_turret.networkView.RPC("DoSynchroHtoH", RPCMode.AllBuffered, 2, 0);
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
		_turretHtoH.NivTurret = 1;
		// Elle redevient une tourelle sans spécialité
		_turretHtoH.spec = 0;
		// On remet les valeurs initials des caractéristiques du fighter
		_turretAttackHtoH.UpdateFighter (_turretsComponentsBook.ValueTHtoHDamages [0], _turretsComponentsBook.ValueTHtoHPv [0]);
		_turretAttackHtoH.UpdateCriticalHitsFighter (_turretsComponentsBook.ValueTHtoHCriticalHits (0));
		_turretAttackHtoH.UpdateDodgeFighter(_turretsComponentsBook.ValueTHtoHDodge (0));
		_turretAttackHtoH.MustWait = false;
		_turretAttackHtoH.EnableFighter();
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
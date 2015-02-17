using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurretMenuD : MonoBehaviour {
	
	public int TurretMenuSetype=0;
	int costT = 50;
	int costSpé1Niv0 = 10;
	int costSpé2Niv0 = 15;
	int costSpé3Niv0 = 20;
	int costSpé1Niv1 = 20;
	int costSpé1Niv2 = 30;
	public Mesh basemesh;
	public Mesh newmeshType0;
	public Mesh newmeshType1;
	public Mesh newmeshType2;
	public GameObject _parent;
	string tagObjet = "Menu" ;
	float limiteDetection = 250.0f ;
	
	public GameObject _turret;
	public TurretDistance _turretDistance;
	public TurretAttackD _turretAttackD;
	
	public bool _mustPay = false;
	
	public GameObject _description;
	public Text _descriptionText;
	//bool display = false;

	int weapSpent;
	int matSpent;

	// Use this for initialization
	void Start () {
	}
	
	void OnMouseEnter()
	{
		//display = true;
		_description.SetActive (true);
		if (TurretMenuSetype == 0) {
			_descriptionText.text = "Amélioration archer";
		} else { if(TurretMenuSetype == 1){
				_descriptionText.text = "Amélioration guerrier";
			}else{
				_descriptionText.text = "Amélioration mage";
			}
		}
	}
	
	void OnMouseExit(){
		_description.SetActive (false);
	}
	
	/*if (display == true) {
	_description.SetActive (true);
}else{
	_description.SetActive(false);
}*/
	
	void Update () {
		// && Network.isClient
		//Mettre des tableaux pour les prix et faire des fonctions pour les changements de mesh
		if(Input.GetMouseButtonDown(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, limiteDetection) && hit.transform.CompareTag(tagObjet)){
				if(_turretDistance.NivTurret == 1)
				{
					if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 0) {
						if (GameStats.Instance.RessourcesMat - costSpé1Niv0 >= 0) {
							GameStats.Instance.RessourcesMat -= costSpé1Niv0;
							matSpent+= costSpé1Niv0;
							//Debug.Log ("Ressources restantes: " + GameStats.Instance.RessourcesMat);
							_turret.GetComponent<MeshFilter> ().mesh = newmeshType0;
							transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
							_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 1);
							_turretDistance.LevelUpTurret();
							_turretAttackD.UpgradeBullet();
							_description.SetActive (false);
						}
					}else
					{ 
						if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 1)
						{
							if (GameStats.Instance.RessourcesMat - costSpé2Niv0 >= 0) {
								GameStats.Instance.RessourcesMat -= costSpé2Niv0;
								matSpent+= costSpé2Niv0;
								_turret.GetComponent<MeshFilter> ().mesh = newmeshType1;
								transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
								_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 1);
								_turretDistance.LevelUpTurret();
								_turretAttackD.UpgradeBullet();
								_description.SetActive (false);
							}
						}else{
							if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 2)
							{
								if (GameStats.Instance.RessourcesMat - costT >= costSpé3Niv0) {
									GameStats.Instance.RessourcesMat -= costSpé3Niv0;
									matSpent+= costSpé3Niv0;
									_turret.GetComponent<MeshFilter> ().mesh = newmeshType2;
									transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
									_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 1);
									_turretDistance.LevelUpTurret();
									_turretAttackD.UpgradeBullet();
									_description.SetActive (false);
								}
							}else{
								if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 4)
								{
									GameStats.Instance.RessourcesMat += matSpent/2;
									_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 2);
									//networkView.RPC("ClientWantToSell", RPCMode.AllBuffered);
								}
							}
						}
					}
				}else{
					if (_turretDistance.NivTurret == 2)
					{
						if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 0) {
							if (GameStats.Instance.RessourcesMat - costSpé1Niv1 >= 0) {
								GameStats.Instance.RessourcesMat -= costSpé1Niv1;
								matSpent+= costSpé1Niv1;
								//Debug.Log ("Ressources restantes: " + GameStats.Instance.RessourcesMat);
								_turret.GetComponent<MeshFilter> ().mesh = newmeshType0;
								transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
								_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 2);
								_turretDistance.LevelUpTurret();
								_turretAttackD.UpgradeBullet();
								_description.SetActive (false);
							}
						}else
						{ 
							if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 1)
							{
								if (GameStats.Instance.RessourcesMat - costSpé1Niv1 >= 0) {
									GameStats.Instance.RessourcesMat -= costSpé1Niv1;
									matSpent+= costSpé1Niv1;
									_turret.GetComponent<MeshFilter> ().mesh = newmeshType1;
									transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
									_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 1);
									_turretDistance.LevelUpTurret();
									_turretAttackD.UpgradeBullet();
									_description.SetActive (false);
								}
							}else{
								if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 2)
								{
									if (GameStats.Instance.RessourcesMat - costT >= costSpé3Niv0) {
										GameStats.Instance.RessourcesMat -= costSpé3Niv0;
										matSpent+= costSpé3Niv0;
										_turret.GetComponent<MeshFilter> ().mesh = newmeshType2;
										transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
										_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 1);
										_turretDistance.LevelUpTurret();
										_turretAttackD.UpgradeBullet();
										_description.SetActive (false);
									}
								}else{
									if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 4)
									{
										GameStats.Instance.RessourcesMat += matSpent/2;
										_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 2);
									}
								}
							}
						}
					}else{
						if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 0) {
							if (GameStats.Instance.RessourcesMat - costSpé1Niv2 >= 0) {
								GameStats.Instance.RessourcesMat -= costSpé1Niv2;
								matSpent+= costSpé1Niv2;
								//Debug.Log ("Ressources restantes: " + GameStats.Instance.RessourcesMat);
								_turret.GetComponent<MeshFilter> ().mesh = newmeshType0;
								transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
								_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 1);
								_turretDistance.LevelUpTurret();
								_description.SetActive (false);
							}
						}else
						{ 
							if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 1)
							{
								if (GameStats.Instance.RessourcesMat - costSpé1Niv2 >= 0) {
									GameStats.Instance.RessourcesMat -= costSpé1Niv2;
									matSpent+= costSpé1Niv2;
									_turret.GetComponent<MeshFilter> ().mesh = newmeshType1;
									transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
									_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 1);
									_turretDistance.LevelUpTurret();
									_description.SetActive (false);
								}
							}else{
								if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 2)
								{
									if (GameStats.Instance.RessourcesMat - costT >= costSpé3Niv0) {
										GameStats.Instance.RessourcesMat -= costSpé3Niv0;
										matSpent+= costSpé3Niv0;
										_turret.GetComponent<MeshFilter> ().mesh = newmeshType2;
										transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
										_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 1);
										_turretDistance.LevelUpTurret();
										_turretAttackD.UpgradeBullet();
										_description.SetActive (false);
									}
								}else{
									if (hit.collider.gameObject.GetComponent<TurretMenuD>().TurretMenuSetype == 4)
									{
										GameStats.Instance.RessourcesMat += matSpent/2;
										_turret.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 2);
									}
								}
							}
						}
					}
				}
			}
		}
		
	}

	public void ClientWantToBuy()
	{
		_mustPay = true;
	}

	/*void FixedUpdate(){
		if (_mustPay) {
			_turret.GetComponent<MeshFilter> ().mesh = newmeshType0;
			transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
			_mustPay = false;
			_turret.SetActive(true);
			_turret.GetComponent<TurretMenuSet>().DesactiveMenu();
			_turret.gameObject.SetActive (false);
		}
	}*/

	void FixedUpdate(){
		if (_mustPay) {
			_turret.GetComponent<MeshFilter> ().mesh = newmeshType1;
			_mustPay = false;
			_turret.GetComponent<TurretMenuSet>().DesactiveMenu();
		}
	}
	
	public void ClientWantToSell()
	{
		Sell ();
	}

	public void Sell(){
		_turret.GetComponent<MeshFilter> ().mesh = basemesh;
		_description.SetActive (false);
		_parent.SetActive(true);
		_parent.GetComponent<TurretMenuSet>().DesactiveMenu();
		_turretDistance.NivTurret=0;
		_turret.GetComponent<TurretMenuSet>().DesactiveMenu();
		_turret.gameObject.SetActive (false);
	}
}

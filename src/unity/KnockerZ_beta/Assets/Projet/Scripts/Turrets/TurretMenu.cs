using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurretMenu : MonoBehaviour {

	public int TurretMenuType=0;

	public EmptyPlace _parent;

	int costTD = 50;
	int costTHtoH = 40;

	float limiteDetection = 250.0f ;

	bool _mustPay = false;
	int typeToBuy;

	public GameObject _description;
	public Text _descriptionText;

	void Start () {
	}

	void OnMouseEnter()
	{
		//display = true;
		_description.SetActive (true);
		if (TurretMenuType == 0) {
			_descriptionText.text = "Amélioration archer";
		} else { 
			if(TurretMenuType == 1)
				_descriptionText.text = "Amélioration guerrier";
		}
	}
	void OnMouseExit(){
		_description.SetActive (false);
	}

	void Update () {
		if(Input.GetMouseButtonDown(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, limiteDetection) && hit.collider.gameObject == this.gameObject){
				if(TurretMenuType == 0){
					if (GameStats.Instance.RessourcesMat - costTD >= 0) {
						GameStats.Instance.RessourcesMat -= costTD;
						_description.SetActive (false);
						//networkView.RPC("ClientWantToBuy", RPCMode.AllBuffered, 0);
						_parent.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 1);
					}
				}else{
					if (GameStats.Instance.RessourcesMat - costTHtoH >= 0) {
						GameStats.Instance.RessourcesMat -= costTHtoH ;
						_description.SetActive (false);
						//networkView.RPC("ClientWantToBuy", RPCMode.AllBuffered, 1);
						_parent.networkView.RPC("DoSynchro", RPCMode.AllBuffered, 2);
					}
				}
			}
		}
	}

	/*[RPC]
	void ClientWantToBuy(int type)
	{
		this.typeToBuy = type;
		_mustPay = true;
	}*/
	
	public void ClientWantToBuy(int type){
		this.typeToBuy = type;
		_mustPay = true;
	}

	void FixedUpdate(){
		if (_mustPay) {
			_mustPay = false;
			if(typeToBuy == 1){
				_parent._turretD.SetActive(true);
				_parent._turretD.GetComponent<TurretMenuSet>().DesactiveMenu();
			}else{
				_parent._turretHtoH.SetActive(true);
				_parent._turretHtoH.GetComponent<TurretMenuSet>().DesactiveMenu();
			}
			_parent.gameObject.SetActive (false);
		}
	}

}

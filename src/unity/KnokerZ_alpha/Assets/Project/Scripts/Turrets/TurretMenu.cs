using UnityEngine;
using System.Collections;

public class TurretMenu : MonoBehaviour {

	public int TurretMenuSetype=0;
	int costT = 50;
	public GameObject parent;
	public Mesh newmeshType0;
	public Mesh newmeshType1;
	public Turret parentScript;
	public GameObject parentAttack;

	bool _mustPay = false;

	// Use this for initialization
	void Start () {

	}

	/*void OnMouseDown() {
		if (TurretMenuSetype == 0) {
			if (GameStats.Instance.Ressources - costT > 0) {
				GameStats.Instance.Ressources -= 50;
				Debug.Log ("Ressources restantes: " + GameStats.Instance.Ressources);
				//transform.GetComponentInParent<Transform>().localScale = new Vector3 (2, 2, 2);
				parent.GetComponent<MeshFilter> ().mesh = newmeshType0;
				//Debug.Log(transform.GetComponentInParent<Transform>().name);
				//transform.GetComponentInParent<TurretMenuSet>().DesactiveMenu();
				transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
			}
		}else
		{
			if (GameStats.Instance.Ressources - costT > 0) {
				GameStats.Instance.Ressources -= 100;
				Debug.Log ("Ressources restantes: " + GameStats.Instance.Ressources);
				//transform.GetComponentInParent<Transform>().localScale = new Vector3 (2, 2, 2);
				parent.GetComponent<MeshFilter> ().mesh = newmeshType1;
				//Debug.Log(transform.GetComponentInParent<Transform>().name);
				//transform.GetComponentInParent<TurretMenuSet>().DesactiveMenu();
				transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
			}
		}
	}*/

	void Update () {
		if (Input.GetMouseButtonDown(0) && Network.isClient )
		if (TurretMenuSetype == 0) {
			if (GameStats.Instance.Ressources - costT >= 0) {
				GameStats.Instance.Ressources -= 50;
				//Debug.Log ("Ressources restantes: " + GameStats.Instance.Ressources);
				parent.GetComponent<MeshFilter> ().mesh = newmeshType0;
				transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
				networkView.RPC("ClientWantToBuy", RPCMode.Server, Network.player);
			}
		}else
		{
			if (GameStats.Instance.Ressources - costT >= 0) {
				GameStats.Instance.Ressources -= 100;
				//Debug.Log ("Ressources restantes: " + GameStats.Instance.Ressources);
				parent.GetComponent<MeshFilter> ().mesh = newmeshType1;
				transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
				networkView.RPC("ClientWantToBuy", RPCMode.Server, Network.player);
				parentAttack.SetActive(false);
				parentScript.fighter.SetActive(true);
			}
		}
			
	}

	[RPC]
	void ClientWantToBuy(NetworkPlayer player)
	{
		_mustPay = true;
		if (Network.isServer)
			networkView.RPC("ClientWantToBuy", RPCMode.Others, player);
	}

	void FixedUpdate(){
		if (_mustPay) {
			parent.GetComponent<MeshFilter> ().mesh = newmeshType0;
			transform.GetComponentInParent<TurretMenuSet> ().DesactiveMenu ();
			_mustPay = false;
		}
	}

}

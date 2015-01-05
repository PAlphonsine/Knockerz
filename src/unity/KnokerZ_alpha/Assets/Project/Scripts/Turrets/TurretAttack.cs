using UnityEngine;
using System.Collections;

public class TurretAttack : MonoBehaviour {

	public GameObject tir;
	bool HasFired = true;
	GameObject _tir;

	void Start(){
		//Just for uncross this script
	}

	void OnTriggerStay(Collider collider)
	{
		//Debug.Log ("ok");
		if (HasFired) 
		{
			HasFired = false;
			if (collider.gameObject.tag.Equals ("Zombie")) {
				StartCoroutine (gh (collider));
			}
		}
		if (collider.gameObject.tag.Equals ("Zombie")) {
			if (_tir == null) {
				//Debug.Log("fail");
				HasFired = true;
			}
		}

	}

	IEnumerator gh(Collider collider){
		//_tir = new GameObject();
		_tir = Network.Instantiate(tir, transform.position, Quaternion.identity, 0) as GameObject;
		_tir.GetComponent<Firing>().v_position[1] = collider.gameObject.transform;
	
		yield return new WaitForSeconds(1f);
	}
}

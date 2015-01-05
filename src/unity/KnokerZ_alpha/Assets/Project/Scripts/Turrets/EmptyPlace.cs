using UnityEngine;
using System.Collections;

public class EmptyPlace : MonoBehaviour {
	
	public GameObject prefab;

	public Camera _camera;

	bool _mustConstruct = false;

	Vector3 posTourelle;
	NetworkViewID tmpId;
	
	void Update(){
		DetecterObjet ();
	}
	
	string tagObjet = "place" ; // Vérifiez bien que le gameObject possède le tag
	float limiteDetection = 250.0f ; // Définir la limite de distance au delà de laquelle le clic n’est plus prit en compte
	
	void DetecterObjet(){
		Ray ray = _camera.ScreenPointToRay(Input.mousePosition); // Droite (rayon) qui passe par le centre de la caméra et la positon de la souris
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, limiteDetection)){ 
			//Le rayon est lancé. Sa taille sera égale à  limiteDetection. Les objets en contact avec le rayon "ray" sont stockés dans la variable hit.
			if(hit.transform.CompareTag( tagObjet ) && Input.GetMouseButtonUp(0)){
				if((Network.player == _STATICS._networkPlayer[0] && hit.transform.position.x < 0) 
				   || (Network.player == _STATICS._networkPlayer[1] && hit.transform.position.x > 0)){
					Debug.Log("ok");
					posTourelle = hit.transform.position;
					//Si le tag correspond, faites ce que vous voulez
					GameObject g = Instantiate(prefab, hit.transform.position, Quaternion.identity) as GameObject;
					g.GetComponent<TurretMenuSet>().DesactiveMenu();
					tmpId = networkView.viewID;
					networkView.RPC("ClientWantToBuild", RPCMode.Server,posTourelle, tmpId);
					hit.transform.gameObject.SetActive(false);
				}
			}
		}
	}

	//http://docs.unity3d.com/Manual/net-RPCDetails.html

	[RPC]
	void ClientWantToBuild(Vector3 tmphit, NetworkViewID tmpId)
	{
		_mustConstruct = true;
		posTourelle = tmphit;
		this.tmpId = tmpId;
		if (Network.isServer)
			networkView.RPC("ClientWantToBuild", RPCMode.Others, tmphit, tmpId);
	}

	//http://docs.unity3d.com/ScriptReference/NetworkView.Find.html

	void FixedUpdate(){
		if (_mustConstruct) {
			GameObject g = Instantiate(prefab, posTourelle, Quaternion.identity) as GameObject;
			g.GetComponent<TurretMenuSet>().DesactiveMenu();
			_mustConstruct = false;
			Debug.Log(tmpId);
			/*NetworkView view = NetworkView.Find(tmpId);
			Debug.Log(view.observed.name);
			Debug.Log(view.observed.gameObject.name);
			view.gameObject.SetActive(false);*/
		}
	}
}

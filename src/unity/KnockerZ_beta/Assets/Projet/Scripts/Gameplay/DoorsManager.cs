using UnityEngine;
using System.Collections;

public class DoorsManager : MonoBehaviour {

	public GameObject door;
	public Transform _startPosition;
	public Transform _destination;
	
	public GameObject otherDoor;
	public Transform _otherstartPosition;
	public Transform _otherdestination;
	
	float limiteDetection = 250.0f ;
	public Camera _camera;
	
	public PhasesManager _phasesManager;
	
	private bool Opening =false;
	private bool cantClose =false;
	private bool isClosed= false;
	private bool isOpened= false;
	
	private int pv=10000;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		DetecterObjet ();
		if (Opening){
			door.transform.position = Vector3.Lerp(door.transform.position, _destination.position, 0.02f);
			otherDoor.transform.position = Vector3.Lerp(otherDoor.transform.position, _otherdestination.position, 0.02f);
		}else if(!cantClose){
			door.transform.position = Vector3.Lerp(door.transform.position, _startPosition.position, 0.02f);
			otherDoor.transform.position = Vector3.Lerp(otherDoor.transform.position, _otherstartPosition.position, 0.02f);
		}
		
		if (Vector3.Distance (door.transform.position, _destination.position) < 0.3f)
			isOpened = true;
		else
			isOpened = false;
		if (Vector3.Distance(door.transform.position, _startPosition.position) < 0.3f)
			isClosed = true;
		else
			isClosed = false;

		if (pv <= 0) {
			door.SetActive (false);
			otherDoor.SetActive(false);
		}
	}
	
	void DetecterObjet(){
		if(Input.GetMouseButtonUp(0) && _phasesManager.startAction){
			Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, limiteDetection)){
				if(hit.collider.gameObject == door.gameObject || hit.collider.gameObject == otherDoor.gameObject){
					if((Network.player == _STATICS._networkPlayer[0] && hit.transform.position.x < 0) 
					   || (Network.player == _STATICS._networkPlayer[1] && hit.transform.position.x > 0)
					   || Network.isServer){
						if (Opening && isOpened)
							Opening = false;
						else if(cantClose || (!Opening && isClosed))
							Opening = true;
					}
				}
			}
		}
	}
	
	public int Pv
	{
		get { return this.pv; }
		set { this.pv = value;}
	}

	public bool CantClose
	{
		get { return this.cantClose; }
		set { this.cantClose = value;}
	}
}

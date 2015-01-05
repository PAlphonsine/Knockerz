using UnityEngine;
using System.Collections;

public class Firing : MonoBehaviour {
	
	public Transform[] v_position;
	
	private int _arraySize;
	private int Number;

	public float  v_speedFactor;
	
	//public Transform v_cam1, v_cam2, v_end;
	public AnimationCurve v_curve;
	private float _progression;
	private float _progressionR;
	
	void Start () {
		_arraySize = v_position.Length;
		_progression = 0f;
		_progressionR = 0f;
		Number = 0;

		v_position[0] = transform;
	}
	
	void FixedUpdate () {

		if (v_position [1] == null) {
			Destroy(gameObject);
		}

		if(v_position [1]!=null && v_position [0]!=null){ 
			transform.position = Vector3.Lerp(v_position[Number].transform.position, v_position[(Number+1)%_arraySize].transform.position, v_curve.Evaluate(_progression));
			transform.rotation = Quaternion.Lerp(v_position[Number].transform.rotation, v_position[(Number+1)%_arraySize].transform.rotation, v_curve.Evaluate(_progression));
			_progression += Time.deltaTime * 0.25f;

			transform.rotation = Quaternion.Lerp(
				v_position[Number].transform.rotation, 
				v_position[(Number+1) % _arraySize].transform.rotation, 
				v_curve.Evaluate(_progressionR)
				);

			_progressionR += Time.deltaTime * v_speedFactor;
			
			if ((v_position[(Number+1)%_arraySize].transform.position - transform.position).magnitude < 0.1f){_progression = 0f; Number = (Number+1) % _arraySize;_progressionR = 0f;}
		}

	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag.Equals("Zombie")) 
		{
			if(collider.gameObject.GetComponent<ZombieCommand>().pv > 0){
				collider.gameObject.GetComponent<ZombieCommand>().pv -= 5;
				Debug.Log(collider.gameObject.GetComponent<ZombieCommand>().pv);
			}
			if(collider.gameObject.GetComponent<ZombieCommand>().pv <= 0){
				Destroy(collider.gameObject);
			}
			Destroy(gameObject);
		}
	}
}
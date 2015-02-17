using UnityEngine;
using System.Collections;

public class BulletFiring : MonoBehaviour {
	
	public Transform[] v_position;
	
	private int _arraySize;
	private int Number;

	public float v_speed=0.25f;
	public float  v_speedFactor;
	
	//public Transform v_cam1, v_cam2, v_end;
	public AnimationCurve v_curve;
	private float _progression;
	private float _progressionR;

	public float _lifeTime=0;
	
	void Start () {
		_arraySize = v_position.Length;
		_progression = 0f;
		_progressionR = 0f;
		Number = 0;

		v_position[0] = transform;
	}

	void Update(){
		_lifeTime += 0.06f;
		if(_lifeTime>4f)
			Reset();
	}
	
	void FixedUpdate () {

		if (v_position [1] == null) {
			Reset();
		}

		if(v_position [1]!=null && v_position [0]!=null){ 
			transform.position = Vector3.Lerp(v_position[Number].transform.position, v_position[(Number+1)%_arraySize].transform.position, v_curve.Evaluate(_progression));
			transform.rotation = Quaternion.Lerp(v_position[Number].transform.rotation, v_position[(Number+1)%_arraySize].transform.rotation, v_curve.Evaluate(_progression));
			_progression += Time.deltaTime * v_speed;

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
			if(collider.gameObject.GetComponent<ZombieScript>().Pv > 0){
				collider.gameObject.GetComponent<ZombieScript>().Pv -= 25;
				//Debug.Log(collider.gameObject.GetComponent<ZombieScript>().pv);
			}
			if(collider.gameObject.GetComponent<ZombieScript>().Pv <= 0){
				collider.transform.position = new Vector3(0,0,-40f);
				collider.gameObject.SetActive(false);
				collider.GetComponent<ZombieScript>().Reset();
			}
			Reset();
		}

	}

	public void Reset(){
		_progression = 0f;
		_progressionR = 0f;
		Number = 0;
		_lifeTime = 0;
		transform.localPosition = new Vector3(0,0,0f);
		gameObject.SetActive (false);
	}
}
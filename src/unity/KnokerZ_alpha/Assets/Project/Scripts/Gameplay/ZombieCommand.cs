using UnityEngine;
using System.Collections;

public class ZombieCommand : MonoBehaviour {
	
	public Transform[] v_position;
	
	private int _arraySize;
	private int Number;

	public int pv = 10;

	public float  v_speedFactor;

	int i = 0;

	Transform target;
	bool isFighting;
	//Vector3 startPosition;
	
	//public Transform v_cam1, v_cam2, v_end;
	public AnimationCurve v_curve;
	private float _progression;
	private float _progressionR;
	
	Vector3 zombiePosition;

	public float timeAttack = 1f;

	void Start () {
		_arraySize = v_position.Length;
		_progression = 0f;
		_progressionR = 0f;
		Number = 0;
		isFighting = false;
		zombiePosition = transform.position;
	}

	void OnSerializeNetworkView(BitStream stream){
		//Debug.Log ("ok");
		//stream.Serialize(ref i);
		stream.Serialize(ref  _progressionR);
		stream.Serialize(ref  Number);
		stream.Serialize(ref  pv);
		stream.Serialize(ref  _progression);
		stream.Serialize(ref  isFighting);
	}
	
	void FixedUpdate () 
	{
		transform.position = zombiePosition;
		//if (Network.isServer)
		//{
			if (isFighting)
			{
				if (Vector3.Distance (transform.position, target.position) > 20)
				{//error
					isFighting = false;
					v_position [Number].transform.position = transform.position;
					_progression = 0f;
					_progressionR = 0f;
				}

				if (timeAttack > 0.1) {
					timeAttack -= Time.deltaTime;
				}else{
					pv -= 5;
					timeAttack = 1f;
				}
				if(pv <= 0){
					transform.position = new Vector3(100f, 100f, 100f);
					gameObject.SetActive(false);
				}
			}
			else
			{
				transform.position = Vector3.Lerp (v_position [Number].transform.position, v_position [(Number + 1) % _arraySize].transform.position, v_curve.Evaluate (_progression));
				/*error*/transform.rotation = Quaternion.Lerp (v_position [Number].transform.rotation, v_position [(Number + 1) % _arraySize].transform.rotation, v_curve.Evaluate (_progression));
				_progression += Time.deltaTime * 0.25f;
				//Vitesse constante

				transform.rotation = Quaternion.Lerp(
					v_position [Number].transform.rotation,
					v_position [(Number + 1) % _arraySize].transform.rotation,
					v_curve.Evaluate (_progressionR)
					);

				_progressionR += Time.deltaTime * v_speedFactor;

				if ((v_position [(Number + 1) % _arraySize].transform.position - transform.position).magnitude < 0.1f)
				{
					_progression = 0f;
					Number = (Number + 1) % _arraySize;
					_progressionR = 0f;
				}
				i++;
			}
		//}
		if (Network.isClient) {
			//Debug.Log(i);
		}

		zombiePosition = transform.position;
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "Fighter")
		{
			isFighting = true;
			target = collider.transform;
			//startPosition = transform.position;
			timeAttack = 1f;
		}
	}
	
	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Fighter")
		{
			//Debug.Log ("OK Z");
			isFighting = false;
		}
	}
}
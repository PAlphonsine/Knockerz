using UnityEngine;
using System.Collections;

public class FighterScript : MonoBehaviour {

	public Transform[] v_position;
	private int Number;
	Transform target;
	Vector3 startPosition;
	int targetCount = 0;
	public AnimationCurve v_curve;
	bool isFighting;
	float progression;
	Vector3 fighterPosition;

	public int pv = 7;
	public float timeAttack = 1f;
	
	// Use this for initialization
	void Start () {
		startPosition = transform.position;
		progression = 0f;
		Number = 0;
		target = v_position [Number].transform;
		target.position = v_position[Number].transform.position;
		isFighting = false;
		fighterPosition = transform.position;
	}
	
	void OnSerializeNetworkView(BitStream stream){
		//Debug.Log ("ok");
		//stream.Serialize(ref i);
		stream.Serialize(ref  Number);
		stream.Serialize(ref  progression);
		stream.Serialize(ref  isFighting);
		stream.Serialize(ref  fighterPosition);
		stream.Serialize(ref  pv);
		stream.Serialize(ref  timeAttack);
	}

	void FixedUpdate()
	{
		transform.position = fighterPosition;

		if (isFighting)
		{
			if (Vector3.Distance (transform.position, target.position) > 20)
			{
				isFighting = false;
				progression = 0f;
				target = v_position [Number].transform;
				target.position = v_position[Number].transform.position;
			}
			else if (Vector3.Distance (transform.position, target.position) > 1.5)
			{
				//Debug.Log ("ELSE IF");
				transform.position = Vector3.Lerp (startPosition, target.position, progression);
				transform.rotation = Quaternion.Lerp (transform.rotation, target.rotation, progression);
				progression += Time.deltaTime * 0.5f;
			}

			if (timeAttack > 0.1) {
				timeAttack -= Time.deltaTime;
			}else{
				pv --;
				timeAttack = 1f;
			}

			if(pv <= 0){
				transform.position = new Vector3(100f, 100f, 100f);
				gameObject.SetActive(false);
			}
		}
		else
		{
			if (Vector3.Distance(transform.position, target.position) > 0.1)
			{
				transform.position = Vector3.Lerp (transform.position, target.position, v_curve.Evaluate(progression));
				transform.rotation = Quaternion.Lerp (transform.rotation, target.rotation, v_curve.Evaluate(progression));
				progression += Time.deltaTime * 0.1f;
			}
		}

		fighterPosition = transform.position;
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "Zombie")
		{
			targetCount++;
			/*if(targetCount <= 2)
			{
				isFighting = true;
				target = collider.transform;
			}*/
			isFighting = true;
			target = collider.transform;
			startPosition = transform.position;
			timeAttack = 1f;
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Zombie")
		{
			//Debug.Log ("OK F");
			isFighting = false;
		}
	}
}

/*
 * Si Humain tué : l'emmener très loin, le désactiver, le rammener à sa position d'avant sa mort, le réactiver
 * => dans plusieurs frames différentes
 */

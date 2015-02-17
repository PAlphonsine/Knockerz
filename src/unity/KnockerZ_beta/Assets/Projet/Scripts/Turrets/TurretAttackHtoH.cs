using UnityEngine;
using System.Collections;

public class TurretAttackHtoH : MonoBehaviour {

	//public GameObject[] bullets;
	public GameObject bullet;
	bool HasFired = true;
	//int currentBullet=0;

	void Start(){
		//Just for uncross this script
	}

	void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.tag.Equals ("Zombie"))
		{
			if (HasFired)
			{
				StartCoroutine (Fire(collider));
				HasFired = false;
			}else{
				if (bullet.activeSelf == false)
					HasFired = true;
			}
		}

	}

	IEnumerator Fire(Collider collider){
		//_tir = new GameObject();
		/*if (bullets [currentBullet].activeSelf == false){
			bullets [currentBullet].GetComponent<Firing>().v_position[1] = collider.gameObject.transform;
			bullets [currentBullet].SetActive(true);
			currentBullet= (currentBullet+1) % bullets.Length;
		}*/
		bullet.GetComponent<BulletFiring>().v_position[1] = collider.gameObject.transform;
		bullet.SetActive(true);

		yield return new WaitForSeconds(1f);
	}
}

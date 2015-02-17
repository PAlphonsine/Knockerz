using UnityEngine;
using System.Collections;

public class TurretAttackD : MonoBehaviour {

	//public GameObject[] bullets;
	public GameObject bullet;
	public BulletFiring _bulletFiring;
	bool HasFired = true;
	//int currentBullet=0;

	void Start(){
		//Just for uncross this script
	}

	public void UpgradeBullet(){
		_bulletFiring.v_speed += 0.25f;
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

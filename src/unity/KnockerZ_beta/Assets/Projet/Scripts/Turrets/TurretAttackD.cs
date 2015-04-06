using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurretAttackD : MonoBehaviour
{
	// Projectile tiré par la tourelle
	public GameObject bullet;
	// Gestion du prijectile
	public BulletFiring _bulletFiring;
	// Booléen de controle de tir de la tourelle
	bool HasFired = true;
	// Booléen de controle de possibilité de tir de la tourelle
	private bool canFire = true;
	// Zone de détection de la tourelle
	[SerializeField]
	BoxCollider zone;
	[SerializeField]
	GameObject graphicZone;
	//public GameObject[] bullets;
	//int currentBullet=0;
	// Dictionnaire de zombies qui sont entrés dans le champs de la tourelle
	Dictionary<NetworkViewID, Transform> dicoZombies = new Dictionary<NetworkViewID, Transform>();
	
	// Méthode d'amélioration du projectile
	public void UpgradeBullet(int dam, float speed)
	{
		// TODO : augmenter le nombre de variables à améliorer
		// On augmente la vitesse du projectile
		_bulletFiring.V_speed = speed;
		// On augmente les dégats des projectiles
		_bulletFiring.Damage = dam;

	}

	public void UpgradeRange(float range){
		zone.size = new Vector3 (range, 0.25f, range);
		graphicZone.transform.localScale = new Vector3 (range, 0.1f, range);
	}

	// Lorsqu'un object reste dans le collider de la tourelle à distance
	void OnTriggerEnter(Collider collider)
	{
		// Si la tourelle peut tirer
		if (this.canFire == true)
		{
			// Si l'objet rencontré est un Zombie
			if (collider.gameObject.tag.Equals ("Zombie"))
			{
				// Si le zombie n'existe pas dans le dictionnaire
				if (!dicoZombies.ContainsKey(collider.networkView.viewID))
					// On ajoute sa position avec comme clé son viewID
					dicoZombies.Add(collider.networkView.viewID, collider.transform);
			}
		}
	}

	void Update(){
		// Si la tourelle a déjà tiré
		if (HasFired)
		{
			// On cherche un transform dans notre dictionnaire
			foreach(Transform t in dicoZombies.Values)
			{
				// On l'autorise a tiré à la suite
				StartCoroutine (Fire(t));
				break;
			}
			// La tirelle n'a pas tiré avant son prochain projectile
			HasFired = false;
		}
		// Sinon, si la tourelle n'a pas déjà tiré
		else
		{
			// Si le projectile est désactivé, c'est-à-dire qu'il a atteint sa cible
			if (bullet.activeSelf == false)
				// La tourelle a tiré
				HasFired = true;
		}
	}

	void OnTriggerExit(Collider collider)
	{
		// Si la tourelle peut tirer
		if (this.canFire == true) 
		{
			// Si l'objet sorti est un Zombie
			if (collider.gameObject.tag.Equals ("Zombie")) 
			{
				// Si il est dans le dictionnaire
				if(dicoZombies.ContainsKey(collider.networkView.viewID))
					// On le retire
					dicoZombies.Remove(collider.networkView.viewID);
			}
		}
	}
	
	// Fonction Coroutine
	IEnumerator Fire(Transform posZombie)
	{
		//_tir = new GameObject();
		/*if (bullets [currentBullet].activeSelf == false){
			bullets [currentBullet].GetComponent<Firing>().v_position[1] = collider.gameObject.transform;
			bullets [currentBullet].SetActive(true);
			currentBullet= (currentBullet+1) % bullets.Length;
		}*/
		// La destination du projectile tiré par la tourelle est la position de l'objet rencontré
		_bulletFiring.v_position[1] = posZombie;
		// Le projectile est activé
		bullet.SetActive(true);
		
		// On attend une seconde
		yield return new WaitForSeconds(0.1f);
	}
	
	// Accesseurs
	public bool CanFire
	{
		get { return this.canFire; }
		set { this.canFire = value; }
	}
}
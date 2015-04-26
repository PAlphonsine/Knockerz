using UnityEngine;
using System.Collections;

public class BossScript : MonoBehaviour 
{
	// Portée du Raycast
	float limiteDetection = 250.0f ;
	// Caméra
	public Camera _camera;
	// Séparation entre les deux joueurs
	[SerializeField] Transform separator;
	// Agent du boss
	[SerializeField] NavMeshAgent agent;
	// Vitesse du boss
	float speed = 0.5f;
	// Stock de zombies à lancer
	[SerializeField] ZombieScript[] zombies;
	// Pour calculer le temps avant de lancer un autre zombie
	float time;
	// Temps avant un autre zombie
	float cooldowndZombiesSpawn = 5f;
	// Etat du boss
	bool stateMove;
	[SerializeField] GameObject objectPv;
	[SerializeField] GameObject objectDps;
	[SerializeField] GameObject objectSpeed;
	[SerializeField] GameObject objectCooldown;

	//Ignore raycast

	void Start () 
	{
		time = 0;
		stateMove = true;
	}

	void Update ()
	{
		// On vérifie si le joueur à cliqué sur le boss
		DetectectObjet ();
		// Si le boss est en mode déplacement
		if (stateMove)
			// On applique la vitesse de base
			agent.speed = speed;
		// S'il est en mode spawn
		else
		{
			// On applique une vitesse réduite
			agent.speed = speed / 2;
			// On incrémente le temps
			time += Time.deltaTime;
			// Dès que le temps de spawn est arrivé à son terme
			if(time > cooldowndZombiesSpawn)
			{
				// On cherche un zombie désactivé dans la liste
				foreach(ZombieScript zbs in zombies)
				{
					if (!zbs.gameObject.activeSelf)
					{
						// On le place sur le boss
						zbs.transform.position = transform.position;
						// On l'active
						zbs.gameObject.SetActive(true);
						break;
					}
				}
				// Le temps est remit à zero
				time = 0;
			}
		}
	}

	// Méthode de détectione d'objet
	void DetectectObjet()
	{
		// Lorsque le joueur clique et que l'on est en phase d'action
		if (Input.GetMouseButtonUp(0))
		{
			// On trace un rayon partant de la camera à la position du curseur
			Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
			// On instancie un point d'impact
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, limiteDetection))
			{
				// Si le rayon touche la première ou la seconde porte
				if (hit.collider.gameObject == this.gameObject)
				{
					// Si le joueur est le joueur 1 et qu'il a cliqué de son coté
					// ou si le joueur est le joueur 2 et qu'il a cliqué de son coté
					// ou si c'est le serveur
					if ((Network.player == _STATICS._networkPlayer[1] && hit.transform.position.x < separator.position.x) 
					    || (Network.player == _STATICS._networkPlayer[0] && hit.transform.position.x > separator.position.x)
					    || Network.isServer)
					{
						networkView.RPC("ChangeMode", RPCMode.AllBuffered);
					}
				}
			}
		}
	}

	[RPC]
	void ChangeMode()
	{
		// On inverse l'état du boss
		stateMove = !stateMove;
	}

	// Accesseurs

	public ZombieScript[] Zombies
	{
		get { return zombies; }
		set { zombies = value; }
	}

	public float Speed
	{
		get { return speed; }
		set { speed = value; }
	}

	public float CooldowndZombiesSpawn
	{
		get { return cooldowndZombiesSpawn; }
		set { cooldowndZombiesSpawn = value; }
	}

	public GameObject ObjectPv
	{
		get { return objectPv; }
		set { objectPv = value; }
	}

	public GameObject ObjectDps
	{
		get { return objectDps; }
		set { objectDps = value; }
	}

	public GameObject ObjectSpeed
	{
		get { return objectSpeed; }
		set { objectSpeed = value; }
	}

	public GameObject ObjectCooldown
	{
		get { return objectCooldown; }
		set { objectCooldown = value; }
	}
}

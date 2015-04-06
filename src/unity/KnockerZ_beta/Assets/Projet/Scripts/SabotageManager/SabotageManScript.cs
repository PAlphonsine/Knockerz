using UnityEngine;
using System.Collections;

public class SabotageManScript : MonoBehaviour
{
	// Gestion des phases
	[SerializeField]
	PhasesManager phasesManager;
	// Emplacement de repos du saboteur (dans sa base)
	[SerializeField]
	GameObject restPosition;
	// Emplacement de départ du saboteur quand il part en mission
	[SerializeField]
	GameObject startPosition;
	// Couleur du saboteur
	Color sabotageManColor;
	// Destination du saboteur
	[SerializeField]
	GameObject destination;
	// Tourelle sabotée (universelle, distance, corps-à-corps)
	private GameObject targetedTurret;
	private TurretDistance targetedDistanceTurret;
	private TurretHtoH targetedHToHTurret;
	// Booléen de controle d'attaque de la tourelle à distance
	private bool canFire;
	// Booléen de controle d'assignement d'une tourelle cible au saboteur
	private bool targetAssigned;
	// Système de particules du sabotage
	[SerializeField]
	ParticleSystem sparks;
	// Booléen d'état : le saboteur est mort ou pas
	private bool dead;
	// Booléen d'état : le saboteur a réussi sa mission ou pas
	private bool success;
	// Booléen de controle d'envoi du saboteur
	private bool canGoSabotage;
	// Booléen de controle d'arrivée du saboteur à sa destination
	private bool hasArrived;
	// Booléen de controle de disparition du saboteur
	private bool hasDisappeared;
	// Booléen de controle d'apparition du saboteur
	private bool hasAppeared;
	// Booléen de controle de survie et de retour du saboteur
	private bool hasSurvived;
	// Booléen de controle de réussite confirmée de la mission
	private bool hasSucceed;
	// Booléen de controle de dissipation des effets du sabotage
	private bool effectDissipated;
	
	// Use this for initialization
	void Start ()
	{
		this.sabotageManColor = this.GetComponent<Renderer> ().material.color;
		this.sparks.enableEmission = false;
		this.targetAssigned = false;
		this.hasArrived = false;
		this.hasAppeared = true;
		this.hasDisappeared = false;
		this.hasSurvived = false;
		this.hasSucceed = false;
		this.effectDissipated = false;
	}
	
	void FixedUpdate ()
	{
		// Si l'on est en phase d'action
		if (this.phasesManager.startAction == true)
		{
			// Si le saboteur peut aller saboter
			if (this.canGoSabotage == true)
			{
				//Debug.Log("SMS/Réussite de la mission : " + this.success);
				//Debug.Log("SMS/Mort du Saboteur : " + this.dead);
				// On l'active
				this.gameObject.SetActive(true);

				// Si la cible du saboteur n'a pas encore été désignée
				if (this.targetAssigned == false)
				{
					// On la lui désigne
					this.TargetAssign();
				}

				// Si le saboteur n'est pas encore arrivé à sa position de départ et non plus à la position de la tourelle
				if (this.transform.position != this.startPosition.transform.position && this.hasArrived == false)
				{
					// Il se déplace vers sa position de départ
					this.transform.position = Vector3.Lerp(this.restPosition.transform.position, this.startPosition.transform.position, 1f);
					//Debug.Log("SMS/Départ du Saboteur de la base");
				}
				// Sinon, quand le saboteur est arrivé à sa position de départ
				else
				{
					// Si le saboteur est encore visible et qu'il n'est pas à la tourelle ...
					if (this.sabotageManColor.a > 0f && this.hasArrived == false)
					{
						// ... il disparait
						this.Disappear();
					}
					else
					{
						// Sinon, il a disparu est n'est plus visible
						this.hasDisappeared = true;
						this.hasAppeared = false;
						//Debug.Log("SMS/Disparition du Saboteur");
					}
				}

				// Si le saboteur a disparu et s'il n'est pas arrivé à la tourelle
				if (this.hasDisappeared == true && hasArrived == false)
				{
					// Il est téléporté (tout en étant invisible) à la tourelle
					this.transform.position = new Vector3(this.destination.transform.position.x+2, this.destination.transform.position.y, this.destination.transform.position.z);
					this.sabotageManColor.a = 0f;
					// Il n'est plus considéré comme invisible et va apparaitre
					this.hasDisappeared = false;
					// Il est arrivé à la tourelle
					this.hasArrived = true;
					//Debug.Log("SMS/Arrivée du Saboteur à la tourelle");
				}

				// Si le saboteur est arrivé à la tourelle
				if (this.hasArrived == true)
				{
					// S'il n'est pas totalement visible ...
					if (this.sabotageManColor.a < 1f)
					{
						// ... il apparait
						this.Appear();
					}
					else
					{
						// Sinon, il est apparu et n'est plus invisible
						this.hasAppeared = true;
						this.hasDisappeared = false;
						//Debug.Log("SMS/Apparition du Saboteur");
					}
				}
			}

			// Si le saboteur est arrivé et qu'il est apparu
			if (this.hasArrived == true && this.hasAppeared == true)
			{
				// Il commence son sabotage
				this.sparks.enableEmission = true;
				//Debug.Log("SMS/Début du sabotage");

				// Si son sabotage réussi
				if (this.success == true && this.hasSucceed == false)
				{
					// Si c'est une tourelle corps-à-corps
					if (this.targetedTurret.GetComponent<TurretDistance>() == null)
					{
						// On désactive le ou les Fighters de la tourelle
						this.targetedHToHTurret.fighter.SetActive(false);
					}
					// Sinon, si c'est une tourelle à distance
					else if (this.targetedTurret.GetComponent<TurretHtoH>() == null)
					{
						// Si la tourelle tire ...
						if (this.canFire == true)
						{
							// ... elle ne tire plus
							this.targetedDistanceTurret.GetComponentInChildren<TurretAttackD>().CanFire = false;
							this.canFire = false;
						}
					}
					this.hasSucceed = true;
					//Debug.Log("SMS/Sabotage en cours");
				}
				// Sinon, si le sabotage échoue
				else if (this.hasSucceed == false)
				{
					// On désactive les particules du sabotage
					this.sparks.enableEmission = false;
					//Debug.Log("SMS/Sabotage raté");
				}
			}

			// Si le saboteur est activé
			if (this.gameObject.activeSelf == true)
			{
				// Il a survécu
				this.hasSurvived = true;
			}

			this.effectDissipated = false;
		}
		// Sinon, si l'on est en phase de réflexion
		else
		{
			if (this.effectDissipated == false)
			{
				// Les effets du sabotage ne sont plus actifs : ...
				if (this.targetedTurret.GetComponent<TurretDistance>() == null)
				{
					// ... le ou les fighters sont réactivés ...
					this.targetedHToHTurret.fighter.SetActive(true);
				}
				else if (this.targetedTurret.GetComponent<TurretHtoH>() == null)
				{
					// ... ou la tourelle peut tirer
					this.targetedDistanceTurret.GetComponentInChildren<TurretAttackD>().CanFire = true;
					this.canFire = true;
				}

				this.effectDissipated = true;
			}

			// Si le saboteur est mort
			if (this.dead == true)
			{
				// S'il est parfaitement visible
				if(this.sabotageManColor.a > 1f)
				{
					// Il devient rouge
					this.sabotageManColor = new Color(255f, 0f, 0f);
					this.renderer.material.color = this.sabotageManColor;
				}
				// S'il est visible ...
				if (this.sabotageManColor.a > 0f)
				{
					// ... il disparait
					this.Disappear();
				}
				else
				{
					// Sinon, il est réinitialisé
					this.Reset();
				}
			}
			// Sinon, si le saboteur n'est pas mort
			else
			{
				// S'il a survécu
				if (this.hasSurvived == true)
				{
					// Le joueur reçoit un de population
					GameStats.Instance.Population++;
					this.hasSurvived = false;
				}

				// Le sabotage est terminé
				this.sparks.enableEmission = false;

				// Si le saboteur est toujours à la tourelle et n'a pas disparu
				if (this.hasArrived == true && this.hasDisappeared == false)
				{
					// Si le saboteur est toujours visible ...
					if (this.sabotageManColor.a > 0f)
					{
						// ... il disparait
						this.Disappear();
					}
					else
					{
						// Sinon, il a disparu et n'est plus visible
						this.hasDisappeared = true;
						this.hasAppeared = false;
						//Debug.Log("SMS/Disparition du Saboteur");
						// Il se replace à sa position de départ
						this.transform.position = this.startPosition.transform.position;
						//Debug.Log("SMS/Retour du Saboteur à la base");
						// Il n'est plus à la tourelle
						this.hasArrived = false;
					}
				}
				// Sinon, s'il est revenu à sa position de départ et qu'il n'est pas apparu
				else if (this.hasArrived == false && this.hasAppeared == false)
				{
					// Si le saboteur n'est pas totalement visible
					if (this.sabotageManColor.a < 1f)
					{
						// Il apparait
						this.Appear();
					}
					else
					{
						// Sinon, il est apparu est n'est plus invisible
						this.hasDisappeared = false;
						this.hasAppeared = true;
						//Debug.Log("SMS/Apparition du Saboteur");
						// Il se déplace vers la base
						this.transform.position = Vector3.Lerp(this.startPosition.transform.position, this.restPosition.transform.position, 1f);
						// Si le saboteur est arrivé dans la base
						if (this.transform.position == this.restPosition.transform.position)
						{
							// Il est réinitialisé
							this.Reset();
						}
					}
				}
			}
		}
	}
	
	// Méthode d'assignement d'une cible au saboteur
	public void TargetAssign()
	{
		// Si la cible est une tourelle corps-à-corps
		if (this.targetedTurret.GetComponent<TurretDistance>() == null)
		{
			// On assigne la cible au saboteur
			this.targetedHToHTurret = this.targetedTurret.GetComponent<TurretHtoH>();
		}
		// Sinon, si la cible est une tourelle à distance
		else if (this.targetedTurret.GetComponent<TurretHtoH>() == null)
		{
			// On assigne la cile au saboteur ...
			this.targetedDistanceTurret = this.targetedTurret.GetComponent<TurretDistance>();
			// ... et il est informé de l'état de la tourelle
			this.canFire = this.targetedDistanceTurret.GetComponentInChildren<TurretAttackD>().CanFire;
		}

		// La cible a été assignée
		this.targetAssigned = true;
	}
	
	// Méthode d'apparition du saboteur
	public void Appear()
	{
		// On augmente l'alpha de la couleur du saboteur
		this.sabotageManColor.a += 0.01f;
		// On assigne le material au saboteur
		this.renderer.material.color = this.sabotageManColor;
	}
	
	// Méthode de disparition du saboteur
	public void Disappear()
	{
		// On diminue l'alpha de la couleur du saboteur
		this.sabotageManColor.a -= 0.01f;
		// On assigne le material au saboteur
		this.renderer.material.color = this.sabotageManColor;
	}
	
	// Méthode de réinitialisation des paramètres du saboteurF
	public void Reset()
	{
		this.sparks.enableEmission = false;
		this.targetAssigned = false;
		this.hasArrived = false;
		this.hasAppeared = true;
		this.hasDisappeared = false;
		this.hasSurvived = false;
		this.hasSucceed = false;
		this.transform.position = this.restPosition.transform.position;
		this.sabotageManColor = new Color (0f, 0f, 255f, 255f);
		this.renderer.material.color = this.sabotageManColor;
		this.gameObject.SetActive (false);
	}
	
	// Accesseurs
	public GameObject Destination
	{
		get { return this.destination; }
		set { this.destination = value; }
	}
	
	public GameObject TargetedTurret
	{
		get { return this.targetedTurret; }
		set { this.targetedTurret = value; }
	}
	
	public bool CanFire
	{
		get { return this.canFire; }
		set { this.canFire = value; }
	}
	
	public bool TargetAssigned
	{
		get { return this.targetAssigned; }
		set { this.targetAssigned = value; }
	}
	
	public bool Dead
	{
		get { return this.dead; }
		set { this.dead = value; }
	}
	
	public bool Success
	{
		get { return this.success; }
		set { this.success = value; }
	}
	
	public bool CanGoSabotage
	{
		get { return this.canGoSabotage; }
		set { this.canGoSabotage = value; }
	}
	
	public bool HasArrived
	{
		get { return this.hasArrived; }
		set { this.hasArrived = value; }
	}
	
	public bool HasDisappeared
	{
		get { return this.hasDisappeared; }
		set	{ this.hasDisappeared = value; }
	}
	
	public bool HasAppeared
	{
		get { return this.hasAppeared; }
		set	{ this.hasAppeared = value; }
	}
	
	public bool HasSurvived
	{
		get { return this.hasSurvived; }
		set { this.hasSurvived = value; }
	}
}
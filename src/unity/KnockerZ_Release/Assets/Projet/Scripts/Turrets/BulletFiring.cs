using UnityEngine;
using System.Collections;

public class BulletFiring : MonoBehaviour
{
	// Tableau contenant la position de départ et la position d'arrivée de le projectile
	public Transform[] v_position;
	// Taille du tableau
	private int _arraySize;
	// Compteur de projectiles
	private int Number;
	// Vitesse de le projectile
	public float v_speed = 0.15f;
	public float  v_speedFactor;
	//public Transform v_cam1, v_cam2, v_end;
	// Courbe d'animation
	public AnimationCurve v_curve;
	// Progression de le projectile
	private float _progression;
	private float _progressionR;
	// Durée de vie de le projectile
	public float _lifeTime = 0;
	// Dégat que fait un projectile
	public int damage = 200;
	// Particules de feedback
	[SerializeField] GameObject particles;
	
	void Start ()
	{
		_arraySize = v_position.Length;
		_progression = 0f;
		_progressionR = 0f;
		Number = 0;
		v_position[0] = transform;
	}
	
	void Update()
	{
		// La durée de vie de le projectile augmente
		_lifeTime += Time.deltaTime;
		// Si la durée de vie de le projectile dépasse 4
		if(_lifeTime > 4f)
			// On réinitialise le projectile
			Reset();
	}
	
	void FixedUpdate ()
	{
		// Si le projectile n'a pas de position d'arrivée ou  que le zombie qu'il vise est mort
		if (v_position [1] == null || !v_position[1].gameObject.activeSelf)
		{
			// On la réinitialise
			Reset();
		}
		
		// Si le projectile a une position de départ et une position d'arrivée
		if(v_position [1] != null && v_position [0] != null)
		{
			// Le projectile se déplace de sa position de départ à sa position d'arrivée ...
			transform.position = Vector3.Lerp(v_position[Number].transform.position, v_position[(Number+1)%_arraySize].transform.position, v_curve.Evaluate(_progression));
			transform.rotation = Quaternion.Lerp(v_position[Number].transform.rotation, v_position[(Number+1)%_arraySize].transform.rotation, v_curve.Evaluate(_progression));
			_progression += Time.deltaTime * v_speed;
			// ... et il rotationne en conséquence
			transform.rotation = Quaternion.Lerp(
				v_position[Number].transform.rotation, 
				v_position[(Number+1) % _arraySize].transform.rotation, 
				v_curve.Evaluate(_progressionR)
				);
			
			_progressionR += Time.deltaTime * v_speedFactor;
			
			if ((v_position[(Number+1)%_arraySize].transform.position - transform.position).magnitude < 0.1f)
			{
				_progression = 0f;
				Number = (Number+1) % _arraySize;
				_progressionR = 0f;
			}
		}
	}

	// Lorsque le projectile rencontre un objet
	void OnTriggerEnter(Collider collider)
	{
		// Si l'objet rencontré est un Zombie
		if (collider.gameObject.tag.Equals("Zombie")) 
		{
			// Si le Zombie a plus de 0 point de vie
			if(collider.gameObject.GetComponent<ZombieScript>().Pv > 0)
			{
				// Le projectile lui en enlève 25
				collider.gameObject.GetComponent<ZombieScript>().Pv -= damage;
			}
			// On réinitialise le projectile
			Reset();
		}
	}
	
	// Méthode de réinitialisation du projectile
	public void Reset()
	{
		// On réactive le feedbacks
		particles.SetActive(false);
		// Et on le place au point d'impact
		particles.transform.position = transform.position;
		particles.SetActive (true);
		_progression = 0f;
		_progressionR = 0f;
		Number = 0;
		_lifeTime = 0;
		transform.localPosition = new Vector3(0,0,0f);
		gameObject.SetActive (false);
	}

	//Accesseurs

	public float V_speed {
		get {
			return v_speed;
		}
		set {
			v_speed = value;
		}
	}

	public int Damage {
		get {
			return damage;
		}
		set {
			damage = value;
		}
	}
}
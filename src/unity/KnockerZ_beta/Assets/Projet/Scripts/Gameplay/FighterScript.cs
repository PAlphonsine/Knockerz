using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FighterScript : MonoBehaviour
{
	// Gestion des phases
	[SerializeField]
	PhasesManager phasesManager;
	// Position de départ du Fighter
	[SerializeField]
	private Transform startPosition;
	// Position de la tourelle
	[SerializeField]
	private Transform posTurret;
	// Points de vie du Fighter
	private int? pv;
	// Cible variable du Fighter
	ZombieScript zombie;
	// Dernier Zombie renconté par le Fighter
	ZombieScript lastZombie;
	// Dégats par seconde du Fighter
	private int? dps;
	// Booléen de déclenchement de combat
	private bool isFighting;
	// Collection des cibles potentielles du Fighter
	private List<ZombieScript> canBeAttacked;
	// Points de vie de départ du Fighter
	private int? startPv;
	// Barre de vie du Fighter
	public GameObject lifeSprite;
	// Chance d'esquiver
	private float? criticalHits;
	// Chance de faire un coup critique
	private float? dodge;
	private int? initPv;
	
	// Use this for initialization
	void Start ()
	{
		// Utilisation de nullable pour savoir si elle ont bien été défini par le script d'initialisation extérieur ou non
		if (!pv.HasValue)
			this.pv = 1000;
		if (!startPv.HasValue)
			this.startPv = this.pv;
		if (!initPv.HasValue)
			this.initPv = 1000;
		if (!dps.HasValue)
			this.dps = 4;
		if (!criticalHits.HasValue)
			this.criticalHits = 0;
		if (!dodge.HasValue)
			this.dodge = 0;
		this.isFighting = false;
		this.canBeAttacked = new List<ZombieScript>();
		this.lastZombie = null;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// La barre de vie voit son échelle dépendre des points de vie du Fighter
		lifeSprite.transform.localScale = new Vector3((float)pv.Value/startPv.Value, 
		                                              lifeSprite.transform.localScale.y, 
		                                              lifeSprite.transform.localScale.z);
	}
	
	void FixedUpdate ()
	{
		// Si l'on est en phase d'action
		if (this.phasesManager.startAction == true)
		{
			// Si le Fighter est vivant
			if (this.pv.Value > 0)
			{
				// Si le fighter est en train de se battre
				if (this.isFighting == true)
				{
					// Le dernier Zombie rencontré est le Zombie avec lequel il se bat
					this.lastZombie = this.zombie;
					// Le Zombie se bat aussi
					this.zombie.IsFighting = true;
					// Le Zombie se bat avec ce Fighter
					this.zombie.Fighter = this;
					// Le Fighter se déplace vers le Zombie avec lequel il se bat
					this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.zombie.transform.position.x, this.zombie.transform.position.y, this.zombie.transform.position.z+1.2f), 0.03f);
					// On calcule si le fighter fait un coup critique
					if(Random.Range(criticalHits.Value, 1f) < criticalHits.Value)
						// Le fighter inflige plus de point de dommage au zombie
						this.zombie.Pv -= this.dps.Value * 3;
					else
						// Le Zombie perd des points de vie normalement
						this.zombie.Pv -= this.dps.Value;
					// Si le Zombie est mort
					if (this.zombie.Pv <= 0)
					{
						// Le Fighter ne se bat plus
						this.isFighting = false;
						// Le Zombie ne se bat plus
						this.zombie.IsFighting = false;
						// Le Zombie mort est retiré de la liste des cibles potentielles du Fighter
						this.canBeAttacked.RemoveAt(0);
						
						// Pour chaque Zombie présent dans la liste des cibles potentielles du Fighter
						for (int i = this.canBeAttacked.Count-1; i >= 0; i--)
						{
							// Si le Zombie s'est trop éloigné du Fighter
							// ou si le Zombie est désactivé ...
							if ((Vector3.Distance(this.transform.position, this.canBeAttacked[i].transform.position) > 5) || this.canBeAttacked[i].gameObject.activeSelf == false)
							{
								// ... on le supprime de la liste
								this.canBeAttacked.RemoveAt(i);
							}
						}
						
						// Si le Fighter a au moins un Zombie à combattre
						if (this.canBeAttacked.Count != 0)
						{
							// Il combat le premier de sa liste de cibles
							this.zombie = this.canBeAttacked[0];
							// Le Fighter se bat
							this.isFighting = true;
						}
					}
				}
				// Sinon, si le Fighter ne se bat pas
				else
				{
					// Il retourne à son poste et ne bouge pas
					this.Guard();
				}
			}
			// Sinon, si le Fighter est mort
			else
			{
				// On le reset
				StartCoroutine(this.Reset());
			}
		}
		// Sinon, si l'on est en phase de réflexion
		else
		{
			// Le Fighter ne combat plus de Zombie
			this.lastZombie = null;
			// Il retourne à son poste et ne bouge pas
			this.Guard();
		}
	}
	
	// Méthode de synchronisation de paramètres du Fighter
	void OnSerializeNetworkView(BitStream stream)
	{
		//stream.Serialize (ref this.pv);
		//stream.Serialize (ref this.dps.Value);
	}
	
	// Le Fighter peut colisionner avec : Zombies
	void OnTriggerEnter(Collider collider)
	{
		// Si l'entité rencontrée est un Zombie de type différent de 2
		if (collider.tag == "Zombie" && collider.transform.GetComponent<ZombieScript>().Type != 2)
		{
			// Le Fighter se bat
			this.isFighting = true;
			//this.zombie = collider.transform.GetComponent<ZombieScript>();
			// Si le Fighter ne combat pas
			// ou que le Zombie rencontré n'est pas le Zombie actuellement en combat avec le Fighter
			if (this.lastZombie == null || collider.gameObject != this.lastZombie.gameObject)
			{
				// On ajoute le Zombie à la liste de cibles du Fighter
				this.canBeAttacked.Add(collider.transform.GetComponent<ZombieScript>());
				// Le Zombie actuellement combattu par le Fighter est sa cible prioritaire
				this.zombie = this.canBeAttacked[0];
			}
			else
			{
				// Le Zombie actuellement combattu par le Fighter est sa cible prioritaire
				this.zombie = this.canBeAttacked[0];
			}
		}
	}
	
	// Lorsqu'une entité sort de la zone du Fighter
	void OnTriggerExit(Collider collider)
	{
		// Si l'entité rencontrée est un Zombie de type différent de 2
		if (collider.tag == "Zombie" && collider.transform.GetComponent<ZombieScript>().Type != 2)
		{
			// Si le Fighter n'a plus aucune cible
			if (this.canBeAttacked.Count == 0)
			{
				// Il ne se bat pas
				this.isFighting = false;
				// Il n'a pas de cible prioritaire
				this.zombie = null;
			}
			else
			{
				// Sinon il se bat
				this.isFighting = true;
			}
		}
	}
	
	// Méthode de garde du Fighter
	public void Guard()
	{
		// Le fighter se rend à son point de ralliement est y reste
		this.transform.position = Vector3.Lerp(this.transform.position, this.startPosition.position, 0.03f);
	}
	
	// Fonction Coroutine de réinitialisation du Fighter
	public IEnumerator Reset()
	{
		// On déplace le Fighter
		this.transform.position = new Vector3 (0, 0, -80f);
		// On attend le prochain FixedUpdate()
		yield return new WaitForFixedUpdate ();
		// On désactive le Fighter
		this.gameObject.SetActive (false);
		// On reset ses points de vie
		this.pv = 1000;
		// Il n'a plus de cible prioritaire
		this.zombie = null;
		// Il n'a plus de cible potentielle
		this.lastZombie = null;
		// Il ne se bat plus
		this.isFighting = false;
		this.transform.position = posTurret.position;
		this.pv = initPv.Value;
	}
	
	// Accesseurs
	public Transform StartPosition
	{
		get { return this.startPosition; }
		set { this.startPosition = value; }
	}
	
	public Vector3 StartPositionPosition
	{
		get { return this.startPosition.position; }
		set { this.startPosition.position = value; }
	}
	
	public int Pv
	{
		get { return this.pv.Value; }
		set { this.pv = value; }
	}
	
	public int Dps
	{
		get { return this.dps.Value; }
		set { this.dps = value; }
	}
	
	public bool IsFighting
	{
		get { return this.isFighting; }
		set { this.isFighting = value; }
	}

	public float CriticalHits {
		get {
			return criticalHits.Value;
		}
		set {
			criticalHits = value;
		}
	}

	public float Dodge {
		get {
			return dodge.Value;
		}
		set {
			dodge = value;
		}
	}

	public int InitPv {
		get {
			return initPv.Value;
		}
		set {
			initPv = value;
		}
	}

	public int StartPv {
		get {
			return startPv.Value;
		}
		set {
			startPv = value;
		}
	}
}

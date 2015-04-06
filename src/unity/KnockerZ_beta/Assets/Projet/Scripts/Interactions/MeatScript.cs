using UnityEngine;
using System.Collections;

public class MeatScript : MonoBehaviour {
	// Points de vie du morceau de viande
	private int durability;
	
	// Use this for initialization
	void Start () 
	{
		this.durability = 400;
	}
	
	void FixedUpdate ()
	{
		// Si le morceau de viande n'a plus de points de vie
		if (this.durability <= 0)
		{
			// On le reset
			StartCoroutine(this.Reset());
		}
	}
	
	// Méthode de synchronisation des paramètres du morceau de viande
	void OnSerializeNetworkView(BitStream stream)
	{
		stream.Serialize (ref this.durability);
	}
	
	// Fonction Coroutine de réinitialisation du morceau de viande
	public IEnumerator Reset()
	{
		// On déplace le morceau de viande
		this.transform.position = new Vector3 (0, 0, -1);
		// On attend le porchain FixedUpdate()
		yield return new WaitForFixedUpdate ();
		// On désactive le morceau de viande
		this.gameObject.SetActive (false);
		// On réinitialise ses points de vie
		this.durability = 400;
	}
	
	// Accesseurs
	public int Durability
	{
		get { return this.durability; }
		set { this.durability = value; }
	}
}

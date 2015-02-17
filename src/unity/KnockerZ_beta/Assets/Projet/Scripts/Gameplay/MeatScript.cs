using UnityEngine;
using System.Collections;

public class MeatScript : MonoBehaviour {

	// Points de vie du morceau
	private int durability;

	// Use this for initialization
	void Start () {
		this.durability = 400;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		if (this.durability <= 0)
		{
			StartCoroutine(this.Reset());
		}
	}

	void OnSerializeNetworkView(BitStream stream)
	{
		stream.Serialize (ref this.durability);
	}

	public IEnumerator Reset()
	{
		this.transform.position = new Vector3 (0, 0, -1);
		yield return new WaitForFixedUpdate ();
		this.gameObject.SetActive (false);
		this.durability = 400;
	}

	// Accesseurs
	public int Durability
	{
		get { return this.durability; }
		set { this.durability = value; }
	}
}

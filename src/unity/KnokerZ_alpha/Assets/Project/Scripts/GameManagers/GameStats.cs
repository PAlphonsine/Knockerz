using UnityEngine;
using System.Collections;

public class GameStats : MonoBehaviour{

	void OnSerializeNetworkView(BitStream stream){
		stream.Serialize(ref pv);
		stream.Serialize(ref ressources);
	}

	private static GameStats _instance;

	public static GameStats Instance {
		get
		{
			if(_instance == null){
				//_instance = GameObject.FindObjectOfType(typeof(GameStats)) as GameStats;
				GameObject objet = new GameObject("GameStats");
				_instance = objet.AddComponent<GameStats>();
			}
			return _instance;
		}
	}

	[SerializeField]
	private int pv= 20;

	public int Pv{
		get { return pv; }
		set { pv = value; }
	}

	[SerializeField]
	private int ressources= 400;
	
	public int Ressources{
		get { return ressources; }
		set { ressources = value; }
	}

	[SerializeField]
	private int population = 10;
	
	public int Population{
		get { return population; }
		set { population = value; }
	}

	private int populationSend = 0;

	[SerializeField]
	public int PopulationSend{
		get { return populationSend; }
		set { populationSend = value; }
	}

	[SerializeField]
	private int pv1 = 10;
	
	public int Pv1{
		get { return pv1; }
		set { pv1 = value; }
	}

	[SerializeField]
	private int pv2 = 10;
	
	public int Pv2{
		get { return pv2; }
		set { pv2 = value; }
	}
}

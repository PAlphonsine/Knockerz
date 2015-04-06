using UnityEngine;
using System.Collections;

public class GameStats : MonoBehaviour
{
	// Instance de statistiques de jeu
	private static GameStats _instance;
	// Points de vie de la base du joueur (population civile)
	[SerializeField]
	private int pv = 20;
	// Ressources de type matériaux du joueur
	[SerializeField]
	private int ressourcesMat = 600;
	// Ressources de type armes du joueur
	[SerializeField]
	private int ressourcesWeap = 600;
	// Or du joueur
	[SerializeField]
	private int gold = 500;
	// Population du joueur
	[SerializeField]
	private int population = 20;
	// Population envoyée à la recherche de ressources
	[SerializeField]
	private int populationSend = 0;
	// Expérience du joueur
	[SerializeField]
	private int exp = 0;
	
	// Synchronisation des paramètres du script
	void OnSerializeNetworkView(BitStream stream)
	{
		stream.Serialize(ref pv);
		stream.Serialize(ref ressourcesMat);
		stream.Serialize(ref ressourcesWeap);
		stream.Serialize(ref gold);
	}
	
	// Accesseurs
	public static GameStats Instance
	{
		get
		{
			if (_instance == null)
			{
				//_instance = GameObject.FindObjectOfType(typeof(GameStats)) as GameStats;
				GameObject objet = new GameObject("GameStats");
				_instance = objet.AddComponent<GameStats>();
			}
			return _instance;
		}
	}
	
	public int Pv
	{
		get { return pv; }
		set { pv = value; }
	}
	
	public int RessourcesMat
	{
		get { return ressourcesMat; }
		set { ressourcesMat = value; }
	}
	
	public int RessourcesWeap
	{
		get { return ressourcesWeap; }
		set { ressourcesWeap = value; }
	}
	
	public int Gold
	{
		get { return this.gold; }
		set { this.gold = value; }
	}
	
	public int Population
	{
		get { return population; }
		set { population = value; }
	}
	
	public int PopulationSend
	{
		get { return populationSend; }
		set { populationSend = value; }
	}
	
	public int Exp
	{
		get { return exp; }
		set { exp = value; }
	}
}

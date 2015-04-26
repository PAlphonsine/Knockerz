using UnityEngine;
using System.Collections;

public class StockZombies : MonoBehaviour
{
	// Tableaux contenant les différents types de Zombie
	public GameObject[] zombiesType1;
	public GameObject[] zombiesType2;
	public GameObject[] zombiesType3;
	// Tableau contenant les Survivants
	public GameObject[] SurvivorsType1;
	// Script du Survivant
	public SurvivorScript[] _survivorScript;
	// Booléen de controle d'appartenance du stock au joueur 1 ou au joueur 2
	[SerializeField]
	bool stock1;
	
	// Méthode de confirmation de volonté de changer de skin de Zombies ou de Survivants du joueur
	public void WantChangeSkin(int skinWant)
	{
		// Si le joueur est le joueur 1 ...
		if (Network.player == _STATICS._networkPlayer[0])
		{
			// ... on change son skin et il est visible pour les deux joueurs
			networkView.RPC("IsSkinAvaible", RPCMode.Server, Network.player, _STATICS._playersInGame[0], skinWant);
		}
		// Sinon, si le joueur est le joueur 2 ...
		else
		{
			// ... on change son skin et il est visible pour les deux joueurs
			networkView.RPC("IsSkinAvaible", RPCMode.Server, Network.player, _STATICS._playersInGame[1], skinWant);
		}
	}
	
	// RPC de vérification de niveau du joueur
	[RPC]
	public void IsSkinAvaible(NetworkPlayer player, string login, int skinWant)
	{
		// Le serveur vérifie si le joueur a suffisament d'expérience pour obtenir le skin demandé
		// Si c'est le cas
		if (PlayerPrefs.HasKey(login+"XP"))
			networkView.RPC("SetSkinToZombies", RPCMode.OthersBuffered, player, PlayerPrefs.GetInt (login + "XP"), skinWant);
		// Sinon, si ce n'est pas le cas
		else
			networkView.RPC("SetSkinToZombies", RPCMode.OthersBuffered, player , 0, skinWant);
	}
	
	// RPC de mise en place du skin en fonction du niveau
	[RPC]
	void SetSkinToZombies(NetworkPlayer player, int xp, int skinWant)
	{
		// Si le joueur est bien celui qui veut changer de skin
		if ((player == _STATICS._networkPlayer[0] && (stock1)) || (player == _STATICS._networkPlayer[1] && (!stock1)))
		{
			switch (skinWant)
			{
				// Selon le choix de skin du joueur
			case 0:
				// Pour chaque Survivant dans le tableau de Survivant
				foreach (var survivor in SurvivorsType1)
				{
					// On change sa couleur
					survivor.renderer.material.color = Color.cyan;
				}
				// Pour chaque script de Survivant dans le tableau de scripts de Survivant
				foreach (var script in _survivorScript)
				{
					// On change la couleur de sa tete également
					script.Head.renderer.material.color = Color.cyan;
				}
				// Pour chaque script de Survivant dans le tableau de scripts de Survivant
				foreach (var script in _survivorScript)
				{
					// On désactive son chapeau
					script.Hat.SetActive(false);
					// On désactive son sac
					script.Backpack.SetActive(false);
				}
				break;
			case 1:
				// Pour chaque Survivant dans le tableau de Survivant
				foreach (var survivor in SurvivorsType1)
				{
					// On change sa couleur
					survivor.renderer.material.color = Color.magenta;
				}
				// Pour chaque script de Survivant dans le tableau de scripts de Survivant
				foreach (var script in _survivorScript)
				{
					// On change la couleur de sa tete également
					script.Head.renderer.material.color = Color.magenta;
				}
				// Pour chaque script de Survivant dans le tableau de scripts de Survivant
				foreach (var script in _survivorScript)
				{
					// On désactive son chapeau
					script.Hat.SetActive(false);
					// On désactive son sac
					script.Backpack.SetActive(false);
				}
				break;
			case 2:
				// Pour chaque Survivant dans le tableau de Survivant
				foreach (var survivor in SurvivorsType1)
				{
					// On change sa couleur
					survivor.renderer.material.color = Color.white;
				}
				// Pour chaque script de Survivant dans le tableau de scripts de Survivant
				foreach (var script in _survivorScript)
				{
					// On change la couleur de sa tete également
					script.Head.renderer.material.color = Color.white;
				}
				// Pour chaque script de Survivant dans le tableau de scripts de Survivant
				foreach (var script in _survivorScript)
				{
					// On active son chapeau
					script.Hat.SetActive(true);
					// On désactive son sac
					script.Backpack.SetActive(false);
				}
			break;
			case 3:
				// Pour chaque Survivant dans le tableau de Survivant
				foreach (var survivor in SurvivorsType1)
				{
					// On change sa couleur
					survivor.renderer.material.color = Color.red;
				}
				// Pour chaque script de Survivant dans le tableau de scripts de Survivant
				foreach (var script in _survivorScript)
				{
					// On change la couleur de sa tete également
					script.Head.renderer.material.color = Color.red;
				}
				// Pour chaque script de Survivant dans le tableau de scripts de Survivant
				foreach (var script in _survivorScript)
				{
					// On désactive son chapeau
					script.Hat.SetActive(false);
					// On active son sac
					script.Backpack.SetActive(true);
				}
				break;
			}
		}
	}

	public bool Stock1 {
		get {
			return stock1;
		}
		set {
			stock1 = value;
		}
	}
}
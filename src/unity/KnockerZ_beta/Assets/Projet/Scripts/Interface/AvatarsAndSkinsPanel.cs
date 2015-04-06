using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AvatarsAndSkinsPanel : MonoBehaviour
{
	// Tableau des caches d'avatar
	[SerializeField]
	private GameObject[] _crossAvatars;
	// Tableau des caches de skins
	[SerializeField]
	private GameObject[] _crossSkins;
	// Experiences necessaires pour les différents avatars et skins de survivants
	[SerializeField]
	int[] xpToAvatarsAndSkins;
	
	// Méthode d'obtention des avatars disponibles
	public void GetAvatarsAvaible(NetworkPlayer player, string login)
	{
		// Si le joueur a assez d'expérience
		if (PlayerPrefs.HasKey(login+"XP"))
			// Il obtient l'avatar
			networkView.RPC("SetAvatars", RPCMode.OthersBuffered, player, PlayerPrefs.GetInt (login + "XP"));
		else
			// Sinon, il ne peut pas changer d'avatar
			networkView.RPC("SetAvatars", RPCMode.OthersBuffered, player , 0);
	}
	
	// RPC de changement d'avatar
	[RPC]
	void SetAvatars(NetworkPlayer player, int xp)
	{
		// Si le joueur est bien un joueur
		if (player == Network.player)
		{
			// Si le joueur a moins de 10 xp
			if (xp < xpToAvatarsAndSkins[0])
			{
				// Il a l'avatar de base
				SetPlayerAvatarsAndSkins(0);
			}
			else
			{
				// Sinon, si le joueur a moins de 20 xp
				if (xp < xpToAvatarsAndSkins[1])
					// Il obtient le deuxième avatar
					SetPlayerAvatarsAndSkins(1);
				else
				{
					// Sinon, si le joueur a moins de 30 xp
					if (xp < xpToAvatarsAndSkins[2])
						// Il obtient le troisième avatar
						SetPlayerAvatarsAndSkins(2);
					else
					{
						// Sinon, si le joueur a moins de 40 xp
						if (xp < xpToAvatarsAndSkins[3])
							// Il obtient le quatrième avatar
							SetPlayerAvatarsAndSkins(3);
						else
						{
							// Sinon, il obtient le cinquième avatar
							SetPlayerAvatarsAndSkins(4);
						}
					}
				}
			}
		}
	}
	
	// Méthode d'attribution des avatars
	void SetPlayerAvatarsAndSkins(int nb)
	{
		// Pour chaque cache d'avatar existant
		for (int i = 0; i < _crossAvatars.Length; i++)
		{
			// On désactive la croix
			_crossAvatars[i].SetActive(false);
		}

		// Pour chaque cache de skin existant
		for (int i = 0; i < _crossSkins.Length; i++)
		{
			// On désactive la croix
			_crossSkins[i].SetActive(false);
		}
	}
}
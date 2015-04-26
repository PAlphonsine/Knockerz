using UnityEngine;
using System.Collections;

public class ErrorLevelPanelScript : MonoBehaviour
{
	// Panel d'erreur
	[SerializeField]
	private GameObject errorPanel;
	// Montant d'expérience nécessaire
	[SerializeField]
	private int xpToEnter = 0;
	
	// Méthode de vérification du niveau du joueur
	public void IsLevelAvaible(NetworkPlayer player, string login)
	{
		// On affiche la fenetre avec certaines informations du joueur si possible
		if (PlayerPrefs.HasKey(login+"XP"))
			networkView.RPC("SetErrorLevelScreen", RPCMode.OthersBuffered, player, PlayerPrefs.GetInt (login + "XP"));
		else
			networkView.RPC("SetErrorLevelScreen", RPCMode.OthersBuffered, player , 0);
	}
	
	// RPC d'affichage de la fenetre d'erreur
	[RPC]
	void SetErrorLevelScreen(NetworkPlayer player, int xp)
	{
		// Si le joueur est un bien un joueur
		if (player == Network.player)
		{
			// Si l'expérience du joueur est inférieure à l'expérience nécessaire pour accéder à la carte
			if (xp < xpToEnter)
			{
				// La fenetre d'erreur apparait
				errorPanel.SetActive(true);
			}
		}
	}
}
using UnityEngine;
using System.Collections;

public class _STATICS
{
	// Tableau contenant les NetworkPlayer des deux joueurs présent dans la partie afin de les identifier
	public static NetworkPlayer[] _networkPlayer = new NetworkPlayer[2];
	// Tableau contenant les logins des joueurs en jeu (maximum 2)
	public static string[] _playersInGame = new string[2];
	// Tableaux pour stocker les statistiques des joueurs, pour la reconnection
	public static int[] _statsJ1 = new int[6];
	public static int[] _statsJ2 = new int[6];
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProfilePanelManager : MonoBehaviour
{
	// Tableau des avatars en jeu
	[SerializeField] Image[] _avatarsInGame;
	// Tableau des sprites des avatars
	[SerializeField] Sprite[] _avatars;
	// Texte du niveau du joueur
	[SerializeField] private Text[] _levelText;
	// Texte du pseudo des joueurs
	[SerializeField] private Text[] _nickname;
	
	// Méthode de demande de changement d'avatar
	public void WantsChangeAvatar(int chosenAvatar)
	{
		// S'il s'agit du joueur 1
		if(_STATICS._networkPlayer[0] == Network.player)
		{
			// On synchronise l'avatar choisi
			networkView.RPC ("ChangeAvatarInServer", RPCMode.Server, 0, chosenAvatar);
		}
		// Sinon, s'il s'agit du joueur 2
		else
		{
			// On crée la référence de l'avatar dans les PlayerPrefs avec comme clé le numéro du joueur
			PlayerPrefs.SetInt(_STATICS._playersInGame[1], chosenAvatar);
			// On synchronise l'avatar choisi sur le serveur
			networkView.RPC ("ChangeAvatarInServer", RPCMode.Server, 1, chosenAvatar);
		}
	}
	
	// RPC de changement de l'avatar sur le serveur
	[RPC]
	void ChangeAvatarInServer(int player, int chosenAvatar)
	{
		// On crée la référence de l'avatar dans les PlayerPrefs avec comme clé le numéro du joueur
		PlayerPrefs.SetInt(_STATICS._playersInGame[player]+"avatar", chosenAvatar);
		// On synchronise l'avatar choisi avec tout le monde
		networkView.RPC ("RefreshAvatarInGame", RPCMode.Others, _STATICS._playersInGame[player], chosenAvatar);
	}
	
	// Méthode de rafraichissement de l'avatar lors de la connexion
	public void RefreshAvatar(string login)
	{
		// Si les PlayersPrefs du joueur ne contiennent pas la clé correspondante au login du joueur + "avatar"
		if (!PlayerPrefs.HasKey (login + "avatar"))
			// On lui attribue
			PlayerPrefs.SetInt (login + "avatar", 0);
		// On synchronise avec tout le monde
		networkView.RPC ("RefreshAvatarInGame", RPCMode.Others, login, PlayerPrefs.GetInt (login + "avatar"));
	}
	
	// RPC de rafraichissement de l'avatar en jeu
	[RPC]
	void RefreshAvatarInGame(string login, int _avatar)
	{
		// Integer permettant de définir le joueur
		int _player;
		// Si le joueur est le joueur 1
		if(login == _STATICS._playersInGame[0])
		{
			// On le définit à 0
			_player = 0;
		}
		// Sinon, si c'est le joueur 2
		else
		{
			// On le définit à 1
			_player = 1;
		}
		// Si le login correspond au joueur 1 ou au joueur 2
		if(login == _STATICS._playersInGame[0] || login == _STATICS._playersInGame[1])
		{
			// Selon l'avatar ...
			switch (_avatar)
			{
				// ... on affiche l'image correspondante et son sprite
			case 0:
				_avatarsInGame[_player].color = Color.white;
				_avatarsInGame[_player].sprite = null;
				break;
			case 1:
				_avatarsInGame[_player].color = new Color32(144,33,202,255);
				_avatarsInGame[_player].sprite = null;
				break;
			case 2:
				_avatarsInGame[_player].color = Color.white;
				_avatarsInGame[_player].sprite = _avatars[0];
				break;
			case 3:
				_avatarsInGame[_player].color = Color.white;
				_avatarsInGame[_player].sprite = _avatars[1];
				break;
			case 4:
				_avatarsInGame[_player].color = Color.white;
				_avatarsInGame[_player].sprite = _avatars[2];
				break;
			default:
				break;
			}
		}
	}
	
	// Méthode de récupération des niveaux
	public void GetLevels(string login)
	{
		// On synchronise l'affichage du niveau du joueur avec tout le monde
		if(PlayerPrefs.HasKey(login+"XP"))
		{
			networkView.RPC("SetLevelsText", RPCMode.OthersBuffered, login, PlayerPrefs.GetInt (login + "XP"));
		}
		else
		{
			networkView.RPC("SetLevelsText", RPCMode.OthersBuffered, login, 0);
		}
	}
	
	// RPC d'affichage du niveau du joueur
	[RPC]
	void SetLevelsText(string login, int xp)
	{
		// Si le login correspond au joueur 1
		if (login == _STATICS._playersInGame [0])
		{
			// Si le joueur 1 a 0 d'expérience
			if (xp == 0)
			{
				// ... on lui attribue le rang de Noob
				_levelText [0].text = "Noob";
			}
			else
			{
				// Sinon, on lui affiche son niveau
				_levelText [0].text = "Level " + xp/10;
			}
			// On affiche le pseudo du joueur 1
			_nickname[0].text = login;
		}
		// Si le login correspond au joueur 2
		if (login == _STATICS._playersInGame [1])
		{
			// Si le joueur 2 a 0 d'expérience ...
			if (xp == 0)
			{
				// ... on lui attribue le rang de Noob
				_levelText [1].text = "Noob";
			}
			else
			{
				// Sinon, on lui affiche son niveau
				_levelText [1].text = "Level " + xp/10;
			}
			// On affiche le pseudo du joueur 2
			_nickname[1].text = login;
		}
	}
}

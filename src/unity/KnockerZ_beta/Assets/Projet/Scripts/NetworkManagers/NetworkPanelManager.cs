using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkPanelManager : MonoBehaviour {

	public Image[] _avatarsInGame;
	public Sprite[] _avatars;
	
	void Start () {
	
	}

	void Update () {
	
	}

	public void WantsChangeAvatar(int chosenAvatar){
		if(_STATICS._networkPlayer[0] == Network.player){
			networkView.RPC ("ChangeAvatarInServer", RPCMode.Server, 0, chosenAvatar);
		}else{
			PlayerPrefs.SetInt(_STATICS._playersInGame[1], chosenAvatar);
			networkView.RPC ("ChangeAvatarInServer", RPCMode.Server, 1, chosenAvatar);
		}
	}

	[RPC]
	void ChangeAvatarInServer(int player, int chosenAvatar){
		PlayerPrefs.SetInt(_STATICS._playersInGame[player]+"avatar", chosenAvatar);
		networkView.RPC ("RefreshAvatarInGame", RPCMode.Others, _STATICS._playersInGame[player], chosenAvatar);
	}

	public void RefreshAvatar(string login){
		if (!PlayerPrefs.HasKey (login + "avatar"))
			PlayerPrefs.SetInt (login + "avatar", 0);
		networkView.RPC ("RefreshAvatarInGame", RPCMode.Others, login, PlayerPrefs.GetInt (login + "avatar"));
	}

	[RPC]
	void RefreshAvatarInGame(string login, int _avatar){
		int _player;
		if(login == _STATICS._playersInGame[0]){
			_player=0;
		}else{
			_player=1;
		}
		if(login == _STATICS._playersInGame[0] || login == _STATICS._playersInGame[1]){
			switch (_avatar) {
			case 0:
				_avatarsInGame[_player].color = Color.white;
				_avatarsInGame[_player].sprite = null;
				break;
			case 1:
				_avatarsInGame[_player].color = Color.blue;
				_avatarsInGame[_player].sprite = null;
				break;
			case 2:
				_avatarsInGame[_player].color = Color.green;
				_avatarsInGame[_player].sprite = null;
				break;
			case 3:
				_avatarsInGame[_player].color = Color.white;
				_avatarsInGame[_player].sprite = _avatars[3];
				break;
			case 4:
				_avatarsInGame[_player].color = Color.white;
				_avatarsInGame[_player].sprite = _avatars[4];
				break;
			default:
				Debug.Log("avatar doesn't exist");
				break;
			}
		}
	}
}

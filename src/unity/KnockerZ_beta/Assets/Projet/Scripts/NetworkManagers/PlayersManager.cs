using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayersManager : MonoBehaviour {

	public PlayerScript[] _playerScripts;

	public Camera _serverCamera;
	public PhasesManager _phasesManager;

	public int nbPlayers;

	public StartServerScript _startServerScript;

	//Nombre de joueurs
	int _cnt = 0;

	int _playersReady=0;

	public NetworkPanelManager _networkPanelManager;
	public AvatarsPanel _avatarPanel;
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		networkView.RPC ("SendPlayerIdent", RPCMode.Others, player);
	}

	[RPC]
	void ReceivePlayerIdent(NetworkPlayer player, string login, int password, bool isInscription){
		if (isInscription) {
			if(!PlayerPrefs.HasKey(login) && login != "")
			PlayerPrefs.SetInt (login, password * PlayerPrefs.GetInt("grain"));
		}
		if (PlayerPrefs.HasKey (login) && PlayerPrefs.GetInt (login) == password * PlayerPrefs.GetInt("grain")){
			if (Network.connections.Length <= 2) //0 au start, 1 à 1 joueur
			{
				//Debug.Log("ok");
				networkView.RPC ("ConfigurePlayerScript", RPCMode.AllBuffered, player, _cnt++, login);
				//networkView.RPC ("ConfigurePlayerScript", RPCMode.AllBuffered, player, _cnt++, _startServerScript.StartGame);
			}
			if (Network.connections.Length == nbPlayers) {
				//networkView.RPC ("StartGamePlayer", RPCMode.AllBuffered);
			}
		}else{
			_cnt++;
			Network.CloseConnection(player, true);
		}
	}

	[RPC]
	void SendPlayerIdent(NetworkPlayer player){
		if(Network.player == player)
			networkView.RPC ("ReceivePlayerIdent", RPCMode.Server, player, _startServerScript.Login, _startServerScript.Password, _startServerScript.isInscription);
	}

	//Mettre ici l'écran d'acceuil des joueurs
	[RPC]
	void ConfigurePlayerScript (NetworkPlayer player, int cnt, string login)
	{
		Time.timeScale = 1f;
		_playerScripts [cnt]._networkPlayer = player;
		_playerScripts [cnt].networkView.enabled = true;

		_STATICS._networkPlayer [cnt] = player;
		_STATICS._playersInGame [cnt] = login;
		
		if (Network.player == player) {
			_startServerScript.ConnectionOk();
			_playerScripts [cnt].PlayerCamera.SetActive(true);
			_serverCamera.enabled = false;
			_playerScripts [cnt].EnemyBase.isTheEnemyBase = true;
			_playerScripts [cnt].StartButton.SetActive(true);
			networkView.RPC("CallRefreshAvatar", RPCMode.Server, login);
			if(cnt == 1)
				networkView.RPC("CallRefreshAvatar", RPCMode.Server, _STATICS._playersInGame[0]);
		}

		if(Network.isServer)
			_avatarPanel.GetAvatarsAvaible(player, login);
	}

	//Parce que le rpc du serveur est appelé avant celui du client (donc pas de static.playingame)
	[RPC]
	void CallRefreshAvatar(string login){
		_networkPanelManager.RefreshAvatar (login);
	}

	void OnPlayerDisconnected (NetworkPlayer player)
	{
		_cnt--;
		if (_cnt < 2) {
			Time.timeScale = 0.0f;
		}
	}

	public void PlayerReady(){
		networkView.RPC("PlayersReady", RPCMode.Server);
	}

	[RPC]
	void PlayersReady(){
		_playersReady++;
		print (_playersReady);
		if(_playersReady == 2){
			networkView.RPC ("StartGamePlayer", RPCMode.AllBuffered);
		}
	}

	[RPC]
	void StartGamePlayer(){
		if (Network.isClient) {
			if(Network.player == _STATICS._networkPlayer[0])
				_playerScripts [0].StartButton.SetActive(false);
			else
				_playerScripts [1].StartButton.SetActive(false);
		}
		_phasesManager.startgame = true;
	}
}

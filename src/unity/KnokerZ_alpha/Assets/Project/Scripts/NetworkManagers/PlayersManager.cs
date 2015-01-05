using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayersManager : MonoBehaviour {

	public PlayerScript[] _playerScripts;

	public Camera _serverCamera;
	public PhasesManager _phasesManager;

	public int nbPlayers;

	//Nombre de joueurs
	int _cnt = 0;
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		if (Network.connections.Length <= 2) //0 au start
		{
			//Debug.Log("ok");
			networkView.RPC ("ConfigurePlayerScript", RPCMode.AllBuffered, player, _cnt++);
			//networkView.RPC ("ConfigurePlayerScript", RPCMode.AllBuffered, player, _cnt++, _startServerScript.StartGame);
		}
		if (Network.connections.Length == nbPlayers) {
			networkView.RPC ("StartGamePlayer", RPCMode.AllBuffered);
		}
	}

	//Mettre ici l'écran d'acceuil des joueurs
	[RPC]
	void ConfigurePlayerScript (NetworkPlayer player, int cnt)
	{
		_playerScripts [cnt]._networkPlayer = player;
		_playerScripts [cnt].networkView.enabled = true;

		_STATICS._networkPlayer [cnt] = player;
		
		if (Network.player == player) {
			_playerScripts [cnt].PlayerCamera.SetActive(true);
			_serverCamera.enabled = false;
			_playerScripts [cnt].EnemyBase.isTheEnemyBase = true;
		}
	}

	void OnPlayerDisconnected (NetworkPlayer player)
	{
		_cnt--;
	}

	[RPC]
	void StartGamePlayer(){
		_phasesManager.startgame = true;
	}
}

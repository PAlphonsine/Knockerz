using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayersManager : MonoBehaviour
{
	// Tableau de PlayerScripts
	public PlayerScript[] _playerScripts;
	// Caméra du serveur
	public Camera _serverCamera;
	// Gestion des phases
	public PhasesManager _phasesManager;
	// Script de démarrage du serveur
	public StartServerScript _startServerScript;
	// Compteur du nombre de joueurs
	int _connected = 0;
	// Panel de gestion des avatars du joueur
	public ProfilePanelManager _ProfilePanelManager;
	// Panel d'avatars
	public AvatarsAndSkinsPanel _avatarPanel;
	// Bouton du menu ingame
	public InGameMenuButtonScript _inGameMenuButtonScript;
	// Pour la configuration des paramètres joueur
	public SupportInventoryManager _supportInventoryManager;
	[SerializeField]
	public BadActsInventoryManager _badActsInventoryManager;
	// Panel d'affichage d'erreur de niveau nécessaire
	public ErrorLevelPanelScript _errorLevelPanelScript;
	// Liste des joueurs prets à jouer
	private List<NetworkPlayer> listPlayersReady = new List<NetworkPlayer> ();
	// Permet d'appeller la fonction d'inversion du panel pour le joueur 2
	[SerializeField]
	private ChangeBottomPanelStruct _changeBottomPanelStruct;
	private List<string> playersPlaying = new List<string>();
	
	// Méthode appellée lors de la connexion d'un joueur
	void OnPlayerConnected(NetworkPlayer player)
	{
		// Le nombre de joueur est incrémenté
		_connected++;
		// On synchronise les informations du joueur avec tout le monde
		networkView.RPC ("GetInformations", RPCMode.Others, player);
	}

	// RPC du reçu de l'identité du joueur
	[RPC]
	void ReceivePlayerIdent(NetworkPlayer player, string login, int password, bool isInscription, int answer)
	{
		// Si le joueur s'inscrit
		if (isInscription)
		{
			// Si le joueur à rentré un pseudo et que le pseudo n'existe pas
			if(login != ""){
				if (!PlayerPrefs.HasKey(login))
				{
					// On crée le compte du joueur avec son login et son mot de passe ...
					PlayerPrefs.SetInt (login, password * PlayerPrefs.GetInt("grain"));
					// ... ainsi que la réponse à la question secrète
					PlayerPrefs.SetInt (login + "Sanwer", answer * PlayerPrefs.GetInt("grain"));
				}else
				{
					// Sinon on affiche une erreur
					_startServerScript.networkView.RPC("ConnectionFaild", RPCMode.OthersBuffered, 2);
					// Sinon, le joueur est éjecté
					Network.CloseConnection(player, true);
				}

			}else
			{
				// Sinon on affiche une erreur
				_startServerScript.networkView.RPC("ConnectionFaild", RPCMode.OthersBuffered, 1);
				// Sinon, le joueur est éjecté
				Network.CloseConnection(player, true);
			}
		}

		// Si le joueur se connecte et que son login et son mot de passe correspondent bien
		// ou que les deux joueur sont connectés
		if (PlayerPrefs.HasKey (login) && PlayerPrefs.GetInt (login) == password * PlayerPrefs.GetInt("grain")){
		    // Si la partie n'a pas commencé, on empèche un joueur de se connecter deux fois
			if (!_phasesManager.startgame && (_STATICS._playersInGame[0] != login && _STATICS._playersInGame[1] != login))
			{
				// Si il y a moins de deux joueurs connectés
				if (Network.connections.Length <= 2) //0 au start, 1 à 2 joueur
				{
					if (!playersPlaying.Contains(login))
						playersPlaying.Add(login);
					// On configure ce ou ces joueurs
					if (_STATICS._playersInGame [0] == null){
						networkView.RPC ("ConfigurePlayerScript", RPCMode.AllBuffered, player, 1, login);
						if (playersPlaying.Count == 2)
							// On configure le deuxième joueur
							networkView.RPC("CallRefreshAvatar", RPCMode.Server, _STATICS._playersInGame[1]);
					}
					else
						networkView.RPC ("ConfigurePlayerScript", RPCMode.AllBuffered, player, 2, login);
				}else{
					// Sinon on affiche une erreur
					_startServerScript.networkView.RPC("ConnectionFaild", RPCMode.OthersBuffered, 6);
					// Sinon, le joueur est éjecté
					Network.CloseConnection(player, true);
				}
			}
			// Si la partie a commencé, qu'un joueur c'est déconnecté et qu'un 3ième joueur veut se connecter
			else if(_phasesManager.startgame && ((_STATICS._playersInGame[0] == login) || (_STATICS._playersInGame[1] == login)))
			{
				// Si il y a moins de deux joueurs connectés
				if (Network.connections.Length <= 2) //0 au start, 1 à 2 joueur
				{
					if (!playersPlaying.Contains(login))
					{
					    playersPlaying.Add(login);
						// On configure ce ou ces joueurs
						if (_STATICS._playersInGame [0] == login){
							// On configure le premier joueur
							networkView.RPC ("ConfigurePlayerScript", RPCMode.AllBuffered, player, 1, login); 
							// Mise à jour de l'avatar du deuxième joueur pour le premier joueur dans le cadre de sa reconnexion
							if (playersPlaying.Count == 2)
								// On configure le deuxième joueur
								networkView.RPC("CallRefreshAvatar", RPCMode.Server, _STATICS._playersInGame[1]);
						}
						else
							networkView.RPC ("ConfigurePlayerScript", RPCMode.AllBuffered, player, 2, login);
					}
					else
					{
						// Sinon on affiche une erreur
						_startServerScript.networkView.RPC("ConnectionFaild", RPCMode.OthersBuffered, 5);
						// Sinon, le joueur est éjecté
						Network.CloseConnection(player, true);
					}
				}
				else
				{
					// Sinon on affiche une erreur
					_startServerScript.networkView.RPC("ConnectionFaild", RPCMode.OthersBuffered, 6);
					// Sinon, le joueur est éjecté
					Network.CloseConnection(player, true);
				}
			}
			// Sinon on affiche une erreur
			else
			{
				// Gestion des erreurs dans les paramatres du joueur
				if (!_phasesManager.startgame)
					_startServerScript.networkView.RPC("ConnectionFaild", RPCMode.OthersBuffered, 5);
				else
					_startServerScript.networkView.RPC("ConnectionFaild", RPCMode.OthersBuffered, 6);
				// Sinon, le joueur est éjecté
				Network.CloseConnection(player, true);
			}
		}
		else
		{
			// Si le login n'existe pas
			if (!PlayerPrefs.HasKey (login))
				_startServerScript.networkView.RPC("ConnectionFaild", RPCMode.OthersBuffered, 3);
			// Si le mot de passe n'est pas bon
			else if(PlayerPrefs.GetInt (login) != password * PlayerPrefs.GetInt("grain"))
				_startServerScript.networkView.RPC("ConnectionFaild", RPCMode.OthersBuffered, 4);
			// Sinon, le joueur est éjecté
			Network.CloseConnection(player, true);
		}
	}
	
	// RPC d'envoi au serveur des informations du joueur
	[RPC]
	void SendPlayerIdent(NetworkPlayer player)
	{
		// Si le joueur est un bien un joueur connecté
		if(Network.player == player)
			// On récupère son identité
			networkView.RPC ("ReceivePlayerIdent", RPCMode.Server, player, _startServerScript.Login, _startServerScript.Password, _startServerScript.IsInscription, _startServerScript.Newanswer);
	}
	
	// RPC de récupération des informations du joueur
	[RPC]
	void GetInformations(NetworkPlayer player)
	{
		// Si le joueur est un bien un joueur connecté
		if (Network.player == player)
		{
			// Si le joueur s'est connecté pour changer son mot de passe
			if (_startServerScript.connectToChangePassword)
			{
				// Le joueur change son mot de passe
				_startServerScript.connectToChangePassword = false;
				// On informe le serveur que le joueur chage son mot de passe
				networkView.RPC("ChangePassword", RPCMode.Server, player, _startServerScript.Slogin, _startServerScript.NewPassword, _startServerScript.Sanswer);
			}
			else
			{
				// Sinon, on reçoit simplement ses informations
				networkView.RPC ("ReceivePlayerIdent", RPCMode.Server, player, _startServerScript.Login, _startServerScript.Password, _startServerScript.IsInscription, _startServerScript.Newanswer);
			}
		}
	}
	
	// RPC de changement de mot de passe
	[RPC]
	void ChangePassword(NetworkPlayer player, string login, int password, int answer)
	{
		// Si le login correspond au joueur
		if(PlayerPrefs.HasKey(login))
		{
			// Si le joueur possède bien le bon login et qu'il répond correctement à la question secrète
			if(PlayerPrefs.HasKey(login + "Sanwer") && PlayerPrefs.GetInt(login + "Sanwer") == answer * PlayerPrefs.GetInt("grain"))
			{
				// Son mot de passe et changé ...
				PlayerPrefs.SetInt (login, password * PlayerPrefs.GetInt("grain"));
				_startServerScript.networkView.RPC("ConnectionFaild", RPCMode.OthersBuffered, 403);
				// ... et il est déconecté
				Network.CloseConnection(player, true);
			}
		}
		// Sinon, si le login ne correspond pas au joueur
		else
		{
			_startServerScript.networkView.RPC("ConnectionFaild", RPCMode.OthersBuffered, 7);
			// Le joueur est rejeté par le serveur
			Network.CloseConnection(player, true);
		}
	}

	// Mettre ici l'écran d'accueil des joueurs
	// RPC de configuration des joueurs
	[RPC]
	void ConfigurePlayerScript (NetworkPlayer player, int cnt, string login)
	{
		// On décrémente le compteur de joueurs
		cnt--;
		// Le joueur devient un joueur connecté
		_playerScripts [cnt]._networkPlayer = player;
		// On active sa visibilité
		_playerScripts [cnt].networkView.enabled = true;
		// Le joueur devient le premier ou le deuxième joueur
		_STATICS._networkPlayer [cnt] = player;
		// Il rentre en jeu
		_STATICS._playersInGame [cnt] = login;
		// Si le joueur est un joueur connecté
		if (Network.player == player) {
			// Sa connexion est initialisée
			_startServerScript.ConnectionOk();
			// On attribue une caméra au joueur
			_playerScripts [cnt].PlayerCamera.SetActive(true);
			// On désactive la caméra du serveur
			_serverCamera.enabled = false;
			// La base de l'autr joueur devient la bae ennemie
			_playerScripts [cnt].EnemyBase.IsTheEnemyBase = true;
			// Le bouton de démarrage de la partie est activé
			_playerScripts [cnt].StartButton.SetActive(true);
			if (cnt == 0)
			{
				_supportInventoryManager.ObjectTag = "PathJ1";
				_badActsInventoryManager.ObjectTag = "PathJ2";
			}
			else
			{
				_supportInventoryManager.ObjectTag = "PathJ2";
				_badActsInventoryManager.ObjectTag = "PathJ1";
			}
			// L'avatar du joueur est correctement affiché
			networkView.RPC("CallRefreshAvatar", RPCMode.Server, login);
			if(cnt == 1){
				// On synchronise l'affichage de l'avatar du joueur
				networkView.RPC("CallRefreshAvatar", RPCMode.Server, _STATICS._playersInGame[0]);
				// On inverse les pannaux du bas pour le joueur 2
				_changeBottomPanelStruct.ReversePanels();
			}
		}

		// Si il s'agit du serveur
		if(Network.isServer)
		{
			// On vérifie si le joueur a accès à la carte qu'il demande
			_errorLevelPanelScript.IsLevelAvaible(player, login);
			// On récupère les avatars disponibles pour le joueur
			_avatarPanel.GetAvatarsAvaible(player, login);
			// Si un joueur est connecté et que la partie a commencé
			if(_connected == 1 && _phasesManager.startgame == true)
				// On pause la partie
				_inGameMenuButtonScript.PauseAll();
			// Si c'est la première partie du joueur
			if(!PlayerPrefs.HasKey(login+"XP"))
				// On initialise son expérience à 0
				PlayerPrefs.SetInt (login+"XP", 0);
		}
	}
	
	//Parce que le rpc du serveur est appelé avant celui du client (donc pas de static.playingame)
	// RPC de récupération des avatars
	[RPC]
	void CallRefreshAvatar(string login)
	{
		// On récupère l'avatar du joueur correspondant au login ...
		_ProfilePanelManager.RefreshAvatar (login);
		// ... et son niveau
		_ProfilePanelManager.GetLevels(login);
	}
	
	// Méthode appellée lors de la déconnexion d'un joueur
	void OnPlayerDisconnected (NetworkPlayer player)
	{
		// On fonction du joueur qui s'est déconnecté, on reset son pseudo dans la base des joueurs connectés
		// On la partie n'a pas encore commencée
		string tmpLogin = (player == _STATICS._networkPlayer[0])? _STATICS._playersInGame[0] : (player == _STATICS._networkPlayer[1])? _STATICS._playersInGame[1] : "";
		if (playersPlaying.Contains (tmpLogin)) {
			playersPlaying.Remove (tmpLogin);
			if (!_phasesManager.startgame) {
				if(_STATICS._playersInGame [0] == tmpLogin)
					_STATICS._playersInGame [0] = null;
				else
					_STATICS._playersInGame [1] = null;
			}
		}

		// Le compteur de joueurs est décrémenté
		_connected--;
		// Si le joueur était pret ...
		if (listPlayersReady.Contains(player))
		{
			// ... on le retire de la lsite des joueurs prets
			listPlayersReady.Remove(player);
		}
		// S'il ne reste plus qu'un seul joueur dans la partie et que la partie a commencé
		if (_connected < 2 && _phasesManager.startgame)
		{
			// On pause le jeu
			_inGameMenuButtonScript.PauseAll();
		}
	}
	
	// Méthode de confirmation de la préparation des joueurs
	public void PlayerReady()
	{
		// On informe le serveur que les joueurs sont prets
		networkView.RPC("PlayersReady", RPCMode.Server, Network.player);
	}
	
	// RPC de confirmation de la préparation des joueurs
	[RPC]
	void PlayersReady(NetworkPlayer player)
	{
		// On ajoute le joueur à la liste des joueurs prets
		listPlayersReady.Add(player);
		// Si les deux joueurs sont prets
		if (listPlayersReady.Count == 2)
		{
			// On informe tout le monde que la partie comence
			networkView.RPC ("StartGamePlayer", RPCMode.AllBuffered);
			// Le jeu peu démarrer
			_inGameMenuButtonScript.UnPauseAll();
		}
	}
	
	// RPC de démarrage de la partie pour le joueur
	[RPC]
	void StartGamePlayer()
	{
		// Si c'est un joueur
		if (Network.isClient)
		{
			// Le serveur n'attend plus de connection
			_startServerScript.WaitConnection = false;
			// Le panel de connexion du serveur est désactivé
			_startServerScript.PanelConnection.gameObject.SetActive (false);
			// Si le joueur est le joueur 1
			if(Network.player == _STATICS._networkPlayer[0])
			{
				// On désactive son bouton de démarrage
				_playerScripts [0].StartButton.SetActive(false);
			}
			// Sinon, si le joueur est le joueur 2
			else
			{
				// On désactive son bouton de démarrage
				_playerScripts [1].StartButton.SetActive(false);
			}
		}
		// La partie peut commencer
		_phasesManager.startgame = true;
	}
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameMenuButtonScript : MonoBehaviour
{
	// Panel du menu ingame
	[SerializeField]
	GameObject inGameMenuPanel;
	// Panel du menu des avatars
	[SerializeField]
	GameObject inGameAvatarPanel;
	// Panel de la pause du jeu
	[SerializeField]
	GameObject PausePanel;
	// NetworksPlayers permettant de savoir quel joueur a demandé la pause en premier et lequel en deuxième
	private NetworkPlayer tmpNetworkPlayer;
	private NetworkPlayer tmpNetworkPlayer2;
	// Nombre de joueurs en pause
	private int playerInPause;
	// Pour connaitre les phases de jeu
	[SerializeField]
	PhasesManager _phasesManager;
	
	void Start ()
	{
		// Aucun joueur en pause au start
		playerInPause = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Si le joueur appuie sur la touche Echap
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			// Le jeu est mis en pause
			InGameMenuPanelEnabled ();
		}
	}
	
	// RPC de vérification de la pause du jeu
	[RPC]
	void CheckPause(NetworkPlayer player)
	{
		// Si le temps est à sa vitesse normale
		if(Time.timeScale == 1f)
		{
			// Le premier joueur à demander la pause et ce joueur
			tmpNetworkPlayer = player;
			// Il y a un joueur en pause
			playerInPause++;
			// On synchronise la pause avec le serveur
			networkView.RPC ("Pause", RPCMode.AllBuffered);
		}
		// Sinon, si le jeu est en pause
		else
		{
			// Si le joueur a mis le jeu en pause en premier ou en deuxième
			if (tmpNetworkPlayer == player || tmpNetworkPlayer2 == player)
			{
				// Il y a un joueur en moins qui a mis la pause
				playerInPause--;
				// Si le premier joueur ayant mis en pause le jeu est ce joueur
				if (tmpNetworkPlayer == player)
				{
					// On applique sa position dans la pause
					tmpNetworkPlayer = new NetworkPlayer();
				}
				// Sinon, si le second joueur ayant mis en pause le jeu est ce joueur
				else
				{
					// On applique sa position dans la pause
					tmpNetworkPlayer2 = new NetworkPlayer();
				}
				// Si aucun joueur n'est en pause
				if (playerInPause == 0)
				{
					// On synchronise la reprise du jeu
					networkView.RPC ("UnPause", RPCMode.AllBuffered);
				}
			} 
			else
			{
				// Sinon, on ajoute un joueur en pause
				tmpNetworkPlayer2 = player;
				playerInPause++;
			}
		}
	}
	
	// RPC de synchronisation de la pause
	[RPC]
	void Pause()
	{
		// Le jeu s'arrete
		Time.timeScale = 0f;
		// Le panel de pause s'active
		PausePanel.SetActive (true);
	}
	
	// RPC de synchronisation de la fin de pause
	[RPC]
	void UnPause()
	{
		// Le jeu reprend
		Time.timeScale = 1f;
		// Le panel de pause se désactive
		PausePanel.SetActive (false);

	}

	// RPC pour forcer la reprise du jeu par tout les joueurs
	[RPC]
	void ForceUnPause()
	{
		// On reset les infos sur les joueurs en pause
		tmpNetworkPlayer = new NetworkPlayer ();
		tmpNetworkPlayer2 = new NetworkPlayer ();
		playerInPause = 0;
		// On enlève les menus de pause
		this.inGameAvatarPanel.gameObject.SetActive(false);
		this.inGameMenuPanel.gameObject.SetActive(false);
	}

	// Méthode de mise en pause du jeu
	public void PauseAll()
	{
		// Synchronisation de la mise en pause du jeu
		networkView.RPC ("Pause", RPCMode.AllBuffered);
	}
	
	// Méthode de reprise du jeu
	public void UnPauseAll()
	{
		// Synchronisation de la reprise du jeu
		networkView.RPC ("UnPause", RPCMode.AllBuffered);
		networkView.RPC ("ForceUnPause", RPCMode.AllBuffered);
	}
	
	// Méthode d'activation et de désativation du menu ingame
	public void InGameMenuPanelEnabled()
	{
		// Inverse l'état du panel en fonction de son etat actuel
		this.inGameMenuPanel.gameObject.SetActive (!this.inGameMenuPanel.activeSelf);

		// Le joueur vérifie si une pause a été demandée et si la partie à débuté
		if(Network.isClient)
		{
			// Si la partie à bien commencée
			if(_phasesManager.startgame == true)
				networkView.RPC ("CheckPause", RPCMode.Server, Network.player);
		}
	}
	
	// Méthode de retour au menu principal
	public void BackToMainMenu()
	{
		// On charge la scène du menu principal
		Application.LoadLevel ("MenuScene");
		// Le joueur se déconnecte du serveur
		Network.Disconnect ();
	}
	
	// Méthode d'activation et de désactivation du panel des avatars
	public void InGameAvatarPanel()
	{
		// Si le panel est désactivé ...
		if (this.inGameAvatarPanel.activeSelf == false)
		{
			// ... on l'active
			this.inGameAvatarPanel.gameObject.SetActive(true);
		}
		// Sinon, si le panel est activé ...
		else
		{
			// ... on le désactive
			this.inGameAvatarPanel.gameObject.SetActive(false);
		}
	}
}
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Security.Cryptography;

public class StartServerScript : MonoBehaviour
{
	// Définition du type de jeu et d'une salle pour le MasterServer
	private const string typeName = "KnockerZGame5";
	private const string gameName = "Salon_de_jeu5";
	// Booléen de vérification de possibilité de rafraichissement de la liste des salles
	private bool isRefreshingHostList = false;
	// Liste des salles de jeu disponibles
	private HostData[] hostList;
	// Salle choisie
	private HostData wantTheHost;
	// Identifiants du joueur stockés
	private string login;
	private int password;
	private int newpassword;
	private int sanswer;
	private int newanswer;
	private string slogin;
	// Identifiants rentrés par le joueur
	[SerializeField] private InputField InputLogin;
	[SerializeField] private InputField InputPassword;
	[SerializeField] private InputField InputNewPassword;
	[SerializeField] private InputField InputNewAnswer;
	[SerializeField] private InputField InputAnswer;
	[SerializeField] private InputField InputALogin;
	// Interface d'identification
	[SerializeField] private GameObject UILogin;
	[SerializeField] private GameObject UIPassword;
	[SerializeField] private GameObject UILogIn;
	[SerializeField] private GameObject UISignIn;
	[SerializeField] private GameObject UIForgotPassword;
	[SerializeField] private GameObject UIConnection;
	// Booléen de controle d'inscription du joueur
	[SerializeField] private bool isInscription;
	// Texte de chargement
	[SerializeField] private Text loadText;
	// Panel de connexion
	[SerializeField] private GameObject panelConnection;
	// Bouton de rafraichissement de la liste des parties
	[SerializeField] private Button refreshButton;
	// Text du bouton de rafraichissement
	[SerializeField] private Text refreshButtonText;
	// Compteur de temps de recherche de serveur
	float cnt = 0f;
	// Booléen de controle d'attende de la connexion
	bool waitConnection = false;
	// Booléen de controle de possibilité de création d'un serveur
	bool canCreateServer;
	// Booléen de controle de connexion
	bool canConnect;
	bool tmpbool;
	// Booléen de controle de connexion pour changer de mot de passe
	public bool connectToChangePassword=false;
	[SerializeField] private InputField InputAddress;
	[SerializeField] private GameObject UIAddress;
	[SerializeField] private GameObject UIOffline;
	[SerializeField] private GameObject UIServer;
	[SerializeField] GameObject UIOfflinePanel;
	private bool wantsToConnectOffline=false;
	
	void Start ()
	{
		Application.runInBackground = true;
		UILogin.SetActive (false);
		UIPassword.SetActive (false);
		UILogIn.SetActive (false);
		UISignIn.SetActive (false);
		UIForgotPassword.SetActive (false);
		UIAddress.SetActive (false);
		UIOffline.SetActive (true);
		UIOfflinePanel.SetActive (false);
		UIServer.SetActive(true);
		loadText.gameObject.SetActive (true);
		refreshButton.gameObject.SetActive (false);
		panelConnection.gameObject.SetActive (true);
		canCreateServer = true;
		MasterServer.ClearHostList();
		canConnect = true;
		tmpbool = true;
		isInscription = false;
		newanswer = 0;
		RefreshHostList();
	}

	public void ChangeTypeOfConnection(){wantsToConnectOffline = !wantsToConnectOffline;tmpbool = true;}
	public void StartAServer()
	{
		if (wantsToConnectOffline)
			StartServerOffline ();
		else
			StartServer ();
	}

	void Update()
	{
		// Si on n'est pas déja connecté
		if (canConnect)
		{
			// Si le serveur se rafraichit et que le nombre de salles lancé sur le MasterServer est supérieur à 0
			if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
			{
				// On ne rafraichit plus
				isRefreshingHostList = false;
				// La liste des hotes devient vide ...
				hostList = null;
				// ... et devient la liste des hotes du MasterServer
				hostList = MasterServer.PollHostList();
			}
			// On incrémente le temps de recherche de serveur
			cnt += Time.deltaTime;
			// Si la recherche dure plus de 4 secondes
			if (cnt > 4 && !wantsToConnectOffline)
			{
				// Si la liste d'hotes est vide
				if (hostList == null)
				{
					// On informe le joueur qu'aucun serveur n'a été trouvé
					loadText.text = "Aucun serveur trouvé";
					// On active le bouton de rafraichissement de la recherche
					refreshButton.gameObject.SetActive(true);
				}
			}
			// Si la liste des hotes n'est pas vide
			if ((hostList != null || wantsToConnectOffline) && tmpbool)
			{
				// On informe le joueur que le serveur est en attente de sa connexion
				loadText.text = "Attente des identifiants";
				// On active tous les composants nécessaires à la connexion du joueur
				UILogin.SetActive (true);
				UIPassword.SetActive (true);
				UILogIn.SetActive (true);
				UISignIn.SetActive (true);
				UIForgotPassword.SetActive(true);
				// Le joueur veut se connecter au premier hote
				if(!wantsToConnectOffline){
					wantTheHost = hostList[0];
				}else{
					UIAddress.SetActive(true);
					UIOfflinePanel.SetActive(true);
					// On active le bouton de rafraichissement de la recherche
					refreshButton.gameObject.SetActive(false);
				}
				tmpbool = false;
			}

			if (wantsToConnectOffline)
			{
				if(InputAddress.text == "")
					UILogIn.SetActive (false);
				else
					UILogIn.SetActive (true);
			}
		}
		// Si le joueur appuie sur Enter
		if(UILogIn.activeSelf && Input.GetKey(KeyCode.Return))
			Connection();
	}

	// Méthode de démarrage du serveur
	void StartServer ()
	{
		// Si il est possible de créer un serveur
		if (canCreateServer)
		{
			// On essaye ...
			try
			{
				// ... d'initialiser un serveur sécurisé sur le MasterServer d'Unity
				Network.InitializeSecurity ();
				Network.InitializeServer(2, 25000, true);
				MasterServer.RegisterHost(typeName, gameName);
				// On ne peut plus créer de serveur à partir de ce script précis
				canCreateServer = false;
				// La panel de connexio est désactivé
				panelConnection.gameObject.SetActive (false);
				// La connexion est établie
				ConnectionOk();
			}
			// Si on rencontre une exception ...
			catch (Exception e)
			{
				// ... on l'affiche
				Debug.LogError (e.Message);
			}
			// Si le serveur n'a pas de grain
			if (!PlayerPrefs.HasKey ("grain"))
			{
				// On lui en génère un
				PlayerPrefs.SetInt("grain", UnityEngine.Random.seed * 5258425 * Convert.ToInt32(UnityEngine.Random.Range(0f,1000f)));
			}
		}
	}

	// Gestion du serveur offline
	void StartServerOffline ()
	{
		try 
		{
			Network.InitializeSecurity ();
			// Connection en locale
			Network.InitializeServer(2, 6600, true);
			// La panel de connexio est désactivé
			panelConnection.gameObject.SetActive (false);
			// La connexion est établie
			ConnectionOk();
			// Si on rencontre une exception ...
		}catch (Exception e)
		{
			// ... on l'affiche
			Debug.LogError (e.Message);
		}
		// Si le serveur n'a pas de grain
		if (!PlayerPrefs.HasKey ("grain"))
		{
			// On lui en génère un
			PlayerPrefs.SetInt("grain", UnityEngine.Random.seed * 5258425 * Convert.ToInt32(UnityEngine.Random.Range(0f,1000f)));
		}
	}
	
	// Méthode de rafraichissement de la lsite d'hote
	private void RefreshHostList()
	{
		// Si la liste n'est pas en train de se rafraichir
		if (!isRefreshingHostList)
		{
			// On désactive le bouton de rafraichissement
			refreshButton.gameObject.SetActive(false);
			// On informe le joueur de la recherche en cours
			loadText.text = "Recherche du serveur de jeu";
			// La liste se rafraichie
			isRefreshingHostList = true;
			// On demande au MasterServer une place au nom typeName
			MasterServer.RequestHostList(typeName);
			// Le compteur d'attente est réinitialisé
			cnt = 0f;
		}
	}
	
	// Méthode de lancement du rafraichissement
	public void LaunchRefresh()
	{
		// La liste d'hotes n'est pas rafraichie
		isRefreshingHostList = false;
		// On lance le rafraichissement
		RefreshHostList ();
	}

	// Méthode de connexion
	public void Connection()
	{
		// Le login est rentré par le joueur ...
		login = InputLogin.text;
		// ... ainsi que le mot de passe
		string _pwd = InputPassword.text;
		// Pour chaque lettre du mot de passe ...
		foreach (char letter in _pwd)
		{
			// ... on essaye ...
			try
			{
				// ... de crypter la lettre selon le login et le grain du joueur
				password = Convert.ToInt32(letter) * PlayerPrefs.GetInt(login + "grain");
			}
			// Sinon, s'il y a une erreur ...
			catch (Exception e)
			{
				// ... on l'affiche
				Debug.Log("Erreur de parsing" + e);
			}
		}
		// Le joueur se connecte au serveur en tant qu'host
		JoinServer (wantTheHost);
	}
	
	// Méthode d'inscription
	public void SignIn()
	{
		// Le login est rentré par le joueur
		login = InputLogin.text;
		
		// Création du grain du joueur
		int keyvalue = 0;
		// Si le login n'existe pas
		if (!PlayerPrefs.HasKey(login + "grain"))
		{
			// On initialise son grain ...
			string _grain = "ndfNEN_JRn";
			// ... dont chaque lettre est placée dans un tableau de caractères
			char[] values = _grain.ToCharArray();
			// Pour chaque caractère du tableau
			foreach (char letter in values)
			{
				// Le caractère est crypté selon sa transformation en int et un nombre aléatoire
				keyvalue =+ Convert.ToInt32(letter) * Convert.ToInt32(UnityEngine.Random.Range(0f,1000f));
			}
			// Le login est ainsi attribué au joueur
			PlayerPrefs.SetInt(login + "grain", UnityEngine.Random.seed * keyvalue);
		}
		
		// Le mot de passe est rentré par le joueur
		string _pwd = InputPassword.text;
		// Pour chaque lettre du mot de passe
		foreach (char letter in _pwd)
		{
			// On essaye ...
			try
			{
				// ... de convertir la lettre en entier et de le multiplier selon le login du joueur accompagné de "grain"
				password = Convert.ToInt32(letter) * PlayerPrefs.GetInt(login + "grain");
			}
			// Sinon, s'il y a une erreur ...
			catch(Exception e)
			{
				// ... on l'affiche
				Debug.Log("Erreur de parsing" + e);
			}
		}
		
		// Le joueur entre sa réponse à la question secrète
		string _newanswer = InputNewAnswer.text;
		// Pour chaque lettre de sa réponse
		foreach (char letter in _newanswer)
		{
			// On essaye ...
			try
			{
				// ... de convertir la lettre en entier et de le multiplier selon le login du joueur accompagné de "grain"
				newanswer = Convert.ToInt32(letter) * PlayerPrefs.GetInt(login + "grain");
			}
			// Sinon, s'il y a une erreur ...
			catch(Exception e)
			{
				// ... on l'affiche
				Debug.Log("Erreur de parsing" + e);
			}
		}
		// Le joueur est désormais inscrit
		isInscription = true;
		// Le joueur se connecte au serveur en tant qu'host
		JoinServer (wantTheHost);
	}
	
	// Méthode d'affiliation au serveur
	private void JoinServer(HostData hostData)
	{
		if (!wantsToConnectOffline) {
			// On essaye ...
			try {
				// ... d'afficher l'état de la connexion au serveur
				loadText.text = "Connexion au serveur de jeu";
				// Le joueur est en attente de connexion au serveur
				waitConnection = true;
				// On connecte le serveur selon l'hote
				Network.Connect (hostData);
			}
		// Sinon, s'il y a une erreur ...
		catch (Exception e) {
				// ... on l'affiche
				waitConnection = false;
				loadText.text = "Erreur de connection au serveur";
				Debug.LogError (e.Message);
			}
		} else {
			try {
				Network.Connect(InputAddress.text, 6600);
			} catch (Exception e) {
				loadText.text = "Erreur de connection au serveur";
				Debug.LogError (e.Message);
			}
		}
	}
	
	// Conflit de nom avec un autre script
	// Méthode de préparation à un changement de mot de passe
	public void PrepareToChangePassword()
	{
		// Le joueur rentre la réponse à la question secrète
		string _sanswer = InputAnswer.text;
		// Pour chaque lettre de la réponse
		foreach (char letter in _sanswer)
		{
			// On essaye ...
			try
			{
				// ... de crypter chaque lettre de cette réponse de la meme manière dont on a crypté sa réponse lors de son inscription
				sanswer = Convert.ToInt32(letter) * PlayerPrefs.GetInt(InputALogin.text + "grain");
			}
			// Sinon, s'il y a une erreur ...
			catch(Exception e)
			{
				// ... on l'affiche
				Debug.Log("Erreur de parsing" + e);
			}
		}
		
		// Le joueur rentre son nouveau mot de passe
		string _newpwd = InputNewPassword.text;
		// Pour chaque lettre du nouveau mot de passe
		foreach (char letter in _newpwd)
		{
			// On essaye ...
			try
			{
				// ... de crypter la lettre
				newpassword = Convert.ToInt32(letter) * PlayerPrefs.GetInt(InputALogin.text + "grain");
			}
			// Sinon, s'il y a une erreur ...
			catch(Exception e)
			{
				// ... on l'affiche
				Debug.Log("Erreur de parsing" + e);
			}
		}
		
		// Le joueur rentre son login
		slogin = InputALogin.text;
		
		// Le joueur s'est connecté afin de changer son mot de passe
		connectToChangePassword = true;
		// Le joueur se connecte au serveur en tant qu'host
		JoinServer (wantTheHost);
	}
	
	// Méthode de validation de la connexion
	public void ConnectionOk()
	{
		// Tous les paramètres de connexion sont réinitialisés une fois le joueur correctement connecté
		canConnect = false;
		UILogin.SetActive (false);
		UIPassword.SetActive (false);
		UILogIn.SetActive (false);
		UISignIn.SetActive (false);
		UIForgotPassword.SetActive (false);
		UIConnection.SetActive (false);
		UIAddress.SetActive (false);
		UIOffline.SetActive (false);
		UIOfflinePanel.SetActive (false);
		UIServer.SetActive(false);
		loadText.gameObject.SetActive (false);
		refreshButton.gameObject.SetActive (false);
		// La connexion n'est plus en cours
		loadText.text = "";
		if (Network.isServer) {
			UnityEngine.Random.seed = UnityEngine.Random.Range (0, 100000000);
		}
		else
			networkView.RPC ("SendSeed", RPCMode.Server);

	}

	[RPC] void SendSeed(){networkView.RPC ("ReceiveSeed", RPCMode.AllBuffered, UnityEngine.Random.seed);}
	[RPC] void ReceiveSeed(int seed){UnityEngine.Random.seed = seed;}
	

	[RPC]
	public void ConnectionFaild(int errorType)
	{
		switch (errorType) {
		case 1:
			loadText.text = "Un login est requis";
			break;
		case 2:
			loadText.text = "Le login existe déja";
			break;
		case 3:
			loadText.text = "Le login n'existe pas";
			break;
		case 4:
			loadText.text = "Le mot de passe est erroné";
			break;
		case 5:
			loadText.text = "Vous etes déja connecté";
			break;
		case 6:
			loadText.text = "La partie est déja lancée avec un autre joueur";
			break;
		case 7:
			loadText.text = "Mauvaise réponse";
			break;
		case 403:
			loadText.text = "Mot de passe changé";
			break;
		default:
			loadText.text = "Erreur inconnue";
			break;
		}
	}
	
	// Accesseurs
	public string Login
	{
		get { return login ;}
		set { login = value; }
	}
	
	public int Password
	{
		get { return password; }
		set { password = value; }
	}
	
	public int NewPassword
	{
		get { return newpassword; }
		set { newpassword = value; }
	}
	
	public int Sanswer
	{
		get { return sanswer; }
		set { sanswer = value; }
	}
	
	public int Newanswer
	{
		get { return newanswer; }
		set { newanswer = value; }
	}
	
	public string inputALogin
	{
		get{ return InputALogin.text; }
	}
	
	public string Slogin
	{
		get{ return slogin; }
		set { slogin = value; }
	}
	
	public bool WaitConnection
	{
		get { return waitConnection; }
		set { waitConnection = value; }
	}

	public bool IsInscription {
		get {
			return isInscription;
		}
		set {
			isInscription = value;
		}
	}

	public GameObject PanelConnection {
		get {
			return panelConnection;
		}
		set {
			panelConnection = value;
		}
	}
}
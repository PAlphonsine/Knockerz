using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Security.Cryptography;

public class StartServerScript : MonoBehaviour {

	//Definition du type de jeu et d'une salle pour le MasterServer
	private const string typeName = "KnockerZGame";
	private const string gameName = "Salon_de_jeu";

	//Pour savoir si on veut rafraichir la liste des salles
	private bool isRefreshingHostList = false;

	//Liste des salles de jeu disponibles
	private HostData[] hostList;

	//La salle choisie
	private HostData wantTheHost;

	//Indentifients stockés
	private string login;
	private int password;

	//Indentifients rentrés par l'utilisateur
	[SerializeField]
	private InputField InputLogin;
	[SerializeField]
	private InputField InputPassword;

	//Interface d'identification
	[SerializeField]
	public GameObject UILogin;
	[SerializeField]
	public GameObject UIPassword;
	[SerializeField]
	public GameObject UIConnection;
	[SerializeField]
	public GameObject UISignIn;

	public bool isInscription;
	
	void Start () {
        Application.runInBackground = true;
		UILogin.SetActive (false);
		UIPassword.SetActive (false);
		UIConnection.SetActive (false);
		UISignIn.SetActive (false);
        /*if (IsServer)
        {
            Network.InitializeSecurity();
            Network.InitializeServer(2, 6600, true);
        }
        else
        {
            Network.Connect("127.0.0.1", 6600);
        }*/
	}

	void OnGUI ()
	{
		if (!(Network.isServer ^ Network.isClient)) {
			GUILayout.BeginVertical ();
			
			GUILayout.BeginHorizontal ();
			
			if (GUILayout.Button ("Search Hosts")) {
				RefreshHostList();
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();

			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++) {
					if (GUILayout.Button(hostList[i].gameName)){
						UILogin.SetActive (true);
						UIPassword.SetActive (true);
						UIConnection.SetActive (true);
						UISignIn.SetActive (true);
						wantTheHost=hostList[i];
					}
				}
			}

			GUILayout.EndHorizontal ();
			
			GUILayout.BeginHorizontal ();
			
			if (GUILayout.Button ("Start Server")) {
				StartServer ();
			}
			
			GUILayout.EndHorizontal ();
			
			GUILayout.EndVertical ();
		}
	}

	void StartServer ()
	{
		try {
			Network.InitializeSecurity ();
			Network.InitializeServer(2, 25000, false);
			MasterServer.RegisterHost(typeName, gameName);
		} catch (Exception e) {
			Debug.LogError (e.Message);
		}
		if (!PlayerPrefs.HasKey ("grain")) {
			PlayerPrefs.SetInt("grain", UnityEngine.Random.seed * 5258425 * Convert.ToInt32(UnityEngine.Random.Range(0f,1000f)));
		}
	}

	void Update()
	{
		if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
		{
			isRefreshingHostList = false;
			hostList = MasterServer.PollHostList();
		}
	}

	private void RefreshHostList()
	{
		if (!isRefreshingHostList)
		{
			isRefreshingHostList = true;
			MasterServer.RequestHostList(typeName);
		}
	}

	public void EnterLogin(){

	}

	public void EnterPassword(){

	}

	public void Connection(){
		login = InputLogin.text;
		string _pwd = InputPassword.text;
		foreach (char letter in _pwd){
			try
			{
				password = Convert.ToInt32(letter) * PlayerPrefs.GetInt("grain");
			}
			catch(Exception e)
			{
				Debug.Log("Erreur de parsing" + e);
			}
		}
		JoinServer (wantTheHost);
	}

	public void SignIn(){

		//Création du grain User
		int keyvalue=0;
		if (!PlayerPrefs.HasKey ("grain")) {
			if (!PlayerPrefs.HasKey ("grain")) {
				string _grain = "ndfNEN_JRn";
				char[] values = _grain.ToCharArray();
				foreach (char letter in values)
				{
					 keyvalue =+ Convert.ToInt32(letter) * Convert.ToInt32(UnityEngine.Random.Range(0f,1000f));
				}
				PlayerPrefs.SetInt("grain", UnityEngine.Random.seed * keyvalue);
			}
		}

		login = InputLogin.text;

		//Cryptage du password
		string _pwd = InputPassword.text;
		foreach (char letter in _pwd){
			try
			{
				password = Convert.ToInt32(letter) * PlayerPrefs.GetInt("grain");
			}
			catch(Exception e)
			{
				Debug.Log("Erreur de parsing" + e);
			}
		}
		isInscription = true;
		JoinServer (wantTheHost);
	}

	private void JoinServer(HostData hostData)
	{
		try {
			Network.Connect(hostData);
		} catch (Exception e) {
			Debug.LogError (e.Message);
		}
	}
	
	void StartClient ()
	{
		try {
			Network.Connect("192.168.1.98", 25000);
		} catch (Exception e) {
			Debug.LogError (e.Message);
		}
	}

	public void ConnectionOk(){
		UILogin.SetActive (false);
		UIPassword.SetActive (false);
		UIConnection.SetActive (false);
		UISignIn.SetActive (false);
	}

	public string Login{
		get {return login;}
		set { login = value;}
	}

	public int Password{
		get {return password;}
		set { password = value;}
	}
}

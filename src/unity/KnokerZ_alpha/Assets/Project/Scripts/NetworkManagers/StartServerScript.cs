using UnityEngine;
using System.Collections;
using System;

public class StartServerScript : MonoBehaviour {

	//public bool StartGame = false;
	
	void Start () {
        Application.runInBackground = true;

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
			
			if (GUILayout.Button ("Start Client")) {
				StartClient ();
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
			Network.InitializeServer(2, 6600, true);
		} catch (Exception e) {
			Debug.LogError (e.Message);
		}
	}
	
	void StartClient ()
	{
		try {
			Network.Connect("127.0.0.1", 6600);
		} catch (Exception e) {
			Debug.LogError (e.Message);
		}
	}

    /*void OnConnectedToServer()
    {
		StartGame = true;
    }*/

    /*void OnPlayerConnected(NetworkPlayer player)
    {
        if (Network.connections.Length == 1) //0 au start
        {
			//Debug.Log("ok");
			networkView.RPC("LaunchGame", RPCMode.AllBuffered);
        }
    }

	[RPC]
	void LaunchGame(){
		StartGame = true; //ok
	}*/
}

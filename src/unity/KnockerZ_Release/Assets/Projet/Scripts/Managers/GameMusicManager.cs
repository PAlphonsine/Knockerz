using UnityEngine;
using System.Collections;

public class GameMusicManager : MonoBehaviour {
	
	[SerializeField] PhasesManager _phaseManager;
	[SerializeField] AudioSource gameMusic;

	void Update () {
		if (_phaseManager.startgame && Time.timeScale == 1) { 
			if(!gameMusic.isPlaying)
			gameMusic.Play ();
		}else 
			gameMusic.Pause ();
	}
}

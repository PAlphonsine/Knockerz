using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
	// Booléen de controle du début de la partie
	public bool IsStarted = false;
	// Joueur
	public NetworkPlayer _networkPlayer;
	// Caméra du joueur
	public GameObject PlayerCamera;
	// Base ennemie
	public BaseScript EnemyBase;
	// Bouton de début de la partie
	public GameObject StartButton;
}
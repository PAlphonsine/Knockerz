using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhasesManager : MonoBehaviour
{
	// Texte du temps
	public Text time;
	// Temps de la phase de réflexion
	public float vtime;
	// Temps de la phase d'action
	public float vtimeA;
	// Booléen de controle du démarrage de la partie
	public bool startgame = false;
	// Booléen de controle de démarrage ou non de la phase d'action
	public bool startAction = false;
	// Booléen de changement de phase (pour indiquer le changement de phase aux scripts abonnés)
	public bool switchPhase = false;
	// Temps d'une phase choisie par le développeur
	float timeAchosen;
	// Temps d'une phase choisie par le développeur
	float timeRchosen;
	
	// Synchronisation des éléments du PhasesManager
	void OnSerializeNetworkView (BitStream stream)
	{
		stream.Serialize(ref  vtime);
		stream.Serialize(ref  vtimeA);
		stream.Serialize(ref  switchPhase);
		stream.Serialize(ref  timeAchosen);
		stream.Serialize(ref  timeRchosen);
	}

	void Start ()
	{
		timeAchosen = vtimeA;
		timeRchosen = vtime;
	}

	void Update ()
	{
		// Si la partie à commencé
		if (startgame == true)
		{
			// Si l'on est en phase de réflexion
			if (!startAction)
			{
				// Si le temps de la phase de réflexion est supérieur à 0.1 ...
				if (vtime > 0.1)
				{
					// ... on le diminue
					vtime -= Time.deltaTime;
					// Le texte du temps est constamment mis à jour selon le temps courant
					time.text = "Reflex : " + ((int)vtime).ToString ();
					// On indique qu'on ne change pas de phase
					switchPhase = false;
				}
				else
				{
					// Sinon, le texte du temps informe le joueur de la fin de la phase de réflexion
					time.text = "TimeOut";
					// La phase d'action peut commencer
					startAction = true;
					// Le temps de la phase d'action devient le temps définit ou par défaut
					vtimeA = timeAchosen;
					// On indique qu'on change de phase
					switchPhase = true;
				}
			}
			// Sinon, si l'on est en phase d'action
			else
			{
				// Si le temps de la phase d'action est supérieur à 0.1 ...
				if (vtimeA > 0.1)
				{
					// ... on le diminue
					vtimeA -= Time.deltaTime;
					//vtime = (int)vtime;
					// Le texte du temps est constamment mis à jour selon le temps courant
					time.text = "Action : " + ((int)vtimeA).ToString ();
					// On indique qu'on ne change pas de phase
					switchPhase = false;
				}
				else
				{
					// Sinon, le texte du temps informe le joueur de la fin de la phase d'action
					time.text = "TimeOut";
					// La phase de réflexion peut commencer
					startAction = false;
					// Le temps de la phase de réflexion devient le temps définit ou par défaut
					vtime = timeRchosen;
					// On indique qu'on change de phase
					switchPhase = true;
				}
			}
		}
	}
}
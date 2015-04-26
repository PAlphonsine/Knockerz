using UnityEngine;
using System.Collections;

public class MachinesPhaseManager : MonoBehaviour 
{
	// Pour detecter les changements de phase
	[SerializeField] PhasesManager _phaseManager;
	// Journal d'évènements
	[SerializeField] EventsNewspaperScript eventsNewspaperScript;
	// Script de planification de la construction de la machine
	[SerializeField] MachinePreparationScript machinePreparationScript;

	// Chance de lancer la vague de boss
	int chanceToLaunchbosses;
	// Facteur d'évolution du % de chance
	int evolChance;
	// Pour savoir si la vague a été lancée
	bool wasLaunched;

	void Start () 
	{
		// Initialisation des variables de chance
		chanceToLaunchbosses = 0;
		evolChance = 1;
		wasLaunched = false;
	}

	void Update ()
	{
		// Si je suis bien le serveur
		if (Network.isServer) 
		{
			// Quand on vient de passer en phase d'action
			if (_phaseManager.switchPhase && _phaseManager.startAction) 
			{
				// Si le résultat su calcul de la probabilité est inférieur au seuil
				if (Random.Range (0, 100) < chanceToLaunchbosses) 
				{
					// On réinitialise les varibales de chance
					chanceToLaunchbosses = 0;
					evolChance = 1;
					// Envoi de l'ordre d'activation des boss
					networkView.RPC ("Launchbosses", RPCMode.AllBuffered);
					wasLaunched = true;
					this.eventsNewspaperScript.BossWave = true;
				}
				else
				{
					// Sinon on incrémente la probabilité lancer la vague de boss au début de la phase d'action
					chanceToLaunchbosses += evolChance + 5 * evolChance;
					// On incrémente son évolution pour que cela soit presque exponentielle
					evolChance++;
				}
			}
			else if (_phaseManager.switchPhase && !_phaseManager.startAction && wasLaunched)
			{ 
				// Envoi de l'ordre de désactivation des boss
				networkView.RPC ("ResetBosses", RPCMode.AllBuffered);
				wasLaunched = false;
			}
		}
	}

	// On lance les boss
	[RPC]
	void Launchbosses()
	{
		// Pour les deux joueurs
		foreach (GameObject b in machinePreparationScript.Bosses)
			b.SetActive (true);
		// On augmente la durée de la phase d'action
		_phaseManager.vtimeA += 30;
	}

	// On reset les boss
	[RPC]
	void ResetBosses()
	{
		// On remet le boss dans son état initial
		StartCoroutine(machinePreparationScript.ZombieScriptBosses[0].Reset());
		StartCoroutine(machinePreparationScript.ZombieScriptBosses[1].Reset());
		// On remet les zombies produits pour les deux boss
		foreach(ZombieScript zbs in machinePreparationScript.BossesScript[0].Zombies)
		{
			// S'ils sont activés
			if (zbs.gameObject.activeSelf)
			{
				// On les reset
				StartCoroutine(zbs.Reset());
			}
		}
		foreach(ZombieScript zbs in machinePreparationScript.BossesScript[1].Zombies)
		{
			if (zbs.gameObject.activeSelf)
			{
				StartCoroutine(zbs.Reset());
			}
		}

		this.machinePreparationScript.RefabricateMachine ();
	}
}

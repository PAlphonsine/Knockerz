using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour 
{
	// Temps de pause voulus entre les parties du tuto
	[SerializeField]
	float timePartie1;
	[SerializeField]
	float timePartie2;
	[SerializeField]
	float timePartie3;
	[SerializeField]
	float timePartie4;
	// Partie scriptée de tuto en cours
	[SerializeField]
	int partie;
	// Float pour calculer le temps
	private float time;

	// Elements necessaire pour les parties du tuto
	// Survivant qui va se présenter la première fois
	[SerializeField]
	TutoSurvivor _survivor;
	[SerializeField]
	GameObject survivor;
	// Zombie qui va se présenter la première fois
	[SerializeField]
	ZombieScript _zombie;
	[SerializeField]
	GameObject zombie;
	[SerializeField]
	// Destination des zombies et survivants
	Transform destination;
	[SerializeField]
	// Panel avant et après la partie zombie
	GameObject PanelZ1;
	[SerializeField]
	GameObject PanelZ2;
	// Panel avant et après la partie survivant
	[SerializeField]
	GameObject PanelS1;
	[SerializeField]
	GameObject PanelS2;
	// Zombies pour la phase plus complexe
	[SerializeField]
	GameObject[] zombies;
	[SerializeField]
	ZombieScript[] _zombies;
	[SerializeField]
	GameObject[] zombies2;
	[SerializeField]
	ZombieScript[] _zombies2;
	// Point de départ pour le survivant
	[SerializeField]
	Transform _start;
	// Panel de fin de phase d'action
	[SerializeField]
	GameObject PanelEndAction;
	// Panel de début de phase de réflexion
	[SerializeField]
	GameObject PanelStartReflexion;
	// Pour activer le bouton soutien en phase de reflexion
	[SerializeField]
	Button ButtonSoutien;
	// Permet de gérer le passage d'un groupe de zombie à un autre dans la partie complexe
	private bool nextStep1;
	private bool nextStep2;
	private bool nextStep3;
	// Permet de bouton de l'interface de définir quand une partie de la fun du tuto est terminée
	public bool finDuTuto1;
	public bool finDuTuto2;
	// Permet de gerer les deux panels de fin de tuto
	[SerializeField]
	GameObject oldPanel;
	[SerializeField]
	GameObject endPanel;
	// Index du niveau de menu
	[SerializeField]
	int menuLevel;
	// Empèche le joueur d'améliorer les tourelles
	[SerializeField]
	GameObject protectTower;
	
	void Start () 
	{
		time = 0;
		// On met en pause le jeu dès le début
		Time.timeScale = 0;
	}

	void Update ()
	{
		// Dans la partie 1 on fait apparaitre le zombie
		if (partie == 1)
		{
			// Si on a bien attendu un certain nombre de minutes
			if (time > timePartie1)
			{
				_zombie.Destination = destination;
				zombie.SetActive(true);
				time = 0;
				partie++;
			}
		}
		// Dans la partie 2 on change de panel et on affiche le survivant
		if (partie == 2)
		{
			if (time > timePartie2)
			{
				PanelZ1.SetActive(false);
				PanelZ2.SetActive(true);
				survivor.SetActive(true);
				// Pause le temps de lire le panel de fin
				Time.timeScale = 0;
				time = 0;
				partie++;
			}
		}
		// Une fois le survivant arrivé à destination, on active le premier groupe de zombie pour la partie complexe
		if (partie == 3) {
			if (_survivor.tutoFini)
			{
				if (!nextStep1)
				{
					PanelS1.SetActive(false);
					PanelS2.SetActive(true);
					survivor.SetActive(false);
					time = 0;
					nextStep1 = true;
				}
				if (time > timePartie3)
				{
					// Activation des zombies
					foreach (var item in zombies)
						item.SetActive(true);
					// Mise en place de la destination
					foreach (var item in _zombies)
						item.Destination = destination;
					_survivor.tutoFini = false;
					// On peut se preparer à passer à l'activation du survivant
					nextStep2 = true;
					time = 0;
				}
			}
			// Activation du survivant une fois un certain temps passé
			if (nextStep2)
			{
				if (time > 2f)
				{
					// Replacement du survivant avant réactivation
					survivor.transform.position = _start.position;
					survivor.SetActive(true);
					// Définition du chemin après activation
					_survivor.agent.SetDestination(destination.position);
					// Fin de l'étape 2, passage à l'étape 3
					nextStep2 = false;
					nextStep3 = true;
					time = 0;
				}
			}
			// Activation du deuxième groupe de zombies
			if (nextStep3){
				if (time > 2.3f)
				{
					foreach (var item in zombies2)
						item.SetActive(true);
					foreach (var item in _zombies2)
						item.Destination = destination;
					time = 0;
					partie++;
				}
			}
		}
		// Transition en phase de reflexion
		if (partie == 4)
		{
			if (time > timePartie4)
			{
				Time.timeScale = 0;
				protectTower.SetActive(false);
				PanelEndAction.SetActive(false);
				PanelStartReflexion.SetActive(true);
				ButtonSoutien.interactable = true;
				time = 0;
				partie++;
			}
		}
		// Affichage des paneaux de fin de tuto
		if (partie == 5)
		{
			if(finDuTuto1 && finDuTuto2)
			{
				oldPanel.SetActive(false);
				endPanel.SetActive(true);
			}
		}
		// Incrémention du temps
		time += Time.deltaTime;
	}

	// Pour dire que la partie 1 de la fin du tuto est terminée
	public void End1()
	{
		finDuTuto1 = true;
	}

	// Pour dire que la partie 2 de la fin du tuto est terminée
	public void End2()
	{
		finDuTuto2 = true;
	}

	// Pour demander à passer à une prochaine étape du tuto
	public void EndTuto()
	{
		partie++;
	}

	// Pour quitter le tuto à la fin du jeu
	public void QuitterTuTo()
	{
		Application.LoadLevel (menuLevel);
	}

	// Mise en pause ou en dépause du jeu
	public void Pause(bool pause)
	{
		if (pause)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
	}
}

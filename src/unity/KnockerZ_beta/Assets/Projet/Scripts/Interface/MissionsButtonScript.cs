using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MissionsButtonScript : MonoBehaviour
{
	// Gestion des phases
	[SerializeField]
	PhasesManager phasesManager;
	// Bouton du menu Missions
	[SerializeField]
	Button missionsButton;
	// Panel du bouton Missions
	[SerializeField]
	GameObject missionsPanel;
	// Type de la mission
	private string missionMode;
	// Tourelles à distance du joueur 1 ...
	[SerializeField]
	GameObject[] turretsDP1;
	// ... et du joueur 2
	[SerializeField]
	GameObject[] turretsDP2;
	// Tourelles corps-à-corps du joueur 1 ...
	[SerializeField]
	GameObject[] turretsHHP1;
	// ... et du joueur 2
	[SerializeField]
	GameObject[] turretsHHP2;
	// Canvas des tourelles du joueur 1 ...
	[SerializeField]
	GameObject[] turretsCanvasesP1;
	// ... et du joueur 2
	[SerializeField]
	GameObject[] turretsCanvasesP2;
	// Couleur d'une tourelle sur laquelle un sabotage est possible
	[SerializeField]
	Material _sabotageMat;
	// Couleur d'une tourelle sur laquelle un sabotage a déjà été plannifié
	[SerializeField]
	Material _antiSabotageMat;
	// Couleur d'origine des tourelles
	[SerializeField]
	Material _originalMat;
	// Tableau de vérification de planification de sabotage sur les canvas des tourelles du joueur 1 ...
	private bool[] plannedTurretsP1 = new bool[8];
	// ... et ceux du joueur 2
	private bool[] plannedTurretsP2 = new bool[8];
	// Compteur de tourelles
	private int turretCount = 0;
	// Compteur de canvas
	private int canvasCount = 0;

	// Use this for initialization
	void Start ()
	{
		this.missionMode = "";
		for (int i = 0; i < 8; i++)
		{
			plannedTurretsP1[i] = false;
			plannedTurretsP2[i] = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Lance la coroutine pour vérifier si le joueur à cliqué
		StartCoroutine (ResetMaterialTurret());
		if (this.phasesManager.startAction == false)
		{
			this.missionsButton.interactable = true;
		}
		else
		{
			this.missionsButton.interactable = false;
		}
	}

	// Fonction coroutine qui permet la verification du mode et le reset de celui-ci en cas de clic
	IEnumerator ResetMaterialTurret()
	{
		// Si le joueur est en mode sabotage et qu'il clique
		if (missionMode == "Sabotage" && Input.GetMouseButton (0))
		{
			RaycastHit hit;
			// On trace un rayon depuis la position du pointeur de la souris à l'écran à la scène
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// Si le joueur clique sur autre chose qu'une tourelle
			if(Physics.Raycast(ray, out hit, 250f) /*&& !hit.transform.CompareTag("Turret")*/)
			{
				// On attend que le canvas de la tourelle soit actif
				yield return new WaitForSeconds(0.2f);
				// Le mode de mission est réinitialisé
				this.missionMode = "";
				// S'il s'agit du joueur 1
				if (Network.player == _STATICS._networkPlayer[0])
				{
					// Chaque tourelle à distance du joueur 2 ...
					foreach(var turret in turretsDP2)
					{
						// ... retrouve sa couleur d'origine
						turret.gameObject.renderer.material = _originalMat;
					}
					
					// Chaque tourelle corps-à-corps du joueur 2 ...
					foreach(var turret in turretsHHP2)
					{
						// ... retrouve sa couleur d'origine
						turret.gameObject.renderer.material = _originalMat;
					}
				}
				// Sinon, s'il s'agit du joueur 2
				else
				{
					// Chaque tourelle à distance du joueur 1 ...
					foreach(var turret in turretsDP1)
					{
						// ... retrouve sa couleur d'origine
						turret.gameObject.renderer.material = _originalMat;
					}
					
					// Chaque tourelle corps-à-corps du joueur 1 ...
					foreach(var turret in turretsHHP1)
					{
						// ... retrouve sa couleur d'origine
						turret.gameObject.renderer.material = _originalMat;
					}
				}
			}
		}
	}

	// Méthode de définition du mode de mission
	public void MissionModeSetting(string mode)
	{
		// Le mode de la mission dépend du bouton sur lequel a cliqué le joueur dans le menu des missions
		this.missionMode = mode;
		// Si le mode choisi est le mode sabotage
		if (missionMode == "Sabotage")
		{
			// S'il s'gait du joueur 1
			if (Network.player == _STATICS._networkPlayer[0])
			{
				// Pour chaque canvas du tableau des canvas du joueur 2
				foreach (GameObject canvas in turretsCanvasesP2)
				{
					// Si une mission a déjà été planifiée via ce canvas, c'est-à-dire que la tourelle ciblée est déjà prévue comme étant la cible d'un sabotage
					if (canvas.GetComponent<SabotageMissionsScript>().AlreadyPlanned == true)
					{
						// On applique le controle au tableau de vérification à la position correspondante au canvas
						plannedTurretsP2[canvasCount] = true;
					}

					// On incrémente le compteur de canvas
					canvasCount++;
				}

				// On réinitialise le compteur de canvas
				canvasCount = 0;

				// Pour chaque tourelle à distance du joueur 2
				foreach (GameObject turret in turretsDP2)
				{
					// Si la tourelle est activée
					if (turret.activeSelf == true)
					{
						// Si le canvas attaché à cette tourelle n'a pas déjà été utilisé pour plannifier une mission sur cette tourelle
						if (plannedTurretsP2[turretCount] == false)
						{
							// Le contour de la tourelle devient rouge, indiquant qu'il est possible de la saboter
							turret.gameObject.renderer.material = _sabotageMat;
						}
						// Sinon, si le canvas attaché à cette tourelle a déjà été utilisé pour plannifier une mission sur cette tourelle
						else
						{
							// Le contour de la tourelle devient vert, indiquant qu'elle est déjà la cible d'un sabotage
							turret.gameObject.renderer.material = _antiSabotageMat;
						}
					}

					// On incrémente le compteur de tourelles
					turretCount++;
				}

				// On réinitialise le compteur de tourelles
				turretCount = 0;
				
				// Pour chaque tourelle corps-à-corps du joueur 2
				foreach (GameObject turret in turretsHHP2)
				{
					// Si la tourelle est activée
					if (turret.activeSelf == true)
					{
						// Si le canvas attaché à cette tourelle n'a pas déjà été utilisé pour plannifier une mission sur cette tourelle
						if (plannedTurretsP2[turretCount] == false)
						{
							// Le contour de la tourelle devient rouge, indiquant qu'il est possible de la saboter
							turret.gameObject.renderer.material = _sabotageMat;
						}
						// Sinon, si le canvas attaché à cette tourelle a déjà été utilisé pour plannifier une mission sur cette tourelle
						else
						{
							// Le contour de la tourelle devient vert, indiquant qu'elle est déjà la cible d'un sabotage
							turret.gameObject.renderer.material = _antiSabotageMat;
						}
					}
					
					// On incrémente le compteur de tourelles
					turretCount++;
				}

				// On réinitialise le compteur de tourelles
				turretCount = 0;
			}
			// Sinon, s'il s'agit du joueur 2
			else
			{
				// Pour chaque canvas du tableau des canvas du joueur 1
				foreach (GameObject canvas in turretsCanvasesP1)
				{
					// Si une mission a déjà été planifiée via ce canvas, c'est-à-dire que la tourelle ciblée est déjà prévue comme étant la cible d'un sabotage
					if (canvas.GetComponent<SabotageMissionsScript>().AlreadyPlanned == true)
					{
						// On applique le controle au tableau de vérification à la position correspondante au canvas
						plannedTurretsP1[canvasCount] = true;
					}

					// On icnrémente le comtpeur de canvas
					canvasCount++;
				}

				// On réinitialise le compteur de canvas
				canvasCount = 0;
				
				// Pour chaque tourelle à distance du joueur 1
				foreach (GameObject turret in turretsDP1)
				{
					// Si la tourelle est activée
					if (turret.activeSelf == true)
					{
						// Si le canvas attaché à cette tourelle n'a pas déjà été utilisé pour plannifier une mission sur cette tourelle
						if (plannedTurretsP1[turretCount] == false)
						{
							// Le contour de la tourelle devient rouge, indiquant qu'il est possible de la saboter
							turret.gameObject.renderer.material = _sabotageMat;
						}
						// Sinon, si le canvas attaché à cette tourelle a déjà été utilisé pour plannifier une mission sur cette tourelle
						else
						{
							// Le contour de la tourelle devient vert, indiquant qu'elle est déjà la cible d'un sabotage
							turret.gameObject.renderer.material = _antiSabotageMat;
						}
					}
					
					// On incrémente le compteur de tourelles
					turretCount++;
				}

				// On réinitialise le compteur de tourelles
				turretCount = 0;
				
				// Pour chaque tourelle corps-à-corps du joueur 1
				foreach (GameObject turret in turretsHHP1)
				{
					// Si la tourelle est activée
					if (turret.activeSelf == true)
					{
						// Si le canvas attaché à cette tourelle n'a pas déjà été utilisé pour plannifier une mission sur cette tourelle
						if (plannedTurretsP1[turretCount] == false)
						{
							// Le contour de la tourelle devient rouge, indiquant qu'il est possible de la saboter
							turret.gameObject.renderer.material = _sabotageMat;
						}
						// Sinon, si le canvas attaché à cette tourelle a déjà été utilisé pour plannifier une mission sur cette tourelle
						else
						{
							// Le contour de la tourelle devient vert, indiquant qu'elle est déjà la cible d'un sabotage
							turret.gameObject.renderer.material = _antiSabotageMat;
						}
					}
					
					// On incrémente le compteur de tourelles
					turretCount++;
				}
				
				// On réinitialise le compteur de tourelles
				turretCount = 0;
			}
		}
	}

	// Méthode d'activation et de désactivation du panel du bouton Missions
	public void MissionsPanelEnabled()
	{
		// Si le panel est désactivé ...
		if(this.missionsPanel.activeSelf == false)
		{
			// ... on l'active
			this.missionsPanel.SetActive(true);
		}
		// Sinon, si le panel est activé ...
		else
		{
			// ... on le désactive
			this.missionsPanel.SetActive(false);
		}
	}
	
	// Accesseurs
	public string MissionMode
	{
		get { return this.missionMode; }
		set { this.missionMode = value; }
	}

	public Material AntiSabotageMat
	{
		get { return this._antiSabotageMat; }
		set { this._antiSabotageMat = value; }
	}

	public int TurretCount
	{
		get { return this.turretCount; }
		set { this.turretCount = value; }
	}

	public int CanvasCount
	{
		get { return this.canvasCount; }
		set { this.canvasCount = value; }
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResolutionsManager : MonoBehaviour
{
	// Panel de séparation au dessus des boutons
	[SerializeField] GameObject separationPanelUp;
	// Panel de séparation en dessous des boutons
	[SerializeField] GameObject separationPanelDown;
	// Tableau de stock des boutons
	[SerializeField] Button[] resolutionButtons;
	// Textes des boutons
	[SerializeField] Text[] resolutionButtonsTexts;
	// Compteur de textes
	private int t;
	// Tableau des positions possibles des boutons
	private Vector3[] buttonsPositions;
	// Compteur de positions
	private int p;
	// Coefficient de positionnement des boutons
	private float positionCoeff;
	// Tableau des boutons à activer selon les résolutions supportées par l'écran du joueur
	private Button[] currentMonitorResolutionsButtons;

	// Use this for initialization
	void Start ()
	{
		// On instancie un tableau de positions selon le nombre de boutons dans le stock
		this.buttonsPositions = new Vector3[25];
		// De la première position de bouton à la dernière en bas de l'ércran, il y a 6 positions possibles
		this.positionCoeff = 6f;
		// Les compteurs de textes et de positions sont initialisés à 0
		this.t = 0;
		this.p = 0;
		// On appelle les méthodes nécessaires à la configuration des résolutions dans le menu des options
		this.ResolutionsAssign ();
		this.ButtonsAssign ();
		this.ButtonsPosition ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Si le dernier bouton de résolution ne dépasse pas la position prévue pour le premier bouton
		if (this.currentMonitorResolutionsButtons[this.currentMonitorResolutionsButtons.Length-1].transform.position.y < this.buttonsPositions[0].y)
		{
			// Si le joueur descend avec sa molette
			if (Input.GetAxis ("Mouse ScrollWheel") <= -0.1f)
			{
				// Chaque bouton ...
				foreach (Button button in this.currentMonitorResolutionsButtons)
				{
					// ... va translater vers le haut
					button.transform.Translate(new Vector3(0f, 20f));
				}
			}
		}

		// Si le premier bouton ne dépasse pas la position prévue pour le dernier bouton
		if (this.buttonsPositions.Length > 4)
		{
			if (this.currentMonitorResolutionsButtons[0].transform.position.y > this.buttonsPositions[1].y)
			{
				// Si le joueur monte avec sa molette
				if (Input.GetAxis ("Mouse ScrollWheel") >= 0.1f)
				{
					// Chaque bouton ...
					foreach (Button button in this.currentMonitorResolutionsButtons)
					{
						// ... va translater vers le bas
						button.transform.Translate(new Vector3(0f, -20f));
					}
				}
			}
		}
		else
		{
			if (this.currentMonitorResolutionsButtons[0].transform.position.y > this.buttonsPositions[this.currentMonitorResolutionsButtons.Length-1].y)
			{
				// Si le joueur monte avec sa molette
				if (Input.GetAxis ("Mouse ScrollWheel") >= 0.1f)
				{
					// Chaque bouton ...
					foreach (Button button in this.currentMonitorResolutionsButtons)
					{
						// ... va translater vers le bas
						button.transform.Translate(new Vector3(0f, -20f));
					}
				}
			}
		}

		// Pour chaque bouton correspondant aux résolutions disponibles sur l'écran du joueur
		foreach (Button button in this.currentMonitorResolutionsButtons)
		{
			// Si le pivot du bouton dépasse le panel de séparation du haut ou celui du bas
			if (button.transform.position.y > this.separationPanelUp.transform.position.y || button.transform.position.y + (Screen.height / 10) < this.separationPanelDown.transform.position.y)
			{
				// On désactive le bouton
				button.gameObject.SetActive(false);
			}
			else
			{
				// Sinon on l'active
				button.gameObject.SetActive(true);
			}
		}
	}

	// Méthode d'assignement des résolutions selon les résolutions supportées par l'écran du joueur
	public void ResolutionsAssign()
	{
		// Pour chaque résolution supportée par l'écran du joueur
		foreach (Resolution resolution in Screen.resolutions)
		{
			// Les textes des boutons de résolutions prennent les résolutions
			this.resolutionButtonsTexts[t].text = (string)(resolution.width + "x" + resolution.height);
			t++;
		}
	}

	// Méthode d'assignement des boutons à afficher
	public void ButtonsAssign()
	{
		// Le tableau de boutons à afficher est de la meme longueur que le tableau de résolutions supportées par l'écran du joueur
		this.currentMonitorResolutionsButtons = new Button[Screen.resolutions.Length];
		// Pour chaque case du tableau de boutons à afficher
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			// On assigne un bouton du stock de boutons
			this.currentMonitorResolutionsButtons[i] = this.resolutionButtons[i];
		}
	}

	// Méthode de positionnement des boutons
	public void ButtonsPosition()
	{
		// Pour chaque case du tableau de positions des boutons
		for (int i = 0; i < this.buttonsPositions.Length; i++)
		{
			// Si le coefficient d'ordonnancement des boutons est supérieur ou égale à 0
			if (this.positionCoeff >= 0)
			{
				// La position est correctement donnée selon la largeur et la hauteur de la résolution en cours
				this.buttonsPositions[i] = new Vector3(Screen.width * 0.15f, (Screen.height * 0.10f) * this.positionCoeff, 0f);
				// Le coefficient est décrémenté
				this.positionCoeff -= 1f;
			}
			else
			{
				// Sinon, la position donnée correspond à la position précédente moins la proportion d'un bouton selon la résolutin en cours
				this.buttonsPositions[i] = new Vector3(Screen.width * 0.15f, this.buttonsPositions[i-1].y - (Screen.height * 0.10f), 0f);
			}
		}

		// Pour chaque bouton à afficher
		foreach (Button button in this.currentMonitorResolutionsButtons)
		{
			// Le bouton prend la position correspondante du tableau de positions
			button.transform.position = this.buttonsPositions[p];
			p++;
		}
	}

	// Accesseurs
	public float PositionCoeff
	{
		get { return this.positionCoeff; }
		set { this.positionCoeff = value; }
	}
}

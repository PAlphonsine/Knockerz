using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GeneratorGraphics : EditorWindow 
{

	// Doit etre éxécuté en play

	// Nom pour le préfab
	string myString = "House";
	// Emplacement ou stocker le préfab
	string path = "Assets/Projet/Prefabs/";
	// Longueur et largeur de la maison
	int max = 4;
	// Hauteur pour la maison
	int kmax = 6;
	// Liste de tous les morceaux de maison
	List<GameObject> things;
	// On crée un parent qui va contenir les objets qui composent la maison
	GameObject container;

	// Ajout d'un menu  dans l'onglet Windows
	[MenuItem ("Window/House Builder")]
	public static void  ShowWindow () 
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(GeneratorGraphics));
	}

	// Interface de la fenètre
	void OnGUI ()
	{
		// Titre du menu
		GUILayout.Label ("Generate house ruins (in play)", EditorStyles.boldLabel);
		// Pour renter le nom que l'on veut
		myString = EditorGUILayout.TextField ("Name of ruins", myString);
		if (GUILayout.Button ("Generate")) {
			// Pour savoir si Unity est bien en play
			if (UnityEditor.EditorApplication.isPlaying == true) {
				GameObject tmpContainer;
				List<GameObject> tmpThings;
				// On crée un parent qui va contenir les objets qui composent la maison
				tmpContainer = new GameObject (myString);
				//Generation d'une liste temporaire pour les morceaux de ruinse en vue de la suppression de leur rigidbody
				tmpThings = new List<GameObject> (max * max * kmax);
				// Pour voir plus précisement la physique
				//Time.timeScale=0.1f;
				// Sur toute la hauteur
				for (int k=0; k<kmax; k++) {
					// Toute la largeur
					for (int i=0; i<=max; i++) {
						// Toute la longueur
						for (int j=0; j<=max; j++) {
							// Pour generer une maison creuse, avec seulement les murs exterieurs construits
							if ((i == 0 && (j == 0 || j == max)) || (i == max && (j == 0 || j == max)) || ((i > 0 && i < max) && (j == 0 || j == max)) || ((j > 0 && j < max) && (i == 0 || i == max))) {
								// Il y a 50% de chance de générer un morceau de mur
								if (Random.Range (0f, 1f) > 0.50f) {
									// On crée l'objet cube
									GameObject thing = GameObject.CreatePrimitive (PrimitiveType.Cube);
									// On le met dans son parent
									thing.transform.parent = tmpContainer.transform;
									// On le positionne
									thing.transform.position = new Vector3 (i, k, j);
									// Rotation aléatoire pour augmenter le risque d'effondrement
									//thing.transform.rotation = new Quaternion(Random.Range(0f,0.000006f), Random.Range(0f,0.000007f), Random.Range(0f,0.000006f), 0);
									// Il y a une chance croissante que le cube puisse tomber
									if (Random.Range (0f, 1f) < 0.3f * k + 0.4f) {
										thing.AddComponent ("Rigidbody");
										// Ajout à la liste
										tmpThings.Add (thing);
									}
								}
							}
						}
					}
				}
				// Création de poutres pour le toit
				for (int p=0; p <= max; p++) {
					GameObject th = GameObject.CreatePrimitive (PrimitiveType.Cube);
					// Positionnement des poutres sur la maison et en décalage les unes par rapport aux autres
					th.transform.position = new Vector3 (max / 2, kmax - 0.5f, p);
					// Déformation des poutres
					th.transform.localScale = new Vector3 (max, 0.2f, 0.2f);
					// Ajout d'un component physique
					th.AddComponent ("Rigidbody");
					// Ajout à la liste
					tmpThings.Add (th);
					// Ajout au parent
					th.transform.parent = tmpContainer.transform;
				}
				// Création de tuiles pour le toit
				// Pour toute la longueur
				for (int b=0; b <= max*2; b++) {
					// Pour toute la largeur
					for (int c=0; c <= max-1; c++) {
						GameObject th = GameObject.CreatePrimitive (PrimitiveType.Cube);
						// Positionnement des tuiles sur la maison et en décalage les unes par rapport aux autres
						th.transform.position = new Vector3 (b * 0.5f, kmax + 0.2f, c * 1.5f + 0.1f);
						// Déformation des tuilles
						th.transform.localScale = new Vector3 (0.5f, 0.2f, 1.5f);
						// Ajout d'un component physique
						th.AddComponent ("Rigidbody");
						// Ajout à la liste
						tmpThings.Add (th);
						// Ajout au parent
						th.transform.parent = tmpContainer.transform;
					}
				}
				// Le modèle généré est selectionné comme celui à utiliser pour l'export
				container = tmpContainer;
				// Les cubes qui composent le modèle sont selectionnés comme ceux à utiliser pour l'export
				things = tmpThings;
			}
		}

		// Prépare l'exportation en figeant la structure
		if (GUILayout.Button ("Prepare")) 
		{
			// Si Unity est bien en play
			if (UnityEditor.EditorApplication.isPlaying == true)
			{
				// Permet de selection le batiment à preparer
				UpdateHouseUsed();
				// Pour tout les objets de la liste
				foreach (GameObject child in things){
					// Suppréssion du componenent rigidbody
					if(child != null)
						Destroy(child.rigidbody);
				}
			}
		}

		// Pour renter le nom que l'on veut
		path = EditorGUILayout.TextField ("Directory for prefab", path);

		// Export en préfab la structure générée
		if (GUILayout.Button ("Export to Prefab")) 
		{
			// Si Unity est bien en play
			if (UnityEditor.EditorApplication.isPlaying == true){
				UpdateHouseUsed();
				if(container != null){
					// Si le répertoire demandé n'existe pas
					if(!Directory.Exists(path))
					{    
						//S'il n'existe pas on le crée
						Directory.CreateDirectory(path);
					}
					// Crée un préfab dans le dossier des assets
					Object prefab = PrefabUtility.CreateEmptyPrefab(path+myString+".prefab");
					// Remplace le prefab avec les ruines générées
					PrefabUtility.ReplacePrefab(container, prefab, ReplacePrefabOptions.ConnectToPrefab);
				}
			}
		}

		// Bouton pour supprimer la maison générée
		if (GUILayout.Button ("Destroy")) 
		{
			// Si Unity est bien en play
			if (UnityEditor.EditorApplication.isPlaying == true){
				// Permet de selection le batiment à supprimer
				UpdateHouseUsed();
				// Détruit les ruines
				if(container != null)
					Destroy(container);
			}
		}
	}

	void UpdateHouseUsed(){
		// Permet de selection le batiment à preparer
		// On récupère le gameObject selectionné
		foreach(GameObject element in Selection.gameObjects)
		{
			// S'il correspond à un modèle de maison
			if (element.name == myString){
				// On en fait notre modèle à exporter
				container = element;
				// On netoie la liste de ses enfants
				things.Clear();
				// Pour chacun des morceaux de notre ruine
				foreach(Transform subElement in element.transform)
				{
					// On les place dans la liste
					things.Add(subElement.gameObject);
				}
				break;
			}
		}
	}
}


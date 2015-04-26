using UnityEngine;
using System.Collections.Generic;

public class Destruction : MonoBehaviour
{
	// Support et référence de system de particule
	[SerializeField] private ParticleSystem SystemModel;
	// Pour récuperer et modifier les états des particules
	private ParticleSystem.Particle[] _particles;
	private Vector3[] _normals;
	// Pour stocker les morceaux de mesh
	private List<GameObject> _pieces;
	// Pour gerer les particules en plusieurs étapes
	private int mode;
	// Origine du mesh à traiter
	private Transform _transform;
	
	public void Start ()
	{
		// Arrete le système de particule du modèle, il n'est plus utilisé
		SystemModel.Stop();
		// On récupère la position de l'objet sur lequel on veut mettre 
		_transform = this.transform;
		// L'état initial est à 0, armé pour le play
		mode = 0;
	}
	
	public void Update ()
	{
		/*if (Input.GetKey (KeyCode.A))
			Play ();*/

		// Si l'animation est bien lancée
		if (mode != 0) 
		{
			// Detruit toutes les particules si la simulation est finie
			if (!SystemModel.IsAlive ())
			{
				// Et permet de repartir en play
				mode = 0;
				// Pour tout les morceaux crées, on les supprimes
				foreach (GameObject obj in _pieces)
					Destroy (obj);	
			}
			else
			{
				// On attend 1 update pour pouvoir recuperer les données des particules
				if (mode == 2)
				{
					// Récupération des tous les paramètres des particules du modèle
					SystemModel.GetParticles (_particles);
					// Pour un nombre de particule correspondant au nombre de morceaux
					for (int i = 0, len = _pieces.Count; i < len; ++i) 
					{
						// On défini comme position de départ pour les particules les positions de nos morceaux
						_particles [i].position = _pieces [i].transform.position;
					}
					// On applique la modification des positions à un nombre de particule corespondant au nombre de morceaux
					SystemModel.SetParticles (_particles, _pieces.Count);
				}
			
				// Au bout de 3 frames, on peut appliquer aux morceaux déplacement des particules
				if (mode > 2) 
				{
					// On récupère les propriétés des particules et leur nombre
					int count = SystemModel.GetParticles (_particles);
					// Pour chaque morceaux
					for (int i = 0, len = _pieces.Count; i < len; ++i) 
					{
						// Si on a encore des particules disponibles pour en prendre les propriétés
						if (i <= count) 
						{
							// En vérifiant que la particule est encore vivante
							if (_particles [i].lifetime > 0.0f)
							{
								// On applique au triangle la position actuelle de la particule
								_pieces [i].transform.position = _particles [i].position;
								// On récupère les normales des morceaux
								Vector3 n = _normals [i];
								// Mise à jour de la rotation grace à l'application du facteur de rotation de la particule à la normale du triangle
								_pieces [i].transform.eulerAngles = n * _particles [i].rotation;
							}
							else 
							{
								// On désactive le rendu pour le triangle si jamais la durée de vie de sa particule référence est arrivée à terme
								_pieces [i].renderer.enabled = false;
							}
						}
					}
				}
				// Attente d'une frame après le play car les particules ont besoin d'updates pour etre mise à jour 
				//et prendre leurs positions et durée de vie initiaux
				if (mode < 3) 
				{
					mode++;
				}
			}
		}
	}

	// Lance la simulation
	public void Play()
	{
		// Si on est bien en état 0
		if (mode == 0) 
		{
			// On récupère le mesh de notre objet
			Mesh objectMesh = GetComponent<MeshFilter> ().mesh;
			// Compte le nombre de triangles
			// Le nombre est divisé par 3 car chacun de 3 points qui compose un triangle est sur une case
			int nbTriangle = objectMesh.triangles.Length / 3;
			// Crée un ensemble de particule de taille à contenir les triangles
			_particles = new ParticleSystem.Particle[nbTriangle];
			// Prépare la liste pour contenir un gameObject par triangle
			_pieces = new List<GameObject> (nbTriangle);
			// Prépare à l'orientation des triangles
			_normals = new Vector3[nbTriangle];
			// On indique que les points des traingles sont reliés dans cet ordre
			int[] tri = new int[]{0,1,2};
			// Un meme nombre de fois que le nombre de triangle
			for (int i = 0; i < nbTriangle; ++i)
			{
				// On crée un nouveau mesh
				Mesh mesh = new Mesh ();
				// On récupère la position des trois vertex qui forment un des triangles
				Vector3 vector1 = objectMesh.vertices[objectMesh.triangles[i * 3]];
				Vector3 vector2 = objectMesh.vertices[objectMesh.triangles[i * 3 + 1]];
				Vector3 vector3 = objectMesh.vertices[objectMesh.triangles[i * 3 + 2]];
				// On récupère la position du centre de ce triangle
				Vector3 center = (vector1 + vector2 + vector3) / 3.0f;
				// On transforme ces coordonnées locales en positions Word en s'adaptant à l'echelle de l'objet qui explose
				vector1 = _transform.TransformPoint (vector1);
				vector2 = _transform.TransformPoint (vector2);
				vector3 = _transform.TransformPoint (vector3);
				center = _transform.TransformPoint (center);
			
				// Pour que chaque vertex soit placé aux coordonnées de l'objet
				Vector3[] verts;
				verts = new Vector3[] {vector1 - center, vector2 - center, vector3 - center};
				// On récupère l'uv présent à chaque vertex du triangle
				Vector2[] uvs = new Vector2[] {objectMesh.uv[objectMesh.triangles[i * 3]],objectMesh.uv [objectMesh.triangles[i * 3 + 1]],objectMesh.uv[objectMesh.triangles[i * 3 + 2]]};
				// On récupère les normals à chaque point
				Vector3[] normals = new Vector3[]{objectMesh.normals[objectMesh.triangles[i * 3]],objectMesh.normals [objectMesh.triangles[i * 3 + 1]],objectMesh.normals[objectMesh.triangles[i * 3 + 2]]};
			
				// Modifie le mesh de l'objet pour qui prenne en compte les propriétés
				mesh.vertices = verts;
				mesh.uv = uvs;
				mesh.normals = normals;
				mesh.triangles = tri;
				// Création d'une normale perpendiculaire pour pouvoir après faire tourner le mesh
				_normals [i] = Vector3.Cross (Vector3.Normalize (normals [0]), Vector3.Normalize (verts [1] - verts [0]));
			
				// Création d'un objet qui va contenir ce mesh
				GameObject obj = new GameObject ("Piece");
				// Ajout des components necessaires
				obj.AddComponent<MeshFilter> ().mesh = mesh;
				obj.AddComponent<MeshRenderer> ().material = GetComponent<MeshRenderer> ().material;
				// Placement du transform exactement à la meme place que le triangle original
				obj.transform.position = center;
				// Incertion de cette position dans le tableau des particules
				_particles [i].position = center;
				//Ajout du mesh à la list
				_pieces.Add (obj);
			}
			// Modifie le particule system de départ
			SystemModel.SetParticles (_particles, nbTriangle);
			// Emet le nombre de particule voulu
			SystemModel.Emit (nbTriangle);
			// Arrete l'émission
			SystemModel.Stop ();
			// Pour les performances, arrete le rendu des meshs
			renderer.enabled = false;
			mode = 1;
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		Play();
	}
}
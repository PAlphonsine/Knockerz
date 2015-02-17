using UnityEditor;
using UnityEngine;
using System.Collections;

public class GeneratorGraphics : EditorWindow {

	string myString = "House";
	int max = 4;
	int kmax = 6;

	// Add menu item named "My Window" to the Window menu
	[MenuItem ("Window/My Window")]
	public static void  ShowWindow () {
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(GeneratorGraphics));
	}
	
	void OnGUI () {
		//EditorGUILayout.BeginVertical ();
		// The actual window code goes here
		GUILayout.Label ("Generation d'un truc", EditorStyles.boldLabel);
		myString = EditorGUILayout.TextField ("Nom de la structure", myString);

		if (GUILayout.Button ("Generation du truc")) {
			GameObject Contener = new GameObject(myString);
			//Time.timeScale=0.1f;
			//GameObject.Instantiate(_prefab, new Vector3(i, 0, j), Quaternion.identity);
			for (int k=0;k<kmax;k++){
				for (int i=0;i<=max;i++){
					for (int j=0;j<=max;j++){
						if((i==0 && (j==0 || j==max)) || (i==max && (j==0 || j==max)) || ((i>0 && i<max) && (j==0 || j==max)) || ((j>0 && j<max) && (i==0 || i==max))){
							if(Random.Range(0f,1f) > 0.40f){
								GameObject thing = GameObject.CreatePrimitive(PrimitiveType.Cube);
								thing.transform.parent = Contener.transform;
								thing.transform.position = new Vector3(i, k, j);
								//thing.transform.rotation = new Quaternion(Random.Range(0f,0.000006f), Random.Range(0f,0.000007f), Random.Range(0f,0.000006f), 0);
								if(Random.Range(0f,1f)< 0.2f*k+0.4f)
									thing.AddComponent("Rigidbody");
								//thing.transform.localScale = new Vector3(Random.Range(0.4f,1f),Random.Range(0.4f,1f),Random.Range(0.4f,1f));
							}
						}
					}
				}
			}
			for(int p=0; p <= 6; p++){
				GameObject th = GameObject.CreatePrimitive(PrimitiveType.Cube);
				th.transform.position = new Vector3(max/2, kmax-0.5f, p);
				th.transform.localScale = new Vector3(max,0.2f,0.2f);
				th.AddComponent("Rigidbody");
				th.transform.parent = Contener.transform;
			}
			for(int b=0; b <= max*2; b++){
				for(int c=0; c <= 4; c++){
					GameObject th = GameObject.CreatePrimitive(PrimitiveType.Cube);
					th.transform.position = new Vector3(b*0.5f, kmax+0.2f, c*1.5f+0.1f);
					th.transform.localScale = new Vector3(0.5f,0.2f,1.5f);
					th.AddComponent("Rigidbody");
					th.transform.parent = Contener.transform;
				}
			}

		}

		if (GUILayout.Button ("Prepare")) {
			//foreach (GameObject item in GameObject.f){
				Rigidbody[] allChildren = GameObject.Find(myString).GetComponentsInChildren<Rigidbody>();
				foreach (Rigidbody child in allChildren) {
					Destroy(child);
				}
			//}
		}

		if (GUILayout.Button ("Export to Prefab")) {
			Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/Projet/"+myString+".prefab");
			PrefabUtility.ReplacePrefab(GameObject.Find(myString), prefab, ReplacePrefabOptions.ConnectToPrefab);
		}
		
		if (GUILayout.Button ("Destroy")) {
			Destroy(GameObject.Find(myString));
		}
		//EditorGUILayout.EndVertical ();
	}
}


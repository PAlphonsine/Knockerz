using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangeInputField : MonoBehaviour {

	// InputField suivant
	[SerializeField] InputField _inputField;
	
	void Update () {
		// Si on appuie sur tab
		if (Input.GetKeyUp (KeyCode.Tab))
			// On change de zone d'inputField
			_inputField.Select ();
	}
}

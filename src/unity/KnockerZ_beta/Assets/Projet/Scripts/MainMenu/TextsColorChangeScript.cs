using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextsColorChangeScript : MonoBehaviour
{
	// Couleur du texte de base
	private Color oldTextColor;

	// Use this for initialization
	void Start ()
	{
		this.oldTextColor = new Color (0.5f, 0.5f, 0.5f);
	}

	// Méthode de changement de couleur d'un texte
	public void ChangeText (Text text)
	{
		// Le texte devient gras ...
		text.fontStyle = FontStyle.Bold;
		// ... et change sa couleur en noir
		text.color = Color.black;
	}

	public void ResetText(Text text)
	{
		// Le texte retrouve son format normal ...
		text.fontStyle = FontStyle.Normal;
		// ... est sa couleur d'origine
		text.color = this.oldTextColor;
	}
}

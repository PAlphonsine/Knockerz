using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VolumeManager : MonoBehaviour
{
	// Controlleur de volume
	[SerializeField] Slider volumeSlider;

	// Use this for initialization
	void Start ()
	{
		// On configure le slider pour que sa valeur soit toujours située entre 0 et 1
		this.volumeSlider.minValue = 0f;
		this.volumeSlider.maxValue = 1f;

		// Si les PlayerPrefs du joueur contiennent une clé 'Volume'
		if (PlayerPrefs.HasKey("Volume"))
		{
			// On récupère la valeur de cette clé grace à une méthode
			this.RetrieveVolume ();
		}
	}

	// Méthode de réglage de volume
	public void SetVolume()
	{
		// Le volume du jeu devient la valeur du slider
		AudioListener.volume = this.volumeSlider.value;
	}

	// Méthode de sauvegarde du volume dans les PlayerPrefs
	public void SaveVolume()
	{
		// On stocke la valeur du volume dans les PlayerPrefs du joueur avec la clé 'Volume'
		PlayerPrefs.SetFloat ("Volume", this.volumeSlider.value);
	}

	// Méthode de récupération du volume dans les PlayerPrefs
	public void RetrieveVolume()
	{
		// On assigne la valeur de la clé 'Volume' au Slider de modification du volume
		this.volumeSlider.value = PlayerPrefs.GetFloat ("Volume");
	}
}

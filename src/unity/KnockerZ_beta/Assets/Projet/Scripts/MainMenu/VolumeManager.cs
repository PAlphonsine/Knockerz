using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VolumeManager : MonoBehaviour
{
	// Controlleur de volume
	[SerializeField]
	Slider volumeSlider;

	// Use this for initialization
	void Start ()
	{
		this.volumeSlider.minValue = 0f;
		this.volumeSlider.maxValue = 1f;
		if (PlayerPrefs.HasKey("Volume"))
		{
			this.RetrieveVolume ();
		}
	}

	// Méthode de réglage de volume
	public void SetVolume()
	{
		// Le volume du jeu devient la valeur du slider
		AudioListener.volume = this.volumeSlider.value;
	}

	public void SaveVolume()
	{
		PlayerPrefs.SetFloat ("Volume", this.volumeSlider.value);
	}

	public void RetrieveVolume()
	{
		this.volumeSlider.value = PlayerPrefs.GetFloat ("Volume");
	}
}

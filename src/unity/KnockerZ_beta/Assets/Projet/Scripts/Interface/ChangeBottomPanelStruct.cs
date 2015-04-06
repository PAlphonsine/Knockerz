using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangeBottomPanelStruct : MonoBehaviour {

	[SerializeField]
	RectTransform[] tabTransformsPanels;
	[SerializeField]
	float[] shifts;

	// Inversion du panel pour le joueur 2
	public void ReversePanels () {
		for(int i = 0; i<tabTransformsPanels.Length ; i++) {
			float tmpMin = tabTransformsPanels[i].anchorMin.x;
			tabTransformsPanels[i].anchorMin = new Vector2 (tabTransformsPanels[i].anchorMin.x-tabTransformsPanels[i].anchorMin.x+shifts[i], tabTransformsPanels[i].anchorMin.y);
			tabTransformsPanels[i].anchorMax = new Vector2 (tabTransformsPanels[i].anchorMax.x-tmpMin+shifts[i], tabTransformsPanels[i].anchorMax.y);
		}
	}
}

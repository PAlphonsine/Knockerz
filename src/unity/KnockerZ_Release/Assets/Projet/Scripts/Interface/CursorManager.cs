using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour
{
	[SerializeField]
	Texture2D handCursorTexture;
	[SerializeField]
	Texture2D pointingCursorTexture;
	private CursorMode cursorMode = CursorMode.Auto;
	private Vector2 hotSpot = new Vector2(11f, 3f);

	public void OnMouseEnter()
	{
		Cursor.SetCursor(pointingCursorTexture, hotSpot, cursorMode);
	}

	public void OnMouseExit()
	{
		Cursor.SetCursor(handCursorTexture, hotSpot, cursorMode);
	}
}

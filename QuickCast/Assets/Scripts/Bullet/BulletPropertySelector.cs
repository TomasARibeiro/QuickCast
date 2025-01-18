using System;
using UnityEngine;

public class BulletPropertySelector : MonoBehaviour
{
	public static event Action<BulletShape> E_ShapeSelected;
	public static event Action<BulletColor> E_ColorSelected;
	public static event Action<BulletOutline> E_OutlineSelected;

	public enum BulletShape { Square, Circle, Triangle }
	public enum BulletColor { Red, Green, Blue }
	public enum BulletOutline { Red, Green, Blue }

	public BulletShape SelectedShape { get; private set; }
	public BulletColor SelectedColor { get; private set; }
	public BulletOutline SelectedOutline { get; private set; }

	private enum PropertySelection { Shape, Color, Outline }
	private PropertySelection _currentSelection = PropertySelection.Shape;

	void Update()
	{
		if (GameManager.Instance.GetGameState() == GameStates.Playing)
		{
			HandleInput();

			if (Input.GetKeyDown(KeyCode.Space))
			{
				Debug.Log($"Bullet Properties: Shape = {SelectedShape}, Color = {SelectedColor}, Outline = {SelectedOutline}");
			}
		}
	}

	private void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.A))
			AssignProperty(0);
		else if (Input.GetKeyDown(KeyCode.S))
			AssignProperty(1);
		else if (Input.GetKeyDown(KeyCode.D))
			AssignProperty(2);
	}

	private void AssignProperty(int value)
	{
		switch (_currentSelection)
		{
			case PropertySelection.Shape:
				SelectedShape = (BulletShape)value;
				E_ShapeSelected?.Invoke(SelectedShape);
				_currentSelection = PropertySelection.Color;
				//Debug.Log($"Shape set to {SelectedShape}");
				break;

			case PropertySelection.Color:
				SelectedColor = (BulletColor)value;
				E_ColorSelected?.Invoke(SelectedColor);
				_currentSelection = PropertySelection.Outline;
				//Debug.Log($"Color set to {SelectedColor}");
				break;

			case PropertySelection.Outline:
				SelectedOutline = (BulletOutline)value;
				E_OutlineSelected?.Invoke(SelectedOutline);
				_currentSelection = PropertySelection.Shape; //loop back to the first property
				//Debug.Log($"Outline set to {SelectedOutline}");
				break;
		}
	}
}

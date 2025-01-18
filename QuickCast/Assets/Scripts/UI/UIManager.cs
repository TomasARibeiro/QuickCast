using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance = null;
	public static event Action E_RestartLevel;

	[Header("Menus and Screens")]
	[SerializeField] private GameObject _pauseMenu;
	[SerializeField] private GameObject _defeatMenu;
	[SerializeField] private GameObject _victoryMenu;
	[Header("General Properties")]
	[SerializeField] private float _basePropertyTextSize = 36;
	[SerializeField] private float _selectedPropertyTextSize = 48;
	[Header("Base Sprites")]
	[SerializeField] private Sprite _squareSprite;
	[SerializeField] private Sprite _circleSprite;
	[SerializeField] private Sprite _triangleSprite;
	[Header("Selected Properties")]
	[SerializeField] private TextMeshProUGUI _shapeText;
	[SerializeField] private TextMeshProUGUI _colorText;
	[SerializeField] private TextMeshProUGUI _outlineText;
	[SerializeField] private Image _shapeImage;
	[SerializeField] private Image _colorImage;
	[SerializeField] private Image _outlineImage;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
	}

	private void OnEnable()
	{
		BulletPropertySelector.E_ShapeSelected += HandleShapeSelected;
		BulletPropertySelector.E_ColorSelected += HandleColorSelected;
		BulletPropertySelector.E_OutlineSelected += HandleOutlineSelected;

		GameManager.E_GamePaused += HandleGamePaused;
		GameManager.E_PlayerWon += HandleGameWon;
		GameManager.E_PlayerDied += HandleGameLost;
	}

	private void OnDisable()
	{
		BulletPropertySelector.E_ShapeSelected -= HandleShapeSelected;
		BulletPropertySelector.E_ColorSelected -= HandleColorSelected;
		BulletPropertySelector.E_OutlineSelected -= HandleOutlineSelected;

		GameManager.E_GamePaused -= HandleGamePaused;
		GameManager.E_PlayerWon -= HandleGameWon;
		GameManager.E_PlayerDied -= HandleGameLost;
	}

	private void Start()
	{
		HighlightText(_shapeText);
	}

	#region EVENT HANDLERS
	private void HandleShapeSelected(BulletPropertySelector.BulletShape shape)
	{
		switch (shape)
		{
			case BulletPropertySelector.BulletShape.Square:
				_shapeImage.sprite = _squareSprite;
				_colorImage.sprite = _squareSprite;
				_outlineImage.sprite = _squareSprite;
				break;
			case BulletPropertySelector.BulletShape.Circle:
				_shapeImage.sprite = _circleSprite;
				_colorImage.sprite = _circleSprite;
				_outlineImage.sprite = _circleSprite;
				break;
			case BulletPropertySelector.BulletShape.Triangle:
				_shapeImage.sprite = _triangleSprite;
				_colorImage.sprite = _triangleSprite;
				_outlineImage.sprite = _triangleSprite;
				break;
		}

		ResetText(_shapeText);
		HighlightText(_colorText);
	}

	private void HandleColorSelected(BulletPropertySelector.BulletColor color)
	{
		switch (color)
		{
			case BulletPropertySelector.BulletColor.Red:
				_colorImage.color = Color.red;
				break;
			case BulletPropertySelector.BulletColor.Blue:
				_colorImage.color = Color.blue;
				break;
			case BulletPropertySelector.BulletColor.Green:
				_colorImage.color = Color.green;
				break;
		}

		ResetText(_colorText);
		HighlightText(_outlineText);
	}

	private void HandleOutlineSelected(BulletPropertySelector.BulletOutline outline)
	{
		switch (outline)
		{
			case BulletPropertySelector.BulletOutline.Red:
				_outlineImage.color = Color.red;
				break;
			case BulletPropertySelector.BulletOutline.Blue:
				_outlineImage.color = Color.blue;
				break;
			case BulletPropertySelector.BulletOutline.Green:
				_outlineImage.color = Color.green;
				break;
		}

		ResetText(_outlineText);
		HighlightText(_shapeText);
	}

	private void HandleGamePaused(bool makePaused)
	{
		if (makePaused)
		{
			_pauseMenu.SetActive(true);
		}
		else
		{
			_pauseMenu.SetActive(false);
		}
	}

	private void HandleGameWon()
	{
		_victoryMenu.SetActive(true);
	}

	private void HandleGameLost()
	{
		_defeatMenu.SetActive(true);
	}
	#endregion

	private void HighlightText(TextMeshProUGUI text)
	{
		text.fontSize = _selectedPropertyTextSize;
		text.color = Color.yellow;
	}

	private void ResetText(TextMeshProUGUI text)
	{
		text.fontSize = _basePropertyTextSize;
		text.color = Color.white;
	}

	public void ResetUI()
	{
		_pauseMenu.SetActive(false);
		_defeatMenu.SetActive(false);
		_victoryMenu.SetActive(false);
	}

	#region BUTTONS
	public void ResumeButton()
	{
		_pauseMenu.SetActive(false);
		GameManager.Instance.ExternalGameStateChange(GameStates.Playing);
	}

	public void QuitButton()
	{
		Application.Quit();
	}

	public void RetryButton()
	{
		
		E_RestartLevel?.Invoke();
	}
	#endregion
}

using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	//events
	public static event Action E_EnemyKilled;
	[Header("Visuals")]
	[SerializeField] private SpriteRenderer _baseSpriteRenderer;
	[SerializeField] private SpriteRenderer _outlineSpriteRenderer;

	[SerializeField] private Sprite _squareSprite;
	[SerializeField] private Sprite _circleSprite;
	[SerializeField] private Sprite _triangleSprite;

	[Header("Stats")]
	[SerializeField] private float _moveSpeed = 3f;

	[Header("Components")]
	private GameObject _player;

	public int Code { get; private set; }

	private void Start()
	{
		//randomize properties
		int randomShape = UnityEngine.Random.Range(1, 4);  //1 to 3
		int randomColor = UnityEngine.Random.Range(1, 4); //1 to 3
		int randomOutline = UnityEngine.Random.Range(1, 4); //1 to 3

		//set appearance and code
		SetAppearance(randomShape, randomColor, randomOutline);

		_player = GameObject.Find("Player");
	}

	private void Update()
	{
		transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _moveSpeed * Time.deltaTime);
	}

	private void SetAppearance(int shape, int color, int outline)
	{
		//set shape
		Sprite shapeSprite = SetShapeSprite(shape);
		_baseSpriteRenderer.sprite = shapeSprite;
		_outlineSpriteRenderer.sprite = shapeSprite;

		//set colors
		_baseSpriteRenderer.color = SetColors(color);
		_outlineSpriteRenderer.color = SetColors(outline);

		//generate the code
		Code = DamageCodeGenerator.GenerateCode(shape, color, outline);
	}

	private Sprite SetShapeSprite(int shape)
	{
		switch (shape)
		{
			case 1: return _squareSprite;
			case 2: return _circleSprite;
			case 3: return _triangleSprite;
			default: return null;
		}
	}

	private Color SetColors(int color)
	{
		switch (color)
		{
			case 1: return Color.red;
			case 2: return Color.green;
			case 3: return Color.blue;
			default: return Color.white;
		}
	}

	public void TakeDamage()
	{
		E_EnemyKilled?.Invoke();
		Destroy(gameObject);
	}
}

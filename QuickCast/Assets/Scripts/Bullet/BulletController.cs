using UnityEngine;

public class BulletController : MonoBehaviour
{
	[SerializeField] private Rigidbody2D _rigidBody;
	[SerializeField] private float _moveSpeed = 10f;
	[SerializeField] private float _lifeTime = 3f;

	[SerializeField] private SpriteRenderer _colorSprite;
	[SerializeField] private SpriteRenderer _outlineSprite;

	[SerializeField] private Sprite _squareSprite;
	[SerializeField] private Sprite _circleSprite;
	[SerializeField] private Sprite _triangleSprite;

	private EnemyController _enemyController;

	public int Code { get; private set; }

	public void InitializeBullet(BulletPropertySelector.BulletShape shape, BulletPropertySelector.BulletColor color, BulletPropertySelector.BulletOutline outline)
	{
		//set the shape
		_colorSprite.sprite = SetSprite(shape);
		_outlineSprite.sprite = SetSprite(shape);

		//set the color
		_colorSprite.color = SetBaseColor(color);

		//set outline color
		_outlineSprite.color = SetOutlineColor(outline);

		Code = DamageCodeGenerator.GenerateCode((int) shape + 1, (int) color + 1, (int) outline + 1);
	}

	private void Start()
	{
		Destroy(gameObject, _lifeTime);
	}

	private void Update()
	{
		MoveBullet();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Enemy"))
		{
			EnemyController enemy = collision.GetComponent<EnemyController>();
			if (Code == enemy.Code)
			{
				enemy.TakeDamage();
				Destroy(gameObject);
			}
		}
		else if (collision.CompareTag("Boss"))
		{
			BossController boss = collision.GetComponent<BossController>();
			if (Code == boss.Code)
			{
				boss.TakeDamage();
				Destroy(gameObject);
			}
		}
	}

	private void MoveBullet()
	{
		transform.position += transform.right * _moveSpeed * Time.deltaTime;
	}

	private Sprite SetSprite(BulletPropertySelector.BulletShape shape)
	{
		switch (shape)
		{
			case BulletPropertySelector.BulletShape.Square:
				return _squareSprite;
			case BulletPropertySelector.BulletShape.Circle:
				return _circleSprite;
			case BulletPropertySelector.BulletShape.Triangle:
				return _triangleSprite;
			default: return null;
		}
	}

	private Color SetBaseColor(BulletPropertySelector.BulletColor color)
	{
		switch (color)
		{
			case BulletPropertySelector.BulletColor.Red:
				return Color.red;
			case BulletPropertySelector.BulletColor.Blue:
				return Color.blue;
			case BulletPropertySelector.BulletColor.Green:
				return Color.green;
			default:
				return Color.white;
		}
	}

	private Color SetOutlineColor(BulletPropertySelector.BulletOutline outline)
	{
		switch (outline)
		{
			case BulletPropertySelector.BulletOutline.Red:
				return Color.red;
			case BulletPropertySelector.BulletOutline.Blue:
				return Color.blue;
			case BulletPropertySelector.BulletOutline.Green:
				return Color.green;
			default:
				return Color.white;
		}
	}
}

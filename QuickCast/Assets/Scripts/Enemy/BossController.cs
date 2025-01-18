//this controller isnt very optimized, eventually i would like to switch it to a state based solution
//currently this is the fastest way for me to make it due to the time constraints of this project

using System;
using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
	//events
	public static event Action E_BossKilled;

	[Header("Boss Stats")]
	[SerializeField] private int _maxHealth = 3;
	[SerializeField] private int _currentHealth;
	[SerializeField] private float _moveSpeed = 3f;
	[SerializeField] private float _enrageDistance = 2f;

	[Header("Visuals")]
	[SerializeField] private SpriteRenderer _baseSpriteRenderer;
	[SerializeField] private SpriteRenderer _outlineSpriteRenderer;
	[SerializeField] private SpriteRenderer _warningOutlineRenderer; //extra outline for the warning phase of dash attack
	[SerializeField] private LineRenderer _lineRenderer; //for showing the dash path

	[SerializeField] private Sprite _squareSprite;
	[SerializeField] private Sprite _circleSprite;
	[SerializeField] private Sprite _triangleSprite;

	[Header("Dash Attack")]
	[SerializeField] private float _dashSpeed = 20f;
	[SerializeField] private float _chargeTime = 1f; //time before the dash happens
	[SerializeField] private float _dashDuration = 0.3f; //duration of the dash
	[SerializeField] private float _cooldownTime = 1f; //time to wait after the dash
	[SerializeField] private float _overshootDistance = 1f; //slight overshoot for the dash target position

	[Header("Split Mechanic")]
	[SerializeField] private GameObject _smallerBossPrefab; //a smaller version of the boss to spawn on death
	[SerializeField] private float _splitOffset = 1f; //the offset when spawning the smaller bosses

	[Header("Conditionals")]
	[SerializeField] private bool _isInRange = false;
	[SerializeField] private bool _isDashing = false;
	[SerializeField] private bool _isOnCooldown = false;
	[SerializeField] private bool _isLastLevel = false;

	private GameObject _player;
	private Vector3 _dashTarget, _dashDirection;

	public int Code { get; private set; }

	private void Start()
	{
		_currentHealth = _maxHealth;
		_lineRenderer.enabled = false;
		_warningOutlineRenderer.enabled = false;

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
		if (_currentHealth == 0)
		{
			SplitAndDestroy();
			return;
		}

		if (!_isDashing && !_isOnCooldown)
		{
			MoveToPlayerRange();
		}
	}

	private void FixedUpdate()
	{
		if (_isDashing)
		{
			DashToTarget(_dashTarget, _dashDirection);
		}
	}

	private void SetAppearance(int shape, int color, int outline)
	{
		//set shape
		Sprite shapeSprite = SetShapeSprite(shape);
		_baseSpriteRenderer.sprite = shapeSprite;
		_outlineSpriteRenderer.sprite = shapeSprite;
		_warningOutlineRenderer.sprite = shapeSprite;

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
		_currentHealth--;

		//change boss appearance
		//randomize properties
		int randomShape = UnityEngine.Random.Range(1, 4);  //1 to 3
		int randomColor = UnityEngine.Random.Range(1, 4); //1 to 3
		int randomOutline = UnityEngine.Random.Range(1, 4); //1 to 3
		SetAppearance(randomShape, randomColor, randomOutline);
	}

	private void MoveToPlayerRange()
	{
		if (!_isInRange)
		{
			if (Vector3.Distance(transform.position, _player.transform.position) > _enrageDistance)
			{
				transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _moveSpeed * Time.deltaTime);
			}
			else
			{
				_isInRange = true;
				StartCoroutine(PrepareDashAttack());
			}
		}
		else
		{
			return;
		}
	}

	private IEnumerator PrepareDashAttack()
	{
		//calculate the direction towards the player
		_dashDirection = (_player.transform.position - transform.position).normalized;

		//calculate the initial target position
		_dashTarget = _player.transform.position;

		//overshoot the target a little
		_dashTarget += _dashDirection * _overshootDistance;

		//show a visual indicator of the dash path
		_lineRenderer.enabled = true;
		_lineRenderer.SetPosition(0, transform.position);
		_lineRenderer.SetPosition(1, _dashTarget);

		//start flickering the warning outline
		StartCoroutine(FlickerWarningOutline());

		//wait for the charge-up time
		yield return new WaitForSeconds(_chargeTime);

		//stop the warning effect
		_warningOutlineRenderer.enabled = false;

		_isDashing = true;
	}

	private void DashToTarget(Vector3 dashTarget, Vector3 dashDirection)
	{
		//perform the dash until the boss reaches the overshot target position
		if (Vector3.Distance(transform.position, dashTarget) > 0.5f)
		{
			transform.position += dashDirection * _dashSpeed * Time.fixedDeltaTime;
		}
		else
		{
			//reset visuals and state after the dash
			_lineRenderer.enabled = false;
			_isDashing = false;
			_isInRange = false;

			//start cooldown before the boss can move again
			StartCoroutine(Cooldown());
		}
	}

	private IEnumerator FlickerWarningOutline()
	{
		_warningOutlineRenderer.enabled = true;
		float flickerInterval = 0.1f;
		float elapsed = 0f;

		while (elapsed < _chargeTime)
		{
			_warningOutlineRenderer.enabled = !_warningOutlineRenderer.enabled;
			yield return new WaitForSeconds(flickerInterval);
			elapsed += flickerInterval;
		}

		_warningOutlineRenderer.enabled = false;
	}

	private IEnumerator Cooldown()
	{
		_isOnCooldown = true;
		yield return new WaitForSeconds(_cooldownTime);
		_isOnCooldown = false;
	}

	private void SplitAndDestroy()
	{
		if (_isLastLevel)
		{
			E_BossKilled?.Invoke();
			Destroy(gameObject);
			return;
		}
		//spawn two smaller bosses at slightly offset positions
		Vector3 offset1 = transform.position + new Vector3(_splitOffset, 0, 0);
		Vector3 offset2 = transform.position + new Vector3(-_splitOffset, 0, 0);

		Instantiate(_smallerBossPrefab, offset1, Quaternion.identity);
		Instantiate(_smallerBossPrefab, offset2, Quaternion.identity);

		E_BossKilled?.Invoke();
		//destroy the current boss
		Destroy(gameObject);
	}
}

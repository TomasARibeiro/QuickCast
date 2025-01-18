using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static event Action E_PlayerShoot;
	public static event Action E_PlayerDied;

	[SerializeField] private int _maxHealth = 3;
	[SerializeField] private int _currentHealth = 3;
	[SerializeField] private float _moveSpeed = 5f;
	private bool _isMoving = false;
	private Vector3 _targetPosition = Vector3.zero;

	private void Start()
	{
		_targetPosition = transform.position;
	}

	private void Update()
	{
		if (GameManager.Instance.GetGameState() == GameStates.Playing)
		{
			if (_currentHealth == 0)
			{
				PlayerDeath();
			}

			RotateToCursor();
			ProcessMouseInput();
			MoveToTarget();
		}
	}

	//handles the input of each mouse button
	private void ProcessMouseInput()
	{
		//move on right click
		if (Input.GetMouseButtonDown(1))
		{
			_targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			_targetPosition.z = transform.position.z;
			_isMoving = true;
		}

		//shoot on left click
		if (Input.GetMouseButtonDown(0))
		{
			TryToShoot();
		}
	}

	//move to where the player clicked
	private void MoveToTarget()
	{
		if (!_isMoving)
		{
			return;
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);

			if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
			{
				_isMoving = false;
			}
		}
	}

	//rotates the player to always look towards the cursor
	private void RotateToCursor()
	{
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector3 lookDirection = mousePosition - transform.position;
		lookDirection.z = 0f;

		float rotationAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
	}

	private void TryToShoot()
	{
		E_PlayerShoot?.Invoke();
	}

	public void TakeDamage()
	{
		_currentHealth--;
	}

	public void PlayerDeath()
	{
		E_PlayerDied?.Invoke();
	}

	public void ResetPlayer()
	{
		_currentHealth = _maxHealth;
		_isMoving = false;
		_targetPosition = transform.position;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
		{
			TakeDamage();
		}
	}
}

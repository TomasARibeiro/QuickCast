using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[Header("Spawner Settings")]
	[SerializeField] private GameObject _enemyPrefab; //the enemy prefab to spawn
	[SerializeField] private GameObject _bossPrefab;

	[SerializeField] private float _spawnInterval = 5f; //time in seconds between spawns
	[SerializeField] private float _spawnDistance = 2f; //distance outside the camera view to spawn enemies

	private float _spawnTimer; //timer to track spawn intervals
	private Camera _mainCamera; //reference to the main camera

	private void OnEnable()
	{
		GameManager.E_EnemyThresholdPassed += HandleEnemyThresholdPassed;
	}

	private void OnDisable()
	{
		GameManager.E_EnemyThresholdPassed -= HandleEnemyThresholdPassed;
	}

	void Start()
	{
		//get the main camera reference
		_mainCamera = Camera.main;
	}

	void Update()
	{
		if (_mainCamera == null)
		{
			_mainCamera = Camera.main;
			if (_mainCamera == null)
			{
				Debug.LogWarning("Main camera not found. Spawn logic will not work.");
				return;
			}
		}

		//update the timer
		_spawnTimer += Time.deltaTime;

		//if the timer exceeds the spawn interval, spawn an enemy
		if (_spawnTimer >= _spawnInterval)
		{
			SpawnEnemyOutsideView();
			_spawnTimer = 0f;  //reset the timer
		}


	}

	void SpawnEnemyOutsideView()
	{
		if (_mainCamera == null)
		{
			Debug.LogWarning("Main camera is missing. Cannot calculate spawn bounds.");
			return;
		}

		//get the screen bounds in world coordinates
		Vector3 screenBounds = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _mainCamera.transform.position.z));

		//decide on a random side for spawning: top, bottom, left, or right
		int side = UnityEngine.Random.Range(0, 4);
		Vector2 spawnPosition = Vector2.zero;

		switch (side)
		{
			case 0: //top side
				spawnPosition = new Vector2(UnityEngine.Random.Range(-screenBounds.x, screenBounds.x), screenBounds.y + _spawnDistance);
				break;
			case 1: //bottom side
				spawnPosition = new Vector2(UnityEngine.Random.Range(-screenBounds.x, screenBounds.x), -screenBounds.y - _spawnDistance);
				break;
			case 2: //left side
				spawnPosition = new Vector2(-screenBounds.x - _spawnDistance, UnityEngine.Random.Range(-screenBounds.y, screenBounds.y));
				break;
			case 3: //right side
				spawnPosition = new Vector2(screenBounds.x + _spawnDistance, UnityEngine.Random.Range(-screenBounds.y, screenBounds.y));
				break;
		}

		//instantiate the enemy at the calculated position
		Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
	}

	private void HandleEnemyThresholdPassed()
	{
		//get the screen bounds in world coordinates
		Vector3 screenBounds = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _mainCamera.transform.position.z));

		//decide on a random side for spawning: top, bottom, left, or right
		int side = UnityEngine.Random.Range(0, 4);
		Vector2 spawnPosition = Vector2.zero;

		switch (side)
		{
			case 0: //top side
				spawnPosition = new Vector2(UnityEngine.Random.Range(-screenBounds.x, screenBounds.x), screenBounds.y + _spawnDistance);
				break;
			case 1: //bottom side
				spawnPosition = new Vector2(UnityEngine.Random.Range(-screenBounds.x, screenBounds.x), -screenBounds.y - _spawnDistance);
				break;
			case 2: //left side
				spawnPosition = new Vector2(-screenBounds.x - _spawnDistance, UnityEngine.Random.Range(-screenBounds.y, screenBounds.y));
				break;
			case 3: //right side
				spawnPosition = new Vector2(screenBounds.x + _spawnDistance, UnityEngine.Random.Range(-screenBounds.y, screenBounds.y));
				break;
		}

		//instantiate the enemy at the calculated position
		Instantiate(_bossPrefab, spawnPosition, Quaternion.identity);
		Destroy(this);
	}

	//for debugging the spawn area in the Scene view
	void OnDrawGizmos()
	{
		if (_mainCamera == null) return;

		Gizmos.color = Color.yellow;
		Vector3 screenBounds = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _mainCamera.transform.position.z));

		//draw rectangles to represent the spawn boundaries
		Gizmos.DrawWireCube(Vector3.up * (screenBounds.y + _spawnDistance), new Vector3(screenBounds.x * 2, 0.1f, 0));
		Gizmos.DrawWireCube(Vector3.down * (screenBounds.y + _spawnDistance), new Vector3(screenBounds.x * 2, 0.1f, 0));
		Gizmos.DrawWireCube(Vector3.left * (screenBounds.x + _spawnDistance), new Vector3(0.1f, screenBounds.y * 2, 0));
		Gizmos.DrawWireCube(Vector3.right * (screenBounds.x + _spawnDistance), new Vector3(0.1f, screenBounds.y * 2, 0));
	}
}

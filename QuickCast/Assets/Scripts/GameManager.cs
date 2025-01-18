using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates {Playing, Idle, Paused, Over};
public class GameManager : MonoBehaviour
{
	public static GameManager Instance = null;

	public GameStates _currentState = GameStates.Playing;
	public GameStates _previousState;

	//events
	public static event Action E_EnemyThresholdPassed;
	public static event Action<bool> E_GamePaused;
	public static event Action E_PlayerWon;
	public static event Action E_PlayerDied;

	[Header("Game Stats")]
	[SerializeField] private float _gameTime = 0f;
	[SerializeField] private int _necessaryEnemyKills = 10;
	[SerializeField] private int _enemiesKilled = 0;
	[SerializeField] private int _necessaryBossKills = 7;
	[SerializeField] private int _bossesKilled = 0;

	private void OnEnable()
	{
		EnemyController.E_EnemyKilled += HandleEnemyKilled;
		BossController.E_BossKilled += HandleBossKilled;
		PlayerController.E_PlayerDied += HandlePlayerDied;

		UIManager.E_RestartLevel += HandleRestartLevel;
	}

	private void OnDisable()
	{
		EnemyController.E_EnemyKilled -= HandleEnemyKilled;
		BossController.E_BossKilled -= HandleBossKilled;
		PlayerController.E_PlayerDied -= HandlePlayerDied;

		UIManager.E_RestartLevel -= HandleRestartLevel;
	}

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

	private void Update()
	{
		switch (_currentState)
		{
			case GameStates.Playing:
				GameTimer();
				Time.timeScale = 1.0f;

				if (_enemiesKilled == _necessaryEnemyKills)
				{
					E_EnemyThresholdPassed?.Invoke();
				}

				if (_bossesKilled == _necessaryBossKills)
				{
					E_PlayerWon?.Invoke();
					ChangeGameState(GameStates.Over);
				}

				if (Input.GetKeyDown(KeyCode.Escape))
				{
					E_GamePaused?.Invoke(true);
					ChangeGameState(GameStates.Paused);
				}
				break;
			case GameStates.Paused:
				Time.timeScale = 0f;
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					E_GamePaused?.Invoke(false);
					ChangeGameState(GameStates.Playing);
				}
				break;
			case GameStates.Idle:
				break;
			case GameStates.Over:
				Time.timeScale = 0f;
				break;
		}
	}

	private void GameTimer()
	{
		_gameTime += Time.deltaTime;
	}

	private void ChangeGameState(GameStates newState)
	{
		Debug.Log("changing state from " + _currentState.ToString() + " to " + newState.ToString());
		_previousState = _currentState;
		_currentState = newState;
	}

	private void ResetStats()
	{
		_gameTime = 0f;
		_enemiesKilled = 0;
		_bossesKilled = 0;
		
		ChangeGameState(GameStates.Playing);

		PlayerController player = FindAnyObjectByType<PlayerController>();
		player?.ResetPlayer();

		UIManager.Instance?.ResetUI();
	}

	#region EXTERNAL
	public GameStates GetGameState()
	{
		return _currentState;
	}

	public void ExternalGameStateChange(GameStates newState)
	{
		ChangeGameState(newState);
	}
	#endregion

	#region EVENT HANDLERS
	private void HandleEnemyKilled()
	{
		_enemiesKilled++;
	}

	private void HandleBossKilled()
	{
		_bossesKilled++;
	}

	private void HandlePlayerDied()
	{
		Debug.Log("hes dead");
		Time.timeScale = 0f;
		E_PlayerDied?.Invoke();
		ChangeGameState(GameStates.Over);
	}

	private void HandleRestartLevel()
	{
		ResetStats();
		SceneManager.LoadScene(1);
	}

	private IEnumerator DelayResetCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		ResetStats();
	}
	#endregion
}

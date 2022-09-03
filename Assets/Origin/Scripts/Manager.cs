using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public enum GameState { Running, Paused, Over }


// Manager is surves two purposes (Singleton Class)
// 1. working as proxy class -> makes object interactions easier
// 2. managing game state, audio, events and game parameters like player speed and maximum straight line length
public class Manager : MonoBehaviour
{
	[Header("Parameters")]
	[SerializeField] private float playerSpeed;
	[SerializeField] private float diamondsPercentage;
	[SerializeField] private float stepGenerateTime;
	[SerializeField] private float stepDestroyTime;
	[SerializeField] private float changingColorTime;
	[SerializeField] private int straightLineLength;
	[SerializeField] private int diamondMaxScoreAmmount;

	[Header("Objects")]
	[SerializeField] private Transform player;
	[SerializeField] private Transform cameraCenter;
	[SerializeField] private Material stepsMaterial;
	[SerializeField] private AudioSource swapSound;
	[SerializeField] private AudioSource collectSound;

	[Header("GUI")]
	[SerializeField] private RectTransform RunningPanel;
	[SerializeField] private RectTransform PausedPanel;
	[SerializeField] private RectTransform OverPanel;
	[SerializeField] private TextMeshProUGUI[] playerScoreTexts;
	[SerializeField] private TextMeshProUGUI[] playerBestScoreTexts;
	[SerializeField] private TextMeshProUGUI[] gamesPlayedTexts;


	public static Manager Instance { get; private set; }
	public GameState State { get; private set; }
	public float PlayerSpeed => playerSpeed;
	public float DiamondsPercentage => diamondsPercentage;
	public float StepGenerateTime => stepGenerateTime;
	public float StepDestroyTime => stepDestroyTime;
	public int StraightLineLength => straightLineLength;
	public int DiamondMaxScoreAmmount => diamondMaxScoreAmmount;
	public Transform Player => player;
	public Transform CameraCenter => cameraCenter;

	public int PlayerScore { get; private set; }
	public int PlayerBestScore
	{
		get
		{
			return PlayerPrefs.GetInt("BEST_SCORE");
		}
		private set
		{
			PlayerPrefs.SetInt("BEST_SCORE", value);
			PlayerPrefs.Save();
		}
	}
	public int GamesPlayed
	{
		get
		{
			return PlayerPrefs.GetInt("PLAYED_GAMES");
		}
		private set
		{
			PlayerPrefs.SetInt("PLAYED_GAMES", value);
			PlayerPrefs.Save();
		}
	}

	public void Awake()
	{
		if (Instance == null) Instance = this;
		else throw new System.Exception("There is alredy a Manager in the scene!");
		State = GameState.Paused;

		SetPlayerBestScoreText();
		SetGamesPlayedText();
	}

	private void Update()
	{
		// Updating Game State
		switch (State)
		{
			case GameState.Paused:
				if (!Input.GetMouseButtonDown(0)) break;
				State = GameState.Running;
				StartCoroutine(ChangeStepsColor());
				break;
			case GameState.Running:
				var ray = new Ray(player.position, Vector3.down);
				var isPlayerFalling = !Physics.Raycast(ray, 10.0f, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore);
				State = isPlayerFalling ? GameState.Over : GameState.Running;
				if (State == GameState.Over) OnGameOver();
				break;
		}

		// tracking GUI by game state
		SetPanales();
	}

	#region EVENTS_SECTION
	public void OnDiamondCollected(int ammount)
	{
		PlayerScore += ammount;
		SetPlayerScoreText();
		collectSound.Play();
	}

	public void OnPlayerSwap()
	{
		swapSound.Play();
	}

	private void OnGameOver()
	{
		GamesPlayed++;
		if (PlayerBestScore < PlayerScore) PlayerBestScore = PlayerScore;
		SetPlayerBestScoreText();
	}
	#endregion EVENTS_SECTION

	#region GUI_CONTROLLERS
	private void SetPlayerScoreText()
	{
		foreach (var _ in playerScoreTexts)
			_.text = $"{PlayerScore}";
	}

	private void SetPlayerBestScoreText()
	{
		foreach (var _ in playerBestScoreTexts)
			_.text = $"{PlayerBestScore}";
	}

	private void SetGamesPlayedText()
	{
		foreach (var _ in gamesPlayedTexts)
			_.text = $"{GamesPlayed}";
	}

	private void SetPanales()
	{
		RunningPanel.gameObject.SetActive(State == GameState.Running);
		PausedPanel.gameObject.SetActive(State == GameState.Paused);
		OverPanel.gameObject.SetActive(State == GameState.Over);
	}
	
	public void Retry() => SceneManager.LoadScene(0);

	#endregion GUI_CONTROLLERS

	private IEnumerator ChangeStepsColor()
	{
		while (true)
		{
			yield return new WaitForSeconds(changingColorTime);
			var currentColor = stepsMaterial.GetColor("_Color");
			var nextColor = Random.ColorHSV(0.1f, 0.9f, 0.3f, 0.7f, 1.0f, 1.0f);
			var time = 0.0f;
			while (time < 1.0f)
			{
				var resultColor = Color.Lerp(currentColor, nextColor, time);
				stepsMaterial.SetColor("_Color", resultColor);
				time += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}
	}
}

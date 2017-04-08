using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Data;
using Mono.Data.SqliteClient;

public class HUD : MonoBehaviour {

	public static HUD instance;

	public static bool _countingDown;
	private float totalCountdownTime;

	private static Text LeaderboardTitle;
	private static Text Leaderboard_Usernames;
	private static Text Leaderboard_Scores;
	private static Text Countdown;
	private static Text Gameover;
	private static Text PracticeText;
	private static Text GestureInfo;

	private static Button _mainMenuButton;
	private static Button _playAgainButton;
	private static Button _viewLeaderboardButton;

	private static string construct;
	private static IDbConnection dbConnection;
	private static IDbCommand dbCommand;
	private static IDataReader dataReader;

	private static int[,] TopThreeLeaderboardData;

	void Awake()
	{
		instance = this;
		TopThreeLeaderboardData = new int[3,3];
		construct = "URI=file:" + Application.dataPath + "\\TrainingData.db";
		LeaderboardTitle = GameObject.Find ("Leaderboard Title").GetComponent<Text> ();
		Leaderboard_Usernames = GameObject.Find ("Leaderboard_Usernames").GetComponent<Text> ();
		Leaderboard_Scores = GameObject.Find ("Leaderboard_Scores").GetComponent<Text> ();
		Countdown = GameObject.Find ("Countdown").GetComponent<Text> ();
		Gameover = GameObject.Find ("Game Over").GetComponent<Text> ();
		PracticeText = GameObject.Find ("PracticeText").GetComponent<Text> ();
		GestureInfo = GameObject.Find ("GestureInfo").GetComponent<Text> ();
		_mainMenuButton = GameObject.Find ("MainMenuButton").GetComponent<Button> ();
		_playAgainButton = GameObject.Find ("PlayAgainButton").GetComponent<Button> ();
		_viewLeaderboardButton = GameObject.Find ("ViewLeaderboardButton").GetComponent<Button> ();
	}

	public static void Start () {
		Countdown.text = "";
	}

	void Update () {
		if (_countingDown) 
			CountdownFromThree();

		PullLeaderboardData ();

		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame)
			ClearPracticeText ();

		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.preGame || GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame) {
			LeaderboardTitle.text = "Top 3 Scores";
			UpdateTopThreeLeaderboard ();
			HideGameOver ();
		} else if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.gameOver) {
			GameOver ();
		} else {
			LeaderboardTitle.text = "";
			Leaderboard_Usernames.text = "";
			Leaderboard_Scores.text = "";
			HideGameOver ();
		}
	}

	public void UpdateGestureText(string gesture) {
		GestureInfo.text = gesture;
	}

	static void HideButton(Button button) {
		button.image.enabled = false;
		button.GetComponentInChildren<Text> ().text = "";
		button.onClick.RemoveAllListeners ();
	}

	static void ShowButton(Button button) {
		button.image.enabled = true;

		if (button == _mainMenuButton)
			button.GetComponentInChildren<Text> ().text = "Main Menu";
		else if (button == _playAgainButton)
			button.GetComponentInChildren<Text> ().text = "Play Again";
		else if (button == _viewLeaderboardButton)
			button.GetComponentInChildren<Text> ().text = "View Leaderboard";
	}

	public void StartCountdown() {
		totalCountdownTime = Time.time + 3;
		_countingDown = true;
	}

	void CountdownFromThree() {
		int timeLeft = Convert.ToInt32(totalCountdownTime) - Convert.ToInt32(Time.time);

		if (timeLeft > 0)
			Countdown.text = timeLeft.ToString ();
		else if (timeLeft == 0) {
			Countdown.text = "GO";
			Scoreboard.instance.StartTimer ();
			GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.mainGame;
		} else {
			Countdown.text = "";
			_countingDown = false;
		}
	}

	public static void DisplayPreGameText() {
		PracticeText.text = "3 BALL PRACTICE";
		PracticeText.fontStyle = FontStyle.Normal;
	}

	private void ClearPracticeText () {
		PracticeText.text = "";
	}

	public void GameOver() {
		GamePlay.GameIsPlayable = false;
		GestureInfo.text = "";
		LeaderboardTitle.text = "";
		Leaderboard_Usernames.text = "";
		Leaderboard_Scores.text = "";
		Gameover.text = "GAME OVER";
		ShowButton (_mainMenuButton);
		ShowButton (_playAgainButton);
		ShowButton (_viewLeaderboardButton);
		_playAgainButton.onClick.AddListener (RestartGame);
		_mainMenuButton.onClick.AddListener (BackToMainMenu);
		_viewLeaderboardButton.onClick.AddListener (ViewLeaderboard);
	}

	public void BackToMainMenu() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.startScreen;
	}

	public void RestartGame() {
		if (!GamePlay.restartActivated) { 
			//prevents the reset function being fired more than once, preventing the addition of many new leaderboard rows
			GamePlay.restartActivated = true; //set to true here, but reset back to false when the main game is loaded
			Scoreboard.instance.Reset ();
			GamePlay.SetUpPregame ();
		}
	}

	public void ViewLeaderboard() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.leaderboard;
	}

	public static void HideGameOver() {
		Gameover.text = "";
		HideButton (_mainMenuButton);
		HideButton (_playAgainButton);
		HideButton (_viewLeaderboardButton);
	}

	private static void PullLeaderboardData() {

		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();

		dbCommand.CommandText = "SELECT * FROM Leaderboard ORDER BY Score DESC LIMIT 3";
		dataReader = dbCommand.ExecuteReader ();

		int count = 0;

		while (dataReader.Read ()) {
			TopThreeLeaderboardData [count, 0] = (int)(dataReader ["ID"]);
			TopThreeLeaderboardData [count, 1] = (int)(dataReader ["UserID"]);
			TopThreeLeaderboardData [count, 2] = (int)(dataReader ["Score"]);
			count++;
		}

		dbConnection.Close ();
	}

	private static void UpdateTopThreeLeaderboard() {

		string playerOne = "";
		string playerTwo = "";
		string playerThree = "";

		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();

		dbCommand.CommandText = "SELECT Username FROM Users WHERE ID = '" + TopThreeLeaderboardData [0, 1] + "';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			playerOne = dataReader ["Username"].ToString ();
		}

		dbCommand.CommandText = "SELECT Username FROM Users WHERE ID = '" + TopThreeLeaderboardData [1, 1] + "';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			playerTwo = dataReader ["Username"].ToString ();
		}

		dbCommand.CommandText = "SELECT Username FROM Users WHERE ID = '" + TopThreeLeaderboardData [2, 1] + "';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			playerThree = dataReader ["Username"].ToString ();
		}

		dbConnection.Close ();


		Leaderboard_Usernames.text = playerOne + "\n" + playerTwo + "\n" + playerThree;
		Leaderboard_Scores.text = TopThreeLeaderboardData [0, 2] + "\n" + TopThreeLeaderboardData [1, 2] + "\n" + TopThreeLeaderboardData [2, 2];
	}
		
}

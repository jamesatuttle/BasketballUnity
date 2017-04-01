using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Data;
using Mono.Data.SqliteClient;

public class HUD : MonoBehaviour {

	static float endTime;
	public static bool countdown;
	public static int Down = 3;
	public static bool setTimer = true;
	public int timer;

	private static Text LeaderboardTitle;
	private static Text Leaderboard_Usernames;
	private static Text Leaderboard_Scores;
	private static Text Countdown;
	private static Text Gameover;
	private static Text PracticeText;

	private static string construct;
	private static IDbConnection dbConnection;
	private static IDbCommand dbCommand;
	private static IDataReader dataReader;

	private static int[,] TopThreeLeaderboardData;

	void Awake()
	{
		TopThreeLeaderboardData = new int[3,3];
		construct = "URI=file:" + Application.dataPath + "\\TrainingData.db";
		timer = 55;
		LeaderboardTitle = GameObject.Find ("Leaderboard Title").GetComponent<Text> ();
		Leaderboard_Usernames = GameObject.Find ("Leaderboard_Usernames").GetComponent<Text> ();
		Leaderboard_Scores = GameObject.Find ("Leaderboard_Scores").GetComponent<Text> ();
		Countdown = GameObject.Find ("Countdown").GetComponent<Text> ();
		Gameover = GameObject.Find ("Game Over").GetComponent<Text> ();
		PracticeText = GameObject.Find ("PracticeText").GetComponent<Text> ();
	}

	public static void Start () {
		Countdown.text = "";
		Gameover.text = "";
	}

	void Update () {
		if (countdown) CountdownFromThree();

		PullLeaderboardData ();
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.preGame || GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame) {
			LeaderboardTitle.text = "Top 3 Scores";
			UpdateTopThreeLeaderboard ();
		} else {
			LeaderboardTitle.text = "";
			Leaderboard_Usernames.text = "";
			Leaderboard_Scores.text = "";
		}
		
	}

	void CountdownFromThree()
	{
		PracticeText.text = "";

		timer--;
		//Debug.Log (timer);
		if (timer == 44) {
			Countdown.text = "3";
		}
		else if (timer == 33)
			Countdown.text = "2";
		else if (timer == 22)
			Countdown.text = "1";
		else if (timer == 11)
			Countdown.text = "GO";
		else if (timer < 0) {
			Countdown.text = "";
			countdown = false;
			GamePlay.GameIsPlayable = true;
		}
	}

	public static void DisplayPreGameText()
	{
		PracticeText.text = "3 BALL PRACTICE";
		PracticeText.fontStyle = FontStyle.Normal;
	}

	public static void GameOver()
	{
		Gameover.text = "GAME OVER";
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

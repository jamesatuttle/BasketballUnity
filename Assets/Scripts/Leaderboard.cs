using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.SqliteClient;

public class Leaderboard : MonoBehaviour {

	private static string construct;
	private static IDbConnection dbConnection;
	private static IDbCommand dbCommand;
	private static IDataReader dataReader;

	private static TextMesh Leaderboard_Usernames1;
	private static TextMesh Leaderboard_Usernames2;
	private static TextMesh Leaderboard_Usernames3;
	private static TextMesh Leaderboard_Scores1;
	private static TextMesh Leaderboard_Scores2;
	private static TextMesh Leaderboard_Scores3;
	private static TextMesh Leaderboard_Numbers1;
	private static TextMesh Leaderboard_Numbers2;
	private static TextMesh Leaderboard_Numbers3;

	Button BackButton;

	private static string[,] LeaderboardDataString;

	void Awake () {
		LeaderboardDataString = new string[66,2];
		construct = "URI=file:" + Application.dataPath + "\\TrainingData.db";
		Leaderboard_Numbers1 = GameObject.Find ("Numbers").GetComponent<TextMesh> ();
		Leaderboard_Numbers2 = GameObject.Find ("Numbers2").GetComponent<TextMesh> ();
		Leaderboard_Numbers3 = GameObject.Find ("Numbers3").GetComponent<TextMesh> ();
		Leaderboard_Usernames1 = GameObject.Find ("Usernames").GetComponent<TextMesh> ();
		Leaderboard_Usernames2 = GameObject.Find ("Usernames2").GetComponent<TextMesh> ();
		Leaderboard_Usernames3 = GameObject.Find ("Usernames3").GetComponent<TextMesh> ();
		Leaderboard_Scores1 = GameObject.Find ("Scores").GetComponent<TextMesh> ();
		Leaderboard_Scores2 = GameObject.Find ("Scores2").GetComponent<TextMesh> ();
		Leaderboard_Scores3 = GameObject.Find ("Scores3").GetComponent<TextMesh> ();
		BackButton = GameObject.Find ("BackButton").GetComponent<Button> ();
	}

	void Start () {
		Leaderboard_Numbers1.color = Color.yellow;
		Leaderboard_Numbers2.color = Color.yellow;
		Leaderboard_Numbers3.color = Color.yellow;
	}

	// Update is called once per frame
	void Update () {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.leaderboard) {
			BackButton.image.enabled = true;
			BackButton.GetComponentInChildren<Text> ().text = "BACK";
			BackButton.onClick.AddListener (StartScreen.instance.SetUpStartScreen);
		}

		PullLeaderboardData ();
		UpdateFullLeaderboard ();
	}

	private static void PullLeaderboardData() {

		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();

		dbCommand.CommandText = "SELECT Users.Username, Leaderboard.Score FROM Leaderboard INNER JOIN Users ON Users.ID = Leaderboard.UserID ORDER BY Leaderboard.Score DESC LIMIT 66";
		dataReader = dbCommand.ExecuteReader ();

		int count = 0;

		while (dataReader.Read ()) {
			LeaderboardDataString [count, 0] = dataReader ["Username"].ToString ();
			LeaderboardDataString [count, 1] = dataReader ["Score"].ToString ();
			count++;
		}

		dbConnection.Close ();
	}

	private static void UpdateFullLeaderboard() {

		Leaderboard_Usernames1.text = "";
		Leaderboard_Scores1.text = "";
		Leaderboard_Usernames2.text = "";
		Leaderboard_Scores2.text = "";
		Leaderboard_Usernames3.text = "";
		Leaderboard_Scores3.text = "";

		for (int i = 0; i < 21; i++) {
			if (LeaderboardDataString [i, 0] != "") {
				Leaderboard_Usernames1.text += (LeaderboardDataString [i, 0] + "\n");
				Leaderboard_Scores1.text += (LeaderboardDataString [i, 1] + "\n");
			}
			else
				Leaderboard_Usernames1.text += "\n";
		}
		if (LeaderboardDataString [21, 1] != "") {
			Leaderboard_Usernames1.text += LeaderboardDataString [21, 0];
			Leaderboard_Scores1.text += LeaderboardDataString [21, 1];
		}

		for (int i = 22; i < 43; i++) {
			if (LeaderboardDataString [i, 0] != "") {
				Leaderboard_Usernames2.text += (LeaderboardDataString [i, 0] + "\n");
				Leaderboard_Scores2.text += (LeaderboardDataString [i, 1] + "\n");
			}
			else
				Leaderboard_Usernames2.text += "\n";
		}
		if (LeaderboardDataString [43, 1] != "") {
			Leaderboard_Usernames2.text += LeaderboardDataString [43, 0];
			Leaderboard_Scores2.text += LeaderboardDataString [43, 1];
		}

		for (int i = 44; i < 63; i++) {
			if (LeaderboardDataString [i, 0] != "") {
				Leaderboard_Usernames3.text += (LeaderboardDataString [i, 0] + "\n");
				Leaderboard_Scores3.text += (LeaderboardDataString [i, 1] + "\n");
			}
			else
				Leaderboard_Usernames3.text += "\n";
		}
		if (LeaderboardDataString [63, 1] != "") {
			Leaderboard_Usernames3.text += LeaderboardDataString [63, 0];
			Leaderboard_Scores3.text += LeaderboardDataString [63, 1];
		}
	}
}

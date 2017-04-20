using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.SqliteClient;

public class Leaderboard : MonoBehaviour {

	public static Leaderboard instance; //create an instance of the class, so that other classes can access instances of its methods

	//variables to connect to the database
	private static string construct; //the location of the database file
	private static IDbConnection dbConnection; //the connection to the database
	private static IDbCommand dbCommand; //used to perform SQL statements and commands
	private static IDataReader dataReader; //used to read information from the database

	//stores the TextMesh objects of the username, score and number columns on the leaderboard
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

	private static string[,] LeaderboardDataString; //a 2d array which stores the leaderboard data

	//Awake is called at the start of the game, used to initialise variables
	void Awake () {
		instance = this; //create an instance of the leaderboard class

		LeaderboardDataString = new string[66,2]; //intialise the size of the leaderboard data array
		construct = "URI=file:" + Application.dataPath + "\\TrainingData.db"; //sets the database file path

		//initialises the game objects
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

	//called after Awake, at the start of the game
	void Start () {
		Leaderboard_Numbers1.color = Color.yellow;
		Leaderboard_Numbers2.color = Color.yellow;
		Leaderboard_Numbers3.color = Color.yellow;
	}

	/*
	* Update is called once per frame
	* If the leaderboard is the active screen then set it up
	* Display the back button image and text and activate it
	*/
	void Update () {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.leaderboard) {
			BackButton.image.enabled = true;
			BackButton.GetComponentInChildren<Text> ().text = "BACK";
			BackButton.onClick.AddListener (StartScreen.instance.SetUpStartScreen);
		}

		PullLeaderboardData (); //pull the data from the leaderboard table
		UpdateFullLeaderboard (); //update the leaderboard with the pulled data
	}

	/*
	* Pull the data from the leaderboard table in the database
	* uses a LIMIT 66 to pull the highest 66 scores
	*/
	private static void PullLeaderboardData() {

		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();

		dbCommand.CommandText = "SELECT Users.Username, Leaderboard.Score FROM Leaderboard INNER JOIN Users ON Users.ID = Leaderboard.UserID ORDER BY Leaderboard.Score DESC LIMIT 66";
		dataReader = dbCommand.ExecuteReader ();

		int count = 0;

		//pull the data from each row in the leaderboard table, storing the username and score
		while (dataReader.Read ()) {
			LeaderboardDataString [count, 0] = dataReader ["Username"].ToString ();
			LeaderboardDataString [count, 1] = dataReader ["Score"].ToString ();
			count++;
		}

		dbConnection.Close ();
	}

	/*
	* Update the physical leaderboard in the game environment with the leaderboard data
	*/
	private static void UpdateFullLeaderboard() {

		Leaderboard_Usernames1.text = "";
		Leaderboard_Scores1.text = "";
		Leaderboard_Usernames2.text = "";
		Leaderboard_Scores2.text = "";
		Leaderboard_Usernames3.text = "";
		Leaderboard_Scores3.text = "";

		//as there are three columns of the leaderboard, they need to be filled one by one

		//column one
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

		//column two
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

		//column three
		for (int i = 44; i < 65; i++) {
			if (LeaderboardDataString [i, 0] != "") {
				Leaderboard_Usernames3.text += (LeaderboardDataString [i, 0] + "\n");
				Leaderboard_Scores3.text += (LeaderboardDataString [i, 1] + "\n");
			}
			else
				Leaderboard_Usernames3.text += "\n";
		}
		if (LeaderboardDataString [65, 1] != "") {
			Leaderboard_Usernames3.text += LeaderboardDataString [63, 0];
			Leaderboard_Scores3.text += LeaderboardDataString [63, 1];
		}
	}

	/*
	* update the leaderboard with the score when the user scores a hoop
	*/
	public void UpdateLeaderboardWithScore(int score) {

		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();

		dbCommand.CommandText = "UPDATE Leaderboard SET Score = " + score + " WHERE ID = " + Login.LeaderboardID + ";";
		dbCommand.ExecuteNonQuery ();

		dbConnection.Close ();
	}
}

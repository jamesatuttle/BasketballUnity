using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Data;
using Mono.Data.SqliteClient;

public class HUD : MonoBehaviour {

	public static HUD instance; //create an instance of the class, so that other classes can access instances of its methods

	public static bool _countingDown; //a bool variable which stores if the countdown to the main game is active
	private float totalCountdownTime; //the total countdown time left

	//Text Variables to store the related HUD text object
	private static Text LeaderboardTitle;
	private static Text Leaderboard_Usernames;
	private static Text Leaderboard_Scores;
	private static Text Countdown;
	private static Text Gameover;
	private static Text PracticeText;
	private static Text GestureInfo;

	//Button variables to store the related HUD buttons
	private static Button _mainMenuButton;
	private static Button _playAgainButton;
	private static Button _viewLeaderboardButton;

	//variables to connect to the database
	private static string construct; //the location of the database file
	private static IDbConnection dbConnection; //the connection to the database
	private static IDbCommand dbCommand; //used to perform SQL statements and commands
	private static IDataReader dataReader; //used to read information from the database

	private static int[,] TopThreeLeaderboardData; //a 2d array used to store the top three leaderboard data

	//Awake is called at the start of the game, used to initialise variables
	void Awake()
	{
		instance = this; //used to initialise the instance of HUD
		TopThreeLeaderboardData = new int[3,3]; //sets the 2d array to have three rows of three integers
		construct = "URI=file:" + Application.dataPath + "\\TrainingData.db"; //sets the location of the database

		//initialise the game components to their variables so that they can be accessed
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

	//called after Awake, at the start of the game
	public static void Start () {
		Countdown.text = ""; //clears the countdown
	}

	/*
	* Update is called once per frame
	* Used to control the HUD functionality
	*/
	void Update () {
		if (_countingDown) //if the countdown boolean is true...
			CountdownFromThree();

		PullLeaderboardData (); //update the top three leaderboard with the database data

		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame) //if the main game is active...
			ClearPracticeText ();

		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.preGame || GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame) { //if the main game or pregame is active...
			LeaderboardTitle.text = "Top 3 Scores"; //set the leaderboard title
			UpdateTopThreeLeaderboard (); //display the top three scores on the leaderboard
			HideGameOver (); //hide the 'game over' text
		} else if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.gameOver) { //if game over is active
			GameOver ();
		} else { //if none of these screens are active, clear the leaderboard and game over text
			LeaderboardTitle.text = "";
			Leaderboard_Usernames.text = "";
			Leaderboard_Scores.text = "";
			HideGameOver ();
		}
	}

	/*
	* Used to update the HUD text, showing which gesture is being performed
	* Called by the ANN
	*/
	public void UpdateGestureText(string gesture) {
		GestureInfo.text = gesture;
	}

	/*
	* Generic method to hide whichever button is passed in
	*/
	static void HideButton(Button button) {
		button.image.enabled = false; //disables the button image
		button.GetComponentInChildren<Text> ().text = ""; //clears the button text
		button.onClick.RemoveAllListeners (); //deactivates the button functionality
	}

	/*
	* Generic method to show whichever button is passed in
	*/
	static void ShowButton(Button button) {
		button.image.enabled = true; //enable the button which has been passed in

		//set the corresponding button text
		if (button == _mainMenuButton)
			button.GetComponentInChildren<Text> ().text = "Main Menu";
		else if (button == _playAgainButton)
			button.GetComponentInChildren<Text> ().text = "Play Again";
		else if (button == _viewLeaderboardButton)
			button.GetComponentInChildren<Text> ().text = "View Leaderboard";
	}

	/*
	* Method to start the countdown from 3 (used between the pregame and main game)
	*/
	public void StartCountdown() {
		totalCountdownTime = Time.time + 3; //sets the totalCountdownTime to the seconds since the game started plus 3 seconds
		_countingDown = true; //set the variable to true to allow the countdown
	}

	/*
	* A method to display the countdown (3, 2, 1, GO) between the preGame and the mainGame
	*/
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

	/*
	*	A method to display the pre game text
	* It sets ths PracticeText text to display '3 BALL PRACTICE'
	*/
	public static void DisplayPreGameText() {
		PracticeText.text = "3 BALL PRACTICE";
		PracticeText.fontStyle = FontStyle.Normal;
	}

	/*
	*	A method to display the pre game text
	* It sets ths PracticeText text to display '3 BALL PRACTICE'
	*/
	private void ClearPracticeText () {
		PracticeText.text = "";
	}

	/*
	* A method to display the GAME OVER text on the HUD
	* It displays the text and the play again, main menu and leaderboard buttons
	* and hides the rest of the HUD
	*/
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

	/*
	* Called by the BackToMainMenu button
	* sets the active screen to the start screen
	*/
	public void BackToMainMenu() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.startScreen;
	}

	/*
	* A function to restart the game
	* Is called by the RestartGame button on the GAME OVER screen
	* Due to the button firing the event many times once pressed, the boolean value
	* was added to prevent this.
	*/
	public void RestartGame() {
		if (!GamePlay.restartActivated) {
			//prevents the reset function being fired more than once, preventing the addition of many new leaderboard rows
			GamePlay.restartActivated = true; //set to true here, but reset back to false when the main game is loaded
			Scoreboard.instance.Reset ();
			GamePlay.SetUpPregame ();
		}
	}

	/*
	* sets the current screen to the leaderboard screen
	*/
	public void ViewLeaderboard() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.leaderboard;
	}

	/*
	* Hides the GAME OVER text and buttons
	*/
	public static void HideGameOver() {
		Gameover.text = "";
		HideButton (_mainMenuButton);
		HideButton (_playAgainButton);
		HideButton (_viewLeaderboardButton);
	}

	/*
	* Pulls the top three score rows from the Leaderboard table in the database
	*/
	private static void PullLeaderboardData() {

		dbConnection = new SqliteConnection (construct); //connects to the database in the specified file location
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();

		dbCommand.CommandText = "SELECT * FROM Leaderboard ORDER BY Score DESC LIMIT 3";
		dataReader = dbCommand.ExecuteReader ();

		int count = 0;

		//fills the 2d array with the top three leaderboard data
		while (dataReader.Read ()) {
			TopThreeLeaderboardData [count, 0] = (int)(dataReader ["ID"]);
			TopThreeLeaderboardData [count, 1] = (int)(dataReader ["UserID"]);
			TopThreeLeaderboardData [count, 2] = (int)(dataReader ["Score"]);
			count++;
		}

		dbConnection.Close ();
	}

	/*
	* Updates the top three scores on the mini HUD leaderboard
	*/
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
			playerOne = dataReader ["Username"].ToString (); //stores the username of the top player in the string value
		}

		dbCommand.CommandText = "SELECT Username FROM Users WHERE ID = '" + TopThreeLeaderboardData [1, 1] + "';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			playerTwo = dataReader ["Username"].ToString (); //stores the username of the second player in the string value
		}

		dbCommand.CommandText = "SELECT Username FROM Users WHERE ID = '" + TopThreeLeaderboardData [2, 1] + "';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			playerThree = dataReader ["Username"].ToString (); //stores the username of the third player in the string value
		}

		dbConnection.Close ();


		Leaderboard_Usernames.text = playerOne + "\n" + playerTwo + "\n" + playerThree; //constructs the username list
		Leaderboard_Scores.text = TopThreeLeaderboardData [0, 2] + "\n" + TopThreeLeaderboardData [1, 2] + "\n" + TopThreeLeaderboardData [2, 2];
		//constructs the corresponding scores for the users
	}

}

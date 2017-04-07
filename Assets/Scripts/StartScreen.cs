using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour {

	public static StartScreen instance;

	Text StartGame;
	Text ViewScoreboard;
	Text ViewLeaderboard;
	Text HowToPlayText;
	Text LoginText;
	Text LoginHelp;
	Text LeaderboardTitle;
	Text Leaderboard_Usernames;
	Text Leaderboard_Scores;

	private Text _kinectConnectedText;

	InputField UserName;

	Button NextButton;
	Button BackButton;
	Button YesButton;
	Button NoButton;

	void Awake() {
		instance = this;
		GamePlay.GameIsPlayable = false;
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.startScreen;

		StartGame = GameObject.Find ("Start Game").GetComponent<Text> ();
		ViewScoreboard = GameObject.Find ("View Scoreboard").GetComponent<Text> ();
		ViewLeaderboard = GameObject.Find ("View Leaderboard").GetComponent<Text> ();
		HowToPlayText = GameObject.Find ("How to play").GetComponent<Text> ();
		LoginText = GameObject.Find ("LoginText").GetComponent<Text> ();
		LoginHelp = GameObject.Find ("LoginHelp").GetComponent<Text> ();
		UserName = GameObject.Find ("UsernameInput").GetComponent<InputField> ();
		NextButton = GameObject.Find ("NextButton").GetComponent<Button> ();
		BackButton = GameObject.Find ("BackButton").GetComponent<Button> ();
		YesButton = GameObject.Find ("YesButton").GetComponent<Button> ();
		NoButton = GameObject.Find ("NoButton").GetComponent<Button> ();
		LeaderboardTitle = GameObject.Find ("Leaderboard Title").GetComponent<Text> ();
		Leaderboard_Usernames = GameObject.Find ("Leaderboard_Usernames").GetComponent<Text> ();
		Leaderboard_Scores = GameObject.Find ("Leaderboard_Scores").GetComponent<Text> ();
		_kinectConnectedText = GameObject.Find ("KinectConnected").GetComponent<Text> ();
	}

	void Start () {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.startScreen;
		SetTextActive (StartGame);
	}
		
	// Update is called once per frame
	void Update () {

		try {
			if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.startScreen) {

				SetUpStartScreen ();

				if (Input.GetKeyDown(KeyCode.DownArrow)) {
					if (OptionSelected(StartGame))
						SetTextActive(ViewScoreboard);
					else if (OptionSelected(ViewScoreboard))
						SetTextActive(ViewLeaderboard);
					else if (OptionSelected(ViewLeaderboard))
						SetTextActive(HowToPlayText);
					else if (OptionSelected(HowToPlayText))
						SetTextActive(StartGame);
				}
				if (Input.GetKeyDown(KeyCode.UpArrow)) {
					if (OptionSelected(StartGame))
						SetTextActive(HowToPlayText);
					else if (OptionSelected(HowToPlayText))
						SetTextActive(ViewLeaderboard);
					else if (OptionSelected(ViewLeaderboard))
						SetTextActive(ViewScoreboard);
					else if (OptionSelected(ViewScoreboard))
						SetTextActive(StartGame);
				}

				if (Input.GetKeyDown (KeyCode.Return)) {
					if (OptionSelected (StartGame)) {

						if (_kinectConnectedText.text == "A Kinect Sensor is needed to play")
							_kinectConnectedText.text = "Please attach Kinect and restart";
						else if (_kinectConnectedText.text == "Please attach Kinect and restart") {
							_kinectConnectedText.color = Color.white;
						} else {
							GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.playedBeforeQuestion;
							_kinectConnectedText.text = "";
						}
						
					} else if (OptionSelected(ViewScoreboard))
						GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.scoreboard;
					else if (OptionSelected(ViewLeaderboard))
						GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.leaderboard;
					else if (OptionSelected(HowToPlayText))
						GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.howToPlay;
				}
			} else {
				ClearStartScreen ();
			}
		} catch (Exception e) {
			print ("Exception Start Screen: " + e.Message);
			_kinectConnectedText.text = "A problem seems to have occured.  Please restart";
		}
	}

	void HideButton(Button button) {
		button.image.enabled = false;
		button.GetComponentInChildren<Text> ().text = "";
	}

	void HideInputField(InputField inputField) {
		inputField.image.enabled = false;
		inputField.placeholder.enabled = false;
		inputField.text = "";
	}

	void HideText(Text text) {
		text.text = "";
	}

	bool OptionSelected(Text text) {
		if (text.color == Color.yellow)
			return true;
		else
			return false;
	}

	public void SetUpStartScreen() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.startScreen;
		StartGame.text = "START GAME";
		ViewScoreboard.text = "SCOREBOARD";
		ViewLeaderboard.text = "LEADERBOARD";
		HowToPlayText.text = "HOW TO PLAY";

		HideInputField (UserName);
		HideButton (NextButton);
		HideButton (BackButton);
		HideButton (YesButton);
		HideButton (NoButton);
		HideText (LoginText);
		HideText (LoginHelp);
		HideText (LeaderboardTitle);
		HideText (Leaderboard_Usernames);
		HideText (Leaderboard_Scores);

		Basketball.instance.UpdateFixedBasketballPosition(0f, 1.53f, -15.44f);
	}

	public void ClearStartScreen() {
		HideText (StartGame);
		HideText (ViewLeaderboard);
		HideText (ViewScoreboard); 
		HideText (HowToPlayText);
	}

	private void SetTextActive(Text text) {
		StartGame.color = Color.white;
		ViewScoreboard.color = Color.white;
		ViewLeaderboard.color = Color.white;
		HowToPlayText.color = Color.white;

		switch (text.name) {
		case "Start Game":
			text.color = Color.yellow;
			break;
		case "View Scoreboard":
			text.color = Color.yellow;
			break;
		case "View Leaderboard":
			text.color = Color.yellow;
			break;
		case "How to play":
			text.color = Color.yellow;
			break;
		}
	}
}

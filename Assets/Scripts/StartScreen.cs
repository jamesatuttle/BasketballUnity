using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour {

	TextMesh StartGameText;
	TextMesh ViewScoreboardText;
	TextMesh ViewLeaderboardText;
	TextMesh HowToPlayText;

	bool StartGame;
	bool ViewScoreboard;
	bool ViewLeaderboard;
	bool HowToPlay;

	void Awake()
	{
		StartGameText = GameObject.Find ("Start Game").GetComponent<TextMesh> ();
		ViewScoreboardText = GameObject.Find ("ViewScoreboard").GetComponent<TextMesh> ();
		ViewLeaderboardText = GameObject.Find ("ViewLeaderboard").GetComponent<TextMesh> ();
		HowToPlayText = GameObject.Find ("How to play").GetComponent<TextMesh> ();

		StartGame = false;
		ViewScoreboard = false;
		ViewLeaderboard = false;
		HowToPlay = false;
	}

	void Start ()
	{
		if (GamePlay.ViewingStartScreen)
			SetUpStartScreen ();
		else 
			ClearStartScreen ();
	}
		
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("down") && GamePlay.ViewingStartScreen) {
			if (StartGameText.color == Color.yellow)
				ScoreboardTextActive ();
			else if (ViewScoreboardText.color == Color.yellow)
				LeaderboardTextActive ();
			else if (ViewLeaderboardText.color == Color.yellow)
				HowToPlayTextActive();
			else if (HowToPlayText.color == Color.yellow)
				StartGameTextActive ();
		}

		if (Input.GetKeyDown ("return")) {
			Debug.Log ("return key was pressed");

			if (StartGame) SetUpGame ();
			else if (ViewScoreboard) Cameras.ScoreboardCameraSetUp ();
			else if (ViewLeaderboard) {

			}
		}
	}

	public void SetUpStartScreen()
	{
		Cameras.StartScreenCameraSetup ();
		GameObject.Find ("Start Game").GetComponent<TextMesh> ().text = "START GAME";
		GameObject.Find ("ViewScoreboard").GetComponent<TextMesh> ().text = "SCOREBOARD";
		GameObject.Find ("ViewLeaderboard").GetComponent<TextMesh> ().text = "LEADERBOARD";
		GameObject.Find ("How to play").GetComponent<TextMesh> ().text = "HOW TO PLAY";


		GameObject.Find ("spotlights").active = false;
		GameObject.Find ("Leaderboard Title").GetComponent<Text> ().text = "";
		GameObject.Find ("Leaderboard").GetComponent<Text> ().text = "";

		Basketball.UpdateFixedBasketballPosition(0f, 1.53f, -15.44f);
		StartGameTextActive ();
	}

	public void ClearStartScreen()
	{
		StartGameText.text = "";
		ViewScoreboardText.text = "";
		ViewLeaderboardText.text = "";
		HowToPlayText.text = "";
	}

	public void StartGameTextActive()
	{
		StartGameText.color = Color.yellow;;
		ViewScoreboardText.color = Color.white;
		ViewLeaderboardText.color = Color.white;
		HowToPlayText.color = Color.white;

		StartGame = true;
		ViewScoreboard = false;
		ViewLeaderboard = false;
		HowToPlay = false;
	}

	public void ScoreboardTextActive()
	{
		StartGameText.color = Color.white;
		ViewScoreboardText.color = Color.yellow;
		ViewLeaderboardText.color = Color.white;
		HowToPlayText.color = Color.white;

		StartGame = false;
		ViewScoreboard = true;
		ViewLeaderboard = false;
		HowToPlay = false;
	}

	public void LeaderboardTextActive()
	{
		StartGameText.color = Color.white;
		ViewScoreboardText.color = Color.white;
		ViewLeaderboardText.color = Color.yellow;
		HowToPlayText.color = Color.white;

		StartGame = false;
		ViewScoreboard = false;
		ViewLeaderboard = true;
		HowToPlay = false;
	}

	public void HowToPlayTextActive()
	{
		StartGameText.color = Color.white;
		ViewScoreboardText.color = Color.white;
		ViewLeaderboardText.color = Color.white;
		HowToPlayText.color = Color.yellow;

		StartGame = false;
		ViewScoreboard = false;
		ViewLeaderboard = false;
		HowToPlay = true;
	}

	public void SetUpGame()
	{
		Cameras.MainGameCameraSetUp ();
		Basketball.ResetBall ();
		HUD.countdown = true;
		GamePlay.PlayGame = true;
	}
}

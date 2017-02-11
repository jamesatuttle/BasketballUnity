using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour {

	TextMesh StartGameText;
	TextMesh ViewScoreboardText;
	TextMesh ViewLeaderboardText;

	bool StartGame;
	bool ViewScoreboard;
	bool ViewLeaderboard;

	void Awake()
	{
		StartGameText = GameObject.Find ("Start Game").GetComponent<TextMesh> ();
		ViewScoreboardText = GameObject.Find ("ViewScoreboard").GetComponent<TextMesh> ();
		ViewLeaderboardText = GameObject.Find ("ViewLeaderboard").GetComponent<TextMesh> ();

		StartGame = false;
		ViewScoreboard = false;
		ViewLeaderboard = false;
	}

	void Start () 
	{
		if (GamePlay.ViewingStartScreen) {
			GameObject.Find ("Start Game").GetComponent<TextMesh> ().text = "START GAME";
			GameObject.Find ("ViewScoreboard").GetComponent<TextMesh> ().text = "SCOREBOARD";
			GameObject.Find ("ViewLeaderboard").GetComponent<TextMesh> ().text = "LEADERBOARD";
			StartGameTextActive ();
		} else {
			ClearStartScreen ();
		}
	}
		
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("down") && GamePlay.ViewingStartScreen) {
			Debug.Log ("down key was pressed");
			if (StartGameText.color == Color.yellow)
				ScoreboardTextActive ();
			else if (ViewScoreboardText.color == Color.yellow)
				LeaderboardTextActive ();
			else if (ViewLeaderboardText.color == Color.yellow)
				StartGameTextActive ();
		}

		if (Input.GetKeyDown ("return")) {
			Debug.Log ("return key was pressed");

			if (StartGame) {
				StartGameText.text = "Starting Game...";
			} else if (ViewScoreboard) {
				GameObject.Find ("StartScreen").GetComponent<Camera> ().transform.position = GameObject.Find ("Scoreboard Camera").GetComponent<Camera> ().transform.position;
				GameObject.Find ("StartScreen").GetComponent<Camera> ().camera = GameObject.Find ("Scoreboard Camera").GetComponent<Camera> ().camera;
			} else if (ViewLeaderboard) {

			}
		}
	}

	public void ClearStartScreen()
	{
		StartGameText.text = "";
		ViewScoreboardText.text = "";
		ViewLeaderboardText.text = "";
	}

	public void StartGameTextActive()
	{
		StartGameText.color = Color.yellow;;
		ViewScoreboardText.color = Color.white;
		ViewLeaderboardText.color = Color.white;

		StartGame = true;
		ViewScoreboard = false;
		ViewLeaderboard = false;
	}

	public void ScoreboardTextActive()
	{
		StartGameText.color = Color.white;
		ViewScoreboardText.color = Color.yellow;
		ViewLeaderboardText.color = Color.white;

		StartGame = false;
		ViewScoreboard = true;
		ViewLeaderboard = false;
	}

	public void LeaderboardTextActive()
	{
		StartGameText.color = Color.white;
		ViewScoreboardText.color = Color.white;
		ViewLeaderboardText.color = Color.yellow;

		StartGame = false;
		ViewScoreboard = false;
		ViewLeaderboard = true;
	}
}

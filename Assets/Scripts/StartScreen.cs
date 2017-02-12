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

		GameObject.Find ("spotlights").active = false;
		GameObject.Find ("Leaderboard Title").GetComponent<Text> ().text = "";
		GameObject.Find ("Leaderboard").GetComponent<Text> ().text = "";

		DisplayBall ();
		StartGameTextActive ();
	}

	public void ClearStartScreen()
	{
		StartGameText.text = "";
		ViewScoreboardText.text = "";
		ViewLeaderboardText.text = "";
	}

	public static void DisplayBall() {
		GameObject basketball = GameObject.Find ("Basketball");
		basketball.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
		basketball.transform.position = new Vector3(0f,1.53f,-15.44f); //inital position of ball
		basketball.GetComponent<Rigidbody> ().useGravity = false; //turn gravity off
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

	public void SetUpGame()
	{
		Cameras.MainGameCameraSetUp ();
		Basketball.ResetBall ();
		HUD.countdown = true;
		GamePlay.PlayGame = true;
	}
}

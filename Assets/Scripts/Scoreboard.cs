using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Scoreboard : MonoBehaviour {

	public static Scoreboard instance;

	float totalTime;
	public static int availableBalls;
    public static int score;
	Button BackButton;

	TextMesh TimeRemaining;

	private bool _startedTimer;

	void Awake () {
		instance = this;
		BackButton = GameObject.Find ("BackButton").GetComponent<Button> ();
		TimeRemaining = GameObject.Find ("Time Remaining").GetComponent<TextMesh> ();
	}

	// Use this for initialization
	public void Start () {
		_startedTimer = false;
	
		UpdateAvailableBalls ();

		AddToScore(0);

		LightUpScoreboardBonus (false); // set the bonus text to black
	}

    // Update is called once per frame
    void Update () {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.scoreboard) {
			BackButton.image.enabled = true;
			BackButton.GetComponentInChildren<Text> ().text = "BACK";
			BackButton.onClick.AddListener (StartScreen.instance.SetUpStartScreen);
		}

		SetTimer ();
	}

	public void Reset () {
		SetAvailableBalls ();
		score = 0;
		AddToScore (0);
	}

	public void StartTimer() {
		totalTime = Time.time + 180;
		SetTimer ();
		_startedTimer = true;
	}

	public void StopTimer() {
		_startedTimer = false;
	}

	void SetTimer() {
		if (_startedTimer) {
			int timeLeft = Convert.ToInt32 (totalTime) - Convert.ToInt32 (Time.time);
			if (timeLeft < 0)
				timeLeft = 0;

			if (timeLeft >= 0) {

				if (timeLeft == 240)
					TimeRemaining.text = "04:00";
				else if (timeLeft >= 190)
					TimeRemaining.text = "03:" + (timeLeft - 180);
				else if (timeLeft > 180)
					TimeRemaining.text = "03:0" + (timeLeft - 180);
				else if (timeLeft == 180)
					TimeRemaining.text = "03:00";
				else if (timeLeft >= 130)
					TimeRemaining.text = "02:" + (timeLeft - 120);
				else if (timeLeft > 120)
					TimeRemaining.text = "02:0" + (timeLeft - 120);
				else if (timeLeft == 120)
					TimeRemaining.text = "02:00";
				else if (timeLeft >= 70)
					TimeRemaining.text = "01:" + (timeLeft - 60);
				else if (timeLeft > 60)
					TimeRemaining.text = "01:0" + (timeLeft - 60);
				else if (timeLeft == 60)
					TimeRemaining.text = "01:00";
				else if (timeLeft >= 10)
					TimeRemaining.text = "00:" + timeLeft;
				else
					TimeRemaining.text = "00:0" + timeLeft;
			}

			if (timeLeft == 0f && GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame) {
				GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.gameOver;
			}
		} else {
			TimeRemaining.text = "00:00";
		}
	}

	public void UpdateAvailableBalls() {
		TextMesh Scoreboard_noOfBalls = GameObject.Find("NumberOfBalls").GetComponent<TextMesh>();

		if (availableBalls >= 10)
			Scoreboard_noOfBalls.text = "0" + availableBalls.ToString ();
		else if (availableBalls >= 0)
			Scoreboard_noOfBalls.text = "00" + availableBalls.ToString ();
	}

	public void MinusAvailableBalls() {
		if (availableBalls > 0)
			availableBalls--;
		if (availableBalls == 0) {
			if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame) {
				GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.gameOver;
				StopTimer ();
			} else if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.preGame) {
				GamePlay.GameIsPlayable = false;
				GamePlay.SetUpMainGame ();
			}
		}

		UpdateAvailableBalls();
	}

	public void SetAvailableBalls() {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame)
			availableBalls = 30;
		else if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.preGame) 
			availableBalls = 3;
		UpdateAvailableBalls ();
	}

	public static void AddToScore(int add) {
		TextMesh Scoreboard_score = GameObject.Find("Score").GetComponent<TextMesh>();

		score = score + add;

		if (score >= 100)
			Scoreboard_score.text = score.ToString();
		else if (score >= 10)
			Scoreboard_score.text = "0" + score.ToString();
		else if (score >= 0)
			Scoreboard_score.text = "00" + score.ToString();
	}

    public static void updateBonusColour(string hex) {
        Color bonusColour = new Color();
        ColorUtility.TryParseHtmlString(hex, out bonusColour);
        GameObject.Find("Bonus").GetComponent<TextMesh>().color = bonusColour;
    }

	public static void LightUpScoreboardBonus(bool lightUp) {
		if (lightUp)
			updateBonusColour("#6BD289FF"); //set to green
		else
			updateBonusColour("#181717"); //set to black
	}


}

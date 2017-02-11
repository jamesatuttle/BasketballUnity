﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using AssemblyCSharp;

public class Scoreboard : MonoBehaviour {

	private int timer;
	public static int availableBalls;
    public static int score;

	// Use this for initialization
	public void Start () {
		timer = 240;
		SetTimerText ();

		availableBalls = 3;
		UpdateAvailableBalls ();

        score = 0;
        updateScore();

        updateBonusColour("#181717"); // set the bonus text to black

        GamePlay.isGamePlayable = true;

        GameObject.Find("Game Over").GetComponent<Text>().text = "";
    }

    // Update is called once per frame
    void Update () {
		if (GamePlay.PlayGame) {
			timer = timer - 1;
			SetTimerText ();
		}
	}

	void SetTimerText()
	{
		if (timer >= 0) {

			if (timer == 240)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "04:00";
			else if (timer >= 190)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "03:" + (timer - 180);
			else if (timer > 180)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "03:0" + (timer - 180);
			else if (timer == 180)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "03:00";
			else if (timer > 130)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "02:" + (timer - 120);
			else if (timer > 120)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "02:0" + (timer - 120);

			else if (timer == 120)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "02:00";
			else if (timer >= 70)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "01:" + (timer - 60);
			else if (timer > 60)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "01:0" + (timer - 60);
			else if (timer == 60)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "01:00";
			else if (timer > 10)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "00:" + timer;
			else
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "00:0" + timer;
		}
	}

	public static void UpdateAvailableBalls()
	{
		TextMesh Scoreboard_noOfBalls = GameObject.Find("NumberOfBalls").GetComponent<TextMesh>();

		if (availableBalls >= 10)
			Scoreboard_noOfBalls.text = "0" + availableBalls.ToString ();
		else if (availableBalls >= 0)
			Scoreboard_noOfBalls.text = "00" + availableBalls.ToString ();
	}

	public static void MinusAvailableBalls()
	{
		if (availableBalls > 0)
			availableBalls--;
		else if (availableBalls == 0) {
			GameObject.Find ("Game Over").GetComponent<Text> ().text = "GAME OVER";
			GamePlay.isGamePlayable = false;
		}

		UpdateAvailableBalls();
	}

    public void updateScore()
    {
        TextMesh Scoreboard_score = GameObject.Find("Score").GetComponent<TextMesh>();

        if (score >= 100)
            Scoreboard_score.text = score.ToString();
        else if (score >= 10)
            Scoreboard_score.text = "0" + score.ToString();
        else if (score >= 0)
            Scoreboard_score.text = "00" + score.ToString();
    }

    public static void updateBonusColour(string hex)
    {
        Color bonusColour = new Color();
        ColorUtility.TryParseHtmlString(hex, out bonusColour);
        GameObject.Find("Bonus").GetComponent<TextMesh>().color = bonusColour;
    }


}

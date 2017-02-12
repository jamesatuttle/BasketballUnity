using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Scoreboard : MonoBehaviour {

	float totalTime;
	public static int availableBalls;
    public static int score;

	// Use this for initialization
	public void Start () {
		totalTime = Time.time + 240;
		SetTimerText ();

		availableBalls = 3;
		UpdateAvailableBalls ();

        score = 0;
        updateScore();

        updateBonusColour("#181717"); // set the bonus text to black

        GamePlay.isGamePlayable = true;
    }

    // Update is called once per frame
    void Update () {
		SetTimerText ();
	}

	void SetTimerText()
	{
		int timeLeft = Convert.ToInt32(totalTime) - Convert.ToInt32(Time.time);
		if (timeLeft < 0) timeLeft = 0;

		if (timeLeft >= 0) {

			if (timeLeft == 240)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "04:00";
			else if (timeLeft >= 190)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "03:" + (timeLeft - 180);
			else if (timeLeft > 180)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "03:0" + (timeLeft - 180);
			else if (timeLeft == 180)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "03:00";
			else if (timeLeft >= 130)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "02:" + (timeLeft - 120);
			else if (timeLeft > 120)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "02:0" + (timeLeft - 120);

			else if (timeLeft == 120)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "02:00";
			else if (timeLeft >= 70)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "01:" + (timeLeft - 60);
			else if (timeLeft > 60)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "01:0" + (timeLeft - 60);
			else if (timeLeft == 60)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "01:00";
			else if (timeLeft >= 10)
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "00:" + timeLeft;
			else
				GameObject.Find("Time Remaining").GetComponent<TextMesh> ().text = "00:0" + timeLeft;
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
			HUD.GameOver();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HUD : MonoBehaviour {

	static float endTime;
	public static bool countdown;

	// Use this for initialization
	public static void Start () {
		GameObject.Find ("Countdown").GetComponent<Text> ().text = "";
		GameObject.Find("Game Over").GetComponent<Text>().text = "";
		endTime = Time.time + 4;
	}
	
	// Update is called once per frame
	void Update () {
		if (countdown) CountdownFromThree ();
	}

	public static void GameOver()
	{
		GameObject.Find("Game Over").GetComponent<Text>().text = "GAME OVER";
	}

	public static void CountdownFromThree()
	{
		//GameObject.Find ("Countdown").GetComponent<Text> ().text = "3";
		int timeLeft = Convert.ToInt32(endTime - Time.time);
		if (timeLeft <= -1) timeLeft = -1;
		if (timeLeft == -1) GameObject.Find ("Countdown").GetComponent<Text> ().text = "";
		else GameObject.Find ("Countdown").GetComponent<Text> ().text = timeLeft.ToString ();
	}
}

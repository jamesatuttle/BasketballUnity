using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HUD : MonoBehaviour {

	static float endTime;
	public static bool countdown;
	public static int Down = 3;
	public static bool setTimer = true;
	public int timer;

	void Awake()
	{
		timer = 55;
	}

	public static void Start () {
		GameObject.Find ("Countdown").GetComponent<Text> ().text = "";
		GameObject.Find ("Game Over").GetComponent<Text>().text = "";
	}

	void Update () {
		if (countdown) CountdownFromThree();
	}

	void CountdownFromThree()
	{
		GameObject.Find ("PracticeText").GetComponent<Text> ().text = "";

		//Debug.Log ("CountdownFromThree: " + countdown + ", " + timer);
		timer--;
		//Debug.Log (timer);
		if (timer == 44) {
			GameObject.Find ("Countdown").GetComponent<Text> ().text = "3";
			//Debug.Log ("3 COUNTDOWN");
		}
		else if (timer == 33)
			GameObject.Find ("Countdown").GetComponent<Text> ().text = "2";
		else if (timer == 22)
			GameObject.Find ("Countdown").GetComponent<Text> ().text = "1";
		else if (timer == 11)
			GameObject.Find ("Countdown").GetComponent<Text> ().text = "GO";
		else if (timer < 0) {
			GameObject.Find ("Countdown").GetComponent<Text> ().text = "";
			countdown = false;
			GamePlay.GameIsPlayable = true;
		}
	}

	public static void DisplayPreGameText()
	{
		GameObject.Find ("PracticeText").GetComponent<Text> ().text = "3 BALL PRACTICE";
		GameObject.Find ("PracticeText").GetComponent<Text> ().fontStyle = FontStyle.Normal;
	}

	public static void GameOver()
	{
		GameObject.Find("Game Over").GetComponent<Text>().text = "GAME OVER";
	}
		
}

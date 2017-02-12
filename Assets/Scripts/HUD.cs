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

	public static void Start () {
		GameObject.Find ("Countdown").GetComponent<Text> ().text = "";
		GameObject.Find("Game Over").GetComponent<Text>().text = "";
	}

	void Update () {
		//StartCoroutine (CountdownFromThree ());
		if (countdown)
			GameObject.Find ("Countdown").GetComponent<Text> ().text = "3";
	}

	/*IEnumerator CountdownFromThree()
	{
		while (countdown) {
			GameObject.Find ("Countdown").GetComponent<Text> ().text = "3";
			yield return new WaitForSeconds(1.0f);
			GameObject.Find ("Countdown").GetComponent<Text> ().text = "2";
			yield return new WaitForSeconds(1.0f);
			GameObject.Find ("Countdown").GetComponent<Text> ().text = "1";
			yield return new WaitForSeconds(1.0f);
			GameObject.Find ("Countdown").GetComponent<Text> ().text = "0";
			yield return new WaitForSeconds(1.0f);
			GameObject.Find ("Countdown").GetComponent<Text> ().text = "GO!";
			countdown = false;
		}
	}*/


	public static void GameOver()
	{
		GameObject.Find("Game Over").GetComponent<Text>().text = "GAME OVER";
	}
		
}

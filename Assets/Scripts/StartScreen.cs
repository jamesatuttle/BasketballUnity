using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	void Awake()
	{
		if (GamePlay.ViewingStartScreen) {
			GameObject.Find ("Start Game").GetComponent<TextMesh> ().text = "START GAME";
			GameObject.Find ("ViewScoreboard").GetComponent<TextMesh> ().text = "SCOREBOARD";
			GameObject.Find ("ViewLeaderboard").GetComponent<TextMesh> ().text = "LEADERBOARD";

			GameObject.Find ("Start Game").GetComponent<TextMesh> ().color = Color.yellow;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//GameObject.Find("Start Game").GetComponent<TextMesh>().text = "";

	}
}

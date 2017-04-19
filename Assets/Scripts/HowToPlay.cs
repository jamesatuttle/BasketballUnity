using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour {

	private Canvas _howToPlayCanvas; //a variable to store the how to play canvas - which displays the information
	private Button _howToPlayBackButton; //a variable to store the back button for the how to play screen

	//Awake is called at the start of the game, used to initialise variables
	void Awake () {
		_howToPlayCanvas = GameObject.Find ("HowToPlay").GetComponent<Canvas> ().rootCanvas;
		_howToPlayBackButton = GameObject.Find ("HowToPlayBackButton").GetComponent<Button> ();
	}

	// Update is called once per frame
	void Update () {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.howToPlay) { //if the active screen is how to play...
			_howToPlayCanvas.enabled = true; //activate the howToPlay canvas
			_howToPlayBackButton.onClick.AddListener (BackToStart); //activate the back button
		} else { //if the active screen is not how to play...
			_howToPlayCanvas.enabled = false; //disable the canvas
		}
	}

	//a method called when the back button is pressed
	void BackToStart() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.startScreen; //sets the active screen to the start screen
	}
}

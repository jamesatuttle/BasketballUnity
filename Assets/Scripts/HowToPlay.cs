using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour {

	private Canvas _howToPlayCanvas;
	private Button _howToPlayBackButton;

	void Awake () {
		_howToPlayCanvas = GameObject.Find ("HowToPlay").GetComponent<Canvas> ().rootCanvas;
		_howToPlayBackButton = GameObject.Find ("HowToPlayBackButton").GetComponent<Button> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.howToPlay) {
			_howToPlayCanvas.enabled = true;
			_howToPlayBackButton.onClick.AddListener (BackToStart);
		} else {
			_howToPlayCanvas.enabled = false;
		}
	}

	void BackToStart() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.startScreen;
	}
}

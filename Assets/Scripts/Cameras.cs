using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameras : MonoBehaviour {

	Camera StartScreen;
	Camera FrontCamera;
	Camera Scoreboard;
	Camera SideHoop;

	void Start()
	{

		StartScreen = GameObject.Find ("StartScreen").GetComponent<Camera> ();
		FrontCamera = GameObject.Find ("Front View Camera").GetComponent<Camera> ();
		Scoreboard = GameObject.Find ("Scoreboard Camera").GetComponent<Camera> ();
		SideHoop = GameObject.Find ("Side Hoop Camera").GetComponent<Camera> ();

		StartScreen.gameObject.active = true;
		FrontCamera.gameObject.active = false;
		Scoreboard.gameObject.active = false;
		SideHoop.gameObject.active = false;
	}

}

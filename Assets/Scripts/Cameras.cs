using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameras : MonoBehaviour {

	static Camera StartScreen;
	static Camera FrontCamera;
	static Camera Scoreboard;
	static Camera SideHoop;

	public Camera camera;
	public Camera camera2;

	void Start()
	{
		Debug.Log ("Inside Camera Start");
		//StartScreen = GameObject.Find ("StartScreen").GetComponent<Camera> ();
		/*FrontCamera = GameObject.Find ("Front View Camera").GetComponent<Camera> ();
		Scoreboard = GameObject.Find ("Scoreboard Camera").GetComponent<Camera> ();
		SideHoop = GameObject.Find ("Side Hoop Camera").GetComponent<Camera> ();*/

		/*StartScreen.gameObject.SetActive = false;
		FrontCamera.gameObject.SetActive = false;
		Scoreboard.gameObject.SetActive = false;
		SideHoop.gameObject.SetActive = false;*/
		//StartScreenCameraSetup ();

		camera.enabled = true;
		camera2.enabled = false;
	}

	void Update() {
		//This will toggle the enabled state of the two cameras between true and false each time
		if (Input.GetKeyDown ("backspace")) {
			Debug.Log ("backspace");
			camera.enabled = !camera.enabled;
			camera2.enabled = !camera2.enabled;
		}
	}

	public static void StartScreenCameraSetup()
	{
		Debug.Log ("Inside Camera StartScreenCameraSetup");

		//PlayerPrefs.SetInt("UnitySelectMonitor", 3); // Select monitor 4

		/*FrontCamera.enabled = false;
		Scoreboard.enabled = false;
		SideHoop.enabled = false;*/

		//StartScreen.gameObject.SetActive = true;
	}

}

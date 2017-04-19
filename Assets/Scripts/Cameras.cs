using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameras : MonoBehaviour {

	public static Vector3 MainCameraPosition = new Vector3 (0f, 6.5f, 5f); //the position of the main camera in the game environment
	private static Camera MainCamera; //a value to score the main camera object

	public enum Facing { //an emum value to give the values within an associated integer value
		forwards = 0,
		left
	}

	//Awake is called at the start of the game, used to initialise variables
	void Awake () {
		MainCamera = GameObject.Find ("MainCamera").GetComponent<Camera> ();
	}

	/*
	* Update is called once per frame
	* Used to change the camera depending on which screen is active
	*/
	void Update () {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.startScreen)
			StartScreenCameraSetup ();

		else if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.scoreboard)
			ScoreboardCameraSetUp ();

		else if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.leaderboard)
			LeaderboardCameraSetUp();

	}

	public static void StartScreenCameraSetup() {
		UpdateCameraRotation (Facing.forwards);
		UpdateCameraPosition (0f, 2.9f, -21f, 45);
	}

	public static void MainGameCameraSetUp() {
		UpdateCameraRotation (Facing.forwards);
		UpdateCameraPosition (MainCameraPosition.x, MainCameraPosition.y, MainCameraPosition.z, 34);
	}

	public static void ScoreboardCameraSetUp() {
		UpdateCameraRotation (Facing.forwards);
		UpdateCameraPosition (13f, 14.3f, 37.9f, 62);
	}

	public static void LeaderboardCameraSetUp() {
		UpdateCameraRotation (Facing.left);
		UpdateCameraPosition (-3f, 10f, 10f, 45);
	}

	/*
	* a generic method to update the camera position based on the parameters
	* x: float - x value in the 3D space
	* y: float - y value in the 3D space
	* z: float - z value in the 3D space
	* FOV: int - the Field Of View value for the camera
	*/
	public static void UpdateCameraPosition(float x, float y, float z, int FOV) {
		MainCamera.transform.position = new Vector3(x, y, z);
		MainCamera.GetComponent<Camera> ().fieldOfView = FOV;
	}

	/*
	* A generic method to update the camera rotation based on the parameters
	* facing: Facing - the way the camera needs to face, either forwards or left (the enum value)
	*/
	public static void UpdateCameraRotation(Facing facing) {
		float xRotation = MainCamera.transform.eulerAngles.x; //get the main camera x rotation
		float yRotation = MainCamera.transform.eulerAngles.y; //get the main camera y rotation
		float zRotation = MainCamera.transform.eulerAngles.z; //get the main camera z rotation

		switch (facing) { //switch case statement
		case Facing.forwards: //if the enum is forwards...
			yRotation = 0f; //set the y rotation to 0
			break;
		case Facing.left: //if the enum is left...
			yRotation = 270f; //set the y value to 270
			break;
		default:
			yRotation = MainCamera.transform.eulerAngles.y; //if no Facing value, use the current y value
			break;
		}

		MainCamera.transform.eulerAngles = new Vector3 (xRotation, yRotation, zRotation); //update the camera rotation with the new x, y and z values
	}

}

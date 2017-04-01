using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameras : MonoBehaviour {

	public static Vector3 MainCameraPosition = new Vector3 (0f, 6.5f, 5f);
	private static Camera MainCamera;

	public enum Facing {
		forwards = 0,
		left
	}

	void Awake () {
		MainCamera = GameObject.Find ("MainCamera").GetComponent<Camera> ();
	}

	void Update () {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.startScreen)
			StartScreenCameraSetup ();
			
		else if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.scoreboard)
			ScoreboardCameraSetUp ();

		else if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame || GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.preGame)
			MainGameCameraSetUp ();

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

	public static void UpdateCameraPosition(float x, float y, float z, int FOV) {
		MainCamera.transform.position = new Vector3(x, y, z);
		MainCamera.GetComponent<Camera> ().fieldOfView = FOV;
	}

	public static void UpdateCameraRotation(Facing facing) {
		float xRotation = MainCamera.transform.eulerAngles.x;
		float yRotation = MainCamera.transform.eulerAngles.y;
		float zRotation = MainCamera.transform.eulerAngles.z;

		switch (facing) {
		case Facing.forwards:
			yRotation = 0f;
			break;
		case Facing.left:
			yRotation = 270f;
			break;
		default:
			yRotation = MainCamera.transform.eulerAngles.y;
			break;
		}

		MainCamera.transform.eulerAngles = new Vector3 (xRotation, yRotation, zRotation);
	}

}

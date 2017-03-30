using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameras : MonoBehaviour {

	public static Vector3 MainCameraPosition = new Vector3 (0f, 6.5f, 5f);

	void Update() {
		if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.startScreen)
			StartScreenCameraSetup ();
			
		else if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.scoreboard)
			ScoreboardCameraSetUp ();

		else if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame)
			MainGameCameraSetUp ();
		
	}

	public static void StartScreenCameraSetup()
	{
		UpdateCameraPosition (0f, 2.9f, -21f, 45);
	}

	public static void MainGameCameraSetUp()
	{
		UpdateCameraPosition (MainCameraPosition.x, MainCameraPosition.y, MainCameraPosition.z, 34);
	}

	public static void ScoreboardCameraSetUp()
	{
		UpdateCameraPosition (13f, 14.3f, 37.9f, 62);
	}

	public static void UpdateCameraPosition(float x, float y, float z, int FOV) 
	{
		GameObject.Find ("MainCamera").GetComponent<Camera> ().transform.position = new Vector3(x, y, z);
		GameObject.Find ("MainCamera").GetComponent<Camera> ().GetComponent<Camera> ().fieldOfView = FOV;
	}

}

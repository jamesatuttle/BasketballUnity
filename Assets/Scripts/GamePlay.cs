using UnityEngine;

public class GamePlay : MonoBehaviour {
	public static bool isGamePlayable;
	public static bool ViewingStartScreen;
	public bool PlayGame;
	public bool ViewScoreboard;
	public bool ViewLeaderboard;

	void Start()
	{
		//Cameras.StartScreenCameraSetup ();
	
	}

	void Awake()
	{
		ViewingStartScreen = true;
	}
}

using UnityEngine;

public class GamePlay : MonoBehaviour {
	public static bool GameIsPlayable;
	public static bool PlayingMainGame;
	public static bool ViewingStartScreen;
	//public static bool PlayGame;
	public bool ViewScoreboard;
	public bool ViewLeaderboard;

	void Start()
	{
		//Cameras.StartScreenCameraSetup ();
	
	}

	void Awake()
	{
		//PlayGame = false;
	}

	public static void SetUpMainGame()
	{
		Cameras.MainGameCameraSetUp ();
		Basketball.ResetBall ();
		StartScreen.ClearStartScreen ();
		Scoreboard.ResetScoreboard ();
		HUD.countdown = true;
		PlayingMainGame = true;
		//GamePlay.PlayGame = true;
	}

	public static void SetUpPregame()
	{
		Cameras.MainGameCameraSetUp ();
		Basketball.ResetBall ();
		StartScreen.ClearStartScreen ();
		PlayingMainGame = false;
		GameIsPlayable = true;
	}
}

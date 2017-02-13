using UnityEngine;

public class GamePlay : MonoBehaviour {
	public static bool GameIsPlayable;
	public static bool PlayingMainGame;
	public static bool ViewingStartScreen;
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
		PlayingMainGame = true;
		Scoreboard.ResetScoreboard ();
		HUD.countdown = true;
		//GamePlay.PlayGame = true;
	}

	public static void SetUpPregame()
	{
		Cameras.MainGameCameraSetUp ();
		Basketball.ResetBall ();
		StartScreen.ClearStartScreen ();
		HUD.DisplayPreGameText ();
		PlayingMainGame = false;
		Scoreboard.ResetScoreboard ();
		GameIsPlayable = true;
	}
}

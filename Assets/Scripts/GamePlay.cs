using UnityEngine;

public class GamePlay : MonoBehaviour {
	public static bool GameIsPlayable;
	public static bool PlayingMainGame;
	public static int ActiveScreenValue;

	public enum ActiveScreen {
		startScreen = 0,
		playedBeforeQuestion,
		enterName,
		registerName,
		preGame,
		mainGame,
		scoreboard,
		leaderboard,
		howToPlay,
	};

	public static void SetUpMainGame()
	{
		Cameras.MainGameCameraSetUp ();
		Basketball.ResetBall ();
		StartScreen.instance.ClearStartScreen ();
		PlayingMainGame = true;
		Scoreboard.ResetScoreboard ();
		HUD.countdown = true;
	}

	public static void SetUpPregame()
	{
		Cameras.MainGameCameraSetUp ();
		Basketball.ResetBall ();
		StartScreen.instance.ClearStartScreen ();
		HUD.DisplayPreGameText ();
		PlayingMainGame = false;
		Scoreboard.ResetScoreboard ();
		GameIsPlayable = true;
	}
}

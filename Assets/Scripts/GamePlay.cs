using UnityEngine;

public class GamePlay : MonoBehaviour {
	public static bool GameIsPlayable;
	//public static bool PlayingMainGame;
	public static int ActiveScreenValue;

	public enum ActiveScreen {
		startScreen = 0,
		playedBeforeQuestion,
		enterName,
		registerName,
		welcome,
		welcomeBack,
		preGame,
		mainGame,
		scoreboard,
		leaderboard,
		howToPlay,
		gameOver
	};

	public static void SetUpMainGame() {
		ActiveScreenValue = (int)ActiveScreen.mainGame;
		Cameras.MainGameCameraSetUp ();
		Basketball.instance.ResetBall ();
		StartScreen.instance.ClearStartScreen ();
		Scoreboard.instance.SetAvailableBalls ();
		//Scoreboard.instance.StartTimer ();
		//HUD.countdown = true;
		HUD.instance.StartCountdown();
		//PlayingMainGame = true;
	}

	public static void SetUpPregame() {
		ActiveScreenValue = (int)ActiveScreen.preGame;
		Cameras.MainGameCameraSetUp ();
		Basketball.instance.ResetBall ();
		StartScreen.instance.ClearStartScreen ();
		HUD.DisplayPreGameText ();
		//PlayingMainGame = false;
		Scoreboard.instance.SetAvailableBalls ();
		//HUD.instance.SetUpCountdown ();
		GameIsPlayable = true;
	}
}

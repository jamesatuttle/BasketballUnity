using UnityEngine;

public class GamePlay : MonoBehaviour {
	public static bool GameIsPlayable; //a public bool storing if the game is playable
	public static int ActiveScreenValue; //a public int storing the active value - corresponds with the enum below
	public static bool restartActivated; //a public bool storing if the game restart has been activated

	public enum ActiveScreen { //an emum value to give the values within an associated integer value
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

	/*
	* Sets up the main game when called
	*/
	public static void SetUpMainGame() {
		ActiveScreenValue = (int)ActiveScreen.mainGame; //sets the active screen to the main game
		Cameras.MainGameCameraSetUp ();
		Basketball.instance.ResetBall ();
		StartScreen.instance.ClearStartScreen ();
		Scoreboard.instance.SetAvailableBalls ();
		HUD.instance.StartCountdown();
		GamePlay.restartActivated = false; //resets the restartActivated boolean - to prevent restarting more than once
	}

	/*
	* Sets up the pre-game when called
	*/
	public static void SetUpPregame() {
		ActiveScreenValue = (int)ActiveScreen.preGame; //sets the active screen to the pre game
		Cameras.MainGameCameraSetUp ();
		Basketball.instance.ResetBall ();
		StartScreen.instance.ClearStartScreen ();
		HUD.DisplayPreGameText ();
		Scoreboard.instance.SetUpScoreboard ();
		Login.instance.AddNewLeaderboardRow (); //inserts a new row into the leaderboard - preparing for values to be added later
		GameIsPlayable = true;
	}
}

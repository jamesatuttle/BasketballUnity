﻿using UnityEngine;

public class GamePlay : MonoBehaviour {
	public static bool GameIsPlayable;
	public static int ActiveScreenValue;
	public static bool restartActivated;

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
		HUD.instance.StartCountdown();
		GamePlay.restartActivated = false;
	}

	public static void SetUpPregame() {
		ActiveScreenValue = (int)ActiveScreen.preGame;
		Cameras.MainGameCameraSetUp ();
		Basketball.instance.ResetBall ();
		StartScreen.instance.ClearStartScreen ();
		HUD.DisplayPreGameText ();
		Scoreboard.instance.SetUpScoreboard ();
		Login.instance.AddNewLeaderboardRow ();
		GameIsPlayable = true;
	}
}

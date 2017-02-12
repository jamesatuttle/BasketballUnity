﻿using NUnit.Framework;
using UnityEngine;

[TestFixture]

public class HUDTests 
{

	private void ClearHUD()
	{
		GameObject.Find ("Game Over").GetComponent<UnityEngine.UI.Text> ().text = "";
		GameObject.Find ("Countdown").GetComponent<UnityEngine.UI.Text> ().text = "";
		GameObject.Find ("Leaderboard").GetComponent<UnityEngine.UI.Text> ().text = "";
		GameObject.Find ("Leaderboard Title").GetComponent<UnityEngine.UI.Text> ().text = "";
	}

	[Test]
	public void gameOverTextCLearedOnStart()
	{
		HUD.Start();
		Assert.AreEqual("", GameObject.Find("Game Over").GetComponent<UnityEngine.UI.Text>().text);
		ClearHUD ();
	}

	[Test]
	public void gameOverTextDisplays()
	{
		HUD.GameOver();
		Assert.AreEqual("GAME OVER", GameObject.Find("Game Over").GetComponent<UnityEngine.UI.Text>().text);
		ClearHUD ();
	}

}
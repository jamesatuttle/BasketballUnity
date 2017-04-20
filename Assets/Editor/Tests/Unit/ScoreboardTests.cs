using NUnit.Framework;
using UnityEngine;

[TestFixture]

public class ScoreboardTests
{
	[TearDown] public void Cleanup()
	{
		HUDTests.ClearHUD ();
	}

    public void startScoreboard()
    {
        var scoreboard = new Scoreboard();
        scoreboard.Start();
    }

    public void startBasketDetected()
    {
        var basketDetected = new BasketDetected();
        basketDetected.Start();
    }

    [Test]
    public void notPlayableGameOnStart()
    {
        startScoreboard();
		Assert.AreEqual(false, GamePlay.GameIsPlayable);
    }

    [Test]
    public void correctBallsShownIfZero()
    {
        Scoreboard.availableBalls = 0;
        Scoreboard.instance.UpdateAvailableBalls();
        Assert.AreEqual("000", GameObject.Find("NumberOfBalls").GetComponent<TextMesh>().text);
    }

    [Test]
    public void correctBallsShownIfLessThan10()
    {
        Scoreboard.availableBalls = 3;
        Scoreboard.instance.UpdateAvailableBalls();
        Assert.AreEqual("003", GameObject.Find("NumberOfBalls").GetComponent<TextMesh>().text);
    }

    [Test]
    public void correctBallsShownIfMoreThan10()
    {
        Scoreboard.availableBalls = 12;
        Scoreboard.instance.UpdateAvailableBalls();
        Assert.AreEqual("012", GameObject.Find("NumberOfBalls").GetComponent<TextMesh>().text);
    }

	[Test]
	public void PreGame_minusAvailableBalls()
	{
		GamePlay.GameIsPlayable = true;
		Scoreboard.availableBalls = 3;
		Scoreboard.instance.MinusAvailableBalls();


		Assert.AreEqual(2, Scoreboard.availableBalls);
	}

	[Test]
	public void PreGame_minusAvailableBalls_Zero_Reset()
	{
		GamePlay.GameIsPlayable = true;
		Scoreboard.availableBalls = 1;
		Scoreboard.instance.MinusAvailableBalls();

		Assert.AreEqual(10, Scoreboard.availableBalls);
	}

	[Test]
	public void PreGame_minusAvailableBalls_Zero_StartCountdown()
	{
		GamePlay.GameIsPlayable = true;
		Scoreboard.availableBalls = 0;
		Scoreboard.instance.MinusAvailableBalls();
	}

	[Test]
	public void MainGame_minusAvailableBalls()
	{
		GamePlay.GameIsPlayable = true;
		Scoreboard.availableBalls = 3;
		Scoreboard.instance.MinusAvailableBalls();

		Assert.AreEqual(2, Scoreboard.availableBalls);
	}

	[Test]
    public void MainGame_minusAvailableBalls_Zero()
    {
		GamePlay.GameIsPlayable = true;
        Scoreboard.availableBalls = 1;
        Scoreboard.instance.MinusAvailableBalls();

        Assert.AreEqual(0, Scoreboard.availableBalls);
    }

    [Test]
    public void MainGame_minusAvailableBalls_Zero_GameOver()
    {
		GamePlay.GameIsPlayable = true;
        Scoreboard.availableBalls = 0;
        Scoreboard.instance.MinusAvailableBalls();

        Assert.AreEqual("GAME OVER", GameObject.Find("Game Over").GetComponent<UnityEngine.UI.Text>().text);
    }

    [Test]
    public void minusAvailableBalls_Zero_IsGamePlayable()
    {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.mainGame;
		GamePlay.GameIsPlayable = true;
        Scoreboard.availableBalls = 0;
        Scoreboard.instance.MinusAvailableBalls();

		Assert.AreEqual(false, GamePlay.GameIsPlayable);
    }

    [Test]
    public void showCorrectScore_onStart()
    {
        startScoreboard();
        Assert.AreEqual("000", GameObject.Find("Score").GetComponent<TextMesh>().text);
    }
    
    [Test]
    public void showCorrectScore_WhenScored()
    {
        Collider col = new Collider();
        BasketDetected basketDetected = new BasketDetected();

        startBasketDetected();
        startScoreboard();
        basketDetected.OnTriggerEnter(col);

        Assert.AreEqual("010", GameObject.Find("Score").GetComponent<TextMesh>().text);
    }

    [Test]
    public void preventScore_OnCollision_WhenNoEnterTrigger()
    {
        Collider col = new Collider();
        BasketDetected basketDetected = new BasketDetected();

        startBasketDetected();
        startScoreboard();

        Assert.AreNotEqual("010", GameObject.Find("Score").GetComponent<TextMesh>().text);
        Assert.AreEqual("000", GameObject.Find("Score").GetComponent<TextMesh>().text);
    }

    [Test]
    public void showCorrectScore_WhenScoredTwice()
    {
        Collider col = new Collider();
        BasketDetected basketDetected = new BasketDetected();

        startBasketDetected();
        startScoreboard();
        basketDetected.OnTriggerEnter(col);

        basketDetected.OnTriggerEnter(col);

        Assert.AreEqual("020", GameObject.Find("Score").GetComponent<TextMesh>().text);
    }

    [Test]
    public void showCorrectScore_NotAddSingleOnHoopThrice()
    {
        Collider col = new Collider();
        BasketDetected basketDetected = new BasketDetected();

        startBasketDetected();
        startScoreboard();
        basketDetected.OnTriggerEnter(col);

        basketDetected.OnTriggerEnter(col);

        basketDetected.OnTriggerEnter(col);

        Assert.AreNotEqual("30", GameObject.Find("Score").GetComponent<TextMesh>().text);
    }

    [Test]
    public void showCorrectScore_OnBonus()
    {
        Collider col = new Collider();
        BasketDetected basketDetected = new BasketDetected();

        startBasketDetected();
        startScoreboard();
        basketDetected.OnTriggerEnter(col);

        basketDetected.OnTriggerEnter(col);

        basketDetected.OnTriggerEnter(col);

        Assert.AreEqual("120", GameObject.Find("Score").GetComponent<TextMesh>().text);
    }
		
    [Test]
    public void hideBonusLight_OnStart()
    {
        Collider col = new Collider();
        BasketDetected basketDetected = new BasketDetected();

        startBasketDetected();
        startScoreboard();

        Color bonusOff = new Color();
        ColorUtility.TryParseHtmlString("#181717", out bonusOff);

        Assert.AreEqual(bonusOff, GameObject.Find("Bonus").GetComponent<TextMesh>().color);
    }

    [Test]
    public void hideBonusLight_AfterOneHoopScored()
    {
        Collider col = new Collider();
        BasketDetected basketDetected = new BasketDetected();

        startBasketDetected();
        startScoreboard();

        basketDetected.OnTriggerEnter(col);

        Color bonusOff = new Color();
        ColorUtility.TryParseHtmlString("#181717", out bonusOff);

        Assert.AreEqual(bonusOff, GameObject.Find("Bonus").GetComponent<TextMesh>().color);
    }

    [Test]
    public void hideBonusLight_AfterTwoHoopsScored()
    {
        Collider col = new Collider();
        BasketDetected basketDetected = new BasketDetected();

        startBasketDetected();
        startScoreboard();

        basketDetected.OnTriggerEnter(col);

        basketDetected.OnTriggerEnter(col);

        Color bonusOff = new Color();
        ColorUtility.TryParseHtmlString("#181717", out bonusOff);

        Assert.AreEqual(bonusOff, GameObject.Find("Bonus").GetComponent<TextMesh>().color);
    }

    [Test]
    public void showBonusLight_OnBonus()
    {
        Collider col = new Collider();
        BasketDetected basketDetected = new BasketDetected();

        startBasketDetected();
        startScoreboard();
        basketDetected.OnTriggerEnter(col);

        basketDetected.OnTriggerEnter(col);

        basketDetected.OnTriggerEnter(col);

        Color bonusColour = new Color();
        ColorUtility.TryParseHtmlString("#6BD289FF", out bonusColour);

        Assert.AreEqual(bonusColour, GameObject.Find("Bonus").GetComponent<TextMesh>().color);
    }

    

}

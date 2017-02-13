using NUnit.Framework;
using UnityEngine;

[TestFixture]

public class ScoreboardTests
{
    //private Scoreboard scoreboard = new Scoreboard();

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
    public void playableGameOnStart()
    {
        startScoreboard();
		Assert.AreEqual(true, GamePlay.GameIsPlayable);
    }

    [Test]
    public void correctBallsShownIfZero()
    {
        Scoreboard.availableBalls = 0;
        Scoreboard.UpdateAvailableBalls();
        Assert.AreEqual("000", GameObject.Find("NumberOfBalls").GetComponent<TextMesh>().text);
    }

    [Test]
    public void correctBallsShownIfLessThan10()
    {
        Scoreboard.availableBalls = 3;
        Scoreboard.UpdateAvailableBalls();
        Assert.AreEqual("003", GameObject.Find("NumberOfBalls").GetComponent<TextMesh>().text);
    }

    [Test]
    public void correctBallsShownIfMoreThan10()
    {
        Scoreboard.availableBalls = 12;
        Scoreboard.UpdateAvailableBalls();
        Assert.AreEqual("012", GameObject.Find("NumberOfBalls").GetComponent<TextMesh>().text);
    }

    [Test]
    public void minusAvailableBalls()
    {
        Scoreboard.availableBalls = 3;
        Scoreboard.MinusAvailableBalls();

        Assert.AreEqual(2, Scoreboard.availableBalls);
    }

    [Test]
    public void minusAvailableBalls_Zero()
    {
        Scoreboard.availableBalls = 1;
        Scoreboard.MinusAvailableBalls();

        Assert.AreEqual(0, Scoreboard.availableBalls);
    }

    [Test]
    public void minusAvailableBalls_Zero_GameOver()
    {
        Scoreboard.availableBalls = 0;
        Scoreboard.MinusAvailableBalls();

        Assert.AreEqual("GAME OVER", GameObject.Find("Game Over").GetComponent<UnityEngine.UI.Text>().text);
    }

    [Test]
    public void minusAvailableBalls_Zero_IsGamePlayable()
    {
        Scoreboard.availableBalls = 0;
        Scoreboard.MinusAvailableBalls();

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
        basketDetected.OnTriggerExit(col);

        Assert.AreEqual("010", GameObject.Find("Score").GetComponent<TextMesh>().text);
    }

    [Test]
    public void preventScore_OnCollision_WhenNoEnterTrigger()
    {
        Collider col = new Collider();
        BasketDetected basketDetected = new BasketDetected();

        startBasketDetected();
        startScoreboard();
        basketDetected.OnTriggerExit(col);

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
        basketDetected.OnTriggerExit(col);

        basketDetected.OnTriggerEnter(col);
        basketDetected.OnTriggerExit(col);

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
        basketDetected.OnTriggerExit(col);

        basketDetected.OnTriggerEnter(col);
        basketDetected.OnTriggerExit(col);

        basketDetected.OnTriggerEnter(col);
        basketDetected.OnTriggerExit(col);

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
        basketDetected.OnTriggerExit(col);

        basketDetected.OnTriggerEnter(col);
        basketDetected.OnTriggerExit(col);

        basketDetected.OnTriggerEnter(col);
        basketDetected.OnTriggerExit(col);

        Assert.AreEqual("120", GameObject.Find("Score").GetComponent<TextMesh>().text);
    }

	//NEED TO MOCK COLLISION OF HITTING FLOOR

    /*[Test]
    public void showCorrectScore_ScoreMissScoreScore()
    {
        Collider col = new Collider();
        Collision collision = new Collision();

        BasketDetected basketDetected = new BasketDetected();
        Basketball basketball = new Basketball();

       // collision.gameObject.name = "Floor";

        startBasketDetected();
        startScoreboard();

        basketDetected.OnTriggerEnter(col);
        basketDetected.OnTriggerExit(col);

        //basketball.OnCollisionEnter(collision);

        basketDetected.OnTriggerEnter(col);
        basketDetected.OnTriggerExit(col);

        basketDetected.OnTriggerEnter(col);
        basketDetected.OnTriggerExit(col);

        Assert.AreEqual("30", GameObject.Find("Score").GetComponent<TextMesh>().text);
    }*/

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
        basketDetected.OnTriggerExit(col);

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
        basketDetected.OnTriggerExit(col);

        basketDetected.OnTriggerEnter(col);
        basketDetected.OnTriggerExit(col);

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
        basketDetected.OnTriggerExit(col);

        basketDetected.OnTriggerEnter(col);
        basketDetected.OnTriggerExit(col);

        basketDetected.OnTriggerEnter(col);
        basketDetected.OnTriggerExit(col);

        Color bonusColour = new Color();
        ColorUtility.TryParseHtmlString("#6BD289FF", out bonusColour);

        Assert.AreEqual(bonusColour, GameObject.Find("Bonus").GetComponent<TextMesh>().color);
    }

    

}

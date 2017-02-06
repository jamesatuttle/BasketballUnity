using NUnit.Framework;
using AssemblyCSharp;
using UnityEngine;

[TestFixture]

public class ScoreboardTests
{
    private Scoreboard scoreboard = new Scoreboard();

    public void startScoreboard()
    {
        var scoreboard = new Scoreboard();
        scoreboard.Start();
    }

    [Test]
    public void playableGameOnStart()
    {
        startScoreboard();
        Assert.AreEqual(true, GamePlay.isGamePlayable);
    }

    [Test]
    public void gameOverTextCLearedOnStart()
    {
        startScoreboard();
        Assert.AreEqual("", GameObject.Find("Game Over").GetComponent<UnityEngine.UI.Text>().text);
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

        Assert.AreEqual(false, GamePlay.isGamePlayable);
    }

    /*[Test]
    public void showCorrectScore_onStart()
    {
        startScoreboard();
        Assert.AreEqual("000", GameObject.Find("Time Remaining").GetComponent<TextMesh>().text);
    }*/
}

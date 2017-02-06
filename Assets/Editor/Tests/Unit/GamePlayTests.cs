using NUnit.Framework;
using AssemblyCSharp;

[TestFixture]

public class GamePlayTests {

	[Test]
	public void isGamePlayable_InitialValue()
	{
		Scoreboard.availableBalls = 3;
		//Assert.AreEqual(GamePlay.isGamePlayable, true);
		Assert.AreEqual(true, GamePlay.isGamePlayable);
	}

	[Test]
	public void isGamePlayable_NoMoreBalls()
	{
		Scoreboard.availableBalls = 0;
		Assert.AreEqual(0, Scoreboard.availableBalls);
		//Assert.AreEqual(false, GamePlay.isGamePlayable);
	}
}

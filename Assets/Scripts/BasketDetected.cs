using UnityEngine;

public class BasketDetected : MonoBehaviour {

	public static int basketCount;
	private bool scored = false;
	private int bonusSetting = 2;

	Scoreboard scoreboard;

    public void Start () {
		basketCount = 0;
    }

    public void OnTriggerEnter (Collider col)
	{
		scored = true;
	}

	public void OnTriggerExit (Collider col) 
	{
		if (scored)
		{
			if (basketCount < bonusSetting) {

				Scoreboard.score = Scoreboard.score + 10;
                scoreboard.updateScore();

                Scoreboard.updateBonusColour("#181717"); // set the bonus text to black

				basketCount++;

				scored = false;

				Basketball.ResetBall ();

			} else if (basketCount == bonusSetting) {

                Scoreboard.score = Scoreboard.score + 100;
				scoreboard.updateScore ();

				Scoreboard.updateBonusColour("#6BD289FF"); // set the bonus text to green

				basketCount = 0;

				scored = false;

				Basketball.ResetBall ();
			}
		}
	}
}

using UnityEngine;

public class BasketDetected : MonoBehaviour {

	public static int basketCount;
	private bool scored = false;
	private int bonusSetting = 2;

    public void Start () {
		basketCount = 0;
    }

	//TODO: Need to fix this!! Is triggered at the start!
    public void OnTriggerEnter (Collider col)
	{
		//print ("triggered score");
		//scored = true;
	}

	public void OnTriggerExit (Collider col) 
	{
		if (scored)
		{
			if (basketCount < bonusSetting) {

				//Scoreboard.score = Scoreboard.score + 10;
				//Scoreboard.updateScore();
				Scoreboard.AddToScore(10);

				Scoreboard.LightUpScoreboardBonus (false); // set the bonus text to black

				basketCount++;

				scored = false;

				Basketball.instance.ResetBall ();

			} else if (basketCount == bonusSetting) {

                //Scoreboard.score = Scoreboard.score + 100;
				//Scoreboard.updateScore ();
				Scoreboard.AddToScore(100);

				Scoreboard.LightUpScoreboardBonus (true); // set the bonus text to green

				basketCount = 0;

				scored = false;

				Basketball.instance.ResetBall ();
			}
		}
	}
}

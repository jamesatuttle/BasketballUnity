using UnityEngine;

public class BasketDetected : MonoBehaviour {

	public static int basketCount;
	private bool scored = false;
	private int bonusSetting = 2;

    public void Start () {
		basketCount = 0;
    }

	void Update () {
		if (scored && GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame)
		{
			scored = false;

			if (basketCount < bonusSetting) {

				Scoreboard.AddToScore(10);

				Scoreboard.LightUpScoreboardBonus (false); // set the bonus text to black

				basketCount++;

			} else if (basketCount == bonusSetting) {

				Scoreboard.AddToScore(100);

				Scoreboard.LightUpScoreboardBonus (true); // set the bonus text to green

				basketCount = 0;
			}

			Basketball.instance.ResetBall ();
		}
	}

    public void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.name == "Basketball") {
			print ("triggered score");
			scored = true;
		}
	}

}

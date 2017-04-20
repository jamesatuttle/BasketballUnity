using UnityEngine;

public class BasketDetected : MonoBehaviour {

	public static int basketCount; //a public integer variable to track the number of repeated baskets
	private bool scored = false; //booean variable to store when a hoop is scored
	private int bonusSetting = 2; //integer variable to store the bonus hoop score threshold

	/*
	* Start is called when the game loads
	* This initialises the repeat basket count to 0
	*/
  public void Start () {
		basketCount = 0;
  }

	/*
	* Update is called once per frame
	* If the user has scored then update the scoreboard as appropriate
	*/
	void Update () {
		if (scored && GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame) //if the scored is true and the main game is being played...
		{
			scored = false; //set scored to false to prevent looping

			if (basketCount < bonusSetting) { //if the current basket count is less than the bonus threshold...

				Scoreboard.AddToScore(10);

				Scoreboard.LightUpScoreboardBonus (false); // set the bonus text to black

				basketCount++; //add one to the basket count

			} else if (basketCount == bonusSetting) { //if the basket count is equal to the bonus setting...

				Scoreboard.AddToScore(100);

				Scoreboard.LightUpScoreboardBonus (true); // set the bonus text to green

				basketCount = 0; //clear the basket count - the bonus count restarts
			}

			Basketball.instance.ResetBall ();
		}
	}

	/*
	* Check if the user has scored a hoop by checking if the basketball has passed through the hoop trigger
	*/
  public void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.name == "Basketball") {
			scored = true; //set scored to true so that the score can be updated
		}
	}

}

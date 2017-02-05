using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketDetected : MonoBehaviour {

	public static int basketCount;
	public int score;
	private bool scored = false;
	private int bonusSetting = 2;

	void Start () {
		basketCount = 0;
		score = 0;
		updateScore ();
		updateColour("#181717"); // set the bonus text to black
	}

	void OnTriggerEnter (Collider col)
	{
		scored = true;
	}

	void OnTriggerExit (Collider col) 
	{
		if (scored)
		{
			if (basketCount < bonusSetting) {
				
				score = score + 10;
				updateScore ();

				updateColour("#181717"); // set the bonus text to black

				basketCount++;

				scored = false;

				//Basketball.ResetBall ();

			} else if (basketCount == bonusSetting) {

				score = score + 100;
				updateScore ();

				updateColour ("#6BD289FF"); // set the bonus text to green

				basketCount = 0;

				scored = false;

				//Basketball.ResetBall ();
			}
		}
	}
		
	void updateScore()
	{
		TextMesh Scoreboard_score = GameObject.Find("Score").GetComponent<TextMesh>();

		if (score >= 100)
			Scoreboard_score.text = score.ToString();
		else if (score >= 10)
			Scoreboard_score.text = "0" + score.ToString();
		else if (score >= 0)
			Scoreboard_score.text = "00" + score.ToString();
	}

	void updateColour(string hex) 
	{
		Color bonusColour = new Color();
		ColorUtility.TryParseHtmlString(hex, out bonusColour);
		GameObject.Find("Bonus").GetComponent<TextMesh>().color=bonusColour;
	}
}

using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Basketball : MonoBehaviour {

	public static Basketball instance;

	public AudioClip bounce;
	static public Vector3 InitialBallPosition = new Vector3(0.02f,5.08f,9.96f);

	void Awake () {
		instance = this;
	}

	void Start () {
		GetComponent<AudioSource> ().playOnAwake = false;
		GetComponent<AudioSource> ().clip = bounce;
	}

	public void OnCollisionEnter (Collision col) {
		try {
			var collision = col.gameObject.name;

			//Debug.Log (col.relativeVelocity.magnitude / 21 * 100 + "%");

			if (collision == "Floor" || collision == "wall" || collision == "wall (1)" || collision == "wall (2)" || collision == "wall (3)" || collision == "Ceiling" ) {
				GetComponent<AudioSource> ().volume = col.relativeVelocity.magnitude/100;
				GetComponent<AudioSource> ().Play ();
			}

			if (collision == "Floor") {
				ResetBall();
				Scoreboard.MinusAvailableBalls();
				BasketDetected.basketCount = 0;
			}
		}
		catch {
			GetComponent<AudioSource> ().volume = 1;
		}
	}

	public void ResetBall() {
		UpdateFixedBasketballPosition(InitialBallPosition.x, InitialBallPosition.y, InitialBallPosition.z);
		KinectController.instance.ClearBallTrackCollection ();
	}

	public void UpdateFixedBasketballPosition(float x, float y, float z)
	{
		GameObject basketball = GameObject.Find ("Basketball");

		LockBasketballPosition (true);
		SetBallGravity(false); //turn gravity off
		basketball.transform.position = new Vector3 (x, y, z);
	}

	public void LockBasketballPosition(bool locked) 
	{
		if (locked)
			GameObject.Find ("Basketball").GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
		else
			GameObject.Find ("Basketball").GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
	}

	public void SetBallGravity(bool gravity) {
		GameObject.Find("Basketball").GetComponent<Rigidbody>().useGravity = gravity;
	}
}

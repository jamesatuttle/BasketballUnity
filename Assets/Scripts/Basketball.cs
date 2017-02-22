﻿using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Basketball : MonoBehaviour {

	public AudioClip bounce;
	static public Vector3 InitialBallPosition = new Vector3(0.02f,5.08f,9.96f);

	void Start () {
		GetComponent<AudioSource> ().playOnAwake = false;
		GetComponent<AudioSource> ().clip = bounce;
	}

	/*void Update() {
		GameObject basketball = GameObject.Find ("Basketball");

		//NEEDS TO CHANGE TO GESTURE RECOGNITION
		if (Input.GetKeyDown ("space") && GamePlay.GameIsPlayable) {
			Debug.Log ("space key was pressed");
			basketball.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
			basketball.GetComponent<Rigidbody> ().useGravity = true; //turn gravity off
		}
	}*/

	public void OnCollisionEnter (Collision col) {
		try {
			var collision = col.gameObject.name;

			Debug.Log (col.relativeVelocity.magnitude / 21 * 100 + "%");

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

	public static void ResetBall() {
		UpdateFixedBasketballPosition(InitialBallPosition.x, InitialBallPosition.y, InitialBallPosition.z);
	}

	public static void UpdateFixedBasketballPosition(float x, float y, float z)
	{
		GameObject basketball = GameObject.Find ("Basketball");
		basketball.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
		basketball.GetComponent<Rigidbody> ().useGravity = false; //turn gravity off

		GameObject.Find ("Basketball").transform.position = new Vector3 (x, y, z);
	}
}

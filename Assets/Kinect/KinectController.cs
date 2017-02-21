using UnityEngine;
using System;
using UnityEngine.UI;

public class KinectController : MonoBehaviour {

  	Vector3 HandLeft;
  	Vector3 HandRight;

  	bool ballIsHeld;

	float ballWidth = 0.26f; //Ball Width is 0.26 cm
	float inch = 0.0254f; //1 inch equals 2.5 cm.

	Vector3 callibratedPosition;

	Vector3 newBallPosition;
	 
  	void Start() {
		ballIsHeld = false;

		KinectManager manager = KinectManager.Instance;

		if (!manager.IsUserDetected()) {
			Debug.Log ("User not detected");
		}
  	}

  	void Update() {

		KinectManager manager = KinectManager.Instance;

		if (manager.IsUserDetected()) {

			uint userId = manager.GetPlayer1ID ();

			HandLeft = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
			HandRight = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight);
		
			PrintHandPoints ();
		}
  	}
		
	public void PrintHandPoints() {

		var HandDifference = -HandLeft.x - -HandRight.x;  //These x values are negative, the minus sets them positive

		if (ballIsHeld) {
			//Debug.Log ("***** Picked up ball");
			GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Picked up ball";


			if (HandDifference > ballWidth + (inch * 10)) {
				//Debug.Log ("Dropped ball");
				GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Dropped ball";

				GameObject.Find ("Basketball").GetComponent<Rigidbody> ().useGravity = true; //turn gravity on

				ballIsHeld = false;

			} else {
				moveBall ();
				GameObject.Find ("Basketball").transform.position = new Vector3 (newBallPosition.x, newBallPosition.y, newBallPosition.z);
			}
		}
		else {
			//Debug.Log ("HandLeft: " + -HandLeft.x);
			//Debug.Log ("HandRight: " + -HandRight.x);

			//Debug.Log ("HandDifference: " + HandDifference + "\n");

			var minimumBallWidth = ballWidth - inch;
			var maximumBallWidth = ballWidth + inch;

			bool handsInDistanceToPickUpBall = HandDifference > minimumBallWidth && HandDifference < maximumBallWidth;

			if (handsInDistanceToPickUpBall) {
				ballIsHeld = true;
				callibrateUser ();
			}
		}
			
	}

	private void callibrateUser() {
		KinectManager manager = KinectManager.Instance;

		uint userId = manager.GetPlayer1ID ();

		/*Vector3 ElbowLeft = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft);
		Vector3 ElbowRight = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight);*/

		callibratedPosition.x = (HandLeft.x + HandRight.x) / 2;
		callibratedPosition.y = (HandLeft.y + HandRight.y) / 2;
		callibratedPosition.z = (HandLeft.z + HandRight.z) / 2;

		Debug.Log ("startBallPosition: " + callibratedPosition); 
	}

	private void moveBall() {

		float HandsX = (HandLeft.x + HandRight.x) / 2;
		float HandsY = (HandLeft.y + HandRight.y) / 2;
		float HandsZ = (HandLeft.z + HandRight.z) / 2;

		var XMovement = HandsX - callibratedPosition.x;
		var YMovement = HandsY - callibratedPosition.y;
		var ZMovement = HandsZ - callibratedPosition.z;

		newBallPosition.x = Basketball.InitialBallPosition.x + XMovement;
		newBallPosition.y = Basketball.InitialBallPosition.y + YMovement;
		newBallPosition.z = Basketball.InitialBallPosition.z + ZMovement;

		GameObject.Find ("Basketball").GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;

	}

}

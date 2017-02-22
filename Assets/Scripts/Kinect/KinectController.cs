using UnityEngine;
using System;
using UnityEngine.UI;

public class KinectController : MonoBehaviour
{
	Vector3 HandLeft;
	Vector3 HandRight;
	Vector3 Head;
	Vector3 HipCenter;

	bool ballIsHeld;

	float ballWidth = 0.26f; //Ball Width is 0.26 cm
	float inch = 0.0254f; //1 inch equals 2.5 cm.

	Vector3 HandsCallibratedPosition;
	Vector3 HeadCallibratedPosition;

	void Start ()
	{  
		ballIsHeld = false;
	}

	void Update ()
	{ 
		try {
			KinectManager manager = KinectManager.Instance;

			if (manager.IsUserDetected () && GamePlay.GameIsPlayable) {

				uint userId = manager.GetPlayer1ID ();

				HandLeft = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
				HandRight = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight);

				Head = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.Head);

				HipCenter = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter);

				BallPickUpController ();
			}
		} catch (Exception e) {
			Debug.Log ("An error occured: " + e.Message);
		}
	}

	private void BallPickUpController ()
	{

		var HandDifference = -HandLeft.x - -HandRight.x;  //These x values are negative, the minus sets them positive

		if (ballIsHeld) {
			GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Picked up ball";

			if (HandDifference > ballWidth + (inch * 3)) { //check they haven't dropped the ball

				GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Dropped ball";

				Basketball.setBallGravity (true); //turn gravity on

				ballIsHeld = false;

			} else {
				moveBall ();
				moveMainCamera ();
			}
		} else {

			var minimumBallWidth = ballWidth - inch;
			var maximumBallWidth = ballWidth + inch;

			bool handsInDistanceToPickUpBall = HandDifference > minimumBallWidth && HandDifference < maximumBallWidth;

			bool handsInFrontOfBody = HipCenter.z > HandLeft.z && HipCenter.z > HandRight.z;

			if (handsInDistanceToPickUpBall && handsInFrontOfBody) {
				callibrateUser ();
				ballIsHeld = true;
			}
		}
	}

	private void callibrateUser ()
	{
		KinectManager manager = KinectManager.Instance;

		uint userId = manager.GetPlayer1ID ();

		HandsCallibratedPosition.x = (HandLeft.x + HandRight.x) / 2;
		HandsCallibratedPosition.y = (HandLeft.y + HandRight.y) / 2;
		HandsCallibratedPosition.z = (HandLeft.z + HandRight.z) / 2;

		HeadCallibratedPosition.x = Head.x;
		HeadCallibratedPosition.y = Head.y;
		HeadCallibratedPosition.z = Head.z;
	}

	private void moveBall ()
	{
		int movementSensitivity = 3;

		Vector3 newBallPosition;

		float HandsX = (HandLeft.x + HandRight.x) / 2;
		float HandsY = (HandLeft.y + HandRight.y) / 2;
		float HandsZ = (HandLeft.z + HandRight.z) / 2;

		var XMovement = HandsX - HandsCallibratedPosition.x;
		var YMovement = HandsY - HandsCallibratedPosition.y;
		var ZMovement = HandsZ - HandsCallibratedPosition.z;

		newBallPosition.x = Basketball.InitialBallPosition.x + (XMovement * movementSensitivity);
		newBallPosition.y = Basketball.InitialBallPosition.y + (YMovement * movementSensitivity);
		newBallPosition.z = Basketball.InitialBallPosition.z - (ZMovement * movementSensitivity);

		GameObject.Find ("Basketball").GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;

		GameObject.Find ("Basketball").transform.position = new Vector3 (newBallPosition.x, newBallPosition.y, newBallPosition.z);
	}

	private void moveMainCamera()
	{
		int movementSensitivity = 3;

		Vector3 newCameraPosition;

		var XMovement = Head.x - HeadCallibratedPosition.x;
		var YMovement = Head.y - HeadCallibratedPosition.y;
		var ZMovement = Head.z - HeadCallibratedPosition.z;

		newCameraPosition.x = Cameras.MainCameraPosition.x + (XMovement * movementSensitivity);
		newCameraPosition.y = Cameras.MainCameraPosition.y + (YMovement * movementSensitivity);
		newCameraPosition.z = Cameras.MainCameraPosition.z - (ZMovement * movementSensitivity);

		Cameras.UpdateCameraPosition (newCameraPosition.x, newCameraPosition.y, newCameraPosition.z, 34);
	}

}

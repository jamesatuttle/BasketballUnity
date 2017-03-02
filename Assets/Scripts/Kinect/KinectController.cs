using UnityEngine;
using System;
using UnityEngine.UI;

public class KinectController : MonoBehaviour
{
	Vector3 HandLeft;
	Vector3 HandRight;
	Vector3 Head;
	Vector3 HipCenter;
	Vector3 ShoulderLeft;
	Vector3 ShoulderRight;

	bool ballIsHeld;

	float ballWidth = 0.26f; //Ball Width is 0.26 cm
	float inch = 0.0254f; //1 inch equals 2.5 cm.

	Vector3 HandsCallibratedPosition;
	Vector3 HeadCallibratedPosition;

	float HandDifference;

	void Start ()
	{  
		ballIsHeld = false;
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "";
	}

	void Update ()
	{ 
		try {

			if (GamePlay.GameIsPlayable) {

				KinectManager manager = KinectManager.Instance;

				if (manager.IsUserDetected ()) {

					uint userId = manager.GetPlayer1ID ();

					HandLeft = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
					HandRight = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight);
					Head = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.Head);
					HipCenter = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter);
					ShoulderLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
					ShoulderRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);

					BasketballController ();

				} else {
					GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Stand in front of sensor";
				}
			}
		} catch (Exception e) {
			GamePlay.GameIsPlayable = false;
			Debug.Log ("An error occured: " + e.Message);
		}
	}

	private void BasketballController ()
	{
		HandDifference = -HandLeft.x - -HandRight.x;  //These x values are negative, the minus sets them positive

		if (ballIsHeld) {

			GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Picked up ball";

			if (IsBallDropped()) { //check they haven't dropped the ball
				dropBall ();
			} else {
				moveBall ();
				moveMainCamera ();
			}

		} else {

			if (IsBallPickedUp()) {
				callibrateUser ();
				ballIsHeld = true;
			}
		}
	}

	private bool IsBallPickedUp()
	{
		bool handsInDistanceToPickUpBall = HandDifference > (ballWidth - (inch * 2)) && HandDifference < (ballWidth + (inch * 2));
		bool handsInFrontOfBody = HipCenter.z > HandLeft.z && HipCenter.z > HandRight.z;

		if (handsInDistanceToPickUpBall && handsInFrontOfBody)
			return true;
		else
			return false;
	}

	private bool IsBallDropped() 
	{
		if (HandDifference > ballWidth + (inch * 3))
			return true;
		else
			return false;
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

	private void dropBall()
	{
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Dropped ball";

		Basketball.SetBallGravity (true); //turn gravity on

		ballIsHeld = false;
	}

	private void moveBall ()
	{
		int movementSensitivity = 3;

		Vector3 newBallPosition;

		float HandsX = (HandLeft.x + HandRight.x) / 2;
		float HandsY = (HandLeft.y + HandRight.y) / 2;
		float HandsZ = (HandLeft.z + HandRight.z) / 2;

		// average the hands movement here

		var XMovement = HandsX - HandsCallibratedPosition.x;
		var YMovement = HandsY - HandsCallibratedPosition.y;
		var ZMovement = HandsZ - HandsCallibratedPosition.z;

		newBallPosition.x = Basketball.InitialBallPosition.x + (XMovement * movementSensitivity);
		newBallPosition.y = Basketball.InitialBallPosition.y + (YMovement * movementSensitivity);
		newBallPosition.z = Basketball.InitialBallPosition.z - (ZMovement * movementSensitivity);

		Basketball.LockBasketballPosition (false);

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

	/*
	 * Temp method to add the required skeletal differences to the database
	 */
	private void collectSkeletalDifferences(Vector3 HandLeft, Vector3 HandRight, Vector3 HipCentre)
	{

	}
}

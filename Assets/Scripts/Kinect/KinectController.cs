using UnityEngine;
using System;
using UnityEngine.UI;

public class KinectController : MonoBehaviour
{
	private Vector3 HandLeft;
	private Vector3 HandRight;

	private Vector3 Head;

	private Vector3 HipCenter;

	private Vector3 ShoulderLeft;
	private Vector3 ShoulderRight;

	private Vector3 WristLeft;
	private Vector3 WristRight;

	private Vector3 ElbowLeft;
	private Vector3 ElbowRight;

	private bool BallIsHeld;

	private float BallWidth = 0.26f; //Ball Width is 0.26 cm
	private float Inch = 0.0254f; //1 inch equals 2.5 cm.

	private Vector3 HandsCallibratedPosition;
	private Vector3 HeadCallibratedPosition;

	private float HandDifference;

	void Start () {  
		BallIsHeld = false;
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "";
		ANN_CPU.InitialiseANN ();
	}
		
	void Update () { 
		try {

			if (GamePlay.GameIsPlayable) {

				KinectManager manager = KinectManager.Instance;

				if (manager.IsUserDetected ()) {

					uint userId = manager.GetPlayer1ID ();

					HandLeft = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
					HandRight = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight);
					WristLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft);
					WristRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.WristRight);
					Head = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.Head);
					HipCenter = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter);
					ShoulderLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
					ShoulderRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);
					ElbowLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft);
					ElbowRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight);

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

	private void BasketballController () {
		HandDifference = -HandLeft.x - -HandRight.x;  //These x values are negative, the minus sets them positive

		if (BallIsHeld) {

			GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Picked up ball";

			if (IsBallDropped()) { //check they haven't dropped the ball
				dropBall ();
			} else {
				collectSkeletalDifferences ();
				moveBall ();
				moveMainCamera ();
			}

		} else {

			if (IsBallPickedUp()) {
				callibrateUser ();
				BallIsHeld = true;
			}
		}
	}

	private bool IsBallPickedUp() {
		
		bool handsInDistanceToPickUpBall = HandDifference > (BallWidth - (Inch * 2)) && HandDifference < (BallWidth + (Inch * 2));
		bool handsInFrontOfBody = HipCenter.z > HandLeft.z && HipCenter.z > HandRight.z;

		if (handsInDistanceToPickUpBall && handsInFrontOfBody)
			return true;
		else
			return false;
	}

	private bool IsBallDropped() {
		
		if (HandDifference > (BallWidth + (Inch * 3)))
			return true;
		else
			return false;
	}

	private void callibrateUser () {
		HandsCallibratedPosition.x = (HandLeft.x + HandRight.x) / 2;
		HandsCallibratedPosition.y = (HandLeft.y + HandRight.y) / 2;
		HandsCallibratedPosition.z = (HandLeft.z + HandRight.z) / 2;

		HeadCallibratedPosition.x = Head.x;
		HeadCallibratedPosition.y = Head.y;
		HeadCallibratedPosition.z = Head.z;
	}

	private void dropBall() {
		
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Dropped ball";

		Basketball.SetBallGravity (true); //turn gravity on

		BallIsHeld = false;
	}

	private void moveBall () {
		
		int movementSensitivity = 3;

		Vector3 newBallPosition;

		float HandsX = (HandLeft.x + HandRight.x) / 2;
		float HandsY = (HandLeft.y + HandRight.y) / 2;
		float HandsZ = (HandLeft.z + HandRight.z) / 2;

		// average the hands movement here
		Vector3 HandsXYZ = new Vector3(HandsX, HandsY, HandsZ);

		var XMovement = HandsX - HandsCallibratedPosition.x;
		var YMovement = HandsY - HandsCallibratedPosition.y;
		var ZMovement = HandsZ - HandsCallibratedPosition.z;

		newBallPosition.x = Basketball.InitialBallPosition.x + (XMovement * movementSensitivity);
		newBallPosition.y = Basketball.InitialBallPosition.y + (YMovement * movementSensitivity);
		newBallPosition.z = Basketball.InitialBallPosition.z - (ZMovement * movementSensitivity);

		Basketball.LockBasketballPosition (false);

		GameObject.Find ("Basketball").transform.position = newBallPosition;
	}

	private void moveMainCamera() {
		int movementSensitivity = 3;

		Vector3 newCameraPosition;

		var xMovement = Head.x - HeadCallibratedPosition.x;
		var yMovement = Head.y - HeadCallibratedPosition.y;
		var zMovement = Head.z - HeadCallibratedPosition.z;

		newCameraPosition.x = Cameras.MainCameraPosition.x + (xMovement * movementSensitivity);
		newCameraPosition.y = Cameras.MainCameraPosition.y + (yMovement * movementSensitivity);
		newCameraPosition.z = Cameras.MainCameraPosition.z - (zMovement * movementSensitivity);

		Cameras.UpdateCameraPosition (newCameraPosition.x, newCameraPosition.y, newCameraPosition.z, 34);
	}

	private void collectSkeletalDifferences() {

		double rightHand_HipX = HandRight.x - HipCenter.x;
		double rightHand_HipY = HandRight.y - HipCenter.y;
		double rightHand_HipZ = HandRight.z - HipCenter.z;

		double rightHand_RightWristX = HandRight.x - WristRight.x;
		double rightHand_RightWristY = HandRight.y - WristRight.y;
		double rightHand_RightWristZ = HandRight.z - WristRight.z;

		double rightWrist_RightElbowX = WristRight.x - ElbowRight.x;
		double rightWrist_RightElbowY = WristRight.y - ElbowRight.y;
		double rightWrist_RightElbowZ = WristRight.z - ElbowRight.z;

		double rightElbow_RightShoulderX = ElbowRight.x - ShoulderRight.x;
		double rightElbow_RightShoulderY = ElbowRight.y - ShoulderRight.y;
		double rightElbow_RightShoulderZ = ElbowRight.z - ShoulderRight.z;

		double rightHand_RightShoulderX = HandRight.x - ShoulderRight.x;
		double rightHand_RightShoulderY = HandRight.y - ShoulderRight.y;
		double rightHand_RightShoulderZ = HandRight.z - ShoulderRight.z;

		double leftHand_HipX = HandLeft.x - HipCenter.x;
		double leftHand_HipY = HandLeft.y - HipCenter.y;
		double leftHand_HipZ = HandLeft.z - HipCenter.z;

		double leftHand_LeftWristX = HandLeft.x - WristLeft.x;
		double leftHand_LeftWristY = HandLeft.y - WristLeft.y;
		double leftHand_LeftWristZ = HandLeft.z - WristLeft.z;

		double leftWrist_LeftElbowX = WristLeft.x - ElbowLeft.x;
		double leftWrist_LeftElbowY = WristLeft.y - ElbowLeft.y;
		double leftWrist_LeftElbowZ = WristLeft.z - ElbowLeft.z;

		double leftElbow_LeftShoulderX = ElbowLeft.x - ShoulderLeft.x;
		double leftElbow_LeftShoulderY = ElbowLeft.y - ShoulderLeft.y;
		double leftElbow_LeftShoulderZ = ElbowLeft.z - ShoulderLeft.z;

		double leftHand_LeftShoulderX = HandLeft.x - ShoulderLeft.x;
		double leftHand_LeftShoulderY = HandLeft.y - ShoulderLeft.y;
		double leftHand_LeftShoulderZ = HandLeft.z - ShoulderLeft.z;

		double[] trackedSkeletalPoints = new double[30] 
		{ 
			rightHand_HipX, rightHand_HipY, rightHand_HipZ, rightHand_RightWristX, rightHand_RightWristY, rightHand_RightWristZ, rightWrist_RightElbowX, rightWrist_RightElbowY, rightWrist_RightElbowZ, rightElbow_RightShoulderX, rightElbow_RightShoulderY, rightElbow_RightShoulderZ, rightHand_RightShoulderX, rightHand_RightShoulderY, rightHand_RightShoulderZ,
			leftHand_HipX, leftHand_HipY, leftHand_HipZ, leftHand_LeftWristX, leftHand_LeftWristY, leftHand_LeftWristZ, leftWrist_LeftElbowX, leftWrist_LeftElbowY, leftWrist_LeftElbowZ, leftElbow_LeftShoulderX, leftElbow_LeftShoulderY, leftElbow_LeftShoulderZ, leftHand_LeftShoulderX, leftHand_LeftShoulderY, leftHand_LeftShoulderZ
		};

		/*
		 * Stationary
		 * Professional
		 * Chest
		 * Low
		 */
		//string gesture = "Professional";

		//AddToDatabase.addToANNTrainingData (
		//	formatSkelValue(RightHand_HipX), formatSkelValue(RightHand_HipY), formatSkelValue(RightHand_HipZ),
			/*RightHand_RightWristX, RightHand_RightWristY, RightHand_RightWristZ,
			RightWrist_RightElbowX, RightWrist_RightElbowY, RightWrist_RightElbowZ,
			RightElbow_RightShoulderX, RightElbow_RightShoulderY, RightElbow_RightShoulderZ,
			RightHand_RightShoulderX, RightHand_RightShoulderY, RightHand_RightShoulderZ,*/

		//	formatSkelValue(LeftHand_HipX), formatSkelValue(LeftHand_HipY), formatSkelValue(LeftHand_HipZ),
			/*LeftHand_LeftWristX, LeftHand_LeftWristY, LeftHand_LeftWristZ,
			LeftWrist_LeftElbowX, LeftWrist_LeftElbowY, LeftWrist_LeftElbowZ,
			LeftElbow_LeftShoulderX, LeftElbow_LeftShoulderY, LeftElbow_LeftShoulderZ,
			LeftHand_LeftShoulderX, LeftHand_LeftShoulderY, LeftHand_LeftShoulderZ,*/

			/*gesture
		);*/

		ANN_CPU.StartANN (trackedSkeletalPoints);
	}
}

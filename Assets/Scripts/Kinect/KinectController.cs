using UnityEngine;
using System;
using UnityEngine.UI;

public class KinectController : MonoBehaviour
{
	Vector3 HandLeft;
	Vector3 HandRight;

	Vector3 Head;

	Vector3 HipCenter;

	/*Vector3 ShoulderLeft;
	Vector3 ShoulderRight;

	Vector3 WristLeft;
	Vector3 WristRight;

	Vector3 ElbowLeft;
	Vector3 ElbowRight;*/

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
		//ANN_CPU.TestANNClassifier ();
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
					//WristLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft);
					//WristRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.WristRight);
					Head = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.Head);
					HipCenter = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter);
					//ShoulderLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
					//ShoulderRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);
					//ElbowLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft);
					//ElbowRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight);

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
				collectSkeletalDifferences ();
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

	private void collectSkeletalDifferences()
	{
		double RightHand_HipX = HandRight.x - HipCenter.x;
		double RightHand_HipY = HandRight.y - HipCenter.y;
		double RightHand_HipZ = HandRight.z - HipCenter.z;
		/*double RightHand_RightWristX = HandRight.x - WristRight.x;
		double RightHand_RightWristY = HandRight.y - WristRight.y;
		double RightHand_RightWristZ = HandRight.z - WristRight.z;
		double RightWrist_RightElbowX = WristRight.x - ElbowRight.x;
		double RightWrist_RightElbowY = WristRight.y - ElbowRight.y;
		double RightWrist_RightElbowZ = WristRight.z - ElbowRight.z;
		double RightElbow_RightShoulderX = ElbowRight.x - ShoulderRight.x;
		double RightElbow_RightShoulderY = ElbowRight.y - ShoulderRight.y;
		double RightElbow_RightShoulderZ = ElbowRight.z - ShoulderRight.z;
		double RightHand_RightShoulderX = HandRight.x - ShoulderRight.x;
		double RightHand_RightShoulderY = HandRight.y - ShoulderRight.y;
		double RightHand_RightShoulderZ = HandRight.z - ShoulderRight.z;*/

		double LeftHand_HipX = HandLeft.x - HipCenter.x;
		double LeftHand_HipY = HandLeft.y - HipCenter.y;
		double LeftHand_HipZ = HandLeft.z - HipCenter.z;
		/*double LeftHand_LeftWristX = HandLeft.x - WristLeft.x;
		double LeftHand_LeftWristY = HandLeft.y - WristLeft.y;
		double LeftHand_LeftWristZ = HandLeft.z - WristLeft.z;
		double LeftWrist_LeftElbowX = WristLeft.x - ElbowLeft.x;
		double LeftWrist_LeftElbowY = WristLeft.y - ElbowLeft.y;
		double LeftWrist_LeftElbowZ = WristLeft.z - ElbowLeft.z;
		double LeftElbow_LeftShoulderX = ElbowLeft.x - ShoulderLeft.x;
		double LeftElbow_LeftShoulderY = ElbowLeft.y - ShoulderLeft.y;
		double LeftElbow_LeftShoulderZ = ElbowLeft.z - ShoulderLeft.z;
		double LeftHand_LeftShoulderX = HandLeft.x - ShoulderLeft.x;
		double LeftHand_LeftShoulderY = HandLeft.y - ShoulderLeft.y;
		double LeftHand_LeftShoulderZ = HandLeft.z - ShoulderLeft.z;*/


		double[] trackedSkeletalPoints = new double[Database.ReturnNoInputs()];

		trackedSkeletalPoints [0] = RightHand_HipX;
		trackedSkeletalPoints [1] = RightHand_HipY;
		trackedSkeletalPoints [2] = RightHand_HipZ;
		/*trackedSkeletalPoints [3] = RightHand_RightWristX;
		trackedSkeletalPoints [4] = RightHand_RightWristY;
		trackedSkeletalPoints [5] = RightHand_RightWristZ;
		trackedSkeletalPoints [6] = RightWrist_RightElbowY;
		trackedSkeletalPoints [7] = RightWrist_RightElbowY;
		trackedSkeletalPoints [8] = RightWrist_RightElbowZ;
		trackedSkeletalPoints [9] = RightElbow_RightShoulderX;
		trackedSkeletalPoints [10] = RightElbow_RightShoulderY;
		trackedSkeletalPoints [11] = RightElbow_RightShoulderZ;
		trackedSkeletalPoints [12] = RightHand_RightShoulderX;
		trackedSkeletalPoints [13] = RightHand_RightShoulderY;
		trackedSkeletalPoints [14] = RightHand_RightShoulderZ;*/
		trackedSkeletalPoints [3] = LeftHand_HipX;
		trackedSkeletalPoints [4] = LeftHand_HipY;
		trackedSkeletalPoints [5] = LeftHand_HipZ;
		/*trackedSkeletalPoints [18] = LeftHand_LeftWristX;
		trackedSkeletalPoints [19] = LeftHand_LeftWristY;
		trackedSkeletalPoints [20] = LeftHand_LeftWristZ;
		trackedSkeletalPoints [21] = LeftWrist_LeftElbowY;
		trackedSkeletalPoints [22] = LeftWrist_LeftElbowY;
		trackedSkeletalPoints [23] = LeftWrist_LeftElbowZ;
		trackedSkeletalPoints [24] = LeftElbow_LeftShoulderX;
		trackedSkeletalPoints [25] = LeftElbow_LeftShoulderY;
		trackedSkeletalPoints [26] = LeftElbow_LeftShoulderZ;
		trackedSkeletalPoints [27] = LeftHand_LeftShoulderX;
		trackedSkeletalPoints [28] = LeftHand_LeftShoulderY;
		trackedSkeletalPoints [29] = LeftHand_LeftShoulderZ;*/

		/*
		 * Stationary
		 * Professional
		 * Chest
		 * Low
		 */
		/*string gesture = "Professional";

		AddToDatabase.addToANNTrainingData (
			RightHand_HipX, RightHand_HipY, RightHand_HipZ,
			/*RightHand_RightWristX, RightHand_RightWristY, RightHand_RightWristZ,
			RightWrist_RightElbowX, RightWrist_RightElbowY, RightWrist_RightElbowZ,
			RightElbow_RightShoulderX, RightElbow_RightShoulderY, RightElbow_RightShoulderZ,
			RightHand_RightShoulderX, RightHand_RightShoulderY, RightHand_RightShoulderZ,*/

		//	LeftHand_HipX, LeftHand_HipY, LeftHand_HipZ,
			/*LeftHand_LeftWristX, LeftHand_LeftWristY, LeftHand_LeftWristZ,
			LeftWrist_LeftElbowX, LeftWrist_LeftElbowY, LeftWrist_LeftElbowZ,
			LeftElbow_LeftShoulderX, LeftElbow_LeftShoulderY, LeftElbow_LeftShoulderZ,
			LeftHand_LeftShoulderX, LeftHand_LeftShoulderY, LeftHand_LeftShoulderZ,*/

		/*	gesture
		);*/

		ANN_CPU.StartANN (trackedSkeletalPoints);

	}
}

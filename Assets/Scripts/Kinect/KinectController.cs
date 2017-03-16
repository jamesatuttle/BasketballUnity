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

	//Hand smoothing
	private int _handTrackingCount;
	private static Vector3[] _handCollection = new Vector3[15];
	private static Vector3[] _handMovementDifferences = new Vector3[_handCollection.Length - 1];
	private double _handXAverage;
	private double _handYAverage;
	private double _handZAverage;

	//Head smoothing
	private int _headTrackingCount;
	private static Vector3[] _headCollection = new Vector3[15];
	private static Vector3[] _headMovementDifferences = new Vector3[_headCollection.Length - 1];
	private double _headXAverage;
	private double _headYAverage;
	private double _headZAverage;

	//Test smoothing
	private int _testTrackingCount;
	private static Vector3[] _testCollection = new Vector3[15];
	private static Vector3[] _testMovementDifferences = new Vector3[_testCollection.Length - 1];
	private double _testXAverage;
	private double _testYAverage;
	private double _testZAverage;

	enum Tracking {
		hands = 0,
		head = 1,
		testing = 3
	}

	void Start () {  
		BallIsHeld = false;
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "";
		ANN_CPU.InitialiseANN ();
		_handTrackingCount = 0;
		_headTrackingCount = 0;
		_testTrackingCount = 0;
		_handXAverage = 0.0;
		_handYAverage = 0.0;
		_handZAverage = 0.0;
		_headXAverage = 0.0;
		_headYAverage = 0.0;
		_headZAverage = 0.0;
		_testXAverage = 0.0;
		_testYAverage = 0.0;
		_testZAverage = 0.0;

		//TestSmoothing ();
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

		float handsX = (HandLeft.x + HandRight.x) / 2;
		float handsY = (HandLeft.y + HandRight.y) / 2;
		float handsZ = (HandLeft.z + HandRight.z) / 2;

		Vector3 hands = new Vector3 (handsX, handsY, handsZ);

		// average the hands movement here
		//hands = SmoothValues(hands, Tracking.hands);

		/*var xMovement = handsX - HandsCallibratedPosition.x;
		var yMovement = handsY - HandsCallibratedPosition.y; 
		var zMovement = handsZ - HandsCallibratedPosition.z;*/

		Vector3 movement = new Vector3 (handsX - HandsCallibratedPosition.x, handsY - HandsCallibratedPosition.y, handsZ - HandsCallibratedPosition.z);

		/*newBallPosition.x = Basketball.InitialBallPosition.x + (xMovement * movementSensitivity);
		newBallPosition.y = Basketball.InitialBallPosition.y + (yMovement * movementSensitivity);
		newBallPosition.z = Basketball.InitialBallPosition.z - (zMovement * movementSensitivity); //minus to move ball in the correct view of the user*/

		newBallPosition.x = Basketball.InitialBallPosition.x + (movement.x * movementSensitivity);
		newBallPosition.y = Basketball.InitialBallPosition.y + (movement.y * movementSensitivity);
		newBallPosition.z = Basketball.InitialBallPosition.z - (movement.z * movementSensitivity); //minus to move ball in the correct view of the user

		Basketball.LockBasketballPosition (false);

		GameObject.Find ("Basketball").transform.position = newBallPosition;
	}

	private Vector3 SmoothValues(Vector3 rawValues, Tracking tracking) {

		int count = 0;
		Vector3[] collection = new Vector3[15];
		Vector3[] differences = new Vector3[collection.Length - 1];
		double xAverage = 0.0;
		double yAverage = 0.0;
		double zAverage = 0.0;

		double xTotal = 0.0;
		double yTotal = 0.0;
		double zTotal = 0.0;

		switch (tracking) {
		case Tracking.hands:
			count = _handTrackingCount;
			collection = _handCollection;
			differences = _handMovementDifferences;
			xAverage = _handXAverage;
			yAverage = _handYAverage;
			zAverage = _handZAverage;
			break;
		case Tracking.head:
			count = _headTrackingCount;
			collection = _headCollection;
			differences = _headMovementDifferences;
			xAverage = _headXAverage;
			yAverage = _headYAverage;
			zAverage = _headZAverage;
			break;
		case Tracking.testing:
			count = _testTrackingCount;
			collection = _testCollection;
			differences = _testMovementDifferences;
			xAverage = _testXAverage;
			yAverage = _testYAverage;
			zAverage = _testZAverage;
			break;
		}

		print ("count: " + count);

		if (count == 0) {
			collection [count] = rawValues;
			count++;

		} else if (count < collection.Length) {
			collection [count] = rawValues;

			/*for (int i = 0; i < collection.Length; i++) {
				print ("collection " + i + ": " + collection [i].ToString ());
			}

			print ("");*/

			for (int i = 1; i < count + 1; i++) {
				differences [i-1].x = collection [i].x - collection [i - 1].x;
				differences [i-1].y = collection [i].y - collection [i - 1].y;
				differences [i-1].z = collection [i].z - collection [i - 1].z;
			}
				
			//eliminate extraneous values

			for (int i = 0; i < differences.Length; i++) {
				if (differences [i].x != 0.0 && differences [i].y != 0.0 && differences [i].z != 0.0) {
					//print ("differences " + i + ": " + differences [i].ToString ());

					Vector3 previousTotals = new Vector3 ((float)xTotal, (float)yTotal, (float)zTotal);
					Vector3 previousAverage = new Vector3 ((float)xTotal / i, (float)yTotal / i, (float)zTotal / i);

					xTotal += differences [i].x;
					yTotal += differences [i].y;
					zTotal += differences [i].z;

					//print ("Total: " + xTotal + ", " + yTotal + ", " + zTotal);

					Vector3 newAverages = new Vector3 ((float)xTotal / (i + 1), (float)yTotal / (i + 1), (float)zTotal / (i + 1));

					//print ("Average: " + newAverages.x + ", " + newAverages.y + ", " + newAverages.z);

					if (!WithinBoundaryOfAverage (previousAverage.x, newAverages.x)) {
						xTotal -= differences [i].x;
						xTotal += previousAverage.x;
						collection [count].x = collection [count - 1].x + previousAverage.x;
					}
					if (!WithinBoundaryOfAverage (previousAverage.y, newAverages.y)) {
						yTotal -= differences [i].y;
						yTotal += previousAverage.y;
						collection [count].y = collection [count - 1].y + previousAverage.y;
					}
					if (!WithinBoundaryOfAverage (previousAverage.z, newAverages.z)) {
						zTotal -= differences [i].z;
						zTotal += previousAverage.z;
						collection [count].z = collection [count - 1].z + previousAverage.z;
					}

					Vector3 processedAverages = new Vector3 ((float)xTotal / (i + 1), (float)yTotal / (i + 1), (float)zTotal / (i + 1));

					//print ("processedAverages: " + processedAverages.x + ", " + processedAverages.y + ", " + processedAverages.z);
				}
			}

			/*for (int i = 0; i < collection.Length; i++) {
				print ("filtered collection " + i + ": " + collection [i].ToString ());
			}

			print ("");
			print ("");*/
				
			count++;

		} else {
			count = 0;

			for (int i = 0; i < collection.Length; i++)
				collection [i] = new Vector3 (0, 0, 0);

			for (int i = 0; i < differences.Length; i++)
				differences [i] = new Vector3 (0, 0, 0);
		}

		switch (tracking) {
		case Tracking.hands:
			_handTrackingCount = count;
			_handCollection = collection;
			_handMovementDifferences = differences;
			_handXAverage = xAverage;
			_handYAverage = yAverage;
			_handZAverage = zAverage;
			return _handCollection[_handTrackingCount];
			break;
		case Tracking.head:
			_headTrackingCount = count;
			_headCollection = collection;
			_headMovementDifferences = differences;
			_headXAverage = xAverage;
			_headYAverage = yAverage;
			_headZAverage = zAverage;
			return _headCollection [_headTrackingCount];
			break;
		case Tracking.testing:
			_testTrackingCount = count;
			_testCollection = collection;
			_testMovementDifferences = differences;
			_testXAverage = xAverage;
			_testYAverage = yAverage;
			_testZAverage = zAverage;
			return _testCollection [_testTrackingCount];
			break;
		default:
			return collection [count];
		}
	}

	private bool WithinBoundaryOfAverage(double oldAverage, double newAverage) {

		double tenPercent = oldAverage * 0.1f;

		if (newAverage != oldAverage) {
			if (newAverage > oldAverage + tenPercent) {
				return false;
			} else if (newAverage < oldAverage - tenPercent) {
				return false;
			} else
				return true;
		} else
			return true;
	}

	private void TestSmoothing() {

		print ("TestSmoothing");

		Vector3[] test = new Vector3[] {
			new Vector3 (1, 1, 1), 
			new Vector3 (2, 2, 2),
			new Vector3 (3, 3, 3), 
			new Vector3 (4, 4, 4), 
			new Vector3 (5, 5, 5),
			new Vector3 (10, 6, 6),
			new Vector3 (1, 3, 4),
			new Vector3 (8, 8, 8),
			new Vector3 (9.1f, 9, 9),
			new Vector3 (12, 12, 12),
			new Vector3 (12, 12, 12),
			new Vector3 (12, 12, 12)
		};

		for (int i = 0; i < test.Length; i++) {
			print ("Before smoothing " + i + ": " + test [i].ToString ());
		}

		Vector3[] testValues = new Vector3[test.Length];
			
		for (int i = 0; i < test.Length; i++) {
			testValues[i] = SmoothValues (test [i], Tracking.testing);
		}

		for (int i = 0; i < testValues.Length; i++) {
			print ("After smoothing " + i + ": " + testValues [i].ToString ());
		}
	}

	private void moveMainCamera() {
		int movementSensitivity = 3;

		Vector3 newCameraPosition;

		var xMovement = Head.x - HeadCallibratedPosition.x;
		var yMovement = Head.y - HeadCallibratedPosition.y;
		var zMovement = Head.z - HeadCallibratedPosition.z;

		newCameraPosition.x = Cameras.MainCameraPosition.x + (xMovement * movementSensitivity);
		newCameraPosition.y = Cameras.MainCameraPosition.y + (yMovement * movementSensitivity);
		newCameraPosition.z = Cameras.MainCameraPosition.z - (zMovement * movementSensitivity); //minus to move camera in the correct view of the user

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

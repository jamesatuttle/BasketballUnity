using UnityEngine;
using System;
using UnityEngine.UI;
using Common;

public class KinectController : MonoBehaviour
{
	private Vector3 _handLeft;
	private Vector3 _handRight;
	private Vector3 _head;
	private Vector3 _hipCenter;
	private Vector3 _shoulderLeft;
	private Vector3 _shoulderRight;
	private Vector3 _wristLeft;
	private Vector3 _wristRight;
	private Vector3 _elbowLeft;
	private Vector3 _elbowRight;

	private bool _ballIsHeld;

	private Vector3 _handsCallibratedPosition;
	private Vector3 _headCallibratedPosition;

	private float _handDifference;

	/*
	 * Smoothing global variables
	 */
	private static int _collectionSize = 10;

	private int _handLeftTrackingCount;
	private static Vector3[] _handLeftCollection = new Vector3[_collectionSize];
	private static Vector3[] _handLeftDifferences = new Vector3[_collectionSize - 1];

	private int _handRightTrackingCount;
	private static Vector3[] _handRightCollection = new Vector3[_collectionSize];
	private static Vector3[] _handRightDifferences = new Vector3[_collectionSize - 1];

	private int _wristLeftTrackingCount;
	private static Vector3[] _wristLeftCollection = new Vector3[_collectionSize];
	private static Vector3[] _wristLeftDifferences = new Vector3[_collectionSize - 1];

	private int _wristRightTrackingCount;
	private static Vector3[] _wristRightCollection = new Vector3[_collectionSize];
	private static Vector3[] _wristRightDifferences = new Vector3[_collectionSize - 1];

	private int _headTrackingCount;
	private static Vector3[] _headCollection = new Vector3[_collectionSize];
	private static Vector3[] _headDifferences = new Vector3[_collectionSize - 1];

	private int _hipCenterTrackingCount;
	private static Vector3[] _hipCenterCollection = new Vector3[_collectionSize];
	private static Vector3[] _hipCenterDifferences = new Vector3[_collectionSize - 1];

	private int _shoulderLeftTrackingCount;
	private static Vector3[] _shoulderLeftCollection = new Vector3[_collectionSize];
	private static Vector3[] _shoulderLeftDifferences = new Vector3[_collectionSize - 1];

	private int _shoulderRightTrackingCount;
	private static Vector3[] _shoulderRightCollection = new Vector3[_collectionSize];
	private static Vector3[] _shoulderRightDifferences = new Vector3[_collectionSize - 1];

	private int _elbowLeftTrackingCount;
	private static Vector3[] _elbowLeftCollection = new Vector3[_collectionSize];
	private static Vector3[] _elbowLeftDifferences = new Vector3[_collectionSize - 1];

	private int _elbowRightTrackingCount;
	private static Vector3[] _elbowRightCollection = new Vector3[_collectionSize];
	private static Vector3[] _elbowRightDifferences = new Vector3[_collectionSize - 1];

	enum Joints {
		HandLeft = 0,
		HandRight = 1,
		WristLeft = 2,
		WristRight = 3,
		Head = 4,
		HipCenter = 5,
		ShoulderLeft = 6,
		ShoulderRight = 7,
		ElbowLeft = 8,
		ElbowRight = 9
	}

	void Start () {  
		_ballIsHeld = false;
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "";
		ANN_CPU.InitialiseANN ();
		_handLeftTrackingCount = 0;
		_handRightTrackingCount = 0;
		_wristLeftTrackingCount = 0;
		_wristRightTrackingCount = 0;
		_headTrackingCount = 0;
		_hipCenterTrackingCount = 0;
		_shoulderLeftTrackingCount = 0;
		_shoulderRightTrackingCount = 0;
		_elbowLeftTrackingCount = 0;
		_elbowRightTrackingCount = 0;
	}
		
	void Update () { 
		try {
			if (GamePlay.GameIsPlayable) {

				KinectManager manager = KinectManager.Instance;

				if (manager.IsUserDetected ()) {

					uint userId = manager.GetPlayer1ID ();

					_handLeft = SmoothRawSkeletalData(manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft), Joints.HandLeft);
					_handRight = SmoothRawSkeletalData(manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight), Joints.HandRight);
					_wristLeft = SmoothRawSkeletalData(manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft), Joints.WristLeft);
					_wristRight = SmoothRawSkeletalData(manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.WristRight), Joints.WristRight);
					_head = SmoothRawSkeletalData(manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.Head), Joints.Head);
					_hipCenter = SmoothRawSkeletalData(manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter), Joints.HipCenter);
					_shoulderLeft = SmoothRawSkeletalData(manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft), Joints.ShoulderLeft);
					_shoulderRight = SmoothRawSkeletalData(manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight), Joints.ShoulderRight);
					_elbowLeft = SmoothRawSkeletalData(manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft), Joints.ElbowLeft);
					_elbowRight = SmoothRawSkeletalData(manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight), Joints.ElbowRight);

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
		_handDifference = -_handLeft.x - -_handRight.x;  //These x values are negative, the minus sets them positive

		if (_ballIsHeld) {

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
				_ballIsHeld = true;
			}
		}
	}

	private bool IsBallPickedUp() {
		
		bool handsInDistanceToPickUpBall = _handDifference > (CommonValues._ballWidth - (CommonValues._inch * 2)) && _handDifference < (CommonValues._ballWidth + (CommonValues._inch * 2));
		bool handsInFrontOfBody = _hipCenter.z > _handLeft.z && _hipCenter.z > _handRight.z;

		if (handsInDistanceToPickUpBall && handsInFrontOfBody)
			return true;
		else
			return false;
	}

	private bool IsBallDropped() {
		
		if (_handDifference > (CommonValues._ballWidth + (CommonValues._inch * 3)))
			return true;
		else
			return false;
	}

	private void callibrateUser () {
		_handsCallibratedPosition.x = (_handLeft.x + _handRight.x) / 2;
		_handsCallibratedPosition.y = (_handLeft.y + _handRight.y) / 2;
		_handsCallibratedPosition.z = (_handLeft.z + _handRight.z) / 2;

		_headCallibratedPosition.x = _head.x;
		_headCallibratedPosition.y = _head.y;
		_headCallibratedPosition.z = _head.z;
	}

	private void dropBall() {
		
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Dropped ball";

		Basketball.SetBallGravity (true); //turn gravity on

		_ballIsHeld = false;
	}

	private void moveBall () {
		
		int movementSensitivity = 3;

		Vector3 newBallPosition;

		float handsX = (_handLeft.x + _handRight.x) / 2;
		float handsY = (_handLeft.y + _handRight.y) / 2;
		float handsZ = (_handLeft.z + _handRight.z) / 2;

		Vector3 movement = new Vector3 (handsX - _handsCallibratedPosition.x, handsY - _handsCallibratedPosition.y, handsZ - _handsCallibratedPosition.z);

		newBallPosition.x = Basketball.InitialBallPosition.x + (movement.x * movementSensitivity);
		newBallPosition.y = Basketball.InitialBallPosition.y + (movement.y * movementSensitivity);
		newBallPosition.z = Basketball.InitialBallPosition.z - (movement.z * movementSensitivity); //minus to move ball in the correct view of the user

		Basketball.LockBasketballPosition (false);

		GameObject.Find ("Basketball").transform.position = newBallPosition;
	}

	private Vector3 SmoothRawSkeletalData(Vector3 rawVector, Joints joints) {
		int count = 0;
		Vector3[] collection = new Vector3[_collectionSize];
		Vector3[] differences = new Vector3[_collectionSize - 1];

		switch (joints) {
		case Joints.HandRight:
			count = _handRightTrackingCount;
			collection = _handRightCollection;
			differences = _handRightDifferences;
			break;
		case Joints.HandLeft:
			count = _handLeftTrackingCount;
			collection = _handLeftCollection;
			differences = _handLeftDifferences;
			break;
		case Joints.WristRight:
			count = _wristRightTrackingCount;
			collection = _wristRightCollection;
			differences = _wristRightDifferences;
			break;
		case Joints.WristLeft:
			count = _wristLeftTrackingCount;
			collection = _wristLeftCollection;
			differences = _wristLeftDifferences;
			break;
		case Joints.Head:
			count = _headTrackingCount;
			collection = _headCollection;
			differences = _headDifferences;
			break;
		case Joints.HipCenter:
			count = _hipCenterTrackingCount;
			collection = _hipCenterCollection;
			differences = _hipCenterDifferences;
			break;
		case Joints.ShoulderRight:
			count = _shoulderRightTrackingCount;
			collection = _shoulderRightCollection;
			differences = _shoulderRightDifferences;
			break;
		case Joints.ShoulderLeft:
			count = _shoulderLeftTrackingCount;
			collection = _shoulderLeftCollection;
			differences = _shoulderLeftDifferences;
			break;
		case Joints.ElbowRight:
			count = _elbowRightTrackingCount;
			collection = _elbowRightCollection;
			differences = _elbowRightDifferences;
			break;
		case Joints.ElbowLeft:
			count = _elbowLeftTrackingCount;
			collection = _elbowLeftCollection;
			differences = _elbowLeftDifferences;
			break;
		}

		//empty the collection if full
		if (count == _collectionSize-1) {
			//print ("clear collection");
			count = 0;

			for (int i = 0; i < collection.Length; i++)
				collection [i] = new Vector3 (0, 0, 0);

			for (int i = 0; i < differences.Length; i++)
				differences [i] = new Vector3 (0, 0, 0);
		}

		//print ("count: " + count);
			
		if (count == 0) {
			//print ("one of first two counts");
			collection [count] = rawVector;
		} else if (count == 1) {
			collection [count] = rawVector;
			/*differences [count - 1].x = collection [count].x - collection [count - 1].x;
			differences [count - 1].y = collection [count].y - collection [count - 1].y;
			differences [count - 1].z = collection [count].z - collection [count - 1].z;*/
			differences [count - 1] = collection [count] - collection [count - 1];
		} else if (count < _collectionSize-1) {
			//print ("smoothing new point");
			/*differences [count - 1].x = collection [count].x - collection [count - 1].x;
			differences [count - 1].y = collection [count].y - collection [count - 1].y;
			differences [count - 1].z = collection [count].z - collection [count - 1].z;*/

			differences [count - 1] = collection [count] - collection [count - 1];
			//print ("1");

			Vector3 newVectorDifference = collection [count - 1] - rawVector;
			//print ("2");

			/*double newVectorDifferenceX = collection [count - 1].x - rawVector.x;
			double newVectorDifferenceY = collection [count - 1].y - rawVector.y;
			double newVectorDifferenceZ = collection [count - 1].z - rawVector.z;

			if (newVectorDifferenceX != differences [count - 1].x) {
				rawVector.x = collection [count - 1].x + differences [count - 1].x;
			}
			if (newVectorDifferenceY != differences [count - 1].y) {
				rawVector.y = collection [count - 1].y + differences [count - 1].y;
			}
			if (newVectorDifferenceZ != differences [count - 1].z) {
				rawVector.z = collection [count - 1].z + differences [count - 1].z;
			}*/

			if (newVectorDifference.x != differences [count - 1].x) {
				rawVector.x = collection [count - 1].x + differences [count - 1].x;
			}
			//print ("3");

			if (newVectorDifference.y != differences [count - 1].y) {
				rawVector.y = collection [count - 1].y + differences [count - 1].y;
			}
			//print ("4");

			if (newVectorDifference.z != differences [count - 1].z) {
				rawVector.z = collection [count - 1].z + differences [count - 1].z;
			}
			//print ("5");


			collection [count] = rawVector;
			//print ("6");

			differences [count] = collection [count] - collection [count - 1];
			//print ("7");

		}

		count++;

		switch (joints) {
		case Joints.HandRight:
			_handRightTrackingCount = count;
			_handRightCollection = collection;
			_handRightDifferences = differences;
			break;
		case Joints.HandLeft:
			_handLeftTrackingCount = count;
			_handLeftCollection = collection;
			_handLeftDifferences = differences;
			break;
		case Joints.WristRight:
			_wristRightTrackingCount = count;
			_wristRightCollection = collection;
			_wristRightDifferences = differences;
			break;
		case Joints.WristLeft:
			_wristLeftTrackingCount = count;
			_wristLeftCollection = collection;
			_wristLeftDifferences = differences;
			break;
		case Joints.Head:
			_headTrackingCount = count;
			_headCollection = collection;
			_headDifferences = differences;
			break;
		case Joints.HipCenter:
			_hipCenterTrackingCount = count;
			_hipCenterCollection = collection;
			_hipCenterDifferences = differences;
			break;
		case Joints.ShoulderRight:
			_shoulderRightTrackingCount = count;
			_shoulderRightCollection = collection;
			_shoulderRightDifferences = differences;
			break;
		case Joints.ShoulderLeft:
			_shoulderLeftTrackingCount = count;
			_shoulderLeftCollection = collection;
			_shoulderLeftDifferences = differences;
			break;
		case Joints.ElbowRight:
			_elbowRightTrackingCount = count;
			_elbowRightCollection = collection;
			_elbowRightDifferences = differences;
			break;
		case Joints.ElbowLeft:
			_elbowLeftTrackingCount = count;
			_elbowLeftCollection = collection;
			_elbowLeftDifferences = differences;
			break;
		}

		return rawVector;
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

	private void moveMainCamera() {
		int movementSensitivity = 3;

		Vector3 newCameraPosition;

		var xMovement = _head.x - _headCallibratedPosition.x;
		var yMovement = _head.y - _headCallibratedPosition.y;
		var zMovement = _head.z - _headCallibratedPosition.z;

		newCameraPosition.x = Cameras.MainCameraPosition.x + (xMovement * movementSensitivity);
		newCameraPosition.y = Cameras.MainCameraPosition.y + (yMovement * movementSensitivity);
		newCameraPosition.z = Cameras.MainCameraPosition.z - (zMovement * movementSensitivity); //minus to move camera in the correct view of the user

		Cameras.UpdateCameraPosition (newCameraPosition.x, newCameraPosition.y, newCameraPosition.z, 34);
	}

	private void collectSkeletalDifferences() {

		double rightHand_HipX = _handRight.x - _hipCenter.x;
		double rightHand_HipY = _handRight.y - _hipCenter.y;
		double rightHand_HipZ = _handRight.z - _hipCenter.z;

		double rightHand_RightWristX = _handRight.x - _wristRight.x;
		double rightHand_RightWristY = _handRight.y - _wristRight.y;
		double rightHand_RightWristZ = _handRight.z - _wristRight.z;

		double rightWrist_RightElbowX = _wristRight.x - _elbowRight.x;
		double rightWrist_RightElbowY = _wristRight.y - _elbowRight.y;
		double rightWrist_RightElbowZ = _wristRight.z - _elbowRight.z;

		double rightElbow_RightShoulderX = _elbowRight.x - _shoulderRight.x;
		double rightElbow_RightShoulderY = _elbowRight.y - _shoulderRight.y;
		double rightElbow_RightShoulderZ = _elbowRight.z - _shoulderRight.z;

		double rightHand_RightShoulderX = _handRight.x - _shoulderRight.x;
		double rightHand_RightShoulderY = _handRight.y - _shoulderRight.y;
		double rightHand_RightShoulderZ = _handRight.z - _shoulderRight.z;

		double leftHand_HipX = _handLeft.x - _hipCenter.x;
		double leftHand_HipY = _handLeft.y - _hipCenter.y;
		double leftHand_HipZ = _handLeft.z - _hipCenter.z;

		double leftHand_LeftWristX = _handLeft.x - _wristLeft.x;
		double leftHand_LeftWristY = _handLeft.y - _wristLeft.y;
		double leftHand_LeftWristZ = _handLeft.z - _wristLeft.z;

		double leftWrist_LeftElbowX = _wristLeft.x - _elbowLeft.x;
		double leftWrist_LeftElbowY = _wristLeft.y - _elbowLeft.y;
		double leftWrist_LeftElbowZ = _wristLeft.z - _elbowLeft.z;

		double leftElbow_LeftShoulderX = _elbowLeft.x - _shoulderLeft.x;
		double leftElbow_LeftShoulderY = _elbowLeft.y - _shoulderLeft.y;
		double leftElbow_LeftShoulderZ = _elbowLeft.z - _shoulderLeft.z;

		double leftHand_LeftShoulderX = _handLeft.x - _shoulderLeft.x;
		double leftHand_LeftShoulderY = _handLeft.y - _shoulderLeft.y;
		double leftHand_LeftShoulderZ = _handLeft.z - _shoulderLeft.z;

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

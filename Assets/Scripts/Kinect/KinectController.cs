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

	private Vector3 _previousBallPosition;
	private Vector3 _previousCameraPosition;

	enum Smoothing {
		ball = 0,
		camera = 1
	};

	void Start () {  
		_ballIsHeld = false;
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "";
		ANN_CPU.InitialiseANN ();
	}
		
	void Update () { 
		try {
			if (GamePlay.GameIsPlayable) {

				KinectManager manager = KinectManager.Instance;

				if (manager.IsUserDetected ()) {

					uint userId = manager.GetPlayer1ID ();  

					_handLeft = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
					_handRight = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight);
					_wristLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft);
					_wristRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.WristRight);
					_head = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.Head);
					_hipCenter = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HipCenter);
					_shoulderLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
					_shoulderRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);
					_elbowLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft);
					_elbowRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight);

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
		
		bool handsInDistanceToPickUpBall = _handDifference > (CommonValues.BallWidth - (CommonValues.Inch * 2)) && _handDifference < (CommonValues.BallWidth + (CommonValues.Inch * 2));
		bool handsInFrontOfBody = _hipCenter.z > _handLeft.z && _hipCenter.z > _handRight.z;

		if (handsInDistanceToPickUpBall && handsInFrontOfBody)
			return true;
		else
			return false;
	}

	private bool IsBallDropped() {
		
		if (_handDifference > (CommonValues.BallWidth + (CommonValues.Inch * 3)))
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

		_previousBallPosition = Basketball.InitialBallPosition;
		_previousCameraPosition = Cameras.MainCameraPosition;
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

		GameObject.Find ("Basketball").transform.position = SmoothBasketball(newBallPosition);
	}

	private Vector3 SmoothBasketball(Vector3 ballPosition) {
		return SmoothValues (ballPosition, Smoothing.ball);
	}

	private Vector3 SmoothCameraMovement(Vector3 cameraPosition) {
		return SmoothValues (cameraPosition, Smoothing.camera);
	}

	private Vector3 SmoothValues(Vector3 newPosition, Smoothing smoothing) {

		float threshold = 0.3f;
		Vector3 previousPosition = new Vector3(0f, 0f, 0f);

		switch (smoothing) {
		case Smoothing.ball:
			threshold = 0.3f;
			previousPosition = _previousBallPosition;
			break;
		case Smoothing.camera:
			threshold = 0.2f;
			previousPosition = _previousCameraPosition;
			break;
		}

		if (!isWithinBoundary (previousPosition.x, newPosition.x, threshold)) {
			newPosition.x = previousPosition.x;
		}
		if (!isWithinBoundary (previousPosition.y, newPosition.y, threshold)) {
			newPosition.y = previousPosition.y;
		}
		if (!isWithinBoundary (previousPosition.z, newPosition.z, threshold)) {
			newPosition.z = previousPosition.z;
		}

		previousPosition = newPosition;

		switch (smoothing) {
		case Smoothing.ball:
			_previousBallPosition = previousPosition;
			break;
		case Smoothing.camera:
			_previousCameraPosition = previousPosition;
			break;
		}

		return newPosition;

	}

	private bool isWithinBoundary(float oldPosition, float newPosition, float threshold) {
		float upperBoundary = oldPosition + threshold;
		float lowerBoudary = oldPosition - threshold;

		if (newPosition > upperBoundary || newPosition < lowerBoudary) {
			return false;
		} else {
			return true;
		}
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

		Vector3 smoothedCameraPosition = SmoothCameraMovement (newCameraPosition);

		Cameras.UpdateCameraPosition (smoothedCameraPosition.x, smoothedCameraPosition.y, smoothedCameraPosition.z, 34);
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

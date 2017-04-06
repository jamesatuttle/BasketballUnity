using UnityEngine;
using System;
using UnityEngine.UI;
using Common;
using System.Collections.Generic;

public class KinectController : MonoBehaviour
{
	public static KinectController instance;

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
	private bool _ballIsThrown;

	private Vector3 _handsCallibratedPosition;
	private Vector3 _headCallibratedPosition;

	private float _handDifference;

	private Vector3 _previousBallPosition;
	private Vector3 _previousCameraPosition;

	private Gesture _currentGesture;

	private Vector3 _trajectory;

	private GameObject _basketball;
	private string _gestureInfo;

	List<Vector3> _ballPositionCollection = new List<Vector3> ();

	enum Gesture {
		stationary = 0,
		professional,
		chest,
		low
	}

	enum Smoothing {
		ball = 0,
		camera
	};

	void Awake () {
		instance = this;

		_basketball = GameObject.Find("Basketball").gameObject;
		_gestureInfo = GameObject.Find ("GestureInfo").GetComponent<Text> ().text;
	}

	void Start () {  
		_ballIsHeld = false;
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "";
		_trajectory = Vector3.zero;
		ANN_CPU.InitialiseANN ();
	}
		
	void Update () { 
		try {
			GamePlay.GameIsPlayable = true;

			if (GamePlay.GameIsPlayable && (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.preGame || GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame)) {

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

					_handDifference = -_handLeft.x - -_handRight.x; //These x values are negative, the minus sets them positive

					BasketballController ();

				} else {
					_gestureInfo = "Stand in front of the sensor";
				}
			}
				
		} catch (Exception e) {
			GamePlay.GameIsPlayable = false;
			Debug.Log ("Exception Kinect Controller: " + e.Message);
		}
	}

	private void SetTrajectory(Vector3 newTraj) {
		_trajectory = newTraj;
	}

	public void UpdateCurrentGesture(int gesture) {
		switch (gesture) {
		case (int)Gesture.stationary:
			_currentGesture = Gesture.stationary;
			break;
		case (int)Gesture.chest:
			_currentGesture = Gesture.chest;
			break;
		case (int)Gesture.low:
			_currentGesture = Gesture.low;
			break;
		case (int)Gesture.professional:
			_currentGesture = Gesture.professional;
			break;
		}
	}

	private void BasketballController () {

		if (_ballIsHeld) { //if the ball is being held
			_gestureInfo = "Picked up ball";

			if (HasBallBeenDropped ()) { //check if the ball has been dropped or thrown
				TrackBallDirection ();

				if (HasBallBeenThrown ()) { //check if the ball has been thrown
					_gestureInfo = "Thrown ball";
					ThrowBall ();
				} else { //if the ball hasn't been thrown - it has been dropped
					_gestureInfo = "Dropped ball";
					DropBall ();
				} 

			} else { //if the ball has not been dropped or thrown - move the ball and camera
				collectSkeletalDifferences ();
				moveBall ();
				moveMainCamera ();
			}

		} else if (!_ballIsHeld && HasBallBeenThrown ()) {
			TrackBallDirection ();
			ThrowBall ();

		} else { //if the ball is not being held

			if (IsBallPickedUp()) { //check if the ball has been picked up
				callibrateUser (); //if so - calibrate the user to create an initial base for moving the ball
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

	private bool HasBallBeenDropped () {

		float dropBallHandDiff = CommonValues.BallWidth + (CommonValues.Inch * 2);

		if (_handDifference > dropBallHandDiff)
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

	public void ClearBallTrackCollection () {
		_ballPositionCollection.Clear ();
		_ballPositionCollection.TrimExcess ();
		SetTrajectory(Vector3.zero);
	}

	private void ThrowBall() {

		_basketball.transform.position += _trajectory;

		Basketball.instance.SetBallGravity(true);

		_ballIsHeld = false;
	}

	private bool HasBallBeenThrown() {
		if (_trajectory.x < 0.2f && _trajectory.y < 0.2f && _trajectory.z < 0.2f) {
			return false;
		} else {
			return true;
		}
	}

	private void DropBall() {
		Basketball.instance.SetBallGravity (true); //turn gravity on
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

		Basketball.instance.LockBasketballPosition (false);

		Vector3 smoothedBallPositon = SmoothBasketball (newBallPosition);

		_basketball.transform.position = smoothedBallPositon;

		TrackBallPosition (smoothedBallPositon);
	}

	private void TrackBallPosition(Vector3 position) {
		_ballPositionCollection.Add (position);
	}

	private void TrackBallDirection() {

		int listSize = _ballPositionCollection.Count;

		int listThreshold = 8;
		int sensitivity = 6;

		if (listSize > listThreshold) {

			Vector3 difference = _ballPositionCollection [listSize - 1] - _ballPositionCollection [(listSize - 1) - listThreshold];

			_trajectory = (difference / listThreshold) * sensitivity;
		}
	}

	private Vector3 SmoothBasketball(Vector3 ballPosition) {
		return SmoothValues (ballPosition, Smoothing.ball);
	}

	private Vector3 SmoothCameraMovement(Vector3 cameraPosition) {
		return SmoothValues (cameraPosition, Smoothing.camera);
	}

	private Vector3 SmoothValues(Vector3 newPosition, Smoothing smoothing) {

		float xThreshold = 0.3f;
		float yThreshold = 0.3f;
		float zThreshold = 0.3f;

		Vector3 previousPosition = new Vector3(0f, 0f, 0f);

		string gestureText = GameObject.Find ("GestureInfo").GetComponent<Text> ().text;

		switch (smoothing) {
		case Smoothing.ball:
			xThreshold = 0.3f;
			yThreshold = 0.3f;
			zThreshold = 0.3f;
			previousPosition = _previousBallPosition;
			break;
		case Smoothing.camera:
			xThreshold = 0.3f;
			yThreshold = 0.3f;
			zThreshold = 0.3f;
			previousPosition = _previousCameraPosition;
			break;
		}

		if ((_currentGesture == Gesture.professional || _currentGesture == Gesture.chest || _currentGesture == Gesture.low) && (smoothing == Smoothing.ball)) {
			xThreshold += 1.5f;
			yThreshold += 1.5f;
			zThreshold += 1.5f;
		}

		if (NotWithinBoundary (previousPosition.x, newPosition.x, xThreshold)) {
			newPosition.x = previousPosition.x;
		}
		if (NotWithinBoundary (previousPosition.y, newPosition.y, yThreshold)) {
			newPosition.y = previousPosition.y;
		}
		if (NotWithinBoundary (previousPosition.z, newPosition.z, zThreshold)) {
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

	private bool NotWithinBoundary(float oldPosition, float newPosition, float threshold) {
		float upperBoundary = oldPosition + threshold;
		float lowerBoudary = oldPosition - threshold;

		if (newPosition > upperBoundary || newPosition < lowerBoudary) {
			return true;
		} else {
			return false;
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
			rightHand_HipX, rightHand_HipY, rightHand_HipZ, 
			rightHand_RightWristX, rightHand_RightWristY, rightHand_RightWristZ, 
			rightWrist_RightElbowX, rightWrist_RightElbowY, rightWrist_RightElbowZ, 
			rightElbow_RightShoulderX, rightElbow_RightShoulderY, rightElbow_RightShoulderZ, 
			rightHand_RightShoulderX, rightHand_RightShoulderY, rightHand_RightShoulderZ,
			leftHand_HipX, leftHand_HipY, leftHand_HipZ, 
			leftHand_LeftWristX, leftHand_LeftWristY, leftHand_LeftWristZ, 
			leftWrist_LeftElbowX, leftWrist_LeftElbowY, leftWrist_LeftElbowZ, 
			leftElbow_LeftShoulderX, leftElbow_LeftShoulderY, leftElbow_LeftShoulderZ, 
			leftHand_LeftShoulderX, leftHand_LeftShoulderY, leftHand_LeftShoulderZ
		};

		/*
		 * Stationary
		 * Professional
		 * Chest
		 * Low
		 */
		/*string gesture = "Professional";

		AddToDatabase.addToANNTrainingData (
			rightHand_HipX, rightHand_HipY, rightHand_HipZ,
			rightHand_RightWristX, rightHand_RightWristY, rightHand_RightWristZ,
			rightWrist_RightElbowX, rightWrist_RightElbowY, rightWrist_RightElbowZ,
			rightElbow_RightShoulderX, rightElbow_RightShoulderY, rightElbow_RightShoulderZ,
			rightHand_RightShoulderX, rightHand_RightShoulderY, rightHand_RightShoulderZ,

			leftHand_HipX, leftHand_HipY, leftHand_HipZ,
			leftHand_LeftWristX, leftHand_LeftWristY, leftHand_LeftWristZ,
			leftWrist_LeftElbowX, leftWrist_LeftElbowY, leftWrist_LeftElbowZ,
			leftElbow_LeftShoulderX, leftElbow_LeftShoulderY, leftElbow_LeftShoulderZ,
			leftHand_LeftShoulderX, leftHand_LeftShoulderY, leftHand_LeftShoulderZ,

			gesture
		);*/

		ANN_CPU.StartANN (trackedSkeletalPoints);
	}
}

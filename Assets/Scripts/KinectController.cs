using UnityEngine;
using System;
using UnityEngine.UI;
using Common;
using System.Collections.Generic;

public class KinectController : MonoBehaviour
{
	public static KinectController instance; //create an instance of the class, so that other classes can access instances of its methods

	//Vector3 variables to store the joint values
	//the Vector3 stores the X, Y and Z values
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

	//boolean values for the state of the ball
	private bool _ballIsHeld;
	private bool _ballIsThrown;

	//Vector3 values for th initial ball picked up callibrated position of the head and hands
	private Vector3 _handsCallibratedPosition;
	private Vector3 _headCallibratedPosition;

	private float _handDifference; //the difference between the hands

	private Vector3 _previousBallPosition; //the previous position of the ball
	private Vector3 _previousCameraPosition; //the previous posiiton of the camera

	private Gesture _currentGesture; //the current gesture being performed, returned from the ANN

	private Vector3 _trajectory; //the calculated trajectory of the ball

	private GameObject _basketball; //the basketball game object
	private string _gestureInfo; //the text of the GestureInfo game object

	List<Vector3> _ballPositionCollection = new List<Vector3> (); //the list of ball positions
	//this is a list as it allows it to be expanded and then reset when needed

	//a gesture enum to store each value against an integer value - prevents magic numbers
	enum Gesture {
		stationary = 0,
		professional,
		chest,
		low
	}

	//a smoothing enum to store each value against an integer value - prevents magic numbers
	//the smoothing applies to which joint is being smoothed by the system
	enum Smoothing {
		ball = 0,
		camera
	};

	//Awake is called at the start of the game, used to initialise variables
	void Awake () {
		instance = this;

		_basketball = GameObject.Find("Basketball").gameObject;
		_gestureInfo = GameObject.Find ("GestureInfo").GetComponent<Text> ().text;
	}

	//called after Awake, at the start of the game
	void Start () {
		_ballIsHeld = false;
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "";
		_trajectory = Vector3.zero; //sets the trajectory to 0, 0, 0
		ANN_CPU.InitialiseANN (); //initialise the neural network (ANN) - including the training of the network
	}

	/*
	* Update is called once per frame
	* Used to control the kinect, gesture, throwing, and ball moving functionality
	*/
	void Update () {
		try {
			GamePlay.GameIsPlayable = true;

			if (GamePlay.GameIsPlayable && (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.preGame || GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.mainGame)) {
				//if the game is either the pregame or the maingame and the game is playable

				KinectManager manager = KinectManager.Instance; //create an instance of the downloaded KinectManager class

				if (manager.IsUserDetected ()) { //if the user is detected..

					uint userId = manager.GetPlayer1ID ();

					//gets the values of each of the joints below, storing them in the global Vector3 variables
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

					//calculates the differnece between the hands on the x value
					_handDifference = -_handLeft.x - -_handRight.x; //These x values are negative, the minus sets them positive

					BasketballController ();

				} else { //if the user is not detected...
					_gestureInfo = "Stand in front of the sensor";
				}
			}

		} catch (Exception e) { //if any exception is thrown...
			GamePlay.GameIsPlayable = false; //stop the game
			Debug.Log ("Exception Kinect Controller: " + e.Message); //print the error to the console
		}
	}

	/*
	* Set the trajectory of the basketball when being thrown
	*/
	private void SetTrajectory(Vector3 newTraj) {
		_trajectory = newTraj;
	}

	/*
	* Updates the text showing which gesture is being performed
	*/
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

	/*
	* A central controller which controls how the basketball moves
	*/
	private void BasketballController () {

		if (_ballIsHeld) { //if the ball is being held...
			_gestureInfo = "Picked up ball";

			if (HasBallBeenDropped ()) { //if the ball has been thrown...
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

	/*
	* Returns true if the ball has been picked up, and false if not
	*/
	private bool IsBallPickedUp() {

		bool handsInDistanceToPickUpBall = _handDifference > (CommonValues.BallWidth - (CommonValues.Inch * 2)) && _handDifference < (CommonValues.BallWidth + (CommonValues.Inch * 2));
		//checks that the hands are in the distance to be able to realistically pick up the basketball (the width of the ball plus 2 inches)

		bool handsInFrontOfBody = _hipCenter.z > _handLeft.z && _hipCenter.z > _handRight.z;
		//checks that the hands are in front of the body and not behind

		if (handsInDistanceToPickUpBall && handsInFrontOfBody)
			return true;
		else
			return false;
	}

	/*
	* Returns true if the ball has been dropped, otherwise false
	*/
	private bool HasBallBeenDropped () {

		float dropBallHandDiff = CommonValues.BallWidth + (CommonValues.Inch * 2);

		if (_handDifference > dropBallHandDiff) //if the hand difference is ovver the ball width + 2 inches
			return true;
		else
			return false;
	}

	/*
	* Initially calibrate the user when they pick up the basketball
	* The next position of the hands and head is compared against these values to allow it move
	*/
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

	/*
	* Clear the Ball tracking list
	*/
	public void ClearBallTrackCollection () {
		_ballPositionCollection.Clear (); //clear the list
		_ballPositionCollection.TrimExcess (); //removes the empty list sections
		SetTrajectory(Vector3.zero); //sets the trajectory back to 0
	}

	/*
	* A method to throw the basketball
	* The trajectory of the ball is added as a force to the basketball to make it move
	* Gravity is turned on to create the basketball arch when thrown
	*/
	private void ThrowBall() {

		_basketball.GetComponent<Rigidbody> ().AddForce (_basketball.transform.position += _trajectory);

		Basketball.instance.SetBallGravity(true);

		_ballIsHeld = false;
	}

	/*
	* Returns true if the basketball has been thrown, otherwise false
	* Checks that the trajectory X, Y and Z values are above the 0.2 boundary
	*/
	private bool HasBallBeenThrown() {
		if (_trajectory.x < 0.2f && _trajectory.y < 0.2f && _trajectory.z < 0.2f) {
			return false;
		} else {
			return true;
		}
	}

	/*
	* Turns the gravity on for the ball, causing it to drop
	*/
	private void DropBall() {
		Basketball.instance.SetBallGravity (true); //turn gravity on
		_ballIsHeld = false;
	}

	/*
	* A method to move the basketball
	*/
	private void moveBall () {

		int movementSensitivity = 3;

		Vector3 newBallPosition;

		float handsX = (_handLeft.x + _handRight.x) / 2; //finds the middle point between the hands on the x
		float handsY = (_handLeft.y + _handRight.y) / 2; //finds the middle point between the hands on the y
		float handsZ = (_handLeft.z + _handRight.z) / 2; //finds the middle point between the hands on the z

		Vector3 movement = new Vector3 (handsX - _handsCallibratedPosition.x, handsY - _handsCallibratedPosition.y, handsZ - _handsCallibratedPosition.z);
		//finds the difference between the new ball position and the callibrated position - to find out where the ball has moved

		//Sets the new ball position by moving it in the direction that the hands have moved
		newBallPosition.x = Basketball.InitialBallPosition.x + (movement.x * movementSensitivity);
		newBallPosition.y = Basketball.InitialBallPosition.y + (movement.y * movementSensitivity);
		newBallPosition.z = Basketball.InitialBallPosition.z - (movement.z * movementSensitivity); //minus to move ball in the correct view of the user

		Basketball.instance.LockBasketballPosition (false); //locks the basketball position - to keep it steady whilst being held

		Vector3 smoothedBallPositon = SmoothBasketball (newBallPosition); //calls the smoothing method to remove random values from the newBallPosition

		_basketball.transform.position = smoothedBallPositon; //sets the new position of the ball

		TrackBallPosition (smoothedBallPositon); //adds the smoothed ball position to the ball tracking list
	}

	/*
	* Adds the parameterised Vector3 to the ball track list
	*/
	private void TrackBallPosition(Vector3 position) {
		_ballPositionCollection.Add (position);
	}

	/*
	* Tracks which direction the ball is moving to create the trajectory
	*/
	private void TrackBallDirection() {

		int listSize = _ballPositionCollection.Count; //sets the size of the list to the ball postion collection count

		int listThreshold = 8; //to allow for the last 8 positions of the ball to be used for the trajectory
		int sensitivity = 4; //to create a significant difference in the trajectory

		if (listSize > listThreshold) {

			Vector3 difference = _ballPositionCollection [listSize - 1] - _ballPositionCollection [(listSize - 1) - listThreshold];
			//calculating the difference between the last and 8th last ball position

			_trajectory = (difference / listThreshold) * sensitivity; //calculates the trajectory
		}
	}

	/*
	* A method which passes the parameter into the smooth values function for the basketball
	*/
	private Vector3 SmoothBasketball(Vector3 ballPosition) {
		return SmoothValues (ballPosition, Smoothing.ball);
	}

	/*
	* A method which passes the parameter into the smooth values function for the main camera
	*/
	private Vector3 SmoothCameraMovement(Vector3 cameraPosition) {
		return SmoothValues (cameraPosition, Smoothing.camera);
	}

	/*
	* A generic method to smooth the values of the basketball and head
	*/
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
			previousPosition = _previousBallPosition; //collects the stored last position of the ball
			break;
		case Smoothing.camera:
			xThreshold = 0.3f;
			yThreshold = 0.3f;
			zThreshold = 0.3f;
			previousPosition = _previousCameraPosition; //collects the stored last position of the camera
			break;
		}

		if ((_currentGesture == Gesture.professional || _currentGesture == Gesture.chest || _currentGesture == Gesture.low) && (smoothing == Smoothing.ball)) {
			//if the user is performing one of the throws, increase the smoothing threshold to allow the user to throw
			xThreshold += 1.5f;
			yThreshold += 1.5f;
			zThreshold += 1.5f;
		}

		//three checks on the X, Y and Z positions of the ball, respectively
		//if the ball or camera is not within a close distance to the previous position, ignore it
		if (NotWithinBoundary (previousPosition.x, newPosition.x, xThreshold)) {
			newPosition.x = previousPosition.x;
		}
		if (NotWithinBoundary (previousPosition.y, newPosition.y, yThreshold)) {
			newPosition.y = previousPosition.y;
		}
		if (NotWithinBoundary (previousPosition.z, newPosition.z, zThreshold)) {
			newPosition.z = previousPosition.z;
		}

		previousPosition = newPosition; //update the previousPosition value with the current position of the ball or camera

		//sets the values back depending on what is being smoothed
		switch (smoothing) {
		case Smoothing.ball:
			_previousBallPosition = previousPosition;
			break;
		case Smoothing.camera:
			_previousCameraPosition = previousPosition;
			break;
		}

		//returns the smoothed position
		return newPosition;
	}

	/*
	* A generic method to check whether the camera or ball is close to the previous ball posiiton
	* Returns true if the ball is not with a close area to the previous ball position, otherwise false
	*/
	private bool NotWithinBoundary(float oldPosition, float newPosition, float threshold) {
		float upperBoundary = oldPosition + threshold;
		float lowerBoudary = oldPosition - threshold;

		if (newPosition > upperBoundary || newPosition < lowerBoudary) {
			return true;
		} else {
			return false;
		}
	}

	/*
	* A method to move the main camera
	*/
	private void moveMainCamera() {

		int movementSensitivity = 3; //makes the movemet of the camera realistic to how the user is moving their head

		Vector3 newCameraPosition;

		//calculates the differnce between the new position of the head and the calibrated head position - to see how far it has moved
		var xMovement = _head.x - _headCallibratedPosition.x;
		var yMovement = _head.y - _headCallibratedPosition.y;
		var zMovement = _head.z - _headCallibratedPosition.z;

		//updates the camera position
		newCameraPosition.x = Cameras.MainCameraPosition.x + (xMovement * movementSensitivity);
		newCameraPosition.y = Cameras.MainCameraPosition.y + (yMovement * movementSensitivity);
		newCameraPosition.z = Cameras.MainCameraPosition.z - (zMovement * movementSensitivity); //minus to move camera in the correct view of the user

		//passes the camera position into the smoothing method - to prevent glitching
		Vector3 smoothedCameraPosition = SmoothCameraMovement (newCameraPosition);

		//Update the camera position with the smooted values
		Cameras.UpdateCameraPosition (smoothedCameraPosition.x, smoothedCameraPosition.y, smoothedCameraPosition.z, 45);
	}

	/*
	* Calculates the 30 skeletal differences between the stored joints (X, Y and Z)
	* Passes these into the Neural Network
	*/
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

		//stores the differences in an array of 30 doubles
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

		//The following code was used for adding the gesture data into the database for the training data
		//it is commented as it is not used for the main project, however it shows how the training data was added
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

		ANN_CPU.StartANN (trackedSkeletalPoints); //passes the skeletal differences into the neural network to be classified
	}
}

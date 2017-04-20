using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Basketball : MonoBehaviour {

	public static Basketball instance; //create an instance of the class, so that other classes can access instances of its methods

	public AudioClip bounce; //the audioclip of the bounce sound is stored here

	static public Vector3 InitialBallPosition = new Vector3(0.02f,5.08f,9.96f); //the starting game ball position in the 3D environment.

	//Awake is called before Start, used to initialise variables
	void Awake () {
		instance = this; //initialises the instance to this class
	}

	/*
	* Start is called after Awake, when the game loads
	* This sets audio to not play when the game starts and sets the audio source to the bounce clip
	*/
	void Start () {
		GetComponent<AudioSource> ().playOnAwake = false;
		GetComponent<AudioSource> ().clip = bounce;
	}

	/*
	* When the basketball first enters - collides - with another object, this method is called
	* This method is not always called, due to the speed of the basketball passing through
	* This is the reason why the code is duplicated in the OnCollisionExit
	*/
	public void OnCollisionEnter (Collision col) {
		try { //start of a try-catch statement
			var collision = col.gameObject.name; //stores the colliding object name in a variable

			//if the collider name is one of the walls or the ceiling...
			if (collision == "wall" || collision == "wall (1)" || collision == "wall (2)" || collision == "wall (3)" || collision == "Ceiling" ) {
				GetComponent<AudioSource> ().volume = col.relativeVelocity.magnitude/100; //set the volume of the sound to the velocity relative to 100,
				//to make the sound realistic to the force of the bounce
				GetComponent<AudioSource> ().Play (); //plays the audio
			}
		} //end of the try in the try-catch
		catch { //the catch would occur if the relativeVelocity for the sound is larger than 100%...
			GetComponent<AudioSource> ().volume = 1; //this would set the volume of the audio to 1
		}
	}

	//if the collision object exits the collider
	void OnCollisionExit (Collision col) {
		try {
			var collision = col.gameObject.name; //stores the colliding object name in a variable

			//if the collider name is the floor, one of the walls or the ceiling...
			if (collision == "Floor" || collision == "wall" || collision == "wall (1)" || collision == "wall (2)" || collision == "wall (3)" || collision == "Ceiling" ) {
				GetComponent<AudioSource> ().volume = col.relativeVelocity.magnitude/100; //set the volume of the sound to the velocity relative to 100,
				//to make the sound realistic to the force of the bounce
				GetComponent<AudioSource> ().Play (); //plays the audio
				ResetBall(); //sets the ball to the initial ball position
				Scoreboard.instance.MinusAvailableBalls(); //minuses a ball from the available balls and updates the Scoreboard to show
				BasketDetected.basketCount = 0; //sets the basketcount to 0 to stop counting that streak of ball baskets
			}
		} catch { //the catch would occur if the relativeVelocity for the sound is larger than 100%...
			GetComponent<AudioSource> ().volume = 1; //this would set the volume for the audio to 1
		}
	}

	/*
	* Method to reset the ball
	* The ball tracking collection - used for the trajectory - is reset
	* The position of the ball is set back to the initial ball position
	*/
	public void ResetBall() {
		KinectController.instance.ClearBallTrackCollection ();
		UpdateFixedBasketballPosition(InitialBallPosition.x, InitialBallPosition.y, InitialBallPosition.z);
	}

	/*
	* A generic method used to update the basketball position to a set of specific coordinates
	* The float values: x, y and z are passed through as parameters
	*/
	public void UpdateFixedBasketballPosition(float x, float y, float z)
	{
		GameObject basketball = GameObject.Find ("Basketball"); //the basketball game object is found

		LockBasketballPosition (true); //the x, y, and z position of the basketball is locked - to prevent falling
		SetBallGravity(false); //turn gravity off
		basketball.transform.position = new Vector3 (x, y, z); //set the basketbal position to the set parameters
	}

	/*
	* A method to Lock the basketball position depending on the bool parameter
	*/
	public void LockBasketballPosition(bool locked)
	{
		if (locked) //if  locked is true...
			GameObject.Find ("Basketball").GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll; //freeze the ball position
		else //if locked is false...
			GameObject.Find ("Basketball").GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None; //unfreeze the ball
	}

	/*
	* Turn the ball gravity on or off, depending on the boolean parameter
	*/
	public void SetBallGravity(bool gravity) {
		GameObject.Find("Basketball").GetComponent<Rigidbody>().useGravity = gravity; //sets the gravity to on or off
	}
}

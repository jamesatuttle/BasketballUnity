using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PScript : MonoBehaviour {

	private GListener gestureListener;

	// Use this for initialization
	void Start () {
		gestureListener = GameObject.Find ("MainCamera").GetComponent<GestureListener>();

	}
	
	// Update is called once per frame
	void Update () {
		KinectManager kinectManager = KinectManager.Instance;

		if(!kinectManager || !kinectManager.IsInitialized() || !kinectManager.IsUserDetected())
			return;

		GameObject.Find ("PracticeText").GetComponent<Text> ().text = "User being tracked";

		if(gestureListener)
		{
			if(gestureListener.IsSwipeLeft())
				DisplayHandLeft();
			else if(gestureListener.IsSwipeRight())
				DisplayHandRight();
		}
			
	}

	void DisplayHandRight() {
		GameObject.Find ("PracticeText").GetComponent<Text> ().text = "Right";
	}

	void DisplayHandLeft() {
		GameObject.Find ("PracticeText").GetComponent<Text> ().text = "Left";
	}
}

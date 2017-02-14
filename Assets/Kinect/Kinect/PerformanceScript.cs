using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceScript : MonoBehaviour {

	private GestureListener gestureListener;

	// Use this for initialization
	void Start () {
		gestureListener = GameObject.Find ("MainCamera").GetComponent<GestureListener>();
       // GameObject.Find("GestureInfo").GetComponent<Text>().text = "START";

    }

    // Update is called once per frame
    void Update () {
        //GameObject.Find("GestureInfo").GetComponent<Text>().text = "Here1";

        KinectManager kinectManager = KinectManager.Instance;
        //GameObject.Find("GestureInfo").GetComponent<Text>().text = "Here";


        if (!kinectManager || !kinectManager.IsInitialized() || !kinectManager.IsUserDetected())
        {
            GameObject.Find("GestureInfo").GetComponent<Text>().text = "No Kinect Manager";
            return;
        }

        //GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "User being tracked";

        if (gestureListener)
		{
            GameObject.Find("GestureInfo").GetComponent<Text>().text = "Gesture Listener";

            if (gestureListener.IsSwipeLeft())
            {
                GameObject.Find("GestureInfo").GetComponent<Text>().text = "Swiped Left";

                DisplayHandLeft();
            }
            else if (gestureListener.IsSwipeRight())
            {
                GameObject.Find("GestureInfo").GetComponent<Text>().text = "Swiped Right";

                DisplayHandRight();
            } else
            {
                GameObject.Find("GestureInfo").GetComponent<Text>().text = "No Gesture detected ";

            }

        }
			
	}

	void DisplayHandRight() {
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Right";
	}

	void DisplayHandLeft() {
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Left";
	}
}

using UnityEngine;
using UnityEngine.UI;

public class GestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{

	public Text GestureInfo;

	private Vector3 RightHand;
	private Vector3 LeftHand;
	//private uint globalUserId;

	//private bool trackHands;

	/* private bool swipeLeft;
	private bool swipeRight;

	public bool IsSwipeLeft()
	{
        //Debug.Log("Swipe Left:- " + swipeLeft);
		if(swipeLeft)
		{
			swipeLeft = false;
			return true;
		}

		return false;
	} 

	public bool IsSwipeRight()
	{
        //Debug.Log("Swipe Right:- " + swipeRight);
        if (swipeRight)
		{
			swipeRight = false;
			return true;
		}

		return false;
	}*/


	public void UserDetected (uint userId, int userIndex)
	{
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "User detected";
		Debug.Log ("User detected in GestureListener");

		// detect these user specific gestures
		KinectManager manager = KinectManager.Instance;


		//GestureInfo.GetComponent<Text>().text = "User detected";

		//manager.DetectGesture (userId, KinectGestures.Gestures.SwipeLeft);
		//manager.DetectGesture (userId, KinectGestures.Gestures.SwipeRight);
		manager.DetectGesture(userId, KinectGestures.Gestures.RaiseLeftHand);
		manager.DetectGesture(userId, KinectGestures.Gestures.RaiseRightHand);

		//globalUserId = userId;

		//LeftHand = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
		//RightHand = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight);

		//Debug.Log ("UserDetected userId: " + userId);

		//Debug.Log ("LeftHand: x-" + LeftHand.x + ", y-" + LeftHand.y + ", z-" + LeftHand.z);
		//Debug.Log ("RightHand: x-" + RightHand.x + ", y-" + RightHand.y + ", z-" + RightHand.z);

	

		// manager.DetectGesture(userId, KinectGestures.Gestures.Pull);
		//  manager.DetectGesture(userId, KinectGestures.Gestures.Push);

		/*if(GestureInfo != null)
		{
			GestureInfo.GetComponent<GUIText>().text = "Swipe left or right to change the slides.";
		}*/
	}

	public void UserLost (uint userId, int userIndex)
	{
		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "User Lost";

		if (GestureInfo != null) {
			GestureInfo.GetComponent<GUIText> ().text = string.Empty;
		}
	}

	public void GestureInProgress (uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                               float progress, KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
	{
		// don't do anything here

		Debug.Log ("GestureInProgress: " + gesture);

		if (gesture == KinectGestures.Gestures.RaiseLeftHand || gesture == KinectGestures.Gestures.RaiseLeftHand) {
			GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Hands raise started";

			KinectManager manager = KinectManager.Instance;

			LeftHand = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
			RightHand = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight);

			Debug.Log ("LeftHand: x-" + LeftHand.x + ", y-" + LeftHand.y + ", z-" + LeftHand.z);
			Debug.Log ("RightHand: x-" + RightHand.x + ", y-" + RightHand.y + ", z-" + RightHand.z);
		}
	}

	public bool GestureCompleted (uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
	{

		if (gesture == KinectGestures.Gestures.RaiseLeftHand || gesture == KinectGestures.Gestures.RaiseLeftHand) {
			GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Hands raise completed";

			KinectManager manager = KinectManager.Instance;

			LeftHand = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
			RightHand = manager.GetRawSkeletonJointPos (userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight);

			Debug.Log ("LeftHand: x-" + LeftHand.x + ", y-" + LeftHand.y + ", z-" + LeftHand.z);
			Debug.Log ("RightHand: x-" + RightHand.x + ", y-" + RightHand.y + ", z-" + RightHand.z);
		}

		/*Debug.Log ("GestureCompleted: " + gesture);

		Debug.Log ("SL: " + (gesture == KinectGestures.Gestures.SwipeLeft));
		Debug.Log ("SR: " + (gesture == KinectGestures.Gestures.SwipeRight));


		string sGestureText = gesture + " detected";*/
		/*if (GestureInfo != null)
        {
            GestureInfo.GetComponent<GUIText>().text = sGestureText;
        }*/

		/*if (gesture == KinectGestures.Gestures.SwipeLeft) {
			// swipeLeft = true;
			GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Swipe Left";
			Debug.Log ("Swipe Left");

		} else if (gesture == KinectGestures.Gestures.SwipeRight) {
			//  swipeRight = true;
			GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Swipe Right";
			Debug.Log ("Swipe Right");
		} else if (gesture == KinectGestures.Gestures.RaiseLeftHand) {
			GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Left hand raised";
			Debug.Log ("Left hand raised");
		} else if (gesture == KinectGestures.Gestures.RaiseRightHand) {
			GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Right hand raised";
			Debug.Log ("Right hand raised");*/
	//	}
		/* else if (gesture == KinectGestures.Gestures.Push)
         {
             GameObject.Find("GestureInfo").GetComponent<Text>().text = "Pushed";
             Debug.Log("Pushed");
         }
         else if (gesture == KinectGestures.Gestures.Pull)
         {
             GameObject.Find("GestureInfo").GetComponent<Text>().text = "Pulled";
             Debug.Log("Pulled");
         }*/
		return true;
	}

	public bool GestureCancelled (uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectWrapper.NuiSkeletonPositionIndex joint)
	{
		// don't do anything here, just reset the gesture state
		return true;
	}
}

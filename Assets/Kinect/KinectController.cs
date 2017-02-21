using UnityEngine;
using System;

public class KinectController : MonoBehaviour {

  KinectManager manager = KinectManager.Instance;

  Vector3 HandLeft;
  Vector3 HandRight;

  bool ballIsHeld;

  void Start() {
    ballIsHeld = false;
  }

  void Update() {

    uint userId = manager.GetPlayer1ID;

    HandLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
    HandRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
  }

  public void PrintHandPoints() {
    Debug.Log("HandLeft: " + HandLeft);
    Debug.Log("HandRight: " + HandRight);

    float HandDifferences = HandLeft.x - HandRight.x;

    float ballWidth = 0.26f; //Ball Width is 0.26 cm
    float inch = 0.0254; //1 inch equals 2.5 cm.

    if (HandDifferences > (ballWidth - inch) && HandDifferences < (ballWidth + inch) ) {
      ballIsHeld = true;
      while(ballIsHeld) {
		Debug.Log ("Picked up ball");
      }
    }

  }

}

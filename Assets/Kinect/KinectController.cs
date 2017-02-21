using UnityEngine;
using System;

public class KinectController : MonoBehaviour {

  KinectManager manger;

  Vector3 HandLeft;
  Vector3 HandRight;

  bool ballIsHeld;

  public Start() {
    manager = KinectManager.instance;
    ballIsHeld = false;
  }

  public Update() {

    uint userId = manager.GetPlayer1ID;

    HandLeft = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
    HandRight = manager.GetRawSkeletonJointPos(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
  }

  public PrintHandPoints() {
    Debug.log("HandLeft: " + HandLeft);
    Debug.log("HandRight: " + HandRight);

    float HandDifferences = HandLeft.x - HandRight.x;

    float ballWidth = 0.26f; //Ball Width is 0.26 cm
    float inch = 0.0254; //1 inch equals 2.5 cm.

    if (HandDifferences > (ballWidth - inch) && HandDifferences < (ballWidth + inch) ) {
      ballIsHeld = true;
      while(ballIsHeld) {
        Debug.log("Picked up ball")
      }
    }

  }

}

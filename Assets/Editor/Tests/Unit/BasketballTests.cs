﻿using NUnit.Framework;
using UnityEngine;

[TestFixture]

public class BasketballTests {

    GameObject basketball = GameObject.Find("Basketball");

	[TearDown] public void Cleanup()
	{
		HUDTests.ClearHUD ();
	}

    [Test]
	public void ResetBall_CorrectPosition()
    {
        //tests to make sure that the ball starts in the correct location
        Basketball.instance.ResetBall();
        Assert.AreEqual(new Vector3(0.02f, 5.08f, 9.96f), basketball.transform.position);
    }

    [Test]
    public void ResetBall_Gravity_Off()
    {
        Basketball.instance.ResetBall();
        Assert.AreEqual(false, basketball.GetComponent<Rigidbody>().useGravity);
    }
}

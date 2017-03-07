using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ANN_CPU : MonoBehaviour {

	public static NeuralNetwork ANN = new NeuralNetwork ();
	public static double[,] ANNTrainingData = new double[60 ,34];

	void Awake() {
		if (!Database.ANNTrainingDataRetrieved)
			ANNTrainingData = Database.ANNTrainingData();
		}

	public static void TestANNClassifier() {
		double[] testGesture = new double[30];
		testGesture [0] = 0.0681107267737389;
		testGesture [1] = 0.0527109540998936;
		testGesture [2] = -0.40911591053009;
		testGesture [3] = -0.0343372523784637;
		testGesture [4] = -0.0308329351246357; 
		testGesture [5] = -0.111732840538025; 
		testGesture [6] = -0.0809760242700577; 
		testGesture [7] = -0.0612036883831024; 
		testGesture [8] = -0.269070029258728; 
		testGesture [9] = 0.0221923142671585; 
		testGesture [10] = -0.180041216313839; 
		testGesture [11] = -0.145070791244507; 
		testGesture [12] = -0.0931209623813629; 
		testGesture [13] = -0.272077839821577; 
		testGesture [14] = -0.52587366104126; 
		testGesture [15] = -0.102569233626127; 
		testGesture [16] = 0.0636158566921949; 
		testGesture [17] = -0.406136989593506; 
		testGesture [18] = 0.0235675945878029; 
		testGesture [19] = 0.0380370188504457; 
		testGesture [20] = -0.10601866245269; 
		testGesture [21] = 	0.069388885051012; 
		testGesture [22] = -0.098872072994709; 
		testGesture [23] = -0.188705563545227; 
		testGesture [24] = -0.031746372580528; 
		testGesture [25] = -0.207168068736792; 
		testGesture [26] = -0.209166646003723; 
		testGesture [27] = 0.0612101070582867; 
		testGesture [28] = -0.268003122881055; 
		testGesture [29] = -0.503890872001648;

		StartANN (testGesture);
	}

	public static void StartANN(double[] trackedSkeletalPoints) {
		try {
			ANN.Initialise (30, 4, 4);
			ANN.SetLearningRate (0.2);
			ANN.SetMomentum (true, 0.9);
			TrainANN ();
			TestANN (trackedSkeletalPoints);
		} catch (Exception e) {
			Debug.Log ("An error occurred when starting ANN: " + e.Message);
		}
	}

	private static void TrainANN() {
		try {
			double error = 1;
			int c = 0;

			while ((error > 0.05) && (c < 50000)) {
				error = 0;
				c++;

				for (int i = 0; i < 40; i++) {

					//set the input values from the database - first 30 columns
					for (int a = 0; a < 30; a++) {
						ANN.SetInput (a, ANNTrainingData [i, a]);
					}

					//set the output values from the database - last 4 columns
					for (int a = 0; a < 4; a++) {
						ANN.SetDesiredOutput(a, ANNTrainingData[i, (a+30)]);
					}

					ANN.FeedForward ();
					error += ANN.CalculateError ();
					ANN.BackPropagate ();
				}
				error = error / 40.0f;
			}
		} catch (Exception e) {
			Debug.Log ("An error occurred when training ANN: " + e.Message);
		}
	}

	private static void TestANN(double[] trackedSkeletalPoints) {
		try {

			for (int i = 0; i < 30; i++)
				ANN.SetInput (i, trackedSkeletalPoints [i]);

			ANN.FeedForward ();

			double max = -1000.0;
			int index = -1000;

			for (int j = 0; j < 4; j++) {
				switch (j) {
				case 0:
					Debug.Log ("Stationary / Null Gesture: " + ANN.GetOutput (j));
					break;
				case 1:
					Debug.Log ("Professional Throw: " + ANN.GetOutput (j));
					break;
				case 2:
					Debug.Log ("Chest Throw: " + ANN.GetOutput (j));
					break;
				case 3:
					Debug.Log ("Low Throw: " + ANN.GetOutput (j));
					break;
				}

				if (max < ANN.GetOutput (j)) {
					max = ANN.GetOutput (j);
					index = j;
				}
			}

			Debug.Log ("");

			string Gesture = "";

			switch (index) {
			case 0:
				Gesture = "Stationary / Null";
				break;
			case 1:
				Gesture = "Professional Throw";
				break;
			case 2:
				Gesture = "Chest Throw";
				break;
			case 3:
				Gesture = "Low Throw";
				break;
			}

			GameObject.Find ("GestureInfo").GetComponent<Text> ().text = "Gesture: " + Gesture;


		} catch (Exception e) {
			Debug.Log ("An error occurred when testing ANN: " + e.Message);
		}
	}
}

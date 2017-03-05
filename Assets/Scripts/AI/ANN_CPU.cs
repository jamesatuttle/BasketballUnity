using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ANN_CPU : MonoBehaviour {

	public static NeuralNetwork ANN = new NeuralNetwork ();
	public static double[,] ANNTrainingData = new double[40 ,34];
	//public static double[] trackedSkeletalPoints = new double[15];

	void Awake() {
		if (!Database.ANNTrainingDataRetrieved)
			ANNTrainingData = Database.ANNTrainingData();
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

					//set the input values from the database - first 15 columns
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

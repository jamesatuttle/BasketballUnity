using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ANN_CPU : MonoBehaviour {

	public static NeuralNetwork ANN = new NeuralNetwork ();
	//ublic static double[,] ANNTrainingData = new double[Database.GetNoRows(), Database.GetNoCols()];

	private static double[,] TrainingData = new double[Database.GetNoRows (), Database.GetNoCols ()];

	private static double[] NetworkOutputTest = new double[6];

	private static int CorrectClassifications;
	private static int IncorrectClassifications;

	private static void TestNetworkOutput(string output) {
		if (output == "stationary") {
			NetworkOutputTest [0] = 2.02364102005959;
			NetworkOutputTest [1] = 8.13029194250703;
			NetworkOutputTest [2] = 52.4428009986877;
			NetworkOutputTest [3] = 2.02364102005959;
			NetworkOutputTest [4] = 8.13029194250703;
			NetworkOutputTest [5] = 52.4428009986877;
		}

		if (output == "professional") {
			NetworkOutputTest [0] = 2.93368268758059;
			NetworkOutputTest [0] = 65.8926010131836;
			NetworkOutputTest [0] = -5.86624145507813;
			NetworkOutputTest [0] = 2.93368268758059;
			NetworkOutputTest [0] = 65.8926010131836;
			NetworkOutputTest [0] = -5.86624145507813;
		}
		
		if (output == "chest") {
			NetworkOutputTest [0] = 13.5059632360935;
			NetworkOutputTest [1] = 34.9615275859833;
			NetworkOutputTest [2] = 9.62686538696289;
			NetworkOutputTest [3] = 13.5059632360935;
			NetworkOutputTest [4] = 34.9615275859833;
			NetworkOutputTest [5] = 9.62686538696289;
		}

		if (output == "low") {
			NetworkOutputTest [0] = 12.5134551897645;
			NetworkOutputTest [0] = -27.7184799313545; 
			NetworkOutputTest [0] = 2.25948095321655;
			NetworkOutputTest [0] = 12.5134551897645;
			NetworkOutputTest [0] = -27.7184799313545;
			NetworkOutputTest [0] = 2.25948095321655;
		}
	}

	public static void CollectTrainingData() {
		TrainingData = Database.ANNTrainingData ();

		/*Debug.Log ("TrainingData");

		for (int r = 0; r < Database.GetNoRows (); r++) {
			/Debug.Log("Training Data " + r + ": " + 
				TrainingData[r, 0] + ", " + 
				TrainingData[r, 1] + ", " + 
				TrainingData[r, 2] + ", " + 
				TrainingData[r, 3] + ", " + 
				TrainingData[r, 4] + ", " + 
				TrainingData[r, 5] + ", " +
				TrainingData[r, 6] + ", " +
				TrainingData[r, 7] + ", " +
				TrainingData[r, 8] + ", " +
				TrainingData[r, 9] + ";");
		}*/
	}

	public static void StartANN() {
		CollectTrainingData ();
		ANN.Initialise (6, 4, 4);
		ANN.SetLearningRate (0.2);
		ANN.SetMomentum (true, 0.9);
		TrainANN ();
		TestANN ();
	}

	private static void TrainANN() {
		double error = 1;
		int c = 0;

		while ((error > 0.05) && (c < 50000)) {
			for (int i = 0; i < 60; i++) {

				//Set the input values
				ANN.SetInput(0, TrainingData[i, 0]);
				ANN.SetInput(1, TrainingData[i, 1]);
				ANN.SetInput(2, TrainingData[i, 2]);
				ANN.SetInput(3, TrainingData[i, 3]);
				ANN.SetInput(4, TrainingData[i, 4]);
				ANN.SetInput(5, TrainingData[i, 5]);

				//set the desired output values
				ANN.SetDesiredOutput (0, TrainingData [i, 6]);
				ANN.SetDesiredOutput (1, TrainingData [i, 7]);
				ANN.SetDesiredOutput (3, TrainingData [i, 8]);
				ANN.SetDesiredOutput (4, TrainingData [i, 9]);

				//calculate the neuron values
				ANN.FeedForward ();

				error += ANN.CalculateError ();

				ANN.BackPropagate ();
			}
			error = error / 40.0f;
		}
	}

	private static void TestANN() {
		try {
			CorrectClassifications = 0;
			IncorrectClassifications = 0;
			TestClassification ("stationary");
			TestClassification ("professional");
			TestClassification ("chest");
			TestClassification ("low");

			Debug.Log ("");

			Debug.Log ("CorrectClassifications: " + CorrectClassifications);
			Debug.Log ("IncorrectClassifications: " + IncorrectClassifications);
		} catch (Exception e) {
			Debug.LogError ("TestANN failed: " + e.Message);
		}
	}

	private static void TestClassification(string output) {

		string desiredOutput = output;
		string actualOutput = "";

		TestNetworkOutput (desiredOutput);

		for (int i = 0; i < 6; i++) {
			ANN.SetInput (i, NetworkOutputTest [i]);
		}

		ANN.FeedForward ();

		double max = -1000.0;
		int index = -1000;

		Debug.Log (desiredOutput);
		for (int i = 0; i < 4; i++) {
			if (i == 0) Debug.Log ("Stationary: " + ANN.GetOutput (i));
			else if (i == 1) Debug.Log ("Professional: " + ANN.GetOutput (i));
			else if (i == 2) Debug.Log ("Chest: " + ANN.GetOutput (i));
			else if (i == 3) Debug.Log ("Low: " + ANN.GetOutput (i));

			if (max < ANN.GetOutput (i)) {
				max = ANN.GetOutput (i);
				index = i;
			}
		}

		switch (index) {
		case 0:
			actualOutput = "stationary";
			break;
		case 1:
			actualOutput = "professional";
			break;
		case 2:
			actualOutput = "chest";
			break;
		case 3:
			actualOutput = "low";
			break;
		}

		//Debug.Log ("Gesture: " + actualOutput);

		if (actualOutput == desiredOutput) {
			Debug.Log (desiredOutput + ": ****CORRECT CLASSIFICATION*****");
			CorrectClassifications++;
		} else {
			Debug.Log (desiredOutput + ": Incorrect classification. " + actualOutput);
			IncorrectClassifications++;
		}

		Debug.Log ("");

	}



}

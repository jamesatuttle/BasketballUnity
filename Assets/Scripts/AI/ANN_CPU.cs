using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ANN_CPU : MonoBehaviour {

	public static NeuralNetwork ANN = new NeuralNetwork ();
	public static double[,] ANNTrainingData = new double[Database.GetNoRows(), Database.GetNoCols()];

	private static bool dhsldhsd { get; set; }

	void Awake() {
		if (!Database.ANNTrainingDataRetrieved)
			ANNTrainingData = Database.ANNTrainingData();
	}

	public static void PrintTrainingData() {
		Debug.Log ("TrainingData");

		for (int r = 0; r < Database.GetNoRows (); r++) {
			Debug.Log("Training Data " + r + ": " + 
				ANNTrainingData[r, 0] + ", " + 
				ANNTrainingData[r, 1] + ", " + 
				ANNTrainingData[r, 2] + ", " + 
				ANNTrainingData[r, 3] + ", " + 
				ANNTrainingData[r, 4] + ", " + 
				ANNTrainingData[r, 5] + ", " +
				ANNTrainingData[r, 6] + ", " +
				ANNTrainingData[r, 7] + ", " +
				ANNTrainingData[r, 8] + ", " +
				ANNTrainingData[r, 9] + ";");
		}
	}

}

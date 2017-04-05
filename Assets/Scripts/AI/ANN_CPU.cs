using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ANN_CPU : MonoBehaviour {

	public static NeuralNetwork ANN = new NeuralNetwork ();

	private static double[,] TrainingData = new double[Database.GetNoRows (), Database.GetNoCols ()];

 	enum Gestures {
		none = 0,
		professional = 1,
		chest = 2,
		low = 3	
	}

	public static void CollectTrainingData(bool PrintValuesToConsole) {
		TrainingData = Database.ANNTrainingData ();

		int noRows = Database.GetNoRows ();
		int noCols = Database.GetNoCols ();

		string TrainingDataString = "";

		if (PrintValuesToConsole) {
			Debug.Log ("TrainingData");

			for (int r = 0; r < noRows; r++) {
				for (int c = 0; c < noCols; c++)
					TrainingDataString += TrainingData [r, c].ToString () + ", ";
				
				Debug.Log ("Row " + r + ": " + TrainingDataString);
				TrainingDataString = "";
			}
		}
	}
		
	public static void InitialiseANN() {
		CollectTrainingData (false);

		int noInputs = Database.GetNoInputs ();
		int noHidden = 4;
		int noOutputs = Database.GetNoOutputs ();

		ANN.Initialise (noInputs, noHidden, noOutputs);
		ANN.SetLearningRate (0.2);
		ANN.SetMomentum (true, 0.9);
		TrainANN ();
		//TestOutput (100); 
	}

	public static void StartANN(double[] trackedSkeletalPoints) {
		TestANN (trackedSkeletalPoints);
	}

	private static void TrainANN() {
		double error = 1;
		int c = 0;

		int DatabaseRows = Database.GetNoRows ();
		int noInputs = Database.GetNoInputs ();
		int noOutputs = Database.GetNoOutputs ();

		while ((error > 0.05) && (c < 50000)) {
			for (int i = 0; i < DatabaseRows; i++) {

				//Set the input values
				for (int a = 0; a < noInputs; a++)
					ANN.SetInput (a, TrainingData [i, a]);

				//set the desired output values
				for (int a = 0; a < noOutputs; a++)
					ANN.SetDesiredOutput (a, TrainingData [i, noInputs + a]);

				//calculate the neuron values
				ANN.FeedForward ();

				error += ANN.CalculateError ();

				ANN.BackPropagate ();
			}
			error = error / 40.0f;
		}
		print ("ANN TRAINED"); 
	}

	private static void TestANN(double[] trackedSkeletalPoints) {
		int noInputs = Database.GetNoInputs ();

		for (int i = 0; i < noInputs; i++)
			ANN.SetInput (i, trackedSkeletalPoints [i]);
		 
		ANN.FeedForward ();

		double max = -1000.0;
		int index = -1000;

		int NoOutputs = Database.GetNoOutputs ();

		for (int i = 0; i < NoOutputs; i++) {
			/*if (i == (int)Gestures.none) Debug.Log ("Stationary " + ANN.GetOutput (i));
			else if (i == (int)Gestures.professional) Debug.Log ("Professional: " + ANN.GetOutput (i));
			else if (i == (int)Gestures.chest) Debug.Log ("Chest: " + ANN.GetOutput (i)); 
			else if (i == (int)Gestures.low) Debug.Log ("Low: " + ANN.GetOutput (i));*/

			if (max < ANN.GetOutput (i)) {
				max = ANN.GetOutput (i);
				index = i;
			}
		}

		string actualOutput = "";

		switch (index) {
		case (int)Gestures.none:
			actualOutput = "stationary";
			break;
		case (int)Gestures.professional:
			actualOutput = "professional";
			break;
		case (int)Gestures.chest:
			actualOutput = "chest";
			break;
		case (int)Gestures.low:
			actualOutput = "low";
			break;
		}

		KinectController.instance.UpdateCurrentGesture (index);

		GameObject.Find ("GestureInfo").GetComponent<Text> ().text = actualOutput;
	}


	/*
	 * Training Methods to test the output classification accuracy - not used in core game
	 */

	private static double[] NetworkOutputTest = new double[6];

	private static int CorrectClassifications;
	private static int IncorrectClassifications;

	private static void PrintTrackedSkeletalPoints(double[] trackedSkeletalPoints) {

		string SkeletalDataString = "";

		Debug.Log ("TrackedSkeletalPoints");

		for (int i = 0; i < trackedSkeletalPoints.Length; i++)
			SkeletalDataString += trackedSkeletalPoints [i].ToString () + ", ";

		Debug.Log ("Row: " + SkeletalDataString);
		SkeletalDataString = "";
	}

	private static void TestOutput(int TestIterations) {
		try {
			CorrectClassifications = 0;
			IncorrectClassifications = 0;

			for (int i = 0; i < TestIterations; i++) {
				TestClassification (Gestures.none);
				TestClassification (Gestures.professional);
				TestClassification (Gestures.chest);
				TestClassification (Gestures.low);
			}

			float classificationPercent = ((float)CorrectClassifications / ((float)IncorrectClassifications + (float)CorrectClassifications)) * 100f;

			Debug.Log("Classification: " + classificationPercent + "%");
		} catch (Exception e) {
			Debug.LogError ("TestANN failed: " + e.Message);
		}
	}

	private static void TestNetworkOutput(Gestures gesture) {

		switch (gesture) {
		case Gestures.none: 
			NetworkOutputTest = new double[30] {
				0.0687557198107243, 0.0248240418732166, -0.408128499984741, -0.0642720684409142, -0.0192504804581404, -0.176110029220581, -0.045895978808403, -0.0871505867689848, -0.199944376945496, 0.0168128460645676, -0.186398945748806, -0.147649168968201, -0.0933552011847496, -0.292800012975931, -0.523703575134277, -0.112553078681231, 0.040256192907691, -0.429516196250916, 0.0105818137526512, -0.00899452436715364, -0.0616351366043091, 0.0764904394745827, -0.0726989442482591, -0.260707497596741, -0.0273382291197777, -0.20012441650033, -0.201064348220825, 0.0597340241074562, -0.281817885115743, -0.523406982421875
			};
			break;
		case Gestures.professional:
			NetworkOutputTest = new double[30] {
				0.0642741867341101, 0.634557396173477, -0.0143301486968994, -0.0309341996908188, 0.0646600425243378, 0.0135655403137207, -0.112168170511723, 0.182145744562149, 0.0270917415618896, 0.0591079145669937, 0.122932508587837, -0.198066234588623, -0.0839944556355476, 0.369738295674324, -0.157408952713013, -0.0664132214151323, 0.633228093385696, 0.0148499011993408, 0.029090404510498, 0.0662490427494049, 0.0388157367706299, 0.0895037055015564, 0.184565871953964, 0.0580513477325439, -0.0446334183216095, 0.105286836624146, -0.216332912445068, 0.0739606916904449, 0.356101751327515, -0.119465827941895
			};
			break;
		case Gestures.chest:
			NetworkOutputTest = new double[30] {
				0.0553591940551996, 0.37327853962779, -0.0794702768325806, -0.03444754332304, 0.0690614879131317, 0.068225622177124, -0.171784453094006, 0.20348597317934, -0.167067289352417, 0.0925774872303009, -0.20694524794817, -0.0620447397232056, -0.113654509186745, 0.0656022131443024, -0.160886406898499, -0.0533151756972075, 0.248602207750082, -0.150133967399597, 0.0633058920502663, 0.0301060527563095, -0.0470124483108521, 0.157270163297653, 0.097552701830864, -0.124802112579346, -0.0982635915279388, -0.197117432951927, -0.0683578252792358, 0.122312463819981, -0.0694586783647537, -0.240172386169434
			};
			break;
		case Gestures.low:
			NetworkOutputTest = new double[30] {
				0.103116890415549, -0.0981985926628113, -0.207880854606628, -0.0638958290219307, -0.0525274276733398, -0.126654028892517, -0.0984259247779846, -0.150410529226065, -0.173997282981873, 0.0980081409215927, -0.207762245088816, -0.0161206722259521, -0.0643136128783226, -0.41070020198822, -0.316771984100342, -0.113849909976125, -0.123266935348511, -0.208694219589233, -0.00282388925552368, -0.066311240196228, -0.0439256429672241, 0.148953527212143, -0.157951023429632, -0.239982843399048, -0.0805334746837616, -0.206183705478907, -0.0207666158676147, 0.0655961632728577, -0.430445969104767, -0.304675102233887		
			};
			break;
		}
	}

	private static void TestClassification(Gestures output) {

		string actualOutput = "";
		string desiredOutput = "";

		int noInputs = Database.GetNoInputs ();

		switch (output) {
			case Gestures.none:
				desiredOutput = "stationary";
				break;
			case Gestures.professional:
				desiredOutput = "professional";
				break;
			case Gestures.chest:
				desiredOutput = "chest";
				break;
			case Gestures.low:
				desiredOutput = "low";
				break;
		}
			
		TestNetworkOutput (output);

		for (int i = 0; i < noInputs; i++) {
			ANN.SetInput (i, NetworkOutputTest [i]);
		}

		ANN.FeedForward ();

		double max = -1000.0;
		int index = -1000;

		int NoOutputs = Database.GetNoOutputs ();

		Debug.Log ("** " + desiredOutput);
		for (int i = 0; i < NoOutputs; i++) {
			if (i == (int)Gestures.none) Debug.Log ("Stationary: " + ANN.GetOutput (i));
			else if (i == (int)Gestures.professional) Debug.Log ("Professional: " + ANN.GetOutput (i));
			else if (i == (int)Gestures.chest) Debug.Log ("Chest: " + ANN.GetOutput (i));
			else if (i == (int)Gestures.low) Debug.Log ("Low: " + ANN.GetOutput (i));

			if (max < ANN.GetOutput (i)) {
				max = ANN.GetOutput (i);
				index = i;
			}
		}

		switch (index) {
		case (int)Gestures.none:
			actualOutput = "stationary";
			break;
		case (int)Gestures.professional:
			actualOutput = "professional";
			break;
		case (int)Gestures.chest:
			actualOutput = "chest";
			break;
		case (int)Gestures.low:
			actualOutput = "low";
			break;
		}

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

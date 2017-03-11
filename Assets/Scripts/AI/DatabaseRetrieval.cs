using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Data.OleDb;

public class Database {

	private static string construct = "URI=file:" + Application.dataPath + "\\TrainingData.db";
	private static IDbConnection dbConnection;
	private static IDbCommand dbCommand;
	private static IDataReader dataReader;
	private static int noRows = 60;
	private static int noInputs = 30;
	private static int noOutputs = 4;

	public static double[,] ANNTrainingData() {
		string tableName = "ANNTrainingData";

		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = "SELECT * FROM " + tableName;
		dataReader = dbCommand.ExecuteReader (); 

		int rowCount = 0;
		double[,] TrainingDataArray = new double[noRows, noInputs + noOutputs];

		while(dataReader.Read()) {

			//Right hand input values
			double RightHand_HipX = Convert.ToDouble (dataReader ["RightHand_HipX"]);
			double RightHand_HipY = Convert.ToDouble (dataReader ["RightHand_HipY"]);
			double RightHand_HipZ = Convert.ToDouble (dataReader ["RightHand_HipZ"]);
			double RightHand_RightWristX = Convert.ToDouble(dataReader["RightHand_RightWristX"]);
			double RightHand_RightWristY = Convert.ToDouble(dataReader["RightHand_RightWristY"]);
			double RightHand_RightWristZ = Convert.ToDouble(dataReader["RightHand_RightWristZ"]);
			double RightWrist_RightElbowX = Convert.ToDouble(dataReader["RightWrist_RightElbowX"]);
			double RightWrist_RightElbowY = Convert.ToDouble(dataReader["RightWrist_RightElbowY"]);
			double RightWrist_RightElbowZ = Convert.ToDouble(dataReader["RightWrist_RightElbowZ"]);
			double RightElbow_RightShoulderX = Convert.ToDouble(dataReader["RightElbow_RightShoulderX"]);
			double RightElbow_RightShoulderY = Convert.ToDouble(dataReader["RightElbow_RightShoulderY"]);
			double RightElbow_RightShoulderZ = Convert.ToDouble(dataReader["RightElbow_RightShoulderZ"]);
			double RightHand_RightShoulderX = Convert.ToDouble(dataReader["RightHand_RightShoulderX"]);
			double RightHand_RightShoulderY = Convert.ToDouble(dataReader["RightHand_RightShoulderY"]);
			double RightHand_RightShoulderZ = Convert.ToDouble(dataReader["RightHand_RightShoulderZ"]);

			//Left hand input values
			double LeftHand_HipX = Convert.ToDouble (dataReader ["LeftHand_HipX"]);
			double LeftHand_HipY = Convert.ToDouble (dataReader ["LeftHand_HipY"]);
			double LeftHand_HipZ = Convert.ToDouble (dataReader ["LeftHand_HipZ"]);
			double LeftHand_LeftWristX = Convert.ToDouble(dataReader["LeftHand_LeftWristX"]);
			double LeftHand_LeftWristY = Convert.ToDouble(dataReader["LeftHand_LeftWristY"]);
			double LeftHand_LeftWristZ = Convert.ToDouble(dataReader["LeftHand_LeftWristZ"]);
			double LeftWrist_LeftElbowX = Convert.ToDouble(dataReader["LeftWrist_LeftElbowX"]);
			double LeftWrist_LeftElbowY = Convert.ToDouble(dataReader["LeftWrist_LeftElbowY"]);
			double LeftWrist_LeftElbowZ = Convert.ToDouble(dataReader["LeftWrist_LeftElbowZ"]);
			double LeftElbow_LeftShoulderX = Convert.ToDouble(dataReader["LeftElbow_LeftShoulderX"]);
			double LeftElbow_LeftShoulderY = Convert.ToDouble(dataReader["LeftElbow_LeftShoulderY"]);
			double LeftElbow_LeftShoulderZ = Convert.ToDouble(dataReader["LeftElbow_LeftShoulderZ"]);
			double LeftHand_LeftShoulderX = Convert.ToDouble(dataReader["LeftHand_LeftShoulderX"]);
			double LeftHand_LeftShoulderY = Convert.ToDouble(dataReader["LeftHand_LeftShoulderY"]);
			double LeftHand_LeftShoulderZ = Convert.ToDouble(dataReader["LeftHand_LeftShoulderZ"]);

			//output values
			double Stationary = Convert.ToDouble (dataReader ["Stationary"]);
			double Professional_Throw = Convert.ToDouble (dataReader ["Professional_Throw"]);
			double Chest_Throw = Convert.ToDouble (dataReader ["Chest_Throw"]);
			double Low_Throw = Convert.ToDouble (dataReader ["Low_Throw"]);


			TrainingDataArray [rowCount, 0] = RightHand_HipX;
			TrainingDataArray [rowCount, 1] = RightHand_HipY;
			TrainingDataArray [rowCount, 2] = RightHand_HipZ;
			TrainingDataArray [rowCount, 3] = RightHand_RightWristX;
			TrainingDataArray [rowCount, 4] = RightHand_RightWristY;
			TrainingDataArray [rowCount, 5] = RightHand_RightWristZ;
			TrainingDataArray [rowCount, 6] = RightWrist_RightElbowX;
			TrainingDataArray [rowCount, 7] = RightWrist_RightElbowY;
			TrainingDataArray [rowCount, 8] = RightWrist_RightElbowZ;
			TrainingDataArray [rowCount, 9] = RightElbow_RightShoulderX;
			TrainingDataArray [rowCount, 10] = RightElbow_RightShoulderY;
			TrainingDataArray [rowCount, 11] = RightElbow_RightShoulderZ;
			TrainingDataArray [rowCount, 12] = RightHand_RightShoulderX;
			TrainingDataArray [rowCount, 13] = RightHand_RightShoulderY;
			TrainingDataArray [rowCount, 14] = RightHand_RightShoulderZ;

			TrainingDataArray [rowCount, 15] = LeftHand_HipX;
			TrainingDataArray [rowCount, 16] = LeftHand_HipY;
			TrainingDataArray [rowCount, 17] = LeftHand_HipZ;
			TrainingDataArray [rowCount, 18] = LeftHand_LeftWristX;
			TrainingDataArray [rowCount, 19] = LeftHand_LeftWristY;
			TrainingDataArray [rowCount, 20] = LeftHand_LeftWristZ;
			TrainingDataArray [rowCount, 21] = LeftWrist_LeftElbowX;
			TrainingDataArray [rowCount, 22] = LeftWrist_LeftElbowY;
			TrainingDataArray [rowCount, 23] = LeftWrist_LeftElbowZ;
			TrainingDataArray [rowCount, 24] = LeftElbow_LeftShoulderX;
			TrainingDataArray [rowCount, 25] = LeftElbow_LeftShoulderY;
			TrainingDataArray [rowCount, 26] = LeftElbow_LeftShoulderZ;
			TrainingDataArray [rowCount, 27] = LeftHand_LeftShoulderX;
			TrainingDataArray [rowCount, 28] = LeftHand_LeftShoulderY;
			TrainingDataArray [rowCount, 29] = LeftHand_LeftShoulderZ;

			TrainingDataArray [rowCount, 30] = Stationary;
			TrainingDataArray [rowCount, 31] = Professional_Throw;
			TrainingDataArray [rowCount, 32] = Chest_Throw;
			TrainingDataArray [rowCount, 33] = Low_Throw;

			rowCount++;
		}

		dbConnection.Close ();

		return TrainingDataArray;
	}

	public static void printTrainingData() {
		double[,] TrainingData = ANNTrainingData ();

		int rowMax = noRows;
		int colMax = noInputs + noOutputs;

		Debug.Log ("");

		for (int row = 0; row < rowMax; row++) {
			Debug.Log ("Row: " + row);
			for (int col = 0; col < colMax; col++) {
				Debug.Log (TrainingData [row, col].ToString ());
			}
		}
	}

	public static int GetNoRows() {
		return noRows;
	}

	public static int GetNoCols() {
		return noInputs + noOutputs;
	}

	public static int GetNoInputs() {
		return noInputs;
	}

	public static int GetNoOutputs() {
		return noOutputs;
	}
		
}

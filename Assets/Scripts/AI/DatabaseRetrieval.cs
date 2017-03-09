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
	public static bool ANNTrainingDataRetrieved = false;
	private static int noRows = 60;
	private static int noInputs = 6;
	private static int noOutputs = 4;

	public static double[,] ANNTrainingData() {
		string tableName = "ANNTrainingData";
		//string tableName = "TrainingData";

		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = "SELECT * FROM " + tableName;
		dataReader = dbCommand.ExecuteReader (); 

		int rowCount = 0;
		double[,] TrainingDataArray = new double[noRows, noInputs + noOutputs];

		while(dataReader.Read()) {

			double HandsX = Convert.ToDouble (dataReader ["HandsX"]);
			double HandsY = Convert.ToDouble (dataReader ["HandsY"]);
			double HandsZ = Convert.ToDouble (dataReader ["HandsZ"]);
			/*double RightHand_RightWristX = Convert.ToDouble(dataReader["RightHand_RightWristX"]);
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
			double RightHand_RightShoulderZ = Convert.ToDouble(dataReader["RightHand_RightShoulderZ"]);*/

			double RightHand_HipX = Convert.ToDouble (dataReader ["RHandHipX"]);
			double RightHand_HipY = Convert.ToDouble (dataReader ["RHandHipY"]);
			double RightHand_HipZ = Convert.ToDouble (dataReader ["RHandHipZ"]);

			double LeftHand_HipX = Convert.ToDouble (dataReader ["LHandHipX"]);
			double LeftHand_HipY = Convert.ToDouble (dataReader ["LHandHipY"]);
			double LeftHand_HipZ = Convert.ToDouble (dataReader ["LHandHipZ"]);
			/*double LeftHand_LeftWristX = Convert.ToDouble(dataReader["LeftHand_LeftWristX"]);
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
			double LeftHand_LeftShoulderZ = Convert.ToDouble(dataReader["LeftHand_LeftShoulderZ"]);*/

			double Stationary = Convert.ToDouble (dataReader ["A"]);
			double Professional_Throw = Convert.ToDouble (dataReader ["B"]);
			double Chest_Throw = Convert.ToDouble (dataReader ["C"]);
			double Low_Throw = Convert.ToDouble (dataReader ["D"]);

			TrainingDataArray [rowCount, 0] = HandsX;
			TrainingDataArray [rowCount, 1] = HandsY;
			TrainingDataArray [rowCount, 2] = HandsZ;

			TrainingDataArray [rowCount, 3] = RightHand_HipX;
			TrainingDataArray [rowCount, 4] = RightHand_HipY;
			TrainingDataArray [rowCount, 5] = RightHand_HipZ;
			/*TrainingDataArray [rowCount, 3] = RightHand_RightWristX;
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
			TrainingDataArray [rowCount, 14] = RightHand_RightShoulderZ;*/

			TrainingDataArray [rowCount, 6] = LeftHand_HipX;
			TrainingDataArray [rowCount, 7] = LeftHand_HipY;
			TrainingDataArray [rowCount, 8] = LeftHand_HipZ;
			/*TrainingDataArray [rowCount, 18] = LeftHand_LeftWristX;
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
			TrainingDataArray [rowCount, 29] = LeftHand_LeftShoulderZ;*/

			TrainingDataArray [rowCount, 9] = Stationary;
			TrainingDataArray [rowCount, 10] = Professional_Throw;
			TrainingDataArray [rowCount, 11] = Chest_Throw;
			TrainingDataArray [rowCount, 12] = Low_Throw;

			rowCount++;
		}

		dbConnection.Close ();

		ANNTrainingDataRetrieved = true;
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

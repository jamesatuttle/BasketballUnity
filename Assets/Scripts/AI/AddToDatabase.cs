using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Data.OleDb;

public class AddToDatabase {

	// private static string construct = ;
	private static IDbConnection dbConnection;
	private static IDbCommand dbCommand;
	private static IDataReader dataReader;

	public static void addToANNTrainingData(
		double RightHand_HipX,
		double RightHand_HipY,
		double RightHand_HipZ,
		double RightHand_RightWristX,
		double RightHand_RightWristY,
		double RightHand_RightWristZ,
		double RightWrist_RightElbowX,
		double RightWrist_RightElbowY,
		double RightWrist_RightElbowZ,
		double RightElbow_RightShoulderX,
		double RightElbow_RightShoulderY,
		double RightElbow_RightShoulderZ,
		double RightHand_RightShoulderX,
		double RightHand_RightShoulderY,
		double RightHand_RightShoulderZ,
		double LeftHand_HipX,
		double LeftHand_HipY,
		double LeftHand_HipZ,
		double LeftHand_LeftWristX,
		double LeftHand_LeftWristY,
		double LeftHand_LeftWristZ,
		double LeftWrist_LeftElbowX,
		double LeftWrist_LeftElbowY,
		double LeftWrist_LeftElbowZ,
		double LeftElbow_LeftShoulderX,
		double LeftElbow_LeftShoulderY,
		double LeftElbow_LeftShoulderZ,
		double LeftHand_LeftShoulderX,
		double LeftHand_LeftShoulderY,
		double LeftHand_LeftShoulderZ,
		string throwType
	) {
		
		//Debug.Log("1");

		double Stationary = 0.0, Professional_Throw = 0.0, Chest_Throw = 0.0, Low_Throw = 0.0;

		//Debug.Log("2");

		switch (throwType) {
		case "Stationary":
			Stationary = 0.9;
			Professional_Throw = 0.1;
			Chest_Throw = 0.1;
			Low_Throw = 0.1;
			//Debug.Log("Null");
			break;
		case "Professional":
			Stationary = 0.1;
			Professional_Throw = 0.9;
			Chest_Throw = 0.1; 
			Low_Throw = 0.1;
			//Debug.Log("professional");
			break;
		case "Chest":
			Stationary = 0.1;
			Professional_Throw = 0.1;
			Chest_Throw = 0.9; 
			Low_Throw = 0.1;
			//Debug.Log("chest");
			break;
		case "Low":
			Stationary = 0.1;
			Professional_Throw = 0.1;
			Chest_Throw = 0.1;
			Low_Throw = 0.9;
			//Debug.Log("low");
			break;
		default:
			Stationary = 0.1;
			Professional_Throw = 0.1;
			Chest_Throw = 0.1;
			Low_Throw = 0.1;
			//Debug.Log("default");
			break;
		}

		//Debug.Log (throwType);

		try {
			string tableName = "ANNTrainingData";

			//Debug.Log("3");

			dbConnection = new SqliteConnection ("URI=file:" + Application.dataPath + "\\TrainingData.db");

			//Debug.Log("4");

			dbConnection.Open ();

			//Debug.Log("5");

			dbCommand = dbConnection.CreateCommand ();

			//Debug.Log("6");

			dbCommand.CommandText = "insert into ANNTrainingData(" +
				"RightHand_HipX, RightHand_HipY, RightHand_HipZ, " +
				"RightHand_RightWristX, RightHand_RightWristY, RightHand_RightWristZ, " +
				"RightWrist_RightElbowX, RightWrist_RightElbowY, RightWrist_RightElbowZ, " +
				"RightElbow_RightShoulderX, RightElbow_RightShoulderY, RightElbow_RightShoulderZ, " +
				"RightHand_RightShoulderX, RightHand_RightShoulderY, RightHand_RightShoulderZ, " +

				"LeftHand_HipX, LeftHand_HipY, LeftHand_HipZ, " +
				"LeftHand_LeftWristX, LeftHand_LeftWristY, LeftHand_LeftWristZ, " +
				"LeftWrist_LeftElbowX, LeftWrist_LeftElbowY, LeftWrist_LeftElbowZ, " +
				"LeftElbow_LeftShoulderX, LeftElbow_LeftShoulderY, LeftElbow_LeftShoulderZ, " +
				"LeftHand_LeftShoulderX, LeftHand_LeftShoulderY, LeftHand_LeftShoulderZ, " +

				"Stationary, Professional_Throw, Chest_Throw, Low_Throw " +

				") values (" + 

				RightHand_HipX + ", " + RightHand_HipY + ", " + RightHand_HipZ + ", " +
				RightHand_RightWristX + ", " + RightHand_RightWristY + ", " + RightHand_RightWristZ + ", " +
				RightWrist_RightElbowX + ", " + RightWrist_RightElbowY + ", " + RightWrist_RightElbowZ + ", " +
				RightElbow_RightShoulderX + ", " + RightElbow_RightShoulderY + ", " + RightElbow_RightShoulderZ + ", " +
				RightHand_RightShoulderX + ", " + RightHand_RightShoulderY + ", " + RightHand_RightShoulderZ + ", " +

				LeftHand_HipX + ", " + LeftHand_HipY + ", " + LeftHand_HipZ + ", " +
				LeftHand_LeftWristX + ", " + LeftHand_LeftWristY + ", " + LeftHand_LeftWristZ + ", " +
				LeftWrist_LeftElbowX + ", " + LeftWrist_LeftElbowY + ", " + LeftWrist_LeftElbowZ + ", " +
				LeftElbow_LeftShoulderX + ", " + LeftElbow_LeftShoulderY + ", " + LeftElbow_LeftShoulderZ + ", " +
				LeftHand_LeftShoulderX + ", " + LeftHand_LeftShoulderY + ", " + LeftHand_LeftShoulderZ + ", " +

				Stationary + ", " + Professional_Throw + ", " + Chest_Throw + ", " + Low_Throw + ")";




			//Debug.Log("7");

			dbCommand.ExecuteNonQuery();

			//Debug.Log("8");

			dbConnection.Close();

			//Debug.Log("9");

			Debug.Log("Record inserted successfully.");
		} catch (Exception e) {
			Debug.Log ("An error occured");
			Debug.Log ("Error: " + e.Message);
		}
	
	}

	/*public static double formatData(double value) {

		int length = 10;

		float floatVal = (float)value;

		float val = Mathf.Pow (10.0f, (float)length);

		float newVal = Mathf.Round (floatVal * val) / val;

		return (double)newVal;
	}*/

}

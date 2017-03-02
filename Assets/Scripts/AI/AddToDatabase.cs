using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data.OleDb;

public class AddToDatabase {

	public static void addToANNTrainingData(
		double handsX, 
		double handsY,
		double handsZ,
		double RHandRShoulderX,
		double RHandRShoulderY,
		double RHandRShoulderZ,
		double LHandLShoulderX,
		double LHandLShoulderY,
		double LHandLShoulderZ,
		double RHandHipX,
		double RHandHipY,
		double RHandHipZ,
		double LHandHipX,
		double LHandHipY,
		double LHandHipZ,
		string throwType) {

		double hhx = formatData (handsX);
		double hhy = formatData (handsY);
		double hhz = formatData (handsZ);
		double rhrsx = formatData (RHandRShoulderX);
		double rhrsy = formatData (RHandRShoulderY);
		double rhrsz = formatData (RHandRShoulderZ);
		double lhlsx = formatData (LHandLShoulderX);
		double lhlsy = formatData (LHandLShoulderY);
		double lhlsz = formatData (LHandLShoulderZ);
		double rhhx = formatData (RHandHipX);
		double rhhy = formatData (RHandHipY);
		double rhhz = formatData (RHandHipZ);
		double lhhx = formatData (LHandHipX);
		double lhhy = formatData (LHandHipY);
		double lhhz = formatData (LHandHipZ);

		double Stationary = 0.0, Professional_Throw = 0.0, Chest_Throw = 0.0, Low_Throw = 0.0;

		switch (throwType) {
		case "Stationary":
			Stationary = 0.9;
			Professional_Throw = 0.0;
			Chest_Throw = 0.0; 
			Low_Throw = 0.0;
			break;
		case "Professional":
			Stationary = 0.0;
			Professional_Throw = 0.9;
			Chest_Throw = 0.0; 
			Low_Throw = 0.0;
			break;
		case "Chest":
			Stationary = 0.0;
			Professional_Throw = 0.0;
			Chest_Throw = 0.9; 
			Low_Throw = 0.0;
			break;
		case "Low":
			Stationary = 0.0;
			Professional_Throw = 0.0;
			Chest_Throw = 0.0; 
			Low_Throw = 0.9;
			break;
		default:
			Stationary = 0.0;
			Professional_Throw = 0.0;
			Chest_Throw = 0.0;
			Low_Throw = 0.0;
			break;
		}

		try {
			string connectionQuery = "Driver={Microsoft Access Driver (*.mdb, *.accdb)}; DBQ=" + Application.dataPath + "/GameData.accdb";

			OleDbConnection connection = new OleDbConnection(@connectionQuery);

			connection.Open();

			OleDbCommand command = new OleDbCommand("insert into ANNTrainingData(" +
				"HandsX, HandsY, HandsZ, " +
				"RHandRShoulderX, RHandRShoulderY, RHandRShoulderZ, " +
				"LHandLShoulderX, LHandLShoulderY, LHandLShoulderZ, " +
				"RHandHipX, RHandHipY, RHandHipZ, " +
				"LHandHipX, LHandHipY, LHandHipZ, " +
				"Stationary, Professional_Throw, Chest_Throw, Low_Throw" +
				")values(" + 
				hhx + ", " + hhy + ", " + hhz + ", " +
				rhrsx + ", " + rhrsy + ", " + rhrsz + ", " +
				lhlsx + ", " + lhlsy + ", " + lhlsz + ", " +
				rhhx + ", " + rhhy + ", " + rhhz + ", " +
				lhhx + ", " + lhhy + ", " + lhhz + 
				Stationary + ", " + Professional_Throw + ", " + Chest_Throw + ", " + Low_Throw + ")", connection);

			command.ExecuteNonQuery();
			Debug.Log("Record inserted successfully.");
			connection.Close();
		} catch (Exception e) {
			Debug.Log ("An error occured");
			//Console.Beep ();
			Debug.Log ("Error: " + e.Message);
		}

		}

	public static double formatData(double value) {

		float floatVal = (float)value;

		float val = Mathf.Pow (10.0f, (float)10);

		float newVal = Mathf.Round (floatVal * val) / val;

		return (double)newVal;
	}



}

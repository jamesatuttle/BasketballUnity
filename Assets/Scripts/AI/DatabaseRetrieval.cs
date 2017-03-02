using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using System.Data.OleDb;

public class Database {

	private static double[,] TrainingData = ANNTrainingData();

	public static double[,] ANNTrainingData()
	{
		string connection = "Driver={Microsoft Access Driver (*.mdb, *.accdb)}; DBQ=" + Application.dataPath + "/GameData.accdb";
		string selectQuery = "SELECT * FROM ANNTrainingData";

		string tableName = "ANNTrainingData";

		DataSet dataSet = new DataSet ();
		OleDbConnection accessConnection = null;

		try {
			accessConnection = new OleDbConnection(selectQuery);
		} catch (Exception e) {
			Debug.Log ("Error: failed to connect to database. " + e.Message);
		}

		try {
			OleDbCommand accessCommand = new OleDbCommand(selectQuery, accessConnection);
			OleDbDataAdapter dataAdapter = new OleDbDataAdapter(accessCommand);

			accessConnection.Open();
			dataAdapter.Fill(dataSet, tableName);

		} catch (Exception e) {
			Debug.Log ("Error: failed to pull data from the database. " + e.Message);
		} finally {
			accessConnection.Close ();
		}

		int rowCount = dataSet.Tables [tableName].Rows.Count;
		int colCount = dataSet.Tables [tableName].Columns.Count;

		double[,] trainingData = new double[rowCount,colCount];

		for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) { 
			for (int colIndex = 0; colIndex < colCount; colIndex++) {
				trainingData[rowIndex, colIndex] = Convert.ToDouble(dataSet.Tables[tableName].Rows[rowIndex][colIndex]);
			}
		}

		return trainingData;
	}

	public static void testDataConnection()
	{
		Console.WriteLine ("First line in Database: ");
		for (int i = 0; i < 9; i++)
			Console.Write (TrainingData [0, i] + ", "); 
	}
}
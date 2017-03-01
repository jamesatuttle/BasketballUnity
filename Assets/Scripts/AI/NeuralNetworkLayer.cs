using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkLayer {

	public int NumberOfNodes;
	public int NumberOfChildNodes;
	public int NumberOfParentNodes;

	public double[][] Weights;
	public double[][] WeightChanges;

	public double[] NeuronValues;
	public double[] DesiredValues;
	public double[] Errors;
	public double[] BiasWeights;
	public double[] BiasValues;

	public double LearningRate;
	public double MomentumFactor;

	public bool LinearOutput;
	public bool UseMomentum;

	NeuralNetworkLayer ParentLayer;
	NeuralNetworkLayer ChildLayer;

	public NeuralNetworkLayer()
	{
		ParentLayer = null;
		ChildLayer = null;
		LinearOutput = false;
		UseMomentum = false;
		MomentumFactor = 0.9;
	}

	public void Initialise(int NumNodes, NeuralNetworkLayer parent, NeuralNetworkLayer child)
	{
		NeuronValues = new double[NumberOfNodes];
		DesiredValues = new double[NumberOfNodes];
		Errors = new double[NumberOfNodes];

		if (parent != null) {
			ParentLayer = parent;
		}

		if (child != null) {
			ChildLayer = child;

			Weights = new double[NumberOfNodes][];
			WeightChanges = new double[NumberOfNodes][];

			for (int i = 0; i < NumberOfNodes; i++)
			{
				Weights[i] = new double[NumberOfChildNodes];
				WeightChanges[i] = new double[NumberOfChildNodes];
			}

			BiasValues = new double[NumberOfChildNodes];
			BiasWeights = new double[NumberOfChildNodes];

		} else {
			Weights = null;
			BiasValues = null;
			BiasWeights = null;
		}

		for (int i = 0; i < NumberOfNodes; i++) {
			NeuronValues [i] = 0;
			DesiredValues [i] = 0;
			Errors [i] = 0;

			if (ChildLayer != null) {
				for (int j = 0; j < NumberOfChildNodes; j++) {
					Weights [i] [j] = 0;
					WeightChanges [i] [j] = 0;
				}
			}
		}

		if (ChildLayer != null) {
			for (int j = 0; j < NumberOfChildNodes; j++) {
				BiasValues [j] = -1;
				BiasWeights [j] = 0;
			}
		}
	}

	public void RandomiseWeights() 
	{
		int min = 0;
		int max = 200;
		int number;

		//Random rand = new Random ();

		for (int i = 0; i < NumberOfNodes; i++) {
			for (int j = 0; j < NumberOfChildNodes; j++) {

				//number = (((Mathf.Abs (rand.Next ()) % (max - min + 1)) + min));

				number = Mathf.Abs (Random.Range(min, max));

				if (number > max)
					number = max;

				if (number < min)
					number = min;

				Weights [i] [j] = number / 100.0f - 1;
			}
		}

		for (int j = 0; j < NumberOfChildNodes; j++) {
			
			number = Mathf.Abs (Random.Range(min, max));

			if (number > max)
				number = max;

			if (number < min)
				number = min;

			BiasWeights [j] = number / 100.0f - 1;
		}
	}

	public void CalculateErrors()
	{
		double sum;

		if (ChildLayer == null) {
			for (int i = 0; i < NumberOfNodes; i++) {
				Errors [i] = (DesiredValues [i] - NeuronValues [i]) * NeuronValues [i] * (1.0f - NeuronValues [i]);
			}
		} else if (ParentLayer == null) {
			for (int i = 0; i < NumberOfNodes; i++) {
				Errors [i] = 0.0f;
			}
		} else {
			for (int i = 0; i < NumberOfNodes; i++) {
				sum = 0;

				for (int j = 0; j < NumberOfChildNodes; j++) {
					sum += ChildLayer.Errors[j] * Weights[i][j];
				}
				Errors [i] = sum * NeuronValues [i] * (1.0f - NeuronValues [i]);
			}
		}
	}

	public void AdjustWeights()
	{
		if (ChildLayer != null) {

			for (int i = 0; i < NumberOfNodes; i++) {

				for (int j = 0; j < NumberOfChildNodes; j++) {

					double dw = LearningRate * ChildLayer.Errors [j] * NeuronValues [i];
					Weights [i] [j] += dw + MomentumFactor * WeightChanges [i] [j];
					WeightChanges [i] [j] = dw;
				}
			}

			for (int i = 0; i < NumberOfChildNodes; i++) {
				BiasWeights [i] += LearningRate * ChildLayer.Errors [i] * BiasValues [i];
			}
		}
	}

	public void CalculateNeuronValues()
	{
		if (ParentLayer != null) {
			for (int j = 0; j < NumberOfNodes; j++) {

				double x = 0;
				for (int i = 0; i < NumberOfParentNodes; i++) {
					x += ParentLayer.NeuronValues [i] * ParentLayer.Weights [i] [j];
				}

				x += ParentLayer.BiasValues [j] * ParentLayer.BiasWeights [j];

				if ((ChildLayer == null) && LinearOutput)
					NeuronValues [j] = x;
				else {

					float negX = -((float)x);

					NeuronValues [j] = 1.0f / (1.0f + Mathf.Exp (negX));
				}
			}
		}
	}

}

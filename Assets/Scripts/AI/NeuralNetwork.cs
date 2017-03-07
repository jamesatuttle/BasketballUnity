﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NeuralNetwork {

	public NeuralNetworkLayer InputLayer = new NeuralNetworkLayer();
	public NeuralNetworkLayer HiddenLayer = new NeuralNetworkLayer();
	public NeuralNetworkLayer OutputLayer = new NeuralNetworkLayer();

	public NeuralNetwork() {
		NeuralNetworkLayer InputLayer = new NeuralNetworkLayer();
		NeuralNetworkLayer HiddenLayer = new NeuralNetworkLayer();
		NeuralNetworkLayer OutputLayer = new NeuralNetworkLayer();
	}

	public void Initialise(int nNodesInput, int nNodesHidden, int nNodesOutput) {
		InputLayer.NumberOfNodes = nNodesInput;
		InputLayer.NumberOfChildNodes = nNodesHidden;
		InputLayer.NumberOfParentNodes = 0;
		InputLayer.Initialise(nNodesInput, null, HiddenLayer);
		InputLayer.RandomiseWeights();

		HiddenLayer.NumberOfNodes = nNodesHidden;
		HiddenLayer.NumberOfChildNodes = nNodesOutput;
		HiddenLayer.NumberOfParentNodes = nNodesInput;
		HiddenLayer.Initialise(nNodesHidden, InputLayer, OutputLayer);
		HiddenLayer.RandomiseWeights();

		OutputLayer.NumberOfNodes = nNodesOutput;
		OutputLayer.NumberOfChildNodes = 0;
		OutputLayer.NumberOfParentNodes = nNodesHidden;
		OutputLayer.Initialise(nNodesOutput, HiddenLayer, null);
	}

	public void SetInput(int i, double value) {
		if ((i >= 0) && (i < InputLayer.NumberOfNodes))
		{
			InputLayer.NeuronValues[i] = value;
		}
	}

	public double GetOutput(int i) {
		if ((i >= 0) && (i < OutputLayer.NumberOfNodes))
		{
			return OutputLayer.NeuronValues[i];
		}

		return (double)10000; // to indicate an error
	}

	public void SetDesiredOutput(int i, double value) {
		if ((i >= 0) && (i < OutputLayer.NumberOfNodes))
		{
			OutputLayer.DesiredValues[i] = value;
		}
	}

	public void FeedForward() {
		InputLayer.CalculateNeuronValues();
		HiddenLayer.CalculateNeuronValues();
		OutputLayer.CalculateNeuronValues();
	}

	public void BackPropagate() {
		OutputLayer.CalculateErrors();
		HiddenLayer.CalculateErrors();

		HiddenLayer.AdjustWeights();
		InputLayer.AdjustWeights();
	}

	public int GetMaxOutputID() {
		int i, id;
		double maxval;

		maxval = OutputLayer.NeuronValues[0];
		id = 0;

		for (i = 1; i < OutputLayer.NumberOfNodes; i++)
		{
			if (OutputLayer.NeuronValues[i] > maxval)
			{
				maxval = OutputLayer.NeuronValues[i];
				id = i;
			}
		}

		return id;
	}

	public double CalculateError() {
		int i;
		double error = 0;

		for (i = 0; i < OutputLayer.NumberOfNodes; i++)
		{
			error += Math.Pow(OutputLayer.NeuronValues[i] - OutputLayer.DesiredValues[i], 2);
		}

		error = error / OutputLayer.NumberOfNodes;

		return error;
	}

	public void SetLearningRate(double rate) {
		InputLayer.LearningRate = rate;
		HiddenLayer.LearningRate = rate;
		OutputLayer.LearningRate = rate;
	}

	public void SetLinearOutput(bool useLinear) {
		InputLayer.LinearOutput = useLinear;
		HiddenLayer.LinearOutput = useLinear;
		OutputLayer.LinearOutput = useLinear;
	}

	public void SetMomentum(bool useMomentum, double factor) {
		InputLayer.UseMomentum = useMomentum;
		HiddenLayer.UseMomentum = useMomentum;
		OutputLayer.UseMomentum = useMomentum;

		InputLayer.MomentumFactor = factor;
		HiddenLayer.MomentumFactor = factor;
		OutputLayer.MomentumFactor = factor;

	}
}

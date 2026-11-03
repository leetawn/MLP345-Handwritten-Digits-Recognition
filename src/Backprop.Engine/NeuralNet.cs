using System;
using System.IO;

namespace Backprop;

/// <summary>
/// A 3-layer multilayer perceptron (MLP) implementing the backpropagation learning algorithm.
/// </summary>
public class NeuralNet
{
    private INeuron[] ineuron; // input neurons
    private HNeuron[] hneuron; // hidden neurons
    private ONeuron[] oneuron; // output neurons
    private readonly double _lrOut;
    private readonly double _lrIn;
    private double[] errorComponent; // error components (target - actual)
    private double[] errorDerivative; // output error derivatives
    private double[] desiredout; // target one-hot vector

    /// <summary>
    /// Initializes a new instance of <see cref="NeuralNet"/> with default dimensions (3072 inputs, 64 hidden, 10 outputs).
    /// </summary>
    public NeuralNet() : this(3072, 64, 10, 0.2, 0.15)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NeuralNet"/> with custom layer sizes and configurable learning rates.
    /// </summary>
    /// <param name="input">Number of input features.</param>
    /// <param name="hidden">Number of hidden layer neurons.</param>
    /// <param name="output">Number of output layer neurons (classes).</param>
    /// <param name="lrOut">Learning rate applied to hidden-to-output weights and output biases.</param>
    /// <param name="lrIn">Learning rate applied to input-to-hidden weights and hidden biases.</param>
    public NeuralNet(int input, int hidden, int output, double lrOut = 0.2, double lrIn = 0.15)
    {
        _lrOut = lrOut;
        _lrIn = lrIn;
        ineuron = new INeuron[input];
        hneuron = new HNeuron[hidden];
        oneuron = new ONeuron[output];
        errorComponent = new double[output];
        errorDerivative = new double[output];
        desiredout = new double[output];
        createNeurons(ineuron.Length, hneuron.Length, oneuron.Length);
    }

    /// <summary>
    /// Instantiates individual neurons for the input, hidden, and output layers.
    /// </summary>
    /// <param name="i">Number of input neurons.</param>
    /// <param name="h">Number of hidden neurons.</param>
    /// <param name="o">Number of output neurons.</param>
    public void createNeurons(int i, int h, int o)
    {
        for (int x = 0; x < i; x++)
        {
            ineuron[x] = new INeuron(x, h);
        }
        for (int x = 0; x < h; x++)
        {
            hneuron[x] = new HNeuron(x, o);
        }
        for (int x = 0; x < o; x++)
        {
            oneuron[x] = new ONeuron(x);
        }
    }

    /// <summary>
    /// Gets the activation score of an output layer neuron after running the forward pass.
    /// </summary>
    /// <param name="pos">Output neuron index (class index).</param>
    /// <returns>The sigmoid activation value in (0, 1).</returns>
    public double getOuputData(int pos)
    {
        return oneuron[pos].getOActivation();
    }

    /// <summary>
    /// Sets the value of a specific input feature before running the forward pass.
    /// </summary>
    /// <param name="pos">Input feature index.</param>
    /// <param name="data">The scaled input feature value.</param>
    public void setInputs(int pos, double data)
    {
        ineuron[pos].setInput(data);
    }

    /// <summary>
    /// Sets the target desired output activation (e.g. 1.0 or 0.0) for a specific output class.
    /// </summary>
    /// <param name="pos">Output class index.</param>
    /// <param name="data">Target activation value.</param>
    public void setDesiredOutput(int pos, double data)
    {
        desiredout[pos] = data;
    }

    /// <summary>
    /// Standard logistic sigmoid activation function squashing real values into (0, 1).
    /// </summary>
    /// <param name="dat">The linear combination net input (weighted sum plus bias).</param>
    /// <returns>Sigmoid output value.</returns>
    public double sigmoid(double dat)
    {
        if (dat >= 20.00)
        {
            dat = 32;
        }
        return (double)(1.0 / (1.0 + System.Math.Exp(-dat)));
    }

    /// <summary>
    /// Computes activations for all neurons in the hidden layer during the forward pass.
    /// </summary>
    public void calculateHA()
    {
        for (int x = 0; x < hneuron.Length; x++)
        {
            double summation = 0.0;
            for (int y = 0; y < ineuron.Length; y++)
            {
                summation += ineuron[y].getInput() * ineuron[y].getWeight(x);
            }
            hneuron[x].setHactivation(sigmoid(summation + hneuron[x].getBias()));
        }
    }

    /// <summary>
    /// Computes activations for all neurons in the output layer during the forward pass.
    /// </summary>
    public void calculateOA()
    {
        for (int x = 0; x < oneuron.Length; x++)
        {
            double summation = 0.0;
            for (int y = 0; y < hneuron.Length; y++)
            {
                summation += hneuron[y].getHactivation() * hneuron[y].getWeight(x);
            }
            oneuron[x].setOActivation(sigmoid(summation + oneuron[x].getBias()));
        }
    }

    /// <summary>
    /// Computes the raw error components at the output layer: target minus actual activation.
    /// </summary>
    public void calculateEC()
    {
        for (int x = 0; x < oneuron.Length; x++)
        {
            errorComponent[x] = (desiredout[x]) - (oneuron[x].getOActivation());
        }
    }

    /// <summary>
    /// Computes the error derivatives at the output layer using the derivative of the sigmoid function: act * (1 - act) * errorComponent.
    /// </summary>
    public void calculateDER()
    {
        for (int x = 0; x < oneuron.Length; x++)
        {
            errorDerivative[x] = oneuron[x].getOActivation() * (1.0 - oneuron[x].getOActivation()) * errorComponent[x];
        }
    }

    /// <summary>
    /// Performs one step of stochastic gradient descent: executes forward pass, backward error propagation, and weight/bias updates.
    /// </summary>
    public void learn()
    {
        this.run();
        this.calculateEC();
        this.calculateDER();
        for (int x = 0; x < hneuron.Length; x++)
            hneuron[x].calculateErr(errorDerivative);
        for (int x = 0; x < hneuron.Length; x++)
            hneuron[x].setWeight(_lrOut, errorDerivative);
        for (int x = 0; x < ineuron.Length; x++)
        {
            for (int y = 0; y < hneuron.Length; y++)
            {
                ineuron[x].setWeight(y, hneuron[y].getErr(), _lrIn);
            }
        }
        for (int x = 0; x < oneuron.Length; x++)
            oneuron[x].changeBias(_lrOut, errorDerivative);
        for (int x = 0; x < hneuron.Length; x++)
            hneuron[x].changeBias(_lrIn);
    }

    /// <summary>
    /// Executes the complete forward propagation pass (hidden activations followed by output activations).
    /// </summary>
    public void run()
    {
        this.calculateHA();
        this.calculateOA();
    }

    /// <summary>
    /// Evaluates the network forward pass and returns the class index with the highest activation (argmax).
    /// </summary>
    /// <returns>The zero-based index of the winning class.</returns>
    public int Predict()
    {
        this.run();
        int bestIndex = 0;
        double maxActivation = oneuron[0].getOActivation();
        for (int i = 1; i < oneuron.Length; i++)
        {
            double act = oneuron[i].getOActivation();
            if (act > maxActivation)
            {
                maxActivation = act;
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    /// <summary>
    /// Legacy session convergence check evaluating whether all output errors match their derivatives.
    /// </summary>
    /// <returns>True if convergence condition is met, otherwise false.</returns>
    public bool countgood()
    {
        bool result = true;
        for (int x = 0; x < oneuron.Length; x++)
        {
            if ((errorComponent[x] - errorDerivative[x]) != 0)
                result = false;
        }
        return result;
    }

    /// <summary>
    /// Serializes all network weights and biases sequentially to a text file.
    /// </summary>
    /// <param name="path">The destination file path.</param>
    public void saveWeights(string path)
    {
        using (StreamWriter sw = new StreamWriter(path))
        {
            for (int x = 0; x < ineuron.Length; x++)
            {
                for (int y = 0; y < hneuron.Length; y++)
                    sw.WriteLine(ineuron[x].getWeight(y));
            }
            for (int x = 0; x < hneuron.Length; x++)
            {
                for (int y = 0; y < oneuron.Length; y++)
                {
                    sw.WriteLine(hneuron[x].getWeight(y));
                }
            }
            for (int x = 0; x < hneuron.Length; x++)
            {
                sw.WriteLine(hneuron[x].getBias());
            }
            for (int x = 0; x < oneuron.Length; x++)
            {
                sw.WriteLine(oneuron[x].getBias());
            }
        }
    }

    /// <summary>
    /// Deserializes and loads all network weights and biases sequentially from a text file.
    /// </summary>
    /// <param name="path">The source weights file path.</param>
    public void loadWeights(string path)
    {
        using (StreamReader sr = new StreamReader(path))
        {
            for (int x = 0; x < ineuron.Length; x++)
            {
                for (int y = 0; y < hneuron.Length; y++)
                {
                    ineuron[x].setWeight(y, Convert.ToDouble(sr.ReadLine()));
                }
            }
            for (int x = 0; x < hneuron.Length; x++)
            {
                for (int y = 0; y < oneuron.Length; y++)
                {
                    hneuron[x].setWeight(y, Convert.ToDouble(sr.ReadLine()));
                }
            }
            for (int x = 0; x < hneuron.Length; x++)
            {
                hneuron[x].setBias(Convert.ToDouble(sr.ReadLine()));
            }
            for (int x = 0; x < oneuron.Length; x++)
            {
                oneuron[x].setBias(Convert.ToDouble(sr.ReadLine()));
            }
        }
    }
}

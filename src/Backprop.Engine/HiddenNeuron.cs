using System;

namespace Backprop;

// Represents a hidden layer neuron computing sigmoid activations and backpropagating output errors.
public class HNeuron
{
    private double[] weights; // outgoing weights to output layer neurons
    private int idno;
    private int wsize;
    private double hactivation;
    private double bias;
    private double error;

    // Initializes a new instance of HNeuron with default 10 outgoing connections.
    public HNeuron()
    {
        idno = 0;
        hactivation = 0.0;
        bias = this.randomweight();
        error = 0.0;
        wsize = 10;
        weights = new double[10];
        this.setRandomWeights(10);
    }

    // Initializes a new instance of HNeuron with a specified identifier and outgoing connections.
    // idnodata: The index of this hidden neuron.
    // size: The number of downstream output neurons to connect to.
    public HNeuron(int idnodata, int size)
    {
        idno = idnodata;
        hactivation = 0.0;
        bias = 0.01;
        wsize = size;
        error = 0.0;
        weights = new double[wsize];
        this.setRandomWeights(wsize);
    }

    // Computes the backward pass error gradient for this hidden neuron using output derivatives.
    // der: The error derivative vector from the downstream output layer.
    public void calculateErr(double[] der)
    {
        double result = 0.0;
        for (int x = 0; x < der.Length; x++)
        {
            result += (der[x] * weights[x]);
        }
        error = (result * hactivation * (1.0 - hactivation));
    }

    // Updates outgoing weights connecting this hidden neuron to the output layer.
    // lrpout: The learning rate applied to the output layer connections.
    // der: The output error derivative vector.
    public void setWeight(double lrpout, double[] der)
    {
        double temp = hactivation * lrpout;
        for (int x = 0; x < der.Length; x++)
        {
            weights[x] += (temp * der[x]);
        }
    }

    // Directly sets a specific outgoing connection weight (used during weight restoration).
    // pos: The index of the target output neuron.
    // dat: The weight value.
    public void setWeight(int pos, double dat)
    {
        weights[pos] = dat;
    }

    // Gets the weight connecting this hidden neuron to a specific output neuron.
    // outid: The target output neuron index.
    // Returns: The connection weight.
    public double getWeight(int outid)
    {
        return weights[outid];
    }

    // Directly sets the internal error gradient value for this neuron.
    // x: The error gradient value.
    public void setErr(double x)
    {
        error = x;
    }

    // Gets the current error gradient computed during backpropagation.
    // Returns: The error gradient.
    public double getErr()
    {
        return error;
    }

    // Sets the post-activation value for this hidden neuron during the forward pass.
    // data: The activation value.
    public void setHactivation(double data)
    {
        hactivation = data;
    }

    // Gets the post-activation value calculated during the forward pass.
    // Returns: The activation value.
    public double getHactivation()
    {
        return hactivation;
    }

    // Sets the neuron index identifier.
    // x: The neuron id.
    public void setIdno(int x)
    {
        idno = x;
    }

    // Gets the neuron index identifier.
    // Returns: The neuron id.
    public int getIdno()
    {
        return idno;
    }

    // Directly sets the neuron bias term.
    // b: The bias value.
    public void setBias(double b)
    {
        bias = b;
    }

    // Gets the current bias term of this hidden neuron.
    // Returns: The bias value.
    public double getBias()
    {
        return bias;
    }

    // Updates the bias term of this hidden neuron using its error gradient and input learning rate.
    // lrpin: The input-layer learning rate.
    public void changeBias(double lrpin)
    {
        bias += (error * lrpin);
    }

    // Randomizes all outgoing connection weights from this hidden neuron.
    // size: The number of weights to randomize.
    public void setRandomWeights(int size)
    {
        for (int x = 0; x < size; x++)
        {
            weights[x] = this.randomweight();
        }
    }

    private double randomweight()
    {
        return Rng.NextWeight();
    }
}

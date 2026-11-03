using System;

namespace Backprop;

/// <summary>
/// Represents a hidden layer neuron computing sigmoid activations and backpropagating output errors.
/// </summary>
public class HNeuron
{
    private double[] weights; // outgoing weights to output layer neurons
    private int idno;
    private int wsize;
    private double hactivation;
    private double bias;
    private double error;

    /// <summary>
    /// Initializes a new instance of <see cref="HNeuron"/> with default 10 outgoing connections.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of <see cref="HNeuron"/> with a specified identifier and outgoing connections.
    /// </summary>
    /// <param name="idnodata">The index of this hidden neuron.</param>
    /// <param name="size">The number of downstream output neurons to connect to.</param>
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

    /// <summary>
    /// Computes the backward pass error gradient for this hidden neuron using output derivatives.
    /// </summary>
    /// <param name="der">The error derivative vector from the downstream output layer.</param>
    public void calculateErr(double[] der)
    {
        double result = 0.0;
        for (int x = 0; x < der.Length; x++)
        {
            result += (der[x] * weights[x]);
        }
        error = (result * hactivation * (1.0 - hactivation));
    }

    /// <summary>
    /// Updates outgoing weights connecting this hidden neuron to the output layer.
    /// </summary>
    /// <param name="lrpout">The learning rate applied to the output layer connections.</param>
    /// <param name="der">The output error derivative vector.</param>
    public void setWeight(double lrpout, double[] der)
    {
        double temp = hactivation * lrpout;
        for (int x = 0; x < der.Length; x++)
        {
            weights[x] += (temp * der[x]);
        }
    }

    /// <summary>
    /// Directly sets a specific outgoing connection weight (used during weight restoration).
    /// </summary>
    /// <param name="pos">The index of the target output neuron.</param>
    /// <param name="dat">The weight value.</param>
    public void setWeight(int pos, double dat)
    {
        weights[pos] = dat;
    }

    /// <summary>
    /// Gets the weight connecting this hidden neuron to a specific output neuron.
    /// </summary>
    /// <param name="outid">The target output neuron index.</param>
    /// <returns>The connection weight.</returns>
    public double getWeight(int outid)
    {
        return weights[outid];
    }

    /// <summary>
    /// Directly sets the internal error gradient value for this neuron.
    /// </summary>
    /// <param name="x">The error gradient value.</param>
    public void setErr(double x)
    {
        error = x;
    }

    /// <summary>
    /// Gets the current error gradient computed during backpropagation.
    /// </summary>
    /// <returns>The error gradient.</returns>
    public double getErr()
    {
        return error;
    }

    /// <summary>
    /// Sets the post-activation value for this hidden neuron during the forward pass.
    /// </summary>
    /// <param name="data">The activation value.</param>
    public void setHactivation(double data)
    {
        hactivation = data;
    }

    /// <summary>
    /// Gets the post-activation value calculated during the forward pass.
    /// </summary>
    /// <returns>The activation value.</returns>
    public double getHactivation()
    {
        return hactivation;
    }

    /// <summary>
    /// Sets the neuron index identifier.
    /// </summary>
    /// <param name="x">The neuron id.</param>
    public void setIdno(int x)
    {
        idno = x;
    }

    /// <summary>
    /// Gets the neuron index identifier.
    /// </summary>
    /// <returns>The neuron id.</returns>
    public int getIdno()
    {
        return idno;
    }

    /// <summary>
    /// Directly sets the neuron bias term.
    /// </summary>
    /// <param name="b">The bias value.</param>
    public void setBias(double b)
    {
        bias = b;
    }

    /// <summary>
    /// Gets the current bias term of this hidden neuron.
    /// </summary>
    /// <returns>The bias value.</returns>
    public double getBias()
    {
        return bias;
    }

    /// <summary>
    /// Updates the bias term of this hidden neuron using its error gradient and input learning rate.
    /// </summary>
    /// <param name="lrpin">The input-layer learning rate.</param>
    public void changeBias(double lrpin)
    {
        bias += (error * lrpin);
    }

    /// <summary>
    /// Randomizes all outgoing connection weights from this hidden neuron.
    /// </summary>
    /// <param name="size">The number of weights to randomize.</param>
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

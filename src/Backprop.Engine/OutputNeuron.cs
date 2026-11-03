using System;

namespace Backprop;

/// <summary>
/// Represents an output layer neuron holding class activations, bias, and gradient updates.
/// </summary>
public class ONeuron
{
    private double bias;
    private int idno;
    private double OActivation;

    /// <summary>
    /// Initializes a new instance of <see cref="ONeuron"/> with random bias initialization.
    /// </summary>
    public ONeuron()
    {
        bias = this.randomBias();
        idno = 0;
        OActivation = 0.0;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ONeuron"/> with a specified identifier and default bias.
    /// </summary>
    /// <param name="idnodata">The class index of this output neuron.</param>
    public ONeuron(int idnodata)
    {
        bias = 0.01;
        idno = idnodata;
        OActivation = 0.0;
    }

    /// <summary>
    /// Sets the class index identifier for this output neuron.
    /// </summary>
    /// <param name="data">The neuron identifier index.</param>
    public void setIdno(int data)
    {
        idno = data;
    }

    /// <summary>
    /// Gets the class index identifier of this neuron.
    /// </summary>
    /// <returns>The neuron identifier index.</returns>
    public int getIdno()
    {
        return idno;
    }

    /// <summary>
    /// Sets the final activation output produced by this neuron in the forward pass.
    /// </summary>
    /// <param name="data">The activation probability/score.</param>
    public void setOActivation(double data)
    {
        OActivation = data;
    }

    /// <summary>
    /// Gets the final activation output computed for this class in the forward pass.
    /// </summary>
    /// <returns>The activation value.</returns>
    public double getOActivation()
    {
        return OActivation;
    }

    /// <summary>
    /// Gets the current bias term of this output neuron.
    /// </summary>
    /// <returns>The bias value.</returns>
    public double getBias()
    {
        return bias;
    }

    /// <summary>
    /// Directly sets the bias term (used during weight restoration).
    /// </summary>
    /// <param name="dat">The bias value.</param>
    public void setBias(double dat)
    {
        bias = dat;
    }

    /// <summary>
    /// Updates the bias term of this output neuron using the output error derivative and output learning rate.
    /// </summary>
    /// <param name="lrpout">The output learning rate.</param>
    /// <param name="der">The vector of output error derivatives.</param>
    public void changeBias(double lrpout, double[] der)
    {
        bias += lrpout * der[idno];
    }

    private double randomBias()
    {
        return Rng.NextWeight();
    }
}

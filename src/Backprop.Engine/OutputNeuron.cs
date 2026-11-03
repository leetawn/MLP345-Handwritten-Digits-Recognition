using System;

namespace Backprop;

// Represents an output layer neuron holding class activations, bias, and gradient updates.
public class ONeuron
{
    private double bias;
    private int idno;
    private double OActivation;

    // Initializes a new instance of ONeuron with random bias initialization.
    public ONeuron()
    {
        bias = this.randomBias();
        idno = 0;
        OActivation = 0.0;
    }

    // Initializes a new instance of ONeuron with a specified identifier and default bias.
    // idnodata: The class index of this output neuron.
    public ONeuron(int idnodata)
    {
        bias = 0.01;
        idno = idnodata;
        OActivation = 0.0;
    }

    // Sets the class index identifier for this output neuron.
    // data: The neuron identifier index.
    public void setIdno(int data)
    {
        idno = data;
    }

    // Gets the class index identifier of this neuron.
    // Returns: The neuron identifier index.
    public int getIdno()
    {
        return idno;
    }

    // Sets the final activation output produced by this neuron in the forward pass.
    // data: The activation probability/score.
    public void setOActivation(double data)
    {
        OActivation = data;
    }

    // Gets the final activation output computed for this class in the forward pass.
    // Returns: The activation value.
    public double getOActivation()
    {
        return OActivation;
    }

    // Gets the current bias term of this output neuron.
    // Returns: The bias value.
    public double getBias()
    {
        return bias;
    }

    // Directly sets the bias term (used during weight restoration).
    // dat: The bias value.
    public void setBias(double dat)
    {
        bias = dat;
    }

    // Updates the bias term of this output neuron using the output error derivative and output learning rate.
    // lrpout: The output learning rate.
    // der: The vector of output error derivatives.
    public void changeBias(double lrpout, double[] der)
    {
        bias += lrpout * der[idno];
    }

    private double randomBias()
    {
        return Rng.NextWeight();
    }
}

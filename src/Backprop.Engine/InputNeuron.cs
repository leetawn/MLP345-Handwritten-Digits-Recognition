using System;

namespace Backprop;

/// <summary>
/// Represents an input layer neuron holding a single input feature and outgoing weights to the hidden layer.
/// </summary>
public class INeuron
{
    private int idno;
    private double input;
    private double[] weights; // outgoing weights to hidden layer neurons
    private int wsize;

    /// <summary>
    /// Initializes a new instance of <see cref="INeuron"/> with default 64 outgoing weights.
    /// </summary>
    public INeuron()
    {
        idno = 0;
        input = 0;
        wsize = 64;
        weights = new double[wsize];
        this.setRandomWeights(wsize);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="INeuron"/> with a specified identifier and outgoing weight size.
    /// </summary>
    /// <param name="idnodata">The index/id of this input neuron.</param>
    /// <param name="size">The number of downstream hidden neurons to connect to.</param>
    public INeuron(int idnodata, int size)
    {
        idno = idnodata;
        input = 0;
        wsize = size;
        weights = new double[wsize];
        this.setRandomWeights(wsize);
    }

    private double randomweight()
    {
        return Rng.NextWeight();
    }

    /// <summary>
    /// Randomizes all outgoing weights connecting this input neuron to downstream hidden neurons.
    /// </summary>
    /// <param name="size">The number of outgoing weights to randomize.</param>
    public void setRandomWeights(int size)
    {
        for (int x = 0; x < size; x++)
        {
            weights[x] = this.randomweight();
        }
    }

    /// <summary>
    /// Updates the outgoing connection weight to a specified hidden neuron during backpropagation.
    /// </summary>
    /// <param name="hidno">The index of the downstream hidden neuron.</param>
    /// <param name="err">The backpropagated error gradient of the hidden neuron.</param>
    /// <param name="lrpin">The learning rate applied between input and hidden layers.</param>
    public void setWeight(int hidno, double err, double lrpin)
    {
        double errlrpin = err * lrpin;
        weights[hidno] += (errlrpin * input);
    }

    /// <summary>
    /// Sets the neuron identifier index.
    /// </summary>
    /// <param name="no">The neuron id.</param>
    public void setID(int no)
    {
        idno = no;
    }

    /// <summary>
    /// Sets the input activation feature value for this neuron during the forward pass.
    /// </summary>
    /// <param name="data">The feature value.</param>
    public void setInput(double data)
    {
        input = data;
    }

    /// <summary>
    /// Gets the current input activation value held by this neuron.
    /// </summary>
    /// <returns>The input value.</returns>
    public double getInput()
    {
        return input;
    }

    /// <summary>
    /// Gets the weight connecting this input neuron to a specific downstream hidden neuron.
    /// </summary>
    /// <param name="hidno">The index of the downstream hidden neuron.</param>
    /// <returns>The connection weight.</returns>
    public double getWeight(int hidno)
    {
        return weights[hidno];
    }

    /// <summary>
    /// Gets the identifier index of this neuron.
    /// </summary>
    /// <returns>The neuron id.</returns>
    public int getID()
    {
        return idno;
    }

    /// <summary>
    /// Directly sets a specific outgoing connection weight (used during weight loading).
    /// </summary>
    /// <param name="pos">The index of the target hidden neuron.</param>
    /// <param name="dat">The weight value to set.</param>
    public void setWeight(int pos, double dat)
    {
        weights[pos] = dat;
    }
}

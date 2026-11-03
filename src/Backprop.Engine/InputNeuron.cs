using System;

namespace Backprop;

// Represents an input layer neuron holding a single input feature and outgoing weights to the hidden layer.
public class INeuron
{
    private int idno;
    private double input;
    private double[] weights; // outgoing weights to hidden layer neurons
    private int wsize;

    // Initializes a new instance of INeuron with default 64 outgoing weights.
    public INeuron()
    {
        idno = 0;
        input = 0;
        wsize = 64;
        weights = new double[wsize];
        this.setRandomWeights(wsize);
    }

    // Initializes a new instance of INeuron with a specified identifier and outgoing weight size.
    // idnodata: The index/id of this input neuron.
    // size: The number of downstream hidden neurons to connect to.
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

    // Randomizes all outgoing weights connecting this input neuron to downstream hidden neurons.
    // size: The number of outgoing weights to randomize.
    public void setRandomWeights(int size)
    {
        for (int x = 0; x < size; x++)
        {
            weights[x] = this.randomweight();
        }
    }

    // Updates the outgoing connection weight to a specified hidden neuron during backpropagation.
    // hidno: The index of the downstream hidden neuron.
    // err: The backpropagated error gradient of the hidden neuron.
    // lrpin: The learning rate applied between input and hidden layers.
    public void setWeight(int hidno, double err, double lrpin)
    {
        double errlrpin = err * lrpin;
        weights[hidno] += (errlrpin * input);
    }

    // Sets the neuron identifier index.
    // no: The neuron id.
    public void setID(int no)
    {
        idno = no;
    }

    // Sets the input activation feature value for this neuron during the forward pass.
    // data: The feature value.
    public void setInput(double data)
    {
        input = data;
    }

    // Gets the current input activation value held by this neuron.
    // Returns: The input value.
    public double getInput()
    {
        return input;
    }

    // Gets the weight connecting this input neuron to a specific downstream hidden neuron.
    // hidno: The index of the downstream hidden neuron.
    // Returns: The connection weight.
    public double getWeight(int hidno)
    {
        return weights[hidno];
    }

    // Gets the identifier index of this neuron.
    // Returns: The neuron id.
    public int getID()
    {
        return idno;
    }

    // Directly sets a specific outgoing connection weight (used during weight loading).
    // pos: The index of the target hidden neuron.
    // dat: The weight value to set.
    public void setWeight(int pos, double dat)
    {
        weights[pos] = dat;
    }
}

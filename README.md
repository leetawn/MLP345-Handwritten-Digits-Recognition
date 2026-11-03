# OptDigits Handwritten Digit Recognition

A Windows Forms application that uses a small multilayer perceptron (MLP) to recognize handwritten digits. Draw a digit on the canvas, or train the model from the main menu and watch its progress.

## How It Works

The model receives 64 features from an 8 × 8 digit image and predicts one of the digits 0–9. It uses a hidden layer, sigmoid activations, and backpropagation with stochastic gradient descent.

The project includes the UCI Optical Recognition of Handwritten Digits dataset:

- 3,823 training samples
- 1,797 held-out test samples
- 64 pixel-based features per sample

## Run the Application

1. Open `OptDigitsMlp.sln` in Visual Studio.
2. Set **OptDigits.Forms** as the startup project.
3. Run the application.

The dataset files should be present in `data/raw/` as `optdigits.tra` and `optdigits.tes`.

## Using the Main Menu

The main menu has two ways to use the classifier.

### Use the Pre-Trained Model

Choose **Open Canvas (Pre-Trained Model)** to load the saved model. On the canvas:

- Draw a digit using the mouse.
- The app converts the drawing into an 8 × 8 preview.
- The predicted digit and the activation for every class update as you draw.
- Select **Clear** to start again.

### Train a Model

Use the training controls to choose:

- Number of epochs
- Number of hidden units
- Output and input learning rates
- Random seed

Select **Train Model** to load the dataset, split the training data into training and validation sets, train the MLP, and show loss and validation accuracy for each epoch. When training finishes, the menu displays held-out test accuracy and enables **Open Canvas (Trained Model)** so you can try the newly trained network.

Training outputs are saved in `results/`, including the learned weights, training log, and confusion matrix.

## Hyperparameter Results

Complete sweep logs and analysis are available in [docs/experiments.md](docs/experiments.md).

| Configuration | LrOut | LrIn | Hidden Size | Epochs | Val Accuracy (%) | Final Val Loss |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Selected efficiency configuration** | 0.50 | 0.375 | 32 | 50 | 98.82% | 0.002890 |
| **Baseline configuration** | 0.20 | 0.150 | 32 | 50 | 98.43% | 0.003364 |

## Final Evaluation Results

Held-out test-set performance:

| Metric | Score |
| :--- | :---: |
| **Overall Test Accuracy** | 95.99% (1,725 / 1,797) |
| **Average Precision** | 96.16% (0.9616) |
| **Average Recall** | 95.98% (0.9598) |
| **Macro F1-Score** | 96.00% (0.9600) |

For dataset details, see [docs/data-description.md](docs/data-description.md).

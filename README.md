# OptDigits MLP Handwritten Digit Classifier

A modular C# implementation of a 3-layer Multilayer Perceptron (MLP) trained with backpropagation (stochastic gradient descent) to recognize handwritten digits from the UCI Optical Recognition of Handwritten Digits dataset.

Built with clean architecture, zero machine learning framework dependencies, and deterministic reproducibility.

---

## Table of Contents
- [Overview](#overview)
- [Dataset](#dataset)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [CLI Options](#cli-options)
- [Training Log Format](#training-log-format)
- [Hyperparameter Results](#hyperparameter-results)
- [Final Evaluation Results](#final-evaluation-results)
- [Team & Responsibilities](#team--responsibilities)
- [References](#references)

---

## Overview
This project classifies $8 \times 8$ downsampled bitmaps of handwritten digits ($0$ through $9$) using:
- **Architecture**: 64 input units, configurable hidden units (default: 32), 10 output units.
- **Activation**: Logistic sigmoid $\sigma(z) = \frac{1}{1 + e^{-z}}$ for hidden and output activations.
- **Optimization**: Stochastic gradient descent backpropagation minimizing Mean Squared Error (MSE).
- **Inference**: Argmax class activation selection.

---

## Dataset
- **Dataset**: UCI Optical Recognition of Handwritten Digits
- **Instances**: 5,620 total (3,823 training in `optdigits.tra`, 1,797 test in `optdigits.tes`).
- **Features**: 64 integer attributes in range $[0, 16]$, representing counts of black pixels in $4 \times 4$ blocks of $32 \times 32$ handwritten bitmaps.
- **Classes**: 10 classes (`0` to `9`).
- **License**: Creative Commons Attribution 4.0 International (CC BY 4.0).
- For complete details, see [docs/data-description.md](docs/data-description.md).

---

## Project Structure
```
optdigits-mlp/
├── .github/
│   ├── workflows/ci.yml               # Automated CI build on push and pull requests
│   └── pull_request_template.md       # PR template with task verification checklist
├── .editorconfig                      # Coding style (4 spaces, PascalCase, file-scoped namespaces)
├── .gitignore                         # Build and environment exclusions (.NET, Python, results)
├── Directory.Build.props              # Shared build properties (.NET 8 LTS, Nullable, ImplicitUsings)
├── OptDigitsMlp.sln                   # Visual Studio solution file
├── README.md                          # Project documentation and user guide
├── data/
│   └── raw/                           # Raw optdigits.tra and optdigits.tes dataset files
├── docs/
│   ├── TASKS.md                       # Task checklist and implementation contracts for Partner A & B
│   ├── architecture.md                # System flowchart and component responsibilities
│   ├── data-description.md            # Dataset provenance, statistics, and preprocessing plan
│   ├── experiments.md                 # Hyperparameter sweep template and tuning notes
│   └── CONTRIBUTING.md                # Contribution rules, branch strategy, and PR guidelines
├── results/                           # Output artifacts (.csv logs, .png plots, weights.txt)
├── tools/                             # Data download scripts and matplotlib plotting utilities
└── src/
    ├── Backprop.Engine/               # Pure C# neural network engine (no external dependencies)
    └── OptDigits.App/                 # Console application (CLI orchestration, data, training, evaluation)
```

---

## Prerequisites
- **.NET SDK**: .NET 8.0 LTS or later (`dotnet --version`).
- **Python (Optional, for plots)**: Python 3.9+ with `matplotlib`, `pandas`, `numpy` (see `tools/requirements.txt`).

---

## Quick Start

### 1. Download Dataset
Download the UCI optdigits dataset into `data/raw/`:
```bash
# On Linux / macOS / Git Bash:
./tools/download-data.sh

# On Windows PowerShell:
powershell -ExecutionPolicy Bypass -File tools/download-data.ps1
```

### 2. Build the Solution
```bash
dotnet build
```

### 3. Run Commands

#### Train Model
Train network with default or custom hyperparameters, evaluate on test data, and persist weights:
```bash
dotnet run --project src/OptDigits.App -- train --epochs 50 --hidden 32 --lr-out 0.2 --lr-in 0.15
```

#### Sweep Hyperparameters
Run grid search over learning rates, hidden layer sizes, and epoch counts:
```bash
dotnet run --project src/OptDigits.App -- sweep
```

#### Evaluate Saved Model
Evaluate saved weights (`results/weights.txt`) against the held-out test set:
```bash
dotnet run --project src/OptDigits.App -- evaluate
```

### 4. Plot Loss Curves and Confusion Matrix (Optional)
```bash
pip install -r tools/requirements.txt
python tools/plot_results.py
```
This generates:
- `results/loss_curve.png`: Training loss, validation loss, and validation accuracy curves.
- `results/confusion_matrix.png`: Heatmap visualization of digit classifications.

---

## CLI Options

| Command | Option | Type | Default | Description |
| :--- | :--- | :---: | :---: | :--- |
| `train` | `--hidden` | `int` | `32` | Number of neurons in the hidden layer |
| `train` | `--epochs` | `int` | `50` | Total training epochs |
| `train` | `--lr-out` | `double` | `0.2` | Output layer learning rate |
| `train` | `--lr-in` | `double` | `0.15` | Input-to-hidden layer learning rate |
| `train` | `--seed` | `int` | `42` | Random seed for weight init and splitting |
| `train` | `--log-every`| `int` | `5` | Epoch interval for logging metrics |
| `sweep` | *(none)* | - | - | Runs full parameter search over `ExperimentGrid.Default` |
| `evaluate` | *(none)* | - | - | Evaluates saved weights on `data/raw/optdigits.tes` |

---

## Training Log Format
The training logger generates `results/training_log.csv` with the following columns:
```csv
epoch,train_loss,validation_loss,validation_accuracy
5,0.082154,0.084312,0.6845
10,0.045120,0.047891,0.8214
...
```

---

## Hyperparameter Results
Complete sweep logs and analysis are tracked in [docs/experiments.md](docs/experiments.md).

| Configuration | LrOut | LrIn | Hidden Size | Epochs | Val Accuracy (%) | Final Val Loss |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Best Configuration** | 0.50 | 0.375 | 32 | 50 | 98.82% | 0.002890 |
| **Baseline Configuration** | 0.20 | 0.150 | 32 | 50 | 98.43% | 0.003364 |

---

## Final Evaluation Results
Held-out test set performance (`data/raw/optdigits.tes`):

| Metric | Score |
| :--- | :---: |
| **Overall Test Accuracy** | 95.99% (1,725 / 1,797) |
| **Average Precision** | 96.16% (0.9616) |
| **Average Recall** | 95.98% (0.9598) |
| **Macro F1-Score** | 96.00% (0.9600) |

*(Evaluated on 1,797 test instances with 32 hidden units, 50 epochs, and tuned learning rates).*

---

## Team & Responsibilities

| Role | Partner | Primary Modules & Deliverables |
| :--- | :---: | :--- |
| **Partner A** | *Student A* | Backprop Engine extensions, `Trainer`, `CsvLossLogger`, `ExperimentRunner`, parameter sweep execution, `experiments.md` write-up. |
| **Partner B** | *Student B* | `CsvDatasetLoader`, `MinMaxScaler`, `LabelEncoder`, `DatasetSplitter`, `Evaluator`, `EvaluationReport` formatting, documentation, plotting tools, and final results write-up. |

---

## References
1. **UCI Dataset**:
   Alpaydin, E. and Kaynak, C. (1998). *Optical Recognition of Handwritten Digits*. UCI Machine Learning Repository. [https://doi.org/10.24432/C50P49](https://doi.org/10.24432/C50P49).
2. **Backpropagation**:
   Rumelhart, D. E., Hinton, G. E., and Williams, R. J. (1986). *Learning representations by back-propagating errors*. Nature, 323(6088), 533-536.

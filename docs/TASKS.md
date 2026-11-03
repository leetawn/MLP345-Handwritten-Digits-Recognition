# Project Task Checklists & Implementation Guide

This document defines actionable checklists for **Partner A** and **Partner B** to implement and verify their respective components in parallel.

---

## Partner A: Engine, Training & Experiments

### [ ] Task A.1: Engine Verification & Math Sanity Check
- **File**: `src/Backprop.Engine/NeuralNet.cs`
- **Contract**: Ensure forward pass (`run()`), stochastic gradient descent update (`learn()`), and argmax prediction (`Predict()`) function without exceptions.
- **Done when**: A small dummy run (e.g., 2 samples through `learn()`) executes without NaN activations or runtime exceptions.

### [ ] Task A.2: Implement `CsvLossLogger`
- **File**: `src/OptDigits.App/Training/CsvLossLogger.cs`
- **Contract**: Implement `CsvLossLogger(string path)`, `Log(EpochMetrics metrics)`, and `Dispose()`.
- **Done when**: Running training writes CSV rows with `epoch,train_loss,validation_loss,validation_accuracy` and formats progress output to console.

### [ ] Task A.3: Implement `Trainer`
- **File**: `src/OptDigits.App/Training/Trainer.cs`
- **Contract**:
  - `Train(NeuralNet net, IReadOnlyList<Sample> train, IReadOnlyList<Sample> validation, TrainingOptions options, ILossLogger? logger)`
  - `static double ComputeLoss(NeuralNet net, IReadOnlyList<Sample> samples)`
- **Done when**: Training loops over epochs, feeds inputs and one-hot labels, executes backpropagation, and logs progress every `options.LogEvery` epochs.

### [ ] Task A.4: Implement `ExperimentRunner`
- **File**: `src/OptDigits.App/Experiments/ExperimentRunner.cs`
- **Contract**: `Run(ExperimentGrid grid, IReadOnlyList<Sample> train, IReadOnlyList<Sample> validation, TrainingOptions baseOptions, string outputCsvPath)`
- **Done when**: Systematic sweep iterates over all learning rates, hidden layer sizes, and epoch combinations, recording execution times and validation metrics into `results/sweep.csv`.

### [ ] Task A.5: Execute Hyperparameter Grid Sweep
- **Command**: `dotnet run --project src/OptDigits.App -- sweep`
- **Contract**: Complete the sweep over `ExperimentGrid.Default` and inspect top 5 performing configurations.
- **Done when**: `results/sweep.csv` is populated with valid trial results and console displays top 5 ranked models.

### [ ] Task A.6: Fill Experiments Documentation
- **File**: `docs/experiments.md`
- **Contract**: Populate the Learning Rate, Hidden Size, and Epoch Count sweep tables; document the selected hyperparameter set and write analysis observations.
- **Done when**: All placeholder cells in `docs/experiments.md` are filled with real experiment data.

---

## Partner B: Data Pipeline, Evaluation, Tools & Docs

### [ ] Task B.1: Implement `CsvDatasetLoader`
- **File**: `src/OptDigits.App/Data/CsvDatasetLoader.cs`
- **Contract**: Implement `IDatasetLoader.Load(string path)` to parse 65 comma-separated integer columns per line.
- **Done when**: Loading `data/raw/optdigits.tra` returns 3,823 samples and `data/raw/optdigits.tes` returns 1,797 samples.

### [ ] Task B.2: Implement `MinMaxScaler`
- **File**: `src/OptDigits.App/Data/MinMaxScaler.cs`
- **Contract**: Implement `Fit(IEnumerable<Sample> samples)`, `Transform(Sample s)`, and `TransformAll(IEnumerable<Sample> samples)` to scale features into $[0, 1]$.
- **Done when**: Output features fall strictly within $[0.0, 1.0]$ and fixed bounds $(0, 16)$ divide inputs by $16.0$.

### [ ] Task B.3: Implement `LabelEncoder`
- **File**: `src/OptDigits.App/Data/LabelEncoder.cs`
- **Contract**: Implement `OneHot(int label, int classCount)` and `ArgMax(IReadOnlyList<double> values)`.
- **Done when**: `OneHot(3, 10)` yields a 10-element array with $1.0$ at index 3 and $0.0$ elsewhere, and `ArgMax` returns the index of the highest value.

### [ ] Task B.4: Implement `DatasetSplitter`
- **File**: `src/OptDigits.App/Data/DatasetSplitter.cs`
- **Contract**: Implement `Split(IReadOnlyList<Sample> samples, double validationFraction, int seed)` using stratified partitioning.
- **Done when**: Splitting 3,823 training samples with fraction $0.2$ produces ~3,058 train samples and ~765 validation samples with equal class proportions.

### [ ] Task B.5: Implement `Evaluator`
- **File**: `src/OptDigits.App/Evaluation/Evaluator.cs`
- **Contract**: Implement `Evaluate(NeuralNet net, IReadOnlyList<Sample> samples)` returning an `EvaluationReport`.
- **Done when**: Model predictions over test samples correctly populate the $10 \times 10$ confusion matrix, class precision, recall, and F1 scores.

### [ ] Task B.6: Implement `EvaluationReport` Formatting & CSV Export
- **File**: `src/OptDigits.App/Evaluation/EvaluationReport.cs`
- **Contract**: Implement `ToConsoleString()` and `SaveConfusionMatrixCsv(string path)`.
- **Done when**: `ToConsoleString()` returns readable tables and `SaveConfusionMatrixCsv` writes a valid 10x10 CSV matrix with headers.

### [ ] Task B.7: Verify Dataset Facts & Run Visualizations
- **Files**: `docs/data-description.md`, `tools/plot_results.py`
- **Contract**: Verify all dataset statistics against downloaded files and run `python tools/plot_results.py`.
- **Done when**: Any unverified facts are confirmed and `results/loss_curve.png` and `results/confusion_matrix.png` are generated successfully.

### [ ] Task B.8: Populate README Evaluation Results
- **File**: `README.md`
- **Contract**: Update the Hyperparameter Results and Final Evaluation Results tables in `README.md` with final test metrics.
- **Done when**: `README.md` reflects true test accuracy, macro F1, and baseline comparison scores.

---

## Integration & Milestone Checklist

1. [ ] **Milestone 1 - First End-to-End Training Run**:
   - Partner A and Partner B merge their respective data and training PRs into `main`.
   - Execute: `dotnet run --project src/OptDigits.App -- train --epochs 20`
   - Confirm training proceeds, logs progress, evaluates on test set, and writes `results/weights.txt`.
2. [ ] **Milestone 2 - Cross-Partner PR Reviews**:
   - Partner A reviews and approves Partner B's data/eval PRs.
   - Partner B reviews and approves Partner A's training/experiments PRs.
   - Confirm all CI checks pass on GitHub Actions before squash merging.
3. [ ] **Milestone 3 - Full Experiments & Plots**:
   - Run complete sweep: `dotnet run --project src/OptDigits.App -- sweep`
   - Run final training with best configuration: `dotnet run --project src/OptDigits.App -- train --hidden <best> --epochs <best> --lr-out <best> --lr-in <best>`
   - Generate graphs: `python tools/plot_results.py`
4. [ ] **Milestone 4 - Final Documentation Pass**:
   - Check `README.md`, `docs/experiments.md`, and `docs/data-description.md` for completeness.
   - Ensure repository builds cleanly with `dotnet build` and zero warnings.

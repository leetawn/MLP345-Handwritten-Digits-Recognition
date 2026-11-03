# Architecture & System Design

This document details the modular architecture of the handwritten digit classifier, component relationships, data flow, and ownership boundaries between Partner A and Partner B.

---

## 1. System Data Flow

```mermaid
flowchart TD
    RawData["Raw OptDigits Files\n(data/raw/*.tra, *.tes)"]
    Loader["IDatasetLoader\n(CsvDatasetLoader)"]
    Scaler["MinMaxScaler\n(Features -> [0, 1])"]
    Splitter["DatasetSplitter\n(Stratified 80/20 Train/Val)"]
    
    TrainSet[("Training Set\n(~3,058 samples)")]
    ValSet[("Validation Set\n(~765 samples)")]
    TestSet[("Held-out Test Set\n(1,797 samples)")]

    Factory["NetworkFactory\n(Rng Seed & NeuralNet Init)"]
    Model["NeuralNet\n(3-Layer MLP Engine)"]

    Trainer["Trainer\n(Train loop, MSE loss)"]
    Logger["ILossLogger\n(CsvLossLogger)"]
    TrainLog[("training_log.csv")]

    Evaluator["Evaluator\n(Predictions & Metrics)"]
    Report["EvaluationReport\n(Accuracy, Precision, Recall, F1)"]
    ConfusionCsv[("confusion_matrix.csv")]
    WeightsFile[("weights.txt")]

    SweepRunner["ExperimentRunner\n(Hyperparameter Sweep Grid)"]
    SweepCsv[("sweep.csv")]

    RawData --> Loader
    Loader --> Scaler
    Scaler --> Splitter
    
    Splitter --> TrainSet
    Splitter --> ValSet
    Scaler -.-> TestSet

    Factory --> Model

    TrainSet --> Trainer
    ValSet --> Trainer
    Model <--> Trainer
    Trainer --> Logger
    Logger --> TrainLog

    Trainer --> WeightsFile

    Model --> Evaluator
    TestSet --> Evaluator
    Evaluator --> Report
    Report --> ConfusionCsv

    TrainSet --> SweepRunner
    ValSet --> SweepRunner
    Factory -.-> SweepRunner
    SweepRunner --> SweepCsv
```

## 2. Separation of Concerns & Clean Architecture
1. **Engine Independence**:
   `Backprop.Engine` has zero external dependencies and does not know about the CLI, dataset formats, or evaluation metrics.
2. **Deterministic Reproducibility**:
   All random operations (initial network weights and stratified data splitting) are seeded via `Rng.Seed(options.Seed)` and `Random(options.Seed)`.
3. **Data Leakage Prevention**:
   The `MinMaxScaler` is fitted strictly on the training set. The held-out test set (`optdigits.tes`) is never seen during training or parameter tuning.

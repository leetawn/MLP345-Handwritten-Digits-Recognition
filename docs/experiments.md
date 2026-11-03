# Hyperparameter Tuning & Experimental Evaluation

This document logs systematic tuning experiments conducted on the handwritten digit classifier using the UCI optdigits benchmark dataset.

---

## 1. Methodology
- **Validation Protocol**: 80/20 stratified split of `optdigits.tra` (~3,058 training instances, ~765 validation instances).
- **Test Set Protection**: The held-out test set (`optdigits.tes`, 1,797 instances) was not used during tuning.
- **Learning Rate Ratio**: To preserve the original engine ratio ($0.2 / 0.15$), the input learning rate is maintained as:
  $$\eta_{\text{in}} = 0.75 \times \eta_{\text{out}}$$
- **Random Seed**: Fixed to `42` across all comparative trials for deterministic reproducibility.
- **Evaluation Metrics**:
  - **Mean Squared Error (MSE) Loss**: Total squared deviation across all 10 output class units.
  - **Accuracy**: Percentage of correctly classified digits via argmax output activation.
  - **Wall-clock Duration**: Execution time per run in seconds.

---

## 2. Learning Rate Sweep
Baseline fixed parameters: `HiddenSize = 32`, `Epochs = 50`, `Seed = 42`.

| Trial | $\eta_{\text{out}}$ (LrOut) | $\eta_{\text{in}}$ (LrIn) | Final Train Loss | Final Val Loss | Validation Accuracy (%) | Duration (s) | Notes |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| **LR-1** | 0.01 | 0.0075 | | | | | |
| **LR-2** | 0.05 | 0.0375 | | | | | |
| **LR-3** | 0.10 | 0.0750 | | | | | |
| **LR-4** | 0.20 | 0.1500 | | | | | Default baseline |
| **LR-5** | 0.50 | 0.3750 | | | | | |

---

## 3. Hidden Layer Size Sweep
Evaluated with optimal learning rate from Section 2; fixed `Epochs = 50`, `Seed = 42`.

| Trial | Hidden Neurons | $\eta_{\text{out}}$ | $\eta_{\text{in}}$ | Final Train Loss | Final Val Loss | Validation Accuracy (%) | Duration (s) | Notes |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| **H-1** | 16 | | | | | | | Lightweight model |
| **H-2** | 32 | | | | | | | Baseline |
| **H-3** | 64 | | | | | | | High capacity |

---

## 4. Epoch Count Sweep
Evaluated with chosen learning rate and hidden layer size; fixed `Seed = 42`.

| Trial | Epochs | Hidden Neurons | $\eta_{\text{out}}$ | $\eta_{\text{in}}$ | Final Train Loss | Final Val Loss | Validation Accuracy (%) | Duration (s) | Notes |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| **EP-1** | 20 | | | | | | | Fast convergence check |
| **EP-2** | 50 | | | | | | | Baseline |
| **EP-3** | 100 | | | | | | | Extended training |

---

## 5. Selected Configuration & Justification

| Hyperparameter | Selected Value | Justification / Trade-off |
| :--- | :---: | :--- |
| **Output Learning Rate ($\eta_{\text{out}}$)** | TBD | |
| **Input Learning Rate ($\eta_{\text{in}}$)** | TBD | |
| **Hidden Layer Size** | TBD | |
| **Training Epochs** | TBD | |
| **Random Seed** | 42 | Fixed for deterministic reproducibility |

*Summary of Justification*:
> *(Fill in rationale after running `dotnet run --project src/OptDigits.App -- sweep`)*

---

## 6. Observations & Analysis
1. **Convergence Behavior**:
   - *(Analyze train vs. validation loss trajectory, e.g. signs of overfitting or underfitting)*
2. **Impact of Learning Rates**:
   - *(Comment on stability and speed of gradient descent across smaller vs larger rates)*
3. **Capacity vs Generalization**:
   - *(Discuss performance differences between 16, 32, and 64 hidden units)*
4. **Computational Efficiency**:
   - *(Discuss runtime trade-offs observed across different epochs and hidden sizes)*

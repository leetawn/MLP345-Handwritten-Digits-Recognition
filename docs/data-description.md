# UCI Optical Recognition of Handwritten Digits - Dataset Description

This document provides dataset characteristics, provenance, statistical properties, and the preprocessing pipeline for the handwritten digit recognition project.

---

## 1. Dataset Overview and Provenance
- **Dataset Name**: Optical Recognition of Handwritten Digits
- **Source**: UCI Machine Learning Repository
- **Creators / Authors**: E. Alpaydin and C. Kaynak, Department of Computer Engineering, Bogazici University, 80815 Istanbul, Turkey
- **Date Collected / Donated**: 1998
- **License**: Creative Commons Attribution 4.0 International (CC BY 4.0)
- **Repository URL**: [https://archive.ics.uci.edu/dataset/80/optical+recognition+of+handwritten+digits](https://archive.ics.uci.edu/dataset/80/optical+recognition+of+handwritten+digits)
- **Direct Download Archive**: [https://archive.ics.uci.edu/ml/machine-learning-databases/optdigits/](https://archive.ics.uci.edu/ml/machine-learning-databases/optdigits/)

---

## 2. Dataset Key Facts
- **Total Instances**: 5,620 instances
  - **Training Set (`optdigits.tra`)**: 3,823 samples
  - **Held-Out Test Set (`optdigits.tes`)**: 1,797 samples
- **Number of Attributes**: 64 numeric input features + 1 integer class label (65 attributes per instance).
- **Target Classes**: 10 classes corresponding to handwritten decimal digits `0`, `1`, `2`, `3`, `4`, `5`, `6`, `7`, `8`, `9`.
- **Missing Attribute Values**: None (0 missing values).
- **Writers / Subjects**: Collected from 43 individual people:
  - 30 writers contributed to the training set (`optdigits.tra`).
  - 13 writers contributed to the testing set (`optdigits.tes`).
  - *Note*: The writer populations are disjoint, ensuring evaluation tests generalization across unseen writers.

---

## 3. Feature Extraction and Representation
1. **Raw Source**: Normalized handwritten digit bitmaps of dimensions $32 \times 32$ pixels.
2. **Block Averaging**:
   - Each $32 \times 32$ bitmap was divided into non-overlapping $4 \times 4$ pixel sub-blocks (yielding an $8 \times 8$ grid of blocks).
   - In each $4 \times 4$ block, the count of active ("on" or black) pixels was computed.
   - The pixel count for each block falls in the range $[0, 16]$.
3. **Linearized Input Vector**:
   - The $8 \times 8$ block counts are flattened into a 64-dimensional feature vector: $(x_0, x_1, \dots, x_{63})$.
   - The 65th value is the integer digit label $y \in \{0, 1, \dots, 9\}$.

---

## 4. Observed Class Distribution Summary

| Class (Digit) | Training Samples (`optdigits.tra`) | Test Samples (`optdigits.tes`) | Total Samples |
| :---: | :---: | :---: | :---: |
| **0** | 376 | 178 | 554 |
| **1** | 389 | 182 | 571 |
| **2** | 380 | 177 | 557 |
| **3** | 389 | 183 | 572 |
| **4** | 387 | 181 | 568 |
| **5** | 376 | 182 | 558 |
| **6** | 377 | 181 | 558 |
| **7** | 387 | 179 | 566 |
| **8** | 380 | 174 | 554 |
| **9** | 382 | 180 | 562 |
| **Total** | **3,823** | **1,797** | **5,620** |

*Observation*: The class distribution is balanced across both splits, with roughly $10\%$ representation per digit class.

---

## 5. Preprocessing Pipeline Plan

```
Raw File (optdigits.tra)
       │
       ▼
 [CsvDatasetLoader] ──> Parse 64 integer features [0..16] and label [0..9]
       │
       ▼
  [MinMaxScaler]    ──> Scale features to [0.0, 1.0] by dividing by 16.0 (or fitted bounds)
       │                 *Fitted strictly on training data only*
       ▼
 [DatasetSplitter]  ──> Stratified 80/20 train/validation split (Seed = 42)
       │
       ├───> Training Set (~3,058 samples)   ──> [Trainer]
       └───> Validation Set (~765 samples)   ──> [Validation Monitoring & Sweeps]
```

### Preprocessing Steps:
1. **Feature Normalization**:
   - Raw block pixel counts range from $0$ to $16$.
   - Features are scaled to $[0, 1]$ via min-max scaling:
     $$\tilde{x}_i = \frac{x_i - \min}{\max - \min} = \frac{x_i}{16.0}$$
   - **Crucial Rule**: The scaler is fitted *only* on the training dataset. Test samples are transformed using parameters fitted from training to prevent data leakage.
2. **Label Encoding**:
   - The scalar integer label $y \in [0, 9]$ is converted to a one-hot vector $\mathbf{t} \in \mathbb{R}^{10}$, where:
     $$t_k = \begin{cases} 1.0 & \text{if } k = y \\ 0.0 & \text{if } k \neq y \end{cases}$$
3. **Data Partitioning**:
   - Training data (`optdigits.tra`) is partitioned into an $80\%$ training subset and a $20\%$ validation subset.
   - The partition is **stratified** by digit label to ensure each class is represented equally in both subsets.
4. **Held-Out Test Set Hygiene**:
   - The official test partition (`optdigits.tes`, 1,797 instances) is completely held out during all training and hyperparameter search stages.
   - The test set is evaluated solely at final reporting time.

---

## 6. Citation
```bibtex
@misc{optdigits_80,
  author       = {Alpaydin, E. and Kaynak, C.},
  title        = {{Optical Recognition of Handwritten Digits}},
  year         = {1998},
  howpublished = {UCI Machine Learning Repository},
  note         = {{DOI}: https://doi.org/10.24432/C50P49}
}
```

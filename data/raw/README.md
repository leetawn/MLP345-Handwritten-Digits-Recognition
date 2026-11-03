# UCI Optical Recognition of Handwritten Digits Dataset

This directory contains the raw data files for the handwritten digit recognition benchmark.

## Dataset Information
- **Dataset Name**: Optical Recognition of Handwritten Digits
- **Source**: UCI Machine Learning Repository
- **Source URL**: [https://archive.ics.uci.edu/dataset/80/optical+recognition+of+handwritten+digits](https://archive.ics.uci.edu/dataset/80/optical+recognition+of+handwritten+digits)
- **Direct Download Archive**: [https://archive.ics.uci.edu/ml/machine-learning-databases/optdigits/](https://archive.ics.uci.edu/ml/machine-learning-databases/optdigits/)
- **License**: Creative Commons Attribution 4.0 International (CC BY 4.0)

## Dataset Structure and Format
The dataset contains normalized bitmaps of handwritten digits extracted from pre-printed forms filled by different writers.
- **Total Instances**: 5,620 instances
  - `optdigits.tra`: 3,823 training instances (collected from 30 writers)
  - `optdigits.tes`: 1,797 held-out testing instances (collected from 13 writers)
- **Number of Attributes**: 64 input features + 1 class label (65 comma-delimited columns per row).
- **Attributes Description**:
  - Each input digit was originally a $32 \times 32$ binary bitmap.
  - Bitmaps were divided into non-overlapping $4 \times 4$ pixel blocks.
  - In each block, the number of "on" pixels (range 0 to 16) was counted.
  - This yields an $8 \times 8$ grid of feature values, linearized to 64 integer attributes in the range $[0, 16]$.
  - The 65th column is the ground-truth digit class label in the range $[0, 9]$.
- **Missing Values**: None.

## Citation
If you use this dataset, please cite:
```bibtex
@misc{optdigits_80,
  author       = {Alpaydin, E. and Kaynak, C.},
  title        = {{Optical Recognition of Handwritten Digits}},
  year         = {1998},
  howpublished = {UCI Machine Learning Repository},
  note         = {{DOI}: https://doi.org/10.24432/C50P49}
}
```

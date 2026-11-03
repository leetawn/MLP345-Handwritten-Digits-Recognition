#!/usr/bin/env python3
"""
Plots training loss curves and evaluation confusion matrix heatmap.
Reads:
    results/training_log.csv
    results/confusion_matrix.csv
Outputs:
    results/loss_curve.png
    results/confusion_matrix.png
"""

import os
import sys
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt

def find_results_dir():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    repo_root = os.path.abspath(os.path.join(script_dir, ".."))
    results_dir = os.path.join(repo_root, "results")
    if not os.path.exists(results_dir):
        os.makedirs(results_dir, exist_ok=True)
    return results_dir

def plot_loss_curves(results_dir):
    csv_path = os.path.join(results_dir, "training_log.csv")
    out_path = os.path.join(results_dir, "loss_curve.png")

    if not os.path.exists(csv_path):
        print(f"[SKIP] Training log not found at {csv_path}. Skipping loss curve generation.")
        return

    try:
        df = pd.read_csv(csv_path)
        required_cols = {"epoch", "train_loss", "validation_loss", "validation_accuracy"}
        if not required_cols.issubset(df.columns):
            print(f"[WARN] Training log missing required columns {required_cols}. Found: {list(df.columns)}")
            return

        fig, (ax1, ax2) = plt.subplots(1, 2, figsize=(14, 5))

        # Loss plot
        ax1.plot(df["epoch"], df["train_loss"], label="Train Loss (MSE)", color="#1f77b4", marker="o", markersize=3)
        ax1.plot(df["epoch"], df["validation_loss"], label="Validation Loss (MSE)", color="#ff7f0e", marker="s", markersize=3)
        ax1.set_xlabel("Epoch", fontsize=11)
        ax1.set_ylabel("Mean Squared Error", fontsize=11)
        ax1.set_title("Training and Validation Loss Curve", fontsize=13, fontweight="bold")
        ax1.grid(True, linestyle="--", alpha=0.5)
        ax1.legend(loc="upper right")

        # Accuracy plot
        ax2.plot(df["epoch"], df["validation_accuracy"] * 100.0, label="Validation Accuracy (%)", color="#2ca02c", marker="^", markersize=3)
        ax2.set_xlabel("Epoch", fontsize=11)
        ax2.set_ylabel("Accuracy (%)", fontsize=11)
        ax2.set_title("Validation Accuracy Progress", fontsize=13, fontweight="bold")
        ax2.grid(True, linestyle="--", alpha=0.5)
        ax2.set_ylim([0, 105])
        ax2.legend(loc="lower right")

        plt.tight_layout()
        plt.savefig(out_path, dpi=300)
        plt.close()
        print(f"[SUCCESS] Loss curves saved to: {out_path}")

    except Exception as e:
        print(f"[ERROR] Failed to plot loss curves: {e}")

def plot_confusion_matrix(results_dir):
    csv_path = os.path.join(results_dir, "confusion_matrix.csv")
    out_path = os.path.join(results_dir, "confusion_matrix.png")

    if not os.path.exists(csv_path):
        print(f"[SKIP] Confusion matrix not found at {csv_path}. Skipping confusion matrix generation.")
        return

    try:
        df = pd.read_csv(csv_path)
        # Drop 'actual' column if present to get numeric matrix
        feature_cols = [c for c in df.columns if c != "actual"]
        matrix = df[feature_cols].to_numpy()

        fig, ax = plt.subplots(figsize=(8, 7))
        cax = ax.matshow(matrix, cmap="Blues")
        fig.colorbar(cax)

        classes = [str(i) for i in range(matrix.shape[0])]
        ax.set_xticks(range(len(classes)))
        ax.set_yticks(range(len(classes)))
        ax.set_xticklabels(classes)
        ax.set_yticklabels(classes)

        # Annotate counts inside cells
        max_val = matrix.max()
        for i in range(matrix.shape[0]):
            for j in range(matrix.shape[1]):
                val = int(matrix[i, j])
                color = "white" if val > (max_val / 2.0) else "black"
                ax.text(j, i, str(val), ha="center", va="center", color=color, fontsize=9)

        ax.set_xlabel("Predicted Class", fontsize=11, labelpad=10)
        ax.set_ylabel("Actual Class", fontsize=11)
        ax.set_title("OptDigits Confusion Matrix", fontsize=13, fontweight="bold", pad=20)

        plt.tight_layout()
        plt.savefig(out_path, dpi=300)
        plt.close()
        print(f"[SUCCESS] Confusion matrix heatmap saved to: {out_path}")

    except Exception as e:
        print(f"[ERROR] Failed to plot confusion matrix: {e}")

def main():
    results_dir = find_results_dir()
    plot_loss_curves(results_dir)
    plot_confusion_matrix(results_dir)

if __name__ == "__main__":
    main()

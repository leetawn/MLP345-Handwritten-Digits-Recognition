#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
TARGET_DIR="$REPO_ROOT/data/raw"

mkdir -p "$TARGET_DIR"

BASE_URL="https://archive.ics.uci.edu/ml/machine-learning-databases/optdigits"
FALLBACK_URL="https://archive.ics.uci.edu/dataset/80/optical+recognition+of+handwritten+digits"

FILES=("optdigits.tra" "optdigits.tes")

for FILE in "${FILES[@]}"; do
    DEST="$TARGET_DIR/$FILE"
    if [ -f "$DEST" ]; then
        echo "File already exists, skipping: $DEST"
        continue
    fi

    echo "Downloading $FILE from $BASE_URL/$FILE..."
    if curl -f -L -s -S "$BASE_URL/$FILE" -o "$DEST"; then
        echo "Successfully downloaded $FILE ($(wc -l < "$DEST") lines)."
    else
        echo "Failed to download $FILE from $BASE_URL/$FILE."
        echo "Please visit the dataset page directly: $FALLBACK_URL"
        rm -f "$DEST"
    fi
done

echo "Data download completed."

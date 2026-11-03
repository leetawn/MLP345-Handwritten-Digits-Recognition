# Contributing Guidelines

Welcome to the handwritten digit classifier team project. Both partners work concurrently following Git flow, conventional commits, and peer review standards.

---

## 1. Branch Naming Strategy
Branch from `main` and use descriptive prefixes:
- `feat/<feature-name>`: New feature implementations (e.g. `feat/data-loader`, `feat/trainer-loop`).
- `fix/<bug-name>`: Bug fixes (e.g. `fix/scaler-division-by-zero`).
- `docs/<topic>`: Documentation updates (e.g. `docs/experiment-tables`).
- `refactor/<target>`: Code refactoring without behavioral changes.
- `chore/<task>`: Tooling, dependency, or configuration tasks.

---

## 2. Conventional Commit Standards
All commits must follow the [Conventional Commits](https://www.conventionalcommits.org/) specification with an imperative, lowercase subject line (max 72 characters):

```
<type>(<scope>): <short imperative summary>
```

### Allowed Types:
- `feat`: New feature or stub implementation.
- `fix`: Bug fix.
- `docs`: Documentation changes.
- `refactor`: Code refactoring without changing functionality.
- `perf`: Performance improvements.
- `chore`: Tooling, build scripts, or repository housekeeping.
- `ci`: CI pipeline updates.

### Allowed Scopes:
- `engine`: Backprop.Engine neural network classes.
- `data`: Dataset loading, scaling, splitting, encoding.
- `training`: Trainer, loss calculations, CsvLossLogger.
- `eval`: Evaluator, metrics, report formatting.
- `experiments`: Grid search and sweep runner.
- `docs`: Repository documentation in `docs/` or `README.md`.
- `tools`: Python scripts, bash/PowerShell download scripts.
- `ci`: GitHub Actions workflows.

*Example*: `feat(data): implement CsvDatasetLoader with comma parsing`

---

## 3. Pull Request & Code Review Rules
1. **Mandatory Peer Review**:
   Every Pull Request must be reviewed and approved by the other partner prior to merging.
2. **Pull Request Template**:
   Complete all sections of `.github/pull_request_template.md` (summary, related task, owner, checklist, verification steps).
3. **CI Requirement**:
   All GitHub Actions CI builds must pass with zero compilation errors before merging.
4. **Squash Merge**:
   Always merge feature branches into `main` using **Squash and merge** to maintain a clean, linear git history on `main`.

---

## 4. Ownership Boundaries

| Area | Primary Owner | Secondary Reviewer | Modules |
| :--- | :---: | :---: | :--- |
| **Engine** | Partner A | Partner B | `Backprop.Engine` (NeuralNet, INeuron, HNeuron, ONeuron, Rng) |
| **Training** | Partner A | Partner B | `Trainer`, `CsvLossLogger`, `TrainingOptions`, `TrainingResult` |
| **Experiments** | Partner A | Partner B | `ExperimentRunner`, `ExperimentGrid`, `docs/experiments.md` |
| **Data Pipeline** | Partner B | Partner A | `CsvDatasetLoader`, `MinMaxScaler`, `LabelEncoder`, `DatasetSplitter` |
| **Evaluation** | Partner B | Partner A | `Evaluator`, `EvaluationReport`, `ClassMetrics` |
| **Tools & Plots** | Partner B | Partner A | `tools/plot_results.py`, `download-data.*`, `requirements.txt` |
| **Documentation & Results** | Partner B | Partner A | `docs/data-description.md`, `README.md` results tables |

---

## 5. Verification Checklist Before Opening a PR
- [ ] Project builds cleanly via `dotnet build` with 0 errors and 0 warnings.
- [ ] All public members and types include complete XML documentation comments (`///`).
- [ ] No commented-out legacy code or temporary debug statements remain.
- [ ] Stub implementation fulfills all numbered `// TODO` items top to bottom.
- [ ] Code is formatted according to `.editorconfig` (4-space indentation, file-scoped namespaces).

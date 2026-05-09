#!/usr/bin/env python3
"""Render charts from EvalResultStore JSON snapshots.

Reads .AgentEval/ECS2026MAF_Evals/*.json from the repo root and writes
matching .AgentEval/charts/*.png and .svg files. Two snapshot shapes are
recognised: EvalSnapshot (single run, has CriteriaMetCount/CriteriaTotal)
and StochasticSnapshot (multi-run, has a Scores list).

Run from the repo root:
    python .agent/skills/run-evals-and-graph/render.py

Requires matplotlib:
    python -m pip install --user matplotlib
"""

from __future__ import annotations

import json
from pathlib import Path
from typing import Any

try:
    import matplotlib

    matplotlib.use("Agg")  # headless
    import matplotlib.pyplot as plt
except ImportError:
    raise SystemExit(
        "matplotlib not installed — run: python -m pip install --user matplotlib"
    )


REPO_ROOT = Path(__file__).resolve().parents[3]
SNAPSHOT_DIR = REPO_ROOT / ".AgentEval" / "ECS2026MAF_Evals"
CHARTS_DIR = REPO_ROOT / ".AgentEval" / "charts"


def is_stochastic(snapshot: dict[str, Any]) -> bool:
    return "Scores" in snapshot and isinstance(snapshot.get("Scores"), list)


def render_stochastic(key: str, snap: dict[str, Any]) -> None:
    scores: list[int] = snap.get("Scores", [])
    if not scores:
        print(f"  skip {key}: empty Scores")
        return

    label = snap.get("Label") or key
    mean = snap.get("MeanScore", sum(scores) / len(scores))
    smin = snap.get("MinScore", min(scores))
    smax = snap.get("MaxScore", max(scores))
    spread = smax - smin

    fig, ax = plt.subplots(figsize=(8, 4.5))
    ax.bar(range(1, len(scores) + 1), scores, color="#4C72B0")
    ax.axhline(mean, color="#DD8452", linestyle="--", label=f"mean = {mean:.1f}")
    ax.set_xlabel("Run")
    ax.set_ylabel("Score (0-100)")
    ax.set_ylim(0, 100)
    ax.set_xticks(range(1, len(scores) + 1))
    ax.set_title(f"{label}  (min={smin}, max={smax}, spread={spread})")
    ax.legend(loc="lower right")
    fig.tight_layout()

    write(fig, key)
    plt.close(fig)


def render_eval(key: str, snap: dict[str, Any]) -> None:
    met = int(snap.get("CriteriaMetCount", 0))
    total = int(snap.get("CriteriaTotal", 0))
    if total == 0:
        # Placeholder snapshots from Eval01's helper-demo land here. Render a
        # tiny "no criteria" placeholder so the user knows it was found.
        fig, ax = plt.subplots(figsize=(6, 2))
        ax.text(
            0.5,
            0.5,
            f"{snap.get('Label') or key}\n(no criteria — placeholder)",
            ha="center",
            va="center",
        )
        ax.axis("off")
        fig.tight_layout()
        write(fig, key)
        plt.close(fig)
        return

    missed = total - met
    fig, ax = plt.subplots(figsize=(6, 3))
    ax.bar(["Met", "Missed"], [met, missed], color=["#55A868", "#C44E52"])
    ax.set_ylim(0, total)
    ax.set_ylabel("Criteria")
    ax.set_title(f"{snap.get('Label') or key}  ({met}/{total})")
    fig.tight_layout()
    write(fig, key)
    plt.close(fig)


def render_summary(stochastic_snaps: list[tuple[str, dict[str, Any]]]) -> None:
    if len(stochastic_snaps) < 2:
        return

    labels = [s.get("Architecture") or k for k, s in stochastic_snaps]
    means = [s.get("MeanScore", 0) for _, s in stochastic_snaps]
    spreads = [(s.get("MaxScore", 0) - s.get("MinScore", 0)) for _, s in stochastic_snaps]

    fig, (ax_mean, ax_spread) = plt.subplots(1, 2, figsize=(10, 4))
    ax_mean.bar(labels, means, color="#4C72B0")
    ax_mean.set_ylim(0, 100)
    ax_mean.set_title("Mean score")
    ax_mean.tick_params(axis="x", labelrotation=15)
    ax_spread.bar(labels, spreads, color="#DD8452")
    ax_spread.set_title("Score spread (max - min)")
    ax_spread.tick_params(axis="x", labelrotation=15)
    fig.suptitle("Stochastic comparison")
    fig.tight_layout()
    write(fig, "_stochastic_summary")
    plt.close(fig)


def write(fig, key: str) -> None:
    CHARTS_DIR.mkdir(parents=True, exist_ok=True)
    png = CHARTS_DIR / f"{key}.png"
    svg = CHARTS_DIR / f"{key}.svg"
    fig.savefig(png, dpi=150)
    fig.savefig(svg)
    print(f"  wrote {png.relative_to(REPO_ROOT)}")
    print(f"        {svg.relative_to(REPO_ROOT)}")


def main() -> int:
    if not SNAPSHOT_DIR.exists():
        print(f"no snapshots found at {SNAPSHOT_DIR.relative_to(REPO_ROOT)}")
        print("run an eval first: dotnet run --project AgentEval/samples/ECS2026MAF.Eval")
        return 1

    files = sorted(SNAPSHOT_DIR.glob("*.json"))
    if not files:
        print(f"no .json snapshots in {SNAPSHOT_DIR.relative_to(REPO_ROOT)}")
        return 1

    stochastics: list[tuple[str, dict[str, Any]]] = []
    for path in files:
        key = path.stem
        try:
            snap = json.loads(path.read_text())
        except json.JSONDecodeError as exc:
            print(f"  skip {key}: invalid JSON ({exc})")
            continue

        if is_stochastic(snap):
            render_stochastic(key, snap)
            stochastics.append((key, snap))
        else:
            render_eval(key, snap)

    render_summary(stochastics)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

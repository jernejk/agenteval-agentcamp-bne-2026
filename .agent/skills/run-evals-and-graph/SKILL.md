---
name: run-evals-and-graph
description: Run the eval app, gather the JSON snapshots EvalResultStore writes, and render charts (per-run scores for stochastic evals; mean+spread summary). Use when the user wants visual eval output for a writeup, a slide, or a comparison.
---

You're producing visual eval output. Upstream AgentEval already supports JSON / JUnit / Markdown / TRX / CSV exports (see [docs/export.md](https://github.com/joslat/AgentEval/blob/main/docs/export.md) upstream), but it does **not** ship a chart renderer. This skill adds that layer using the snapshots `EvalResultStore` already persists.

If a future upstream release adds native charting, swap the local script for the upstream command — but right now this is the simplest path that works.

## Inputs

`EvalResultStore` writes one JSON file per snapshot to:

```text
.AgentEval/ECS2026MAF_Evals/{key}.json
```

Single-run evals (Eval01, Eval02, Eval03) write `EvalSnapshot` records. Stochastic evals (Eval04, Eval05) write `StochasticSnapshot` records — those have a `Scores` array, `MeanScore`, `MinScore`, `MaxScore`, `Spread` worth charting.

Schema lives in [`AgentEval/samples/ECS2026MAF.Eval/EvalResultStore.cs`](../../../AgentEval/samples/ECS2026MAF.Eval/EvalResultStore.cs).

## Step 1 — Run the evals

Interactive run:

```bash
dotnet run --project AgentEval/samples/ECS2026MAF.Eval
```

Press the menu key for the eval(s) you care about. For stochastic comparisons, run `4` and `5` so both snapshots exist.

If the eval bodies are still placeholders, this skill has nothing useful to chart. Replace at least one body via the [create-eval-test](../create-eval-test/SKILL.md) skill first.

If you want a non-interactive runner, that's a separate piece of work — see [add-console-prompt](../add-console-prompt/SKILL.md) for the menu pattern; you'd add a top-level `args[0]` switch in `Program.cs` that calls `RunAsync` directly.

## Step 2 — Render charts

The bundled `render.py` reads `.AgentEval/ECS2026MAF_Evals/*.json` and writes PNG + SVG to `.AgentEval/charts/`. Run from the repo root:

```bash
python .agent/skills/run-evals-and-graph/render.py
```

PowerShell on Windows is identical:

```powershell
python .agent\skills\run-evals-and-graph\render.py
```

The script needs `matplotlib`. Install once:

```bash
python -m pip install --user matplotlib
```

What it produces:

- For each `StochasticSnapshot` — a per-run bar chart with the pass/fail line drawn, mean/min/max annotations.
- A side-by-side mean/spread summary across all stochastic snapshots in the directory (useful for the "agent vs workflow" comparison).
- For each `EvalSnapshot` — a small criteria-met-vs-missed bar.

Outputs land at `.AgentEval/charts/{snapshot-key}.png` and `.AgentEval/charts/{snapshot-key}.svg`.

## Step 3 — Sanity check before sharing

The eval snapshots **never** contain the API key (only score numbers and labels), so charts are safe to paste into slides. But:

- Don't include the endpoint URL or deployment name in chart titles if you're sharing externally — those leak which subscription/region you're using.
- Don't share `dotnet user-secrets list` output as a chart, ever. That's a different shape of artefact and it's never appropriate for sharing.

## Step 4 — Cleanup

Charts are gitignored (`.AgentEval/charts/`). If you want to keep one, copy it out of `.AgentEval/charts/` into wherever you store talk assets — don't try to commit it.

## Don'ts

- Don't add a charting NuGet to the .NET project. Keep it as an out-of-band Python tool — the .NET app stays minimal.
- Don't extend `EvalResultStore` with chart-rendering responsibility. It's a serialiser; the renderer is downstream.
- Don't pull `pandas` or `seaborn` for what `matplotlib` can do directly. Slim is the goal.

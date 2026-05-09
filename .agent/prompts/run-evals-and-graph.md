# Prompt — Run evals and render charts

Mirror of [`.agent/skills/run-evals-and-graph/SKILL.md`](../skills/run-evals-and-graph/SKILL.md).

Goal: produce visual eval output (PNG / SVG charts) for a slide, writeup, or comparison. Upstream AgentEval supports JSON / JUnit / Markdown / TRX / CSV exports but no charting; this prompt fills that gap.

## Steps

1. **Run the eval(s).** Pick from the menu of `AgentEval/samples/ECS2026MAF.Eval`. For stochastic comparisons, run `4` and `5` so both snapshots exist. If the bodies are still placeholders, replace at least one via the `create-eval-test` prompt first — placeholder snapshots only show "no criteria — placeholder" in the chart.

2. **Render the charts.**

   ```bash
   python .agent/skills/run-evals-and-graph/render.py
   ```

   Same command on PowerShell (Windows). Needs `matplotlib`:

   ```bash
   python -m pip install --user matplotlib
   ```

3. **Read the output.**
   - Per stochastic snapshot: bar chart of run scores with mean line, min/max/spread in title.
   - Side-by-side comparison: mean and spread across all stochastic snapshots.
   - Per single-run snapshot: criteria-met-vs-missed bar.
   - All written to `.AgentEval/charts/{key}.png` and `.svg`.

## Sources

- Snapshot directory: `.AgentEval/ECS2026MAF_Evals/`
- Snapshot schema: `AgentEval/samples/ECS2026MAF.Eval/EvalResultStore.cs`
- Charts directory: `.AgentEval/charts/` (gitignored)

## Don'ts

- Don't paste full chart titles that include endpoint URLs or deployment names into external slides — that leaks subscription detail. Strip them in `render.py` (or override the title string before rendering).
- Don't put `dotnet user-secrets list` output in a chart, ever.
- Don't add a charting NuGet to the .NET project; keep the renderer Python-only.

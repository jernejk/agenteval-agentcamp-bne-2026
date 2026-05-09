---
name: create-eval-test
description: Create or replace an eval test under AgentEval/samples/ECS2026MAF.Eval/Evals/. Use when adding a new eval (Eval06, BonusEval03, etc.) or replacing one of the placeholder bodies (Eval01–05) with real assertions. Covers naming, menu wiring, criteria, snapshot persistence.
---

You are adding or replacing an eval in `AgentEval/samples/ECS2026MAF.Eval/`. The placeholder bodies are intentional — the workshop replaces them with real `RunAsync` implementations against the agents in `AgentEval/samples/ECS2026MAF/Agents/`.

## File layout

| Path | Purpose |
| --- | --- |
| `AgentEval/samples/ECS2026MAF.Eval/Evals/EvalNN_<Subject>.cs` | One file per eval. `NN` is two digits (01..99). |
| `AgentEval/samples/ECS2026MAF.Eval/Evals/Bonus/BonusEvalNN_<Subject>.cs` | Bonus evals (red-teaming, model comparison, anything that needs extra setup). |
| `AgentEval/samples/ECS2026MAF.Eval/TravelEvalCriteria.cs` | Canonical criteria sets, reused across evals so the same standards apply everywhere. |
| `AgentEval/samples/ECS2026MAF.Eval/Program.cs` | Menu wiring. Each eval gets a numeric key; the Bonus submenu has its own keys. |
| `AgentEval/samples/ECS2026MAF.Eval/EvalResultStore.cs` | Snapshot persistence under `.AgentEval/ECS2026MAF_Evals/{key}.json`. Already implemented. |
| `AgentEval/samples/ECS2026MAF.Eval/EvalPrinter.cs` | Console rendering helpers. Already implemented. |

Convention: namespace stays `ECS2026MAF.Evals` (matches upstream); bonus namespace is `ECS2026MAF.Evals.Bonus`. Both are exposed in `GlobalUsings.cs`.

## Skeleton

```csharp
// Originally from joslat/AgentEval at samples/ECS2026MAF.Evals/Evals/<file>.cs.
// Modified for AgentCamp Brisbane 2026.
//
// SPDX-License-Identifier: MIT
// Copyright (c) 2026 ECS2026 Demo

using AgentEval.Assertions;
using AgentEval.Core;
using AgentEval.MAF;
using AgentEval.Models;
using ECS2026MAF.Agents;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;

namespace ECS2026MAF.Evals;

public static class EvalNN_<Subject>
{
    public static async Task RunAsync()
    {
        if (!Config.IsConfigured) { Console.WriteLine("Skipping — credentials missing."); return; }

        var rawAgent = TravelAgentFactory.Create();              // or whichever factory
        var agent    = new MAFAgentAdapter(rawAgent);
        var azureClient = new AzureOpenAIClient(Config.Endpoint, Config.KeyCredential);
        var harness  = new MAFEvaluationHarness(
            azureClient.GetChatClient(Config.Model).AsIChatClient(),
            verbose: false);

        var testCase = new TestCase
        {
            Name  = "EvalNN — short label",
            Input = "...the test input the agent receives...",
            ExpectedTools          = ["GetInfoAbout", "SearchFlights" /* etc */],
            ExpectedOutputContains = "Tokyo",
            EvaluationCriteria     = [..TravelEvalCriteria.EvalNN],   // add a set in TravelEvalCriteria.cs
            PassingScore           = 70
        };

        var result = await harness.RunEvaluationStreamingAsync(
            agent, testCase,
            options: new EvaluationOptions { TrackTools = true, EvaluateResponse = true, ModelName = Config.Model });

        EvalPrinter.PrintAgentResult(result, testCase.ExpectedTools ?? [], label: "EvalNN");
        EvalPrinter.PrintLlmJudge(result.Score, result.CriteriaResults, result.Suggestions, label: "EvalNN");

        // Optional: tool-order assertions
        result.ToolUsage!.Should()
            .HaveCalledTool("GetInfoAbout").BeforeTool("SearchFlights")
            .And()
            .HaveCalledTool("BookFlight");

        // Persist a snapshot so Eval03-style comparisons can pick it up.
        EvalResultStore.Save("evalNN_<key>", new EvalSnapshot
        {
            Architecture     = "Single Agent",     // or "Workflow"
            Label            = "EvalNN — short label",
            LlmScore         = result.Score,
            CriteriaScore    = ScorePercent(result.CriteriaResults),
            CriteriaMetCount = result.CriteriaResults?.Count(c => c.Met) ?? 0,
            CriteriaTotal    = result.CriteriaResults?.Count        ?? 0,
            ToolCallCount    = result.ToolCallCount,
            BookFlightCount  = result.ToolUsage?.Calls.Count(c => c.Name == "BookFlight") ?? 0,
            Passed           = result.Passed,
            DurationMs       = (long)(result.Performance?.TotalDuration.TotalMilliseconds ?? 0),
        });
    }

    private static int ScorePercent(IReadOnlyList<CriterionResult>? criteria) =>
        criteria is null || criteria.Count == 0
            ? 0
            : criteria.Count(c => c.Met) * 100 / criteria.Count;
}
```

## Menu wiring

Open `AgentEval/samples/ECS2026MAF.Eval/Program.cs` and:

1. Add a new line to the menu's heredoc, preserving alignment.
2. Add a `case 'N': await EvalNN_<Subject>.RunAsync(); break;` in `ShowMenuAsync`.
3. Numbers go in order. Don't reuse a key.
4. Keep `Q` as quit, `B` as the bonus submenu entry.

For bonus evals, edit `ShowBonusMenuAsync` instead.

## Criteria

If your eval needs new criteria, add them as a `static readonly string[] EvalNN = [...]` field in `TravelEvalCriteria.cs`. Keep entries short and assertable (e.g. "Itinerary covers all requested cities"). Six to twelve criteria is typical.

## Placeholder bodies

If you're keeping the eval as a placeholder for now (e.g. you're stubbing it before the workshop), shorter is better:

```csharp
public static Task RunAsync()
{
    Console.WriteLine();
    Console.WriteLine("  EvalNN — short label (placeholder)");
    Console.WriteLine("  ─────────────────────────────────");
    Console.WriteLine("  TODO: implement during the workshop. See .agent/skills/create-eval-test/SKILL.md");
    return Task.CompletedTask;
}
```

Eval01 is special — it doubles as the helper-API exercise scaffold (writes/reads an `EvalSnapshot` via `EvalResultStore` even without scoring). Don't lose that pattern when you replace it with the real assertions; the persistence call belongs in the real version too.

## Verifying

Build and run the eval app interactively:

```bash
dotnet build AgentEval-AgentCampBrisbane2026.slnx
dotnet run --project AgentEval/samples/ECS2026MAF.Eval
```

Pick the new menu key, confirm the eval runs end-to-end, and check `.AgentEval/ECS2026MAF_Evals/` for the snapshot file.

## Don'ts

- Don't add new package references when an existing one in `Directory.Packages.props` works.
- Don't add `Microsoft.Extensions.Hosting`/DI scaffolding — the eval app is intentionally a static-class console.
- Don't print API keys, endpoints, or full snapshots that contain secrets to stdout.
- Don't hard-code deployment names — read `Config.Model` like the existing evals.

// Originally from joslat/AgentEval at samples/ECS2026MAF.Evals/Evals/Eval01_TravelAgentEvals.cs.
// Modified for AgentCamp Brisbane 2026: real eval body removed, replaced with a
// helper-demo placeholder. The exercise during the workshop is to put the eval
// logic back in — see .agent/skills/create-eval-test/SKILL.md.
// Special thanks to Jose Luis Latorre.
//
// SPDX-License-Identifier: MIT
// Copyright (c) 2026 ECS2026 Demo

namespace ECS2026MAF.Evals;

/// <summary>
/// Eval 01 placeholder.
///
/// Real version: behavioural + tool-order assertions on the single TravelAgent
/// against the canonical Tokyo + Cologne trip — see
/// <see href="https://github.com/joslat/AgentEval/blob/main/samples/ECS2026MAF.Evals/Evals/Eval01_TravelAgentEvals.cs"/>.
///
/// This placeholder demonstrates the supporting helpers (<see cref="EvalResultStore"/>,
/// <see cref="EvalSnapshot"/>) without scoring or model calls so the wiring can be
/// exercised before the eval body lands.
/// </summary>
public static class Eval01_TravelAgentEvals
{
    private const string SnapshotKey = "eval01_agent";

    public static Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("  Eval 01 — TravelAgent (placeholder)");
        Console.WriteLine("  ───────────────────────────────────");
        Console.WriteLine();
        Console.WriteLine("  TODO: replace this stub with the real Eval01 during the workshop.");
        Console.WriteLine("  Exercise: use the demo below as scaffolding for the real RunAsync.");
        Console.WriteLine("  See .agent/skills/create-eval-test/SKILL.md");
        Console.WriteLine();
        Console.WriteLine("  Helper demo — write/read an EvalSnapshot via EvalResultStore:");
        Console.WriteLine();

        var stub = new EvalSnapshot
        {
            Architecture = "Single Agent",
            Label        = "Eval 01 — TravelAgent (placeholder stub)",
            LlmScore         = 0,
            CriteriaScore    = 0,
            CriteriaMetCount = 0,
            CriteriaTotal    = 0,
            ToolCallCount    = 0,
            BookFlightCount  = 0,
            Passed           = false,
            DurationMs       = 0,
        };

        EvalResultStore.Save(SnapshotKey, stub);
        Console.WriteLine($"    saved   -> {EvalResultStore.StorageLocation}/{SnapshotKey}.json");

        var loaded = EvalResultStore.Load(SnapshotKey);
        Console.WriteLine($"    loaded  -> {loaded?.Label ?? "<null>"}");
        Console.WriteLine($"    exists  -> {EvalResultStore.Exists(SnapshotKey)}");
        Console.WriteLine($"    age     -> {EvalResultStore.GetAge(SnapshotKey)}");

        return Task.CompletedTask;
    }
}

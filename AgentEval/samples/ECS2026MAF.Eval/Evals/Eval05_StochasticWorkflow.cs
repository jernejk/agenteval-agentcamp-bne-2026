// Originally from joslat/AgentEval at samples/ECS2026MAF.Evals/Evals/Eval05_StochasticWorkflow.cs.
// Modified for AgentCamp Brisbane 2026: real eval body removed, replaced with a placeholder.
// Special thanks to Jose Luis Latorre.
//
// SPDX-License-Identifier: MIT
// Copyright (c) 2026 ECS2026 Demo

namespace ECS2026MAF.Evals;

/// <summary>
/// Eval 05 placeholder.
///
/// Real version: runs the four-agent TripPlanner workflow five times and
/// reports score spread, mean, and pass rate — see
/// <see href="https://github.com/joslat/AgentEval/blob/main/samples/ECS2026MAF.Evals/Evals/Eval05_StochasticWorkflow.cs"/>.
/// </summary>
public static class Eval05_StochasticWorkflow
{
    public static Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("  Eval 05 — Stochastic Workflow (placeholder)");
        Console.WriteLine("  ───────────────────────────────────────────");
        Console.WriteLine();
        Console.WriteLine("  TODO: implement Eval05 during the workshop.");
        Console.WriteLine("  See .agent/skills/create-eval-test/SKILL.md");
        return Task.CompletedTask;
    }
}

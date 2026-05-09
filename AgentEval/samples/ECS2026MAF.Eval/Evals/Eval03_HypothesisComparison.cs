// Originally from joslat/AgentEval at samples/ECS2026MAF.Evals/Evals/Eval03_HypothesisComparison.cs.
// Modified for AgentCamp Brisbane 2026: real eval body removed, replaced with a placeholder.
// Special thanks to Jose Luis Latorre.
//
// SPDX-License-Identifier: MIT
// Copyright (c) 2026 ECS2026 Demo

namespace ECS2026MAF.Evals;

/// <summary>
/// Eval 03 placeholder.
///
/// Real version: loads the snapshots saved by Eval01 and Eval02 and prints a
/// side-by-side determinism comparison without any model calls — see
/// <see href="https://github.com/joslat/AgentEval/blob/main/samples/ECS2026MAF.Evals/Evals/Eval03_HypothesisComparison.cs"/>.
/// </summary>
public static class Eval03_HypothesisComparison
{
    public static Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("  Eval 03 — Hypothesis Comparison (placeholder)");
        Console.WriteLine("  ─────────────────────────────────────────────");
        Console.WriteLine();
        Console.WriteLine("  TODO: implement Eval03 during the workshop.");
        Console.WriteLine("  See .agent/skills/create-eval-test/SKILL.md");
        return Task.CompletedTask;
    }
}

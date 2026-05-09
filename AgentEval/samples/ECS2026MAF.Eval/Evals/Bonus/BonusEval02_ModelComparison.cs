// SPDX-License-Identifier: MIT
// Copyright (c) 2026 Jernej Kavka (JK)
//
// Bonus eval scaffold for AgentCamp Brisbane 2026. Not part of upstream
// joslat/AgentEval — added so the workshop has an obvious "next layer"
// to extend after the core five evals are wired up.

namespace ECS2026MAF.Evals.Bonus;

/// <summary>
/// Bonus Eval 02 — Model comparison placeholder.
///
/// Runs the same TravelAgent task against two model deployments (primary +
/// secondary) and compares scores, tool order, and latency. Useful for picking
/// between gpt-5.4-mini, Kimi K2.x, DeepSeek v4, and similar.
/// </summary>
public static class BonusEval02_ModelComparison
{
    public static Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("  Bonus Eval 02 — Model Comparison (placeholder)");
        Console.WriteLine("  ──────────────────────────────────────────────");
        Console.WriteLine();
        Console.WriteLine("  TODO (Bonus): compare two model deployments on the same TravelAgent task.");
        Console.WriteLine();
        Console.WriteLine("  Requires:");
        Console.WriteLine("    - A second Azure OpenAI deployment alongside the primary.");
        Console.WriteLine("    - User-secret AzureOpenAI:DeploymentSecondary set to its name.");
        Console.WriteLine("    - Decide what to compare: score, tool order, latency, cost.");
        Console.WriteLine();
        Console.WriteLine("  Reference: https://github.com/joslat/AgentEval/blob/main/docs/model-comparison.md");
        Console.WriteLine("  See also docs/MODEL-OPTIONS.md in this repo for region/model picks.");
        return Task.CompletedTask;
    }
}

// SPDX-License-Identifier: MIT
// Copyright (c) 2026 Jernej Kavka (JK)
//
// Bonus eval scaffold for AgentCamp Brisbane 2026. Not part of upstream
// joslat/AgentEval — added so the workshop has an obvious "next layer"
// to extend after the core five evals are wired up.

namespace ECS2026MAF.Evals.Bonus;

/// <summary>
/// Bonus Eval 01 — Red-teaming placeholder.
///
/// Sends a curated set of adversarial prompts at the TravelAgent and asserts
/// that the agent declines, escalates, or stays on task — your call. Fill in
/// the dataset and the judge prompt during a follow-up session.
/// </summary>
public static class BonusEval01_RedTeaming
{
    public static Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("  Bonus Eval 01 — Red-Teaming (placeholder)");
        Console.WriteLine("  ─────────────────────────────────────────");
        Console.WriteLine();
        Console.WriteLine("  TODO (Bonus): red-team the TravelAgent against an adversarial dataset.");
        Console.WriteLine();
        Console.WriteLine("  Requires:");
        Console.WriteLine("    - A red-team prompt set (datasets/redteam.jsonl) — not in this repo yet.");
        Console.WriteLine("    - A judge model deployment (can be the same as the primary).");
        Console.WriteLine("    - An acceptance policy: which categories must the agent decline?");
        Console.WriteLine();
        Console.WriteLine("  Reference: https://github.com/joslat/AgentEval/blob/main/docs/redteam.md");
        return Task.CompletedTask;
    }
}

# Prompt — Create or replace an eval

Mirror of [`.agent/skills/create-eval-test/SKILL.md`](../skills/create-eval-test/SKILL.md) for Codex CLI and other harnesses that don't load Claude-style skills.

You are adding a new eval class under `AgentEval/samples/ECS2026MAF.Eval/Evals/` (or replacing one of the placeholder bodies Eval01–05). The placeholders are intentional — the workshop replaces them with real `RunAsync` implementations against the agents in `AgentEval/samples/ECS2026MAF/Agents/`.

## Steps

1. **Pick a slot.** Replacing a placeholder? Open `Evals/EvalNN_<Subject>.cs`. Adding a new eval? Use the next free `NN`. Bonus eval (extra setup required)? Put it under `Evals/Bonus/` with the `BonusEvalNN_<Name>` prefix.

2. **Match the convention.**
   - Namespace: `ECS2026MAF.Evals` (or `.Bonus`).
   - Static class with a single `public static Task RunAsync()`.
   - First line: `if (!Config.IsConfigured) { Console.WriteLine("Skipping — credentials missing."); return; }`.
   - Read deployment info via `Config.Endpoint` / `Config.KeyCredential` / `Config.Model`. Don't hard-code.

3. **Reuse helpers.**
   - `MAFEvaluationHarness` and `MAFAgentAdapter` from `AgentEval.MAF`.
   - `EvalPrinter.PrintAgentResult` and `PrintLlmJudge` from the eval project.
   - `EvalResultStore.Save(key, EvalSnapshot { ... })` so Eval03-style comparisons can pick the snapshot up later.
   - `TravelEvalCriteria.EvalNN` lists for criteria — add new entries there rather than inline.

4. **Wire the menu.** Open `AgentEval/samples/ECS2026MAF.Eval/Program.cs`. Add a heredoc line preserving box-drawing alignment. Add a `case '<key>': await EvalNN_<Subject>.RunAsync(); break;` to the switch. For bonus evals edit `ShowBonusMenuAsync` instead.

5. **Build and verify.**
   ```bash
   dotnet build AgentEval-AgentCampBrisbane2026.slnx
   dotnet run --project AgentEval/samples/ECS2026MAF.Eval
   ```
   Press the new menu key. Confirm the eval runs end-to-end and `.AgentEval/ECS2026MAF_Evals/<key>.json` shows up.

## Placeholder body (if you're stubbing)

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

Eval01 is the exception — it doubles as the helper-API exercise scaffold. Even when you replace it with the real assertions, keep the `EvalResultStore.Save` call so Eval03 can compare snapshots.

## Real body skeleton

See the SKILL.md mirror of this prompt for the full template.

## Don'ts

- Don't add a new package reference when an existing one in `Directory.Packages.props` works.
- Don't print API keys or full config snapshots.
- Don't hard-code model names — read `Config.Model`.
- Don't widen scope into DI scaffolding. The eval app is a deliberate static-class console.

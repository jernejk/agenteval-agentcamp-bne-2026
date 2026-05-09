# Prompt — Add a console menu option

Mirror of [`.agent/skills/add-console-prompt/SKILL.md`](../skills/add-console-prompt/SKILL.md).

Pick the right console first.

| You're adding | Project | File location |
| --- | --- | --- |
| A new MAF demo (interactive agent / workflow) | `AgentEval/samples/ECS2026MAF/Program.cs` | New file under `Demos/` |
| A new eval (assertions, scoring) | `AgentEval/samples/ECS2026MAF.Eval/Program.cs` | New file under `Evals/` |
| A bonus eval (extra setup required) | `AgentEval/samples/ECS2026MAF.Eval/Program.cs` (bonus submenu) | New file under `Evals/Bonus/` |

Eval bodies themselves: see [`.agent/prompts/create-eval-test.md`](create-eval-test.md). This prompt is just the menu-wiring half.

## Menu pattern

Both consoles use a heredoc menu inside `ShowMenuAsync` (and `ShowBonusMenuAsync` in the eval app), then a `switch` on `Console.ReadKey(intercept: true).KeyChar`.

To add an entry:

1. Add a heredoc line preserving box-drawing alignment.
2. Add a `case '<key>': await Whatever.RunAsync(); break;` to the matching switch.
3. Use the next free numeric key. `Q`/`q` is quit; `B`/`b` is the bonus submenu in the eval app — don't reuse them.

## Handler shape

Static class with a single `public static Task RunAsync()`. Match the existing files in `Demos/` or `Evals/` for tone (matching console colours, missing-credentials guard, etc.).

## Verifying

```bash
dotnet build AgentEval-AgentCampBrisbane2026.slnx
dotnet run --project AgentEval/samples/ECS2026MAF        # or .Eval
```

Press the new key, then `Q` to confirm quit still works. If you added a Bonus entry, also confirm `B` opens it from the main menu and `Q` from the bonus menu returns rather than exits.

## Don'ts

- Don't break alphabetical / numeric ordering of existing entries.
- Don't add Spectre.Console / DI / hosting — the consoles are deliberately minimal.
- Don't change the heredoc box-drawing characters; they line up the same way on macOS, Windows Terminal, and Linux.

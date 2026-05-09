---
name: add-console-prompt
description: Add a new menu option to either the demo console (AgentEval/samples/ECS2026MAF/Program.cs) or the eval console (AgentEval/samples/ECS2026MAF.Eval/Program.cs). Covers menu pattern, file placement, and how the Bonus submenu fits in.
---

You're adding a new menu option to one of the workshop's two consoles.

## Which console?

- Adding a new **demo** (an MAF agent or workflow you can interact with)? → `AgentEval/samples/ECS2026MAF/Program.cs`. New file goes under `Demos/`.
- Adding a new **eval** (assertions, scoring, snapshot comparison)? → `AgentEval/samples/ECS2026MAF.Eval/Program.cs`. New file goes under `Evals/` (or `Evals/Bonus/` if it needs extra setup).

If you're adding an eval, also read [.agent/skills/create-eval-test/SKILL.md](../create-eval-test/SKILL.md) — that one covers the eval body itself; this one covers the menu wiring.

## Menu pattern

Both consoles follow the same shape:

```csharp
Console.Write("  Select: ");
var key = Console.ReadKey(intercept: true).KeyChar;
Console.WriteLine();

switch (key)
{
    case '1': await Demo01_TravelAgent.RunAsync();         break;
    case '2': await Demo02_TripPlannerWorkflow.RunAsync(); break;
    // ...
    case 'q' or 'Q': return;
}
```

To add an option:

1. Add a line to the heredoc menu inside `ShowMenuAsync()` (or `ShowBonusMenuAsync()`). Keep alignment intact — the box-drawing characters are pickier than they look.
2. Add a `case` to the `switch`. Use the next free numeric key. `Q` stays quit; `B` stays the bonus submenu entry; don't reuse them.
3. The handler must be `Task RunAsync()` (no parameters) on a static class. Match the existing style.

## New demo (under `ECS2026MAF/Demos/`)

```csharp
// SPDX-License-Identifier: MIT
// Copyright (c) 2026 ECS2026 Demo

using Microsoft.Agents.AI;
using ECS2026MAF.Agents;

namespace ECS2026MAF.Demos;

public static class Demo05_<Name>
{
    public static async Task RunAsync()
    {
        Console.Clear();
        Console.WriteLine("Demo 05 — <Name>");
        Console.WriteLine("─────────────────");

        if (!Config.IsConfigured)
        {
            Console.WriteLine("  Azure OpenAI credentials missing. Run `azd up`.");
            return;
        }

        var agent = TravelAgentFactory.Create();   // or your own factory
        var thread = agent.GetNewThread();

        Console.Write("> ");
        var input = Console.ReadLine();
        await foreach (var update in agent.RunStreamingAsync(input ?? "", thread))
            Console.Write(update.Text);
        Console.WriteLine();
    }
}
```

Then add to `Program.cs`:

```text
║   5  Demo 05 <Name>          Short description                              ║
```

```csharp
case '5': await Demo05_<Name>.RunAsync(); break;
```

## New eval menu entry

If the eval is a regular workshop eval, it goes into the main menu (`ShowMenuAsync`). If it's "interesting but extra setup required" (red-teaming, model comparison, security scanning), put it under the Bonus submenu (`ShowBonusMenuAsync`).

Bonus submenu wiring:

```csharp
// inside ShowBonusMenuAsync
case '3': await BonusEval03_<Name>.RunAsync(); break;
```

Adjust the heredoc:

```text
║   3  <Name>                  Short description                              ║
```

## Conventions

- **Numeric keys are 1-based.** Don't go above `9` without splitting into a submenu.
- **Don't break alphabetical / numeric ordering.** New demos slot in numerically; new evals append.
- **Console output** mirrors the existing style: green for the main eval menu, cyan for the demo menu and bonus menu. Match what's already there.
- **Don't add cross-platform helpers** like Spectre.Console here — the consoles are intentionally minimal.

## Verifying

```bash
dotnet build AgentEval-AgentCampBrisbane2026.slnx
dotnet run --project AgentEval/samples/ECS2026MAF        # or .Eval
```

Press the new key, confirm the handler runs, press any key to return to the menu, press `q` or `Q` to quit. If you added a Bonus entry, make sure `B` from the main menu still opens the bonus screen and that `Q` from the bonus screen returns to the main menu rather than exiting the whole app.

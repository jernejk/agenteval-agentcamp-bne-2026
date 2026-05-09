// Originally from joslat/AgentEval at samples/ECS2026MAF.Evals/Program.cs.
// Modified for AgentCamp Brisbane 2026: missing-credentials hint points at azd
// and user-secrets, and a Bonus submenu (red-teaming, model comparison) is wired
// in. Eval bodies themselves are placeholders attendees fill in during the
// workshop — see .agent/skills/create-eval-test/SKILL.md.
// Special thanks to Jose Luis Latorre.
//
// SPDX-License-Identifier: MIT
// Copyright (c) 2026 ECS2026 Demo
//
// Run:
//   dotnet run --project AgentEval/samples/ECS2026MAF.Eval

using System.Text;

Console.OutputEncoding = Encoding.UTF8;

await ShowMenuAsync();

static async Task ShowMenuAsync()
{
    while (true)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════════╗
║                ECS 2026 — AgentEval Evaluation Demos                        ║
╠══════════════════════════════════════════════════════════════════════════════╣
║                                                                              ║
║   1  TravelAgent Evals       Behavioral policies + tool assertions           ║
║   2  TripPlanner Evals       Workflow structure + tool-level assertions      ║
║   3  Hypothesis Comparison   Load snapshots — side-by-side (no LLM calls)   ║
║   4  Stochastic Agent        5-run reliability test · single agent           ║
║   5  Stochastic Workflow     5-run reliability test · 4-agent pipeline       ║
║                                                                              ║
║   B  Bonus evals →          Red-teaming · Model comparison                  ║
║   Q  Quit                                                                    ║
║                                                                              ║
╚══════════════════════════════════════════════════════════════════════════════╝
");
        Console.ResetColor();

        if (!Config.IsConfigured)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ⚠️  Azure OpenAI credentials not found.");
            Console.WriteLine("     Run `azd up` from the repo root to provision + write user-secrets,");
            Console.WriteLine("     or set AzureOpenAI:{Endpoint,ApiKey,Deployment} via `dotnet user-secrets set`.\n");
            Console.ResetColor();
        }

        Console.Write("  Select: ");
        var key = Console.ReadKey(intercept: true).KeyChar;
        Console.WriteLine();

        switch (key)
        {
            case '1': await Eval01_TravelAgentEvals.RunAsync();        break;
            case '2': await Eval02_TripPlannerEvals.RunAsync();        break;
            case '3': await Eval03_HypothesisComparison.RunAsync();    break;
            case '4': await Eval04_StochasticAgent.RunAsync();         break;
            case '5': await Eval05_StochasticWorkflow.RunAsync();      break;
            case 'b' or 'B': await ShowBonusMenuAsync();               continue;
            case 'q' or 'Q': return;
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey(intercept: true);
    }
}

static async Task ShowBonusMenuAsync()
{
    while (true)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════════╗
║                    Bonus Evals — extend after the basics                    ║
╠══════════════════════════════════════════════════════════════════════════════╣
║                                                                              ║
║   1  Red-teaming             Adversarial dataset · safety policy            ║
║   2  Model Comparison        Two deployments side-by-side                    ║
║                                                                              ║
║   Q  Back                                                                    ║
║                                                                              ║
╚══════════════════════════════════════════════════════════════════════════════╝
");
        Console.ResetColor();

        Console.Write("  Select: ");
        var key = Console.ReadKey(intercept: true).KeyChar;
        Console.WriteLine();

        switch (key)
        {
            case '1': await BonusEval01_RedTeaming.RunAsync();      break;
            case '2': await BonusEval02_ModelComparison.RunAsync(); break;
            case 'q' or 'Q': return;
            default: continue;
        }

        Console.WriteLine("\nPress any key to return to the bonus menu...");
        Console.ReadKey(intercept: true);
    }
}

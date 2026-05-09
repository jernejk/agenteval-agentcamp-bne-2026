// Originally from joslat/AgentEval at samples/ECS2026MAF/Program.cs.
// Modified for AgentCamp Brisbane 2026: --smoke flag for the cheapest possible
// round-trip against the deployment (used by the setup-project skill).
// Special thanks to Jose Luis Latorre.
//
// SPDX-License-Identifier: MIT
// Copyright (c) 2026 ECS2026 Demo
//
// Run:
//   dotnet run --project AgentEval/samples/ECS2026MAF
//   dotnet run --project AgentEval/samples/ECS2026MAF -- --smoke

using System.Text;
using Azure.AI.OpenAI;
using ECS2026MAF;
using ECS2026MAF.Demos;
using Microsoft.Extensions.AI;
using ChatOptions = Microsoft.Extensions.AI.ChatOptions;

Console.OutputEncoding = Encoding.UTF8;

if (args.Length > 0 && (args[0] == "--smoke" || args[0] == "-s"))
{
    return await RunSmokeTestAsync();
}

await ShowMenuAsync();
return 0;

static async Task<int> RunSmokeTestAsync()
{
    Console.WriteLine("Smoke test — one-shot 'Hi' against the deployment.");
    if (!Config.IsConfigured)
    {
        Console.Error.WriteLine("FAIL: AzureOpenAI credentials missing. Run `azd up` or `dotnet user-secrets set AzureOpenAI:* ...`.");
        return 1;
    }

    Console.WriteLine($"  endpoint   = {Config.Endpoint}");
    Console.WriteLine($"  deployment = {Config.Model}");
    Console.WriteLine();

    try
    {
        var azureClient = new AzureOpenAIClient(Config.Endpoint, Config.KeyCredential);
        var chat        = azureClient.GetChatClient(Config.Model).AsIChatClient();

        var sw       = System.Diagnostics.Stopwatch.StartNew();
        var response = await chat.GetResponseAsync(
            "Reply with a single short greeting.",
            new ChatOptions { MaxOutputTokens = 64 });
        sw.Stop();

        var text = response.Text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            Console.Error.WriteLine("FAIL: deployment returned an empty response.");
            return 1;
        }

        Console.WriteLine($"  reply      = {text}");
        Console.WriteLine($"  latency    = {sw.ElapsedMilliseconds} ms");
        Console.WriteLine();
        Console.WriteLine("Smoke test passed. The model said hi.");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
        Console.Error.WriteLine();
        Console.Error.WriteLine("Common causes:");
        Console.Error.WriteLine("  - Deployment name mismatch (check `dotnet user-secrets list`).");
        Console.Error.WriteLine("  - Region without quota for this model.");
        Console.Error.WriteLine("  - Wrong subscription selected (check `az account show`).");
        return 1;
    }
}

static async Task ShowMenuAsync()
{
    while (true)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════════╗
║                 ECS 2026 — Microsoft Agent Framework Demos                  ║
╠══════════════════════════════════════════════════════════════════════════════╣
║                                                                              ║
║   1  TravelAgent             Single agent with tools                         ║
║   2  TripPlanner Workflow    Multi-agent pipeline (4 agents + tools)         ║
║   3  Live Demo               Placeholder — built live during the talk        ║
║   4  Live Demo (Complete)    Completed reference — chat loop from scratch    ║
║                                                                              ║
║   Q  Quit                                                                    ║
║                                                                              ║
╚══════════════════════════════════════════════════════════════════════════════╝
");
        Console.ResetColor();

        if (!ECS2026MAF.Config.IsConfigured)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ⚠️  Azure OpenAI credentials not found.");
            Console.WriteLine("     Set AZURE_OPENAI_ENDPOINT, AZURE_OPENAI_API_KEY, AZURE_OPENAI_DEPLOYMENT.\n");
            Console.ResetColor();
        }

        Console.Write("  Select: ");
        var key = Console.ReadKey(intercept: true).KeyChar;
        Console.WriteLine();

        switch (key)
        {
            case '1': await Demo01_TravelAgent.RunAsync();             break;
            case '2': await Demo02_TripPlannerWorkflow.RunAsync();     break;
            case '3': await Demo03_LiveDemo.RunAsync();                break;
            case '4': await Demo03_LiveDemoComplete.RunAsync();        break;
            case 'q' or 'Q': return;
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey(intercept: true);
    }
}

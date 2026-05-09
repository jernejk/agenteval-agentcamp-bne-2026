# Attendee instructions

Workshop: **From Vibe-Coded to Prod-Ready: Testing .NET Agents with MAF + AgentEval.**

AgentEval samples by **Jose Luis Latorre** ([@joslat](https://github.com/joslat)) — see [CREDITS.md](../CREDITS.md). The workshop packaging (azd, user-secrets, placeholder evals) is by Jernej Kavka (JK).

## What you need

- A laptop with a terminal (Windows PowerShell, macOS, or Linux all work).
- [.NET 10 SDK](https://dotnet.microsoft.com/download).
- [Azure Developer CLI (`azd`)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd).
- [Azure CLI (`az`)](https://learn.microsoft.com/cli/azure/install-azure-cli).
- An Azure subscription you can deploy to. The provisioned resources tear down with one command at the end.
- ~15 minutes of your life.

If any of those is missing, the [`setup-project` skill](../.agent/skills/setup-project/SKILL.md) (in Claude Code or Codex CLI) will walk you through installing them and the rest of the workshop in one go. Otherwise the steps below are the manual happy path.

## 1. Clone the repo

```bash
git clone https://github.com/jernejk/agenteval-agentcamp-bne-2026.git
cd agenteval-agentcamp-bne-2026
```

You should see these folders:

```text
AgentEval/samples/ECS2026MAF
AgentEval/samples/ECS2026MAF.Eval
docs/
infra/
```

## 2. Sign in to Azure

```bash
azd auth login
```

This opens a browser and prompts you to choose the subscription you want to deploy to. If you have only one, it picks it.

## 3. `azd up`

```bash
azd up
```

`azd up` does three things:

1. Asks you to name the environment (e.g. `agentcamp-bne-2026-yourname`) and pick a location. Defaults to `australiaeast`.
2. Creates a resource group + an Azure OpenAI account + one model deployment (default `gpt-5.4-mini`).
3. Runs the post-provision hook, which fetches your key with `az cognitiveservices account keys list` and writes the three values into `dotnet user-secrets` against `AgentEval/samples/ECS2026MAF`.

Expect ~2-4 minutes for the provisioning step. The hook output ends with:

```text
postprovision: user-secrets written for ECS2026MAF (shared with ECS2026MAF.Eval).
postprovision: endpoint   = https://oai-agentcamp-bne-2026.openai.azure.com/
postprovision: deployment = gpt-5.4-mini
postprovision: tear down with 'azd down --purge' when finished.
```

If the hook fails (e.g. quota error, region mismatch), see [AZURE-FOUNDRY-SETUP.md](AZURE-FOUNDRY-SETUP.md) for the manual fallback. Don't burn 20 minutes silently fighting it — pair with someone who got through, or use the fallback.

## 4. Verify secrets are shared

Both projects read from the same `<UserSecretsId>`, so this should print three keys against either project:

```bash
dotnet user-secrets list --project AgentEval/samples/ECS2026MAF
dotnet user-secrets list --project AgentEval/samples/ECS2026MAF.Eval
```

```text
AzureOpenAI:Endpoint = https://oai-agentcamp-bne-2026.openai.azure.com/
AzureOpenAI:ApiKey = <redacted>
AzureOpenAI:Deployment = gpt-5.4-mini
```

## 5. Build

```bash
dotnet build AgentEval-AgentCampBrisbane2026.slnx
```

```text
Build succeeded.
```

## 6. Run the demo app

```bash
dotnet run --project AgentEval/samples/ECS2026MAF
```

You should see a menu:

```text
1  TravelAgent
2  TripPlanner Workflow
3  Live Demo
4  Live Demo (Complete)

Q  Quit
```

Press `1` for the single-agent demo, then any key to return. Press `2` for the four-agent workflow. Press `q` to quit.

What to watch for: a manual demo can produce a plausible answer, but it doesn't tell you how reliable that answer is. That's where the eval app comes in.

## 7. Run the eval app

```bash
dotnet run --project AgentEval/samples/ECS2026MAF.Eval
```

```text
1  TravelAgent Evals
2  TripPlanner Evals
3  Hypothesis Comparison
4  Stochastic Agent
5  Stochastic Workflow

B  Bonus evals →
Q  Quit
```

**Every eval here is a placeholder.** Press `1` and you'll see:

```text
Eval 01 — TravelAgent (placeholder)
───────────────────────────────────

TODO: replace this stub with the real Eval01 during the workshop.
Exercise: use the demo below as scaffolding for the real RunAsync.
See .agent/skills/create-eval-test/SKILL.md

Helper demo — write/read an EvalSnapshot via EvalResultStore:
  saved   -> .../.AgentEval/ECS2026MAF_Evals/eval01_agent.json
  loaded  -> Eval 01 — TravelAgent (placeholder stub)
  exists  -> True
  age     -> 0 min ago
```

That's the hook the workshop uses: filling in those `RunAsync` bodies with real assertions against the agents. Eval01 already exercises `EvalResultStore` so you can see the persistence layer working before you wire up the eval logic itself.

`B` opens a Bonus submenu (red-teaming, model comparison) — both placeholders, both note exactly what's required to enable them.

## 8. Tear down

When you're done:

```bash
azd down --purge
```

That deletes the resource group and everything in it. The user-secrets stay on your machine harmlessly; clear them if you like:

```bash
dotnet user-secrets clear --project AgentEval/samples/ECS2026MAF
```

## If `azd` doesn't work

`azd up` can fail because of quota, region, subscription policy, or local CLI version. See [AZURE-FOUNDRY-SETUP.md](AZURE-FOUNDRY-SETUP.md) for two fallback paths: pointing at an existing Azure OpenAI resource you already have, or provisioning manually with `az cognitiveservices`.

## Takeaway

Before shipping an agent, the questions worth asking:

- What did we evaluate?
- What did we miss?
- Did the expected tools run?
- How much did scores vary across runs?
- What's the cost and latency?
- Which evals belong in CI?
- What would block the PR?

Manual testing tells you what happened once. Evals tell you what keeps happening.

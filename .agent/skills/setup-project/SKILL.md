---
name: setup-project
description: Bootstrap the AgentCamp Brisbane 2026 workshop sample on a fresh clone — Azure account check, prereq install prompts, build/test smoke run, infra Q&A, `azd up`, post-provision smoke test, harness-aware tail. Use on the first run after cloning.
---

You are the setup on-ramp for **AgentEval-AgentCampBrisbane2026**. The attendee just cloned the repo and wants to go from zero to "the model said hi" without reading three docs.

Use the host harness's native question UI (Claude Code: `AskUserQuestion`; Codex: interactive prompt). Don't shell-prompt the attendee — every choice is a clickable option. After the last step, celebrate.

## Step 1 — Greeting and scope

In one paragraph: tell them you'll do roughly 60 seconds of prereq checks, then 3-4 minutes of provisioning, then a smoke test. Reassure: everything tears down with `azd down --purge`. Read [CREDITS.md](../../../CREDITS.md) and credit Jose Luis Latorre / joslat/AgentEval upstream once.

**Show the active subscription before doing anything else.** Run:

```bash
az account show --query "{name:name, id:id, user:user.name}" -o table
```

State the subscription name and ID back to the user explicitly: "I'm about to provision into **&lt;name&gt;** (`&lt;id&gt;`)." If they look surprised, ask whether to switch:

> Use a different subscription? List with `az account list -o table` and switch with `az account set --subscription &lt;name-or-id&gt;`.

Don't proceed to step 2 until the user has acknowledged the subscription. Anything provisioned in the wrong subscription wastes their cleanup time.

## Step 2 — Q1: Azure account

Recommended option first.

| Option | What it means |
| --- | --- |
| **Yes** | Proceed to step 3. |
| **No, help me out** | Point at <https://azure.microsoft.com/free/> and the Azure CLI sign-up flow; offer to wait while they create one. Loop back to Q1 when ready. |
| **No, I want OpenAI / another provider** | Stop. Tell them v1 of this sample is wired to Azure OpenAI; using OpenAI directly or another provider requires editing `Config.cs` and the agent factories. Point at upstream `joslat/AgentEval` for the cross-provider patterns. End the skill here. |

## Step 3 — Prereq checks

Run the four commands and report status with a tick/cross. Don't run them in parallel — race conditions are confusing for the user.

```bash
dotnet --list-sdks    # need a 10.x line
azd version
az version
git --version
```

For each missing tool, ask:

> Install **{tool}** now?
> - Yes — use the platform default (`winget`, `brew`, or `apt`).
> - No, I'll handle it.

Platform installer commands:

- macOS: `brew install azure-cli azd dotnet`
- Windows: `winget install Microsoft.AzureCLI`, `winget install Microsoft.Azd`, `winget install Microsoft.DotNet.SDK.10`
- Ubuntu/Debian: `curl -fsSL https://aka.ms/install-azd.sh | bash` for azd; the rest via apt or Microsoft's package feeds.

If the user picks "I'll handle it", pause. Ask them to come back when ready.

## Step 4 — Build and placeholder run

```bash
dotnet build AgentEval-AgentCampBrisbane2026.slnx
```

Expect "Build succeeded." If it fails, stop and surface the error — common cause is the SDK version not being net10.

## Step 5 — Q2: Resource group name

Default name: `rg-agentcamp-bne-2026`. **Before proposing it, check whether it already exists in the active subscription:**

```bash
az group exists --name rg-agentcamp-bne-2026
```

### If it returns `false`

Propose `rg-agentcamp-bne-2026` as the default and accept any free-text override.

### If it returns `true`

Inspect what's already in there before asking:

```bash
az resource list \
  --resource-group rg-agentcamp-bne-2026 \
  --query "[].{name:name, type:type, location:location}" \
  -o table
```

Then ask the user, listing exactly what you saw:

> A resource group named **`rg-agentcamp-bne-2026`** already exists with these resources:
>
> &lt;table from above&gt;
>
> What do you want to do?
>
> 1. **Reuse it** — point this workshop at the existing OpenAI account/deployment (recommended if everything below is present).
> 2. **Use a fresh group** — append a six-char random suffix and provision into the new one.
> 3. **Use a different name entirely** — type your own.

For option 1, run a quick health check before continuing. The reused RG must contain:

- One `Microsoft.CognitiveServices/accounts` (kind `OpenAI`).
- At least one deployment under it.

```bash
RG=rg-agentcamp-bne-2026
ACCOUNT=$(az cognitiveservices account list -g "$RG" --query "[?kind=='OpenAI'].name | [0]" -o tsv)
if [ -z "$ACCOUNT" ]; then echo "no Azure OpenAI account in $RG"; fi

az cognitiveservices account deployment list \
  --name "$ACCOUNT" --resource-group "$RG" \
  --query "[].{name:name, model:properties.model.name, version:properties.model.version, sku:sku.name}" \
  -o table
```

If the OpenAI account or a deployment is missing, switch to option 2 (fresh group with suffix) — partial reuse is more pain than it's worth.

If reuse is healthy, **skip steps 6 and 7** and instead write user-secrets directly:

```bash
ENDPOINT=$(az cognitiveservices account show --name "$ACCOUNT" --resource-group "$RG" --query properties.endpoint -o tsv)
DEPLOY=$(az cognitiveservices account deployment list --name "$ACCOUNT" --resource-group "$RG" --query "[0].name" -o tsv)
KEY=$(az cognitiveservices account keys list --name "$ACCOUNT" --resource-group "$RG" --query key1 -o tsv)

dotnet user-secrets set 'AzureOpenAI:Endpoint'   "$ENDPOINT" --project AgentEval/samples/ECS2026MAF
dotnet user-secrets set 'AzureOpenAI:ApiKey'     "$KEY"      --project AgentEval/samples/ECS2026MAF
dotnet user-secrets set 'AzureOpenAI:Deployment' "$DEPLOY"   --project AgentEval/samples/ECS2026MAF
```

Then jump straight to step 9 (smoke test).

## Step 6 — Q3: Region

Recommended first.

| Option | Notes |
| --- | --- |
| **`australiaeast`** (Recommended) | Workshop default. Lowest latency for Brisbane. |
| **`eastus2`** | Use if Australia East quota is full or you want US alternatives (Kimi, DeepSeek). |
| **Other (type your own)** | Warn that you can't pre-validate model availability outside the two known regions. |

## Step 7 — Q4: Primary model

Recommended first.

| Option | Notes |
| --- | --- |
| **`gpt-5.4-mini`** (Recommended) | Workshop default. Cheap and fast. |
| **`gpt-5.4`** | Standard SKU if mini is exhausted. |
| **Other** | Point at [docs/MODEL-OPTIONS.md](../../../docs/MODEL-OPTIONS.md). User picks model name + version from the Foundry catalogue. |

## Step 8 — Confirmation

Echo all chosen values back, **including the subscription name and ID** from step 1 — that's what's actually getting charged. Remind: `azd down --purge` deletes everything when done. Then run:

```bash
azd auth login                                                 # only if `azd auth show` reports no session
azd env new agentcamp-bne-2026-${RANDOM_SUFFIX}
azd env set AZURE_LOCATION             ${region}
azd env set AZURE_RESOURCE_GROUP_NAME   ${rg}
azd env set AZURE_OPENAI_RESOURCE_NAME  oai-${suffix}
azd env set AZURE_OPENAI_DEPLOYMENT     ${model}
azd env set AZURE_OPENAI_MODEL_NAME     ${model}
azd env set AZURE_OPENAI_MODEL_VERSION  ${version}             # query first via az cognitiveservices account list-models
azd up
```

Watch for the post-provision hook output: "user-secrets written for ECS2026MAF (shared with ECS2026MAF.Eval)." If that line is missing, something's wrong with the hook — surface the full hook output.

## Step 9 — Smoke test

Cheapest possible round-trip: send `"Hi"` with `max_tokens=8`. Cost is well under a cent.

The .NET sample exposes a `--smoke` flag for this. If that flag isn't wired yet, write a one-line dotnet-script call inline:

```bash
dotnet run --project AgentEval/samples/ECS2026MAF -- --smoke
```

Expect a short reply within 2 seconds. If it errors, run:

```bash
dotnet user-secrets list --project AgentEval/samples/ECS2026MAF
azd env get-values
```

…and ask the user whether to open [docs/AZURE-FOUNDRY-SETUP.md](../../../docs/AZURE-FOUNDRY-SETUP.md).

## Step 10 — Harness-aware tail

- **Claude Code**: "Your `.claude/settings.json` has the dotnet-format hook and the destructive-command guard wired. Open the slash menu to find the other skills (`create-eval-test`, `add-console-prompt`, `run-evals-and-graph`)."
- **Codex CLI**: "[AGENTS.md](../../../AGENTS.md) has the skills index. The same workflows are mirrored as plain prompts under [.agent/prompts/](../../../.agent/prompts/)."
- **Other harness**: "[AGENTS.md](../../../AGENTS.md) is the canonical source of truth — start there."

## Step 11 — Celebrate

Print one line and stop:

```text
Setup complete. The model said hi. 🥳
```

This is the only emoji in the repo; it's scoped to terminal output only. Do not put it in any source file or doc.

## Failure modes you may hit

- **Quota exhausted in `australiaeast`** — `azd env set AZURE_LOCATION eastus2`, `azd provision` again. Hook re-runs.
- **Model name not deployable** — list the catalogue, pick a different version, repeat.
- **`azd up` succeeds but hook fails** — re-run just the hook: `bash infra/hooks/postprovision.sh` (or the .ps1 on Windows).
- **Wrong subscription selected** — `az account set --subscription <id>`, `azd up` again.

Keep the failure recovery short. Don't burn ten minutes troubleshooting; bail to [docs/AZURE-FOUNDRY-SETUP.md](../../../docs/AZURE-FOUNDRY-SETUP.md) if `azd` keeps fighting back.

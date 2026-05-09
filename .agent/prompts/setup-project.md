# Prompt — Setup the workshop sample

Use this prompt when starting from a fresh clone of `agenteval-agentcamp-bne-2026`. Mirror of [`.agent/skills/setup-project/SKILL.md`](../skills/setup-project/SKILL.md) for harnesses (e.g. Codex CLI) that don't load Claude-style skills.

You are walking the user from "I just cloned the repo" to "the model said hi", with three goals:

1. Confirm an Azure account and the four prereq tools (`dotnet 10`, `azd`, `az`, `git`).
2. Provision Azure OpenAI via `azd up` and write user-secrets via the post-provision hook.
3. Run a one-shot smoke test (`"Hi"` with `max_tokens=8`) to confirm the loop works.

Walk it linearly. Ask one question at a time, recommend the safe option first, and never run `azd down` without explicit confirmation.

## Show the subscription up front

Before any questions, run `az account show --query "{name:name, id:id, tenantId:tenantId, user:user.name}" -o table` and state back to the user "I'm about to provision into **&lt;name&gt;** (`&lt;id&gt;`) in tenant `&lt;tenantId&gt;`." Get explicit acknowledgement before continuing. Anything provisioned in the wrong subscription wastes their cleanup time.

If they want to switch: `az account list -o table` then `az account set --subscription <name-or-id>`.

**Cross-tenant check.** `azd auth login` defaults to the user's home tenant, which often doesn't match the tenant the chosen subscription lives in (Microsoft staff, MVPs, multi-tenant consultants). Catch it now before `azd up` blows up later with `failed to resolve user access to subscription`:

```bash
SUB_TENANT=$(az account show --query tenantId -o tsv)
AZD_TENANT=$(azd auth show --output json 2>/dev/null | jq -r '.account.tenantId // empty')
[ "$SUB_TENANT" = "$AZD_TENANT" ] || azd auth login --tenant-id "$SUB_TENANT"
```

## Questions to ask

1. **Do you have an Azure account?**
   - Yes → continue.
   - No, help me out → point at <https://azure.microsoft.com/free/>, wait, loop.
   - I want OpenAI / another provider → stop. Tell them v1 is wired to Azure OpenAI; using OpenAI directly requires editing `Config.cs` and the agent factories. Point at `joslat/AgentEval` upstream for the cross-provider patterns.

2. **Resource group name?** Default: `rg-agentcamp-bne-2026`. **First check whether it already exists:**

   ```bash
   az group exists --name rg-agentcamp-bne-2026
   ```

   - `false` → propose the default, accept any free-text override.
   - `true` → list what's already inside (`az resource list -g rg-agentcamp-bne-2026 -o table`). Ask:
     1. **Reuse it** — point this workshop at the existing OpenAI account/deployment.
     2. **Use a fresh group** — append a six-char random suffix.
     3. **Use a different name entirely** — type your own.

   For option 1, verify the RG actually has what we need:

   ```bash
   ACCOUNT=$(az cognitiveservices account list -g rg-agentcamp-bne-2026 --query "[?kind=='OpenAI'].name | [0]" -o tsv)
   az cognitiveservices account deployment list --name "$ACCOUNT" --resource-group rg-agentcamp-bne-2026 -o table
   ```

   If the OpenAI account is missing or has no deployments, fall back to option 2 — partial reuse is more pain than it's worth.

   If the reused RG is healthy, **skip the region/model questions** and write user-secrets directly:

   ```bash
   ENDPOINT=$(az cognitiveservices account show --name "$ACCOUNT" --resource-group rg-agentcamp-bne-2026 --query properties.endpoint -o tsv)
   DEPLOY=$(az cognitiveservices account deployment list --name "$ACCOUNT" --resource-group rg-agentcamp-bne-2026 --query "[0].name" -o tsv)
   KEY=$(az cognitiveservices account keys list --name "$ACCOUNT" --resource-group rg-agentcamp-bne-2026 --query key1 -o tsv)
   dotnet user-secrets set 'AzureOpenAI:Endpoint'   "$ENDPOINT" --project AgentEval/samples/ECS2026MAF
   dotnet user-secrets set 'AzureOpenAI:ApiKey'     "$KEY"      --project AgentEval/samples/ECS2026MAF
   dotnet user-secrets set 'AzureOpenAI:Deployment' "$DEPLOY"   --project AgentEval/samples/ECS2026MAF
   ```

   Then jump straight to the smoke test.

3. **Region?** Default `australiaeast`. Alternative `eastus2`. Anything else: warn you can't pre-validate model availability.

4. **Primary model?** Default `gpt-5.4-mini`. Alternative `gpt-5.4`. Other: open `docs/MODEL-OPTIONS.md`.

## Before `azd up`

Run prereq checks. If any are missing, offer the platform installer command (`brew install …` / `winget install …` / `apt …`). Pause if the user wants to install themselves.

Build the placeholders to confirm the toolchain works:

```bash
dotnet build AgentEval-AgentCampBrisbane2026.slnx
```

## Provisioning (only if not reusing an existing RG)

Echo back the subscription name + ID one more time before launching `azd up`. Then:

```bash
azd auth login                                 # only if `azd auth show` reports no session
azd env new agentcamp-bne-2026-{suffix}
azd env set AZURE_LOCATION             {region}
azd env set AZURE_RESOURCE_GROUP_NAME   {rg}
azd env set AZURE_OPENAI_RESOURCE_NAME  oai-{suffix}
azd env set AZURE_OPENAI_DEPLOYMENT     {model}
azd env set AZURE_OPENAI_MODEL_NAME     {model}
azd env set AZURE_OPENAI_MODEL_VERSION  {version}   # query first: az cognitiveservices account list-models
azd up
```

The post-provision hook output should end with `postprovision: user-secrets written for ECS2026MAF (shared with ECS2026MAF.Eval).`

## Smoke test

```bash
dotnet run --project AgentEval/samples/ECS2026MAF -- --smoke
```

Expect a short reply within ~2 seconds. If the smoke test errors:

```bash
dotnet user-secrets list --project AgentEval/samples/ECS2026MAF
azd env get-values
```

…and ask whether to open `docs/AZURE-FOUNDRY-SETUP.md`.

## End the prompt with

```text
Setup complete. The model said hi. 🥳
```

That's the only emoji in the repo and it's only ever in terminal output. Don't put it in source or docs.

## Tear-down

When the user is done with the workshop:

```bash
azd down --purge
```

Confirm before running it.

# Prompt — Setup the workshop sample

Use this prompt when starting from a fresh clone of `agenteval-agentcamp-bne-2026`. Mirror of [`.agent/skills/setup-project/SKILL.md`](../skills/setup-project/SKILL.md) for harnesses (e.g. Codex CLI) that don't load Claude-style skills.

You are walking the user from "I just cloned the repo" to "the model said hi", with three goals:

1. Confirm an Azure account and the four prereq tools (`dotnet 10`, `azd`, `az`, `git`).
2. Provision Azure OpenAI via `azd up` and write user-secrets via the post-provision hook.
3. Run a one-shot smoke test (`"Hi"` with `max_tokens=8`) to confirm the loop works.

Walk it linearly. Ask one question at a time, recommend the safe option first, and never run `azd down` without explicit confirmation.

## Questions to ask

1. **Do you have an Azure account?**
   - Yes → continue.
   - No, help me out → point at <https://azure.microsoft.com/free/>, wait, loop.
   - I want OpenAI / another provider → stop. Tell them v1 is wired to Azure OpenAI; using OpenAI directly requires editing `Config.cs` and the agent factories. Point at `joslat/AgentEval` upstream for the cross-provider patterns.

2. **Resource group name?** Default: `rg-agentcamp-bne-2026-{6-random-chars}`.

3. **Region?** Default `australiaeast`. Alternative `eastus2`. Anything else: warn you can't pre-validate model availability.

4. **Primary model?** Default `gpt-5.4-mini`. Alternative `gpt-5.4`. Other: open `docs/MODEL-OPTIONS.md`.

## Before `azd up`

Run prereq checks. If any are missing, offer the platform installer command (`brew install …` / `winget install …` / `apt …`). Pause if the user wants to install themselves.

Build the placeholders to confirm the toolchain works:

```bash
dotnet build AgentEval-AgentCampBrisbane2026.slnx
```

## Provisioning

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

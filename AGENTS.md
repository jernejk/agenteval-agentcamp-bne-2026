# AGENTS.md

Guide for AI coding agents (Claude Code, Codex CLI, Cursor, others) working in this repo.

## What this repo is

A workshop sample for **AgentCamp Brisbane 2026**: From Vibe-Coded to Prod-Ready: Testing .NET Agents with MAF + AgentEval.

The .NET sources under `AgentEval/samples/` are derived from Jose Luis Latorre's [joslat/AgentEval](https://github.com/joslat/AgentEval). Special thanks to Jose. See [CREDITS.md](CREDITS.md) and [LICENSE](LICENSE).

The workshop additions are: an `azd` template that provisions Azure OpenAI, a shared `<UserSecretsId>` so demo + eval projects share secrets, placeholder evaluation bodies attendees fill in, a Bonus submenu (red-teaming, model comparison), and the docs / skills you're reading right now.

## Setup

The only setup command is `azd up` (after `azd auth login`). Do not introduce env-var exports, `appsettings.json` files, or `.env` shims into the happy path. Env-var fallback exists in `Config.cs` for CI; that's its only intended audience.

If an attendee is starting from zero, point them at the `setup-project` skill rather than walking them through prereqs by hand.

## Secrets

Both `AgentEval/samples/ECS2026MAF/ECS2026MAF.csproj` and `AgentEval/samples/ECS2026MAF.Eval/ECS2026MAF.Eval.csproj` set the same `<UserSecretsId>agentcamp-bne-2026-ecs2026maf</UserSecretsId>`. That's how setting `AzureOpenAI:Endpoint` against one project shows up in the other.

- Read secrets only via `ECS2026MAF.Config` (in `AgentEval/samples/ECS2026MAF/Config.cs`).
- Never paste a key into source, docs, screenshots, scrollbacks, or commit messages.
- Never commit `.azure/` (azd local env state — already in `.gitignore`).
- Never write user-secrets into a chart, snapshot, or eval result file.

## Code style

- Match `.editorconfig`. `dotnet format` is wired into the Claude Code `PostToolUse` hook (`.claude/settings.json`) and runs on every C# edit.
- No emojis in source files. Skill terminal output may use one celebratory emoji where called out (e.g. the `setup-project` skill's success line). Docs stay emoji-free.
- File-scoped namespaces, `var` when the type is apparent, expression-bodied members where they fit on one line.
- Don't widen scope. Bug fixes don't need surrounding cleanup, console placeholders don't need wider abstractions.

## Skills index

Canonical files live under `.agent/skills/`. Claude Code reads them via the symlink at `.claude/skills`. Codex CLI reads the markdown mirrors at `.agent/prompts/`.

| Skill | When to use |
| --- | --- |
| [setup-project](.agent/skills/setup-project/SKILL.md) | First-time setup. Walks through Azure account check, prereqs, build, infra Q&A, smoke test. Use this on a fresh clone. |
| [create-eval-test](.agent/skills/create-eval-test/SKILL.md) | Adding or replacing an eval body. Use when an attendee asks "how do I implement Eval02?" or wants to add Eval06. |
| [add-console-prompt](.agent/skills/add-console-prompt/SKILL.md) | Adding a new menu option to either console (`ECS2026MAF/Program.cs` or `ECS2026MAF.Eval/Program.cs`). |
| [run-evals-and-graph](.agent/skills/run-evals-and-graph/SKILL.md) | Running evals, exporting JSON snapshots, rendering charts of stochastic results. |

## Don'ts

- Don't run `azd down` without the user confirming. The Claude `PreToolUse` hook blocks `azd*down*`, `az*group*delete*`, and `rm -rf *`; bypassing it requires explicit user approval.
- Don't commit `.azure/`, `bin/`, `obj/`, `.AgentEval/charts/`, or anything matching `*.user`.
- Don't add unrelated Azure resources to `infra/main.bicep`. Keep it: resource group + cognitive-services account + one model deployment.
- Don't rename `ECS2026MAF.Evals` namespace in source — folder/csproj are `.Eval`, the namespace stays `.Evals` to match upstream.
- Don't pin `AZURE_OPENAI_MODEL_VERSION` in `infra/main.parameters.json`. Versions roll. Take it from the Foundry catalogue at provisioning time.

## Codex CLI parity

Codex doesn't have a project-hooks system equivalent to Claude Code's `.claude/settings.json`. The destructive-command guard and the `dotnet format` autorun are Claude-only. Skills work through `.agent/prompts/` mirrors that Codex picks up via this AGENTS.md (or `agents.md` if symlinked further). No `.codex/` directory is necessary.

## Useful commands

```bash
azd auth login
azd up
azd env get-values
azd down --purge

dotnet build AgentEval-AgentCampBrisbane2026.slnx
dotnet run --project AgentEval/samples/ECS2026MAF
dotnet run --project AgentEval/samples/ECS2026MAF.Eval

dotnet user-secrets list  --project AgentEval/samples/ECS2026MAF
dotnet user-secrets list  --project AgentEval/samples/ECS2026MAF.Eval
dotnet user-secrets clear --project AgentEval/samples/ECS2026MAF
```

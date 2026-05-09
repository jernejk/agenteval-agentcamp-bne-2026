# AgentEval — AgentCamp Brisbane 2026

Workshop sample: **From Vibe-Coded to Prod-Ready: Testing .NET Agents with MAF + AgentEval.**

Originally based on [joslat/AgentEval](https://github.com/joslat/AgentEval) — special thanks to **Jose Luis Latorre** ([@joslat](https://github.com/joslat)). See [CREDITS.md](CREDITS.md) for the full attribution and [LICENSE](LICENSE) for the MIT terms.

## Quick start

```bash
git clone https://github.com/jernejk/agenteval-agentcamp-bne-2026.git
cd agenteval-agentcamp-bne-2026

azd auth login
azd up

dotnet build AgentEval-AgentCampBrisbane2026.slnx
dotnet run --project AgentEval/samples/ECS2026MAF
dotnet run --project AgentEval/samples/ECS2026MAF.Eval

azd down --purge   # when finished
```

`azd up` provisions Azure OpenAI in `australiaeast` and writes `dotnet user-secrets` to both .NET projects. No env-var exports, no manual config, no shell-history secrets. The eval app starts as placeholders — filling them in is the workshop.

## Where to look

- [docs/ATTENDEE-INSTRUCTIONS.md](docs/ATTENDEE-INSTRUCTIONS.md) — full attendee path with cross-platform commands.
- [docs/AZURE-FOUNDRY-SETUP.md](docs/AZURE-FOUNDRY-SETUP.md) — optional fallback if `azd up` cannot run, or if you have an existing resource.
- [docs/MODEL-OPTIONS.md](docs/MODEL-OPTIONS.md) — region/model picks (`australiaeast` vs `eastus2`, alternatives).
- [docs/SPEAKER-INSTRUCTIONS.md](docs/SPEAKER-INSTRUCTIONS.md) — speaker runbook.
- [.agent/skills/](.agent/skills/) — Claude Code / Codex CLI skills: `setup-project`, `create-eval-test`, `add-console-prompt`, `run-evals-and-graph`. Mirrored as plain prompts at [.agent/prompts/](.agent/prompts/).
- [AGENTS.md](AGENTS.md) (alias [CLAUDE.md](CLAUDE.md)) — agent rules for this repo.

## Layout

```text
.
├── AgentEval/samples/
│   ├── ECS2026MAF/        # MAF demo (single agent + four-agent workflow)
│   └── ECS2026MAF.Eval/   # AgentEval eval harness, bodies are placeholders
├── infra/                  # azd template: bicep + post-provision hook
├── docs/                   # attendee, speaker, setup, model docs
├── .agent/                 # cross-tool skills + prompts
└── .claude/                # Claude Code hooks (PostToolUse format, PreToolUse guard)
```

## License

MIT. Copyright (c) 2026 Jose Luis Latorre Millas (upstream samples) and Jernej Kavka (JK) (workshop additions). See [LICENSE](LICENSE).

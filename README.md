# AgentEval — AgentCamp Brisbane 2026

Workshop sample: **From Vibe-Coded to Prod-Ready: Testing .NET Agents with MAF + AgentEval.**

Originally based on [joslat/AgentEval](https://github.com/joslat/AgentEval) — special thanks to **Jose Luis Latorre**. See [CREDITS.md](CREDITS.md) for the full attribution and [LICENSE](LICENSE) for the MIT terms.

> Repository scaffold in progress. Sources, infra, and docs land in subsequent commits.

## Quick start

```bash
azd auth login
azd up
dotnet build
dotnet run --project AgentEval/samples/ECS2026MAF
```

Once those commits land, see `docs/ATTENDEE-INSTRUCTIONS.md` for the full attendee path and `docs/AZURE-FOUNDRY-SETUP.md` for the optional manual fallback.

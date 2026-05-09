# Credits

The .NET samples in `AgentEval/samples/ECS2026MAF/` and `AgentEval/samples/ECS2026MAF.Eval/`
are derived from Jose Luis Latorre's open-source AgentEval project.

Special thanks to **Jose Luis Latorre** ([@joslat](https://github.com/joslat)) for the
underlying AgentEval framework, the original ECS2026MAF samples, and the evaluation
patterns this workshop builds on. None of this exists without his work upstream.

- Upstream repository: <https://github.com/joslat/AgentEval>
- Upstream license: MIT (preserved in [LICENSE](LICENSE))
- Upstream copyright: Copyright (c) 2026 Jose Luis Latorre Millas

This repo packages those samples for AgentCamp Brisbane 2026 with:

- An `azd up` happy-path that provisions Azure OpenAI and writes `dotnet user-secrets` automatically.
- A shared `<UserSecretsId>` between the demo app and the eval project, so secrets land in one place.
- Placeholder eval bodies attendees fill in during the workshop, plus a Bonus submenu (red-teaming, model comparison) for follow-up.
- Cross-platform attendee, speaker, and setup docs.
- Agent tooling (`AGENTS.md`, `.agent/skills/`) so Claude Code, Codex CLI, and similar agents pick up the same on-ramp.

Any bugs in this packaging are ours, not Jose's. If you find one upstream, please file it at <https://github.com/joslat/AgentEval/issues>; if you find one in the workshop packaging, file it on this repo.

— Jernej Kavka (JK)

# Speaker instructions

Private runbook for the AgentCamp Brisbane 2026 workshop. Don't share this verbatim with attendees — they get [ATTENDEE-INSTRUCTIONS.md](ATTENDEE-INSTRUCTIONS.md), which is the cleaner happy path.

The samples come from Jose Luis Latorre's [joslat/AgentEval](https://github.com/joslat/AgentEval). When introducing the workshop, credit that upstream first — see [CREDITS.md](../CREDITS.md).

## Goals

1. Show that an MAF agent can be measured, compared, and improved without relying on demo vibes.
2. Give attendees a working repo where the eval scaffolding is in place and the only thing they fill in is the *interesting* part — the assertions.
3. End the session with a real example of a placeholder eval becoming a real eval, ideally pasted in live.

## Pre-event checklist (24h out)

- [ ] Confirm `gpt-5.4-mini` is currently deployable in `australiaeast`. Run:
  ```bash
  az cognitiveservices account list-models \
    --name <a-test-resource> --resource-group <its-rg> \
    --query "[?name=='gpt-5.4-mini'].{name:name,version:version,sku:skus[0].name}" \
    -o table
  ```
  If it's gone or the SKU is exhausted, swap the default to `eastus2` in `infra/main.parameters.json` and re-test. Default model name does not change.
- [ ] Run `azd up` end-to-end on a clean machine. Time it. Target ≤ 4 minutes from `azd up` to "user-secrets written".
- [ ] Run `dotnet build` and start both consoles. Walk every menu option and confirm placeholders return cleanly.
- [ ] Walk the Bonus submenu (`B`). Confirm both bonus placeholders print prerequisites and return.
- [ ] Run `azd down --purge` and verify the resource group is gone.
- [ ] Re-clone the repo from scratch and re-do the above. The second pass must be as smooth as the first.

## 90-minute room flow

| Minute | Segment | Notes |
| --- | --- | --- |
| 0–10 | Welcome + framing | Why "vibe coded" agents fail in prod. Credit Jose Luis Latorre / AgentEval upstream. |
| 10–20 | Live `azd up` on the speaker laptop | Show the post-provision hook output. Land "secrets are shared between two projects via one UserSecretsId." |
| 20–35 | Demo01 + Demo02 | Single agent vs four-agent workflow. The single agent might miss a leg or skip confirmation — that's the hook for the eval pitch. |
| 35–45 | Walk the eval menu (placeholders) | Press `1`, show the helper-API output. Show the Bonus submenu. The room understands what "filling these in" means. |
| 45–75 | Live coding: turn one placeholder into a real eval | Use `.agent/skills/create-eval-test/SKILL.md` as the template — paste in the real Eval01 from upstream and run it. Buffer here is intentional. |
| 75–85 | Stochastic + comparison segment | If time, run a quick 5x stochastic eval. Score spread is the talking point. |
| 85–90 | Wrap, Q&A, point at Bonus evals | Red-teaming and model comparison are exactly the kind of "next step" that lives well as a follow-up. |

## Speaker notes

- "Our agent passes the demo. That's not the same as 'our agent passes'."
- "If you see only the score, you're missing the criteria. The eval app prints both."
- "Single agent won this run. Run it again — does it still win?"
- "The placeholder bodies are not laziness. They're the actual workshop. Everything around them is plumbing you should never have to write."

## Things that have gone wrong before

- **Quota exhausted in `australiaeast`:** swap `AZURE_LOCATION` to `eastus2` via `azd env set AZURE_LOCATION eastus2` then `azd provision`. The hook re-runs after.
- **`azd` CLI too old:** `winget upgrade Microsoft.Azd` / `brew upgrade azd`. Anything below 1.7 has bugs around hook env-var resolution.
- **Wi-Fi flaky during `azd up`:** keep a screen recording of a successful run on hand and narrate over it; don't try to debug Azure live.
- **Attendee asks where the actual eval logic is:** point at upstream `joslat/AgentEval` and `.agent/skills/create-eval-test/SKILL.md`.

## Don'ts

- Don't show your API key on stage. The post-provision hook keeps it in user-secrets, not in the terminal scrollback. Keep `dotnet user-secrets list` off the projector.
- Don't `azd down` mid-segment to demonstrate teardown. The room won't appreciate the four-minute pause.
- Don't add new Azure resources to `infra/main.bicep` during the demo. Keep it boring — the boring infra is the point.

## Cleanup the day after

```bash
azd down --purge
dotnet user-secrets clear --project AgentEval/samples/ECS2026MAF
```

Then archive any captured terminal recordings into your speaker notes for next year.

# Model and region options

Workshop default: `gpt-5.4-mini` in `australiaeast`. That's what `azd up` provisions if you accept the defaults. This doc covers when and how to deviate.

## Default at a glance

| Setting | Default | Where set |
| --- | --- | --- |
| Region | `australiaeast` | `infra/main.parameters.json` (`location`) |
| Model name | `gpt-5.4-mini` | `infra/main.parameters.json` (`modelName`) |
| Deployment name | `gpt-5.4-mini` | `infra/main.parameters.json` (`deploymentName`) |
| SKU | `GlobalStandard` | `infra/main.parameters.json` (`skuName`) |
| Capacity | `1` | `infra/main.parameters.json` (`skuCapacity`) |
| Model version | (none — pick one) | `AZURE_OPENAI_MODEL_VERSION` env var |

`modelVersion` is intentionally not defaulted: published versions roll forward and we don't want to pin to a string that disappears. Pick a version with `az cognitiveservices account list-models` (see below) and set it via `azd env set AZURE_OPENAI_MODEL_VERSION <value>` before `azd provision`.

## Region picks

| Region | Default model | When to use | Strong alternatives |
| --- | --- | --- | --- |
| `australiaeast` | `gpt-5.4-mini` | Workshop default — minimum latency for the Brisbane crowd. | `gpt-5.4` standard SKU if the mini quota is exhausted. |
| `eastus2` | `gpt-5.4-mini` | Australia East quota is full, or you want a cross-region comparison run. | Kimi K2.5 / K2.6 (when listed in the Foundry catalogue), DeepSeek v4. Useful for the Bonus model-comparison eval. |

Always check the live catalogue before betting on availability:

```bash
az cognitiveservices account list-models \
  --name <your-resource> \
  --resource-group <your-rg> \
  --query "[?name=='gpt-5.4-mini'].{name:name,version:version,sku:skus[0].name,format:format}" \
  -o table
```

If that returns empty for your region, either pick a different model or pick a different region.

## Plugging in a non-default model

Before `azd provision` (whether for the first time or to re-roll the deployment):

```bash
azd env set AZURE_LOCATION eastus2
azd env set AZURE_OPENAI_RESOURCE_NAME oai-agentcamp-bne-2026-eus2
azd env set AZURE_OPENAI_DEPLOYMENT my-deepseek-v4
azd env set AZURE_OPENAI_MODEL_NAME deepseek-v4
azd env set AZURE_OPENAI_MODEL_VERSION 2026-04-01
azd provision
```

After it lands, the post-provision hook writes the new endpoint/deployment/key into user-secrets and the .NET sample picks them up on the next run.

## Bonus model-comparison wiring

The placeholder Bonus eval (`AgentEval/samples/ECS2026MAF.Eval/Evals/Bonus/BonusEval02_ModelComparison.cs`) reads two deployment names: `AzureOpenAI:Deployment` (primary, from `azd up`) and `AzureOpenAI:DeploymentSecondary` (the second one you want to compare against).

To wire a secondary deployment in:

```bash
# Provision a second deployment in the same resource (or a different one).
az cognitiveservices account deployment create \
  --name "$(azd env get-value AZURE_OPENAI_RESOURCE_NAME)" \
  --resource-group "$(azd env get-value AZURE_RESOURCE_GROUP_NAME)" \
  --deployment-name "kimi-k2-6" \
  --model-name "kimi-k2.6" \
  --model-version "<from-list-models>" \
  --model-format OpenAI \
  --sku-name GlobalStandard \
  --sku-capacity 1

# Tell the sample about it.
dotnet user-secrets set "AzureOpenAI:DeploymentSecondary" "kimi-k2-6" \
  --project AgentEval/samples/ECS2026MAF
```

Then fill in `BonusEval02_ModelComparison.RunAsync` to spin up two clients (one per deployment), run the same task against each, and compare scores/tools/latency. The skill at `.agent/skills/create-eval-test/SKILL.md` covers the assertion pattern.

## Reminder

Model names in this doc are starting hints. The Foundry catalogue evolves; treat the list-models output as the source of truth on the day of the workshop.

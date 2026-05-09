# Azure AI Foundry setup (optional fallback)

> **Optional.** Most attendees should run `azd up` from [ATTENDEE-INSTRUCTIONS.md](ATTENDEE-INSTRUCTIONS.md). Use this doc only if `azd up` cannot run (no `azd` CLI, restricted subscription policy) or you want to point the sample at an Azure OpenAI resource you already have.

The sample uses `Azure.AI.OpenAI.AzureOpenAIClient`, which expects three values:

- `AzureOpenAI:Endpoint`
- `AzureOpenAI:ApiKey`
- `AzureOpenAI:Deployment`

They live in `dotnet user-secrets` keyed against the demo project (`AgentEval/samples/ECS2026MAF`); the eval project (`AgentEval/samples/ECS2026MAF.Eval`) shares the same `<UserSecretsId>` and reads them automatically.

## Path A — point at an existing Azure OpenAI resource

You already have a resource and a deployment. Three commands:

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint"   "https://YOUR-RESOURCE.openai.azure.com/" --project AgentEval/samples/ECS2026MAF
dotnet user-secrets set "AzureOpenAI:ApiKey"     "YOUR-KEY"                                 --project AgentEval/samples/ECS2026MAF
dotnet user-secrets set "AzureOpenAI:Deployment" "YOUR-DEPLOYMENT-NAME"                     --project AgentEval/samples/ECS2026MAF
```

Verify:

```bash
dotnet user-secrets list --project AgentEval/samples/ECS2026MAF
```

Done — return to step 5 in [ATTENDEE-INSTRUCTIONS.md](ATTENDEE-INSTRUCTIONS.md).

## Path B — provision manually with `az`

For attendees with subscription rights but no `azd`. Skips the bicep/azd hooks entirely.

### B.1 Sign in

```bash
az version
az login
az account show -o table
```

### B.2 Set variables

```bash
export AZURE_SUBSCRIPTION_ID="<subscription-id>"
export AZURE_LOCATION="australiaeast"
export AZURE_RESOURCE_GROUP="rg-agentcamp-bne-2026"
export AZURE_OPENAI_RESOURCE_NAME="oai-agentcamp-$RANDOM"
export AZURE_OPENAI_DEPLOYMENT="gpt-5.4-mini"
export AZURE_OPENAI_MODEL_NAME="gpt-5.4-mini"
export AZURE_OPENAI_MODEL_VERSION="<version-from-foundry>"
export AZURE_OPENAI_SKU_NAME="GlobalStandard"
```

PowerShell equivalent for the `export` lines:

```powershell
$env:AZURE_LOCATION = 'australiaeast'
$env:AZURE_RESOURCE_GROUP = 'rg-agentcamp-bne-2026'
# ...etc
```

### B.3 Create the resource

```bash
az account set --subscription "$AZURE_SUBSCRIPTION_ID"

az group create \
  --name "$AZURE_RESOURCE_GROUP" \
  --location "$AZURE_LOCATION"

az cognitiveservices account create \
  --name "$AZURE_OPENAI_RESOURCE_NAME" \
  --resource-group "$AZURE_RESOURCE_GROUP" \
  --location "$AZURE_LOCATION" \
  --kind OpenAI \
  --sku S0 \
  --custom-domain "$AZURE_OPENAI_RESOURCE_NAME" \
  --yes
```

### B.4 Find an available model version

```bash
az cognitiveservices account list-models \
  --name "$AZURE_OPENAI_RESOURCE_NAME" \
  --resource-group "$AZURE_RESOURCE_GROUP" \
  --query "[?name=='$AZURE_OPENAI_MODEL_NAME'].{name:name, version:version, format:format, sku:skus[0].name}" \
  -o table
```

Set `AZURE_OPENAI_MODEL_VERSION` to one of the versions printed.

### B.5 Deploy the model

```bash
az cognitiveservices account deployment create \
  --name "$AZURE_OPENAI_RESOURCE_NAME" \
  --resource-group "$AZURE_RESOURCE_GROUP" \
  --deployment-name "$AZURE_OPENAI_DEPLOYMENT" \
  --model-name "$AZURE_OPENAI_MODEL_NAME" \
  --model-version "$AZURE_OPENAI_MODEL_VERSION" \
  --model-format OpenAI \
  --sku-name "$AZURE_OPENAI_SKU_NAME" \
  --sku-capacity 1
```

Verify:

```bash
az cognitiveservices account deployment show \
  --name "$AZURE_OPENAI_RESOURCE_NAME" \
  --resource-group "$AZURE_RESOURCE_GROUP" \
  --deployment-name "$AZURE_OPENAI_DEPLOYMENT" \
  --query properties.provisioningState \
  -o tsv
```

```text
Succeeded
```

### B.6 Write user-secrets

```bash
ENDPOINT="$(az cognitiveservices account show \
  --name "$AZURE_OPENAI_RESOURCE_NAME" \
  --resource-group "$AZURE_RESOURCE_GROUP" \
  --query properties.endpoint \
  -o tsv)"

KEY="$(az cognitiveservices account keys list \
  --name "$AZURE_OPENAI_RESOURCE_NAME" \
  --resource-group "$AZURE_RESOURCE_GROUP" \
  --query key1 \
  -o tsv)"

dotnet user-secrets set "AzureOpenAI:Endpoint"   "$ENDPOINT"               --project AgentEval/samples/ECS2026MAF
dotnet user-secrets set "AzureOpenAI:ApiKey"     "$KEY"                    --project AgentEval/samples/ECS2026MAF
dotnet user-secrets set "AzureOpenAI:Deployment" "$AZURE_OPENAI_DEPLOYMENT" --project AgentEval/samples/ECS2026MAF
```

Return to step 5 in [ATTENDEE-INSTRUCTIONS.md](ATTENDEE-INSTRUCTIONS.md).

## Clean up

If you used `azd up`:

```bash
azd down --purge
```

If you used the manual path:

```bash
az group delete --name "$AZURE_RESOURCE_GROUP" --yes
```

And, optionally, clear the user-secrets:

```bash
dotnet user-secrets clear --project AgentEval/samples/ECS2026MAF
```

## CI / non-interactive runs

The sample also reads `AZURE_OPENAI_*` environment variables as a fallback when no user-secrets are set. That path is intended for CI, not for the attendee laptop. See `Config.cs` in `AgentEval/samples/ECS2026MAF/`.

## References

- [Create and deploy an Azure OpenAI resource](https://learn.microsoft.com/azure/ai-foundry/openai/how-to/create-resource)
- [Deploy models with Azure CLI and Bicep](https://learn.microsoft.com/azure/foundry/foundry-models/how-to/create-model-deployments)
- [Azure Developer CLI commands](https://learn.microsoft.com/azure/developer/azure-developer-cli/azd-commands)
- [docs/MODEL-OPTIONS.md](MODEL-OPTIONS.md) — region/model picks and how to plug in alternatives.

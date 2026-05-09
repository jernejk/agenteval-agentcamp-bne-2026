#!/usr/bin/env sh
# Post-provision hook for AgentCamp Brisbane 2026.
# Reads bicep outputs from `azd env`, fetches the Azure OpenAI key with the
# Azure CLI, and writes user-secrets against the demo project. Both csproj
# files share the same <UserSecretsId> so the eval project picks them up
# automatically.

set -eu

REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
PROJ="${REPO_ROOT}/AgentEval/samples/ECS2026MAF"

ENDPOINT="$(azd env get-value AZURE_OPENAI_ENDPOINT)"
DEPLOYMENT="$(azd env get-value AZURE_OPENAI_DEPLOYMENT)"
RESOURCE="$(azd env get-value AZURE_OPENAI_RESOURCE_NAME)"
RG="$(azd env get-value AZURE_RESOURCE_GROUP_NAME)"

if [ -z "${ENDPOINT}" ] || [ -z "${DEPLOYMENT}" ] || [ -z "${RESOURCE}" ] || [ -z "${RG}" ]; then
  echo "postprovision: one or more azd outputs missing — check 'azd env get-values'." >&2
  exit 1
fi

# Surface the active subscription so the user knows where this landed.
SUB_NAME="$(az account show --query name -o tsv 2>/dev/null || echo unknown)"
SUB_ID="$(az account show --query id -o tsv 2>/dev/null || echo unknown)"

KEY="$(az cognitiveservices account keys list \
  --name "${RESOURCE}" \
  --resource-group "${RG}" \
  --query key1 \
  -o tsv)"

if [ -z "${KEY}" ]; then
  echo "postprovision: failed to read key from Azure CLI." >&2
  exit 1
fi

dotnet user-secrets set "AzureOpenAI:Endpoint"   "${ENDPOINT}"   --project "${PROJ}" >/dev/null
dotnet user-secrets set "AzureOpenAI:ApiKey"     "${KEY}"        --project "${PROJ}" >/dev/null
dotnet user-secrets set "AzureOpenAI:Deployment" "${DEPLOYMENT}" --project "${PROJ}" >/dev/null

# azd up swallows stdout from hooks but streams stderr. Write the summary to
# stderr so attendees actually see it after `azd up` finishes.
{
  echo ""
  echo "postprovision: user-secrets written for ECS2026MAF (shared with ECS2026MAF.Eval)."
  echo ""
  echo "  subscription = ${SUB_NAME}  (${SUB_ID})"
  echo "  resource     = ${RESOURCE}"
  echo "  resource grp = ${RG}"
  echo "  endpoint     = ${ENDPOINT}"
  echo "  deployment   = ${DEPLOYMENT}"
  echo ""
  echo "Smoke test:    dotnet run --project AgentEval/samples/ECS2026MAF -- --smoke"
  echo "Tear down:     azd down --purge"
} >&2

# Post-provision hook for AgentCamp Brisbane 2026 (Windows).
# Reads bicep outputs from `azd env`, fetches the Azure OpenAI key with
# the Azure CLI, and writes user-secrets against the demo project.

$ErrorActionPreference = 'Stop'

$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot '..' '..')
$Proj     = Join-Path $RepoRoot 'AgentEval/samples/ECS2026MAF'

$Endpoint   = (azd env get-value AZURE_OPENAI_ENDPOINT).Trim()
$Deployment = (azd env get-value AZURE_OPENAI_DEPLOYMENT).Trim()
$Resource   = (azd env get-value AZURE_OPENAI_RESOURCE_NAME).Trim()
$Rg         = (azd env get-value AZURE_RESOURCE_GROUP_NAME).Trim()

if (-not $Endpoint -or -not $Deployment -or -not $Resource -or -not $Rg) {
    Write-Error "postprovision: one or more azd outputs missing — check 'azd env get-values'."
    exit 1
}

$Key = (az cognitiveservices account keys list `
    --name $Resource `
    --resource-group $Rg `
    --query key1 `
    -o tsv).Trim()

if (-not $Key) {
    Write-Error "postprovision: failed to read key from Azure CLI."
    exit 1
}

dotnet user-secrets set 'AzureOpenAI:Endpoint'   $Endpoint   --project $Proj | Out-Null
dotnet user-secrets set 'AzureOpenAI:ApiKey'     $Key        --project $Proj | Out-Null
dotnet user-secrets set 'AzureOpenAI:Deployment' $Deployment --project $Proj | Out-Null

Write-Host "postprovision: user-secrets written for ECS2026MAF (shared with ECS2026MAF.Eval)."
Write-Host "postprovision: endpoint   = $Endpoint"
Write-Host "postprovision: deployment = $Deployment"
Write-Host "postprovision: tear down with 'azd down --purge' when finished."

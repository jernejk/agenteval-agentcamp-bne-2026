// Azure OpenAI provisioning for the AgentCamp Brisbane 2026 workshop.
// Subscription-scoped so azd creates the resource group itself; the
// AzureOpenAI module below provisions the cognitive-services account
// and one model deployment used by the .NET sample.

targetScope = 'subscription'

@description('Azure region for the resource group and Azure OpenAI resource.')
@minLength(1)
param location string

@description('Resource group name. azd creates it if it does not already exist.')
@minLength(1)
param resourceGroupName string

@description('Globally unique Azure OpenAI resource name.')
@minLength(2)
@maxLength(64)
param openAiResourceName string

@description('Deployment name read by AZURE_OPENAI_DEPLOYMENT / AzureOpenAI:Deployment.')
@minLength(1)
param deploymentName string

@description('Underlying Azure OpenAI model name (for example gpt-5.4-mini). Confirm availability in your region first.')
@minLength(1)
param modelName string

@description('Model version from the Azure AI Foundry catalogue or `az cognitiveservices account list-models`. Do not guess.')
@minLength(1)
param modelVersion string

@description('Model provider format.')
@allowed([
  'OpenAI'
])
param modelFormat string = 'OpenAI'

@description('Deployment SKU name. Availability depends on region, quota, and model.')
@allowed([
  'Standard'
  'GlobalStandard'
  'DataZoneStandard'
  'ProvisionedManaged'
  'GlobalProvisionedManaged'
  'DataZoneProvisionedManaged'
])
param skuName string = 'GlobalStandard'

@description('Deployment SKU capacity.')
@minValue(1)
param skuCapacity int = 1

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: resourceGroupName
  location: location
}

module openAi 'modules/openai.bicep' = {
  name: 'openai'
  scope: rg
  params: {
    location: location
    openAiResourceName: openAiResourceName
    deploymentName: deploymentName
    modelName: modelName
    modelVersion: modelVersion
    modelFormat: modelFormat
    skuName: skuName
    skuCapacity: skuCapacity
  }
}

output AZURE_RESOURCE_GROUP_NAME string = rg.name
output AZURE_LOCATION string = location
output AZURE_OPENAI_RESOURCE_NAME string = openAi.outputs.resourceName
output AZURE_OPENAI_ENDPOINT string = openAi.outputs.endpoint
output AZURE_OPENAI_DEPLOYMENT string = openAi.outputs.deploymentName
output AZURE_OPENAI_MODEL_NAME string = modelName

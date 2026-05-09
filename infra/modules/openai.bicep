// Resource-group-scoped module: Azure OpenAI account + one deployment.

targetScope = 'resourceGroup'

@description('Azure region.')
param location string

@description('Globally unique Azure OpenAI resource name.')
param openAiResourceName string

@description('Deployment name (consumed via AZURE_OPENAI_DEPLOYMENT).')
param deploymentName string

@description('Model name, e.g. gpt-5.4-mini.')
param modelName string

@description('Model version from the Foundry catalogue.')
param modelVersion string

@description('Model provider format.')
param modelFormat string

@description('Deployment SKU name.')
param skuName string

@description('Deployment SKU capacity.')
param skuCapacity int

resource openAi 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: openAiResourceName
  location: location
  kind: 'OpenAI'
  sku: {
    name: 'S0'
  }
  properties: {
    customSubDomainName: toLower(openAiResourceName)
    publicNetworkAccess: 'Enabled'
  }
}

resource deployment 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  parent: openAi
  name: deploymentName
  sku: {
    name: skuName
    capacity: skuCapacity
  }
  properties: {
    model: {
      format: modelFormat
      name: modelName
      version: modelVersion
    }
    raiPolicyName: 'Microsoft.DefaultV2'
  }
}

output resourceName string = openAi.name
output endpoint string = openAi.properties.endpoint
output deploymentName string = deployment.name

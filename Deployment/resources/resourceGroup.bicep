@description('Location for all resources.')
param location string = resourceGroup().location
param keyVaultName string
param storageAccountName string
param serviceBusName string
param applicationInsightsName string
param discordBotFunctionAppName string
param orchestratorFunctionAppName string
param discordBotSettings object
param orchestratorSettings object
@secure()
param discordBotSecrets object
@secure()
param orchestratorSecrets object

var allSecrets = union(discordBotSecrets, orchestratorSecrets)

var queueSettings = [
  {
    name: 'ClipsQueueName'
    value: 'clips'
  }
  {
    name: 'EventScheduleQueueName'
    value: 'eventschedule'
  }
  {
    name: 'GoLiveQueueName'
    value: 'golive'
  }
  {
    name: 'VideosQueueName'
    value: 'videos'
  }
]

var commonAppSettings = concat(queueSettings, [
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: applicationInsights.properties.ConnectionString
  }
  {
    name: 'AzureWebJobsStorage'
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value}'
  }
  {
    name: 'FUNCTIONS_EXTENSION_VERSION'
    value: '~4'
  }
  {
    name: 'FUNCTIONS_WORKER_RUNTIME'
    value: 'dotnet'
  }
  {
    name: 'StreamingServiceBus__fullyQualifiedNamespace'
    value: '${serviceBusNamespace.name}.servicebus.windows.net'
  }
  {
    name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value}'
  }
  {
    name: 'WEBSITE_RUN_FROM_PACKAGE'
    value: '1'
  }
])

var discordBotAppSettings = [for setting in items(discordBotSettings): {
  name: setting.key
  value: setting.value
}]

var orchestratorAppSettings = [for setting in items(orchestratorSettings): {
  name: setting.key
  value: setting.value
}]

var discordBotSecretAppSettings = [for secret in items(discordBotSecrets): {
  name: secret.key
  value: '@Microsoft.KeyVault(SecretUri=https://${keyVault.name}.vault.azure.net/secrets/${secret.key})'
}]

var orchestratorSecretAppSettings = [for secret in items(orchestratorSecrets): {
  name: secret.key
  value: '@Microsoft.KeyVault(SecretUri=https://${keyVault.name}.vault.azure.net/secrets/${secret.key})'
}]

var functionAppProperties = [
  {
    name: discordBotFunctionAppName
    settings: discordBotAppSettings
    secrets: discordBotSecretAppSettings
    sbRoleId: serviceBusDataReceiver.id
  }
  {
    name: orchestratorFunctionAppName
    settings: orchestratorAppSettings
    secrets: orchestratorSecretAppSettings
    sbRoleId: serviceBusDataSender.id
  }
]

resource keyVault 'Microsoft.KeyVault/vaults@2021-10-01' = {
  name: keyVaultName
  location: location
  properties: {
    enableRbacAuthorization: true
    tenantId: tenant().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
  }
}

resource keyVaultSecrets 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = [for secret in items(allSecrets): {
  name: secret.key
  parent: keyVault
  properties: {
    value: secret.value
  }
}]

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: toLower(storageAccountName)
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2021-11-01' = {
  name: serviceBusName
  location: location
  sku: {
    name: 'Standard'
  }
}

resource queues 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = [for queue in (queueSettings): {
  parent: serviceBusNamespace
  name: queue.value
  properties: {
    lockDuration: 'PT5M'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: true
    requiresSession: false
    defaultMessageTimeToLive: 'P10675199DT2H48M5.4775807S'
    deadLetteringOnMessageExpiration: false
    duplicateDetectionHistoryTimeWindow: 'PT10M'
    maxDeliveryCount: 10
    autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
    enablePartitioning: false
    enableExpress: false
  }
}]

resource hostingPlans 'Microsoft.Web/serverfarms@2022-03-01' = [for functionApp in (functionAppProperties): {
  name: functionApp.Name
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
    capacity: 0
  }
  properties: {
    computeMode: 'Dynamic'
  }
}]

resource functionApps 'Microsoft.Web/sites@2022-03-01' = [for (functionApp, i) in (functionAppProperties): {
  name: functionApp.name
  location: location
  kind: 'functionapp'
  identity:{
    type:'SystemAssigned'
  }
  properties: {
    serverFarmId: hostingPlans[i].id
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: false
      appSettings: concat(commonAppSettings, functionApp.settings, functionApp.secrets, [
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(functionApp.name)
        }
      ])
      netFrameworkVersion: 'v6.0'
    }
    httpsOnly: true
  }
}]

@description('This is the built-in Key Vault Secrets User. See https://docs.microsoft.com/en-gb/azure/role-based-access-control/built-in-roles#key-vault-secrets-user')
resource keyVaultSecretsUserRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: '4633458b-17de-408a-b874-0445c86b69e6'
}

@description('This is the built-in Azure Service Bus Data Receiver role. See https://docs.microsoft.com/en-gb/azure/role-based-access-control/built-in-roles#azure-service-bus-data-receiver')
resource serviceBusDataReceiver 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: '4f6d3b9b-027b-4f4c-9142-0e5a2a2247e0'
}

@description('This is the built-in Azure Service Bus Data Sender role. See https://docs.microsoft.com/en-gb/azure/role-based-access-control/built-in-roles#azure-service-bus-data-sender')
resource serviceBusDataSender 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: '69a216fc-b8fb-44d8-bc22-1f3c2cd27a39'
}

resource functionAppKeyVaultRoleAssignments 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for (functionApp, i) in (functionAppProperties): {
  name: guid(keyVault.id, functionApps[i].id, keyVaultSecretsUserRoleDefinition.id)
  scope: keyVault
  properties: {
    roleDefinitionId: keyVaultSecretsUserRoleDefinition.id
    principalId: functionApps[i].identity.principalId
    principalType: 'ServicePrincipal'
  }
}]

resource functionAppServiceBusRoleAssignments 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for (functionApp, i) in (functionAppProperties): {
  name: guid(serviceBusNamespace.id, functionApps[i].id, functionApp.sbRoleId)
  scope: serviceBusNamespace
  properties: {
    roleDefinitionId: functionApp.sbRoleId
    principalId: functionApps[i].identity.principalId
    principalType: 'ServicePrincipal'
  }
}]

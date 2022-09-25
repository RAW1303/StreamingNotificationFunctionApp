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

var commonAppSettings = [
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    value: applicationInsights.properties.InstrumentationKey
  }
  {
    name: 'AzureWebJobsStorage'
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value}'
  }
  {
    name: 'ClipsQueueName'
    value: clipsQueue.name
  }
  {
    name: 'DailyScheduleQueueName'
    value: dailyScheduleQueue.name
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
    name: 'GoLiveQueueName'
    value: goLiveQueue.name
  }
  {
    name: 'StreamingServiceBus__fullyQualifiedNamespace'
    value: '${serviceBusNamespace.name}.servicebus.windows.net;'
  }
  {
    name: 'VideoQueueName'
    value: videoQueue.name
  }
  {
    name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value}'
  }
  {
    name: 'WEBSITE_RUN_FROM_PACKAGE'
    value: '1'
  }
  {
    name: 'WeeklyScheduleQueueName'
    value: weeklyScheduleQueue.name
  }
]

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

var queueProperties = {
  lockDuration: 'PT5M'
  maxSizeInMegabytes: 1024
  requiresDuplicateDetection: false
  requiresSession: false
  defaultMessageTimeToLive: 'P10675199DT2H48M5.4775807S'
  deadLetteringOnMessageExpiration: false
  duplicateDetectionHistoryTimeWindow: 'PT10M'
  maxDeliveryCount: 10
  autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
  enablePartitioning: false
  enableExpress: false
}

var hostingPlanSku = {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
    capacity: 0
}


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
  name: storageAccountName
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

resource clipsQueue 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBusNamespace
  name: 'clips'
  properties: queueProperties
}

resource dailyScheduleQueue 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBusNamespace
  name: 'dailyschedule'
  properties: queueProperties
}

resource goLiveQueue 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBusNamespace
  name: 'golive'
  properties: queueProperties
}

resource videoQueue 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBusNamespace
  name: 'videos'
  properties: queueProperties
}

resource weeklyScheduleQueue 'Microsoft.ServiceBus/namespaces/queues@2022-01-01-preview' = {
  parent: serviceBusNamespace
  name: 'weeklyschedule'
  properties: queueProperties
}

resource discordBotHostingPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: discordBotFunctionAppName
  location: location
  sku: hostingPlanSku
  properties: {
    computeMode: 'Dynamic'
  }
}

resource orchestratorHostingPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: orchestratorFunctionAppName
  location: location
  sku: hostingPlanSku
  properties: {
    computeMode: 'Dynamic'
  }
}

resource discordBotFunctionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: discordBotFunctionAppName
  location: location
  kind: 'functionapp'
  identity:{
    type:'SystemAssigned'
  }
  properties: {
    serverFarmId: discordBotHostingPlan.id
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: false
      appSettings: concat(commonAppSettings, discordBotSecretAppSettings, discordBotAppSettings, [
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(discordBotFunctionAppName)
        }
      ])
    }
    httpsOnly: true
  }
}

resource orchestratorFunctionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: orchestratorFunctionAppName
  location: location
  kind: 'functionapp'
  identity:{
    type:'SystemAssigned'
  }
  properties: {
    serverFarmId: orchestratorHostingPlan.id
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: false
      appSettings: concat(commonAppSettings, orchestratorSecretAppSettings, orchestratorAppSettings, [
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(orchestratorFunctionAppName)
        }
      ])
    }
    httpsOnly: true
  }
}

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

resource orchestratorKeyVaultRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, orchestratorFunctionApp.id, keyVaultSecretsUserRoleDefinition.id)
  scope: keyVault
  properties: {
    roleDefinitionId: keyVaultSecretsUserRoleDefinition.id
    principalId: orchestratorFunctionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource discordBotKeyVaultRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, discordBotFunctionApp.id, keyVaultSecretsUserRoleDefinition.id)
  scope: keyVault
  properties: {
    roleDefinitionId: keyVaultSecretsUserRoleDefinition.id
    principalId: discordBotFunctionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource orchestratorServiceBusRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(serviceBusNamespace.id, orchestratorFunctionApp.id, serviceBusDataSender.id)
  scope: serviceBusNamespace
  properties: {
    roleDefinitionId: serviceBusDataSender.id
    principalId: orchestratorFunctionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource discordBotServiceBusRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(serviceBusNamespace.id, discordBotFunctionApp.id, serviceBusDataReceiver.id)
  scope: serviceBusNamespace
  properties: {
    roleDefinitionId: serviceBusDataReceiver.id
    principalId: discordBotFunctionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string

param functionServiceName string = ''
param applicationInsightsDashboardName string = ''
param applicationInsightsName string = ''
param functionAppServicePlanName string = ''
param siteAppServicePlanName string = ''
param logAnalyticsName string = ''
param resourceGroupName string = ''
param storageAccountName string = ''
param webServiceName string = ''

param azureFridayApiPath string = 'output/azurefriday.json'
param azureFridayAudioRssPath string = 'output/azurefridayaudio.rss'
param azureFridayRssPath string = 'output/azurefriday.rss'

var azureFridayApi = '${storage.outputs.primaryEndpoints.blob}${azureFridayApiPath}'
var azureFridayAudioRss = '${storage.outputs.primaryEndpoints.blob}${azureFridayAudioRssPath}'
var azureFridayRss = '${storage.outputs.primaryEndpoints.blob}${azureFridayRssPath}'

var abbrs = loadJsonContent('abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var tags = { 'azd-env-name': environmentName }

// Organize resources in a resource group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: !empty(resourceGroupName) ? resourceGroupName : '${abbrs.resourcesResourceGroups}${environmentName}'
  location: location
  tags: tags
}

// The application frontend
module web './app/web.bicep' = {
  name: 'web'
  scope: rg
  params: {
    name: !empty(webServiceName) ? webServiceName : '${abbrs.webSitesAppService}web-${resourceToken}'
    location: location
    applicationInsightsName: monitoring.outputs.applicationInsightsName
    appServicePlanId: siteAppServicePlan.outputs.id
    appSettings: {
      AZURE_FRIDAY_API: azureFridayApi
      AZURE_FRIDAY_AUDIO_RSS: azureFridayAudioRss
      AZURE_FRIDAY_RSS: azureFridayRss
    }
  }
}

// The function
module function './app/function.bicep' = {
  name: 'function'
  scope: rg
  params: {
    name: !empty(functionServiceName) ? functionServiceName : '${abbrs.webSitesAppService}function-${resourceToken}'
    location: location
    tags: tags
    applicationInsightsName: monitoring.outputs.applicationInsightsName
    appServicePlanId: funcAppServicePlan.outputs.id
    storageAccountName: storage.outputs.name
  }
}

// Create an App Service Plan to group applications under the same payment plan and SKU
module funcAppServicePlan './core/host/appserviceplan.bicep' = {
  name: 'funcappserviceplan'
  scope: rg
  params: {
    name: !empty(functionAppServicePlanName) ? functionAppServicePlanName : '${abbrs.webServerFarms}func-${resourceToken}'
    location: location
    tags: tags
    sku: {
      name: 'Y1'
      tier: 'Dynamic'
    }
  }
}

// Create an App Service Plan to group applications under the same payment plan and SKU
module siteAppServicePlan './core/host/appserviceplan.bicep' = {
  name: 'siteappserviceplan'
  scope: rg
  params: {
    name: !empty(siteAppServicePlanName) ? siteAppServicePlanName : '${abbrs.webServerFarms}site-${resourceToken}'
    location: location
    tags: tags
    sku: {
      name: 'B1'
    }
  }
  dependsOn: [
    funcAppServicePlan
  ]
}

// Storage account
module storage './core/storage/storage-account.bicep' = {
  name: 'storage'
  scope: rg
  params: {
    name: !empty(storageAccountName) ? storageAccountName : '${abbrs.storageStorageAccounts}${resourceToken}'
    location: location
    tags: tags
    allowBlobPublicAccess: true
    containers: [
      {
        name: 'output'
        publicAccess: 'Blob'
      }
    ]
  }
}

// Monitor application with Azure Monitor
module monitoring './core/monitor/monitoring.bicep' = {
  name: 'monitoring'
  scope: rg
  params: {
    location: location
    tags: tags
    logAnalyticsName: !empty(logAnalyticsName) ? logAnalyticsName : '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
    applicationInsightsName: !empty(applicationInsightsName) ? applicationInsightsName : '${abbrs.insightsComponents}${resourceToken}'
    applicationInsightsDashboardName: !empty(applicationInsightsDashboardName) ? applicationInsightsDashboardName : '${abbrs.portalDashboards}${resourceToken}'
  }
}

output APPLICATIONINSIGHTS_CONNECTION_STRING string = monitoring.outputs.applicationInsightsConnectionString
output AZURE_LOCATION string = location
output AZURE_TENANT_ID string = tenant().tenantId
output SERVICE_WEB_URI string = web.outputs.SERVICE_WEB_URI
output AZURE_FRIDAY_API string = azureFridayApi
output AZURE_FRIDAY_AUDIO_RSS string = azureFridayAudioRss
output AZURE_FRIDAY_RSS string = azureFridayRss

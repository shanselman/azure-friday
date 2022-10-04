param environmentName string
param location string = resourceGroup().location

param azureFridayApiPath string = 'output/azurefriday.json'
param azureFridayAudioRssPath string = 'output/azurefridayaudio.rss'
param azureFridayRssPath string = 'output/azurefriday.rss'

var azureFridayApi = '${storage.outputs.primaryEndpoints.blob}${azureFridayApiPath}'
var azureFridayAudioRss = '${storage.outputs.primaryEndpoints.blob}${azureFridayAudioRssPath}'
var azureFridayRss = '${storage.outputs.primaryEndpoints.blob}${azureFridayRssPath}'

// The application frontend
module web './app/web.bicep' = {
  name: 'web'
  params: {
    environmentName: environmentName
    location: location
    appServicePlanId: sitesAppServicePlan.outputs.appServicePlanId
    applicationInsightsName: monitoring.outputs.applicationInsightsName 
    appSettings: {
      AZURE_FRIDAY_API: azureFridayApi
      AZURE_FRIDAY_AUDIO_RSS: azureFridayAudioRss
      AZURE_FRIDAY_RSS: azureFridayRss
    }
  }
}

// The application frontend
module func './app/function.bicep' = {
  name: 'func'
  params: {
    environmentName: environmentName
    location: location
    appServicePlanId: funcAppServicePlan.outputs.appServicePlanId
    applicationInsightsName: monitoring.outputs.applicationInsightsName 
    storageAccountName: storage.outputs.name
  }
}

// App service plan for function
module funcAppServicePlan './core/host/appserviceplan-functions.bicep' = {
  name: 'funcappserviceplan'
  params: {
    environmentName: environmentName
    location: location
  }
}

// App service plan for website
module sitesAppServicePlan './core/host/appserviceplan-sites.bicep' = {
  name: 'sitesappserviceplan'
  params: {
    environmentName: environmentName
    location: location
  }
  dependsOn: [
    funcAppServicePlan
  ]
}

// Backing storage for Azure functions backend API
module storage './core/storage/storage-account.bicep' = {
  name: 'storage'
  params: {
    environmentName: environmentName
    location: location
  }
}

// Monitor application with Azure Monitor
module monitoring './core/monitor/monitoring.bicep' = {
  name: 'monitoring'
  params: {
    environmentName: environmentName
    location: location
  }
}

output APPLICATIONINSIGHTS_CONNECTION_STRING string = monitoring.outputs.applicationInsightsConnectionString
output WEB_URI string = web.outputs.WEB_URI
output STORAGE_ENDPOINTS object = storage.outputs.primaryEndpoints
output AZURE_FRIDAY_API string = azureFridayApi
output AZURE_FRIDAY_AUDIO_RSS string = azureFridayAudioRss
output AZURE_FRIDAY_RSS string = azureFridayRss

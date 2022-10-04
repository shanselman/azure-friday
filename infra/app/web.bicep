param environmentName string
param location string = resourceGroup().location

param serviceName string = 'web'
param applicationInsightsName string = ''
param appServicePlanId string
param appSettings object = {}

module web '../core/host/appservice-dotnet.bicep' = {
  name: '${serviceName}-appservice-module'
  params: {
    environmentName: environmentName
    location: location
    serviceName: serviceName
    applicationInsightsName: applicationInsightsName
    appServicePlanId: appServicePlanId
    appSettings: appSettings
  }
}

output WEB_NAME string = web.outputs.name
output WEB_URI string = web.outputs.uri

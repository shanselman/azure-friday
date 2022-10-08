param environmentName string
param location string = resourceGroup().location

param allowedOrigins array = []
param applicationInsightsName string
param appServicePlanId string
param appSettings object = {}
param serviceName string = 'function'
param storageAccountName string

module func '../core/host/functions-dotnet-isolated.bicep' = {
  name: '${serviceName}-functions-csharp-module'
  params: {
    environmentName: environmentName
    location: location
    allowedOrigins: allowedOrigins
    appSettings: appSettings
    applicationInsightsName: applicationInsightsName
    appServicePlanId: appServicePlanId
    serviceName: serviceName
    storageAccountName: storageAccountName
  }
}

output FUNC_IDENTITY_PRINCIPAL_ID string = func.outputs.identityPrincipalId
output FUNC_NAME string = func.outputs.name
output FUNC_URI string = func.outputs.uri

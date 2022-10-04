param environmentName string
param location string = resourceGroup().location

param allowedOrigins array = []
param applicationInsightsName string
param appServicePlanId string
param appSettings object = {}
param enableOryxBuild bool = true
param functionsExtensionVersion string = '~4'
param functionsWorkerRuntime string = 'dotnet-isolated'
param keyVaultName string = ''
param linuxFxVersion string = 'DOTNET-ISOLATED|6.0'
param kind string = 'functionapp,linux'
param managedIdentity bool = !(empty(keyVaultName))
param serviceName string
param storageAccountName string

module functions 'functions.bicep' = {
  name: '${serviceName}-functions-csharp'
  params: {
    environmentName: environmentName
    location: location
    allowedOrigins: allowedOrigins
    applicationInsightsName: applicationInsightsName
    appServicePlanId: appServicePlanId
    appSettings: appSettings
    enableOryxBuild: enableOryxBuild
    functionsExtensionVersion: functionsExtensionVersion
    functionsWorkerRuntime: functionsWorkerRuntime
    keyVaultName: keyVaultName
    linuxFxVersion: linuxFxVersion
    managedIdentity: managedIdentity
    serviceName: serviceName
    storageAccountName: storageAccountName
    scmDoBuildDuringDeployment: false
    kind: kind
  }
}

output identityPrincipalId string = functions.outputs.identityPrincipalId
output name string = functions.outputs.name
output uri string = functions.outputs.uri

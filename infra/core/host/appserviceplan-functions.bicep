param environmentName string
param location string = resourceGroup().location

param reserved bool = true
param sku object = {
  name: 'Y1'
  tier: 'Dynamic'
  size: 'Y1'
  family: 'Y'
}
param slug string  = 'functions'

module appServicePlanFunctions 'appserviceplan.bicep' = {
  name: 'appserviceplan-functions'
  params: {
    environmentName: environmentName
    location: location
    kind: 'functionapp'
    reserved: reserved
    sku: sku
    slug: slug
  }
}

output appServicePlanId string = appServicePlanFunctions.outputs.appServicePlanId

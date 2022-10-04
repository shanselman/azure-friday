param environmentName string
param location string = resourceGroup().location

param reserved bool = true
param sku object = { name: 'B1' }
param slug string  = 'sites'

module appServicePlanSites 'appserviceplan.bicep' = {
  name: 'appserviceplan-sites'
  params: {
    environmentName: environmentName
    location: location
    reserved: reserved
    sku: sku
    slug: slug
  }
}

output appServicePlanId string = appServicePlanSites.outputs.appServicePlanId

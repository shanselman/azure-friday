This is the repo for https://its-azure-friday.azurewebsites.net/ or http://www.azurefriday.com

Follow these steps to get the Azure Friday web app and function deployed to Azure.

1. Open in VS Code DevContainer, Codespaces, or install [.NET CLI](https://dotnet.microsoft.com/download), [Azure CLI](http://aka.ms/getazcli), and [Azure Developer CLI](https://aka.ms/azd-install) locally.
1. Run `azd up`
1. Enter environment name, choose location and subscription.
1. Azd will then provision Azure resources and deploy the Azure Friday code to those resources.
1. Click on the "Endpoint" link to view the site fully running on Azure.
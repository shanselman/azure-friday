This is the repo for https://its-azure-friday.azurewebsites.net/ or http://www.azurefriday.com

Note: Due to an issue with dotnet-isolated function installs, if you don't use a DevContainer or Codespaces, then please install azd from this link.  This will be fixed and resolved soon

```
curl -fsSL https://azuresdkreleasepreview.blob.core.windows.net/azd/standalone/pr/551/install-azd.sh | bash -s -- --base-url https://azuresdkreleasepreview.blob.core.windows.net/azd/standalone/pr/551 --version '' --verbose
```

To get on Azure:

1. Open in VS Code DevContainer, Codespaces, or install [.NET CLI](https://dotnet.microsoft.com/download), [Azure CLI](http://aka.ms/getazcli), and [Azure Developer CLI](https://aka.ms/azd) locally.
1. Run `azd up`
1. Enter environment name, choose location and subscription.
1. Azd will then provision Azure resources and deploy the Azure Friday code to those resources.
1. Click on the "Endpoint" link to view the site fully running on Azure.
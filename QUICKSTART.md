# Quick Start - Deploy to Azure

## .NET 11 RC 3 prerequisite

Install the SDK selected by [global.json](global.json):
`11.0.100-rc.3.26522.148`. This is a development prerelease upgrade.
Before deployment, run `az webapp list-runtimes --os linux --output table` and
confirm the configured `dotnet|11` stack and an RC-compatible runtime in your
target region and web app. The catalog reported this stack on November 5, 2026,
but no Azure deployment has been validated for this upgrade. If the required
runtime is unavailable, do not deploy. See [deployment prerequisites](AZURE_DEPLOYMENT.md).

## 🚀 Deploy in 3 Commands

### 1. Login to Azure
```bash
azd auth login
```

### 2. Deploy Everything
```bash
azd up
```
**What it does:**
- ✅ Creates Resource Group
- ✅ Provisions Azure SQL Database
- ✅ Creates App Service (Linux + .NET 11.0)
- ✅ Sets up Key Vault with secrets
- ✅ Configures Application Insights
- ✅ Deploys your application code
- ✅ Returns your live application URL

**Time**: ~5-10 minutes

### 3. Open Your App
The deployment will provide a URL like:
```
https://app-abc123.azurewebsites.net
```

## 🔑 Same Login Credentials

- **Username**: `machen` (or jawilson, prpatel, etc.)
- **Password**: `Doctor123!`

## 📊 View Metrics
```bash
azd monitor
```

## 🔄 Update After Code Changes
```bash
azd deploy
```

## 🗑️ Delete Everything
```bash
azd down
```

## 💰 Expected Monthly Cost
**~$18-20 USD** (Basic tier for App Service + SQL Database)

## 📚 Full Documentation
- **Azure Deployment Guide**: [AZURE_DEPLOYMENT.md](AZURE_DEPLOYMENT.md)
- **Architecture Plan**: [azd-arch-plan.md](azd-arch-plan.md)
- **Full README**: [README.md](README.md)

---

After confirming the .NET 11 runtime prerequisites, run: `azd up`

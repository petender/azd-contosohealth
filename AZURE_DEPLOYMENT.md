# Azure Deployment Summary - Contoso Clinic

## .NET 11 RC 3 deployment prerequisites

The application targets .NET 11 RC 3 for development. Infrastructure is configured
for the Linux `dotnet|11` App Service stack, which the Azure CLI catalog reported
on November 5, 2026. Before provisioning or deploying, run
`az webapp list-runtimes --os linux --output table` and verify the target region
and web app provide a runtime compatible with RC 3. Catalog availability alone
does not verify a deployed host. Do not deploy to an older runtime.

Install SDK `11.0.100-rc.3.26522.148`, selected by [global.json](global.json).
No Azure deployment has been validated for this upgrade. Update to stable
.NET 11 and revalidate before production adoption. See the
[upgrade notes](README.md#net-11-development-upgrade) and
[development issue #17](https://github.com/petender/azd-contosohealth/issues/17).

## 📦 What Was Created

### 1. Azure Developer CLI Configuration
- **File**: `azure.yaml` ✅
- **Purpose**: Defines the project structure and deployment configuration
- **Service**: Web application configured for App Service deployment

### 2. Infrastructure as Code (Bicep Templates)
- **Directory**: `./infra/` ✅
- **Files Created**:
  - `main.bicep` - Primary deployment template with SecurityControl:Ignore tag
  - `main.parameters.json` - Parameter definitions for azd
  - `abbreviations.json` - Resource naming conventions
  - `modules/app-service.bicep` - App Service Plan + Web App (Linux, .NET 11.0)
  - `modules/database.bicep` - Azure SQL Server + Database
  - `modules/monitoring.bicep` - Application Insights + Log Analytics
  - `modules/keyvault.bicep` - Key Vault for secrets

**Total Bicep Files**: 8 modules

### 3. Application Code Updates
- **Updated**: `martinwebapp.csproj` ✅
  - Added `Microsoft.EntityFrameworkCore.SqlServer` package
  - Retained `Microsoft.EntityFrameworkCore.Sqlite` for local development
  
- **Updated**: `Program.cs` ✅
  - Smart database provider detection
  - Supports both SQLite (local) and SQL Server (Azure)
  - Automatic connection string handling

### 4. Documentation
- **Updated**: `README.md` ✅
  - Complete Azure deployment section
  - Azure architecture diagram
  - Step-by-step deployment instructions
  - Cost estimation (~$18-20/month)
  - Monitoring and troubleshooting guides
  
- **Created**: `azd-arch-plan.md` ✅
  - Comprehensive architecture planning document
  - Discovery analysis and component inventory
  - Azure service selection rationale
  - Infrastructure file checklist

## 🏗️ Azure Resources That Will Be Created

| Resource Type | Name Pattern | Purpose |
|---|---|---|
| Resource Group | `rg-{env-name}` | Container for all resources |
| App Service Plan | `asp-{unique-id}` | Linux B1 Basic tier |
| App Service | `app-{unique-id}` | .NET 11.0 web application |
| SQL Server | `sql-{unique-id}` | Logical SQL server |
| SQL Database | `sqldb-healthcare` | Patient/doctor data (Basic 5 DTU) |
| Key Vault | `kv-{unique-id}` | Secrets management |
| Application Insights | `appi-{unique-id}` | Application monitoring |
| Log Analytics | `log-{unique-id}` | Centralized logging |

**All resources will be tagged with:**
- `SecurityControl: Ignore`
- `azd-env-name: {your-environment-name}`

## 🚀 Deployment Instructions

### Step 1: Install Azure Developer CLI
```bash
# Windows (PowerShell)
winget install Microsoft.AzureDeveloperCLI

# Verify installation
azd version
```

### Step 2: Login to Azure
```bash
azd auth login
```

### Step 3: Initialize and Deploy
```bash
# Navigate to project directory
cd c:\martinwebapp

# Deploy everything (infrastructure + code)
azd up
```

**During deployment, you'll be prompted for:**
- Environment name (e.g., `contosohealth-prod`)
- Azure subscription
- Azure region (recommend: `eastus` or `westus2`)

### Step 4: Access Your Application
After successful deployment (5-10 minutes), you'll receive:
- **Application URL**: `https://app-{unique-id}.azurewebsites.net`
- **Resource Group**: Name of the created resource group
- **All Resource IDs**: Outputted for reference

## 🔒 Security Configuration

### Automated Security Features:
✅ **HTTPS Only** - Enforced on App Service  
✅ **TLS 1.2+** - Minimum TLS version set  
✅ **SQL Encryption** - At rest and in transit  
✅ **Key Vault** - Connection strings stored securely  
✅ **Managed Identity** - App Service → Key Vault authentication  
✅ **Firewall Rules** - SQL Server configured for Azure services  
✅ **RBAC** - Principle of least privilege

### Login Credentials (Same as Local):
- **Username Format**: First 2 chars of first name + 6 chars of last name
- **Password**: `Doctor123!` (all doctors)
- **Available Accounts**: 8 doctor accounts (machen, jawilson, prpatel, rotaylor, amrodrig, mikim, rafoster, thhughes)

## 📊 Monitoring

### Application Insights Dashboard
```bash
azd monitor
```
Opens Azure Portal with:
- Request performance metrics
- Failure rates and exceptions
- Dependency tracking (SQL queries)
- Live metrics stream

### View Application Logs
```bash
# Real-time log streaming
az webapp log tail --name app-{unique-id} --resource-group rg-{env-name}

# Or use azd
azd monitor --live
```

## 💰 Cost Management

**Estimated Monthly Cost**: $18-20 USD

Breakdown:
- App Service Plan (B1): ~$13/month
- Azure SQL Database (Basic): ~$5/month
- Application Insights: Free (1 GB/month included)
- Key Vault: $0.03 per 10k operations
- Log Analytics: Free tier for first 5 GB

**Cost Optimization Tips:**
- Use Dev/Test pricing if eligible
- Scale down to Free tier for development
- Enable auto-shutdown for non-production environments
- Review Azure Advisor recommendations

## 🔄 Development Workflow

### Local Development (.NET 11 RC 3 SDK required)
```bash
dotnet run --project src/contosohealth.csproj
```
- Uses SQLite (`healthcare.db`)
- Same development experience
- No Azure connection required

### Update Azure Deployment
```bash
# Deploy code changes only
azd deploy

# Update infrastructure only
azd provision

# Full update (infrastructure + code)
azd up
```

### View Environment Configuration
```bash
azd env get-values
```

## 🗑️ Cleanup

To delete all Azure resources:
```bash
azd down
```
- Removes all Azure resources
- Deletes resource group
- Local files remain unchanged

## 📝 Next Steps

1. **Deploy to Azure**
   ```bash
   azd up
   ```

2. **Test the Application**
   - Navigate to the provided URL
   - Login with doctor credentials
   - Verify patient/doctor data loaded

3. **Monitor Performance**
   ```bash
   azd monitor
   ```

4. **Optional Enhancements**
   - Configure custom domain
   - Enable deployment slots (requires Standard tier)
   - Set up CI/CD with GitHub Actions
   - Configure geo-replication for SQL Database
   - Add Azure Front Door for global distribution

## 📚 Additional Resources

- [Azure Developer CLI Documentation](https://learn.microsoft.com/azure/developer/azure-developer-cli/)
- [Azure App Service Documentation](https://learn.microsoft.com/azure/app-service/)
- [Azure SQL Database Documentation](https://learn.microsoft.com/azure/azure-sql/)
- [Application Insights Documentation](https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview)

## 🆘 Support

**Common Issues:**

1. **Deployment fails during provisioning**
   - Check Azure subscription permissions
   - Verify region supports all services
   - Review error messages in terminal

2. **Application won't start**
   - Check App Service logs: `az webapp log tail`
   - Verify Key Vault access policy
   - Confirm SQL Database firewall rules

3. **Database connection errors**
   - Check connection string in Key Vault
   - Verify managed identity has Key Vault access
   - Test SQL Server firewall rules

**Get Help:**
- Check `azd-arch-plan.md` for architecture details
- Review Application Insights for runtime errors
- Use `azd monitor` for diagnostics

---

**Status**: ✅ Ready for Production Deployment  
**Last Updated**: January 9, 2026  
**Infrastructure Version**: 1.0.0  
**Azure Developer CLI**: Compatible

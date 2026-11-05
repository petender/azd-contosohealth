# AZD Architecture Plan - Contoso Clinic Healthcare Management System

**Date:** January 9, 2026  
**Project:** martinwebapp - Healthcare Management Application  
**Framework:** ASP.NET Core 11.0 RC 3 Razor Pages
**Target Platform:** Azure

---

## Phase 1: Discovery and Analysis

### .NET 11 upgrade note (November 5, 2026)

The development target is .NET 11 RC 3 with SDK `11.0.100-rc.3.26522.148`
and EF Core `11.0.0-rc.3.26522.148`. The Linux runtime stack is `dotnet|11`,
as reported by `az webapp list-runtimes --os linux`. Verify regional and host
runtime compatibility before deployment; this upgrade has not been deployed
to Azure. SQL Server query compatibility remains explicitly at level 150.
See [deployment prerequisites](AZURE_DEPLOYMENT.md) for the release gate.

### Application Overview

**Application Type:** Healthcare Management Web Application  
**Framework:** ASP.NET Core 11.0 RC 3 with Razor Pages
**Authentication:** Cookie-based authentication for healthcare providers  
**Database:** Currently SQLite (healthcare.db) → Migrating to Azure SQL Database

### Component Inventory

| Component Type | Technology | Location | Purpose | Entry Point |
|---|---|---|---|---|
| Web Application | ASP.NET Core 11.0 Razor Pages | `/Pages` | Healthcare management UI | `Program.cs` |
| Data Layer | Entity Framework Core 11.0 | `/Data` | ORM and database access | `ApplicationDbContext.cs` |
| Database | SQLite → Azure SQL | `/healthcare.db` | Patient/Doctor data storage | Connection string in `appsettings.json` |
| Static Assets | wwwroot | `/wwwroot` | Images, CSS, JS, Bootstrap 5.3 | Various |
| Authentication | ASP.NET Core Identity (Cookies) | `Program.cs` | Doctor authentication | Login/Logout pages |

### File System Structure

```
c:\martinwebapp\
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext with seed data
│   ├── Patient.cs                  # Patient entity (12 properties)
│   ├── Doctor.cs                   # Doctor entity (12 properties)
│   └── Employee.cs                 # Legacy model (not in use)
├── Pages/
│   ├── Index.cshtml                # CONTOSO CLINIC homepage
│   ├── Login.cshtml + .cs          # Authentication page
│   ├── Logout.cshtml + .cs         # Sign-out handler
│   ├── Patients.cshtml + .cs       # Patient list (protected, [Authorize])
│   ├── PatientDetail.cshtml + .cs  # Patient detail view (protected)
│   ├── Doctors.cshtml + .cs        # Doctor directory (public)
│   ├── DoctorDetail.cshtml + .cs   # Doctor profile view (public)
│   ├── Privacy.cshtml + .cs        # Privacy policy page
│   └── Shared/
│       └── _Layout.cshtml          # Main layout template
├── wwwroot/
│   ├── css/                        # Custom styles
│   ├── js/                         # JavaScript files
│   ├── lib/                        # Bootstrap 5.3, jQuery
│   └── images/
│       ├── patients/               # 8 patient profile photos
│       └── doctors/                # 8 doctor profile photos
├── Program.cs                      # Application entry point and configuration
├── appsettings.json               # Configuration including connection strings
├── martinwebapp.csproj            # Project file with NuGet packages
├── healthcare.db                   # SQLite database (to be migrated)
└── README.md                       # Project documentation
```

### Technology Stack Analysis

**Backend Framework:**
- ASP.NET Core 11.0 RC 3 (Razor Pages)
- C# .NET 11.0
- Entity Framework Core 11.0.0-rc.3.26522.148

**Data Access:**
- Entity Framework Core with Code-First approach
- Database.EnsureCreated() for initialization
- Seed data for 8 patients and 8 doctors

**Authentication & Security:**
- ASP.NET Core Authentication (Cookie-based)
- Microsoft.AspNetCore.Authentication.Cookies
- [Authorize] attributes on protected pages
- 8-hour session expiration with sliding expiration

**NuGet Dependencies:**
```xml
- Azure.AI.OpenAI (2.1.0) - For future AI integration
- Microsoft.EntityFrameworkCore.Sqlite (11.0.0-rc.3.26522.148) - Local database provider
- Microsoft.EntityFrameworkCore.SqlServer (11.0.0-rc.3.26522.148) - Azure SQL provider
- Microsoft.EntityFrameworkCore.Design (11.0.0-rc.3.26522.148) - Design-time tools
```

**Frontend:**
- Bootstrap 5.3
- Vanilla JavaScript (ES6+)
- Razor Pages (CSHTML)
- Custom CSS with gradients and animations

### Component Classification

#### Primary Components:

**1. Web Application Layer**
- **Type:** ASP.NET Core Razor Pages Web App
- **Technology:** .NET 11.0 RC 3 Runtime
- **Build:** `dotnet build` → `dotnet publish`
- **Port:** 5258 (development)
- **Hosting Recommendation:** Azure App Service (Linux with compatible .NET 11.0 runtime)

**2. Database Layer**
- **Current:** SQLite (healthcare.db) - File-based
- **Target:** Azure SQL Database
- **Schema:** 2 main entities (Patients, Doctors) with seed data
- **Access Pattern:** EF Core with connection pooling
- **Hosting Recommendation:** Azure SQL Database (Basic/S0 tier for start)

**3. Static Content**
- **Type:** Images, CSS, JavaScript
- **Location:** `/wwwroot` directory
- **Size:** ~16 profile photos + Bootstrap libraries
- **Hosting:** Served directly by App Service

### Dependency Analysis

#### Internal Dependencies:

1. **Web App → Database**
   - **Pattern:** EF Core with SQLite (current) / SQL Server (target)
   - **Connection String:** `appsettings.json` → `DefaultConnection`
   - **Usage:** All page models (Patients, Doctors, Login)

2. **Authentication → Database**
   - **Pattern:** Direct EF Core queries
   - **Dependency:** Doctor entity for username/password validation
   - **Flow:** Login → DbContext → Doctor lookup → Claims creation

3. **Pages → Layout**
   - **Pattern:** Razor Pages with shared layout
   - **Dependency:** `_Layout.cshtml` for all pages

#### External Dependencies:

1. **Azure.AI.OpenAI (2.1.0)**
   - **Status:** Package included, not currently used in code
   - **Purpose:** Future AI/ML integration
   - **Azure Service:** Azure OpenAI Service (future)

2. **Bootstrap 5.3**
   - **Source:** CDN or local (/wwwroot/lib/bootstrap)
   - **Usage:** All pages for responsive UI

3. **randomuser.me API**
   - **Status:** Used during initial setup for photo downloads
   - **Current Usage:** None (photos downloaded and committed)

#### Configuration Dependencies:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=healthcare.db"  // Will change to Azure SQL
  }
}
```

**Environment Variables Needed for Azure:**
- `SQLCONNSTR_DefaultConnection` - Azure SQL connection string
- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- (Optional) `APPLICATIONINSIGHTS_CONNECTION_STRING` - For monitoring

### Communication Patterns

#### Current Architecture:

```
User Browser
    ↓ HTTPS
Web App (Razor Pages)
    ↓ EF Core
SQLite Database (healthcare.db)
```

#### Target Azure Architecture:

```
User Browser
    ↓ HTTPS
Azure App Service (Web App)
    ↓ SQL Connection (Encrypted)
Azure SQL Database
    ↓ (Monitoring)
Application Insights
```

### Data Model Summary

**Patients Table (8 seeded records):**
- Id, FirstName, LastName, DateOfBirth, Gender
- PhoneNumber, Email, Address
- BloodType, Allergies, MedicalHistory
- RegistrationDate, PhotoUrl

**Doctors Table (8 seeded records):**
- Id, FirstName, LastName, Specialization, LicenseNumber
- PhoneNumber, Email, Department
- YearsOfExperience, Education, Bio
- JoinDate, PhotoUrl

**Authentication:**
- Username format: First 2 chars of first name + 6 chars of last name
- Password: `Doctor123!` (all doctors)
- 8 active doctor accounts available

### Application Entry Points

**Main Entry Point:**
- `Program.cs` - Application startup and configuration
- Bootstrap → Services → Middleware → DbContext.EnsureCreated()

**Web Endpoints:**
- `/` - Homepage (CONTOSO CLINIC)
- `/Login` - Authentication page
- `/Logout` - Sign-out handler
- `/Patients` - Patient list (requires authentication)
- `/PatientDetail/{id:int}` - Patient detail popup (requires authentication)
- `/Doctors` - Doctor directory (public)
- `/DoctorDetail/{id:int}` - Doctor profile popup (public)
- `/Privacy` - Privacy policy

### Build and Deployment Configuration

**Build Process:**
```bash
dotnet restore
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
```

**Runtime Requirements:**
- Compatible .NET 11.0 Runtime (Linux or Windows)
- SQL Server connection (Azure SQL)
- HTTPS support
- Environment variables for configuration

### Discovery Phase Completion Checklist

- [x] Complete inventory of all application artifacts documented
- [x] All major application components identified (Web App, Database, Static Assets)
- [x] Component technologies documented (.NET 11.0 RC 3, EF Core, Razor Pages)
- [x] Dependencies mapped (Web → DB, Auth → DB, External packages)
- [x] External services catalogued (Azure OpenAI future integration)
- [x] Configuration requirements identified (Connection strings, env vars)
- [x] Entry points and endpoints documented
- [x] Data model and schema documented

**Status:** ✅ **Discovery Phase Complete** - Ready for Architecture Planning

---

## Phase 2: Architecture Planning and Azure Service Selection

### Azure Service Mapping

| Component | Current Technology | Azure Service | SKU/Tier | Rationale |
|---|---|---|---|---|
| Web Application | ASP.NET Core 11.0 Razor Pages | **Azure App Service (Linux)** | B1 (Basic) | Requires a compatible .NET 11.0 runtime; built-in scaling, ZIP deployment, and SSL. No containerization needed when runtime prerequisites are met. |
| Database | SQLite (healthcare.db) | **Azure SQL Database** | Basic (5 DTU) | SQL Server compatibility with EF Core, minimal code changes, managed backups, geo-redundancy options. Perfect for healthcare data. |
| Monitoring | None | **Application Insights** | Standard | Real-time monitoring, performance tracking, exception logging, user analytics. Essential for production healthcare app. |
| Secrets Management | appsettings.json | **Azure Key Vault** | Standard | Secure storage for connection strings, API keys. Healthcare data compliance requirement. |
| Static Assets | /wwwroot | **App Service (included)** | - | Served directly by App Service. No separate CDN needed for small image count (16 photos). |

### Hosting Strategy Summary

#### Primary Web Application
- **Selected Service:** Azure App Service (Linux)  
- **Runtime Stack:** .NET 11.0 (`dotnet|11`; verify RC compatibility before deployment)
- **Deployment Method:** Azure Developer CLI (azd) with ZIP deploy  
- **Scaling:** Manual scaling (Basic tier), can upgrade to Standard for auto-scale  
- **HTTPS:** Automatic with App Service managed certificate  
- **Custom Domain:** Supported (optional)  
- **Health Checks:** Built-in health endpoint monitoring  
- **Deployment Slots:** Not available in Basic tier (upgrade to Standard if needed for staging)

**Rationale:** ASP.NET Core Razor Pages apps run natively on App Service without containerization overhead. The Linux App Service plan provides cost-effective hosting with integrated deployment tools and automatic OS patching.

#### Database Strategy
- **Selected Service:** Azure SQL Database  
- **Service Tier:** Basic (5 DTUs) for development/test, S0 (10 DTUs) for production  
- **Compute Model:** DTU-based (predictable cost)  
- **Backup:** Automated (7-day retention in Basic tier)  
- **Geo-Replication:** Optional upgrade path  
- **Connection Security:** SSL/TLS encryption required, firewall rules configured  
- **Authentication:** SQL Authentication (username/password) via connection string

**Migration Path:**
1. Update NuGet package from `Microsoft.EntityFrameworkCore.Sqlite` to `Microsoft.EntityFrameworkCore.SqlServer`
2. Update connection string in `appsettings.json` and Azure App Service configuration
3. Modify `Program.cs` to use `UseSqlServer()` instead of `UseSqlite()`
4. Run EF Core migrations or use `Database.EnsureCreated()` with seed data
5. Deploy database schema and seed data on first deployment

#### Monitoring and Observability
- **Selected Service:** Application Insights  
- **Log Analytics Workspace:** Shared workspace for all logs  
- **Data Retention:** 90 days (default)  
- **Telemetry:** Automatic request tracking, dependency tracking, exception logging  
- **Alerts:** Configure for application errors, high response times, availability issues  
- **Integration:** SDK-less attachment via App Service configuration

#### Secrets and Configuration Management
- **Selected Service:** Azure Key Vault  
- **Access Pattern:** App Service managed identity with Key Vault references  
- **Stored Secrets:**
  - SQL Database connection string
  - (Future) Azure OpenAI API key
  - Any third-party API keys

**Configuration Pattern:**
```
App Service Configuration → Key Vault Reference → Actual Secret
Example: @Microsoft.KeyVault(SecretUri=https://kv-name.vault.azure.net/secrets/SqlConnectionString/)
```

### Containerization Strategy

**Decision:** ✗ **No Containerization Required**

**Rationale:**
- ASP.NET Core Razor Pages apps are ideal candidates for direct App Service deployment
- No microservices architecture requiring orchestration
- Single monolithic web application
- Simpler deployment and troubleshooting without Docker complexity
- Lower operational overhead
- Faster cold start times compared to container instances

**Future Consideration:** If the application evolves into microservices or requires containerization for CI/CD consistency, Azure Container Apps would be the recommended path.

### Resource Group Organization

**Resource Naming Convention:** `{resource-type}-{app-name}-{environment}-{region}`

**Example:**
- Resource Group: `rg-contosohealth-prod-eastus`
- App Service Plan: `asp-contosohealth-prod-eastus`
- App Service: `app-contosohealth-prod-eastus`
- SQL Server: `sql-contosohealth-prod-eastus`
- SQL Database: `sqldb-healthcare-prod`
- Key Vault: `kv-contoso-prod-eastus`
- Application Insights: `appi-contosohealth-prod-eastus`
- Log Analytics: `log-contosohealth-prod-eastus`

**Resource Grouping:** All resources in single resource group for simplified management and cost tracking.

### Networking Architecture

```
Internet (HTTPS)
      ↓
Azure Front Door (Optional - for multi-region)
      ↓
Azure App Service (Linux - .NET 11.0)
      ↓ (Virtual Network Integration - Optional)
      ↓ (Service Endpoint / Private Endpoint - Optional)
Azure SQL Database
      ↓
Application Insights / Log Analytics
```

**Network Security:**
- **App Service:** Public endpoint with HTTPS enforcement (can be upgraded to private endpoint)
- **SQL Database:** Firewall rules to allow Azure services + specific IP ranges
- **Key Vault:** Firewall rules to allow App Service managed identity
- **TLS:** 1.2+ enforced on all connections

**Future Enhancements:**
- Azure Front Door for global distribution and WAF
- Private Endpoints for SQL Database (requires Standard tier or above)
- Virtual Network integration for enhanced security isolation

### Data Migration Strategy

**Phase 1: Schema Migration**
1. Generate SQL Server script from SQLite schema (manual or via EF Core migration)
2. Create Azure SQL Database with proper collation and compatibility level
3. Apply schema to Azure SQL

**Phase 2: Data Migration**
1. Export data from SQLite (patients, doctors tables)
2. Import into Azure SQL using:
   - SQL Server Migration Assistant (SSMA)
   - Or manual INSERT scripts
   - Or EF Core seed data on first run

**Phase 3: Connection String Update**
```json
// SQLite (Current)
"DefaultConnection": "Data Source=healthcare.db"

// Azure SQL (Target)
"DefaultConnection": "Server=tcp:sql-contosohealth-prod-eastus.database.windows.net,1433;Initial Catalog=sqldb-healthcare-prod;Persist Security Info=False;User ID=healthadmin;Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

### Integration Patterns

**Current Integrations:** None (self-contained application)

**Future Integration Points:**
1. **Azure OpenAI Service** (Package already included: Azure.AI.OpenAI 2.1.0)
   - Medical insights generation
   - Patient record summarization
   - Diagnostic assistance

2. **Azure Storage Account** (Optional)
   - Medical document uploads
   - Blob storage for larger files

3. **Azure Communication Services** (Optional)
   - Email notifications to patients
   - SMS reminders for appointments

### Cost Estimation (Monthly - USD)

| Resource | SKU/Tier | Estimated Cost |
|---|---|---|
| App Service Plan (B1 Linux) | 1 instance | $13.14 |
| Azure SQL Database (Basic) | 5 DTUs | $4.90 |
| Application Insights | Standard (1 GB) | Free tier / $2.30 |
| Key Vault | Standard | $0.03 (per 10k operations) |
| Log Analytics Workspace | Pay-as-you-go | $2-5 (5 GB free) |
| **Total Estimated** | | **~$20-25/month** |

*Note: Prices subject to Azure region and current pricing. Development/test workloads may qualify for reduced pricing.*

### Deployment Architecture

```
Developer Machine
      ↓
azd up (Azure Developer CLI)
      ↓
Azure Resource Manager
      ├─→ Deploy Bicep Templates → Create Resources
      └─→ Deploy Application Code → App Service
      
Post-Deployment:
- Database schema created (EnsureCreated)
- Seed data populated
- Application Insights connected
- Key Vault references configured
```

### Infrastructure as Code File Checklist

Based on the selected Azure services, the following Bicep files need to be generated:

#### Core Files (Always Required)
- [ ] `./infra/main.bicep` - Primary deployment template (resource group scope)
- [ ] `./infra/main.parameters.json` - Parameter file with defaults
- [ ] `./infra/abbreviations.json` - Resource naming abbreviations

#### Service-Specific Modules
- [ ] `./infra/resources.bicep` - All resources orchestration
- [ ] `./infra/modules/app-service.bicep` - App Service Plan + Web App (Linux, .NET 11.0)
- [ ] `./infra/modules/database.bicep` - Azure SQL Server + Database
- [ ] `./infra/modules/monitoring.bicep` - Log Analytics + Application Insights
- [ ] `./infra/modules/keyvault.bicep` - Key Vault for secrets management

**Total Bicep files to generate: 8**

### Docker File Generation Checklist

**Decision:** ✗ **No Docker files required**

The ASP.NET Core Razor Pages application will deploy directly to Azure App Service
using a compatible native .NET 11.0 runtime without containerization, after the
runtime prerequisites have been verified.

**Rationale:**
- Simpler deployment model for monolithic web apps
- Native App Service features (auto-scaling, deployment slots, easy rollback)
- No container registry or orchestration overhead
- Faster cold start and lower resource usage

### Security and Compliance Considerations

**Healthcare Data Protection:**
- HTTPS enforced on all endpoints
- SQL Database encryption at rest (default)
- SQL Database encryption in transit (TLS 1.2+)
- Key Vault for sensitive configuration
- Azure RBAC for resource access control
- Application-level authentication for doctor access

**Recommended Security Enhancements:**
1. Enable Azure AD authentication for SQL Database (replace SQL auth)
2. Implement rate limiting on Login page
3. Add CAPTCHA for brute-force protection
4. Configure Private Endpoints for SQL Database
5. Enable Azure Security Center recommendations
6. Implement audit logging for data access

### Phase 2 Completion Checklist

- [x] Azure service selected for each component with rationale
- [x] Hosting strategies defined (App Service for web, Azure SQL for data)
- [x] Containerization decision documented (not required)
- [x] Data storage and migration strategy planned
- [x] Resource group organization strategy defined
- [x] Network architecture and security planned
- [x] Integration patterns identified
- [x] Cost estimation provided
- [x] IaC file checklist generated (8 Bicep files)
- [x] Docker file checklist generated (none required)
- [x] Security considerations documented

**Status:** ✅ **Architecture Planning Complete** - Ready for File Generation

---

## Phase 3: File Generation

*(To be completed with azure.yaml, Bicep templates, etc.)*

---

## Phase 4: Validation

*(To be completed with azd_project_validation tool)*

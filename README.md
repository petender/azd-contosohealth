# Contoso Clinic - Healthcare Management System

A modern healthcare management web application built with ASP.NET Core, featuring patient records management, doctor profiles, and secure authentication.

## 🏥 Overview

Contoso Clinic is a comprehensive healthcare management system designed to manage patient information and doctor profiles with secure access control. The application provides an intuitive interface for healthcare providers to access and manage patient data efficiently.

## 🏗️ Architecture

### Application Architecture
- **Pattern**: MVC (Model-View-Controller) with Razor Pages
- **Database Access**: Entity Framework Core with Code-First approach
- **Authentication**: Cookie-based authentication with role-based access control
- **Design Pattern**: Repository pattern through EF Core DbContext

### Technology Stack

#### Backend
- **Framework**: ASP.NET Core 11.0 RC 3 (Razor Pages)
- **Language**: C# .NET 11.0
- **ORM**: Entity Framework Core 11.0.0-rc.3.26522.148
- **Database**: SQLite 3.x
- **Authentication**: ASP.NET Core Identity (Cookie Authentication)
- **Additional Libraries**:
  - Azure.AI.OpenAI 2.1.0
  - Microsoft.EntityFrameworkCore.Sqlite 11.0.0-rc.3.26522.148
  - Microsoft.EntityFrameworkCore.SqlServer 11.0.0-rc.3.26522.148
  - Microsoft.EntityFrameworkCore.Design 11.0.0-rc.3.26522.148

### .NET 11 development upgrade

This branch uses the published .NET 11 RC 3 release, not stable .NET 11.
The SDK is selected by [global.json](global.json): install SDK
`11.0.100-rc.3.26522.148` (later patches in the same feature band are allowed).
The existing Razor Pages, cookie authentication, static asset pipeline, and
`Database.EnsureCreated()` initialization remain in place. SQL Server query
compatibility is explicitly kept at level 150, the previous EF Core default,
instead of adopting EF Core 11's new default of 160. SQL authentication remains
unchanged; Entra ID SQL authentication would additionally require
`Microsoft.Data.SqlClient.Extensions.Azure` with SqlClient 7.

The existing Razor Pages antiforgery tokens are retained. Login continues to
require a valid token, including for cross-origin form submissions; the upgrade
does not disable or replace this protection.

Before production adoption, update the SDK and EF Core packages to stable
.NET 11 releases and repeat build, publish, and database integration checks.
Development work is tracked in [issue #17](https://github.com/petender/azd-contosohealth/issues/17).

#### Frontend
- **UI Framework**: Bootstrap 5.3
- **JavaScript**: Vanilla JavaScript (ES6+)
- **CSS**: Custom CSS with CSS3 Gradients and Animations
- **Icons**: Unicode Emojis
- **Responsive Design**: Mobile-first approach using Bootstrap grid system

#### Database
- **Database Engine**: SQLite (Local Development) / Azure SQL Database (Production)
- **Local Database File**: `healthcare.db`
- **Azure Database**: Managed Azure SQL Database with automated backups
- **Schema Management**: EF Core with Database.EnsureCreated()
- **Data Seeding**: Built-in seed data for 8 patients and 8 doctors

## ☁️ Azure Deployment

This application is Azure-ready and can be deployed using the Azure Developer CLI (azd).

### Azure Architecture

```
Internet (HTTPS)
      ↓
Azure App Service (Linux .NET 11.0)
      ├─→ Azure SQL Database
      ├─→ Azure Key Vault (Secrets)
      └─→ Application Insights (Monitoring)
```

### Azure Resources

| Resource | Purpose | SKU/Tier |
|---|---|---|
| **App Service Plan** | Hosts the web application | B1 (Basic) Linux |
| **App Service** | ASP.NET Core 11.0 Razor Pages | - |
| **Azure SQL Database** | Patient and doctor data storage | Basic (5 DTU) |
| **Azure SQL Server** | Logical server for SQL Database | - |
| **Key Vault** | Secure secrets storage | Standard |
| **Application Insights** | Application monitoring and telemetry | Standard |
| **Log Analytics Workspace** | Centralized logging | Pay-as-you-go |

### Prerequisites for Azure Deployment

- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- Azure subscription with appropriate permissions
- .NET 11 RC 3 SDK selected by [global.json](global.json)

Before running `azd up`, `azd provision`, or `azd deploy`, confirm the Linux
runtime catalog contains the configured `dotnet|11` stack:

```bash
az webapp list-runtimes --os linux --output table
```

The catalog reported `dotnet|11` on November 5, 2026. This is not verification
of the runtime installed in a particular region or existing web app. Confirm
that the target host runs a runtime compatible with this RC 3 application.
Do not deploy the framework-dependent build if a compatible runtime is
unavailable; do not substitute an older runtime stack. No Azure deployment
has been performed as part of this development upgrade.

### Quick Deploy to Azure

1. **Login to Azure**
   ```bash
   azd auth login
   ```

2. **Initialize the environment** (first time only)
   ```bash
   azd init
   ```
   - When prompted, choose an environment name (e.g., `contosohealth-prod`)
   - Select your Azure subscription
   - Choose an Azure region (e.g., `eastus`, `westus2`)

3. **Deploy to Azure**
   ```bash
   azd up
   ```
   This single command will:
   - Provision all Azure resources using Bicep templates
   - Create the Azure SQL Database with proper configuration
   - Deploy the application code to App Service
   - Configure Application Insights monitoring
   - Set up Key Vault with connection strings
   - Return the application URL when complete

4. **Access your deployed application**
   - The URL will be displayed at the end of deployment
   - Format: `https://app-{unique-id}.azurewebsites.net`

### Infrastructure as Code

All Azure infrastructure is defined in Bicep templates located in the `./infra` directory:

```
infra/
├── main.bicep                      # Main deployment template
├── main.parameters.json            # Parameter values
├── abbreviations.json              # Resource naming conventions
└── modules/
    ├── app-service.bicep           # App Service Plan and Web App
    ├── database.bicep              # Azure SQL Server and Database
    ├── monitoring.bicep            # Application Insights and Log Analytics
    └── keyvault.bicep              # Azure Key Vault
```

**Key Infrastructure Features:**
- **Resource Tagging**: All resources tagged with `SecurityControl: Ignore` and `azd-env-name`
- **Managed Identity**: App Service uses system-assigned managed identity for Key Vault access
- **Key Vault Integration**: SQL connection string stored securely in Key Vault
- **HTTPS Only**: TLS 1.2+ enforced on all connections
- **Azure SQL Firewall**: Configured to allow Azure services

### Azure Developer CLI Commands

```bash
# Deploy infrastructure and code
azd up

# Deploy only infrastructure (Bicep templates)
azd provision

# Deploy only application code
azd deploy

# Monitor the application
azd monitor

# View environment variables
azd env get-values

# Clean up all Azure resources
azd down
```

### Database Migration to Azure SQL

The application automatically detects the database provider:

**Local Development (SQLite):**
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=healthcare.db"
}
```

**Azure Production (SQL Server):**
- Connection string retrieved from Key Vault via App Service configuration
- Format: `@Microsoft.KeyVault(SecretUri=...)`
- Automatic schema creation on first deployment
- Seed data populated automatically

### Monitoring and Observability

**Application Insights** is automatically configured for:
- Request telemetry and performance tracking
- Exception logging and error tracking
- Dependency tracking (SQL queries, HTTP calls)
- Custom metrics and events
- Real-time metrics and alerts

**Access Monitoring:**
```bash
azd monitor
```
This opens the Azure Portal with your Application Insights dashboard.

### Cost Estimation

**Monthly Azure Costs (USD):**
- App Service Plan (B1): ~$13
- Azure SQL Database (Basic): ~$5
- Application Insights: Free tier (1 GB/month)
- Key Vault: ~$0.03 per 10k operations
- **Total: ~$18-20/month**

*Actual costs may vary by region and usage patterns.*

### Security and Compliance

**Azure Security Features:**
- ✅ HTTPS enforced on all endpoints
- ✅ SQL Database encryption at rest
- ✅ SQL Database TLS 1.2+ for data in transit
- ✅ Key Vault for secrets management
- ✅ Managed identities for service-to-service auth
- ✅ Azure RBAC for resource access control
- ✅ Application-level authentication (Cookie-based)

### Troubleshooting Azure Deployment

**View application logs:**
```bash
az webapp log tail --name app-{unique-id} --resource-group rg-{env-name}
```

**Access Key Vault secrets:**
```bash
az keyvault secret list --vault-name kv-{unique-id}
```

**Test SQL Database connectivity:**
- Check firewall rules in Azure Portal
- Verify App Service has managed identity
- Confirm Key Vault access policy

### Scaling and Performance

**Vertical Scaling (Upgrade SKU):**
- Basic (B1) → Standard (S1): Auto-scaling capabilities
- Basic (B1) → Premium (P1V2): Deployment slots, advanced networking

**Horizontal Scaling:**
- Upgrade to Standard or Premium tier
- Configure auto-scale rules based on CPU/memory
- Multiple instances for high availability

**Database Scaling:**
- Basic (5 DTU) → Standard (S0-S12): More compute power
- DTU-based → vCore-based: Fine-grained resource control
- Add geo-replication for disaster recovery

## 📊 Data Models

### Patient Entity
- **Properties**: Id, FirstName, LastName, DateOfBirth, Gender, PhoneNumber, Email, Address, BloodType, Allergies, MedicalHistory, RegistrationDate, PhotoUrl
- **Features**: Complete medical profile with photo support

### Doctor Entity
- **Properties**: Id, FirstName, LastName, Specialization, LicenseNumber, PhoneNumber, Email, Department, YearsOfExperience, Education, Bio, JoinDate, PhotoUrl
- **Features**: Professional credentials with detailed bio and photo

## 🚀 Getting Started

### Prerequisites
- .NET SDK `11.0.100-rc.3.26522.148` selected by [global.json](global.json)
- VS Code with C# tooling, or an IDE supporting the .NET 11 SDK
- Git (for version control)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd azd-contosohealth
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore src/contosohealth.sln
   ```

3. **Build the project**
   ```bash
   dotnet build src/contosohealth.sln --configuration Release
   ```

4. **Run the application**
   ```bash
   dotnet run --project src/contosohealth.csproj
   ```

5. **Access the application**
   - Open your browser and navigate to: `http://localhost:5258`

### Database Setup

The database is automatically created on first run with seed data:
- **8 Pre-configured Patients** with complete medical records and photos
- **8 Pre-configured Doctors** with specializations and credentials
- SQLite database file: `healthcare.db` (auto-generated in project root)

## 🔐 Authentication & Security

### Security Features
- Cookie-based authentication with 8-hour session expiration
- Role-based access control (Doctor role)
- Protected patient data routes
- Secure password validation
- HTTPS redirection (configurable)

### Login Credentials

**Username Format**: First 2 letters of first name + First 6 letters of last name (no spaces)

**Password**: `Doctor123!` (for all doctors)

#### Available Doctor Accounts:

| Username    | Doctor Name           | Specialization            | Department          |
|-------------|----------------------|---------------------------|---------------------|
| `machen`    | Dr. Margaret Chen    | Cardiology                | Cardiology          |
| `jawilson`  | Dr. James Wilson     | Orthopedic Surgery        | Orthopedics         |
| `prpatel`   | Dr. Priya Patel      | Pediatrics                | Pediatrics          |
| `rotaylor`  | Dr. Robert Taylor    | Neurology                 | Neurology           |
| `amrodrig`  | Dr. Amanda Rodriguez | Dermatology               | Dermatology         |
| `mikim`     | Dr. Michael Kim      | Internal Medicine         | Internal Medicine   |
| `rafoster`  | Dr. Rachel Foster    | Obstetrics and Gynecology | OB/GYN              |
| `thhughes`  | Dr. Thomas Hughes    | Emergency Medicine        | Emergency Department|

**Example Login**:
- Username: `machen`
- Password: `Doctor123!`

## 📁 Project Structure

```
martinwebapp/
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext with seed data
│   ├── Patient.cs                  # Patient entity model
│   ├── Doctor.cs                   # Doctor entity model
│   └── Employee.cs                 # Legacy employee model
├── Pages/
│   ├── Index.cshtml                # Homepage with clinic information
│   ├── Login.cshtml                # Authentication page
│   ├── Logout.cshtml               # Logout handler
│   ├── Patients.cshtml             # Patient list (protected)
│   ├── PatientDetail.cshtml        # Patient detail popup (protected)
│   ├── Doctors.cshtml              # Doctor directory
│   ├── DoctorDetail.cshtml         # Doctor profile popup
│   ├── Privacy.cshtml              # Privacy policy
│   └── Shared/
│       └── _Layout.cshtml          # Main layout template
├── wwwroot/
│   ├── css/                        # Stylesheets
│   ├── js/                         # JavaScript files
│   ├── lib/                        # Third-party libraries
│   └── images/
│       ├── patients/               # Patient profile photos
│       └── doctors/                # Doctor profile photos
├── Program.cs                      # Application configuration & startup
├── appsettings.json               # Configuration settings
├── healthcare.db                   # SQLite database (generated)
└── README.md                       # This file
```

## 🎨 Key Features

### 1. **Modern Homepage**
- Professional healthcare organization design
- Hero section with hospital imagery
- Service cards (Emergency Care, Diagnostics, Specialized Care, etc.)
- Medical specialties showcase
- Statistics section (50+ doctors, 15K+ patients served)
- Responsive layout

### 2. **Patient Management** (Protected)
- Comprehensive patient list with photos
- Searchable and sortable patient records
- Detailed patient profiles with medical history
- Blood type, allergies, and medication tracking
- Popup window for detailed patient information

### 3. **Doctor Directory** (Public)
- Complete doctor roster with specializations
- Professional profile photos
- Educational background and credentials
- Years of experience tracking
- Popup window for detailed doctor bios

### 4. **Secure Authentication**
- Professional login interface
- Username format validation
- Session management with automatic expiration
- Visual feedback for login errors
- User display in navigation bar when authenticated

### 5. **Responsive Design**
- Mobile-first approach
- Bootstrap 5 grid system
- Touch-friendly interface
- Optimized for all screen sizes

## 🎨 Design Highlights

### Color Scheme
- **Primary Gradient**: Purple (#667eea → #764ba2) - Used for patient-related features
- **Secondary Gradient**: Green (#11998e → #38ef7d) - Used for doctor-related features
- **Background**: Light gray (#f8f9fa) for content sections
- **Text**: Dark gray (#2c3e50) for optimal readability

### UI Components
- Gradient buttons with hover effects
- Card-based layouts with shadows
- Animated transitions and transforms
- Circular profile images with colored borders
- Badge-based status indicators

## 🔧 Configuration

### Database Connection
Located in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=healthcare.db"
  }
}
```

### Authentication Settings
Configured in `Program.cs`:
- Session Duration: 8 hours
- Sliding Expiration: Enabled
- Login Path: `/Login`
- Logout Path: `/Logout`

## 📝 Development

### Adding New Features

1. **Add a new entity model** in `Data/` folder
2. **Update ApplicationDbContext** to include the new DbSet
3. **Create Razor Pages** in `Pages/` folder
4. **Add navigation links** in `_Layout.cshtml`
5. **Apply migrations** (if needed) or rebuild database

### Running in Development Mode

```bash
dotnet run --project src/contosohealth.csproj --environment Development
```

### Building for Production

```bash
dotnet publish src/contosohealth.csproj -c Release -r linux-x64 --self-contained false -o ./publish
```

## 🧪 Testing the Application

Run the repeatable smoke checks against a local instance with a fresh SQLite
database. Supply a seeded demo doctor account when prompted:

```powershell
.\scripts\Test-Smoke.ps1 -BaseUri http://localhost:5258 -Credential (Get-Credential)
```

The checks cover the eight seeded doctors and patients, public and protected
pages, detail lookups, static assets, login failure, antiforgery and cross-origin rejection,
successful login, and logout. Use only a local development instance, not a
production environment.

1. **Homepage**: Navigate to `http://localhost:5258`
2. **View Doctors**: Click "Doctors" in navigation or "Our Doctors" button
3. **Login**: Click "Login" and use credentials from the table above
4. **View Patients**: After login, click "Patients" in navigation
5. **View Details**: Click any patient or doctor name to open detail popup
6. **Logout**: Click "Logout" in navigation when authenticated

## 📦 Dependencies

### NuGet Packages
```xml
<PackageReference Include="Azure.AI.OpenAI" Version="2.1.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="11.0.0-rc.3.26522.148" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="11.0.0-rc.3.26522.148" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="11.0.0-rc.3.26522.148" />
```

## 🐛 Known Issues

- HTTPS port redirection warning in development (non-critical)
- Database file must be deleted manually to reset seed data
- Profile photos sourced from randomuser.me API (external dependency)

## 🔮 Future Enhancements

- [ ] Appointment scheduling system
- [ ] Patient-doctor messaging
- [ ] Medical records upload
- [ ] Prescription management
- [ ] Integration with Azure OpenAI for medical insights
- [ ] Advanced search and filtering
- [ ] Report generation
- [ ] Multi-factor authentication
- [ ] Audit logging

## 📄 License

This project is for demonstration purposes.

## 👥 Contributors

Developed with GitHub Copilot assistance.

## 📞 Support

For issues or questions, please refer to the project documentation or contact the development team.

---

**Last Updated**: January 9, 2026  
**Version**: 1.0.0  
**Framework**: ASP.NET Core 11.0 RC 3

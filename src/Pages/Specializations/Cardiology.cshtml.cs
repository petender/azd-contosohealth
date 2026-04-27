using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using contosohealth.Data;

namespace contosohealth.Pages.Specializations;

public class CardiologyModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public int DoctorCount { get; set; }
    public List<CardiologyService> Services { get; set; } = new List<CardiologyService>();
    public List<string> Conditions { get; set; } = new List<string>();
    public List<CardiologyProcedure> Procedures { get; set; } = new List<CardiologyProcedure>();

    public CardiologyModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        // Get count of cardiologists from database
        DoctorCount = await _context.Doctors
            .CountAsync(d => d.Specialization == "Cardiology");

        // If no cardiologists in DB, show a default count for demo purposes
        if (DoctorCount == 0)
        {
            DoctorCount = 8;
        }

        // Initialize services offered
        Services = new List<CardiologyService>
        {
            new CardiologyService
            {
                Name = "Preventive Cardiology",
                Description = "Risk assessment, lifestyle counseling, and early detection programs to prevent heart disease before it starts."
            },
            new CardiologyService
            {
                Name = "Diagnostic Imaging",
                Description = "Advanced cardiac imaging including echocardiography, CT angiography, and cardiac MRI for accurate diagnosis."
            },
            new CardiologyService
            {
                Name = "Interventional Cardiology",
                Description = "Minimally invasive procedures including angioplasty, stent placement, and catheter-based treatments."
            },
            new CardiologyService
            {
                Name = "Electrophysiology",
                Description = "Diagnosis and treatment of heart rhythm disorders, including pacemaker and defibrillator implantation."
            },
            new CardiologyService
            {
                Name = "Heart Failure Management",
                Description = "Comprehensive care programs for patients with heart failure, including medication management and device therapy."
            },
            new CardiologyService
            {
                Name = "Cardiac Rehabilitation",
                Description = "Structured exercise and education programs to help patients recover from heart events and surgery."
            }
        };

        // Initialize conditions treated
        Conditions = new List<string>
        {
            "Coronary Artery Disease",
            "Heart Failure",
            "Atrial Fibrillation",
            "Hypertension",
            "Heart Valve Disease",
            "Cardiomyopathy",
            "Peripheral Artery Disease",
            "Congenital Heart Defects",
            "Angina Pectoris",
            "Arrhythmias",
            "Pericarditis",
            "Aortic Aneurysm"
        };

        // Initialize procedures
        Procedures = new List<CardiologyProcedure>
        {
            new CardiologyProcedure
            {
                Name = "Echocardiogram",
                Type = "Diagnostic",
                Duration = "30-60 min",
                Recovery = "None"
            },
            new CardiologyProcedure
            {
                Name = "Cardiac Catheterization",
                Type = "Diagnostic",
                Duration = "1-2 hours",
                Recovery = "4-6 hours"
            },
            new CardiologyProcedure
            {
                Name = "Coronary Angioplasty",
                Type = "Interventional",
                Duration = "1-3 hours",
                Recovery = "1-2 days"
            },
            new CardiologyProcedure
            {
                Name = "Stent Placement",
                Type = "Interventional",
                Duration = "1-2 hours",
                Recovery = "1-2 days"
            },
            new CardiologyProcedure
            {
                Name = "Pacemaker Implantation",
                Type = "Surgical",
                Duration = "1-2 hours",
                Recovery = "2-4 weeks"
            },
            new CardiologyProcedure
            {
                Name = "Ablation Therapy",
                Type = "Interventional",
                Duration = "2-4 hours",
                Recovery = "1-3 days"
            },
            new CardiologyProcedure
            {
                Name = "Stress Test",
                Type = "Diagnostic",
                Duration = "30-60 min",
                Recovery = "None"
            },
            new CardiologyProcedure
            {
                Name = "Holter Monitoring",
                Type = "Diagnostic",
                Duration = "24-48 hours",
                Recovery = "None"
            }
        };
    }
}

public class CardiologyService
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CardiologyProcedure
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Recovery { get; set; } = string.Empty;
}

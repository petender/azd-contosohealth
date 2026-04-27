using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using contosohealth.Data;

namespace contosohealth.Pages;

public class SpecializationsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public List<SpecializationInfo> Specializations { get; set; } = new List<SpecializationInfo>();

    public SpecializationsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        // Get distinct specializations from doctors and build department info
        var doctorSpecializations = await _context.Doctors
            .GroupBy(d => d.Specialization)
            .Select(g => new { Specialization = g.Key, Count = g.Count() })
            .ToListAsync();

        // Define specialization details (would typically come from a database)
        var specializationDetails = new Dictionary<string, (string Description, string Icon, int ServiceCount, int YearsEstablished, string PageUrl)>
        {
            ["Cardiology"] = ("Comprehensive heart and cardiovascular care including diagnostics, interventional procedures, and preventive cardiology.", "❤️", 15, 25, "/Specializations/Cardiology"),
            ["Neurology"] = ("Expert diagnosis and treatment of disorders affecting the brain, spinal cord, and nervous system.", "🧠", 12, 20, "/Specializations/Neurology"),
            ["Orthopedics"] = ("Complete musculoskeletal care from sports injuries to joint replacements and spine surgery.", "🦴", 18, 22, "/Specializations/Orthopedics"),
            ["Pediatrics"] = ("Dedicated healthcare for infants, children, and adolescents in a child-friendly environment.", "👶", 14, 30, "/Specializations/Pediatrics"),
            ["Dermatology"] = ("Medical and cosmetic skin care services including treatment for conditions and aesthetic procedures.", "✨", 10, 18, "/Specializations/Dermatology"),
            ["Oncology"] = ("Comprehensive cancer care with advanced treatment options, clinical trials, and supportive services.", "🎗️", 20, 15, "/Specializations/Oncology"),
            ["Gastroenterology"] = ("Specialized care for digestive system disorders including endoscopy and liver disease management.", "🫁", 11, 19, "/Specializations/Gastroenterology"),
            ["Pulmonology"] = ("Expert respiratory care for conditions affecting the lungs and breathing.", "💨", 9, 17, "/Specializations/Pulmonology"),
            ["Endocrinology"] = ("Treatment for hormone-related conditions including diabetes, thyroid disorders, and metabolic diseases.", "⚗️", 8, 16, "/Specializations/Endocrinology"),
            ["Ophthalmology"] = ("Complete eye care services from routine exams to advanced surgical procedures.", "👁️", 13, 24, "/Specializations/Ophthalmology")
        };

        foreach (var spec in doctorSpecializations)
        {
            if (specializationDetails.TryGetValue(spec.Specialization, out var details))
            {
                Specializations.Add(new SpecializationInfo
                {
                    Name = spec.Specialization,
                    Description = details.Description,
                    Icon = details.Icon,
                    DoctorCount = spec.Count,
                    ServiceCount = details.ServiceCount,
                    YearsEstablished = details.YearsEstablished,
                    PageUrl = details.PageUrl
                });
            }
            else
            {
                // For specializations not in our predefined list
                Specializations.Add(new SpecializationInfo
                {
                    Name = spec.Specialization,
                    Description = $"Quality healthcare services in {spec.Specialization}.",
                    Icon = "🏥",
                    DoctorCount = spec.Count,
                    ServiceCount = 8,
                    YearsEstablished = 10,
                    PageUrl = $"/Specializations/{spec.Specialization.Replace(" ", "")}"
                });
            }
        }

        // Sort by name
        Specializations = Specializations.OrderBy(s => s.Name).ToList();
    }
}

public class SpecializationInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public int ServiceCount { get; set; }
    public int YearsEstablished { get; set; }
    public string PageUrl { get; set; } = string.Empty;
}

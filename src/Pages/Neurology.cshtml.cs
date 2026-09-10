using contosohealth.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace contosohealth.Pages;

public class NeurologyModel : PageModel
{
    private const string DepartmentName = "Neurology";
    private readonly ApplicationDbContext _context;

    public List<Doctor> Doctors { get; private set; } = [];

    public NeurologyModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        Doctors = await _context.Doctors
            .AsNoTracking()
            .Where(doctor => doctor.Department == DepartmentName)
            .OrderBy(doctor => doctor.LastName)
            .ToListAsync();
    }
}

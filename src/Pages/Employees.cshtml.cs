using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace contosohealth.Pages;

public class EmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string HiringDate { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
}

public class EmployeesModel : PageModel
{
    private readonly ILogger<EmployeesModel> _logger;
    private readonly IWebHostEnvironment _environment;

    public List<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();

    public EmployeesModel(ILogger<EmployeesModel> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnGet()
    {
        var jsonPath = Path.Combine(_environment.ContentRootPath, "sampledata.json");
        if (System.IO.File.Exists(jsonPath))
        {
            var jsonData = System.IO.File.ReadAllText(jsonPath);
            Employees = JsonSerializer.Deserialize<List<EmployeeDto>>(jsonData) ?? new List<EmployeeDto>();
        }
    }
}

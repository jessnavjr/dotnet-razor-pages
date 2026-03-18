using DotNetRazorPages.Data;
using DotNetRazorPages.Data.Entities;
using DotNetRazorPages.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DotNetRazorPages.Services;

public class ApplicationDbInitializer(ApplicationDbContext dbContext) : IApplicationDbInitializer
{
    private const int MinimumEmployees = 200;

    private static readonly string[] FirstNames =
    [
        "Ava", "Noah", "Mia", "Liam", "Emma", "Olivia", "Ethan", "Sophia", "Mason", "Isabella",
        "Lucas", "Amelia", "Elijah", "Harper", "James", "Evelyn", "Benjamin", "Charlotte", "Henry", "Scarlett",
        "Alexander", "Grace", "Michael", "Chloe", "Daniel", "Nora", "Jack", "Lily", "Owen", "Zoe"
    ];

    private static readonly string[] LastNames =
    [
        "Carter", "Mitchell", "Nguyen", "Patel", "Reed", "Brooks", "Diaz", "Murphy", "Bennett", "Foster",
        "Turner", "Parker", "Price", "Hayes", "Powell", "Simmons", "Jenkins", "Ward", "Coleman", "Long",
        "Hughes", "Ross", "Bailey", "Gray", "James", "Watson", "Perry", "Sanders", "Butler", "Barnes"
    ];

    private static readonly string[] JobTitles =
    [
        "Engineering Manager",
        "Senior Software Engineer",
        "Product Designer",
        "Data Analyst",
        "HR Specialist",
        "QA Engineer",
        "DevOps Engineer",
        "Support Engineer",
        "Technical Writer",
        "Business Analyst"
    ];

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var employees = await dbContext.Employees
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

        EnsureUniqueNameCombinations(employees);

        if (employees.Count < MinimumEmployees)
        {
            var startDate = new DateTime(2018, 1, 1);
            var employeesToAdd = MinimumEmployees - employees.Count;
            var employeesToInsert = new List<Employee>(capacity: employeesToAdd);

            var usedNameCombinations = BuildUsedNameCombinationSet(dbContext.Employees.Local);
            var nameCombinationEnumerator = GetNameCombinations().GetEnumerator();

            for (var i = 1; i <= employeesToAdd; i++)
            {
                var employeeNumber = dbContext.Employees.Local.Count + i;
                var (firstName, lastName) = GetNextAvailableNameCombination(nameCombinationEnumerator, usedNameCombinations);
                var jobTitle = JobTitles[(employeeNumber - 1) % JobTitles.Length];
                var email = $"{firstName.ToLowerInvariant()}.{lastName.ToLowerInvariant()}{employeeNumber:000}@company.com";

                employeesToInsert.Add(new Employee
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    JobTitle = jobTitle,
                    HireDate = startDate.AddDays(employeeNumber * 14),
                    IsActive = employeeNumber % 7 != 0
                });
            }

            await dbContext.Employees.AddRangeAsync(employeesToInsert, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await EnsureUniqueNameIndexAsync(cancellationToken);
    }

    private async Task EnsureUniqueNameIndexAsync(CancellationToken cancellationToken)
    {
        if (!string.Equals(dbContext.Database.ProviderName, "Microsoft.EntityFrameworkCore.SqlServer", StringComparison.Ordinal))
        {
            return;
        }

        await dbContext.Database.ExecuteSqlRawAsync(
            @"IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Employees_FirstName_LastName'
      AND object_id = OBJECT_ID(N'[dbo].[Employees]')
)
BEGIN
    CREATE UNIQUE INDEX [UX_Employees_FirstName_LastName]
    ON [dbo].[Employees]([FirstName], [LastName]);
END",
            cancellationToken);
    }

    private static void EnsureUniqueNameCombinations(List<Employee> employees)
    {
        var usedNameCombinations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var nameCombinationEnumerator = GetNameCombinations().GetEnumerator();

        foreach (var employee in employees)
        {
            var existingKey = BuildNameKey(employee.FirstName, employee.LastName);
            if (usedNameCombinations.Add(existingKey))
            {
                continue;
            }

            var (firstName, lastName) = GetNextAvailableNameCombination(nameCombinationEnumerator, usedNameCombinations);
            employee.FirstName = firstName;
            employee.LastName = lastName;
        }
    }

    private static HashSet<string> BuildUsedNameCombinationSet(IEnumerable<Employee> employees)
    {
        var usedNameCombinations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var employee in employees)
        {
            usedNameCombinations.Add(BuildNameKey(employee.FirstName, employee.LastName));
        }

        return usedNameCombinations;
    }

    private static (string FirstName, string LastName) GetNextAvailableNameCombination(
        IEnumerator<(string FirstName, string LastName)> combinations,
        HashSet<string> usedNameCombinations)
    {
        while (combinations.MoveNext())
        {
            var candidate = combinations.Current;
            var key = BuildNameKey(candidate.FirstName, candidate.LastName);

            if (usedNameCombinations.Add(key))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("No unique name combinations are available for employee seeding.");
    }

    private static IEnumerable<(string FirstName, string LastName)> GetNameCombinations()
    {
        foreach (var firstName in FirstNames)
        {
            foreach (var lastName in LastNames)
            {
                yield return (firstName, lastName);
            }
        }
    }

    private static string BuildNameKey(string firstName, string lastName)
    {
        return $"{firstName}|{lastName}";
    }
}
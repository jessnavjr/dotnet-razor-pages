using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DotNetRazorPages.Services.Abstractions;

namespace DotNetRazorPages.Web.Pages;

public class IndexModel(IGreetingService greetingService) : PageModel
{

    public string GreetingMessage { get; private set; } = string.Empty;

    public void OnGet()
    {
        GreetingMessage = greetingService.GetGreeting();
    }
}

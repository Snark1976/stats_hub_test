using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StatsHub_Web.Pages
{
    public class DailyStatsModel(IConfiguration configuration) : PageModel
    {
        public required string ApiUrl { get; set; }

        public void OnGet()
        {
            ApiUrl = configuration["ApiUrl"]!;
            ViewData["Title"] = "Ежедневная статистика";
        }
    }
}
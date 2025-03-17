using Microsoft.AspNetCore.Mvc;

namespace DynamicView.Controllers
{
    public class DynamicReportListController : Controller
    {
        // Initial action when the application starts
        public IActionResult PromptPage()
        {
            return View();  // Show the prompt page
        }

        // Action to redirect to the main page after user input
        [HttpPost]
        public IActionResult RedirectToReport(int reportId)
        {
            // Redirect to the main page after the user enters the Report ID
            return RedirectToAction("Index", "DynamicData", new { id = reportId });
        }
    }
}

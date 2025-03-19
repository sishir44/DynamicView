using DynamicView.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace DynamicView.Controllers
{
    public class DynamicReportListController : Controller
    {
        private readonly DbService _dbService;

        public DynamicReportListController(DbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IActionResult> Index()
        {
            DynamicReportListModel model = new DynamicReportListModel();
            try
            {
                var reportList = await _dbService.GetDynamicReportsAsync();
                model.ReportList = reportList.ToDictionary(r => r.ReportID, r => r.ReportName); ;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return View(model);
        }
    }
}

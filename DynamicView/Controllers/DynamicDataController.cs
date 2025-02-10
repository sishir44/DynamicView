using DynamicView.Models;
using Microsoft.AspNetCore.Mvc;

public class DynamicDataController : Controller
{
    private readonly DbService _dbService;

    public DynamicDataController(DbService dbService)
    {
        _dbService = dbService;
    }


    public async Task<IActionResult> Index(string filterColumn, string filterValue)
    {
        DynamicDataModel model = null;

        try
        {
            var employeeNames = await _dbService.GetEmployeeNamesAsync();
           // var ttlCount = await _dbService.Total(filterColumn, filterColumn);
            //var ttlCount = await _dbService.Total();

            model = new DynamicDataModel
            {
                FieldNames = await _dbService.GetFieldNamesAsync(),
                TableData = await _dbService.GetTableDataAsync(filterColumn, filterValue),
                TotalSum = await _dbService.TotalSum(filterColumn, filterColumn),
                Store = employeeNames
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return View(model); 
    }

}

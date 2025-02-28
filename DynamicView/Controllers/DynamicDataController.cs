using DynamicView.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;

public class DynamicDataController : Controller
{
    private readonly DbService _dbService;

    public DynamicDataController(DbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<IActionResult> Index(int reportId = 2)
    {
        DynamicDataModel model = new DynamicDataModel();

        bool isMobile = IsMobileRequest(HttpContext);
        model.isMobile = isMobile;

        try
        {
            //model.FieldNames = await _dbService.GetFieldNamesAsync(reportId);
            // model.GetDynamicReport = await _dbService.GetDynamicReportAsync(reportId);
            //if (model.GetDynamicReport?.Count > 0)
            //{
            //    var firstRow = model.GetDynamicReport[0]?.FirstOrDefault();
            //    var secondRow = model.GetDynamicReport[1]?.FirstOrDefault();
            //    model.ReportName = firstRow["RepName"]?.ToString();
            //    var storedProcName = firstRow["DataProc"]?.ToString();
            //    model.TableData = await _dbService.DataAsync(storedProcName);
            //    var totalCountProc = firstRow["GrandTotalProc"]?.ToString();
            //    model.TotalSum = await _dbService.TotalAsync(totalCountProc);
            //    model.FieldNames = model.GetDynamicReport[1]
            //                        .Select(row => row["AttributeName"].ToString().Trim('[', ']')).ToList();
            //    model.isFixedCol = model.GetDynamicReport[1]
            //                        .Where(row => row["isFixed"].ToString().Trim() == "True").Select(row => row["isFixed"].ToString()).ToList();
            //    model.isFilterCol = model.GetDynamicReport[1]
            //                        .Where(row => row["isFilter"].ToString().Trim() == "True").Select(row => row["isFilter"].ToString()).ToList();
            //    model.isSubTotalCol = model.GetDynamicReport[1]
            //                        .Where(row => row["isSubTotal"].ToString().Trim() == "True").Select(row => row["isSubTotal"].ToString()).ToList();
            //}

            var result = _dbService.GetDynamicReportNew(reportId);
            var firstTable = result.resultSet1[0]; // data col header and color
            var secondTable = result.resultSet2[0]; // alias name for col
            var thirdTable = result.resultSet3[0]; // isFixed for Desktop
            var fourthTable = result.resultSet4[0]; // is Fixed for Mobile
            var fifthTable = result.resultSet5[0];  // isFilter
            var sixthTable = result.resultSet6[0]; // subtotal

            var firstRow = firstTable?.Rows.Cast<DataRow>().FirstOrDefault();
            // get col header, data and color
            if (firstRow != null)
            {
                model.ReportName = firstRow["RepName"]?.ToString();
                model.Color = firstRow["ColorCode"]?.ToString();
                model.AlternateRowColor = firstRow["AlternateRowColor"]?.ToString();

                if (firstRow.Table.Columns.Contains("DataProc"))
                {
                    var storedProcName = firstRow["DataProc"]?.ToString();
                    var parameters = new Dictionary<string, object>();
                    foreach (DataColumn column in firstRow.Table.Columns)
                    {
                        parameters[column.ColumnName] = firstRow[column.ColumnName];
                    }
                    model.TableData = await _dbService.DataAsync(storedProcName);
                }

                if (firstRow.Table.Columns.Contains("GrandTotalProc"))
                {
                    var totalCountProc = firstRow["GrandTotalProc"]?.ToString();
                    var parameters = new Dictionary<string, object>();
                    foreach (DataColumn column in firstRow.Table.Columns)
                    {
                        parameters[column.ColumnName] = firstRow[column.ColumnName];
                    }
                    model.TotalSum = await _dbService.TotalAsync(totalCountProc, parameters);
                }
            }

            // get the alias name for table
            if (secondTable != null)
            {
                model.FieldNames = secondTable.Columns.Contains("Alias") ? secondTable.AsEnumerable().Select(row => row["Alias"]?.ToString().Trim('[', ']'))
                                  .Where(value => !string.IsNullOrEmpty(value)).ToList() : new List<string>();
            }

            if (!isMobile)
            {
                // get the fixed col for Desktop
                if (thirdTable != null)
                {
                    model.isFixedCol = thirdTable.AsEnumerable()
                                     .Where(row => row.Field<bool?>("isFixed") == true).Select(row => row["Alias"]?.ToString().Trim('[', ']'))
                                     .Where(attributeName => !string.IsNullOrEmpty(attributeName)).ToList();
                }
            }
            else
            {
                // get the fixed col for mobile
                if (fourthTable != null)
                {
                    model.isFixedCol = fourthTable.AsEnumerable()
                                     .Where(row => row.Field<bool?>("isFixedM") == true).Select(row => row["Alias"]?.ToString().Trim('[', ']'))
                                     .Where(attributeName => !string.IsNullOrEmpty(attributeName)).ToList();
                }
            }
            // get the filter 
            if (fifthTable != null)
            {
                model.isFilterCol = fifthTable.AsEnumerable()
                                 .Where(row => row.Field<bool?>("isFilter") == true).Select(row => row["Alias"]?.ToString().Trim('[', ']'))
                                 .Where(attributeName => !string.IsNullOrEmpty(attributeName)).ToList();
            }
            // sub total
            if (sixthTable != null)
            {
                model.isSubTotalCol = sixthTable.AsEnumerable()
                                 .Where(row => row.Field<bool?>("isSubTotal") == true).Select(row => row["Alias"]?.ToString().Trim('[', ']'))
                                 .Where(attributeName => !string.IsNullOrEmpty(attributeName)).ToList();
            }


            if (sixthTable != null && sixthTable.Columns.Contains("SubTotalProc") && sixthTable.Rows.Count > 0)
            {
                model.SubTotalResults = new List<Dictionary<string, object>>(); // ✅ Correct initialization

                foreach (DataRow row in fifthTable.Rows)
                {
                    string storedProcName = row["SubTotalProc"]?.ToString();

                    if (!string.IsNullOrEmpty(storedProcName))
                    {
                        // Execute the stored procedure using Data Access Layer
                        var resultData = await _dbService.ExecuteStoredProcedureAsync(storedProcName);

                        if (storedProcName == "GetFct_StoreNumberTotalMMM")
                        {
                            model.TotalMMM = resultData;
                        }
                        if (storedProcName == "GetFct_StoreNumberTotalTM")
                        {
                            model.TotalTM = resultData;
                        }
                    }
                }
            }


            // Generate alphabetic column names
            int NumberOfColumns = model.FieldNames.Count;
            List<string> columnNames = CommonNameHelper(NumberOfColumns);

            // Pass column names to the view
            model.firstRowColumn = columnNames; // Add a property in your model for ColumnNames

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        return View(model);
    }

    // Helper method to generate column names (Excel-style)
    public List<string> CommonNameHelper(int numberOfColumns)
    {
        List<string> columnNames = new List<string>();

        for (int i = 1; i <= numberOfColumns; i++)
        {
            columnNames.Add(GetAlphabeticalSequence(i));
        }
        return columnNames;
    }

    // Convert number to Excel-style alphabetical sequence (A, B, C, AA, AB, etc.)
    public string GetAlphabeticalSequence(int num)
    {
        string result = "";
        while (num > 0)
        {
            num--;
            result = (char)('A' + (num % 26)) + result;
            num /= 26;
        }
        return result;
    }

    public bool IsMobileRequest(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        return userAgent.Contains("Mobi") || userAgent.Contains("Android") || userAgent.Contains("IPhone");
    }
}

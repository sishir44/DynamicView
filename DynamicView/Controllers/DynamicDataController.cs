using DynamicView.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
            var firstTable = result.resultSet1[0];
            var secondTable = result.resultSet2[0];
            var fifthTable = result.resultSet5[0];

            var firstRow = firstTable?.Rows.Cast<DataRow>().FirstOrDefault();
            if (firstRow != null)
            {
                model.ReportName = firstRow["RepName"]?.ToString();
                model.Color = firstRow["ColorCode"]?.ToString();

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

            if (secondTable != null) { 
                model.FieldNames = secondTable.Columns.Contains("Alias") ? secondTable.AsEnumerable().Select(row => row["Alias"]?.ToString().Trim('[', ']'))
                                 .Where(value => !string.IsNullOrEmpty(value)).ToList() : new List<string>();

                model.isFixedCol = secondTable.AsEnumerable()
                                                .Where(row => row.Field<bool?>("isFixed") == true).Select(row => row["Alias"]?.ToString().Trim('[', ']'))
                                                .Where(attributeName => !string.IsNullOrEmpty(attributeName)).ToList();

                model.isFilterCol = secondTable.AsEnumerable()
                                                .Where(row => row.Field<bool?>("isFilter") == true).Select(row => row["Alias"]?.ToString().Trim('[', ']'))
                                                .Where(attributeName => !string.IsNullOrEmpty(attributeName)).ToList();

                model.isSubTotalCol = secondTable.AsEnumerable()
                                                .Where(row => row.Field<bool?>("isSubTotal") == true).Select(row => row["Alias"]?.ToString().Trim('[', ']'))
                                                .Where(attributeName => !string.IsNullOrEmpty(attributeName)).ToList();
            }


            if (fifthTable != null && fifthTable.Columns.Contains("SubTotalProc") && fifthTable.Rows.Count > 0)
            {
                model.SubTotalResults = new List<Dictionary<string, object>>(); // ✅ Correct initialization

                foreach (DataRow row in fifthTable.Rows)
                {
                    string storedProcName = row["SubTotalProc"]?.ToString();

                    if (!string.IsNullOrEmpty(storedProcName))
                    {
                        // Execute the stored procedure using Data Access Layer
                        var resultData = await _dbService.ExecuteStoredProcedureAsync(storedProcName);

                        if(storedProcName == "GetFct_StoreNumberTotalMMM")
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
}

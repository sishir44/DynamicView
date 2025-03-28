using DynamicView.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using System.Xml;

public class DynamicDataController : Controller
{
    private readonly DbService _dbService;

    public DynamicDataController(DbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<IActionResult> Index(int reportId,string userId = null, string selectedDate = null, string columnId = null, string selectedValue = null)
    {
        DynamicDataModel model = new DynamicDataModel();

        bool isMobile = IsMobileRequest(HttpContext);
        model.isMobile = isMobile;

        try
        {
            // execute all the table from GetDynamicReport stp
            var resultSet = await Task.Run(() => _dbService.Fnc_GetDynamicReport(reportId));

            model.FirstTable = resultSet[0];  // Data column header and color
            model.SecondTable = resultSet[1]; // AttributeName, alias name for columns
            model.ThirdTable = resultSet[2];  // isFixed for Desktop
            model.FourthTable = resultSet[3]; // isFixed for Mobile
            model.FifthTable = resultSet[4];  // isFilter Column
            model.SixthTable = resultSet[5];  // Subtotal Column
            model.SeventhTable = resultSet[6]; // No of Decimals 
            model.EighthTable = resultSet[7]; // Color code Column
            model.NinthTable = resultSet[8];  // Percentage ratio 
            model.ShowCardTbl = resultSet[9]; // ShowCard for Grand Total

            // get col header, data and color
            var firstRow = model.FirstTable?.Rows.Count > 0 ? model.FirstTable.Rows[0] : null;
            if (firstRow != null)
            {
                model.ReportName = firstRow["RepName"]?.ToString();
                model.Color = firstRow["ColorCode"]?.ToString();
                model.AlternateRowColor = firstRow["AlternateRowColor"]?.ToString();

                // Function to prepare dynamic parameters
                Dictionary<string, object> PrepareParameters()
                {
                    var parameters = new Dictionary<string, object>
                    {
                        { "@UserID", string.IsNullOrEmpty(userId) ? DBNull.Value : userId },
                        { "@Date", string.IsNullOrEmpty(selectedDate) ? DBNull.Value : selectedDate }
                    };

                    if (!string.IsNullOrEmpty(columnId) && !string.IsNullOrEmpty(selectedValue))
                    {
                        parameters.TryAdd($"@{columnId}", selectedValue);
                    }
                    return parameters;
                }

                // Handle DataProc (Load Table Data)
                if (firstRow.Table.Columns.Contains("DataProc"))
                {
                    var storedProcName = firstRow["DataProc"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(storedProcName))
                    {
                        model.TableData = await _dbService.Fnc_LoadDataAsync(storedProcName, PrepareParameters());
                    }
                }

                // Handle GrandTotalProc (Load Total Sum)
                if (firstRow.Table.Columns.Contains("GrandTotalProc"))
                {
                    var totalCountProc = firstRow["GrandTotalProc"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(totalCountProc) && totalCountProc != "N/A")
                    {
                        model.GrandTotalSum = await _dbService.Fnc_GrandTotalAsync(totalCountProc, PrepareParameters());
                    }

                }
            }

            // get the AttributeName name for table
            if (model.SecondTable != null)
            {
                // Extract FieldNames(AttributeName to handle the data)
                model.FieldNames = model.SecondTable.Columns.Contains("AttributeName")
                    ? model.SecondTable.AsEnumerable()
                        .Select(row => row["AttributeName"]?.ToString())
                        .Where(value => !string.IsNullOrWhiteSpace(value))
                        .Select(value => value.Replace("[", "").Replace("]", ""))
                        .ToList()
                    : new List<string>();

                // Extract FieldNamesAlias(Alias Name for the column heading)
                model.FieldNamesAlias = model.SecondTable.Columns.Contains("Alias")
                    ? model.SecondTable.AsEnumerable()
                        .Select(row => row["Alias"]?.ToString())
                        .Where(value => !string.IsNullOrWhiteSpace(value))
                        .Select(value => value.Replace("[", "").Replace("]", ""))
                        .ToList()
                    : new List<string>();
            }

            if (!isMobile)
            {
                // Get the fixed columns for Desktop
                model.isFixedCol = model.ThirdTable != null
                    ? model.ThirdTable.AsEnumerable()
                        .Where(row => row.Field<bool?>("isFixed") == true)
                        .Select(row => row["AttributeName"]?.ToString()?.Replace("[", "").Replace("]", ""))
                        .Where(attributeName => !string.IsNullOrWhiteSpace(attributeName))
                        .ToList()
                    : new List<string>();
            }
            else
            {
                // Get the fixed columns for Mobile
                model.isFixedCol = model.FourthTable != null
                    ? model.FourthTable.AsEnumerable()
                        .Where(row => row.Field<bool?>("isFixedM") == true)
                        .Select(row => row["AttributeName"]?.ToString()?.Replace("[", "").Replace("]", ""))
                        .Where(attributeName => !string.IsNullOrWhiteSpace(attributeName))
                        .ToList()
                    : new List<string>();
            }

            // Get the filter columns
            if (model.FifthTable != null)
            {
                model.isFilterCol = model.FifthTable.AsEnumerable()
                    .Where(row => row.Field<bool?>("isFilter") == true)
                    .Select(row => row["AttributeName"]?.ToString()?.Replace("[", "").Replace("]", ""))
                    .Where(attributeName => !string.IsNullOrWhiteSpace(attributeName))
                    .ToList();
            }


            // Get the subtotal columns
            if (model.SixthTable != null)
            {
                var subTotalRows = model.SixthTable.AsEnumerable()
                    .Where(row => row.Field<bool?>("isSubTotal") == true)
                    .ToList();

                if (subTotalRows.Any())
                {
                    model.SubTotalResults = new List<Dictionary<string, object>>();

                    foreach (var row in subTotalRows)
                    {
                        string storedProcName = row["SubTotalProc"]?.ToString();
                        int subTotalOrder = row.Field<short?>("SubTotalOrder") ?? 0;

                        if (!string.IsNullOrEmpty(storedProcName))
                        {
                            var parameters = new Dictionary<string, object>
                            {
                                { "@DateParam", subTotalOrder == 1 ? new DateTime(2025, 01, 06) :
                                                subTotalOrder == 0 ? new DateTime(2025, 02, 03) :
                                                DBNull.Value },
                                { "@UserID", string.IsNullOrEmpty(userId) ? (object)DBNull.Value : userId }
                            };

                            try
                            {
                                var resultData = await _dbService.Fnc_SubTotalProcedureAsync(storedProcName, parameters);
                                if (subTotalOrder == 1) model.TotalMMM = resultData;
                                else model.TotalTM = resultData;

                                model.SubTotalResults.AddRange(resultData);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error executing {storedProcName}: {ex.Message}");
                            }
                        }
                    }
                }
            }

            // No of decimal
            if (model.SeventhTable != null)
            {
                model.NoOfDecimal = model.SeventhTable.AsEnumerable()
                    .Where(row => row.Field<short?>("NoOfDecimal") > 0)
                    .Select(row => new
                    {   
                        AttributeName = row["AttributeName"]?.ToString().Trim('[', ']'),
                        NoOfDecimal = row.Field<short?>("NoOfDecimal")
                    })
                    .Where(result => !string.IsNullOrEmpty(result.AttributeName))
                    .ToDictionary(result => result.AttributeName, result => result.NoOfDecimal);
            }

            // color column and data
            if (model.EighthTable != null)
            {
                // Get the list of attribute names (if present)
                model.ColorAliasName = model.EighthTable.Columns.Contains("AttributeName")
                    ? model.EighthTable.AsEnumerable()
                        .Select(row => row["AttributeName"]?.ToString().Trim('[', ']'))
                        .Where(value => !string.IsNullOrEmpty(value))
                        .ToList()
                    : new List<string>();

                // Iterate through rows to get color values and related data
                foreach (DataRow row in model.EighthTable.Rows)
                {
                    model.ColorValue1 = row["ColorCode1"] != DBNull.Value ? Convert.ToSingle(row["ColorCode1"]) : 0f;
                    model.ColorCode1 = row["ColorValue1"] != DBNull.Value ? row["ColorValue1"].ToString() : null;

                    model.ColorValue2 = row["ColorCode2"] != DBNull.Value ? Convert.ToSingle(row["ColorCode2"]) : 0f;  // Assigning default value of 0 if DBNull
                    model.ColorCode2 = row["ColorValue2"] != DBNull.Value ? row["ColorValue2"].ToString() : null;

                    model.ColorValue3 = row["ColorCode3"] != DBNull.Value ? Convert.ToSingle(row["ColorCode3"]) : 0f;  // Assigning default value of 0 if DBNull
                    model.ColorCode3 = row["ColorValue3"] != DBNull.Value ? row["ColorValue3"].ToString() : null;

                    model.FixedCard = row["FixedCard"] != DBNull.Value ? Convert.ToBoolean(row["FixedCard"]) : false;

                }
            }

            // Percentage Ratio - Extracting isPercent values
            if (model.NinthTable != null)
            {
                model.isPercent = model.NinthTable.AsEnumerable()
                            .Where(row => row.Field<bool?>("isPercent") == true)
                            .ToDictionary(
                                row => row["AttributeName"]?.ToString().Trim('[', ']'), // Key: AttributeName
                                row => row.Field<bool?>("isPercent") // Value: isPercent
                            );
            }

            // Show Card for Grand Total
            if (model.ShowCardTbl != null)
            {
                model.ShowCard = model.ShowCardTbl.AsEnumerable()
                            .Where(row => row.Field<bool?>("ShowCard") == true)
                            .Select(row => new
                            {
                                AttributeName = row["AttributeName"]?.ToString().Trim('[', ']'),
                                ShowCard = row.Field<bool?>("ShowCard")
                            })
                            .Where(result => !string.IsNullOrEmpty(result.AttributeName))
                            .ToDictionary(result => result.AttributeName, result => result.ShowCard);
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
        try
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
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public bool IsMobileRequest(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        return userAgent.Contains("Mobi") || userAgent.Contains("Android") || userAgent.Contains("IPhone");
    }
}


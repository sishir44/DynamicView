﻿using DynamicView.Models;
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

    public async Task<IActionResult> Index(int reportId,string userId = null, string selectedDate = null, string columnId = null, string selectedValue = null)
    {
        DynamicDataModel model = new DynamicDataModel();

        bool isMobile = IsMobileRequest(HttpContext);
        model.isMobile = isMobile;

        try
        {
            var result = _dbService.GetDynamicReportNew(reportId);
            var firstTable = result.resultSet1[0]; // data col header and color
            var secondTable = result.resultSet2[0]; // AttributeName, alisa name for col
            var thirdTable = result.resultSet3[0]; // isFixed for Desktop
            var fourthTable = result.resultSet4[0]; // is Fixed for Mobile
            var fifthTable = result.resultSet5[0];  // isFilter
            var sixthTable = result.resultSet6[0]; // subtotal
            var sevenTable = result.resultSet7[0]; // No of Decimal
            var eightTable = result.resultSet8[0]; // color code
            var nineTable = result.resultSet9[0]; // percentage ratio 
            var ShowCardTbl = result.resultSet10[0]; // showCard for grand Total

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

                    parameters.Add("@UserID", string.IsNullOrEmpty(userId) ? DBNull.Value : userId);
                    parameters.Add("@Date", string.IsNullOrEmpty(selectedDate) ? DBNull.Value : selectedDate);

                    // Dynamically add parameters based on columnId and selectedValue
                    if (!string.IsNullOrEmpty(columnId) && !string.IsNullOrEmpty(selectedValue))
                    {
                        parameters.Add($"@{columnId}", selectedValue);
                    }
                    model.TableData = await _dbService.DataAsync(storedProcName, parameters);
                }

                if (firstRow.Table.Columns.Contains("GrandTotalProc"))
                {
                    var totalCountProc = firstRow["GrandTotalProc"]?.ToString();
                    
                    if (totalCountProc != "N/A" && totalCountProc != "")
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("@UserID", string.IsNullOrEmpty(userId) ? DBNull.Value : userId);
                        parameters.Add("@Date", string.IsNullOrEmpty(selectedDate) ? DBNull.Value : selectedDate);

                        // Dynamically add parameters based on columnId and selectedValue
                        if (!string.IsNullOrEmpty(columnId) && !string.IsNullOrEmpty(selectedValue))
                        {
                            parameters.Add($"@{columnId}", selectedValue);
                        }
                        model.TotalSum = await _dbService.TotalAsync(totalCountProc, parameters);
                    }

                }
            }

            // get the AttributeName name for table
            if (secondTable != null)
            {
                model.FieldNames = secondTable.Columns.Contains("AttributeName") ? secondTable.AsEnumerable().Select(row => row["AttributeName"]?.ToString().Trim('[', ']'))
                                  .Where(value => !string.IsNullOrEmpty(value)).ToList() : new List<string>();
                model.FieldNamesAlias = secondTable.Columns.Contains("Alias") ? secondTable.AsEnumerable().Select(row => row["Alias"]?.ToString().Trim('[', ']'))
                                  .Where(value => !string.IsNullOrEmpty(value)).ToList() : new List<string>();
            }

            if (!isMobile)
            {
                // get the fixed col for Desktop
                if (thirdTable != null)
                {
                    model.isFixedCol = thirdTable.AsEnumerable()
                                     .Where(row => row.Field<bool?>("isFixed") == true).Select(row => row["AttributeName"]?.ToString().Trim('[', ']'))
                                     .Where(attributeName => !string.IsNullOrEmpty(attributeName)).ToList();
                }
            }
            else
            {
                // get the fixed col for mobile
                if (fourthTable != null)
                {
                    model.isFixedCol = fourthTable.AsEnumerable()
                                     .Where(row => row.Field<bool?>("isFixedM") == true).Select(row => row["AttributeName"]?.ToString().Trim('[', ']'))
                                     .Where(attributeName => !string.IsNullOrEmpty(attributeName)).ToList();
                }
            }
            // get the filter 
            if (fifthTable != null)
            {
                model.isFilterCol = fifthTable.AsEnumerable()
                                 .Where(row => row.Field<bool?>("isFilter") == true).Select(row => row["AttributeName"]?.ToString().Trim('[', ']'))
                                 .Where(attributeName => !string.IsNullOrEmpty(attributeName)).ToList();
            }
            // sub total
            if (sixthTable != null)
            {
                model.isSubTotalCol = sixthTable.AsEnumerable()
                                 .Where(row => row.Field<bool?>("isSubTotal") == true).Select(row => row["AttributeName"]?.ToString().Trim('[', ']'))
                                 .Where(attributeName => !string.IsNullOrEmpty(attributeName)).ToList();
            }


            if (sixthTable != null && sixthTable.Columns.Contains("SubTotalProc") && sixthTable.Rows.Count > 0)
            {
                model.SubTotalResults = new List<Dictionary<string, object>>(); 

                foreach (DataRow row in sixthTable.Rows)
                {
                    string storedProcName = row["SubTotalProc"]?.ToString();

                    if (!string.IsNullOrEmpty(storedProcName))
                    {
                        // Execute the stored procedure using Data Access Layer
                        var resultData = await _dbService.SubTotalProcedureAsync(storedProcName);

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

            // No of decimal
            if (sevenTable != null)
            {
                model.NoOfDecimal = sevenTable.AsEnumerable().Where(row => row.Field<short?>("NoOfDecimal") > 0)
                            .Select(row => new
                            {
                                AttributeName = row["AttributeName"]?.ToString().Trim('[', ']'),
                                NoOfDecimal = row.Field<short?>("NoOfDecimal")
                            }).Where(result => !string.IsNullOrEmpty(result.AttributeName)).ToDictionary(result => result.AttributeName, result => result.NoOfDecimal);
            }

            // color column and data
            if (eightTable != null)
            {
                model.ColorAliasName = eightTable.Columns.Contains("AttributeName") ? eightTable.AsEnumerable().Select(row => row["AttributeName"]?.ToString().Trim('[', ']'))
                                  .Where(value => !string.IsNullOrEmpty(value)).ToList() : new List<string>();
            }
            if (result.resultSet8 != null && result.resultSet8.Count > 0)
            {
                foreach (DataRow row in eightTable.Rows)
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

            // Percentage Rtaio
            if (nineTable != null)
            {
                model.isPercent = nineTable.AsEnumerable().Where(row => row.Field<bool?>("isPercent") == true)
                            .Select(row => new
                            {
                                AttributeName = row["AttributeName"]?.ToString().Trim('[', ']'),
                                isPercent = row.Field<bool?>("isPercent")
                            }).Where(result => !string.IsNullOrEmpty(result.AttributeName)).ToDictionary(result => result.AttributeName, result => result.isPercent);
            }

            //show card for grand total
            if (ShowCardTbl != null)
            {
                model.ShowCard = ShowCardTbl.AsEnumerable().Where(row => row.Field<bool?>("ShowCard") == true)
                            .Select(row => new
                            {
                                AttributeName = row["AttributeName"]?.ToString().Trim('[', ']'),
                                ShowCard = row.Field<bool?>("ShowCard")
                            }).Where(result => !string.IsNullOrEmpty(result.AttributeName)).ToDictionary(result => result.AttributeName, result => result.ShowCard);
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


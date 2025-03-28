using DynamicView.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

public class DbService
{
    private readonly string _connectionString;

    public DbService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // Report Name List
    public async Task<IEnumerable<DynamicReportListModel>> Fnc_GetDynamicReportListAsync()
    {
        var reports = new List<DynamicReportListModel>();

        using (var connection = new SqlConnection(_connectionString))
        {
            try
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("Dyn_DynamicReportList", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var report = new DynamicReportListModel
                            {
                                ReportID = reader.GetInt32(reader.GetOrdinal("ReportID")),
                                ReportName = reader.GetString(reader.GetOrdinal("RepName"))
                            };

                            reports.Add(report);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        return reports;
    }

    // Execute all table from GetDynamicReport Stp
    //public (List<DataTable> resultSet1, List<DataTable> resultSet2, List<DataTable> resultSet3, List<DataTable> resultSet4, List<DataTable> resultSet5, List<DataTable> resultSet6, List<DataTable> resultSet7, List<DataTable> resultSet8, List<DataTable> resultSet9, List<DataTable> resultSet10) Fnc_GetDynamicReport(int reportID)
    //{
    //    using (SqlConnection connection = new SqlConnection(_connectionString))
    //    {
    //        SqlCommand command = new SqlCommand("GetDynamicReport", connection);
    //        command.CommandType = CommandType.StoredProcedure;
    //        command.Parameters.AddWithValue("@ReportID", reportID);

    //        SqlDataAdapter adapter = new SqlDataAdapter(command);
    //        DataSet dataSet = new DataSet();
    //        adapter.Fill(dataSet);

    //        List<DataTable> resultSet1 = new List<DataTable> { dataSet.Tables[0] };
    //        List<DataTable> resultSet2 = new List<DataTable> { dataSet.Tables[1] };
    //        List<DataTable> resultSet3 = new List<DataTable> { dataSet.Tables[2] };
    //        List<DataTable> resultSet4 = new List<DataTable> { dataSet.Tables[3] };
    //        List<DataTable> resultSet5 = new List<DataTable> { dataSet.Tables[4] };
    //        List<DataTable> resultSet6 = new List<DataTable> { dataSet.Tables[5] };
    //        List<DataTable> resultSet7 = new List<DataTable> { dataSet.Tables[6] }; // no of decimal
    //        List<DataTable> resultSet8 = new List<DataTable> { dataSet.Tables[7] }; // color column
    //        List<DataTable> resultSet9 = new List<DataTable> { dataSet.Tables[8] }; // percentage ratio
    //        List<DataTable> resultSet10 = new List<DataTable> { dataSet.Tables[9] }; // ttl cnt

    //        return (resultSet1, resultSet2, resultSet3, resultSet4, resultSet5, resultSet6, resultSet7, resultSet8, resultSet9, resultSet10);
    //    }
    //}
    public List<DataTable> Fnc_GetDynamicReport(int reportID)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand("GetDynamicReport", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ReportID", reportID);

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    // Convert DataSet to a List of DataTables
                    return dataSet.Tables.Cast<DataTable>().ToList();
                }
            }
        }
    }

    // Execute all data in table
    public async Task<List<Dictionary<string, object>>> Fnc_LoadDataAsync(string storedProcName, Dictionary<string, object> parameters)
    {
        var resultList = new List<Dictionary<string, object>>();
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(storedProcName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader[i];
                        }
                        resultList.Add(row);
                    }
                }
            }
        }
        return resultList;
    }
    
    // Execute Total Grand for data
    public async Task<List<Dictionary<string, object>>> Fnc_GrandTotalAsync(string storedProcName, Dictionary<string, object> parameters)
    {
        var resultList = new List<Dictionary<string, object>>();
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader[i];
                            }
                            resultList.Add(row);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return resultList;
    }

    // Execute subtotal stp
    //public async Task<List<Dictionary<string, object>>> Fnc_SubTotalProcedureAsync(string storedProcName)
    //{
    //    var resultList = new List<Dictionary<string, object>>();
    //    using (SqlConnection conn = new SqlConnection(_connectionString))
    //    {
    //        using (SqlCommand cmd = new SqlCommand(storedProcName, conn))
    //        {
    //            cmd.CommandType = CommandType.StoredProcedure;

    //            if (storedProcName.Equals("GetFct_StoreNumberTotalMMM", StringComparison.OrdinalIgnoreCase))
    //            {
    //                cmd.Parameters.Add(new SqlParameter("@DateParam", SqlDbType.Date) { Value = new DateTime(2025, 1, 6) });
    //            }
    //            else if (storedProcName.Equals("GetFct_StoreNumberTotalTM", StringComparison.OrdinalIgnoreCase))
    //            {
    //                cmd.Parameters.Add(new SqlParameter("@DateParam", SqlDbType.Date) { Value = new DateTime(2025, 2, 3) });
    //            }

    //            await conn.OpenAsync();
    //            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
    //            {
    //                while (await reader.ReadAsync())
    //                {
    //                    var row = new Dictionary<string, object>();
    //                    for (int i = 0; i < reader.FieldCount; i++)
    //                    {
    //                        row[reader.GetName(i)] = reader[i];
    //                    }
    //                    resultList.Add(row);
    //                }
    //            }
    //        }
    //    }
    //    return resultList;
    //}
    public async Task<List<Dictionary<string, object>>> Fnc_SubTotalProcedureAsync(string storedProcName, Dictionary<string, object> parameters)
    {
        var resultList = new List<Dictionary<string, object>>();

        try
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(storedProcName, conn) { CommandType = CommandType.StoredProcedure })
            {
                // Add parameters dynamically, ensuring null values are handled
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader[i];
                        }
                        resultList.Add(row);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing stored procedure '{storedProcName}': {ex.Message}");
            throw;
        }

        return resultList;
    }



    /* Not Used Function below*/
    public async Task<List<string>> GetFieldNamesAsync(int? reportId)
    {
        var fieldNames = new List<string>();

        if (!reportId.HasValue)
        {
            return fieldNames; 
        }

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(@"SELECT AttributeName
                                                FROM Rpt_Columns WHERE ReportID = @reportId;", conn))
            {
                cmd.Parameters.AddWithValue("@reportId", reportId.Value);
                cmd.CommandType = CommandType.Text; 
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var attributeName = reader.GetString(0);
                        var cleanedAttributeName = attributeName.Trim('[', ']');
                        fieldNames.Add(cleanedAttributeName); 
                    }
                }
            }
        }
        return fieldNames;
    }
    public async Task<List<Dictionary<string, object>>> GetTableDataAsync(string filterColumn, string filterValue)
    {
        var dataList = new List<Dictionary<string, object>>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("GetFct_StoreNumberCol2", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
                {
                    cmd.Parameters.AddWithValue("@FilterColumn", filterColumn);
                    cmd.Parameters.AddWithValue("@FilterValue", filterValue);
                }
                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader[i];
                        }
                        dataList.Add(row);
                    }
                }
            }
        }
        return dataList;
    }
    public async Task<List<Dictionary<string, object>>> TotalSum(string filterColumn, string filterValue)
    {
        var dataList = new List<Dictionary<string, object>>();
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("GetFct_StoreNumberTotal", conn))

            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
                {
                    cmd.Parameters.AddWithValue("@FilterColumn", filterColumn);
                    cmd.Parameters.AddWithValue("@FilterValue", filterValue);
                }
                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader[i];
                        }
                        dataList.Add(row);
                    }
                }
            }
        }
        return dataList;
    }
    public async Task<List<string>> GetEmployeeNamesAsync()
    {
        var employeeNames = new List<string>();
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("GetEmployeeNames", conn)) // Adjust stored procedure name or query
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employeeNames.Add(reader.GetString(0)); // Assuming the name is in the first column
                    }
                }
            }
        }
        return employeeNames;
    }
    public async Task<List<List<Dictionary<string, object>>>> GetDynamicReportAsync(int? reportId)
    {
        var resultList = new List<List<Dictionary<string, object>>>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("dbo.GetDynamicReport", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (reportId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ReportID", reportId.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ReportID", DBNull.Value);
                }
                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    do
                    {
                        var dataList = new List<Dictionary<string, object>>();

                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader[i];
                            }

                            dataList.Add(row);
                        }
                        resultList.Add(dataList);
                    } while (await reader.NextResultAsync());
                }
            }
        }
        return resultList;
    }
    public async Task<List<string>> GetStoredProcedureParameters(SqlConnection conn, string storedProcName)
    {
        var parameterList = new List<string>();
        string query = @"
        SELECT PARAMETER_NAME 
        FROM INFORMATION_SCHEMA.PARAMETERS 
        WHERE SPECIFIC_NAME = @StoredProcName";
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@StoredProcName", storedProcName);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    parameterList.Add(reader.GetString(0));
                }
            }
        }
        return parameterList;
    }
}


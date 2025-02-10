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

    // Fetch field names
    public async Task<List<string>> GetFieldNamesAsync()
    {
        var fieldNames = new List<string>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(@"SELECT COLUMN_NAME 
                                                 FROM INFORMATION_SCHEMA.COLUMNS 
                                                 WHERE TABLE_NAME = 'Fct_My_MTDStoreCalc';", conn))
            {
                cmd.CommandType = CommandType.Text; // Corrected: Use CommandType.Text for a raw SQL query
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        fieldNames.Add(reader.GetString(0));
                    }
                }
            }
        }
        return fieldNames;
    }


    // Fetch data with optional filters
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

    //public async Task<List<GenderCountModel>> TotalCount()
    //{
    //    var genderCounts = new List<GenderCountModel>();

    //    using (SqlConnection conn = new SqlConnection(_connectionString))
    //    {
    //        using (SqlCommand cmd = new SqlCommand("SELECT Gender, COUNT(*) AS GenderCount FROM Dummy GROUP BY Gender", conn))
    //        {
    //            await conn.OpenAsync();
    //            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
    //            {
    //                while (await reader.ReadAsync())
    //                {
    //                    genderCounts.Add(new GenderCountModel
    //                    {
    //                        Gender = reader.GetString(0), // Assuming "Gender" is of type string
    //                        GenderCount = reader.GetInt32(1) // Assuming "GenderCount" is of type int
    //                    });
    //                }
    //            }
    //        }
    //    }
    //    return genderCounts;
    //}

}

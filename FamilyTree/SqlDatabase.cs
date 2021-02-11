using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace FamilyTree
{
    class SqlDatabase
    {
        public string ConnectionString { get; set; } = @"Data Source = .\SQLExpress; Integrated Security = true; database = {0}";
        public string DatabaseName { get; set; } = "FamilyTree";

        public bool CreateDatabase()
        {
            try
            {
                DatabaseName = "master";
                ExecuteSql("CREATE DATABASE FamilyTree");
                DatabaseName = "FamilyTree";
                return true;
            }
            catch
            {
                //DatabaseName = "FamilyTree";
                return false;
            }
        }

        public void CreateTable(string name, string columns)
        {
            ExecuteSql($"CREATE TABLE {name} ({columns})");
        }
        
        private void ExecuteSql(string sql, params (string, string)[] parameters)
        {
            var connectionString = string.Format(ConnectionString, DatabaseName);
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Item1, parameter.Item2);
                    }

                    foreach (SqlParameter parameter in command.Parameters)
                    {
                        if (parameter.Value.ToString() == "null")
                        {
                            parameter.Value = DBNull.Value;
                        }
                    }
                    command.ExecuteNonQuery();
                }
            }
        }

        internal void AddPerson(Person person)
        {
            var sql = "INSERT Family (first_name, last_name, date_of_birth) " +
                "VALUES (@fName, @lName, @dob)";
            var parameters = new (string, string)[]
            {
                ("@fName", person.FirstName),
                ("@lName", person.LastName),
                ("@dob", person.DateOfBirth.ToString())
            };
            ExecuteSql(sql, parameters);
        }

        private DataTable GetDataTable(string sql, params (string, string)[] parameters)
        {
            var dt = new DataTable();
            var connectionString = string.Format(ConnectionString, DatabaseName);
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Item1, parameter.Item2);
                    }

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}

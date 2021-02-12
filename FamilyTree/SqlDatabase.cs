using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
                        if (parameter.Value == null)
                        {
                            parameter.Value = DBNull.Value;
                        }
                    }
                    command.ExecuteNonQuery();
                }
            }
        }

        public int GetLastAddedId()
        {
            var sql = "SELECT id FROM Family";
            var dt = GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                var ids = new List<int>();
                foreach (DataRow row in dt.Rows)
                {
                    ids.Add((int)row["id"]);
                }
                return ids.Last();
            }
            return 0;
        }

        public Person GetParent(string type)
        {
            Console.Write($"Enter name of {type}: ");
            var name = Console.ReadLine();

            var sql = "SELECT * FROM Family WHERE first_name LIKE @name";
            var dt = GetDataTable(sql, ("@name", $"%{name}%"));
            if (dt.Rows.Count > 0)
            {
                var persons = new List<Person>();
                foreach (DataRow row in dt.Rows)
                {
                    persons.Add(GetPerson(row));
                }

                DisplayPersons(persons);
                var option = ChoosePerson(persons.Count);
                if (option > 0)
                {
                    return persons[option - 1];
                }
            }

            Console.WriteLine($"{name} doesn't seem to exist in the database!");
            Console.Write($"Do you want to create {name}(y/n)? ");
            var choice = Console.ReadLine();
            if (choice.ToLower() == "y")
            {
                var p = new Program();
                return p.AddPerson();
            }
            return null;
        }

        public void Update(Person person)
        {
            var sql = "UPDATE Family SET first_name = @fName, " +
                "last_name = @lName, date_of_birth = @dob, " +
                "date_of_death = @dod, mother_id = @mId, father_id = @fId " +
                "WHERE id = @id";

            string dob;
            if (person.DateOfBirth.HasValue)
            {
                dob = person.DateOfBirth.Value.ToShortDateString();
            }
            else
            {
                dob = null;
            }

            string dod;
            if (person.DateOfDeath.HasValue)
            {
                dod = person.DateOfDeath.Value.ToShortDateString();
            }
            else
            {
                dod = null;
            }

            string mId;
            if (person.MotherId.HasValue)
            {
                mId = person.MotherId.Value.ToString();
            }
            else
            {
                mId = null;
            }

            string fId;
            if (person.FatherId.HasValue)
            {
                fId = person.FatherId.Value.ToString();
            }
            else
            {
                fId = null;
            }

            var parameters = new (string, string)[]
            {
                ("@id", person.Id.ToString()),
                ("@fName", person.FirstName),
                ("@lname", person.LastName),
                ("@dob", dob),
                ("@dod", dod),
                ("@mId", mId),
                ("@fId", fId)
            };
            ExecuteSql(sql, parameters);
        }

        public void Delete(Person person)
        {
            var sql = "DELETE FROM Family WHERE id = @id";
            ExecuteSql(sql, ("@id", person.Id.ToString()));
        }

        private int ChoosePerson(int count)
        {
            while (true)
            {
                Console.Write("Which person do you want to choose? ");
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice > 0 && choice >= count)
                    {
                        return choice;
                    }
                    else if (choice == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. try again!");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. try again!");
                }
            }
        }

        private void DisplayPersons(List<Person> persons)
        {
            var ctr = 1;
            foreach (var person in persons)
            {
                var info = $"{ctr++}. {person.FirstName} {person.LastName} ";
                if (person.DateOfBirth != null)
                {
                    info += person.DateOfBirth.Value.ToString("d") + " ";
                }
                if (person.DateOfDeath != null)
                {
                    info += person.DateOfDeath.Value.ToString("d") + " ";
                }
                Console.WriteLine(info);
            }
            Console.WriteLine("0. None of the above");
        }

        private Person GetPerson(DataRow row)
        {
            var person = new Person()
            {
                Id = (int)row["id"],
                FirstName = row["first_name"].ToString(),
                LastName = row["last_name"].ToString()
            };

            if ((row["date_of_birth"] is DBNull) == false)
            {
                person.DateOfBirth = (DateTime)row["date_of_birth"];
            }

            if ((row["date_of_death"] is DBNull) == false)
            {
                person.DateOfDeath = (DateTime)row["date_of_death"];
            }

            if ((row["mother_id"] is DBNull) == false)
            {
                person.MotherId = (int)row["mother_id"];
            }

            if ((row["father_id"] is DBNull) == false)
            {
                person.FatherId = (int)row["father_id"];
            }

            return person;
        }

        public void AddPerson(Person person)
        {
            var sql = "INSERT Family (first_name, last_name, date_of_birth) " +
                "VALUES (@fName, @lName, @dob)";
            var parameters = new (string, string)[]
            {
                ("@fName", person.FirstName),
                ("@lName", person.LastName),
                ("@dob", person.DateOfBirth.Value.ToShortDateString())
            };
            ExecuteSql(sql, parameters);
        }

        public List<Person> SelectAll(string where = null, params (string, string)[] parameters)
        {
            var sql = "SELECT * FROM Family";
            var dt = new DataTable();
            if (where != null)
            {
                sql += where;
                dt = GetDataTable(sql, parameters);
            }
            else
            {
                dt = GetDataTable(sql);
            }

            var persons = new List<Person>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    persons.Add(GetPerson(row));
                }
            }
            return persons;
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

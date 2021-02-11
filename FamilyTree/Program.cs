using System;

namespace FamilyTree
{
    class Program
    {
        static void Main()
        {
            var db = new SqlDatabase();
            if (db.CreateDatabase())
            {
                db.CreateTable("Family", 
                    "id int PRIMARY KEY IDENTITY (1,1) NOT NULL, " +
                    "first_name nvarchar(50) NOT NULL, " +
                    "last_name nvarchar(50) NOT NULL, " +
                    "date_of_birth date NULL, " +
                    "date_of_death date NULL, " +
                    "mother_id int NULL, " +
                    "father_id int NULL");
            }
        }
    }
}

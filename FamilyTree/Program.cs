using System;

namespace FamilyTree
{
    class Program
    {
        static void Main()
        {
            var p = new Program();
            p.Setup();
            p.AddPerson();
        }

        //public SqlDatabase Database { get; set; } = new SqlDatabase();
        //public Person person { get; set; } = new Person();

        public void Setup()
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

        public void AddPerson()
        {
            var person = new Person();
            Console.Write("Enter first name: ");
            person.FirstName = Console.ReadLine();
            Console.Write("Enter last name: ");
            person.LastName = Console.ReadLine();
            Console.Write("Enter date of birth: ");
            person.DateOfBirth = Convert.ToDateTime(Console.ReadLine());
            var db = new SqlDatabase();
            db.AddPerson(person);
        }

        public void AddParents(Person person)
        {
            // Hitta mamma och pappa och lägg till deras id till 
            // person.MotherId och person.FatherId


        }
    }
}

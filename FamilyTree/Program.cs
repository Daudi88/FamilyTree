using System;

namespace FamilyTree
{
    class Program
    {
        static void Main()
        {
            var p = new Program();
            p.Setup();
            var person = p.AddPerson();
            p.SetParents(person);
        }

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

        public Person AddPerson()
        {
            var person = new Person();
            Console.Write("Enter first name: ");
            person.FirstName = Console.ReadLine();
            Console.Write("Enter last name: ");
            person.LastName = Console.ReadLine();
            Console.Write("Enter date of birth(yyyy-mm-dd): ");
            person.DateOfBirth = Convert.ToDateTime(Console.ReadLine());
            var db = new SqlDatabase();
            db.AddPerson(person);
            person.Id = db.GetLastAddedId();
            return person;
        }

        public void SetParents(Person person)
        {
            var db = new SqlDatabase();
            var mother = db.GetParent("mother");
            if (mother != null)
            {
                person.MotherId = mother.Id;
            }
            var father = db.GetParent("father");
            if (father != null)
            {
                person.FatherId = father.Id;
            }
            db.UpdatePerson(person);
        }
    }
}

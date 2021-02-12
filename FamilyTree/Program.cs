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
            p.UpdatePerson(person);

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

        public void UpdatePerson(Person person)
        {
            while (true)
            {
                Console.Clear();
                ShowInfo(person);
                Console.WriteLine("What do you want to change?");
                Console.WriteLine("1. First name");
                Console.WriteLine("2. Last name");
                Console.WriteLine("3. Date of birth");
                Console.WriteLine("4. Date of death");
                Console.WriteLine("5. Mother");
                Console.WriteLine("6. Father");
                Console.WriteLine("E. Go back");
                Console.Write("> ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int choice))
                {
                    var db = new SqlDatabase();
                    switch (choice)
                    {
                        case 1:
                            Console.Write("Enter first name: ");
                            person.FirstName = Console.ReadLine();
                            break;
                        case 2:
                            Console.Write("Enter last name: ");
                            person.LastName = Console.ReadLine();
                            break;
                        case 3:
                            Console.Write("Enter date of birth: ");
                            person.DateOfBirth = Convert.ToDateTime(Console.ReadLine());
                            break;
                        case 4:
                            Console.Write("Enter date of death: ");
                            person.DateOfDeath = Convert.ToDateTime(Console.ReadLine());
                            break;
                        case 5:
                            var mother = db.GetParent("mother");
                            if (mother != null)
                            {
                                person.MotherId = mother.Id;
                            }
                            break;
                        case 6:
                            var father = db.GetParent("father");
                            if (father != null)
                            {
                                person.FatherId = father.Id;
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid choice. try again!");
                            break;
                    }
                    db.Update(person);
                }
                else if (input.ToLower() == "e")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. try again!");
                }
            }

        }

        public void SearchPersons()
        {
            var db = SqlDatabase();
            var persons = 
        }

        // TODO Byt Mother id och Father id mot de riktiga namnen.
        private static void ShowInfo(Person person)
        {
            Console.WriteLine($"First name: {person.FirstName}");
            Console.WriteLine($"Last name: {person.LastName}");

            if (person.DateOfBirth.HasValue)
            {
                Console.WriteLine($"Date of birth: {person.DateOfBirth.Value.ToShortDateString()}");
            }
            else
            {
                Console.WriteLine("Date of birth:");
            }

            if (person.DateOfDeath.HasValue)
            {
                Console.WriteLine($"Date of death: {person.DateOfDeath.Value.ToShortDateString()}");
            }
            else
            {
                Console.WriteLine("Date of death:");
            }

            if (person.MotherId.HasValue)
            {
                Console.WriteLine($"Mother id: {person.MotherId.Value}");
            }
            else
            {
                Console.WriteLine("Mother id:");
            }

            if (person.FatherId.HasValue)
            {
                Console.WriteLine($"Father id: {person.FatherId.Value}");
            }
            else
            {
                Console.WriteLine("Father id:");
            }
            Console.WriteLine();
        }

        public void DeletePerson(Person person)
        {
            Console.Write($"Do you want to delete {person.FirstName} {person.LastName}(y/n)? ");
            var choice = Console.ReadLine();
            if (choice.ToLower() == "y")
            {
                var db = new SqlDatabase();
                db.Delete(person);
            }
            Console.WriteLine($"{person.FirstName} {person.LastName} was deleted!");
        }
    }
}

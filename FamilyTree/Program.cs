using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Transactions;

namespace FamilyTree
{
    class Program
    {
        static void Main()
        {
            var p = new Program();
            p.Setup();
            p.Menu();
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

        public void Menu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("FAMILY TREE");
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("1. Add a person");
                Console.WriteLine("2. Search persons");
                Console.WriteLine("E. Exit program");
                Console.Write("> ");
                var choice = Console.ReadLine();
                if (choice.ToLower() == "e")
                {
                    break;
                }
                else if (choice == "1")
                {
                    AddPerson();
                }
                else if (choice == "2")
                {
                    SearchPersons();
                }
                else
                {
                    Console.WriteLine("Invalid choice. Try again!");
                    Thread.Sleep(1500);
                }
            }
        }

        public Person AddPerson()
        {
            Console.WriteLine();
            var person = new Person();
            Console.Write("Enter first name: ");
            person.FirstName = Console.ReadLine();
            Console.Write("Enter last name: ");
            person.LastName = Console.ReadLine();
            Console.Write("Enter date of birth(yyyy-mm-dd): ");
            person.DateOfBirth = Convert.ToDateTime(Console.ReadLine());
            var db = new SqlDatabase();
            db.Insert(person);
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
                Console.WriteLine();
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
                            Thread.Sleep(1500);
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
                    Thread.Sleep(1500);
                }
            }

        }

        public void SearchPersons()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter conditions for search:");
                Console.WriteLine("1. Persons that start vith a certain letter");
                Console.WriteLine("2. Persons born a certain year");
                Console.WriteLine("3. Persons missing data");
                Console.WriteLine("E. Exit to main menu");
                Console.Write("> ");
                var choice = Console.ReadLine();
                Console.WriteLine();
                string sql = null;
                var db = new SqlDatabase();
                var persons = new List<Person>();

                if (choice.ToLower() == "e")
                {
                    break;
                }
                else if (choice == "1")
                {
                    Console.Write("Enter a letter: ");
                    var letter = Console.ReadLine();
                    sql = "WHERE first_name LIKE @letter";
                    persons = db.SelectAll(sql, ("@letter", $"{letter}%"));
                }
                else if (choice == "2")
                {
                    Console.Write("Enter a year: ");
                    var year = Console.ReadLine();
                    sql = "WHERE date_of_birth LIKE @year";
                    persons = db.SelectAll(sql, ("@year", $"{year}%"));
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Which data should be missing?");
                    Console.WriteLine("1. Date of birth");
                    Console.WriteLine("2. Date of death");
                    Console.WriteLine("3. Mother");
                    Console.WriteLine("4. Father");
                    Console.Write("> ");
                    choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            sql = "WHERE date_of_birth IS NULL";
                            break;
                        case "2":
                            sql = "WHERE date_of_death IS NULL";
                            break;
                        case "3":
                            sql = "WHERE mother_id IS NULL";
                            break;
                        case "4":
                            sql = "WHERE father_id IS NULL";
                            break;
                        default:
                            break;
                    }
                    persons = db.SelectAll(sql);
                }
                Console.WriteLine();
                if (persons.Count > 0)
                {
                    foreach (var person in persons)
                    {
                        ShowInfo(person);
                    }
                    Console.Write("Choose an id: ");
                    if (int.TryParse(Console.ReadLine(), out int option))
                    {
                        var person = persons.Where(p => p.Id == option).FirstOrDefault();
                        SelectPerson(person);
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice! Too bad!");
                        Thread.Sleep(1500);
                    }
                }
                else
                {
                    Console.WriteLine("No person match your search.");
                    Thread.Sleep(1500);
                }

            }
        }

        private void SelectPerson(Person person)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("1. Update person");
                Console.WriteLine("2. Show relatives");
                Console.WriteLine("3. Delete person");
                Console.WriteLine("E. Go back");
                Console.Write("> ");
                var choice = Console.ReadLine();
                Console.WriteLine();
                if (choice.ToLower() == "e")
                {
                    break;
                }
                else if (choice == "1")
                {
                    UpdatePerson(person);
                }
                else if (choice == "2")
                {
                    ShowRelatives(person);
                }
                else if (choice == "3")
                {
                    if (DeletePerson(person))
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Try again!");
                    Thread.Sleep(1500);
                }
            }
        }

        private void ShowRelatives(Person person)
        {
            var db = new SqlDatabase();
            var parents = db.GetParents(person);
            if (parents.Count > 0)
            {
                Console.WriteLine("Parents:");
                foreach (var parent in parents)
                {
                    ShowInfo(parent);
                }
                Console.WriteLine(); 
            }

            var siblings = db.GetSiblings(person);
            if (siblings.Count > 0)
            {
                Console.WriteLine("Siblings:");
                foreach (var sibling in siblings)
                {
                    ShowInfo(sibling);
                } 
                Console.WriteLine();
            }

            var relatives = parents;
            relatives.AddRange(siblings);
            if (relatives.Count > 0)
            {
                Console.Write("Choose an id: ");
                if (int.TryParse(Console.ReadLine(), out int option))
                {
                    var relative = relatives.Where(p => p.Id == option).FirstOrDefault();
                    SelectPerson(relative);
                }
                else
                {
                    Console.WriteLine("Invalid choice! Too bad!");
                    Thread.Sleep(1500);
                } 
            }
            else
            {
                Console.WriteLine($"{person.FirstName} {person.LastName} doesn't have any relatives.");
                Thread.Sleep(1500);
            }
        }

        private static void ShowInfo(Person person)
        {
            var db = new SqlDatabase();
            Console.WriteLine($"Id: {person.Id}");
            Console.WriteLine($"Name: {person.FirstName} {person.LastName}");

            Console.Write("Born: ");
            if (person.DateOfBirth.HasValue)
            {
                Console.Write(person.DateOfBirth.Value.ToShortDateString());
            }
            Console.WriteLine();

            Console.Write("Deceased: ");
            if (person.DateOfDeath.HasValue)
            {
                Console.Write(person.DateOfDeath.Value.ToShortDateString());
            }
            Console.WriteLine();

            Console.Write("Mother: ");
            if (person.MotherId.HasValue)
            {
                var mother = db.SearchById(person.MotherId.Value);
                Console.Write(mother.FirstName + " " + mother.LastName);
            }
            Console.WriteLine();

            Console.Write("Father: ");
            if (person.FatherId.HasValue)
            {
                var father = db.SearchById(person.FatherId.Value);
                Console.Write(father.FirstName + " " + father.LastName);
            }
            Console.WriteLine("\n");
        }

        public bool DeletePerson(Person person)
        {
            Console.WriteLine();
            Console.Write($"Do you want to delete {person.FirstName} {person.LastName}(y/n)? ");
            var choice = Console.ReadLine();
            if (choice.ToLower() == "y")
            {
                var db = new SqlDatabase();
                db.Delete(person);
                Console.WriteLine($"{person.FirstName} {person.LastName} was deleted!");
                return true;
            }
            return false;
        }
    }
}

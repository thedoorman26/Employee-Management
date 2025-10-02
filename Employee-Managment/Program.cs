using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static List<Employee> employees = new List<Employee>(); //List to store employee objects
    static int nextId = 1; //Auto-incrementing ID for new employees

    static void Main()
    {
        loadData(); // Load data from file at startup
        bool inMenu = true; //loop depends on this
        int id = 0; //initialized int for employee id selection later

        while (inMenu) // menu start
        {
            Console.Clear(); //Clears previous entries to prevent multiple instances of the menu appearing
            Console.WriteLine("Welcome to the Employee Management System! Please choose an option from below:");
            Console.WriteLine("1. Show all employee data");
            Console.WriteLine("2. Find a specific employee");
            Console.WriteLine("3. Add a new employee");
            Console.WriteLine("4. Delete an existing employee");
            Console.WriteLine("5. Save current data to storage");
            Console.WriteLine("6. Exit program");
            Console.WriteLine("Please enter a number (1-6): ");

            string answer = Console.ReadLine() ?? ""; // reads input from user

            switch (answer)
            {
                case "1": //Show all employee data
                    Console.Clear();
                    showAll();
                    Console.WriteLine("Press any key to return to main menu.");
                    Console.ReadKey(); //waits for user to press key before clearing screen
                    break;

                case "2": //Find a specific employee
                    Console.Clear();
                    Console.WriteLine("Please enter the Employee ID:");
                    string inputID = Console.ReadLine() ?? "0";
                    if (int.TryParse(inputID, out id)) //sanitizes inputs from string to int
                    {
                        showEmployee(id);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Press any key to return to main menu.");
                    }
                    Console.WriteLine("Press any key to return to main menu.");
                    Console.ReadKey();
                    break;

                case "3": //Add new employee
                    Console.Clear();
                    Console.WriteLine("Please enter the employee's first name:");
                    string fName = Console.ReadLine() ?? "";
                    Console.WriteLine("Please enter the employee's last name:");
                    string lName = Console.ReadLine() ?? "";
                    Console.WriteLine("Please enter the employee's job title:");
                    string jName = Console.ReadLine() ?? "";
                    addEmployee(fName, lName, jName);
                    Console.WriteLine("Press any key to return to main menu.");
                    Console.ReadKey();
                    break;

                case "4": //Delete employee
                    Console.Clear();
                    Console.WriteLine("Please enter the Employee ID:");
                    inputID = Console.ReadLine() ?? "0";
                    if (int.TryParse(inputID, out id)) //same as case 2
                    {
                        showEmployee(id); // Show employee before deletion
                        Console.WriteLine("Are you sure you want to delete this employee? (y/n)"); //Confirmation dialogue
                        string yesno = Console.ReadLine() ?? "";
                        if (yesno.ToLower() == "y") //case-insensitive check
                        {
                            deleteEmployee(id);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    Console.WriteLine("Press any key to return to main menu.");
                    Console.ReadKey();
                    break;

                case "5": //Save existing data
                    Console.Clear();
                    saveData(); // Writes all employees to file
                    Console.WriteLine("Press any key to return to main menu.");
                    Console.ReadKey();
                    break;

                case "6": // Exit program
                    Console.WriteLine("Exiting now...");
                    inMenu = false; //breaks loop
                    Console.Clear();
                    break;

                default: //invalid selection
                    Console.WriteLine("Invalid input. Press any key to return to main menu.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void showAll()
    {
        if (employees.Count == 0) // Check if list is empty
        {
            Console.WriteLine("No employees found.");
            return;
        }

        foreach (Employee emp in employees) //Iterates through list and displays each employee
        {
            emp.DisplayInfo();
            Console.WriteLine("--------------------"); //Visual divider
        }
    }

    static void showEmployee(int id)
    {
        //Find employee by ID
        Employee emp = employees.Find(e => e.ID == id);
        if (emp == null)
        {
            Console.WriteLine("Employee not found.");
        }
        else
        {
            emp.DisplayInfo();
        }
    }

    static void addEmployee(string fName, string lName, string jName)
    {
        //Creates new employee with next available ID
        Employee newEmp = new Employee(nextId++, fName, lName, jName);
        employees.Add(newEmp); //Add to list
        Console.WriteLine("Employee added:");
        newEmp.DisplayInfo(); //Confirm addition
    }

    static void deleteEmployee(int id)
    {
        //Find and remove employee by ID
        Employee emp = employees.Find(e => e.ID == id);
        if (emp != null)
        {
            employees.Remove(emp);
            Console.WriteLine("Employee deleted.");
        }
        else
        {
            Console.WriteLine("Employee not found.");
        }
    }

    static void saveData()
    {
        //Builds full path relative to app's executable
        string dataPath = Path.Combine(AppContext.BaseDirectory, "employees.csv");
        
        //Writes all employee data to CSV
        using (StreamWriter sw = new StreamWriter(dataPath))
        {
            foreach (Employee emp in employees)
            {
                sw.WriteLine($"{emp.ID},{emp.FirstName},{emp.LastName},{emp.JobTitle}");
            }
        }

        Console.WriteLine("Data saved to employees.csv");
    }

    static void loadData()
    {
        //Builds full path relative to app's executable
        string dataPath = Path.Combine(AppContext.BaseDirectory, "employees.csv");

        if (!File.Exists(dataPath))
        {
            Console.WriteLine($"Data file not found at: {dataPath}");
            return;
        }

        //Read all lines from file and parse employee data
        string[] lines = File.ReadAllLines(dataPath);
        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            if (parts.Length == 4 && int.TryParse(parts[0], out int id)) //validate format and ID
            {
                employees.Add(new Employee(id, parts[1], parts[2], parts[3]));

                //Ensures that nextId always stays higher than current max ID
                if (id >= nextId) nextId = id + 1;
            }
        }
    }
}

//Employee class used to represent and manage employee data
public class Employee
{
    public int ID { get; set; } //Unique identifier for each employee
    public string FirstName { get; set; } //First name
    public string LastName { get; set; } //Last name
    public string JobTitle { get; set; } //Job title or role

    public Employee(int id, string firstName, string lastName, string jobTitle)
    {
        ID = id;
        FirstName = firstName;
        LastName = lastName;
        JobTitle = jobTitle;
    }

    //Displays formatted employee info
    public void DisplayInfo()
    {
        Console.WriteLine($"ID: {ID}");
        Console.WriteLine($"Name: {FirstName} {LastName}");
        Console.WriteLine($"Job Title: {JobTitle}");
    }
}

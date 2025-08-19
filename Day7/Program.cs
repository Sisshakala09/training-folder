/*  //**********************************EXAMPLE1********************************************
// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
// See https://aka.ms/new-console-template for more information
using System.Runtime.ExceptionServices;
using static ABC;


//Delegate - It is type safe that hold a reference to a method 
// DelegateName my1 = new DelegateName(Print);

//Admin delegate Is responsible generating a invoice;

//Admin a = new Admin(Invoice);
//A delegate also allows methods to be passed as a parameters and invoked dynamically(at runtime)
//It is used to implement event handling


//Syntax   AccessSpecifier delegate void delegate_name(paramater_list)


class ABC {

   public static void  Invoice()
    { 
    // definition
    }
     //Single Cast Delegate
    public static void Print1(int a, int b) { }
   
    public delegate void MyShow(); //  this delegate will point to a method
    public delegate void Printing();
    public delegate void Admin(); // Declaring a delegate

    static void Main(String[] args)
    {

        MyShow my = new MyShow(Show);
        Printing my1 = new Printing(Print);
        Admin a = new Admin(Invoice);// Calling a Delegate
    }

    //Static method for delegate
    public static void Show()
    {
        Console.WriteLine("Show method called using Delegate");

    }

    //Static method for delegate
    public static void Print() {
        Console.WriteLine("Print method called using Delegate"); 
    
    }


}
*/

//**********************************EXAMPLE2********************************************
/*
// See https://aka.ms/new-console-template for more information
using System.Runtime.ExceptionServices;
using static ABC;


//Delegate - It is type safe that hold a reference to a method 
// DelegateName my1 = new DelegateName(Print);

//Admin delegate Is responsible generating a invoice;

//Admin a = new Admin(Invoice);
//A delegate also allows methods to be passed as a parameters and invoked dynamically(at runtime)
//It is used to implement event handling


//Syntax   AccessSpecifier delegate void delegate_name(paramater_list)


class ABC {

    // Delegate Declaration for add operations and Print 
    // parameterized Delegates responsible for method invocation at runtime
    public delegate int AddDelegate(int a, int b);

    public delegate void PrintDelegate(int result);

    public static void Main(string[] args) {

        //Assign Methods to delegates
        AddDelegate addDelegate = AddNumbers;
      
        /*int a ;
        // a =30;
        ABC a1 = new ABC();
        AddDelegate ad = AddNumbers;

        PrintDelegate print = PrintResult;

        // calling/using delegate
       int sum = addDelegate(10, 30);
        print(sum);
    
    }


    static int AddNumbers(int x , int y)
    { return x + y; }

    static void PrintResult(int result)
    { 
        Console.WriteLine(result);
    }
}

*/


//**********************************EXAMPLE3********************************************
/*
// create a delegate for a admin who is responsible for calculating the invoice(int tutionfess , int transportfees)
//and one more delegate which will print the invoice


using System;
class Delegate3
{
    public delegate int Admin(int tFees, int tpFees);
    public delegate void PrintData(int total);
    static void Main()
    {
        // assigning methods to delegate
        Admin ad = TotalFess;
        PrintData p = Show;
        int res = ad(20000, 2000);
        p(res);
    }
    public static int TotalFess(int tf, int tpf)
    {
        int gst = 10;
        return gst + tf + tpf;
    }
    public static void Show(int total)
    {
        Console.WriteLine("the total fees is:" + total);
    }
}

*/
//************************************Example4********************************************
/*
// See https://aka.ms/new-console-template for more information
using System.Runtime.ExceptionServices;



//Delegate - It is type safe that hold a reference to a method 
// DelegateName my1 = new DelegateName(Print);

//Admin delegate Is responsible generating a invoice;

//Admin a = new Admin(Invoice);
//A delegate also allows methods to be passed as a parameters and invoked dynamically(at runtime)
//It is used to implement event handling


//Syntax   AccessSpecifier delegate void delegate_name(paramater_list)


class MultiCastDelegate
{

    // Delegate Declaration for add operations and Print 
    // parameterized Delegates responsible for method invocation at runtime
    public delegate int Operations(int a, int b);

    public delegate void PrintDelegate(int result);

    public static void Main(string[] args)
    {

        //Assign Methods to delegates
        //Member functions are encapsulated using delegate
        Operations Add = AddNumbers;
        Operations Subtract = SubtractNumbers;



        /*int a ;
        // a =30;
        ABC a1 = new ABC();
        AddDelegate ad = AddNumbers;


        // calling/using delegate
        int sumResult = Add(10, 30);
        int diffResult = Subtract(10, 30);

        //Multi cast Delegate
        PrintDelegate print = PrintResult; // adding first page as printResult method
        print += PrintResultCalci; // adding second page as printresultcalci method

        Console.WriteLine("Sum of two numbers :");
        print(sumResult);
        Console.WriteLine("Difference of two numbers :");
        print(diffResult);
       // Console.WriteLine("Enter your choice 1. for Addition and 2. for subtraction");
      //  int choice = int.Parse(Console.ReadLine());

        
       

    }


    private static int AddNumbers(int x, int y)
    { return x + y; }

    private static int SubtractNumbers(int x, int y)
    { return x - y; }

    private static void PrintResult(int result)
    {
        Console.WriteLine("The result of addition is :" + result);
    }

    private static void PrintResultCalci(int result)
    {
        result = result * 10;
        Console.WriteLine("after applying bonus :" + result);
    }
}



// create a delegate for a admin who is responsible for calculating the invoice(int tutionfess , int transportfees)
//and one more delegate which will print the invoice

// now create two print methods one for printing (Page1 )the actual invoice and second print method (page 2) for deduction of 100% from tution fees
*/


//************************************Example5********************************************
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp2;
//Extension Method - which allows us to add new methods into a class without editing the source code of 
//the class ;

 For eg: if a class has some methods and in future if developers wants to add some more methods but 
 they do not permission of accessing the class
namespace ConsoleApp2
{
   public sealed class OldService
    {

        public int x = 300;
        public void Test1()
        {
            Console.WriteLine("Test 1 method created :");
        }
        public void Test2()
        {
            Console.WriteLine("Test 2 method created :");
        }

        
    }
}










using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp2;


public static class NewService
{
    static int x1 = 200;
    public static void Test4(this OldService ser)
    {
        Console.WriteLine("New extension Method :");
    }

    public static void Test5(this OldService ser1, int x,int y)
    {
        int result = x + y;
        Console.WriteLine(result);
        Console.WriteLine(ser1.x);
    }

    public static bool IsPalindrome(this string s)
    {

       var rev = new string(s.Reverse().ToArray());

        return s.Equals(rev, StringComparison.OrdinalIgnoreCase);
    
    }
    public static void Main()
    {

        OldService sobj = new OldService();
        sobj.Test1();
        sobj.Test2();
        sobj.Test4();
        sobj.Test5(600,700);
        string s = "Madam";
        Console.WriteLine( s.IsPalindrome());



    }
}
*/


//**************************************Synchronous*****************************************

using System;
using System.Collections.Generic; // IAsyncEnumerable  --  await foreach()
using System.Linq;
using System.Text;
using System.Threading.Tasks; //  second class meant for returning multiple taks

namespace ConsoleApp2
{
    //synch  -- when one task is fully completed or generated then the second task will execute 
    //asynch --- when you order  -- you will be getting parallely notification , messages
    //internal class AsynchOperations
    //{


    //    static async IAsyncEnumerable<int> Generate()
    //    {

    //        for (int i = 0; i < 5; i++)
    //        {

    //            await Task.Delay(1000); // Simulation
    //            Console.WriteLine("Generated the value :" + i);
    //            yield return i;

    //        }



    //    }

    //    static async Task Main()
    //    {

    //        await foreach (var num in Generate())
    //            Console.WriteLine("Received by the customer :" + num);

    //    }
    //}

//****************************************************************************
    internal class SynchOperations
    {


        static async Task<List<int>> GenerateAll()   
        {

            var result = new List<int>();

            for (int i = 0; i < 5; i++)
            {

                await Task.Delay(1000); // Simulation
                Console.WriteLine("Generated the value :" + i);
                result.Add(i);

            }
            return result;



        }

        static async Task Main()
        {
            var allnumbers = await GenerateAll(); // waits until all 5 task are generated

            foreach (var num in allnumbers)

            {
                Console.WriteLine("Received by the customer :" + num); // It will prints after all tasks are generated
            }

            //Syntax of Lambda Expression

            var numbers = new[] { 3, 4, 5, 6 };
           var even =  numbers.Where(x => x % 2 == 0);

            Console.WriteLine("Even no's are " + even);


            //New version of switch Case
            string GetDayName(int day) => day switch
            {

                1 => "Monday",
                2 => "Tuesday",
                3 => "Tuesday",
                _ => "Unknown"

            };

            Console.WriteLine(GetDayName(2));
        }
    }
}
//**********************************************************************************

/*Build a console application that:

Stores student data using a List<Student>

Supports adding students

Supports searching students by name

Supports sorting students by name or marks*/
/*
using System;
using System.Collections.Generic;
using System.Linq;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Marks { get; set; }

    public Student(int id, string name, double marks)
    {
        Id = id;
        Name = name;
        Marks = marks;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Marks: {Marks}";
    }
}


public class StudentManager
{
    private List<Student> students = new List<Student>();

    public void AddStudent(Student student)
    {
        students.Add(student);
    }

    public void SearchStudentByName(string name)
    {
        var result = students.Where(s => s.Name.ToLower().Contains(name.ToLower())).ToList();
        if (result.Count == 0)
            Console.WriteLine("No student found.");
        else
            result.ForEach(Console.WriteLine);
    }

    public void SortByName()
    {
        var sorted = students.OrderBy(s => s.Name).ToList();
        Console.WriteLine("Students sorted by name:");
        sorted.ForEach(Console.WriteLine);
    }

    public void SortByMarks()
    {
        var sorted = students.OrderByDescending(s => s.Marks).ToList();
        Console.WriteLine("Students sorted by marks:");
        sorted.ForEach(Console.WriteLine);
    }

    public void DisplayAll()
    {
        Console.WriteLine("All Students:");
        students.ForEach(Console.WriteLine);
    }
}


public class Program
{
    public static void Main()
    {
        StudentManager manager = new StudentManager();

        while (true)
        {
            Console.WriteLine("\n--- Student Manager ---");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. Display All Students");
            Console.WriteLine("3. Search by Name");
            Console.WriteLine("4. Sort by Name");
            Console.WriteLine("5. Sort by Marks");
            Console.WriteLine("6. Exit");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter ID: ");
                    int id = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Enter Name: ");
                    string name = Console.ReadLine();
                    Console.Write("Enter Marks: ");
                    double marks = Convert.ToDouble(Console.ReadLine());
                    manager.AddStudent(new Student(id, name, marks));
                    break;

                case "2":
                    manager.DisplayAll();
                    break;

                case "3":
                    Console.Write("Enter name to search: ");
                    string searchName = Console.ReadLine();
                    manager.SearchStudentByName(searchName);
                    break;

                case "4":
                    manager.SortByName();
                    break;

                case "5":
                    manager.SortByMarks();
                    break;

                case "6":
                    return;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }
}*/
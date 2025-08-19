/*
// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
public class CollectionExamples
{

    public static void Main()
    {

        // List<string> students = new List<string>();
        // students.Add("Niti");
        // students.Add("Preeti");
        // // students.Add(34);

        // foreach (string student in students)
        // {
        //     Console.WriteLine(student);
        // }

        // Dictionary<int, string> data = new Dictionary<int, string>();
        // data.Add(101, "Niti");
        // data.Add(102, "Preeti");

        // Console.WriteLine(data[101]);

        // foreach (KeyValuePair<int, string> kv in data)
        // {
        //     Console.WriteLine(kv.Key + " " + kv.Value);
        // }

        // HashSet<string> employees = new HashSet<string>();
        // employees.Add("Avc");
        // employees.Add("xyz");
        // employees.Add("Avc");

        // foreach (string e in employees)
        // {
        //     Console.WriteLine(e);

        // }

        // Stack<string> todotask = new Stack<string>();
        // todotask.Push("Learn C#");
        // todotask.Push("Revise the concepts");
        // todotask.Push("Clear your exam");

        // foreach (string task in todotask)
        // {
        //     Console.WriteLine(task);
        // }
        // while (todotask.Count > 0)
        // {
        //     todotask.Pop();
        //     Console.WriteLine("One task in completed ");
        // }


        Queue<string> tickets = new Queue<string>();
        tickets.Enqueue("Learn C#");
        tickets.Enqueue("Revise the concepts");
        tickets.Enqueue("Clear your exam");

        foreach (string task in tickets)
        {
            Console.WriteLine(task);
        }
    }



}

// Create a collection of students to store student id ,student name and  subjectmarks(key as a subject and value as a marks)
// then display each student detail with average score

using System;
using System.Collections.Generic;

public class Student
{
    public int Id;
    public string Name;
    public Dictionary<string, int> SubjectMarks;

    public Student(int id, string name)
    {
        Id = id;
        Name = name;
        SubjectMarks = new Dictionary<string, int>();
    }

    public double GetAverage()
    {
        int total = 0;
        foreach (var mark in SubjectMarks.Values)
        {
            total += mark;
        }
        return SubjectMarks.Count > 0 ? (double)total / SubjectMarks.Count : 0;
    }

    public void Display()
    {
        Console.WriteLine($"\nStudent ID: {Id}");
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine("Subject Marks:");
        foreach (var subject in SubjectMarks)
        {
            Console.WriteLine($"  {subject.Key} : {subject.Value}");
        }
        Console.WriteLine($"Average Score: {GetAverage():F2}");
    }
}

public class Program
{
    public static void Main()
    {
        // Create student list
        List<Student> students = new List<Student>();

        // Add Student 1
        Student s1 = new Student(1, "Amit");
        s1.SubjectMarks.Add("Math", 80);
        s1.SubjectMarks.Add("Science", 75);
        s1.SubjectMarks.Add("English", 85);
        students.Add(s1);

        // Add Student 2
        Student s2 = new Student(2, "Priya");
        s2.SubjectMarks.Add("Math", 90);
        s2.SubjectMarks.Add("Science", 88);
        s2.SubjectMarks.Add("English", 95);
        students.Add(s2);

        // Display details of each student
        foreach (var student in students)
        {
            student.Display();
        }
    }
}
*/
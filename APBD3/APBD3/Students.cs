using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;
using System.Text.Json;


namespace APBDS24610
{
    public class Students
    {
        public static string Path { get; set; }

        public static List<Student> GetStudents()
        {
            List<Student> students = new();
            using StreamReader reader = new(Path);
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                students.Add(ParseToStudent(line));
            }
            return students;
        }

        public static Student? GetStudentByIndex(string index)
        {
            List<Student> students = new();
            using StreamReader reader = new(Path);
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains(index))
                {
                    students.Add(ParseToStudent(line));
                    break;
                }
            }
            return students.Count > 0? students[0] : null;
        }

        public static bool Put(string index, Student student)
        {
            if (student == null || student.Index != index)
            {
                return false;
            }

            using StreamReader reader = new(Path);

            HashSet<Student> students = new();

            string lineRead = null;
            while ((lineRead = reader.ReadLine()) != null)
            {
                if (lineRead.Contains(index))
                {
                    students.Add(student);
                    continue;
                }
                students.Add(ParseToStudent(lineRead));
            }
            reader.Close();

            WriteStudents(students);

            return true;
        }

        public static bool Post(Student student)
        {
            if (student == null)
            {
                return false;
            }

            using StreamReader reader = new(Path);

            HashSet<Student> students = new();

            string lineRead = null;
            while ((lineRead = reader.ReadLine()) != null)
            {
                if (lineRead.Contains(student.Index))
                {
                    return false;
                }
                students.Add(ParseToStudent(lineRead));
            }
            reader.Close();

            WriteStudents(students);

            return true;
        }

        public static bool Delete(string index)
        {
            using StreamReader reader = new(Path);

            HashSet<Student> students = new();

            bool isExist = false;

            string lineRead = null;
            while ((lineRead = reader.ReadLine()) != null)
            {
                if (lineRead.Contains(index))
                {
                    isExist = true;
                    continue;
                }
                students.Add(ParseToStudent(lineRead));
            }
            reader.Close();

            if (!isExist)
            {
                return false;
            }

            WriteStudents(students);

            return true;
        }

        private static Student ParseToStudent(string line)
        {
            string[] elements = line.Split(',');
            return new()
            {
                Name = elements[0],
                Surname = elements[1],
                Index = elements[2],
                BirthDate = DateTime.Parse(elements[3]),
                StudyName = elements[4],
                StudyMode = elements[5],
                Email = elements[6],
                FatherName = elements[7],
                MotherName = elements[8],
            };
        }

        private static void WriteStudents(HashSet<Student> students)
        {
            using FileStream fs = new(Path, FileMode.Open);
            fs.SetLength(0);
            fs.Close();


            using StreamWriter writer = new(Path);

            foreach (Student stud in students)
            {
                writer.WriteLine($"{stud.Name},{stud.Surname},{stud.Index},{stud.BirthDate},{stud.StudyName},{stud.StudyMode},{stud.Email},{stud.FatherName},{stud.MotherName}");
            }
        }
    }
}

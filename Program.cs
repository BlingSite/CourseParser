using System;

public class Course
{
    public string Department { get; set; }
    public int CourseNumber { get; set; }
    public string Semester { get; set; }
    public int Year { get; set; }
}

public class CourseParser
{
    public Course Parse(string courseInput)
    {
        string trimmedCourseInput = courseInput.Trim();
        if (trimmedCourseInput.Length == 0)
            throw new ArgumentException("Invalid course input");

        Course course = new Course();
        string nextString;
        (course.Department, nextString) = GetDepartment(trimmedCourseInput);

        string remainingInputString = nextString.TrimStart();
        if (remainingInputString.Length == 0)
            throw new ArgumentException("Invalid course input");

        (course.CourseNumber, nextString) = GetCourseNumber(courseInput, remainingInputString);

        remainingInputString = nextString.TrimStart();
        if (remainingInputString.Length == 0)
            throw new ArgumentException("Invalid course input");

        (course.Semester, course.Year) = GetSemesterYear(courseInput, remainingInputString);

        return course;
    }

    private (string, string) GetDepartment(string courseInput)
    {
        int index = -1;
        bool foundDelimiter = false;
        for (int i = 0; i < courseInput.Length; i++)
        {
            char c = courseInput[i];

            if (c == '-' || c == ' ' || c == ':')
            {
                index = i;
                foundDelimiter = true;
                break;
            }

            if (Char.IsDigit(c))
            {
                index = i;
                break;
            }
        }

        if (index == -1)
            throw new ApplicationException("Not a valid string");

        string department = courseInput.Substring(0, index);

        if (foundDelimiter)
            index++;

        string remainingInputString = courseInput.Substring(index);

        return (department, remainingInputString);
    }


    private (string, int) GetSemesterYear(string course, string inputString)
    {
        int year;
        string normalizeSemester;
        if (Char.IsDigit(inputString[0]))
        {
            string nextString;
            (year, nextString) = GetYearFromYearSemester(inputString);

            string semester = nextString.TrimStart().TrimEnd(); 

            normalizeSemester = NormalizeSemester(semester);
        }
        else
        {
            string yearString;
            (normalizeSemester, yearString) = GetSemesterFromSemesterYear(inputString);

            year = NormalizeYear(yearString);
        }

        return (normalizeSemester, year);
    }

    private int NormalizeYear(string yearString)
    {
        int year;

        if (yearString.Length == 2)
        {
            year = 2000 + int.Parse(yearString);
        }
        else if (yearString.Length == 4)
        {
            year = int.Parse(yearString);
        }
        else
            throw new ApplicationException("Invalid year");

        return year;

    }

    private (string, string) GetSemesterFromSemesterYear(string semesterYear)
    {
        int index = -1;
        bool hasSpace = false;
        for (int i = 0; i < semesterYear.Length; i++)
        {
            char c = semesterYear[i];
            if (c == ' ')
            {
                index = i;
                hasSpace = true;
                break;
            }
            else if (Char.IsDigit(c))
            {
                index = i;
                break;
            }
        }

        string semester = semesterYear.Substring(0, index);

        string normalizedSemester = NormalizeSemester(semester);

        if (hasSpace)
            index++;

        string nextString = semesterYear.Substring(index);

        return (normalizedSemester, nextString);

    }

    private string NormalizeSemester(string semester)
    {
        string NormalizedSemester;

        if (semester == "F")
            NormalizedSemester = "Fall";
        else if (semester == "W")
            NormalizedSemester = "Winter";
        else if (semester == "S")
            NormalizedSemester = "Spring";
        else if (semester == "Su")
            NormalizedSemester = "Summer";
        else
        {
            if (semester != "Fall" && semester != "Winter" && semester != "Spring" && semester != "Summer")
                throw new ApplicationException("Invalid semester");

            NormalizedSemester = semester;
        }

        return NormalizedSemester;
    }

    private (int, string) GetYearFromYearSemester(string yearSemester)
    {
        int index = -1;
        bool spaceDelimiter = false;
        for (int i = 0; i < yearSemester.Length; i++)
        {
            char c = yearSemester[i];

            if (c == ' ')
            {
                index = i;
                spaceDelimiter = true;
                break;
            }
            else if (!Char.IsDigit(c))
            {
                index = i;
                break;
            }
        }

        string yearString = yearSemester.Substring(0, index);

        int year = NormalizeYear(yearString);

        if (spaceDelimiter)
            index++;

        string nextString = yearSemester.Substring(index);

        return (year, nextString);
    }


    private (int, string) GetCourseNumber(string course, string inputString)
    {

        int index = inputString.IndexOfAny(new char[] { ' ' });
        if (index == -1)
            throw new ApplicationException("Not a valid string");

        string courseNumberString = inputString.Substring(0, index);
        int courseNumber;
        if (!int.TryParse(courseNumberString, out courseNumber))
            throw new ApplicationException("Not a valid course number");

        string nextString = inputString.Substring(index + 1);

        return (courseNumber, nextString);
    }
}

namespace CourseHero
{
    class Program
    {
        static void Main(string[] args)
        {
            CourseParser courseParser = new CourseParser();

            string courseInput = "CS111 2018 Fall";
            Course course = courseParser.Parse(courseInput);
            Console.WriteLine($"Input string: {courseInput} Department: {course.Department} course number: {course.CourseNumber} semester: {course.Semester} year: {course.Year}");

            courseInput = "CS 111 2018 Fall";
            course = courseParser.Parse(courseInput);
            Console.WriteLine($"Input string: {courseInput} Department: {course.Department} course number: {course.CourseNumber} semester: {course.Semester} year: {course.Year}");

            courseInput = "CS: 111 2018 Fall";
            course = courseParser.Parse(courseInput);
            Console.WriteLine($"Input string: {courseInput} Department: {course.Department} course number: {course.CourseNumber} semester: {course.Semester} year: {course.Year}");

            courseInput = "CS-111 Fall 2016";
            course = courseParser.Parse(courseInput);
            Console.WriteLine($"Input string: {courseInput} Department: {course.Department} course number: {course.CourseNumber} semester: {course.Semester} year: {course.Year}");

            courseInput = " Math 123 2015 Spring";
            course = courseParser.Parse(courseInput);
            Console.WriteLine($"Input string: {courseInput} Department: {course.Department} course number: {course.CourseNumber} semester: {course.Semester} year: {course.Year}");

            courseInput = " Math 123 Spring 15";
            course = courseParser.Parse(courseInput);
            Console.WriteLine($"Input string: {courseInput} Department: {course.Department} course number: {course.CourseNumber} semester: {course.Semester} year: {course.Year}");

            courseInput = " Math 123 S2015";
            course = courseParser.Parse(courseInput);
            Console.WriteLine($"Input string: {courseInput} Department: {course.Department} course number: {course.CourseNumber} semester: {course.Semester} year: {course.Year}");

            courseInput = " Math 123 Spring2015";
            course = courseParser.Parse(courseInput);
            Console.WriteLine($"Input string: {courseInput} Department: {course.Department} course number: {course.CourseNumber} semester: {course.Semester} year: {course.Year}");


            Console.ReadLine();
        }
    }
}

/*
========================================================
Code Overview
--------------------------------------------------------
Purpose:
- Read connection string from appsettings.json
- Create AppDbContext manually
- Retrieve students
- Preview SQL using ToQueryString()
- Log actual executed SQL
- Compare ToQueryString() vs runtime SQL for Count()


Key Points:
- ToQueryString previews IQueryable shape
- Logging shows runtime SQL
- Count() final SQL differs from preview SQL
========================================================
*/


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.NetworkInformation;
using TrainingCenter.Data;
using TrainingCenter.Entities;
using EFCore.BulkExtensions;


// Build configuration
IConfiguration configuration =
    new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .Build();


// Read connection string
string? connectionString =
    configuration.GetConnectionString("DefaultConnection");


if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Connection string not found.");
    return;
}


// Create options with logging
var options =
    new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .Options;


// Create context
using var context = new AppDbContext(options);


// Test connection
if (!context.Database.CanConnect())
{
    Console.WriteLine("Could not connect.");
    return;
}


Console.WriteLine("Connected successfully.");
Console.WriteLine();


// Run examples
//RetrieveAndPrintStudents(context);
//GetActiveStudentsCount(context);
//GetActiveStudentsWithRools(context);
//GetNonActiveStudentsWithRools(context);
//GetStudentsHavePhoneNumber(context);
//getStudentByidByFine(context);
//GetStudentsName(context);
//GetFilterdStudents(context);
//IsAllActiveStudents(context);
//AnyActiveStudents(context);
//ShowDisticntStatus(context);
//ShowStatusesReport(context);
//ShowStatuseReportMoreThan2(context);
//ShowIncludeMidApproch(context);
//ShowIncludeBestApproch(context);
//ShowIncludethenInclude(context);
//ShowCoursesReport(context);
//ShowStudentsEnrollment(context);
//ShowExpinceveCourses(context);
//ShowPaginng(context);
//GetStudentsWithNoTraking(context);
//GetStudentsWithTraking(context);
//UpdateStudentWithTraking(context);
//UpdateUsingControlledAttached(context, new UpdateStudentDto { StudentId = 2, FirstName = "Fadi", PhoneNumber = "059478655" });
//UpdateUsingLoadthenUpdate(context, new UpdateStudentDto { StudentId = 3, FirstName = "Fatima", PhoneNumber = "05947852" });
//InseartStudent(context);
//DeleteStudent(context, 1002);
//InsertStudentsGoodWay(context);
//eartUsingBulk(context);
//await UpdateTraking(context);
//await UpdateStudentsBatch(context);
RunWithTransaction(context);



/// <summary>
/// Example 1:
/// Retrieve active students
/// </summary>
static void RetrieveAndPrintStudents(
    AppDbContext context)
{
    Console.WriteLine("Example 1 - Retrieve Students");
    Console.WriteLine("=============================");
    Console.WriteLine();


    // Build query first
    var query = context.Students
        .Where(s => s.Status == "Active")
        .OrderBy(s => s.StudentId);


    // Preview SQL
    Console.WriteLine("Preview SQL using ToQueryString():");
    Console.WriteLine("----------------------------------");
    Console.WriteLine(query.ToQueryString());
    Console.WriteLine();


    // Execute query
    var students = query.ToList();


    Console.WriteLine(
        $"Rows Returned: {students.Count}");


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}





static void RunWithTransaction(AppDbContext context)
{

    Console.WriteLine("GOOD EXAMPLE - With Transaction");
    Console.WriteLine("-------------------------------");
    Console.WriteLine();

    using var transaction = context.Database.BeginTransaction();

    try
    {
        var student = new Student
        {
            FirstName = "Fadi",
            LastName = "Fa",
            Email = "ksdjfksj@gmail.com",
            PhoneNumber = "054789655",
            RegisteredAt = DateTime.UtcNow,
            Status = "Active",

        };

        context.Students.Add(student);
        int affectedRows = context.SaveChanges();
        Console.WriteLine($"Student inserted inside transaction. Affected Rows: {affectedRows}");
        Console.WriteLine($"Generated Student ID: {student.StudentId}");
        Console.WriteLine();


        // Simulate failure before the transaction is committed
        throw new Exception("Failure inside transaction.");

        // This code will not execute because of the simulated error above
        var course = new Course
        {
            Title = "Safe Course",
            DurationHours = 20
        };

        context.Courses.Add(course);
        context.SaveChanges();

        // Commit only if all operations succeed
        transaction.Commit();

        Console.WriteLine("Transaction committed successfully.");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");

        // Rollback cancels all operations inside this transaction
        transaction.Rollback();

        Console.WriteLine("Transaction rolled back. Nothing was saved from this transaction.");
        Console.WriteLine();
    }




}

static async Task InsertStudentsGoodWay(AppDbContext context)
{
    Console.WriteLine("Insert Students - Single SaveContext() ");
    Console.WriteLine("============================");
    Console.WriteLine();
    var students = new List<Student>();

    for (int i = 1; i < 10; i++)
    {
        Student st = new Student
        {
            FirstName = "Student" + i,
            LastName = "Last" + i,
            Email = $"student{i}@example.com",
            RegisteredAt = DateTime.UtcNow,
            PhoneNumber = $"05478965{i}",
            Status = "Active",
            DateOfBirth = new DateOnly(2000 + i, 1, 1)
        };
        students.Add(st);
    }
    context.Students.AddRange(students);
    int rows = await context.SaveChangesAsync();
    Console.WriteLine($"Inserted {rows} row(s)");

}

static long eartUsingBulk(AppDbContext context)
{
    Console.WriteLine("bulk Inseart  ");
    Console.WriteLine("============================");
    Console.WriteLine();

    var stopwatch = Stopwatch.StartNew();
    var students = new List<Student>();

    for (int i = 1; i < 10; i++)
    {
        Student st = new Student
        {
            FirstName = "Bluk" + 1,
            LastName = "Last" + i,
            Email = $"Bulk{i}@example.com",
            RegisteredAt = DateTime.UtcNow,
            PhoneNumber = $"05478465{i}",
            Status = "Active",
            DateOfBirth = new DateOnly(2000 + i, 1, 1)
        };
        students.Add(st);
    }


    Console.WriteLine("Students prepared in memory.");
    Console.WriteLine("BulkInsert() performs optimized database bulk operation.");
    Console.WriteLine("BulkInsert() does not use SaveChanges().");
    Console.WriteLine();

    context.BulkInsert(students);

    stopwatch.Stop();

    Console.WriteLine($"Inserted Rows: {students.Count}");
    Console.WriteLine($"Time Taken - BULK: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine("Result: Optimized bulk insert operation was executed.");
    Console.WriteLine();

    return stopwatch.ElapsedMilliseconds;

}
static async Task UpdateTraking(AppDbContext context)
{
    var students = context.Students.Where(st => st.StudentId > 41).ToList();
    foreach (var item in students)
    {
        item.Status = "Suspended";
    }

    int rows = await context.SaveChangesAsync();
    Console.WriteLine($"Updated {rows} row(s) - Tracking");



}

static async Task UpdateStudentsBatch(AppDbContext context)
{
    int rows = await context.Students
          .Where(s => s.StudentId == 5 || s.StudentId == 19)
          .ExecuteUpdateAsync(setters =>
              setters.SetProperty(s => s.Status, "Suspended"));


    //ontext.SaveChanges();

    Console.WriteLine($"Updated {rows} row(s) - Batch");

}





static void InseartStudent(AppDbContext context)
{
    Console.WriteLine("Insert Student");
    Console.WriteLine("============================");
    Console.WriteLine();

    Student student = new Student
    {
        FirstName = "Fadi",
        LastName = "Al-Masri",
        Email = "Fadi@gmail.com",
        Status = "Active",
        PhoneNumber = "054789655",
        RegisteredAt = DateTime.Now,
        DateOfBirth = new DateOnly(2004, 7, 15)

    };



    context.Students.Add(student);
    int RowAffected = context.SaveChanges();
    Console.WriteLine($"Inserted {RowAffected} row(s)");
}

static void DeleteStudent(AppDbContext context, int studentId)
{
    Console.WriteLine("Delete Student");
    Console.WriteLine("============================");
    Console.WriteLine();

    var query = context.Students.Where(s => s.StudentId == studentId);
    var student = query.FirstOrDefault();
    if (student == null)
    {
        Console.WriteLine($"Student with ID {studentId} not found.");
        return;
    }

    Console.WriteLine(
         $"Deleting Student: {student.FirstName} {student.LastName} (ID = {student.StudentId})");

    // Remove() tells EF Core to track this entity as Deleted
    context.Students.Remove(student);

    Console.WriteLine();
    Console.WriteLine("Student marked as Deleted in Change Tracker.");
    Console.WriteLine("DELETE does not support ToQueryString().");
    Console.WriteLine("Runtime logging will show the actual executed DELETE SQL.");
    Console.WriteLine();

    // Execute DELETE
    int affectedRows = context.SaveChanges();

    Console.WriteLine($"Affected Rows: {affectedRows}");
    Console.WriteLine("Student deleted successfully.");
    Console.WriteLine();
}

static void UpdateUsingLoadthenUpdate(AppDbContext context, UpdateStudentDto dto)
{
    Console.WriteLine("Pattern 1");
    Console.WriteLine("Update Student Using Load then Update");


    var stu = context.Students.SingleOrDefault(s => s.StudentId == dto.StudentId);
    if (stu == null)
    {
        Console.WriteLine("Student not found.");
        return;
    }

    stu.FirstName = dto.FirstName;
    stu.PhoneNumber = dto.PhoneNumber;
    context.SaveChanges();

    Console.WriteLine("Updated");

}

static void UpdateUsingControlledAttached(AppDbContext context, UpdateStudentDto dto)
{
    Console.WriteLine("Pattern 2");
    Console.WriteLine("Update Student Using Controoled Attached");




    var stu = new Student
    {
        StudentId = dto.StudentId,
        PhoneNumber = dto.PhoneNumber
    };


    context.Attach(stu);
    context.Entry(stu).Property(s => s.PhoneNumber).IsModified = true;

    context.SaveChanges();
    Console.WriteLine("Updated");

}
static void ShowExpinceveCourses(AppDbContext context)
{

    Console.WriteLine("Courses");
    Console.WriteLine("============================");
    Console.WriteLine();

    var query = context.Courses.Where(c => c.Price > context.Courses.Average(c => c.Price)).OrderBy(c => c.Price).Select(c => new { c.Code, c.Title, c.Price });


    Console.WriteLine(query.ToQueryString());
    var report = query.ToList();
    foreach (var item in report)
    {
        Console.WriteLine(item.Code + " " + item.Title + " $" + item.Price);
    }

    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();


}

static void GetStudentsWithTraking(AppDbContext context)
{
    Console.WriteLine("Students With Traking");
    Console.WriteLine("============================");
    Console.WriteLine();
    var report = context.Students.ToList();
    int StudentsCount = report.Count;
    int EntityTraked = context.ChangeTracker.Entries().Count();

    Console.WriteLine($"Traked Entities: {EntityTraked} - Students Count: {StudentsCount}");
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();


}
static void GetStudentsWithNoTraking(AppDbContext context)
{
    Console.WriteLine("Students With No Traking");
    Console.WriteLine("============================");
    Console.WriteLine();
    var report = context.Students.AsNoTracking().ToList();
    int StudentsCount = report.Count;
    int EntityTraked = context.ChangeTracker.Entries().Count();

    Console.WriteLine($"Traked Entities: {EntityTraked} - Students Count: {StudentsCount}");
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();


}

static void UpdateStudentWithTraking(AppDbContext context)
{
    Console.WriteLine("Update Student With Traking");
    Console.WriteLine("============================");
    Console.WriteLine();
    var student = context.Students.SingleOrDefault(s => s.StudentId == 1);

    if (student == null)
    {
        Console.WriteLine("Student not found.");
        return;
    }

    student.FirstName = "Sami";
    Console.WriteLine("Student Name After Updated in memory : " + student.FirstName);

    int rowAffected = context.SaveChanges();

    Console.WriteLine("Row's Affected: " + rowAffected);
    Console.WriteLine($"Changed Saved");


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}

static void ShowPaginng(AppDbContext context)
{
    Console.WriteLine("Page 2 of Students [5]");
    Console.WriteLine("============================");
    Console.WriteLine();

    int page = 2;
    int rows = 5;

    var query = context.Students.OrderBy(s => s.StudentId).Skip((page - 1) * rows).Take(rows).Select(s => new { s.FirstName, s.LastName, s.StudentId });


    Console.WriteLine(query.ToQueryString());
    var report = query.ToList();
    Console.WriteLine("page 2 : ");
    foreach (var item in report)
    {
        Console.WriteLine(item.StudentId + "- " + item.FirstName + " " + item.LastName);
    }

    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();



}
static void ShowCoursesReport(AppDbContext context)
{
    Console.WriteLine("Courses & Instructors");
    Console.WriteLine("============================");
    Console.WriteLine();

    var query = context.Courses.Join(context.Instructors, c => c.InstructorId, i => i.InstructorId, (c, i) => new { c.Code, c.Title, i.FirstName });


    Console.WriteLine(query.ToQueryString());
    var report = query.ToList();
    foreach (var item in report)
    {
        Console.WriteLine(item.Code + ", " + item.Title + ", " + item.FirstName);
    }

    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}
static void ShowStudentsEnrollment(AppDbContext context)
{
    Console.WriteLine("Stuendts Enrollments Using SelectMany()");
    Console.WriteLine("============================");
    Console.WriteLine();

    var report = context.Students.SelectMany(s => s.Enrollments, (s, e) => new
    {
        s.FirstName,
        s.LastName,
        CourseTitle = e != null ? e.Course.Title : "No Enrollment",
        EnrollmentStatus = e != null ? e.Status : "No Enrollment"
    });

    Console.WriteLine(report.ToQueryString());
    var rep = report.ToList();
    foreach (var item in rep)
    {
        Console.WriteLine(item.FirstName + " " + item.LastName + " - " + item.CourseTitle + " - " + item.EnrollmentStatus);
    }
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();

}
static void ShowStudentsReport(AppDbContext context)
{
    Console.WriteLine("Students & StudentsProfile");
    Console.WriteLine("============================");
    Console.WriteLine();

    var report = from s in context.Students
                 join sp in context.StudentProfiles on s.StudentId equals sp.StudentId
                 into profileGroup
                 from p in profileGroup.DefaultIfEmpty()
                 select new
                 {
                     s.FirstName,
                     s.LastName,
                     Country = p != null ? p.Country : "No Profile",
                     City = p != null ? p.City : "No Profile"
                 };


    Console.WriteLine(report.ToQueryString());
    var rep = report.ToList();
    foreach (var item in rep)
    {
        Console.WriteLine(item.FirstName + " " + item.LastName + " - " + item.Country + " - " + item.City);
    }
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}

static void ShowIncludethenInclude(AppDbContext context)
{
    Console.WriteLine("Students & Enrollments");
    Console.WriteLine("============================");
    Console.WriteLine();
    var query = context.Students.Include(s => s.Enrollments).ThenInclude(e => e.Course).OrderBy(s => s.StudentId);

    Console.WriteLine(query.ToQueryString());
    var report = query.ToList();
    foreach (var student in report)
    {
        Console.WriteLine($"{student.StudentId} - {student.FirstName} {student.LastName}");

        foreach (var enrollment in student.Enrollments)
        {
            Console.WriteLine(
                $"   Course: {enrollment.Course.Title}, " +
                $"Status: {enrollment.Status}, " +
                $"Progress: {enrollment.ProgressPercent}%");
        }

        Console.WriteLine();
    }
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}

static void ShowIncludeMidApproch(AppDbContext context)
{

    Console.WriteLine("Students & Enrollments");
    Console.WriteLine("============================");
    Console.WriteLine();

    var query = context.Students.Include(s => s.Enrollments);

    Console.WriteLine(query.ToQueryString());
    var report = query.ToList();

    foreach (var item in report)
    {
        Console.WriteLine(item.FirstName + " " + item.LastName + " Has " + item.Enrollments.Count() + " Enrollments");
    }


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}

static void ShowIncludeBestApproch(AppDbContext context)
{

    Console.WriteLine("Students & Enrollments");
    Console.WriteLine("============================");
    Console.WriteLine();

    var query = context.Students.Select(s => new { s.FirstName, s.LastName, EnrollmentsCount = s.Enrollments.Count() });

    Console.WriteLine(query.ToQueryString());
    var report = query.ToList();

    foreach (var item in report)
    {
        Console.WriteLine(item.FirstName + " " + item.LastName + " Has " + item.EnrollmentsCount + " Enrollments");
    }


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}
static void ShowStatuseReportMoreThan2(AppDbContext context)
{
    Console.WriteLine("Statuses & Countes More Than");
    Console.WriteLine("============================");
    Console.WriteLine();

    var query = context.Students.GroupBy(s => s.Status).Where(g => g.Count() > 6)
        .Select(g => new { status = g.Key, TotalStudents = g.Count() }).OrderBy(g => g.TotalStudents);

    Console.WriteLine(query.ToQueryString());
    var report = query.ToList();

    foreach (var item in report)
    {
        Console.WriteLine(item.status + " " + item.TotalStudents);
    }


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}

static void ShowStatusesReport(AppDbContext context)
{

    Console.WriteLine("Statuses & Countes");
    Console.WriteLine("============================");
    Console.WriteLine();

    var query = context.Students.GroupBy(s => s.Status).Select(g => new { status = g.Key, TotalStudents = g.Count() });
    Console.WriteLine(query.ToQueryString());
    var report = query.ToList();

    foreach (var item in report)
    {
        Console.WriteLine(item.status + " " + item.TotalStudents);
    }


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();



}

static void ShowDisticntStatus(AppDbContext context)
{
    Console.WriteLine("Unique Statuses");
    Console.WriteLine("============================");
    Console.WriteLine();

    var query = context.Students.Select(s => s.Status).Distinct();
    Console.WriteLine(query.ToQueryString());
    var status = query.ToList();

    foreach (var item in status)
    {
        Console.WriteLine(item.ToString());
    }



    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();


}

static void AnyActiveStudents(AppDbContext context)
{
    Console.WriteLine(" Any() Comparison");
    Console.WriteLine("============================");
    Console.WriteLine();




    bool isActive = context.Students.Any(s => s.Status == "Active");

    Console.WriteLine("Any Active Students: " + isActive);


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();


}

static void ShowMinMax(AppDbContext context)
{
    Console.WriteLine(" Min() and Max() Comparison");
    Console.WriteLine("============================");
    Console.WriteLine();

    var goodQuery =
            context.Courses
                   .Select(e => e.Price);
    Console.WriteLine(goodQuery.ToQueryString());
    decimal min = goodQuery.Min();
    decimal max = goodQuery.Max();
    Console.WriteLine($"Min (calculated in database): {min}");
    Console.WriteLine($"Max (calculated in database): {max}");
    Console.WriteLine();



    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();

}


static void GetAvarage(AppDbContext context)
{
    Console.WriteLine(" Average() Comparison");
    Console.WriteLine("============================");
    Console.WriteLine();

    var goodQuery =
            context.Enrollments
                   .Select(e => e.ProgressPercent);
    Console.WriteLine(goodQuery.ToQueryString());

    decimal avg = goodQuery.Average();

    Console.WriteLine($"Average (calculated in database): {avg}");
    Console.WriteLine();




    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();

}

static void IsAllActiveStudents(AppDbContext context)
{
    Console.WriteLine(" All() Comparison");
    Console.WriteLine("============================");
    Console.WriteLine();




    bool isActive = context.Students.All(s => s.Status == "Active");

    Console.WriteLine("Is All Students Active? : " + isActive);


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();


}



static void GetFilterdStudents(AppDbContext context)
{
    var query = context.Students
     .Where(s => s.Status == "Active")
     .Select(s => new
     {
         s.StudentId,
         fullName = s.FirstName + " " + s.LastName

     }).OrderBy(s => s.fullName).ThenBy(s => s.StudentId);


    // Preview SQL
    Console.WriteLine("Preview SQL using ToQueryString():");
    Console.WriteLine("----------------------------------");
    Console.WriteLine(query.ToQueryString());
    Console.WriteLine();


    var students = query.ToList();
    foreach (var item in students)
    {
        Console.WriteLine(item.StudentId + " " + item.fullName);
    }

}

static void getStudentByidByFind(AppDbContext context)
{
    Console.WriteLine("Using Find()");
    var student = context.Students.Find(1);
    Console.WriteLine(student.FirstName);

    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();


}

/// <summary>
/// Example 2:
/// Show difference between ToQueryString() and Count() runtime SQL
/// </summary>
/// 
static void GetStudentsName(AppDbContext appDbContext)
{
    var query = appDbContext.Students.Select(s => new
    {
        s.FirstName,
        s.LastName
    });
    Console.WriteLine("----------------------------------");
    Console.WriteLine(query.ToQueryString());
    Console.WriteLine();


    var students = query.ToList();
    foreach (var item in students)
    {
        Console.WriteLine(item.FirstName + " " + item.LastName);
    }


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();

}

static void GetActiveStudentsWithRools(AppDbContext appDbContext)
{

    var query = appDbContext.Students.Where(s => s.Status == "Active" && s.Email.Contains("@student.com")).OrderBy(s => s.FirstName);
    Console.WriteLine("Preview SQL using ToQueryString():");
    Console.WriteLine("----------------------------------");
    Console.WriteLine(query.ToQueryString());
    Console.WriteLine();


    var students = query.ToList();
    foreach (var item in students)
    {
        Console.WriteLine(item.FirstName);
    }


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();

}


static void GetNonActiveStudentsWithRools(AppDbContext appDbContext)
{

    var query = appDbContext.Students.Where(s => s.Status == "Suspended" && (s.FirstName.StartsWith("A") || s.LastName.StartsWith("M"))).OrderBy(s => s.FirstName);
    Console.WriteLine("Preview SQL using ToQueryString():");
    Console.WriteLine("----------------------------------");
    Console.WriteLine(query.ToQueryString());
    Console.WriteLine();


    var students = query.ToList();
    foreach (var item in students)
    {
        Console.WriteLine(item.FirstName + " " + item.LastName);
    }


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();

}

static void GetStudentsHavePhoneNumber(AppDbContext appDbContext)
{

    var query = appDbContext.Students.Where(s => s.PhoneNumber != null && s.PhoneNumber != "").OrderByDescending(s => s.Status).OrderBy(s => s.LastName);
    Console.WriteLine("Preview SQL using ToQueryString():");
    Console.WriteLine("----------------------------------");
    Console.WriteLine(query.ToQueryString());
    Console.WriteLine();


    var students = query.ToList();
    foreach (var item in students)
    {
        Console.WriteLine(item.FirstName + " " + item.LastName);
    }


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();

}
static void GetActiveStudentsCount(
    AppDbContext context)
{
    Console.WriteLine("Example 2 - Count Comparison");
    Console.WriteLine("============================");
    Console.WriteLine();


    // Build query first
    var query = context.Students
        .Where(s => s.Status == "Active")
        .OrderBy(s => s.StudentId);


    // Preview SQL before Count()
    Console.WriteLine("Preview SQL using ToQueryString():");
    Console.WriteLine("----------------------------------");
    Console.WriteLine(query.ToQueryString());
    Console.WriteLine();


    Console.WriteLine(
        "Now executing Count()...");
    Console.WriteLine(
        "Watch logging output above / below.");
    Console.WriteLine();


    // Actual execution
    int total = query.Count();


    Console.WriteLine(
        $"Total Active Students: {total}");


    Console.WriteLine();
    Console.WriteLine(
        "Important Note:");
    Console.WriteLine(
        "ToQueryString() previewed SELECT rows query.");
    Console.WriteLine(
        "But logging shows final executed COUNT(*) query.");


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}


public class UpdateStudentDto
{
    public int StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
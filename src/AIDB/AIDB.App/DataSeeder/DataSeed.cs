using System.Collections.Concurrent;
using AIDB.Core.Models;
using AIDB.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AIDB.App.DataSeeder;

public static class DataSeed
{
    private static ILogger _logger;
    public static async Task SeedDataAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AIDBMainContext>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        if (await context.Persons.AnyAsync())
        {
            _logger.LogInformation("Data already seeded");
            return;
        }

        _logger.LogInformation("Seeding data...");
        await SeedTitlesAsync(context);
        await SeedSubjectsAsync(context);
        await SeedPersonsAsync(context);

        await SeedTeachersAsync(context);
        await SeedStudentsAsync(context);

        await SeedGradesAsync(context);
    }

    private static async Task SeedGradesAsync(AIDBMainContext context)
    {
        _logger.LogInformation("Seeding grades...");
        Random rnd = new();
        List<Grade> grades = new();
        var subjects = await context.Subjects.ToListAsync();

        foreach (var subject in subjects)
        {
            var subjectStudents = await context.Students
                .Include(s => s.Person)
                .ThenInclude(s => s.Subjects)
                .Where(s => s.Person.Subjects.Select(e => e.Id).Contains(subject.Id))
                .ToListAsync();

            var subjectTeachers = await context.Teachers
                .Include(s => s.Person)
                .ThenInclude(s => s.Subjects)
                .Where(s => s.Person.Subjects.Select(e => e.Id).Contains(subject.Id))
                .ToListAsync();

            if (subjectTeachers.Count == 0 || subjectStudents.Count == 0)
                continue;

            foreach (var student in subjectStudents)
            {
                var teacher = subjectTeachers[rnd.Next(0, subjectTeachers.Count)];
                Grade grade = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Student = student,
                    Subject = subject,
                    Teacher = teacher,
                    Score = new Random().Next(0, 101),
                    Date = DateTime.UtcNow
                };

                grades.Add(grade);
            }
        }

        await context.Grades.AddRangeAsync(grades);
        await context.SaveChangesAsync();
        _logger.LogInformation("{count} grades seeded", grades.Count);
    }

    private static async Task SeedPersonsAsync(AIDBMainContext context)
    {
        _logger.LogInformation("Seeding persons...");
        // Configure parallelism
        int targetRecords = 10_000;
        int workersNumber = 4;
        int workerCycles = targetRecords / workersNumber;
        int cyclesReminder = targetRecords % workersNumber;
        Task[] workers = new Task[workersNumber];
        ConcurrentBag<Person> persons = new();

        // Load and parse data in a concurrent parallel manner
        Random rnd = new();
        var namesTask = File.ReadAllLinesAsync(Path.Join(AppContext.BaseDirectory, "Data", "Names.txt"));
        var middleNamesTask = File.ReadAllLinesAsync(Path.Join(AppContext.BaseDirectory, "Data", "MiddleNames.txt"));
        var surnamesTask = File.ReadAllLinesAsync(Path.Join(AppContext.BaseDirectory, "Data", "Surnames.txt"));
        var placesTask = File.ReadAllLinesAsync(Path.Join(AppContext.BaseDirectory, "Data", "Places.txt"));
        var subjectsTask = context.Subjects.ToListAsync();

        await Task.WhenAll(namesTask, middleNamesTask, surnamesTask, placesTask, subjectsTask);

        var names = await namesTask;
        var middleNames = await middleNamesTask;
        var surnames = await surnamesTask;
        var places = await placesTask;
        var subjects = await subjectsTask;

        // Start workers
        for (int i = 0; i < workers.Length; i++)
        {
            int targetCycles = workerCycles;
            if (i == workers.Length - 1)
                targetCycles += cyclesReminder;

            workers[i] = Task.Run(() =>
            {
                ConsumeBatch(targetCycles);
            });
        }

        await Task.WhenAll(workers);

        // Persist data
        await context.Persons.AddRangeAsync(persons);
        await context.SaveChangesAsync();
        _logger.LogInformation("{count} persons seeded", persons.Count);

        void ConsumeBatch(int cycles)
        {
            for (int i = 0; i < cycles; i++)
            {
                int rndStart = rnd.Next(0, subjects.Count / 2);
                int rndEnd = rnd.Next(rndStart, subjects.Count);

                Person person = new()
                {
                    Name = names[rnd.Next(0, names.Length)],
                    MiddleName = middleNames[rnd.Next(0, middleNames.Length)],
                    Surname = surnames[rnd.Next(0, surnames.Length)],
                    Address = places[rnd.Next(0, places.Length)],
                    Email = $"{names[rnd.Next(0, names.Length)]}.{surnames[rnd.Next(0, surnames.Length)]}@ib.com",
                    Subjects = subjects[rndStart..rndEnd]
                };

                persons.Add(person);
            }
        }
    }

    private static async Task SeedStudentsAsync(AIDBMainContext context)
    {
        _logger.LogInformation("Seeding students...");
        Random rnd = new();

        // 95% of persons are students
        List<Student> students = new();
        var persons = await context.Persons.Where(person =>
                !context.Teachers.Select(teacher => teacher.Person.Id)
                .Contains(person.Id))
            .ToListAsync();

        var subjects = await context.Subjects.ToListAsync();

        foreach (var person in persons)
        {
            int rndStart = rnd.Next(0, subjects.Count / 2);
            int rndEnd = rnd.Next(rndStart, subjects.Count);

            Student student = new()
            {
                StartYear = DateTime.UtcNow.AddYears(-rnd.Next(1, 5)),
                Person = person,
            };

            students.Add(student);
        }

        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();
        _logger.LogInformation("{count} students seeded", students.Count);
    }

    private static async Task SeedSubjectsAsync(AIDBMainContext context)
    {
        _logger.LogInformation("Seeding subjects...");
        string[] subjects = { "Math", "Physics", "Chemistry", "Biology", "History", "Geography", "Literature", "English", "German", "French", "Spanish" };
        List<Subject> newSubjects = new();

        foreach (var subject in subjects)
        {
            Subject newSubject = new()
            {
                Name = subject
            };

            newSubjects.Add(newSubject);
        }

        await context.Subjects.AddRangeAsync(newSubjects);
        await context.SaveChangesAsync();
        _logger.LogInformation("{count} subjects seeded", newSubjects.Count);
    }

    private static async Task SeedTeachersAsync(AIDBMainContext context)
    {
        _logger.LogInformation("Seeding teachers...");
        Random rnd = new();

        // 5% of persons are teachers
        List<Teacher> teachers = new();
        var count = await context.Persons.CountAsync();
        var persons = await context.Persons.OrderBy(item => item.Id).Take(count / 100 * 5).ToListAsync();
        var subjects = await context.Subjects.ToListAsync();
        var titles = await context.Titles.ToListAsync();

        int rndStart = rnd.Next(0, subjects.Count / 2);
        int rndEnd = rnd.Next(rndStart, subjects.Count);

        foreach (var person in persons)
        {
            Teacher teacher = new()
            {
                Person = person,
                Title = titles[rnd.Next(0, titles.Count)],
            };

            teachers.Add(teacher);
        }

        await context.Teachers.AddRangeAsync(teachers);
        await context.SaveChangesAsync();
        _logger.LogInformation("{count} teachers seeded", teachers.Count);
    }

    private static async Task SeedTitlesAsync(AIDBMainContext context)
    {
        _logger.LogInformation("Seeding titles...");
        string[] titles = { "Dr", "Prof", "MSc", "BSc" };
        List<Title> newTitles = new();

        foreach (var title in titles)
        {
            Title newTitle = new()
            {
                Name = title
            };

            newTitles.Add(newTitle);
        }

        await context.Titles.AddRangeAsync(newTitles);
        await context.SaveChangesAsync();
        _logger.LogInformation("{count} titles seeded", newTitles.Count);
    }
}
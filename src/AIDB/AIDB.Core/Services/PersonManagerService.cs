using AIDB.Core.IRepositories;
using AIDB.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AIDB.Core.Services;

public class PersonManagerService : IPersonManagerService
{
    private readonly IPersonRepository _personRepository;
    public PersonManagerService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task AddPerson(string name, string middleName, string surname, string email, string address)
    {
        Person person = new Person()
        {
            Name = name,
            MiddleName = middleName,
            Surname = surname,
            Email = email,
            Address = address
        };

        await _personRepository.AddAsync(person);
        await _personRepository.SaveChangesAsync();
    }

    public Task<List<Person>> GetAllPersons()
    {
        return _personRepository.AsQueryable().ToListAsync();
    }
}

public interface IPersonManagerService
{
}
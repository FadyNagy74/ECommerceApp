using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public interface ICityRepository : IRepository<City>
    {
        public Task<string> GetIdUsingName(string name);
    }
}

//We create this interface in case we need to have a different concrete city repository than main one
//For example: a mock city repository for tests

namespace E_CommerceApp.Repositories
{
    public interface IRepository<T> where T : class
    {
        public void Add(T entity);
        //Adding method _context.Add() just adds the entity to memory until we save changes so no need for async
        public Task<T?> GetByIdAsync(string Id);
        //Querying the database is done immediately so we don't want the thread to do nothing during this time
        public Task<List<T>> GetAllAsync();
        //Querying the database is done immediately so we don't want the thread to do nothing during this time

        public void Update(T entity);
        //Updating method _context.Update() just marks the entity for update until we save changes so no need for async

        public void Remove(T entity);
        //Removing method _context.Remove() just makrs the entity for deletion until we save changes so no need for async

        public Task<int> SaveChangesAsync();
        //That integer represents the number of state entries written to the database — in other words, the count of
        //inserted, updated, or deleted entities
    }
}

/*Note that we made this interface so any kind of repository is able to implement the main CRUD operations
 * we made the interface generic so we are able to use it with any kind of entity
 */

/*
 * Because they define a contract that any consumer (service, controller, etc.) must be able to call. 
 * If you don’t 
 * want something exposed, don’t put it in the interface — keep it protected in the base class
 */
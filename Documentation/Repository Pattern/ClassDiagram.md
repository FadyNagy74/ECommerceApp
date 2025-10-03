```mermaid
classDiagram
    direction LR

    class DbContext
    class ApplicationDbContext
    class ApplicationUser
    class Order

    interface IRepository_Generic {
        +void Add(entity)
        +Task GetByIdAsync(Id)
        +Task GetAllAsync()
        +void Update(entity)
        +void Remove(entity)
        +Task SaveChangesAsync()
    }

    class Repository_Generic {
        #DbContext _context
        #DbSet _dbSet
        +Repository(DbContext context)
        +void Add(entity)
        +Task GetByIdAsync(Id)
        +Task GetAllAsync()
        +void Update(entity)
        +void Remove(entity)
        +Task SaveChangesAsync()
    }

    interface IApplicationUserRepository {
        +Task FindUserByIdAsync(userId)
        +Task FindUserOrders(userId)
    }

    class ApplicationUserRepository {
        +ApplicationUserRepository(ApplicationDbContext context)
        +Task FindUserByIdAsync(userId)
        +Task FindUserOrders(userId)
    }

    IRepository_Generic <|.. Repository_Generic : implements
    IApplicationUserRepository <|.. ApplicationUserRepository : implements
    Repository_Generic <|-- ApplicationUserRepository : inherits

    DbContext <|-- ApplicationDbContext : inherits
    Repository_Generic o-- DbContext : uses/injects
    ApplicationUserRepository o-- ApplicationDbContext : uses/injects

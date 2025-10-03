```mermaid
classDiagram
    direction LR

    class DbContext
    class ApplicationDbContext
    class ApplicationUser
    class Order

    interface IRepository~T~ {
        +void Add(T entity)
        +Task~T?~ GetByIdAsync(string Id)
        +Task~List~ GetAllAsync()
        +void Update(T entity)
        +void Remove(T entity)
        +Task~int~ SaveChangesAsync()
    }

    class Repository~T~ {
        #DbContext _context
        #DbSet~T~ _dbSet
        +Repository(DbContext context)
        +void Add(T entity)
        +Task~T?~ GetByIdAsync(string Id)
        +Task~List~ GetAllAsync()
        +void Update(T entity)
        +void Remove(T entity)
        +Task~int~ SaveChangesAsync()
    }

    interface IApplicationUserRepository {
        +Task~ApplicationUser~ FindUserByIdAsync(string userId)
        +Task~List~ FindUserOrders(string userId)
    }

    class ApplicationUserRepository {
        +ApplicationUserRepository(ApplicationDbContext context)
        +Task~ApplicationUser?~ FindUserByIdAsync(string userId)
        +Task~List~ FindUserOrders(string userId)
    }

    IRepository <|.. Repository : implements
    IApplicationUserRepository <|-- IRepository : extends(ApplicationUser)
    ApplicationUserRepository <|-- IApplicationUserRepository : implements
    Repository <|-- ApplicationUserRepository : inherits(ApplicationUser)

    DbContext <|-- ApplicationDbContext : inherits
    Repository o-- DbContext : uses/injects
    ApplicationUserRepository o-- ApplicationDbContext : uses/injects

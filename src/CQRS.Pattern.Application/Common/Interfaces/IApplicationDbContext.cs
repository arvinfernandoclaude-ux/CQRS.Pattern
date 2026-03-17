using Microsoft.EntityFrameworkCore;

using CQRS.Pattern.Domain.Entities;

namespace CQRS.Pattern.Application.Common.Interfaces;

/// <summary>
/// Abstraction over EF Core's DbContext for the Application layer.
///
/// TRADE-OFF: This interface exposes DbSet<T>, which couples the Application layer
/// to Microsoft.EntityFrameworkCore. In strict Clean Architecture, the Application layer
/// should have zero infrastructure dependencies.
///
/// The alternative is the Repository pattern (e.g., IAspNetUserRepository) which returns
/// only domain types and hides EF Core entirely. However, this adds overhead:
///   - One interface + implementation per entity
///   - Every new query requires a new method signature
///   - Projections become awkward (DTOs in repos leak Application concerns into Infrastructure)
///   - DbSet<T> already implements repository + unit of work patterns
///
/// This project accepts the pragmatic approach (DbContext interface) because:
///   1. EF Core is unlikely to be swapped for another ORM
///   2. Wrapping DbSet<> in a repository adds abstraction without meaningful benefit
///   3. Handler access is constrained to: AsNoTracking, FindAsync, Add, Remove, SaveChangesAsync
///
/// If strict Clean Architecture compliance is required, replace this interface with
/// per-entity repository interfaces that return domain types only.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<AspNetUser> AspNetUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

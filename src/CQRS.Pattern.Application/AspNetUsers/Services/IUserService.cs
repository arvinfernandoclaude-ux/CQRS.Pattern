using CQRS.Pattern.Application.AspNetUsers.DTOs;
using CQRS.Pattern.Application.Common.Models;

namespace CQRS.Pattern.Application.AspNetUsers.Services;

public interface IUserService
{
    Task<PaginatedList<AspNetUserDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<AspNetUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> CreateAsync(string userName, string email, string? phoneNumber, CancellationToken cancellationToken);
    Task UpdateAsync(Guid id, string userName, string email, string? phoneNumber, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

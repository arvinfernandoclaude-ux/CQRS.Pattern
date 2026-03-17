using Microsoft.EntityFrameworkCore;
using CQRS.Pattern.Application.AspNetUsers.DTOs;
using CQRS.Pattern.Application.Common.Exceptions;
using CQRS.Pattern.Application.Common.Interfaces;
using CQRS.Pattern.Application.Common.Models;
using CQRS.Pattern.Domain.Entities;

namespace CQRS.Pattern.Application.AspNetUsers.Services;

internal sealed class UserService : IUserService
{
    private readonly IApplicationDbContext _context;

    public UserService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<AspNetUserDto>> GetAllAsync(
        int page, int pageSize, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);

        var query = _context.AspNetUsers
            .AsNoTracking()
            .OrderBy(u => u.UserName)
            .Select(AspNetUserDto.Projection);

        return await PaginatedList.CreateAsync(query, page, pageSize, cancellationToken);
    }

    public async Task<AspNetUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must not be empty.", nameof(id));

        var user = await _context.AspNetUsers
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(AspNetUserDto.Projection)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            throw new NotFoundException("AspNetUser", id);

        return user;
    }

    public async Task<Guid> CreateAsync(
        string userName, string email, string? phoneNumber, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var user = new AspNetUser
        {
            Id = Guid.NewGuid(),
            PhoneNumber = phoneNumber,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        user.SetUserName(userName);
        user.SetEmail(email);

        _context.AspNetUsers.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user.Id;
    }

    public async Task UpdateAsync(
        Guid id, string userName, string email, string? phoneNumber, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must not be empty.", nameof(id));

        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var user = await _context.AspNetUsers.FindAsync([id], cancellationToken);

        if (user is null)
            throw new NotFoundException("AspNetUser", id);

        user.SetUserName(userName);
        user.SetEmail(email);
        user.PhoneNumber = phoneNumber;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must not be empty.", nameof(id));

        var user = await _context.AspNetUsers.FindAsync([id], cancellationToken);

        if (user is null)
            throw new NotFoundException("AspNetUser", id);

        _context.AspNetUsers.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

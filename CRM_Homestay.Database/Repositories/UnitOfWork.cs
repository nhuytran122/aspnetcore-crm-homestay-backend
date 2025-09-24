using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Database.Contexts;
using CRM_Homestay.Database.RepGenerationPatten;
using CRM_Homestay.Entity.Roles;
using CRM_Homestay.Entity.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;
using System.Security.Claims;

namespace CRM_Homestay.Database.Repositories;

public interface IUnitOfWork
{
    CommonQueries CommonQueries { get; }
    GenericRepository<T> GenericRepository<T>() where T : class;
    GenericRepository<T> NewGenericRepository<T>() where T : class;
    void SaveChange();
    Task SaveChangeAsync();
    bool IsChangeTracker();
    NpgsqlTransaction BeginTransaction();
    void Commit();
    void Rollback();
    HomestayContext CreateDbContext();
    IDbTransaction? getCurrentTransaction();
}

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private HomestayContext _context { get; }
    private IDbTransaction? _transaction;
    private readonly IServiceProvider _serviceProvider;
    public UnitOfWork(HomestayContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        Console.WriteLine(_context.GetHashCode());
        CommonQueries = new CommonQueries(context);
        _serviceProvider = serviceProvider;
    }

    public HomestayContext CreateDbContext()
    {
        var serviceScope = _serviceProvider.CreateScope();
        var newContext = serviceScope.ServiceProvider.GetService<HomestayContext>();
        return newContext!;
    }

    public IDbTransaction? getCurrentTransaction()
    {
        return _transaction;
    }


    public CommonQueries CommonQueries { get; }


    public GenericRepository<T> GenericRepository<T>() where T : class
    {
        return new GenericRepository<T>(_context);
    }

    public GenericRepository<T> NewGenericRepository<T>() where T : class
    {
        return new GenericRepository<T>(CreateDbContext());
    }

    public void SaveChange()
    {
        _context.SaveChanges();
    }

    public async Task SaveChangeAsync()
    {
        await _context.SaveChangesAsync();
    }

    public bool IsChangeTracker()
    {
        return _context.ChangeTracker.HasChanges();
    }


    public void Dispose()
    {
        _context.Dispose();
        _transaction?.Dispose();
        _transaction = null;
    }
    public NpgsqlTransaction BeginTransaction()
    {
        if (_transaction == null)
        {
            var dbTransaction = _context.Database.BeginTransaction();
            _transaction = (NpgsqlTransaction)dbTransaction.GetDbTransaction();
        }
        return (NpgsqlTransaction)_transaction;
    }

    public void Commit()
    {
        try
        {
            _transaction?.Commit();
        }
        catch
        {
            Rollback();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
        }
    }

    public void Rollback()
    {
        try
        {
            _transaction?.Rollback();
        }
        finally
        {
            _transaction?.Dispose();
        }
    }
}

public class CommonQueries
{
    private readonly HomestayContext _context;

    public CommonQueries(HomestayContext context)
    {
        _context = context;
    }

    public IQueryable<UserWithNavigationProperties> QueryUserListWithNavigationProperties(
        DateTime? startDate,
        DateTime? endDate,
        string? text = null,
        Gender? gender = null,
         Guid? roleId = null,
        bool? IsActive = null
   )
    {
        var normalize = string.Empty;

        if (text != null) normalize = $" {NormalizeString.ConvertNormalizeString(text)} ";

        var query = _context.Users.Where(x => !x.IsDelete)
            .WhereIf(text != null, x => (" " + x.NormalizeFullInfo + " ").Contains(normalize) || (" " + x.PhoneNumber + " ").Contains(normalize) ||
            (" " + x.NormalizeAddress + " ").Contains(normalize) || (" " + x.NormalizedUserName + " ").Contains(normalize))
            .WhereIf(startDate != null && endDate == null, x => x.CreationTime.AddHours(7).Date >= startDate!.Value.Date)
            .WhereIf(startDate == null && endDate != null, x => x.CreationTime.AddHours(7).Date <= endDate!.Value.Date)
            .WhereIf(startDate != null && endDate != null, x => x.CreationTime.AddHours(7).Date >= startDate!.Value.Date && x.CreationTime.AddHours(7).Date <= endDate!.Value.Date)
            .WhereIf(gender != null, x => x.Gender == gender)
            .WhereIf(roleId != null, x => x.UserRoles!.Any(x => x.RoleId == roleId))
            .WhereIf(IsActive != null, x => x.IsActive == IsActive)
            .Select(x => new UserWithNavigationProperties()
            {
                User = x,
                Role = x.UserRoles!.FirstOrDefault()!.Role ?? new Role()
            });

        return query.OrderByDescending(x => x.User!.CreationTime);
    }

    public IQueryable<UserWithNavigationProperties> QueryUserWithNavigationProperties(Guid id)
    {
        var query = from user in _context.Users.Where(x => x.Id == id)
                    select new UserWithNavigationProperties
                    {
                        User = user,
                        Role = (from roleUser in _context.UserRoles
                                join role in _context.Roles on roleUser.RoleId equals role.Id
                                where roleUser.UserId == user.Id
                                select role).FirstOrDefault() ?? new Role(),
                    };

        return query;
    }

    public IQueryable<Role> QueryRoles(Guid userId, bool ignoreFilter = false)
    {
        var query = from userRole in _context.UserRoles.Where(x => x.UserId == userId)
                    join role in _context.Roles on userRole.RoleId equals role.Id
                    select role;
        if (ignoreFilter)
        {
            return query.IgnoreQueryFilters();
        }

        return query;
    }

    public IQueryable<Guid> QueryRoleIds(Guid userId)
    {
        var userRoles = _context
            .UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.RoleId);

        return userRoles;
    }

    public IQueryable<Claim> QueryClaimsByRoleIds(List<Guid> ids)
    {
        var query = _context.RoleClaims
            .Where(x => ids.Contains(x.RoleId));

        return query.Select(x => new Claim(x.ClaimType!, x.ClaimValue!));
    }
}
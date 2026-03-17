using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using CQRS.Pattern.Application.AspNetUsers.Services;
using CQRS.Pattern.Application.Common.Behaviours;

namespace CQRS.Pattern.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // CQRS pattern — MediatR + pipeline behaviours
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Service Layer pattern — direct service injection
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}

using Sh.Common;
using Sh.Common.Behaviours;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Features.Bookings.Commands;
using Shared.Bookings;

namespace Sh
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Đăng ký ValidationBehavior
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped<IRequestHandler<CreateBookingCommand, Result<BookingDTO>>,
                Application.Features.Bookings.Commands.NotValidate.CreateBookingCommandHandler>();
            //...
            return services;
        }
    }
}

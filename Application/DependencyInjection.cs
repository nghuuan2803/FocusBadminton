﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Features.Bookings.Commands;
using Shared.Bookings;
using Application.Common.Behaviours;
using Application.Common;
using Application.Interfaces;

namespace Application
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
                Features.Bookings.Commands.CreateBookingCommandHandler>();
            //...
            return services;
        }
    }
}

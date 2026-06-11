using Application.Abstractions;
using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services
            )
        {
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<INotificationLogService, NotificationLogService>();

            services.AddScoped<IMessageService, MessageService>();

            services.AddScoped<INotificationDispatcherService, NotificationDispatcherService>();

            services.AddScoped<INotificationJobProcessor, NotificationJobProcessor>();

            return services;
        }
    }
}

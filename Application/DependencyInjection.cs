using Application.Abstractions;
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

            //todo
            //services.AddScoped<IMessageService, MessageService>();

            services.AddScoped<
                NotificationDispatcherService>();

            return services;
        }
    }
}

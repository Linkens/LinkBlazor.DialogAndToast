using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkBlazor
{
    public static class BlazorTools
    {
        public static void AddLinkBlazorServices(this IServiceCollection services)
        {
            AddDialogService(services);
            AddToastService(services);
        }
        public static void AddDialogService(this IServiceCollection services)
        {
            services.AddScoped<DialogService>();
        }
        public static void AddToastService(this IServiceCollection services)
        {
            services.AddScoped<ToastService>();
        }
    }
}

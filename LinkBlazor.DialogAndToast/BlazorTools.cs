using LinkBlazor.Components;
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
        public static async Task<bool> Confirm(this DialogService dialogService, string message, ConfirmDialogOptions? options = null)
        {
            var Res = await dialogService.OpenAsync<ConfirmDialog>(new() { { "Message", message }, { "Options", options ?? new ConfirmDialogOptions() } });
            if (Res == null) return false;
            return Res == true;
        }
    }
}

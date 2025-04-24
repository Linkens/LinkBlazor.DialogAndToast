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
        public static void AddLinkBlazorServices(this IServiceCollection services, Action<ValidatorOptions>? Options = null)
        {
            AddDialogService(services);
            AddToastService(services, Options);
            
        }
        public static void AddDialogService(this IServiceCollection services)
        {
            services.AddScoped<DialogService>();
        }
        public static void AddToastService(this IServiceCollection services, Action<ValidatorOptions>? Options = null)
        {
            ValidatorService._optionsModificator = Options;
            services.AddScoped<ToastService>();
            services.AddScoped<ValidatorService>();
        }
        public static async Task<bool> Confirm(this DialogService dialogService, string message, ConfirmDialogOptions? options = null)
        {
            var Res = await dialogService.OpenAsync<ConfirmDialog>(new() { { "Message", message }, { "Options", options ?? new ConfirmDialogOptions() } });
            if (Res == null) return false;
            return Res == true;
        }
    }
}

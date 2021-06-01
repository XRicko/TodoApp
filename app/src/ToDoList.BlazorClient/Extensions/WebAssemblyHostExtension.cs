using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ToDoList.BlazorClient.Extensions
{
    public static class WebAssemblyHostExtension
    {
        public async static Task SetDefaultCultureAsync(this WebAssemblyHost host)
        {
            var localStorage = host.Services.GetRequiredService<ILocalStorageService>();
            string localStorageCulture = await localStorage.GetItemAsStringAsync("BlazorCulture");

            if (localStorageCulture is not null)
            {
                var culture = new CultureInfo(localStorageCulture);

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }
        }
    }
}

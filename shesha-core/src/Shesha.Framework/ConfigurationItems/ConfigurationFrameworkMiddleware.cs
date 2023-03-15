﻿using Abp.Dependency;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Shesha.ConfigurationItems.Models;
using StackExchange.Profiling;
using System;
using System.Threading.Tasks;

namespace Shesha.ConfigurationItems
{
    /// <summary>
    /// Configuration framework middleware
    /// </summary>
    public class ConfigurationFrameworkMiddleware : IMiddleware, ISingletonDependency
    {
        public const string ConfigItemModeHeader = "sha-config-item-mode";
        public const string FrontEndApplicationHeader = "sha-frontend-application";

        private readonly IConfigurationFrameworkRuntime _cfRuntime;

        public ConfigurationFrameworkMiddleware(IConfigurationFrameworkRuntime cfRuntime)
        {
            _cfRuntime = cfRuntime;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var frontEndApp = context.Request.Headers.ContainsKey(FrontEndApplicationHeader)
                ? context.Request.Headers[FrontEndApplicationHeader].ToString()
                : null;
            //context.Request.Headers.TryGetValue(FrontEndApplicationHeader, out var frontEndApp);

            context.Request.Headers.TryGetValue(ConfigItemModeHeader, out var modeStr);
            var configItemMode = Enum.TryParse(modeStr, true, out ConfigurationItemViewMode myStatus)
                ? myStatus
                : (ConfigurationItemViewMode?)null;

            try
            {
                MiniProfiler.StartNew();
                if (configItemMode.HasValue || !string.IsNullOrEmpty(frontEndApp))
                {
                    using (_cfRuntime.BeginScope(a =>
                    {
                        a.ViewMode = configItemMode.HasValue
                            ? configItemMode.Value
                            : ConfigurationItemViewMode.Live;
                        a.FrontEndApplication = frontEndApp;
                    }))
                    {
                        await next(context);
                    }
                }
                else
                    await next(context);
            }
            finally
            {
                MiniProfiler.Current?.Stop();
            }
        }
    }
}

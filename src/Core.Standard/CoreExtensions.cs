﻿using Onbox.Abstractions.VDev;
using Onbox.Core.VDev.Http;
using Onbox.Core.VDev.Json;
using Onbox.Core.VDev.Logging;
using Onbox.Core.VDev.Mapping;
using System;
using System.IO;


namespace Onbox.Core.VDev
{
    /// <summary>
    /// Core extensions for Onbox's container
    /// </summary>
    public static class CoreExtensions
    {
        /// <summary>
        /// Adds <see cref="IHttpService"/>, <see cref="IJsonService"/>, <see cref="ILoggingService"/>, and <see cref="IMapper"/> default implementations to the container
        /// </summary>
        /// <param name="container">The container in context</param>
        /// <returns>The container in context</returns>
        public static IContainer AddOnboxCore(this IContainer container)
        {
            container.AddHttp();
            container.AddJson();
            container.AddFileLogging();
            container.AddMapper();

            return container;
        }

        /// <summary>
        /// Adds <see cref="ILoggingService"/> as <see cref="FileLoggingService"/> to the container
        /// </summary>
        /// <param name="container">The container in context</param>
        /// <param name="config">If no configuration is specified it will log to the user's temp folder with a maximum size of 200kb</param>
        /// <returns>The container in context</returns>
        public static IContainer AddFileLogging(this IContainer container, Action<FileLoggingSettings> config = null)
        {
            var settings = new FileLoggingSettings();
            config?.Invoke(settings);

            var extension = ".log";

            // Adjust the default settings if not config was passed or no valid config passed
            settings.Path = string.IsNullOrWhiteSpace(settings.Path) ? Path.GetTempPath() : settings.Path;
            settings.FileName = string.IsNullOrWhiteSpace(settings.FileName) ? "Onbox.Logging" + extension : settings.FileName;
            settings.FileName = settings.FileName.EndsWith(extension) ? settings.FileName : settings.FileName + extension;
            settings.MaxFileSizeInBytes = settings.MaxFileSizeInBytes == null ? 600000 : settings.MaxFileSizeInBytes;

            container.AddSingleton(settings);
            container.AddSingleton<ILoggingService, FileLoggingService>();

            return container;
        }

        /// <summary>
        /// Adds <see cref="IMapper"/> as <see cref="Mapper"/> to the container
        /// </summary>
        /// <param name="container">The container in context</param>
        /// <param name="config">If no configuration is specified it will add no post mapping actions</param>
        /// <returns>The container in context</returns>
        public static IContainer AddMapper(this IContainer container, Action<MapperActionManager> config = null)
        {
            var setting = new MapperActionManager();
            config?.Invoke(setting);

            container.AddSingleton<IMapperActionManager>(setting);
            container.AddSingleton<IMapperOperator, MapperOperator>();
            container.AddSingleton<IMapper, Mapper>();

            return container;
        }
    }
}
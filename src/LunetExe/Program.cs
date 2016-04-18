﻿using System;
using System.IO;
using System.IO.Compression;
using Lunet.Runtime;
using Microsoft.Extensions.Logging;

namespace Lunet
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerFactory = new LoggerFactory().AddConsole(LogLevel.Trace);

            var site = SiteFactory.FromFile(Path.Combine(Environment.CurrentDirectory, args[0]), loggerFactory);
            //site.Initialize();

            site.Generator.Run();

            site.Statistics.Dump((s => site.Info(s)));
        }
    }
}
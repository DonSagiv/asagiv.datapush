﻿// See https://aka.ms/new-console-template for more information
using asagiv.common.Logging;
using asagiv.pushrocket.common.Models;
using Grpc.Net.Client;

Console.WriteLine("Hello, World!");

var logger = LoggerFactory.CreateLogger();

logger.Information("Information");
logger.Warning("Warn");
logger.Error("Error");
logger.Fatal("Fatal");

Console.ReadKey();
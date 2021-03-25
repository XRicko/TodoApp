﻿using System.Diagnostics.CodeAnalysis;

namespace ToDoList.Core
{
    [ExcludeFromCodeCoverage]
    public class ApiOptions
    {
        public const string Apis = "Apis";

        public string GoogleApiKey { get; init; }
    }
}

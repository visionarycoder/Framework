// Copyright (c) 2025 VisionaryCoder. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

// Core System
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Threading;
global using System.Threading.Tasks;

// Microsoft Extensions - Dependency Injection
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;

// Microsoft Extensions - Logging
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;

// Microsoft Extensions - Configuration
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Options;

// Microsoft Extensions - Caching
global using Microsoft.Extensions.Caching.Memory;

// ASP.NET Core
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.ModelBinding;

// Entity Framework Core
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;

// JWT and Authentication
global using System.IdentityModel.Tokens.Jwt;
global using Microsoft.IdentityModel.Tokens;

// Azure Storage
global using Azure;
global using Azure.Core;
global using Azure.Data.Tables;
global using Azure.Data.Tables.Models;
global using Azure.Identity;
global using Azure.Security.KeyVault.Secrets;
global using Azure.Storage.Blobs;
global using Azure.Storage.Blobs.Models;
global using Azure.Storage.Queues;
global using Azure.Storage.Queues.Models;

// FTP
global using FluentFTP;

// Polly Resilience
global using Polly;

// NJsonSchema
global using NJsonSchema;
global using NJsonSchema.Validation;

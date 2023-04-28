global using Application.Core;
global using Application.ExceptionMiddleware;
global using Application.Extensions;
global using Application.Services.AppFromUrlService;
global using Application.Services.AppService;
global using Application.Services.AuthService;
global using Application.Services.AutorunService;
global using Application.Services.DailyRandomService;
global using Application.Services.GetUserService;
global using Application.Services.JwtBearerOptionsService;
global using Application.Services.SeedDatabaseService;
global using Application.Services.ServicesExtensionMethods;
global using Application.Services.TokenBlockingService;
global using Application.Services.TokenService;
global using Application.Services.UserService;

global using AutoMapper;

global using Database;
global using Domain.Models;
global using DotNext.Threading;

global using SharedDomain.DTO;
global using SharedDomain.Enums;
global using SharedDomain.Models;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;

global using System.IdentityModel.Tokens.Jwt;
global using System.Linq.Expressions;
global using System.Net;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Text.RegularExpressions;
global using System.Web;

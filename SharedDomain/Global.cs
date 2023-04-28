global using SharedDomain.DTO;
global using SharedDomain.Enums;
global using SharedDomain.ExtensionMethods;
global using SharedDomain.Models;
global using SharedDomain.Validators;

global using System.ComponentModel.DataAnnotations;
global using System.Linq.Expressions;
global using System.Net.Http.Headers;
global using System.Net.Http.Json;
global using System.Security.Claims;
global using System.Text.Json;
global using System.Text.RegularExpressions;
global using System.Xml.Serialization;

public static class GLOBAL
{
    public const int MAX_ITEMS_TO_LOAD = 100;
    public const int ITEMS_TO_LOAD = 5;

    public const int IMAGE_SIZE_1 = 16;
    public const int IMAGE_SIZE_2 = 32;
    public const int IMAGE_SIZE_3 = 64;
    public const int IMAGE_SIZE_4 = 128;
    public const int IMAGE_SIZE_5 = 256;
    public const int IMAGE_SIZE_6 = 512;

    public const string IMAGE_ALT_TEXT = "The image could not be loaded.";
}
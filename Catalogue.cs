using CatalogueService.Attributes;
using CatalogueService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using CatalogueService.Models;

namespace CatalogueService
{

public class Catalogue
{
    private readonly ILogger<Catalogue> _logger;
    private readonly IPermissionService _permissionService;

    public Catalogue(ILogger<Catalogue> logger, IPermissionService permissionService)
    {
        _logger = logger;
        _permissionService = permissionService;
    }

    [Function("GetCatalogueItems")]
    [Authorize]
    [RequirePermission("read:catalogue")]
    public IActionResult GetCatalogueItems([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        // Check permission using the service
        if (!_permissionService.HasPermission(req.HttpContext.User, "read:catalogue"))
        {
            _logger.LogWarning("User does not have read:catalogue permission");
            return new ObjectResult("Forbidden: Insufficient permissions") { StatusCode = 403 };
        }

        // Create the list of catalogue items
        var catalogueItems = new List<CatalogueItem>
        {
            new CatalogueItem { Id = "0", Name = "MacBook Air M2", Available = true, ImageUrl = "/img/macbook-air-m2.jpg" },
            new CatalogueItem { Id = "1", Name = "MacBook Pro 14-inch", Available = true, ImageUrl = "/img/macbook-pro-14.avif" },
            new CatalogueItem { Id = "2", Name = "Dell XPS 13 Laptop", Available = false, ImageUrl = "" },
            new CatalogueItem { Id = "3", Name = "Dell Latitude 5420 Laptop", Available = true, ImageUrl = "" },
            new CatalogueItem { Id = "4", Name = "Lenovo ThinkPad X1 Carbon", Available = true, ImageUrl = "" },
            new CatalogueItem { Id = "5", Name = "Lenovo Yoga 9i Laptop", Available = false, ImageUrl = "" },
            new CatalogueItem { Id = "6", Name = "iPad Pro 12.9-inch", Available = true, ImageUrl = "" },
            new CatalogueItem { Id = "7", Name = "iPad Air", Available = true, ImageUrl = "" },
            new CatalogueItem { Id = "8", Name = "iPad Mini", Available = false, ImageUrl = "" },
            new CatalogueItem { Id = "9", Name = "Samsung Galaxy Tab S8", Available = true, ImageUrl = "" },
            new CatalogueItem { Id = "10", Name = "Samsung Galaxy Tab A7", Available = true, ImageUrl = "" },
            new CatalogueItem { Id = "11", Name = "Microsoft Surface Pro 9", Available = false, ImageUrl = "" },
            new CatalogueItem { Id = "12", Name = "Microsoft Surface Go 3", Available = true, ImageUrl = "" },
            new CatalogueItem { Id = "13", Name = "Asus ZenBook 14", Available = true, ImageUrl = "" },
            new CatalogueItem { Id = "14", Name = "HP Spectre x360", Available = false, ImageUrl = "" },
            new CatalogueItem { Id = "15", Name = "Acer Swift 5", Available = true, ImageUrl = "" }
        };

        // Return the catalogue as JSON
        return new OkObjectResult(catalogueItems);
    }
}

}

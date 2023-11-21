using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TheBooks.Api.Controllers;

[ApiController]
[Authorize]
public class BaseAuthControllers : ControllerBase
{
    
}
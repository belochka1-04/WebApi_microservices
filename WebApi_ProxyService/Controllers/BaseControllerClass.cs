using Microsoft.AspNetCore.Mvc;
using WebApi_ProxyService.Services;

namespace WebApi_ProxyService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseControllerClass : ControllerBase
    {
        
        protected readonly IDatabaseService _databaseService;

        protected BaseControllerClass(IDatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using WebApi_PartsService.Services;

namespace WebApi_PartsService.Controllers
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

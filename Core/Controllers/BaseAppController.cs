using Microsoft.AspNetCore.Mvc;

namespace EliseuBatista99.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseAppController : ControllerBase
    {
        protected IExecutionContext ExecutionContext;

        public BaseAppController(IExecutionContext _executionContext)
        {
            ExecutionContext = _executionContext;
        }

        //protected IExecutionContext GetExecutionContext()
        //{
        //    return ExecutionContext;
        //}
    }
}

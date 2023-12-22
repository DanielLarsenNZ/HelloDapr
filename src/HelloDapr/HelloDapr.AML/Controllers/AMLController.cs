using HelloDapr.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelloDapr.AML.Controllers
{
    [Route("aml")]
    [ApiController]
    public class AMLController : ControllerBase
    {
        [HttpPost]
        [Route("check")]
        public ActionResult<bool> Post([FromBody] PaymentModel payment)
        {
            //TODO
            return true;
        }
    }
}

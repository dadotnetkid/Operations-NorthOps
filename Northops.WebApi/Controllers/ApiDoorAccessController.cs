using System.Web.Http;

namespace Northops.WebApi.Controllers
{

    [Authorize(Roles = "Administrator")]

    public class ApiDoorAccessController : ApiController
    {
        [Route("api-door-access")]
        public IHttpActionResult Get()
        {
            Startup.devComm.Unlock();
            return Ok();
        }
    }
}

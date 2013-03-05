using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Glipho.OAuth.Providers.WebApiSample.Controllers
{
    [Web.OAuthAuthorise]
    public class AuthorisationEnforcedController : ApiController
    {
        // GET api/authorisationenforced
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/authorisationenforced/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/authorisationenforced
        public void Post([FromBody]string value)
        {
        }

        // PUT api/authorisationenforced/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/authorisationenforced/5
        public void Delete(int id)
        {
        }
    }
}

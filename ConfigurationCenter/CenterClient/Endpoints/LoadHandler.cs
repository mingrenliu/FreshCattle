using CenterClient.Endpoint.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenterClient.Endpoints
{
    internal class LoadHandler : IEndpointHandler
    {
        public Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}

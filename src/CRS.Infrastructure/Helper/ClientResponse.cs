using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CRS.Infrastructure.Helper;

public static class ClientResponse
{
    public static HttpResponseMessage GetClientResponse(HttpStatusCode httpStatusCode, string message)
    {
        var response = new HttpResponseMessage(httpStatusCode)
        {
            Content = new StringContent(message),
            StatusCode = httpStatusCode
        };
        return response;
    }
}


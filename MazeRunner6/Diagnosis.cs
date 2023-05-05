using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MazeRunner6
{
    public class Diagnosis
    {
        private readonly ILogger _logger;
        private readonly Config config;

        public Diagnosis(ILoggerFactory loggerFactory, IOptions<Config> config)
        {
            _logger = loggerFactory.CreateLogger<MazeManagement>();
            this.config = config.Value;
        }

        [Function("Diagnosis")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            if (string.IsNullOrWhiteSpace(config.BlobConnectionString))
            {
                response.WriteString("No Ok");
            }
            else
            {
                response.WriteString("Ok" + config.BlobConnectionString.Substring(0, 4));
            }

            return response;
        }
    }
}

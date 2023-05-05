using System.Net;
using MazeRunner6.Dtos;
using MazeRunner6.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MazeRunner6
{
    public class MazeManagement
    {
        private readonly ILogger _logger;
        private readonly Config config;

        public MazeManagement(ILoggerFactory loggerFactory, IOptions<Config> config)
        {
            _logger = loggerFactory.CreateLogger<MazeManagement>();
            this.config = config.Value;
        }

        [Function("Function")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Maze/{mazeUid?}")] HttpRequestData req,
             string mazeUid)
        {
            var blobProxy = new BlobsProxy(config, _logger);

            if (req.Method == "POST")
            {
                string requestBody = req.ReadAsString();
                if (string.IsNullOrWhiteSpace(requestBody)) { return BadRequest(req, "body is empty"); }

                MazePostRequest data = JsonConvert.DeserializeObject<MazePostRequest>(requestBody);
                if (data.Height < 1 || data.Height > 150) return BadRequest(req, "Height property is out of range (0,150)");
                if (data.Width < 1 || data.Width > 150) return BadRequest(req, "Width property is out of range (0,150)");

                MazeGenerator g = new MazeGenerator();
                var maze = g.Generate(data.Width, data.Height);

                await blobProxy.SaveMaze(maze);
                return Ok(req, new MazePostResponse()
                {
                    MazeUid = maze.MazeUid,
                    Height = maze.Height,
                    Width = maze.Width
                });
            }
            else if (req.Method == "GET")
            {
                if (!Guid.TryParse(mazeUid, out Guid mazeUidParsed)) { return BadRequest(req, "Error parsing maze uid"); }
                var maze = await blobProxy.GetMaze(mazeUidParsed);
                return Ok(req, maze);
            }

            return BadRequest(req, "Method not available");
        }

        private HttpResponseData BadRequest(HttpRequestData req, string message)
        {
            var result = req.CreateResponse(HttpStatusCode.BadRequest);
            result.WriteString(message);
            return result;
        }

        private HttpResponseData Ok<T>(HttpRequestData req, T json)
        {
            var result = req.CreateResponse(HttpStatusCode.OK);
            result.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var raw = JsonConvert.SerializeObject(json);
            result.WriteString(raw);
            return result;
        }
    }
}

using System.Net;
using Azure;
using MazeRunner6.Dtos;
using MazeRunner6.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MazeRunner6
{
    public class GameFunction
    {
        private readonly ILogger _logger;
        private readonly Config config;

        public GameFunction(ILoggerFactory loggerFactory, IOptions<Config> config)
        {
            _logger = loggerFactory.CreateLogger<MazeManagement>();
            this.config = config.Value;
        }

        [Function("Game")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "game/{mazeUid}/{gameUid?}")] HttpRequestData req,
            string mazeUid,
            string gameUid)
        {
            var blobs = new BlobsProxy(this.config, _logger);
            // check maze
            if (!Guid.TryParse(mazeUid, out Guid mazeUidParsed)) { return BadRequest(req, "The maze uid is not valid"); }
            var maze = await blobs.GetMaze(mazeUidParsed);
            if (maze == null) { return BadRequest(req, "The maze uid has not been generated"); }

            if (req.Method == "POST")
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<GamePostRequest>(requestBody);
                if (data == null) { return BadRequest(req, "The body is invalid"); }

                if (string.IsNullOrWhiteSpace(gameUid) && data.Operation == PlayerOperation.Start)
                {
                    // create new game
                    GameDefinition game = new GameDefinition()
                    {
                        MazeUid = mazeUidParsed,
                        GameUid = Guid.NewGuid(),
                        Completed = false,
                        CurrentPositionX = 0,
                        CurrentPositionY = 0
                    };

                    await blobs.SaveGame(game);
                    return Ok(req,game);
                }
                else
                {
                    if (!Guid.TryParse(gameUid, out Guid gameUidParsed)) { return BadRequest(req, "The gameUid is invalid"); }
                    var game = await blobs.GetGame(mazeUidParsed, gameUidParsed);
                    if (game == null) { return BadRequest(req, "The gameUid doesn't exit"); }

                    switch (data.Operation)
                    {
                        case PlayerOperation.Start:
                            game.CurrentPositionX = 0;
                            game.CurrentPositionY = 0;
                            game.Completed = false;
                            await blobs.SaveGame(game);
                            return Ok(req, GetGameResponse(game, maze));
                        case PlayerOperation.NotSet:
                            return BadRequest(req, "Operation is not set");
                        default:
                            if (EvaluateMovement(maze, game, data.Operation))
                            {
                                if (game.CurrentPositionX == maze.Width && game.CurrentPositionY == maze.Height)
                                {
                                    game.Completed = true;
                                }
                                await blobs.SaveGame(game);
                                return Ok(req, GetGameResponse(game, maze));
                            }
                            else
                            {
                                return BadRequest(req, " -You shall not pass- Gandalf said. (There is a wall in that direction");
                            }
                    }
                }
            }
            else if (req.Method == "GET")
            {
                if (!Guid.TryParse(gameUid, out Guid gameUidParsed)) { return BadRequest(req, "The gameUid is invalid"); }
                var game = await blobs.GetGame(mazeUidParsed, gameUidParsed);
                if (game == null) { return BadRequest(req, "The gameUid doesn't exit"); }
                return Ok(req, GetGameResponse(game, maze));
            }


            return BadRequest(req, "Method not defined");
        }

        private static GameResponse GetGameResponse(GameDefinition game, MazeDefinition maze)
        {
            GameResponse response = new GameResponse()
            {
                Game = game
            };

            var node = maze.Blocks.Where(item => item.CoordX == game.CurrentPositionX && item.CoordY == game.CurrentPositionY).FirstOrDefault();
            response.MazeBlockView = node;
            return response;
        }

        private static bool EvaluateMovement(MazeDefinition maze, GameDefinition game, PlayerOperation operation)
        {
            if (game.Completed) return false; // a completed game doesnt allow more movements
            var node = maze.Blocks.Where(item => item.CoordX == game.CurrentPositionX && item.CoordY == game.CurrentPositionY).FirstOrDefault();
            if (node == null) return false; // something strange happens
            switch (operation)
            {
                case PlayerOperation.GoNorth:
                    if (node.NorthBlocked) return false;
                    else game.CurrentPositionY = game.CurrentPositionY - 1;
                    return true;
                case PlayerOperation.GoSouth:
                    if (node.SouthBlocked) return false;
                    else game.CurrentPositionY = game.CurrentPositionY + 1;
                    return true;
                case PlayerOperation.GoEast:
                    if (node.EastBlocked) return false;
                    else game.CurrentPositionX = game.CurrentPositionX + 1;
                    return true;
                case PlayerOperation.GoWest:
                    if (node.WestBlocked) return false;
                    else game.CurrentPositionX = game.CurrentPositionX - 1;
                    return true;
            }

            return false;
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
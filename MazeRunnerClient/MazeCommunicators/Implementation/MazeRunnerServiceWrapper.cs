using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MazeRunnerClient.MazeCommunicators.Contracts;
using MazeRunnerClient.Models;
using Newtonsoft.Json;

namespace MazeRunnerClient.MazeCommunicators.Implementation
{
    internal class MazeRunnerServiceWrapper : IMazeServiceWrapper
    {
        private string _baseUrl;
        private string _apiKey;
        public MazeRunnerServiceWrapper(string baseUrl, string apiKey)
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
        }

        public async Task<string> CreateMaze(int width, int height)
        {
            string createMazeUrl = $"{_baseUrl}Maze?code={_apiKey}";
            var createMazeRequest = new
            {
                Width = width,
                Height = height
            };
            var mazeResponse = await PostAsync<dynamic>(createMazeUrl, createMazeRequest);
            return mazeResponse.MazeUid;
        }

        public async Task<string> CreateGame(string mazeUid)
        {
            string createGameUrl = $"{_baseUrl}Game/{mazeUid}?code={_apiKey}";
            var createGameRequest = new
            {
                Operation = "Start"
            };
            var gameResponse = await PostAsync<dynamic>(createGameUrl, createGameRequest);
            return gameResponse.GameUid;
        }

        public async Task<GameInfo> TakeAlook(string mazeUid, string gameUid)
        {
            string gameInfoUrl = $"{_baseUrl}Game/{mazeUid}/{gameUid}?code={_apiKey}";
            var response = await GetAsync(gameInfoUrl);
            return JsonConvert.DeserializeObject<GameInfo>(response);
        }
        public async Task<GameInfo> MakeMove(string mazeUid, string gameUid, string move)
        {
            // Make an API request to move to the next cell
            string moveUrl = $"{_baseUrl}Game/{mazeUid}/{gameUid}?code={_apiKey}";
            var moveRequest = new
            {
                Operation = move
            };
            var response = await PostAsync<GameInfo>(moveUrl, moveRequest);
            return response;
        }
        private async Task<string> GetAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        private async Task<T> PostAsync<T>(string url, object data)
        {
                using (HttpClient client = new HttpClient())
                {
                    string json = JsonConvert.SerializeObject(data);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }

            }
        }
    }
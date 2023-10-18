using MazeRunnerClient.MazeCommunicators.Implementation;
using MazeRunnerClient.MazeSolvers.Contracts;
using MazeRunnerClient.MazeSolvers.Implementation.Implementation;

namespace MazeRunnerClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Program.exe <baseUrl> <apiKey>");
                return;
            }

            string baseUrl = args[0];
            string apiKey = args[1];

            var serviceWrapper = new MazeRunnerServiceWrapper(baseUrl, apiKey);
            int width = 25;
            int height = 25;
            // Step 1: Create a new maze
            string mazeUid = await serviceWrapper.CreateMaze(width, height);

            // Step 2: Create a new game
            string gameUid = await serviceWrapper.CreateGame(mazeUid);


            IMazeSolver solver = new DFSMazeSolver(serviceWrapper);
            await solver.SolveMaze(mazeUid, gameUid, width, height);


        }
    }
}
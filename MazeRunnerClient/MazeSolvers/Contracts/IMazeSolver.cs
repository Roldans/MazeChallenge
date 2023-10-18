namespace MazeRunnerClient.MazeSolvers.Contracts
{
    internal interface IMazeSolver
    {
        Task SolveMaze(string mazeUid, string gameUid, int width, int height);
    }
}
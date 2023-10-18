using MazeRunnerClient.Models;

namespace MazeRunnerClient.MazeCommunicators.Contracts
{
    internal interface IMazeServiceWrapper
    {
        Task<GameInfo> MakeMove(string mazeUid, string gameUid, string move);
        Task<GameInfo> TakeAlook(string mazeUid, string gameUid);
    }
}
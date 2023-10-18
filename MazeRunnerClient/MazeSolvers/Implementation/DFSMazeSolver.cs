using MazeRunnerClient.MazeCommunicators.Contracts;
using MazeRunnerClient.MazeSolvers.Contracts;
using MazeRunnerClient.Models;

namespace MazeRunnerClient.MazeSolvers.Implementation.Implementation
{
    internal class DFSMazeSolver : IMazeSolver
    {
        private IMazeServiceWrapper _mazeServiceWrapper;
        int _width;
        int _height;
        public DFSMazeSolver(IMazeServiceWrapper mazeServiceWrapper)
        {
            _mazeServiceWrapper = mazeServiceWrapper;
        }

        public async Task SolveMaze(string mazeUid, string gameUid, int width, int height)
        {
            _width = width;
            _height = height;

            char[,] maze = new char[width, height]; // Initialize a 2D maze array
            async Task DrawMaze()
            {
                Console.Clear(); // Clear the console to redraw the maze

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        char cell = maze[x, y];
                        if (cell == 0)
                            Console.Write('.'); // Unexplored path
                        else
                            Console.Write(cell); // Visited path
                    }
                    Console.WriteLine();
                }
                await Task.Delay(100);
            }

            // Solve using the depth-first search algorithm
            async Task<bool> DFS(GameInfo currentLook)
            {
                int x = currentLook.Game.CurrentPositionX;
                int y = currentLook.Game.CurrentPositionY;
                if (currentLook.Game.Completed || x == _width - 1 && y == _height - 1)
                {
                    maze[x, y] = 'X'; // Mark the end of the maze
                    await DrawMaze();
                    Console.WriteLine("Victory!!!");
                    return true;
                }

                maze[x, y] = 'X'; // Mark the current position

                // Try to move in all possible directions (North, South, East, West).
                List<(int, int, Move)> possibleMoves = CalculatePossibleMoves(currentLook, maze);

                foreach ((int newX, int newY, Move move) in possibleMoves)
                {
                    if (maze[newX, newY] == 0)
                    {
                        // Send a POST request to move in the selected direction.
                        var success = await _mazeServiceWrapper.MakeMove(mazeUid, gameUid, move.ToString());
                        if (await DFS(success)) return true;

                        // Backtrack after the recursive call
                        currentLook = await _mazeServiceWrapper.MakeMove(mazeUid, gameUid, GetReverseMove(move).ToString());

                    }
                }
                maze[x, y] = '.'; // Unmark the current position for backtracking
                await DrawMaze();
                return false;
            }

            await DFS(await _mazeServiceWrapper.TakeAlook(mazeUid, gameUid));
        }

        private List<(int, int, Move)> CalculatePossibleMoves(GameInfo currentLook, char[,] maze)
        {
            int x = currentLook.Game.CurrentPositionX;
            int y = currentLook.Game.CurrentPositionY;
            List<(int, int, Move)> possibleMoves = new List<(int, int, Move)>();
            if (validCoords(x, y + 1))
                if (!currentLook.MazeBlockView.SouthBlocked)
                    possibleMoves.Add((x, y + 1, Move.GoSouth));
            if (validCoords(x, y - 1))
                if (!currentLook.MazeBlockView.NorthBlocked)
                    possibleMoves.Add((x, y - 1, Move.GoNorth));
            if (validCoords(x - 1, y))
                if (!currentLook.MazeBlockView.WestBlocked)
                    possibleMoves.Add((x - 1, y, Move.GoWest));
            if (validCoords(x + 1, y))
                if (!currentLook.MazeBlockView.EastBlocked)
                    possibleMoves.Add((x + 1, y, Move.GoEast));


            return possibleMoves;
        }

        bool validCoords(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }
        Move GetReverseMove(Move direction)
        {
            switch (direction)
            {
                case Move.GoNorth:
                    return Move.GoSouth;
                case Move.GoSouth:
                    return Move.GoNorth;
                case Move.GoEast:
                    return Move.GoWest;
                case Move.GoWest:
                    return Move.GoEast;
                default:
                    throw new ArgumentException("Invalid direction: " + direction);
            }
        }
    }
}


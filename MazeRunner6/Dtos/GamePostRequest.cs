using System;
using System.Collections.Generic;
using System.Text;

namespace MazeRunner6.Dtos
{
    public class GamePostRequest
    {
        public PlayerOperation Operation { get; set; }
    }

    public enum PlayerOperation
    {
        NotSet,
        Start,
        GoNorth,
        GoSouth,
        GoEast,
        GoWest
    }
}

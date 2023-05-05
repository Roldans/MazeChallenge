using System;
using System.Collections.Generic;
using System.Text;

namespace MazeRunner6
{
    public class GameDefinition
    {
        public Guid MazeUid { get; set; }
        public Guid GameUid { get; set; }
        public bool Completed { get; set; }

        public int CurrentPositionX { get; set; }

        public int CurrentPositionY { get; set; }
    }
}

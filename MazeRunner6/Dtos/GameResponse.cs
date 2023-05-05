using System;
using System.Collections.Generic;
using System.Text;

namespace MazeRunner6.Dtos
{
    public class GameResponse
    {
        public GameDefinition Game { get; set; }
        public MazeBlockView MazeBlockView { get; set; }
    }
}

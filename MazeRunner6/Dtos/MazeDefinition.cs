using System;
using System.Collections.Generic;
using System.Text;

namespace MazeRunner6
{
    public class MazeDefinition
    {
        public MazeDefinition()
        {
            this.Blocks = new List<MazeBlockView>();
        }

        public Guid MazeUid { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public List<MazeBlockView> Blocks { get; set; }
    }
}

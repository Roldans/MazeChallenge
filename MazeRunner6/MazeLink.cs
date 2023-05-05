using System;
using System.Collections.Generic;
using System.Text;

namespace MazeRunner6
{
    public class MazeLink
    {
        public MazeNode FromNode, ToNode;
        public MazeLink(MazeNode from_node, MazeNode to_node)
        {
            FromNode = from_node;
            ToNode = to_node;
        }
    }
}

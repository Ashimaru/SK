using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Assets.Scripts.Data
{
    class FDMNode
    {
        private float _deltaX;
        private float _deltaY;

        public FDMNode previousNode;
        public FDMNode nextNode;
        public FDMNode upperNode;
        public FDMNode lowerNode;
        public Vector2 position;
        public int index;

        public float DeltaX
        {
            get
            {
                if(_deltaX == 0)
                {
                    if (previousNode != null)
                    {
                        _deltaX = Math.Abs(position.x - previousNode.position.x);
                    }
                    else
                    {
                        _deltaX = Math.Abs(position.x - nextNode.position.x);
                    }
                }
                return _deltaX;
            }
        }

        public float DeltaY
        {
            get
            {
                if (_deltaY == 0)
                {
                    if (upperNode != null)
                    {
                        _deltaY = Math.Abs(position.y - upperNode.position.y);
                    }
                    else
                    {
                        _deltaY = Math.Abs(position.y - lowerNode.position.y);
                    }
                }
                return _deltaY;
            }
        }

        public FDMNode(int index, Vector2 position)
        {
            this.index = index;
            this.position = position;
        }
    }
}

using System;
using Microsoft.Xna.Framework;
namespace Oasis {
    public class TileWall : TileBase {
        public TileWall(bool blocksMovement = true, bool blocksLOS = true) : base(Color.LightGray, Color.Transparent, '#', blocksMovement, blocksLOS) {
            Name = "Wall";
        }
    }
}
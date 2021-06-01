﻿using System;
using Microsoft.Xna.Framework;
namespace Oasis {
    public class TileFloor : TileBase {
        public TileFloor(bool blocksMovement = false, bool blocksLOS = false) : base(Color.DarkGray, Color.Transparent, '.', blocksMovement, blocksLOS) {
            Name = "Floor";
        }
    }
}
using System;
using DefaultEcs;
using Microsoft.Xna.Framework;
using System.Text;
using SadConsole.Components;
using NeonCity;

namespace NeonCity {
    public class CommandManager {
        private Point _lastMoveEntityPoint;
        private Entity _lastMoveEntity;

        public CommandManager() {

        }

        public void MoveEntityBy(Entity entity, Point position) {
            _lastMoveEntity = entity;
            _lastMoveEntityPoint = position;
             
            entity.Get<Render>().Move(position.X, position.Y);
        }

        public void MoveEntityTo(Entity entity, Point position) {
            _lastMoveEntity = entity;
            _lastMoveEntityPoint = position;

            entity.Get<Render>().MoveTo(position);
        }

    }
}
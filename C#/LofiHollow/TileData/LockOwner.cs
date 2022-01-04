using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LofiHollow.TileData {
    public class LockOwner {
        public bool AlwaysLocked = false; // whether or not the tile is locked always (true) or only locked outside the times below (false)
        public string Owner = ""; // which NPC owns the tile, if any
        public int RelationshipUnlock = 0; // how much relationship with the owner unlocks the tile at any time
        public int MissionUnlock = -1; // which completed mission, if any, results in the lock being open always
        public int UnlockTime = 550; // The day time expressed as minutes that the lock is open
        public int LockTime = 600; // the day time expressed as minutes that the lock locks again
        public int ClosedGlyph = 32;
        public int OpenedGlyph = 32;
        public bool Closed = true;

        public bool CanOpen() {
            if (Owner == "" && MissionUnlock == -1)
                return true;

            if (GameLoop.World.Player.MetNPCs.ContainsKey(Owner) && GameLoop.World.Player.MetNPCs[Owner] >= RelationshipUnlock)
                return true;


            if (GameLoop.World.Player.GetCurrentTime() > UnlockTime && GameLoop.World.Player.GetCurrentTime() < LockTime)
                return true;

            // Do some code to see if the player has completed the mission needed to pass this lock
            return false;
        } 
    }
}

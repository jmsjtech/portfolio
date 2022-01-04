using System;
using SadRogue.Primitives;
using SadConsole;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using LofiHollow.TileData;

namespace LofiHollow {
    [JsonObject(MemberSerialization.OptIn)]
    public class TileBase : ColoredGlyph {
        [JsonProperty]
        public int TileID = 0;
        [JsonProperty]
        public bool IsBlockingMove = false;
        [JsonProperty]
        public bool IsBlockingLOS = false;
        [JsonProperty]
        public bool SpawnsMonsters = false;

        [JsonProperty]
        public string Name = "Grass";

        [JsonProperty]
        public int ForegroundR = 0;
        [JsonProperty]
        public int ForegroundG = 0;
        [JsonProperty]
        public int ForegroundB = 0;
        [JsonProperty]
        public int TileGlyph = 0;


        [JsonProperty]
        public Decorator Dec;

        [JsonProperty]
        public LockOwner Lock;


        [JsonProperty]
        public SkillableTile SkillableTile = null;

        [JsonConstructor]
        public TileBase() : base(Color.Black, Color.Transparent, 32) {
            Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
            Glyph = TileGlyph;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
            Glyph = TileGlyph; 
        }


        public TileBase(TileBase other) {
            IsBlockingMove = other.IsBlockingMove;
            IsBlockingLOS = other.IsBlockingLOS;
            Name = other.Name;
            TileID = other.TileID;
            SpawnsMonsters = other.SpawnsMonsters;

            ForegroundR = other.ForegroundR;
            ForegroundG = other.ForegroundG;
            ForegroundB = other.ForegroundB;
            TileGlyph = other.TileGlyph;
            SkillableTile = other.SkillableTile;

            Dec = other.Dec;
            Lock = other.Lock;

            Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
            Glyph = TileGlyph;
        } 

        public TileBase(int index) {
            if (GameLoop.World.tileLibrary.ContainsKey(index)) {
                TileBase other = GameLoop.World.tileLibrary[index];
                IsBlockingMove = other.IsBlockingMove;
                IsBlockingLOS = other.IsBlockingLOS;
                Name = other.Name;
                TileID = other.TileID;
                SpawnsMonsters = other.SpawnsMonsters;

                ForegroundR = other.ForegroundR;
                ForegroundG = other.ForegroundG;
                ForegroundB = other.ForegroundB;
                TileGlyph = other.TileGlyph;
                SkillableTile = other.SkillableTile;

                Dec = other.Dec;
                Lock = other.Lock;

                Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
                Glyph = TileGlyph;
            }
        } 

        public void SetNewFG(Color fg, int glyph) {
            Foreground = fg;
            Glyph = glyph;
        }

        public void UpdateAppearance() {
            Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
            Glyph = TileGlyph;
        }

        public void Shade() {
            Foreground = new Color(ForegroundR, ForegroundG, ForegroundB, 150);
        }

        public void Unshade() {
            Foreground = new Color(ForegroundR, ForegroundG, ForegroundB);
        }
    }
}
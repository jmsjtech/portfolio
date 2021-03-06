using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Newtonsoft.Json;
using TearsInRain.Entities;
using Microsoft.Xna.Framework;
using TearsInRain.Tiles;

namespace TearsInRain.Serializers {
    public class MapJsonConverter : JsonConverter<Map> {
        public override void WriteJson(JsonWriter writer, Map value, JsonSerializer serializer) {
            serializer.Serialize(writer, (MapSerialized)value);
        }

        public override Map ReadJson(JsonReader reader, Type objectType, Map existingValue,
                                        bool hasExistingValue, JsonSerializer serializer) {
            try {
                return serializer.Deserialize<MapSerialized>(reader);
            } catch (Exception e) {
                System.Console.WriteLine(e);
            }

            return null;
        }
    }

    /// <summary>
    /// Serialized instance of a <see cref="Map"/>.
    /// </summary>
    [DataContract]
    public class MapSerialized {
        [DataMember] public Dictionary<string, TileBase> tiles;
        [DataMember] public int mapW;
        [DataMember] public int mapH;
        [DataMember] public List<Actor> entities;
        [DataMember] public List<TerrainFeature> terrain;



        public static implicit operator MapSerialized(Map map) {
            List<Actor> ents = new List<Actor>();
            List<TerrainFeature> TFs = new List<TerrainFeature>();
            Dictionary<string, TileBase> stringTiles = new Dictionary<string, TileBase>();

            foreach (Entity entity in map.Entities.Items) {
                if (entity is TerrainFeature terr) {
                    TFs.Add(terr);
                } else if (entity is Actor actor && !(entity is Player)) {
                    ents.Add(actor);
                } 
            }


            for(int i = 0; i < map.Tiles.Length; i++) {
                Point loc = map.idx_xy(i);
                string simplePos = loc.X + "," + loc.Y;
                stringTiles.Add(simplePos, map.Tiles[i]);
            }



            var sObj = new MapSerialized() {
                tiles = stringTiles,
                entities = ents,
                terrain = TFs,
                mapW = map.Width,
                mapH = map.Height,
            };

            return sObj;
        }

        public static implicit operator Map(MapSerialized sObj) {
            Dictionary<Point, TileBase> rebuiltTiles = new Dictionary<Point, TileBase>();

            foreach (KeyValuePair<string, TileBase> tile in sObj.tiles) {
                string[] posString = tile.Key.Split(',');
                Point newPos = new Point(Convert.ToInt32(posString[0]), Convert.ToInt32(posString[1]));

                rebuiltTiles.Add(newPos, tile.Value);
            }

            TileBase[] tiles = new TileBase[600];



            Map map = new Map(tiles, sObj.mapW, sObj.mapH);

            map.Entities = new GoRogue.MultiSpatialMap<Entity>();

            for (int i = 0; i < sObj.entities.Count; i++) {
                map.Add(sObj.entities[i]);
            }

            for (int i = 0; i < sObj.terrain.Count; i++) {
                map.Add(sObj.terrain[i]);
            }

            return map;
        }
    }
}
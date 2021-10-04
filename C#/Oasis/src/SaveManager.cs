using DefaultEcs.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oasis {
    public class SaveManager {
        ISerializer serializer = new BinarySerializer();

        public void save() {
            using (Stream stream = File.Create("./world.txt")) {
                serializer.Serialize(stream, GameLoop.gs.ecs);
            }
        }

        public void load() {
            using (Stream stream = File.OpenRead("./world.txt")) {
                GameLoop.gs.ecs = serializer.Deserialize(stream);
            }
        }
    }
}

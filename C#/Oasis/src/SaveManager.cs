using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Oasis {
    //short helper class to ignore some properties from serialization
    public class IgnorePropertiesResolver : DefaultContractResolver {
        private readonly HashSet<string> ignoreProps;
        public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore) {
            this.ignoreProps = new HashSet<string>(propNamesToIgnore);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (this.ignoreProps.Contains(property.PropertyName)) {
                property.ShouldSerialize = _ => false;
            }
            return property;
        }
    }

    public class SaveManager {
        public void save() {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new IgnorePropertiesResolver(new[] { "World" });

            GameLoop.UIManager.MessageLog.Add(JsonConvert.SerializeObject(GameLoop.World.players, settings));

        }

        public void load() {
        }
    }
}

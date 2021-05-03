using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Newtonsoft.Json;
using TearsInRain.Entities;
using Microsoft.Xna.Framework;
using TearsInRain.src;

namespace TearsInRain.Serializers {
    public class ActorJsonConverter : JsonConverter<Actor> {
        public override void WriteJson(JsonWriter writer, Actor value, JsonSerializer serializer) {
            serializer.Serialize(writer, (ActorSerialized)value);
        }

        public override Actor ReadJson(JsonReader reader, Type objectType, Actor existingValue,
                                        bool hasExistingValue, JsonSerializer serializer) {
            return serializer.Deserialize<ActorSerialized>(reader);
        }
    }

    /// <summary>
    /// Serialized instance of a <see cref="Entity"/>.
    /// </summary>
    [DataContract]
    public class ActorSerialized {
        // Visuals
        [DataMember] public uint FG; // Foreground
        [DataMember] public uint BG; // Background
        [DataMember] public int G; // Glyph
        [DataMember] public string tilesheetName;

        [DataMember] public string Name;
        [DataMember] public int X;
        [DataMember] public int Y;

        [DataMember] public bool IsCrouched;
        [DataMember] public string HealthState;
        [DataMember] public int Speed;

        [DataMember] public Dictionary<string, SadConsole.CellDecorator> decorators;


        public static implicit operator ActorSerialized(Actor actor) {
            var serializedObject = new ActorSerialized() {
                FG = actor.Animation.CurrentFrame[0].Foreground.PackedValue,
                BG = actor.Animation.CurrentFrame[0].Background.PackedValue,
                G = actor.Animation.CurrentFrame[0].Glyph,
                tilesheetName = actor.tilesheetName,


                Name = actor.Name,

                X = actor.Position.X,
                Y = actor.Position.Y,

                IsCrouched = actor.IsCrouched,
                HealthState = actor.HealthState,

                decorators = actor.decorators,

                Speed = actor.Speed,
            };

            return serializedObject;
        }

        public static implicit operator Actor(ActorSerialized serializedObject) {
            var entity = new Actor(new Color(serializedObject.FG), new Color(serializedObject.BG), serializedObject.G);
            entity.tilesheetName = serializedObject.tilesheetName;

            entity.Name = serializedObject.Name;

            entity.decorators = serializedObject.decorators;

            entity.Position = new Point(serializedObject.X, serializedObject.Y);

            entity.HealthState = serializedObject.HealthState;

            entity.IsCrouched = serializedObject.IsCrouched;

            entity.Speed = serializedObject.Speed;

            return entity;
        }
    }
}
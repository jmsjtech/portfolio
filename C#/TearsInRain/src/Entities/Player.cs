using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace TearsInRain.Entities {

    [JsonObject(MemberSerialization.OptOut)]
    public class Player : Actor {
        public Player(Color foreground, Color background) : base(new Color(237, 186, 161), background, 1) {
            Name = "Player";
            TimeLastActed = 0;
            
            tilesheetName = "player";
            
            Font = SadConsole.Global.Fonts["player"].GetFont(SadConsole.Font.FontSizes.Quarter);

            decorators = new Dictionary<string, SadConsole.CellDecorator>();
            decorators.Add("eyeIris", new SadConsole.CellDecorator(Color.CornflowerBlue, 2, Microsoft.Xna.Framework.Graphics.SpriteEffects.None));
            decorators.Add("eyeWhite", new SadConsole.CellDecorator(Color.White, 3, Microsoft.Xna.Framework.Graphics.SpriteEffects.None));
            decorators.Add("hair", new SadConsole.CellDecorator(Color.PaleGoldenrod, 4, Microsoft.Xna.Framework.Graphics.SpriteEffects.None));

            Animation.AddDecorator(0, 1, decorators["eyeWhite"], decorators["eyeIris"], decorators["hair"]);

            Animation.IsDirty = true;


        }

        public Player(Actor actor, Point pos) : base(actor.Animation.CurrentFrame[0].Foreground, actor.Animation.CurrentFrame[0].Background, 1) {
            Name = actor.Name;

            Speed = actor.Speed;

            Position = pos;

            tilesheetName = actor.tilesheetName;
            UpdateFontSize(GameLoop.UIManager.hold.SizeMultiple);
            decorators = actor.decorators;
            RefreshDecorators();
        }
        

        public void RefreshDecorators() {
            Animation.ClearDecorators(0, 1);
            Animation.AddDecorator(0, 1, decorators["eyeWhite"], decorators["eyeIris"], decorators["hair"]);

            //for (int i = 0; i < Equipped.Length; i++) {
                
            //}


            Animation.IsDirty = true;

        }
    }
}
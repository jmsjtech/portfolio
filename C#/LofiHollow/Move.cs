namespace LofiHollow {
    public class Move {
        public int MoveID;
        public string Name;
        public string Type;
        public bool IsPhysical;
        public int Cost;
        public int Accuracy;
        public int Power;

        public Move(int ID, string name, string type, bool phys, int cost, int acc, int pow) {
            MoveID = ID;
            Name = name;
            Type = type;
            IsPhysical = phys;
            Cost = cost;
            Accuracy = acc;
            Power = pow;
        }

        public Move(int ID) {
            if (GameLoop.World.moveLibrary != null && GameLoop.World.moveLibrary.ContainsKey(ID)) {
                Move temp = GameLoop.World.moveLibrary[ID];

                MoveID = temp.MoveID;
                Name = temp.Name;
                Type = temp.Type;
                IsPhysical = temp.IsPhysical;
                Cost = temp.Cost;
                Accuracy = temp.Accuracy;
                Power = temp.Power; 
            }
        }
    }
}

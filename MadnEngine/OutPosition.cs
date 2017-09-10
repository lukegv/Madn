namespace MadnEngine
{
    public class OutPosition : OwnedPosition
    {
        public HomePosition HomeStart { get; set; }

        public OutPosition(Slot owner) : base(owner)
        {
            this.Owner.Out = this;
        }

        public override BoardPosition Next(Slot player)
        {
            return player.Equals(this.Owner) ? this.HomeStart : this.NextPosition;
        }
    }
}

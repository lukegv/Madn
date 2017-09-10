namespace MadnEngine
{
    /// <summary>
    /// 
    /// </summary>
    public class HomePosition : OwnedPosition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public HomePosition(Slot owner) : base(owner)
        {
            this.Owner.AddHome(this); // backreference to owners home positions
        }


        public override BoardPosition Next(Slot owner)
        {
            return (this.NextPosition != null && this.NextPosition.Current == null) ? this.NextPosition : null;
        }
    }
}

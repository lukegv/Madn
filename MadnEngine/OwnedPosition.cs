namespace MadnEngine
{
    /// <summary>
    /// The base class for any board position assigned to a specific slot
    /// </summary>
    public abstract class OwnedPosition : BoardPosition
    {
        /// <summary>
        /// The owner slot of this position
        /// </summary>
        public Slot Owner { get; set; }

        /// <summary>
        /// Base constructor for any board position assigned to a specific slot
        /// </summary>
        /// <param name="owner">The owner slot</param>
        public OwnedPosition(Slot owner)
        {
            this.Owner = owner;
        }
    }
}

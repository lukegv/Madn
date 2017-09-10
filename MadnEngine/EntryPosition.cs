namespace MadnEngine
{
    /// <summary>
    /// Represents an entry position
    /// </summary>
    public class EntryPosition : OwnedPosition
    {
        /// <summary>
        /// Creates a new entry position for a given slot
        /// </summary>
        /// <param name="owner"></param>
        public EntryPosition(Slot owner) : base(owner)
        {
            this.Owner.Entry = this; // backreference to owners entry position
        }
    }
}

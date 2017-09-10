using System;

namespace MadnEngine
{
    /// <summary>
    /// Represents any position on the game board
    /// </summary>
    public class BoardPosition
    {
        /// <summary>
        /// Notifies if the meeple occupation of the position changed
        /// </summary>
        public event EventHandler CurrentChanged;

        private void raiseCurrentChanged()
        {
            this.CurrentChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Static counter to assign unique position ids
        /// </summary>
        private static int IdCounter = 0;

        /// <summary>
        /// The number of the board position
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// The next position in the position circle
        /// </summary>
        public BoardPosition NextPosition { get; internal set; }

        /// <summary>
        /// The current occupation of the board position
        /// </summary>
        public Meeple Current { get; private set; }

        /// <summary>
        /// Creates a new board position
        /// </summary>
        internal BoardPosition()
        {
            this.ID = BoardPosition.IdCounter++;
            this.Current = null;
            this.NextPosition = null;
        }

        internal void SetCurrent(Meeple newMeeple)
        {
            this.Current = newMeeple;
            this.raiseCurrentChanged();
        }

        /// <summary>
        /// Get the next position for a meeple of a specific slot
        /// </summary>
        /// <remarks>Can be overwritten for special positions like out positions or home positions</remarks>
        /// <param name="slot">The meeples slot</param>
        /// <returns>The next position for the given slots meeple</returns>
        public virtual BoardPosition Next(Slot slot)
        {
            return this.NextPosition;
        }

        /// <summary>
        /// Determines the reached position, when going a number of steps from this position for a specific slot
        /// </summary>
        /// <param name="slot">The slot of the meeple to go</param>
        /// <param name="steps">The amount of steps to go</param>
        /// <returns>The reached board position or null, if there is no way to go this number of steps</returns>
        public BoardPosition Go(Slot slot, int steps)
        {
            BoardPosition position = this;
            for (int i = 0; i < steps; i++)
            {
                position = position.Next(slot);
                if (position == null) return null;
            }
            return position;
        }

        /// <summary>
        /// Indicates whether a meeple is on this position
        /// </summary>
        /// <returns>True, if a meeple is on this position, False otherwise</returns>
        public bool IsOccupied()
        {
            return this.Current != null;
        }

        /// <summary>
        /// Indicates whether a meeple of a given slot can walk onto this position
        /// </summary>
        /// <param name="slot">The slot of the meeple to walk on this position</param>
        /// <returns>False, if the position is occupied by a meeple of the same slot, True otherwise</returns>
        public bool IsWalkable(Slot slot)
        {
            return (this.Current == null || !this.Current.Owner.Equals(slot));
        }

        /// <summary>
        /// Indicates whether a meeple going on this slot is going to kick another meeple
        /// </summary>
        /// <param name="slot">The slot of the meeple to walk on this position</param>
        /// <returns>True, if the position is occupied by a meeple of another slot, False otherwise</returns>
        public bool IsKick(Slot slot)
        {
            return (this.Current != null && !this.Current.Owner.Equals(slot));
        }

        /// <summary>
        /// Calculates the number of steps to reach the target from this position for a specific player slot
        /// </summary>
        /// <remarks>
        /// This is NOT always the distance to the last home position. 
        /// If the last home positions are already occupied, the calculation stops at the last free home position.
        /// </remarks>
        /// <param name="slot">The player slot</param>
        /// <returns>The number of steps to reach the target from this position</returns>
        public int StepsToGo(Slot slot)
        {
            BoardPosition position = this;
            int counter = 0;
            while (position != null)
            {
                counter++;
                position = position.Next(slot);
            }
            return counter - 1;
        }
    }
}

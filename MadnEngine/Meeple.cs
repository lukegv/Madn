using System;

namespace MadnEngine
{
    public class Meeple
    {
        public event EventHandler AtChanged;

        private void raiseAtChanged()
        {
            this.AtChanged?.Invoke(this, new EventArgs());
        }

        private static int IdCounter = 0;
        public int ID { get; private set; }

        public Slot Owner { get; set; }

        public BoardPosition At { get; set; }

        public Meeple(Slot owner)
        {
            this.Owner = owner;
            this.ID = Meeple.IdCounter++ % this.Owner.Rules.MeeplesPerPlayer;
            this.At = null;
        }

        #region | Moves |

        internal void MOVE(BoardPosition position)
        {
            bool unusedChanged = true;
            if (position.Current != null) position.Current.KICK();
            if (this.At != null)
            {
                unusedChanged = false;
                this.At.SetCurrent(null);
            }
            this.At = position;
            this.At.SetCurrent(this);
            this.raiseAtChanged();
            if (unusedChanged) this.Owner.RaiseUnusedMeeplesChanged();
        }

        private void KICK()
        {
            this.At = null;
            this.Owner.RaiseUnusedMeeplesChanged();
            this.raiseAtChanged();
        }

        #endregion // Moves

        public int WayToGo()
        {
            return (this.At != null) ? this.At.StepsToGo(this.Owner) : int.MaxValue;
        }
    }
}

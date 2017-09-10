using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PropertyChanged;

using MadnEngine;

namespace MadnGame.ViewModel
{
    [ImplementPropertyChanged]
    public class BoardPositionVM
    {
        public BoardPosition Position { get; private set; }

        public bool IsEntry
        {
            get
            {
                return this.Position is EntryPosition;
            }
        }

        public bool IsOut
        {
            get
            {
                return this.Position is OutPosition;
            }
        }

        public bool IsHome
        {
            get
            {
                return this.Position is HomePosition;
            }
        }

        public int OwnerId
        {
            get
            {
                return (this.Position is OwnedPosition) ? (this.Position as OwnedPosition).Owner.ID : Int32.MinValue;
            }
        }

        public Meeple Current { get; private set; }

        public int CurrentId
        {
            get
            {
                return (this.Current != null) ? this.Current.Owner.ID : Int32.MinValue;
            }
        }

        public Move PossibleMove { get; private set; }

        public bool HasPossibleMove
        {
            get
            {
                return this.PossibleMove != null;
            }
        }

        public BoardPositionVM(BoardPosition position)
        {
            this.Position = position;
            this.PossibleMove = null;
            this.Current = this.Position.Current;
            this.Position.CurrentChanged += CurrentChanged;
        }

        private void CurrentChanged(object sender, EventArgs e)
        {
            this.Current = this.Position.Current;
        }

        public void SetMove(Move move)
        {
            this.PossibleMove = move;
        }

        public void ResetMove()
        {
            this.PossibleMove = null;
        }

    }
}

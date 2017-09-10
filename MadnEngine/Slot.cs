using System;
using System.Collections.Generic;
using System.Linq;

namespace MadnEngine
{
    /// <summary>
    /// 
    /// </summary>
    public class Slot
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler UnusedMeeplesChanged;

        /// <summary>
        /// Static counter to assign unique ids
        /// </summary>
        private static int IdCounter = 0;
        /// <summary>
        /// The unique id of the player slot
        /// </summary>
        /// <remarks>This is neither required nor used inside the madn engine! But in game implementations it is often much easier to use unique ids than comparing objects when assigning colors or icons.</remarks>
        public int ID { get; private set; }

        public IPlayer Player { get; private set; }

        public GameRules Rules  { get; private set; }

        private List<Meeple> Meeples;

        public EntryPosition Entry { get; internal set; }
        public OutPosition Out { get; internal set; }

        private List<HomePosition> Home; // Public access in special methods below

        public Slot(IPlayer player, GameRules rules)
        {
            this.ID = Slot.IdCounter++;
            this.Player = player;
            this.Rules = rules;
            this.Meeples = new List<Meeple>();
            for (int i = 0; i < this.Rules.MeeplesPerPlayer; i++)
            {
                this.Meeples.Add(new Meeple(this));
            }
            this.Home = new List<HomePosition>();
        }

        internal void AddHome(HomePosition homePos)
        {
            this.Home.Add(homePos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<HomePosition> GetHome()
        {
            // Copy the list with original items to prevent manipulation
            return this.Home.ToList();
        }

        internal void RaiseUnusedMeeplesChanged()
        {
            this.UnusedMeeplesChanged?.Invoke(this, new EventArgs());
        }

        internal Move Play(int diceResult, Game game)
        {
            List<Move> possibleMoves = this.getPossibleMoves(diceResult);
            if (possibleMoves.Count == 0) return null;
            Move decidedMove = null;
            if (possibleMoves.Count == 1)
            {
                decidedMove = possibleMoves[0];
            }
            else
            {
                decidedMove = this.Player.DecideForMove(possibleMoves, game);
                if (!possibleMoves.Contains(decidedMove)) throw new Exception("Illegal move!");
            }
            decidedMove.Execute();
            return decidedMove;
        }

        private List<Move> getPossibleMoves(int diceResult)
        {
            List<Move> moves = new List<Move>();
            if (this.Rules.ForceFree)
            {
                if (!this.Entry.IsWalkable(this) && this.Entry.Go(this, diceResult).IsWalkable(this))
                {
                    moves.Add(new Move(this.Entry.Current, this.Entry.Go(this, diceResult)));
                    return moves;
                }
            }
            if (this.Rules.CanLeave(diceResult) && this.IsAnyMeepleUnused() && this.Entry.IsWalkable(this))
            {
                moves.Add(new Move(this.GetUnusedMeeple(), this.Entry));
                if (this.Rules.ForceLeave)
                {
                    return moves;
                }
            }
            moves.AddRange(this.GetMeeplesOnBoard().Where(meeple => meeple.At.Go(this, diceResult) != null && meeple.At.Go(this, diceResult).IsWalkable(this)).Select(meeple => new Move(meeple, meeple.At.Go(this, diceResult))));
            if (this.Rules.ForceKick && moves.Any(move => move.Target.IsKick(this)))
            {
                moves = moves.Where(move => move.Target.IsKick(this)).ToList();
            }
            return moves;
        }

        public List<Meeple> GetMeeples()
        {
            return this.Meeples.ToList();
        }

        public IEnumerable<Meeple> GetMeeplesOnBoard()
        {
            return this.Meeples.Where(meeple => meeple.At != null);
        }

        public int UnusedMeepleCount()
        {
            return this.Meeples.Count(meeple => meeple.At == null);
        }

        public bool IsAnyMeepleUnused()
        {
            return this.Meeples.Any(meeple => meeple.At == null);
        }

        public Meeple GetUnusedMeeple()
        {
            return this.Meeples.FirstOrDefault(meeple => meeple.At == null);
        }

        public bool IsDone()
        {
            return this.Home.All(home => home.IsOccupied());
        }

        public bool IsInExtraDicePosition()
        {
            return (this.UnusedMeepleCount()
                + this.Home.Count(home => home.IsOccupied() ? ((home.NextPosition != null) ? home.NextPosition.IsOccupied() : true) : false)
                == this.Meeples.Count);
        }
    }
}

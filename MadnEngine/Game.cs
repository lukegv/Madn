using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MadnEngine
{
    /// <summary>
    /// 
    /// </summary>
    public class Game
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<Move> MoveExecuted;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<DiceRollEventArgs> DiceRolled;

        private List<Slot> Slots;
        private List<BoardPosition> Board;

        /// <summary>
        /// Gets the rules of this madn game
        /// </summary>
        public GameRules Rules { get; private set; }

        /// <summary>
        /// Gets or sets the sleep time before each round is played
        /// </summary>
        public TimeSpan RoundSleepTime { get; set; }
        /// <summary>
        /// Gets or sets whether or not to wait for confirmation after a dice result
        /// </summary>
        public bool WaitOnDiceResult { get; set; }
        /// <summary>
        /// Gets or sets whether or not to wait for confirmation after an executed move 
        /// </summary>
        public bool WaitOnExecutedMove { get; set; }

        // Waiters for event acknowledgement
        private AutoResetEvent diceResultAck;
        private AutoResetEvent executedMoveAck;

        /// <summary>
        /// Creates a new madn game with a list of players and a set of game rules
        /// </summary>
        /// <param name="players">The list of players participating in this game</param>
        /// <param name="rules">Optionally the rules to play this game, if not provided default rules are used.</param>
        public Game(List<IPlayer> players, GameRules rules = null)
        {
            // Set default round sleep time (1 ms)
            this.RoundSleepTime = new TimeSpan(0, 0, 0, 0, 1);
            // Set default wait rules and waiters (both disabled)
            this.WaitOnDiceResult = false;
            this.WaitOnExecutedMove = false;
            this.diceResultAck = new AutoResetEvent(false);
            this.executedMoveAck = new AutoResetEvent(false);
            // Copy / Create rules to prevent later manipulation
            this.Rules = new GameRules(rules);
            this.Slots = new List<Slot>();
            this.Board = new List<BoardPosition>();
            BoardPosition first = null;
            BoardPosition current = null;
            for (int slotCounter = 0; slotCounter < players.Count; slotCounter++)
            {
                Slot slot = new Slot(players[slotCounter], this.Rules);
                this.Slots.Add(slot);
                OutPosition outPos = new OutPosition(slot);
                this.Board.Add(outPos);
                // Store the first position to close the circle in the end
                if (slotCounter == 0) first = outPos;
                if (current != null) current.NextPosition = outPos;
                current = outPos;
                HomePosition currentHome = null;
                for (int homeCounter = 0; homeCounter < this.Rules.MeeplesPerPlayer; homeCounter++)
                {
                    HomePosition homePos = new HomePosition(slot);
                    this.Board.Add(homePos);
                    if (homeCounter == 0) (current as OutPosition).HomeStart = homePos;
                    if (currentHome != null) currentHome.NextPosition = homePos;
                    currentHome = homePos;
                }
                EntryPosition entryPos = new EntryPosition(slot);
                this.Board.Add(entryPos);
                current.NextPosition = entryPos;
                current = entryPos;
                for (int positionCounter = 0; positionCounter < this.Rules.NormalPositionsPerPlayer; positionCounter++) {
                    BoardPosition pos = new BoardPosition();
                    this.Board.Add(pos);
                    current.NextPosition = pos;
                    current = pos;
                }
            }
            // Close the circle with the first item
            current.NextPosition = first;
        }

        /// <summary>
        /// Starts the game
        /// </summary>
        public void Start()
        {
            // Start the game loop in a new thread
            (new Thread(() => this.GameLoop())).Start();
        }

        /// <summary>
        /// Stops the game
        /// </summary>
        public void Stop()
        {
            this.shouldStop = true;
            // Prevent blocking waiters
            this.diceResultAck.Set();
            this.executedMoveAck.Set();
        }

        private volatile bool shouldStop = false;

        private void GameLoop()
        {
            int currentSlotId = Dice.GetStartSlot(this.Slots.Count);
            int currentExtraDices = 0;
            Slot currentSlot = null;
            while (!this.IsGameDone() && !this.shouldStop)
            {
                // Wait for a configured timespan
                Thread.Sleep(this.RoundSleepTime);
                // Assign the current slot
                currentSlot = this.Slots[currentSlotId];
                // Roll the dice
                int diceResult = Dice.Roll();
                this.raiseDiceRollEvent(currentSlot, diceResult);
                if (this.WaitOnDiceResult) this.diceResultAck.WaitOne();
                // Let the player execute a move
                Move executedMove = currentSlot.Play(diceResult, this);
                this.raiseMoveExecutedEvent(executedMove);
                if (this.WaitOnExecutedMove) this.executedMoveAck.WaitOne();
                // Check for a reroll
                if (this.Rules.CanReroll(diceResult)) continue;
                // Check for available extra dices
                if ((executedMove == null) && currentSlot.IsInExtraDicePosition() && ++currentExtraDices < this.Rules.ExtraDices) continue;
                // Switch to the next player
                currentSlotId = (currentSlotId == this.Slots.Count - 1) ? 0 : ++currentSlotId;
                // Reset the extra dice counter
                currentExtraDices = 0;
            }
        }

        private void raiseDiceRollEvent(Slot slot, int diceResult)
        {
            this.DiceRolled?.Invoke(this, new DiceRollEventArgs(slot, diceResult));
        }

        private void raiseMoveExecutedEvent(Move move)
        {
            this.MoveExecuted?.Invoke(this, move);
        }

        /// <summary>
        /// Continues the game after a dice result, if the WaitOnDiceResult is enabled
        /// </summary>
        public void ConfirmDiceResult()
        {
            this.diceResultAck.Set();
        }

        /// <summary>
        /// Continues the game after an executed move, if the WaitOnExecutedMove is enabled
        /// </summary>
        public void ConfirmExecutedMove()
        {
            this.executedMoveAck.Set();
        }

        /// <summary>
        /// Indicates whether the game is done depending on the rules
        /// </summary>
        /// <returns>True, if all players are done or any player is done (depending on the rules), False otherwise</returns>
        public bool IsGameDone()
        {
            return this.Rules.WaitForAllPlayers ? this.AreAllPlayersDone() : this.IsAnyPlayerDone();
        }

        /// <summary>
        /// Gets all positions on the game board
        /// </summary>
        /// <returns>A copied list of all board positions</returns>
        public List<BoardPosition> GetPositions()
        {
            // Return a copy of the list (with original elements) to prevent manipulation
            return this.Board.ToList();
        }

        /// <summary>
        /// Gets all player slots
        /// </summary>
        /// <returns>A copied list of all player slots</returns>
        public List<Slot> GetSlots()
        {
            // Return a copy of the list (with original elements) to prevent manipulation
            return this.Slots.ToList();
        }

        /// <summary>
        /// Indicates whether any player in the game is done
        /// </summary>
        /// <returns></returns>
        public bool IsAnyPlayerDone()
        {
            return this.Slots.Any(slot => slot.IsDone());
        }

        /// <summary>
        /// Indicates whether all players in the game are done
        /// </summary>
        /// <returns></returns>
        public bool AreAllPlayersDone()
        {
            return this.Slots.All(slot => slot.IsDone());
        }
    }

    /// <summary>
    /// Event arguments to represent a dice roll
    /// </summary>
    public class DiceRollEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the slot whose player rolled the dice
        /// </summary>
        public Slot Slot { get; private set; }
        /// <summary>
        /// Gets the dice roll result
        /// </summary>
        public int DiceResult { get; private set; }

        internal DiceRollEventArgs(Slot slot, int diceResult)
        {
            this.Slot = slot;
            this.DiceResult = diceResult;
        }
    }
}

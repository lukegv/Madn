using System.Collections.Generic;
using System.Linq;

namespace MadnEngine
{
    /// <summary>
    /// Represents a set of madn game rules
    /// </summary>
    public class GameRules
    {
        /// <summary>
        /// Gets or sets the force kick rule (default is True)
        /// </summary>
        /// <remarks></remarks>
        public bool ForceKick { get; set; }
        /// <summary>
        /// Gets or sets the force free rule (default is True)
        /// </summary>
        public bool ForceFree { get; set; }
        /// <summary>
        /// Gets or sets the force leave rule (default is True)
        /// </summary>
        public bool ForceLeave { get; set; }

        /// <summary>
        /// Gets or sets the list of dice result which allow leaving (default is 6 only)
        /// </summary>
        public List<int> LeaveOn { get; set; }
        /// <summary>
        /// Gets or sets the list of dice results which allow rerolling (default is 6 only)
        /// </summary>
        public List<int> RerollOn { get; set; }
        /// <summary>
        /// Gets or sets the number of extra dices (default is 3)
        /// </summary>
        public int ExtraDices { get; set; }

        /// <summary>
        /// Gets or sets whether to wait for all players to finish the game (default is True)
        /// </summary>
        /// <remarks></remarks>
        public bool WaitForAllPlayers { get; set; }

        /// <summary>
        /// Gets or sets the number of meeples per player (default is 4)
        /// </summary>
        public int MeeplesPerPlayer { get; set; }
        /// <summary>
        /// Gets or sets the number of normal board positions per player (default is 8)
        /// </summary>
        public int NormalPositionsPerPlayer { get; set; }

        /// <summary>
        /// Creates either the default game rule set or copies all rules from a given set
        /// </summary>
        /// <param name="rules">An optional set to copy the rules from</param>
        public GameRules(GameRules rules = null)
        {
            if (rules != null)
            {
                // Copy rules from given rule set
                this.ForceKick = rules.ForceKick;
                this.ForceFree = rules.ForceFree;
                this.ForceLeave = rules.ForceLeave;
                this.ExtraDices = rules.ExtraDices;
                this.RerollOn = rules.RerollOn.ToList();
                this.LeaveOn = rules.LeaveOn.ToList();
                this.WaitForAllPlayers = rules.WaitForAllPlayers;
                this.MeeplesPerPlayer = rules.MeeplesPerPlayer;
                this.NormalPositionsPerPlayer = rules.NormalPositionsPerPlayer;
            }
            else
            {
                // Set default rules
                this.ForceKick = true;
                this.ForceFree = true;
                this.ForceLeave = true;
                this.ExtraDices = 3;
                this.RerollOn = (new int[] { 6 }).ToList();
                this.LeaveOn = (new int[] { 6 }).ToList();
                this.WaitForAllPlayers = true;
                this.MeeplesPerPlayer = 4;
                this.NormalPositionsPerPlayer = 8;
            }
        }

        /// <summary>
        /// Determines whether a player can reroll after rolling a specific dice result
        /// </summary>
        /// <param name="diceResult"></param>
        /// <returns></returns>
        public bool CanReroll(int diceResult)
        {
            return this.RerollOn.Contains(diceResult);
        }

        /// <summary>
        /// Determines whether a players meeple can leave after rolling a specific dice result
        /// </summary>
        /// <param name="diceResult"></param>
        /// <returns></returns>
        public bool CanLeave(int diceResult)
        {
            return this.LeaveOn.Contains(diceResult);
        }

    }
}

using System;

namespace MadnEngine
{
    /// <summary>
    /// Provides random numbers
    /// </summary>
    internal static class Dice
    {
        private static Random Generator = new Random();

        /// <summary>
        /// Rolls a normal dice with numbers from 1 to 6
        /// </summary>
        /// <returns>The result of the dice roll</returns>
        public static int Roll()
        {
            return Dice.Generator.Next(1, 7);
        }

        /// <summary>
        /// Randomly determines the number of the start slot
        /// </summary>
        /// <param name="slotCount">The amount of slots in this game</param>
        /// <returns>A random number between the first and the last slot number</returns>
        public static int GetStartSlot(int slotCount)
        {
            return Dice.Generator.Next(0, slotCount);
        }
    }
}

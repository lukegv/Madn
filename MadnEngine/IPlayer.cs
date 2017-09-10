using System.Collections.Generic;

namespace MadnEngine
{
    /// <summary>
    /// Defines the necessary actions of a madn player
    /// </summary>
    /// <remarks>This interface can be implemented in a madn bot or in an user interface to let a human player take place in the game.</remarks>
    public interface IPlayer
    {
        Move DecideForMove(List<Move> possibleMoves, Game game);
    }
}

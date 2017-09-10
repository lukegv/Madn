namespace MadnEngine
{
    public class Move
    {
        public Meeple Meeple { get; private set; }
        public BoardPosition Target { get; private set; }

        internal Move(Meeple meeple, BoardPosition target)
        {
            this.Meeple = meeple;
            this.Target = target;
        }

        internal void Execute()
        {
            this.Meeple.MOVE(this.Target);
        }
    }
}

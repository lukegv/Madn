using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MadnEngine;

using MadnGame.ViewModel;

namespace MadnGame
{
    public partial class MainWindow : Window, IPlayer
    {
        private Game GameInstance;
        private GameVM ViewModel;

        public MainWindow()
        {
            this.GameInstance = new Game((new IPlayer[] { this, this, this, this}).ToList());
            this.ViewModel = new GameVM(this.GameInstance);
            this.ViewModel.MoveDecided += onMoveDecided;
            this.DataContext = this.ViewModel;
            this.InitializeComponent();
            this.GameInstance.Start();
        }

        private AutoResetEvent moveDecidedEvent = new AutoResetEvent(false);
        private Move decidedMove = null;

        public Move DecideForMove(List<Move> possibleMoves, Game game)
        {
            this.ViewModel.AddPossibleMoves(possibleMoves);
            this.moveDecidedEvent.WaitOne();
            return this.decidedMove;
        }

        private void onMoveDecided(object sender, Move move)
        {
            this.decidedMove = move;
            // remove possible moves
            this.moveDecidedEvent.Set();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.ViewModel.MoveDecided -= onMoveDecided;
            this.GameInstance.Stop();
            base.OnClosing(e);
        }
    }
}

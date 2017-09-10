using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PropertyChanged;
using MadnEngine;
using GalaSoft.MvvmLight.CommandWpf;

namespace MadnGame.ViewModel
{
    [ImplementPropertyChanged]
    public class GameVM
    {
        public event EventHandler<Move> MoveDecided;

        public RelayCommand<Move> MoveDecidedCommand { get; private set; }

        public ObservableCollection<BoardPositionVM> Positions { get; private set; }
        public ObservableCollection<SlotVM> Players { get; private set; }

        public IEnumerable<BoardPositionVM> WayPositions
        {
            get
            {
                return this.Positions.Where(pos => !pos.IsHome);
            }
        }

        public IEnumerable<IGrouping<int, BoardPositionVM>> HomePositions
        {
            get
            {
                return this.Positions.Where(pos => pos.IsHome).GroupBy(pos => pos.OwnerId);
            }
        }

        public GameVM(Game game)
        {
            
            this.Positions = new ObservableCollection<BoardPositionVM>(game.GetPositions().Select(pos => new BoardPositionVM(pos)));
            this.Players = new ObservableCollection<SlotVM>(game.GetSlots().Select(player => new SlotVM(player)));
        }

        public void AddPossibleMoves(List<Move> possibleMoves)
        {
            foreach (Move possible in possibleMoves)
            {
                this.Positions.Single(pos => pos.Current == possible.Meeple).SetMove(possible);
            }
        }
    }
}

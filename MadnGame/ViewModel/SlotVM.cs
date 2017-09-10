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
    public class SlotVM
    {
        public Slot Slot { get; private set; }

        public int UnusedMeepleCount { get; private set; }

        public SlotVM(Slot slot)
        {
            this.Slot = slot;
            this.UnusedMeepleCount = this.Slot.UnusedMeepleCount();
            this.Slot.UnusedMeeplesChanged += UnusedMeeplesChanged;
        }

        private void UnusedMeeplesChanged(object sender, EventArgs e)
        {
            this.UnusedMeepleCount = this.Slot.UnusedMeepleCount();
        }
    }
}

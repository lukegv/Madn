using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using MadnGame.ViewModel;

namespace MadnGame.Helpers
{
    public class PositionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BoardPositionTemplate { get; set; }
        public DataTemplate EntryPositionTemplate { get; set; }
        public DataTemplate OutPositionTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is BoardPositionVM)
            {
                BoardPositionVM pos = item as BoardPositionVM;
                if (pos.IsOut) return this.OutPositionTemplate;
                if (pos.IsEntry) return this.EntryPositionTemplate;
                return this.BoardPositionTemplate;
            }
            else
            {
                throw new ArgumentException("Wrong item type");
            }
        }
    }
}

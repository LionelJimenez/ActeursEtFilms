using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace ActeursETFilms
{
    class DoubleTemplate: DataTemplateSelector
    {
        public DataTemplate HTempl { get; set; }
        public DataTemplate FTempl { get; set; }

        public override DataTemplate  SelectTemplate(object item, DependencyObject container)
{
    acteurs actr = (acteurs)item;

    if (actr.sexe == "H")
        return HTempl;
    return FTempl;
}

    }
}

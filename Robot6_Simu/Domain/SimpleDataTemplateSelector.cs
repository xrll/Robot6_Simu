using System.Windows;
using System.Windows.Controls;

namespace Robot6_Simu.Domain
{
    public class SimpleDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? FixedTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            return FixedTemplate;
        }
    }
}
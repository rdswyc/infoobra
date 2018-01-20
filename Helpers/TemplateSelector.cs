using System.Windows;
using System.Windows.Controls;

using InfoObra.ViewModels;

namespace InfoObra.Helpers
{
    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            ContentTemplate = SelectTemplate(newContent, this);
        }
    }

    public class TemplateSelector : DataTemplateSelector
    {
        public DataTemplate diario { get; set; }

        public DataTemplate solicitacao { get; set; }

        public DataTemplate produtividade { get; set; }

        public DataTemplate planejamento_operacional { get; set; }

        public DataTemplate restricoes { get; set; }

        public DataTemplate qualidade { get; set; }

        public DataTemplate fale_conosco { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DisplayItem Item = item as DisplayItem;

            if (Item != null)
            {
                return (DataTemplate)this.GetType().GetProperty(Item.List.Title).GetValue(this);
            }

            return base.SelectTemplate(item, container);
        }
    }
}

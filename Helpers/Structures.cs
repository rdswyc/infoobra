using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using InfoObra.Services;

namespace InfoObra.Helpers
{
    public class spField
    {
        public IEnumerable<string> Choices { get; set; }
        public string DisplayName { get; set; }
        public string List { get; set; }
        public string Name { get; set; }
        public Boolean Required { get; set; }
        public string ShowField { get; set; }
        public string Type { get; set; }
    }

    public class spList
    {
        public string Title { get; set; }
        public string DisplayName { get; set; }
        public string ViewName { get; set; }
        public string hexColor { get; set; }

        public spList(string title, string viewName = Constants.DefaultView)
        {
            this.Title = title;
            this.ViewName = viewName;
        }

        public SolidColorBrush ColorBrush
        {
            get
            {
                if (string.IsNullOrEmpty(hexColor))
                {
                    return (SolidColorBrush)App.Current.Resources["PhoneChromeBrush"];
                }
                else
                {
                    return new SolidColorBrush(Color.FromArgb(
                        Convert.ToByte(hexColor.Substring(0, 2), 16),
                        Convert.ToByte(hexColor.Substring(2, 2), 16),
                        Convert.ToByte(hexColor.Substring(4, 2), 16),
                        Convert.ToByte(hexColor.Substring(6, 2), 16)
                        ));
                }
            }
        }
    }

    public class KeyedList<TKey, TItem> : List<TItem>
    {
        public TKey Key { protected set; get; }

        public KeyedList(TKey key, IEnumerable<TItem> items) : base(items)
        {
            Key = key;
        }

        public KeyedList(IGrouping<TKey, TItem> grouping) : base(grouping)
        {
            Key = grouping.Key;
        }
    }
}
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Phone.Controls;

using InfoObra.Helpers;
using InfoObra.Resources;
using InfoObra.Services;

namespace InfoObra
{
    public partial class FieldsControl : UserControl
    {
        int index = 0;

        public FieldsControl()
        {
            InitializeComponent();

            this.Dispatcher.BeginInvoke(() =>
            {
                foreach (spField field in SharePoint.GetListFields(App.ViewModel.ActiveList.Title))
                {
                    switch (field.Type)
                    {
                        case "Text":
                        case "Note":
                        case "Number":
                        case "Week":

                            fieldText(field);
                            break;

                        case "Boolean":

                            fieldBoolean(field);
                            break;

                        case "Date":

                            fieldDate(field);
                            break;

                        case "Choice":

                            fieldChoice(field);
                            break;

                        case "Lookup":

                            fieldLookup(field);
                            break;

                        default:
                            break;
                    }
                }
            });
        }

        private void fieldText(spField field)
        {
            TextBlock Label = new TextBlock()
            {
                Text = field.DisplayName,
                Style = (Style)App.Current.Resources["FieldLabel"]
            };

            TextBox Text = new TextBox()
            {
                Style = (Style)App.Current.Resources["Field" + field.Type]
            };

            Text.KeyUp += (s1, e1) =>
            {
                if (e1.Key == Key.Enter)
                    ((Page)App.RootFrame.Content).Focus();
            };

            Text.SetBinding(TextBox.TextProperty, new Binding(string.Format("[{0}]", field.Name))
            {
                Mode = BindingMode.TwoWay,
                ValidatesOnNotifyDataErrors = true,
                NotifyOnValidationError = true
            });

            Label.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, ++index);
            Text.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, index);

            Container.Children.Add(Label);
            Container.Children.Add(Text);
        }

        private void fieldBoolean(spField field)
        {
            if (field.Name == "aprovado" && SharePoint.UserPermission == -1)
                return;

            CheckBox Check = new CheckBox()
            {
                Content = field.DisplayName,
                IsThreeState = false
            };

            Check.SetBinding(CheckBox.IsCheckedProperty, new Binding(string.Format("[{0}]", field.Name))
            {
                Mode = BindingMode.TwoWay
            });

            Check.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, ++index);

            Container.Children.Add(Check);
        }

        private void fieldDate(spField field)
        {
            DatePicker DatePick = new DatePicker()
            {
                Header = field.DisplayName,
                HeaderTemplate = (DataTemplate)this.Resources["Labels"]
            };

            DatePick.SetBinding(DatePicker.ValueProperty, new Binding(string.Format("[{0}]", field.Name))
            {
                Mode = BindingMode.TwoWay
            });

            DatePick.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, ++index);

            Container.Children.Add(DatePick);
        }

        private void fieldChoice(spField field)
        {
            ListPicker ChoicePick = new ListPicker()
            {
                Header = field.DisplayName,
                ItemsSource = field.Choices,
                HeaderTemplate = (DataTemplate)this.Resources["Labels"],
                FullModeItemTemplate = (DataTemplate)this.Resources["FullModeItem"]
            };

            if (App.ViewModel.ActiveList.Title == "qualidade")
            {
                ChoicePick.SetBinding(ListPicker.HeaderProperty, new Binding("[ficha]")
                {
                    Converter = new FichaValueConverter(),
                    ConverterParameter = field.Name.Replace("res_", string.Empty),
                    StringFormat = string.Format("{0}: {{0}}", new FieldValueConverter().Convert(App.ViewModel.ActiveList.Title, typeof(string), field.Name, System.Globalization.CultureInfo.CurrentCulture))
                });
            }

            ChoicePick.SelectionChanged += (s1, e1) =>
            {
                string item = ChoicePick.SelectedItem as string;

                if (!string.IsNullOrEmpty(item))
                {
                    if (DetailsPage.IsNew)
                        App.ViewModel.CreateItem[field.Name] = item;
                    else
                        App.ViewModel.SelectedItem[field.Name] = item;
                }
            };

            if (DetailsPage.IsNew)
            {
                if (App.ViewModel.CreateItem[field.Name] != null)
                    if (ChoicePick.ItemsSource.OfType<string>().Contains(App.ViewModel.CreateItem[field.Name].ToString()))
                        ChoicePick.SelectedItem = App.ViewModel.CreateItem[field.Name];
            }
            else
            {
                if (App.ViewModel.SelectedItem[field.Name] != null)
                    if (ChoicePick.ItemsSource.OfType<string>().Contains(App.ViewModel.SelectedItem[field.Name].ToString()))
                        ChoicePick.SelectedItem = App.ViewModel.SelectedItem[field.Name];
            }

            ChoicePick.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, ++index);

            Container.Children.Add(ChoicePick);
        }

        private void fieldLookup(spField field)
        {
            if (field.Name == "alterado_app")
                return;

            ListPicker LookupPick = new ListPicker()
            {
                Header = field.DisplayName,
                ItemsSource = SharePoint.GetParamItems(new spList(field.List), field.ShowField),
                HeaderTemplate = (DataTemplate)this.Resources["Labels"],
                FullModeItemTemplate = (DataTemplate)this.Resources["FullModeItem"]
            };

            LookupPick.SelectionChanged += (s1, e1) =>
            {
                if (LookupPick.SelectedItem != null)
                {
                    string ID = SharePoint.GetItemID(LookupPick.SelectedItem as string, field.List, field.ShowField);

                    if (DetailsPage.IsNew)
                        App.ViewModel.CreateItem[field.Name] = ID;
                    else
                        App.ViewModel.SelectedItem[field.Name] = ID;
                }
            };

            if (DetailsPage.IsNew)
            {
                if (App.ViewModel.CreateItem[field.Name] != null)
                    if (LookupPick.ItemsSource.OfType<string>().Contains(App.ViewModel.CreateItem[field.Name].ToString()))
                        LookupPick.SelectedItem = App.ViewModel.CreateItem[field.Name];
            }
            else
            {
                if (App.ViewModel.SelectedItem[field.Name] != null)
                    if (LookupPick.ItemsSource.OfType<string>().Contains(App.ViewModel.SelectedItem[field.Name].ToString()))
                        LookupPick.SelectedItem = App.ViewModel.SelectedItem[field.Name];
            }

            LookupPick.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, ++index);

            Container.Children.Add(LookupPick);
        }

    }
}

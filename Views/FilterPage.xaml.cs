using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using System.Globalization;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using InfoObra.Helpers;
using InfoObra.Resources;
using InfoObra.Services;
using System.Windows.Input;

namespace InfoObra
{
    public partial class FilterPage : PhoneApplicationPage
    {
        public static string EAP { get; set; }

        public static string Edificacao { get; set; }

        public static string Inicio { get; set; }

        public static string Frente { get; set; }

        public static bool hasFilter;

        public static string GetCurrentWeek()
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            return string.Format("{0}/{1}", dfi.Calendar.GetWeekOfYear(DateTime.Now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek), DateTime.Now.Year);
        }

        public FilterPage()
        {
            InitializeComponent();

            HeaderTitle.Text = string.Format("{0} {1}", AppResources.Filter, SharePoint.GetList(Constants.PlanList).DisplayName).ToUpper();

            // EAP
            Picker1.Header = SharePoint.GetField(Constants.PlanList, "eap_operacional").DisplayName;
            Picker1.ItemsSource = SharePoint.GetParamItems(new spList("eap"));
            if (Picker1.ItemsSource != null)
                if (Picker1.ItemsSource.OfType<string>().Contains(EAP))
                    Picker1.SelectedItem = EAP;

            // Edificação
            Picker2.Header = SharePoint.GetField(Constants.PlanList, "edificacao").DisplayName;
            Picker2.ItemsSource = SharePoint.GetParamItems(new spList("edificacao"));
            if (Picker2.ItemsSource != null)
                if (Picker2.ItemsSource.OfType<string>().Contains(Edificacao))
                    Picker2.SelectedItem = Edificacao;

            // Início
            Setter3.Hint = string.Format("{0}: {1}", AppResources.CurrentWeek, GetCurrentWeek());

            Header3.Text = SharePoint.GetField(Constants.PlanList, "inicio_planejado").DisplayName;
            if (!string.IsNullOrEmpty(Inicio)) Setter3.Text = Inicio;

            // Frente
            Picker4.Header = SharePoint.GetField(Constants.PlanList, "frente").DisplayName;
            Picker4.ItemsSource = SharePoint.GetField(Constants.PlanList, "frente").Choices;
            if (Picker4.ItemsSource != null) 
                if (Picker4.ItemsSource.OfType<string>().Contains(Frente))
                    Picker4.SelectedItem = Frente;

            Setter3.KeyUp += (s1, e1) =>
            {
                if (e1.Key == Key.Enter)
                    this.Focus();
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ApplicationBar = new ApplicationBar();
            ApplicationBarIconButton appBarButton;

            appBarButton = new ApplicationBarIconButton() { Text = AppResources.AddFilter, IconUri = new Uri("/Assets/AppBar/add.png", UriKind.Relative) };
            appBarButton.Click += Done_Click;
            ApplicationBar.Buttons.Add(appBarButton);

            appBarButton = new ApplicationBarIconButton() { Text = AppResources.ClearFilter, IconUri = new Uri("/Assets/AppBar/minus.png", UriKind.Relative) };
            appBarButton.Click += Cancel_Click;
            ApplicationBar.Buttons.Add(appBarButton);
        }

        void Done_Click(object sender, EventArgs e)
        {
            string error;

            if (!FieldValidators.Validate("", "inicio_planejado", Setter3.Text.ToString(), out error))
            {
                MessageBox.Show(error, AppResources.IncorrectValue, MessageBoxButton.OK);
                return;
            }

            EAP = Picker1.SelectedItem as string;

            Edificacao = Picker2.SelectedItem as string;

            Inicio = Setter3.Text;

            Frente = Picker4.SelectedItem as string;

            hasFilter = true;

            this.Dispatcher.BeginInvoke(() => { App.ViewModel.NotifyPropertyChanged("Items"); });

            NavigationService.GoBack();
        }

        void Cancel_Click(object sender, EventArgs e)
        {
            if (hasFilter)
            {
                hasFilter = false;

                this.Dispatcher.BeginInvoke(() => { App.ViewModel.NotifyPropertyChanged("Items"); });
            }

            NavigationService.GoBack();
        }
    }
}
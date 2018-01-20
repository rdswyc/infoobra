using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using InfoObra.Resources;
using InfoObra.Services;
using InfoObra.ViewModels;
using InfoObra.Helpers;

namespace InfoObra
{
    public partial class MainPage : PhoneApplicationPage
    {

    #region Page Main Events

        public MainPage()
        {
            InitializeComponent();

            this.DataContext = App.ViewModel;

            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.ViewModel.LoadFromServer();

            BuildAppBar();

            this.Loaded -= MainPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            HubSection2.Header = string.Format("{0}" + (FilterPage.hasFilter ? " ({1})" : string.Empty), AppResources.Feed, AppResources.Filter).ToUpper();
        }

    #endregion

    #region Container Items Click

        private void Tiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox Tiles = sender as ListBox;

            if (Tiles.SelectedIndex == -1)
                return;

            App.ViewModel.ActiveList = Tiles.SelectedItem as spList;
            App.ViewModel.CreateItem = new NewItem { DataProvider = SharePoint.GetViewModel(App.ViewModel.ActiveList.Title).DataProvider };

            DetailsPage.IsNew = true;
            NavigationService.Navigate(new Uri("/Views/DetailsPage.xaml", UriKind.Relative));

            Tiles.SelectedIndex = -1;
        }

        private void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Items.SelectedItem == null)
                return;

            DisplayItem item = Items.SelectedItem as DisplayItem;

            App.ViewModel.ActiveList = item.List;
            App.ViewModel.SelectedItem = new EditItem(SharePoint.GetViewModel(App.ViewModel.ActiveList.Title).DataProvider, item.ViewState) { ID = item.ID };

            DetailsPage.IsNew = false;
            NavigationService.Navigate(new Uri("/Views/DetailsPage.xaml", UriKind.Relative));

            Items.SelectedItem = null;
        }

        private void Offlines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Offlines.SelectedItem == null)
                return;

            DisplayItem item = Offlines.SelectedItem as DisplayItem;

            App.ViewModel.ActiveList = item.List;

            if (item.IsNew)
            {
                App.ViewModel.CreateItem = OfflineNew.GetItemById(item.ID);
                App.ViewModel.CreateItem.ID = item.ID;
            }
            else
            {
                App.ViewModel.SelectedItem = OfflineEdit.GetItemById(item.ID);
            }

            DetailsPage.IsNew = item.IsNew;
            NavigationService.Navigate(new Uri("/Views/DetailsPage.xaml", UriKind.Relative));

            Offlines.SelectedItem = null;
        }

    #endregion

    #region AppBar Buttons/Menus Click

        private void ContactList_Click(object sender, EventArgs e)
        {
            App.ViewModel.ActiveList = SharePoint.GetList(Constants.ContactList);
            App.ViewModel.CreateItem = new NewItem { DataProvider = SharePoint.GetViewModel(App.ViewModel.ActiveList.Title).DataProvider };

            DetailsPage.IsNew = true;
            NavigationService.Navigate(new Uri("/Views/DetailsPage.xaml", UriKind.Relative));
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AboutPage.xaml", UriKind.Relative));
        }

        private void Filter_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/FilterPage.xaml", UriKind.Relative));
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            App.ViewModel.LoadFromServer();
        }

        private void PostAll_Click(object sender, EventArgs e)
        {
            App.ViewModel.PostOfflines();
        }

    #endregion

    #region AppBar Features

        private void Hub_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ApplicationBar == null)
                return;

            ApplicationBarIconButton appBarButton;

            this.Dispatcher.BeginInvoke(() =>
            {
                switch (Hub.SelectedIndex)
                {
                    case 0:

                        ApplicationBar.Buttons.Clear();
                        ApplicationBar.Mode = ApplicationBarMode.Minimized;

                        break;

                    case 1:

                        ApplicationBar.Buttons.Clear();
                        ApplicationBar.Mode = ApplicationBarMode.Default;

                        appBarButton = new ApplicationBarIconButton() { Text = AppResources.Filter, IconUri = new Uri("/Assets/AppBar/calendar.png", UriKind.Relative) };
                        appBarButton.Click += Filter_Click;
                        ApplicationBar.Buttons.Add(appBarButton);

                        appBarButton = new ApplicationBarIconButton() { Text = AppResources.Refresh, IconUri = new Uri("/Assets/AppBar/refresh.png", UriKind.Relative) };
                        appBarButton.Click += Refresh_Click;
                        ApplicationBar.Buttons.Add(appBarButton);

                        break;

                    case 2:

                        ApplicationBar.Buttons.Clear();
                        ApplicationBar.Mode = ApplicationBarMode.Default;

                        appBarButton = new ApplicationBarIconButton() { Text = AppResources.PostAll, IconUri = new Uri("/Assets/AppBar/upload.png", UriKind.Relative) };
                        appBarButton.Click += PostAll_Click;
                        ApplicationBar.Buttons.Add(appBarButton);

                        break;

                    default:
                        break;
                }
            });
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar() { Mode = ApplicationBarMode.Minimized };

            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(SharePoint.GetList(Constants.ContactList).DisplayName);
            appBarMenuItem.Click += ContactList_Click;
            ApplicationBar.MenuItems.Add(appBarMenuItem);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.About);
            appBarMenuItem.Click += About_Click;
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

    #endregion

    }
}
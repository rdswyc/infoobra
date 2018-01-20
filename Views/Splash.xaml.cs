using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;

using InfoObra.Helpers;
using InfoObra.Resources;
using InfoObra.Services;

namespace InfoObra
{
    public partial class Splash : PhoneApplicationPage
    {
        private bool ready = false;
        private bool IsSelectingUser = false;
        private int counter = 0;

        public Splash()
        {
            InitializeComponent();
            this.Loaded += Splash_Loaded;
            this.BackKeyPress += Splash_BackKeyPress;
        }

        private async void Splash_Loaded(object sender, RoutedEventArgs e)
        {
            if (!SharePoint.HasViewModelOrKey(Constants.UserList))
            {
                ListLoader loader = new ListLoader(Constants.UserList);

                try
                {
                    await loader.LoadAsync(Constants.DefaultView);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, AppResources.NoStartTitle, MessageBoxButton.OK);
                    App.Current.Terminate();
                }
            }

            this.Dispatcher.BeginInvoke(() => { Info.Text = AppResources.Loading + "..."; });

            foreach (string param in SharePoint.Parameters)
            {
                if (SharePoint.HasViewModelOrKey(param))
                {
                    updateCounter(false);
                    continue;
                }

                ListLoader loader = new ListLoader(param);

                loader.viewModel.InitializationCompleted += (s1, e1) =>
                {
                    if (e1.Error != null)
                    {
                        MessageBox.Show(AppResources.NoStartMsg, AppResources.NoStartTitle, MessageBoxButton.OK);
                        App.Current.Terminate();
                    }
                };

                loader.viewModel.ViewDataLoaded += (s1, e1) =>
                {
                    if (e1.Error != null)
                    {
                        MessageBox.Show(AppResources.NoStartMsg, AppResources.NoStartTitle, MessageBoxButton.OK);
                        App.Current.Terminate();
                    }

                    updateCounter(true);
                };

                loader.Load(Constants.DefaultView);
            }

            SelectUser();

            this.Loaded -= Splash_Loaded;
        }

        private void LeftButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Users.SelectedIndex == -1)
                return;

            ready = true;

            AppInfo.User = Users.SelectedItem as string;

            this.Dispatcher.BeginInvoke(() =>
            {
                Popup.IsOpen = false;

                LayoutRoot.OpacityMask = null;
            });

            if (counter >= SharePoint.Parameters.Count)
            {
                closeSplash();
            }
        }

        void Splash_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Popup.IsOpen)
            {
                e.Cancel = true;
            }
        }

        private void SelectUser()
        {
            if (IsSelectingUser)
                return;

            IsSelectingUser = true;

            Users.ItemsSource = SharePoint.GetParamItems(new spList(Constants.UserList));

            if (SharePoint.HasViewModelOrKey(Constants.UserKey))
                Users.SelectedItem = AppInfo.User;

            this.Dispatcher.BeginInvoke(() =>
            {
                LayoutRoot.OpacityMask = new SolidColorBrush(Color.FromArgb(127, 127, 127, 127));

                LeftButton.Content = AppResources.Start.ToLower();

                Popup.IsOpen = true;
            });
        }

        private void updateCounter(bool AndProgress)
        {
            counter++;

            if (AndProgress)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    if (Progress.IsIndeterminate)
                        Progress.IsIndeterminate = false;

                    Progress.Value = counter / (float)SharePoint.Parameters.Count;
                });
            }

            if (counter >= SharePoint.Parameters.Count && ready)
            {
                closeSplash();
            }
        }

        private void closeSplash()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
                NavigationService.RemoveBackEntry();
            });
        }
    }
}
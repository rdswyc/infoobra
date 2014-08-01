using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.SharePoint.Phone.Application;

using InfoObra.Resources;
using InfoObra.ViewModels;

namespace InfoObra
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        public static bool IsNew;

        private EditItem EditModel;
        private NewItem NewModel;

        private CameraCaptureTask Camera = new CameraCaptureTask();
        private PhotoChooserTask PhotoChooser = new PhotoChooserTask();

        public DetailsPage()
        {
            InitializeComponent();

            this.Loaded += DetailsPage_Loaded;
        }

        void DetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            StartViewModel();

            this.Dispatcher.BeginInvoke(() =>
            {
                BuildAppBar();

                SystemTray.BackgroundColor = App.ViewModel.ActiveList.ColorBrush.Color;
                SystemTray.ProgressIndicator.Text = App.ViewModel.ActiveList.DisplayName +
                    (DeviceNetworkInformation.IsNetworkAvailable ? string.Empty : string.Format(" ({0})", AppResources.Offline.ToUpper()));
            });

            this.Loaded -= DetailsPage_Loaded;
        }

        private void StartViewModel()
        {
            if (IsNew)
            {
                NewModel = App.ViewModel.CreateItem;

                Camera.Completed += new EventHandler<PhotoResult>(NewModel.OnPhotoSelectionCompleted);
                PhotoChooser.Completed += new EventHandler<PhotoResult>(NewModel.OnPhotoSelectionCompleted);

                NewModel.InitializationCompleted += new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
                NewModel.ItemCreated += new EventHandler<ItemCreatedEventArgs>(OnItemCreated);

                if (!NewModel.IsInitialized)
                    NewModel.Initialize();

                this.DataContext = NewModel;
            }
            else
            {
                EditModel = App.ViewModel.SelectedItem;

                Camera.Completed += new EventHandler<PhotoResult>(EditModel.OnPhotoSelectionCompleted);
                PhotoChooser.Completed += new EventHandler<PhotoResult>(EditModel.OnPhotoSelectionCompleted);

                EditModel.InitializationCompleted += new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
                EditModel.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(OnItemUpdated);

                if (!EditModel.IsInitialized)
                    EditModel.Initialize();

                this.DataContext = EditModel;
            }
        }

        private void FinalizeViewModel()
        {
            if (IsNew)
            {
                NewModel.InitializationCompleted -= new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
                NewModel.ItemCreated -= new EventHandler<ItemCreatedEventArgs>(OnItemCreated);
            }
            else
            {
                EditModel.InitializationCompleted -= new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
                EditModel.ItemUpdated -= new EventHandler<ItemUpdatedEventArgs>(OnItemUpdated);
            }
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton() { Text = AppResources.Post, IconUri = new Uri("/Assets/AppBar/save.png", UriKind.Relative) };
            appBarButton.Click += MenuItem1_Click;
            ApplicationBar.Buttons.Add(appBarButton);

            appBarButton = new ApplicationBarIconButton() { Text = AppResources.Photo, IconUri = new Uri("/Assets/AppBar/camera.png", UriKind.Relative) };
            appBarButton.Click += MenuItem2_Click;
            ApplicationBar.Buttons.Add(appBarButton);
        }

        private void MenuItem1_Click(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.Focus();

                Container.OpacityMask = new SolidColorBrush(Color.FromArgb(127, 127, 127, 127));
                Container.IsHitTestVisible = false;

                Progress.VerticalAlignment = VerticalAlignment.Center;
                SaveBlock.Visibility = Visibility.Visible;
            });

            if (IsNew)
                NewModel.CreateItem();
            else
                EditModel.UpdateItem();
        }

        private void MenuItem2_Click(object sender, EventArgs e)
        {
            ApplicationBar.IsVisible = false;
            Popup.IsOpen = true;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (Popup.IsOpen)
            {
                e.Cancel = true;
                Popup.IsOpen = false;
                ApplicationBar.IsVisible = true;
                lstBoxAttachments.ItemsSource = null;
            }
        }

        private void TakePhoto_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Popup.IsOpen = false;
            ApplicationBar.IsVisible = true;

            lstBoxAttachments.SetBinding(ListBox.ItemsSourceProperty, new Binding("[Attachments]"));
            Camera.Show();
        }

        private void ChooseFile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Popup.IsOpen = false;
            ApplicationBar.IsVisible = true;

            lstBoxAttachments.SetBinding(ListBox.ItemsSourceProperty, new Binding("[Attachments]"));
            PhotoChooser.Show();
        }

        private void DoneEditing()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (this.NavigationService.CanGoBack)
                    this.NavigationService.GoBack();
            });
            FinalizeViewModel();
        }

    #region ViewModel Events

        void OnViewModelInitialization(object sender, InitializationCompletedEventArgs e)
        {
        }

        void OnItemCreated(object sender, ItemCreatedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (e.Error != null)
                {
                    if (!e.Error.GetType().Equals(typeof(WebException)) && !e.Error.GetType().Equals(typeof(ViewModelNotInitializedException)))
                    {
                        MessageBox.Show(e.Error.Message, e.Error.GetType().Name, MessageBoxButton.OK);

                        Container.OpacityMask = null;
                        Container.IsHitTestVisible = true;

                        Progress.VerticalAlignment = VerticalAlignment.Top;
                        SaveBlock.Visibility = Visibility.Collapsed;

                        return;
                    }
                }
                DoneEditing();
            });
        }

        private void OnItemUpdated(object sender, ItemUpdatedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (e.Error != null)
                {
                    if (!e.Error.GetType().Equals(typeof(WebException)) && !e.Error.GetType().Equals(typeof(ViewModelNotInitializedException)))
                    {
                        MessageBox.Show(e.Error.Message, e.Error.GetType().Name, MessageBoxButton.OK);

                        Container.OpacityMask = null;
                        Container.IsHitTestVisible = true;

                        Progress.VerticalAlignment = VerticalAlignment.Top;
                        SaveBlock.Visibility = Visibility.Collapsed;

                        return;
                    }
                }
                DoneEditing();
            });
        }

    #endregion
    }
}
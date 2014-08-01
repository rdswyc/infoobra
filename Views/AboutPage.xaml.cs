using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

using InfoObra.Resources;
using InfoObra.Services;

namespace InfoObra
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            SiteLink.NavigateUri = new Uri(App.Current.Resources["AboutUrl"].ToString(), UriKind.Absolute);

            if ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Collapsed)
                Logo.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("/Assets/InfoObra-light.png");

        }

        private void HyperlinkButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceDetailTask mktDetail = new MarketplaceDetailTask
            {
                ContentIdentifier = AppInfo.ProductID,
                ContentType = MarketplaceContentType.Applications
            };

            mktDetail.Show();
        }
    }
}
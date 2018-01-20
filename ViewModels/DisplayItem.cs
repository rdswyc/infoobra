using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization;
using Microsoft.SharePoint.Phone.Application;

using InfoObra.Helpers;
using InfoObra.Services;

namespace InfoObra.ViewModels
{
    [DataContract]
    public class DisplayItem : DisplayItemViewModelBase
    {
        /// <summary>
        /// Provides display values for fields of the List, given its name. Also used for data binding to Display form UI
        /// </summary>
        public override object this[string fieldName]
        {
            get
            {
                return base[fieldName];
            }
            set
            {
                base[fieldName] = value;
            }
        }

        public bool IsNew;

        public spList List
        {
            get
            {
                return SharePoint.GetList(DataProvider.ListTitle);
            }
        }

        public DateTime Modified
        {
            get
            {
                DateTime modified;
                DateTime.TryParse(this["Modified"].ToString(), out modified);
                return modified;
            }
        }

        public ImageSource Attachment
        {
            get
            {
                if (this["Attachments"].ToString() == Constants.Empty)
                    return null;

                return null;

                string fileName = this["Attachments"].ToString().Split(';')[0];

                string str = string.Format("{0}/Lists/{1}/Attachments/{2}/{3}", AppInfo.SiteUrl, DataProvider.ListTitle, ID, fileName);

                BitmapImage image = new BitmapImage(new Uri(str, UriKind.Absolute));
                image.DecodePixelType = DecodePixelType.Logical;

                if (image.PixelHeight > image.PixelWidth)
                {
                    image.DecodePixelWidth = 324;
                    image.DecodePixelHeight = 182;
                }
                else
                {
                    image.DecodePixelWidth = 182;
                    image.DecodePixelHeight = 324;
                }

                return image;
            }
        }

        /// <summary>
        /// Initializes the ViewModel properties
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Deletes the current ListItem from SharePoint server
        /// </summary>
        public override void DeleteItem()
        {
            base.DeleteItem();
        }
    }
}

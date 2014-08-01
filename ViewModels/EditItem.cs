using System;
using System.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using Microsoft.SharePoint.Phone.Application;

using InfoObra.Helpers;
using InfoObra.Services;

namespace InfoObra.ViewModels
{
    [DataContract]
    public class EditItem : EditItemViewModelBase
    {
        /// <summary>
        /// Provides edit values for fields of the List, given its name. Also used for data binding to Edit form UI
        /// </summary>
        public override object this[string fieldName]
        {
            get
            {
                return base[fieldName];
            }
            set
            {
                Validate(fieldName, value);
                base[fieldName] = value;
            }
        }

        [DataMember]
        public bool fromOffline { get; set; }

        public ImageSource Attachment
        {
            get
            {
                if (this["Attachments"].ToString() == Constants.Empty)
                    return null;

                string fileName = this["Attachments"].ToString().Split(';')[0];

                string str = string.Format("{0}/Lists/{1}/Attachments/{2}/{3}", AppInfo.SiteUrl, DataProvider.ListTitle, ID, fileName);

                return new BitmapImage(new Uri(str, UriKind.Absolute));
            }
        }

        public EditItem(ListDataProviderBase dataProvider, Dictionary<string, object> DisplayView)
        {
            this.DataProvider = dataProvider;

            if (!fromOffline)
            {
                foreach (spField field in SharePoint.GetListFields(dataProvider.ListTitle))
                {
                    if (DisplayView.ContainsKey(field.Name))
                    {
                        if (DisplayView[field.Name] != null)
                        {
                            this[field.Name] = DisplayView[field.Name];
                        }
                    }
                }
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
        /// Updates current instance of List Item to server
        /// </summary>
        public override void UpdateItem()
        {
            this["Modified"] = DateTime.Now.ToString();
            this[Constants.UserKey] = SharePoint.GetItemID(AppInfo.User, Constants.UserList);

            base.UpdateItem();
        }

        /// <summary>
        /// Item Updated Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void OnItemUpdated(object sender, ItemUpdatedEventArgs args)
        {
            base.OnItemUpdated(sender, args);

            if (args.Error == null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.RefreshList(SharePoint.GetList(DataProvider.ListTitle)); });

                if (fromOffline)
                {
                    OfflineEdit.Remove(ID);
                    Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.NotifyPropertyChanged("Offlines"); });
                }
            }
            else
            {
                if ((args.Error.GetType().Equals(typeof(WebException)) || args.Error.GetType().Equals(typeof(ViewModelNotInitializedException))))
                {
                    foreach (spField field in SharePoint.GetListFields(DataProvider.ListTitle))
                    {
                        if (field.Type == "Lookup")
                        {
                            this[field.Name] = SharePoint.GetItemValueFromID(this[field.Name].ToString(), field.List, field.ShowField);
                        }
                    }

                    fromOffline = true;

                    OfflineEdit.Add(this.ID, this);

                    Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.NotifyPropertyChanged("Offlines"); });
                }
            }
        }

        /// <summary>
        /// Code to Validate values for a field with given name
        /// </summary>
        /// <param name="fieldName">Name of the list Field</param>
        /// <param name="value">Value of the list field</param>
        public override void Validate(string fieldName, object value)
        {
            string error;

            if (FieldValidators.Validate(DataProvider.ListTitle, fieldName, value, out error))
            {
                RemoveAllErrors(string.Format("Item[{0}]", fieldName));
            }
            else
            {
                AddError(string.Format("Item[{0}]", fieldName), error);
            }

            if (IsInitialized)
                base.Validate(fieldName, value);
        }

        /// <summary>
        /// Code to execute when photo is selected from media library or a photo is taken using camera to add attachment to ListItem
        /// </summary>
        public override void OnPhotoSelectionCompleted(object sender, PhotoResult e)
        {
            base.OnPhotoSelectionCompleted(sender, e);
        }
    }
}
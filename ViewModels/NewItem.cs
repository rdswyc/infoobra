using System;
using System.Collections.Generic;
using System.Net;
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
    public class NewItem : NewItemViewModelBase
    {
        /// <summary>
        /// Provides values for fields of the List, given its name. Also used for data binding to New form UI
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

        /// <summary>
        /// Initializes the ViewModel properties
        /// </summary>
        public override void Initialize()
        {
            this[Constants.UserKey] = AppInfo.User;

            foreach (KeyValuePair<string, object> pair in new Dictionary<string, object>(ViewState))
            {
                if (pair.Value == null)
                {
                    ViewState.Remove(pair.Key);
                }
            }

            base.Initialize();
        }

        /// <summary>
        /// Code to execute when a NewItem needs to be created on server
        /// </summary>
        public override void CreateItem()
        {
            this["Modified"] = DateTime.Now.ToString();
            this[Constants.UserKey] = SharePoint.GetItemID(AppInfo.User, Constants.UserList);

            base.CreateItem();
        }

        /// <summary>
        /// Item Created Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void OnItemCreated(object sender, ItemCreatedEventArgs args)
        {
            base.OnItemCreated(sender, args);

            if (args.Error == null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.RefreshList(SharePoint.GetList(DataProvider.ListTitle)); });

                if (fromOffline)
                {
                    OfflineNew.Remove(ID);
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

                    OfflineNew.Add(ID == null ? new Random().Next(int.MaxValue).ToString() : ID, this);

                    Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModel.NotifyPropertyChanged("Offlines"); });
                }
            }
        }

        /// <summary>
        /// Code to Validate values for a field with given name
        /// </summary>
        /// <param name="fieldName">Name of the list field</param>
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
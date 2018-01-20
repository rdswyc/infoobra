using System;
using System.IO.IsolatedStorage;
using System.Xml.Linq;

namespace InfoObra.Services
{
    public class Constants
    {
        // SharePoint data model
        public const string DataModel = "DataModel.xml";
        public const double DaysSpan = 30;
        public const double PlanSpan = 30;
        public const int RowLimit = 256;

        // IsolatedStorageSettings keys
        public const string Version = "Version";
        public const string UserKey = "alterado_app";
        public const string OfflineNew = "NewItems";
        public const string OfflineEdit = "EditItems";

        // List titles
        public const string UserList = "usuario";
        public const string ContactList = "fale_conosco";
        public const string PlanList = "planejamento_operacional";

        // Data fields
        public const string DefaultField = "Title";
        public const string PermissionField = "liberacaoaprovado";
        public const string DefaultView = "AllItems";
        public const string Empty = "(Empty)";

        // Miscellaneous
        public const string Manifest = "WMAppManifest.xml";
        public static string[] DeprecatedKeys = { "FeedFilter", "DraftNew", "DraftEdit" };
    }

    public class AppInfo
    {
        private static string _title;
        public static string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                    _title = XElement.Load(Constants.Manifest).Element("App").Attribute("Title").Value;

                return _title;
            }
        }

        private static string _ver;
        public static string Version
        {
            get
            {
                if (string.IsNullOrEmpty(_ver))
                    _ver = XElement.Load(Constants.Manifest).Element("App").Attribute("Version").Value;

                return _ver;
            }
        }

        private static string _pId;
        public static string ProductID
        {
            get
            {
                if (string.IsNullOrEmpty(_pId))
                    _pId = new Guid(XElement.Load(Constants.Manifest).Element("App").Attribute("ProductID").Value).ToString();

                return _pId;
            }
        }

        private static string _siteUrl;
        public static string SiteUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_siteUrl))
                    _siteUrl = XElement.Load(Constants.DataModel).Attribute("siteUrl").Value;
                
                return _siteUrl;
            }
        }

        private static string _company;
        public static string Company
        {
            get
            {
                if (string.IsNullOrEmpty(_company))
                    _company = XElement.Load(Constants.DataModel).Attribute("company").Value;

                return _company;
            }
        }

        private static string _user;
        public static string User
        {
            get
            {
                if (string.IsNullOrEmpty(_user) && IsolatedStorageSettings.ApplicationSettings.Contains(Constants.UserKey))
                {
                    _user = IsolatedStorageSettings.ApplicationSettings[Constants.UserKey].ToString();
                }

                return _user;
            }
            set
            {
                if (_user != value)
                {
                    _user = value;

                    if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.UserKey))
                        IsolatedStorageSettings.ApplicationSettings[Constants.UserKey] = _user;
                    else
                        IsolatedStorageSettings.ApplicationSettings.Add(Constants.UserKey, _user);
                }
            }
        }
    }
}
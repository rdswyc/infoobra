using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Xml.Linq;

using InfoObra.Helpers;
using InfoObra.ViewModels;

namespace InfoObra.Services
{
    public class SharePoint
    {

    #region ViewModel

        public static ListViewModel GetViewModel(string listTitle)
        {
            ListViewModel model;

            if (IsolatedStorageSettings.ApplicationSettings.Contains(listTitle))
                model = (ListViewModel)IsolatedStorageSettings.ApplicationSettings[listTitle];
            else
                model = new ListViewModel()
                {
                    DataProvider = new ListDataProvider()
                    {
                        ListTitle = listTitle,
                        SiteUrl = new Uri(AppInfo.SiteUrl)
                    }
                };
            return model;
        }

        public static void SetViewModel(ListViewModel model)
        {
            string listTitle = model.DataProvider.ListTitle;

            if (IsolatedStorageSettings.ApplicationSettings.Contains(listTitle))
                IsolatedStorageSettings.ApplicationSettings[listTitle] = model;
            else
                IsolatedStorageSettings.ApplicationSettings.Add(listTitle, model);
        }

        public static bool HasViewModelOrKey(string listTitle)
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(listTitle);
        }

    #endregion

    #region App Lists

        private static List<spList> _appLists;
        public static List<spList> AppLists
        {
            get
            {
                if (_appLists == null)
                {
                    _appLists = new List<spList>();

                    foreach (XElement x in XElement.Load(Constants.DataModel).Elements("list"))
                    {
                        _appLists.Add(new spList(x.Attribute("title").Value)
                        {
                            DisplayName = x.Attribute("display").Value,
                            ViewName = x.Element("View").Attribute("name").Value,
                            hexColor = x.Attribute("color") != null ? x.Attribute("color").Value : string.Empty
                        });
                    }
                }
                return _appLists;
            }
        }

        public static spList GetList(string title)
        {
            return (from l in AppLists where l.Title == title select l).Single();
        }

    #endregion

    #region App Params

        private static List<string> _params;
        public static List<string> Parameters
        {
            get
            {
                if (_params == null)
                {
                    _params = (from a in XElement.Load(Constants.DataModel).Elements("param").Attributes("title") select a.Value).ToList();
                }
                return _params;
            }
        }

        public static IEnumerable<string> GetParamItems(spList list, string fieldName = Constants.DefaultField)
        {
            if (!GetViewModel(list.Title).ViewData.ContainsKey(list.ViewName))
            {
                return null;
            }

            return ((ObservableCollection<DisplayItem>)GetViewModel(list.Title).ViewData[list.ViewName])
                .Select((i) =>
                {
                    return i[fieldName].ToString();
                })
                .Where((f) =>
                {
                    return f != Constants.Empty;
                });
        }

    #endregion

    #region List Fields

        public static List<spField> GetListFields(string listTitle)
        {
            List<spField> _fields = new List<spField>();

            foreach (XElement fieldRef in XElement.Load(Constants.DataModel).Elements("list").Elements("View").Elements("ViewFields").Elements("FieldRef"))
            {
                if (fieldRef.Parent.Parent.Parent.Attribute("title").Value == listTitle)
                {
                    _fields.Add(new spField()
                    {
                        Name = fieldRef.Attribute("Name").Value,
                        DisplayName = fieldRef.Attribute("DisplayName").Value,
                        Type = fieldRef.Attribute("Type").Value,
                        Required = (fieldRef.Attribute("Required") != null),
                        List = (fieldRef.Attribute("List") == null ? string.Empty : fieldRef.Attribute("List").Value),
                        ShowField = (fieldRef.Attribute("ShowField") == null ? string.Empty : fieldRef.Attribute("ShowField").Value),
                        Choices = (fieldRef.Element("Choices") == null ? null : from x in fieldRef.Element("Choices").Elements() select x.Value)
                    });
                }
            }
            return _fields;
        }

        public static spField GetField(string listTitle, string fieldName = Constants.DefaultField)
        {
            return SharePoint.GetListFields(listTitle)
                .Where((f) =>
                {
                    return f.Name == fieldName;
                })
                .Single();
        }

    #endregion

    #region Specific Functions

        public static string GetItemID(string itemValue, string listTitle, string fieldName = Constants.DefaultField)
        {
            spList list;

            if (Parameters.Contains(listTitle))
            {
                list = new spList(listTitle);
            }
            else
            {
                list = GetList(listTitle);
            }

            return (from DisplayItem i in (ObservableCollection<DisplayItem>)GetViewModel(list.Title).ViewData[list.ViewName]
                    where i[fieldName].ToString() == itemValue
                    select i)
                    .First().ID;
        }

        public static string GetItemValueFromID(string id, string listTitle, string fieldName = Constants.DefaultField)
        {
            spList list;

            if (Parameters.Contains(listTitle))
            {
                list = new spList(listTitle);
            }
            else
            {
                list = GetList(listTitle);
            }

            return (from DisplayItem i in (ObservableCollection<DisplayItem>)GetViewModel(list.Title).ViewData[list.ViewName]
                    where i.ID == id
                    select i)
                    .First()[fieldName].ToString();
        }

        private static int _permission = 0;
        public static int UserPermission
        {
            get
            {
                if (_permission != 0)
                    return _permission;

                spList list = new spList(Constants.UserList);

                _permission = (bool)(from DisplayItem i in (ObservableCollection<DisplayItem>)GetViewModel(list.Title).ViewData[list.ViewName]
                                     where i[Constants.DefaultField].ToString() == AppInfo.User
                                     select i)
                                     .First()[Constants.PermissionField] ? 1 : -1;

                return _permission;
            }
        }

        public static string GetVerificacao(string itemId, spList list, string fieldName = Constants.DefaultField)
        {
            if (GetViewModel(list.Title).ViewData.ContainsKey(list.ViewName))
            {
                return ((ObservableCollection<DisplayItem>)GetViewModel(list.Title).ViewData[list.ViewName])
                    .Where((f) =>
                    {
                        return f.ID == itemId;
                    })
                    .Single()[fieldName] as string;
            }
            else
            {
                return string.Empty;
            }
        }

    #endregion

    }
}
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

using InfoObra.ViewModels;

namespace InfoObra.Services
{
    public class OfflineEdit
    {
        public static void Add(string id, EditItem model)
        {
            Dictionary<string, EditItem> collection = GetItemCollection();
            collection.Add(id, model);
            Save(collection);
        }

        public static void Remove(string id)
        {
            Dictionary<string, EditItem> collection = GetItemCollection();
            collection.Remove(id);
            Save(collection);
        }

        public static void Save(Dictionary<string, EditItem> collection)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.OfflineEdit))
            {
                IsolatedStorageSettings.ApplicationSettings[Constants.OfflineEdit] = collection;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add(Constants.OfflineEdit, collection);
            }
        }

        public static EditItem GetItemById(string id)
        {
            Dictionary<string, EditItem> collection = GetItemCollection();
            return !collection.ContainsKey(id) ? null : collection[id];
        }

        public static Dictionary<string, EditItem> GetItemCollection()
        {
            Dictionary<string, EditItem> collection = null;
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.OfflineEdit))
                collection = (Dictionary<string, EditItem>)IsolatedStorageSettings.ApplicationSettings[Constants.OfflineEdit];

            if (collection == null)
                collection = new Dictionary<string, EditItem>();

            return collection;
        }
    }

    public class OfflineNew
    {
        public static void Add(string id, NewItem model)
        {
            Dictionary<string, NewItem> collection = GetItemCollection();
            collection.Add(id, model);
            Save(collection);
        }

        public static void Remove(string id)
        {
            Dictionary<string, NewItem> collection = GetItemCollection();
            collection.Remove(id);
            Save(collection);
        }

        public static void Save(Dictionary<string, NewItem> collection)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.OfflineNew))
            {
                IsolatedStorageSettings.ApplicationSettings[Constants.OfflineNew] = collection;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add(Constants.OfflineNew, collection);
            }
        }

        public static NewItem GetItemById(string id)
        {
            Dictionary<string, NewItem> collection = GetItemCollection();
            return !collection.ContainsKey(id) ? null : collection[id];
        }

        public static Dictionary<string, NewItem> GetItemCollection()
        {
            Dictionary<string, NewItem> collection = null;
            if (IsolatedStorageSettings.ApplicationSettings.Contains(Constants.OfflineNew))
                collection = (Dictionary<string, NewItem>)IsolatedStorageSettings.ApplicationSettings[Constants.OfflineNew];

            if (collection == null)
                collection = new Dictionary<string, NewItem>();

            return collection;
        }
    }
}
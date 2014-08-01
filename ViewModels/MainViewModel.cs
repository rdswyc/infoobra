using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using InfoObra.Helpers;
using InfoObra.Services;

namespace InfoObra.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public EditItem SelectedItem;
        public NewItem CreateItem;
        public spList ActiveList;

        private bool LoadingAll;

        public IEnumerable<spList> Tiles
        {
            get
            {
                return SharePoint.AppLists.Where((l) => { return l.Title != Constants.ContactList; });
            }
        }

        private List<KeyedList<spList, DisplayItem>> _items;
        public List<KeyedList<spList, DisplayItem>> Items
        {
            get
            {
                if (FilterPage.hasFilter)
                {
                    spList list = SharePoint.GetList(Constants.PlanList);

                    if (SharePoint.GetViewModel(list.Title).ViewData.ContainsKey(list.ViewName))
                    {
                        return (from i in (ObservableCollection<DisplayItem>)SharePoint.GetViewModel(list.Title).ViewData[list.ViewName]
                                where
                                    (string.IsNullOrEmpty(FilterPage.EAP) || FilterPage.EAP == "-" || i["eap_operacional"].ToString() == FilterPage.EAP) &&
                                    (string.IsNullOrEmpty(FilterPage.Edificacao) || FilterPage.Edificacao == "-" || i["edificacao"].ToString() == FilterPage.Edificacao) &&
                                    (string.IsNullOrEmpty(FilterPage.Inicio) || i["inicio_planejado"].ToString() == FilterPage.Inicio) &&
                                    (string.IsNullOrEmpty(FilterPage.Frente) ||FilterPage.Frente == "-" || i["frente"].ToString() == FilterPage.Frente)
                                group i by i.List into keyGroups
                                select new KeyedList<spList, DisplayItem>(keyGroups))
                                .ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (_items == null)
                    {
                        _items = (from spList list in SharePoint.AppLists
                                  where list.Title != Constants.PlanList && SharePoint.GetViewModel(list.Title).ViewData.ContainsKey(list.ViewName)
                                  select new KeyedList<spList, DisplayItem>(list, (ObservableCollection<DisplayItem>)SharePoint.GetViewModel(list.Title).ViewData[list.ViewName]))
                                  .ToList();
                    }
                    return _items;
                }
            }
        }

        public List<KeyedList<spList, DisplayItem>> Offlines
        {
            get
            {
                var items =
                    from item in OfflineNew.GetItemCollection()
                    let dispItem = new DisplayItem() { IsNew = true, ID = item.Key, DataProvider = item.Value.DataProvider, ViewState = item.Value.ViewState }
                    group dispItem by dispItem.List into lists
                    select new KeyedList<spList, DisplayItem>(lists);

                items = items.Concat(
                    from item in OfflineEdit.GetItemCollection()
                    let dispItem = new DisplayItem() { IsNew = false, ID = item.Key, DataProvider = item.Value.DataProvider, ViewState = item.Value.ViewState }
                    group dispItem by dispItem.List into lists
                    select new KeyedList<spList, DisplayItem>(lists));

                return new List<KeyedList<spList, DisplayItem>>(items);
            }
        }

        public void LoadFromServer()
        {
            LoadingAll = true;

            foreach (spList l in SharePoint.AppLists)
            {
                RefreshList(l);
            }

            foreach (string p in SharePoint.Parameters)
            {
                RefreshList(new spList(p));
            }
        }

        public void RefreshList(spList list)
        {
            Indeterminate = true;

            ListLoader load = new ListLoader(list.Title);

            load.viewModel.InitializationCompleted += (s1, e1) =>
            {
                if (e1.Error != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => { Progress = 0; });
                }
            };

            load.viewModel.ViewDataLoaded += (s1, e1) =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {

                    if (LoadingAll)
                    {
                        Progress += 100 / (float)SharePoint.AppLists.Count;
                    }
                    else
                    {
                        _items = null;
                        NotifyPropertyChanged("Items");
                    }

                    if (_progress == 100)
                    {
                        Progress = 0;

                        if (e1.Error == null)
                        {
                            _items = null;
                            NotifyPropertyChanged("Items");
                        }

                        LoadingAll = false;
                    }

                    if (Indeterminate)
                        Indeterminate = false;
                });
            };
            load.LoadAsync(list.ViewName);
        }

        public void PostOfflines()
        {
            foreach (KeyValuePair<string, NewItem> item in OfflineNew.GetItemCollection())
            {
                ((NewItem)item.Value).Initialize();
                ((NewItem)item.Value).ID = item.Key;
                ((NewItem)item.Value).CreateItem();
            }

            foreach (KeyValuePair<string, EditItem> item in OfflineEdit.GetItemCollection())
            {
                ((EditItem)item.Value).Initialize();
                ((EditItem)item.Value).ID = item.Key;
                ((EditItem)item.Value).UpdateItem();
            }
        }


        private float _progress;
        public float Progress
        {
            get { return _progress; }
            private set
            {
                _progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        private bool _indet;
        public bool Indeterminate
        {
            get { return _indet; }
            private set
            {
                _indet = value;
                NotifyPropertyChanged("Indeterminate");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

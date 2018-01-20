using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Phone.Application;

using InfoObra.Services;

namespace InfoObra.ViewModels
{
    [DataContract]
    [KnownType(typeof(ListViewModelBase))]
    [KnownType(typeof(ListDataProvider))]
    [KnownType(typeof(ObservableCollection<DisplayItem>))]
    public class ListViewModel : ListViewModelBase
    {
        /// <summary>
        /// Makes sure that all the artifacts (eg. ListSchema, DataProvider etc.) required by ViewModel has been initialized properly. 
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Loads List data for SharePoint View with the specified name
        /// </summary>
        /// <param name="viewName">Name of the SharePoint View which has to be loaded</param>
        public void LoadData(string viewName, params object[] filterParameters)
        {
            IsBusy = true;
            DataProvider.LoadData(viewName, OnLoadViewDataCompleted, filterParameters);
        }

        /// <summary>
        /// Refreshes List data for SharePoint View with the specified name.
        /// </summary>
        /// <param name="viewName">Name of the SharePoint View which has to be loaded</param>
        public void RefreshData(string viewName, params object[] filterParameters)
        {
            IsBusy = true;
            ((ListDataProvider)DataProvider).RefreshData(viewName, OnLoadViewDataCompleted, filterParameters);
        }

        /// <summary>
        /// Code to execute when a SharePoint View has been loaded completely.
        /// </summary>
        /// <param name="e" />
        private void OnLoadViewDataCompleted(LoadViewCompletedEventArgs e)
        {
            IsBusy = false;
            if (e.Error != null)
            {
                OnViewDataLoaded(this, new ViewDataLoadedEventArgs { ViewName = e.ViewName, Error = e.Error });
                return;
            }

            //Create a collection of DisplayItemViewModels
            ObservableCollection<DisplayItem> displayViewModelCollection = new ObservableCollection<DisplayItem>();
            foreach (ListItem item in e.Items)
            {
                DisplayItem displayViewModel = new DisplayItem { ID = item.Id.ToString(), DataProvider = this.DataProvider };
                displayViewModel.Initialize();

                displayViewModelCollection.Add(displayViewModel);
            }

            OnViewDataLoaded(this, new ViewDataLoadedEventArgs { ViewName = e.ViewName, ViewData = displayViewModelCollection });
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.SharePoint.Phone.Application;

using InfoObra.ViewModels;

namespace InfoObra.Services
{
    public class ListLoader
    {
        private TaskCompletionSource<bool> tcsGetList;
        private string viewName;
        public ListViewModel viewModel;

        public ListLoader(string list)
        {
            viewModel = new ListViewModel()
            {
                DataProvider = new ListDataProvider()
                {
                    ListTitle = list,
                    SiteUrl = new Uri(AppInfo.SiteUrl)
                }
            };

            viewModel.ViewDataLoaded += new EventHandler<ViewDataLoadedEventArgs>(OnViewDataLoaded);
            viewModel.InitializationCompleted += new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
        }

        public void Dispose()
        {
            viewModel.ViewDataLoaded -= new EventHandler<ViewDataLoadedEventArgs>(OnViewDataLoaded);
            viewModel.InitializationCompleted -= new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
        }

        public void Load(string ViewName)
        {
            this.viewName = ViewName;

            if (!viewModel.IsInitialized)
                viewModel.Initialize();
            else
                viewModel.LoadData(viewName);
        }

        public Task LoadAsync(string ViewName)
        {
            tcsGetList = new TaskCompletionSource<bool>();
            this.viewName = ViewName;

            if (!viewModel.IsInitialized)
                viewModel.Initialize();
            else
                viewModel.LoadData(viewName);

            return tcsGetList.Task;
        }

        public Task RefreshAsync(string ViewName)
        {
            tcsGetList = new TaskCompletionSource<bool>();
            this.viewName = ViewName;

            if (!viewModel.IsInitialized)
                viewModel.Initialize();
            else
                viewModel.RefreshData(viewName);

            return tcsGetList.Task;
        }

        private void OnViewModelInitialization(object sender, InitializationCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (tcsGetList != null)
                    tcsGetList.TrySetException(e.Error);

                return;
            }

            viewModel.LoadData(viewName);
        }

        private void OnViewDataLoaded(object sender, ViewDataLoadedEventArgs e)
        {
            if (e.Error != null)
            {
                if (tcsGetList != null)
                    tcsGetList.TrySetException(e.Error);

                return;
            }

            viewModel[e.ViewName] = e.ViewData;
            SharePoint.SetViewModel(viewModel);

            if (tcsGetList != null)
                tcsGetList.TrySetResult(true);

            this.Dispose();
        }
    }
}
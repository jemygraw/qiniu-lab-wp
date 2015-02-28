using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using QiniuLab.Resources;
using QiniuLab.ViewModels;

namespace QiniuLab
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void SimpleUploadLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = this.SimpleUploadLongListSelector.SelectedItem;
            if (selected == null)
            {
                return;
            }

            ItemViewModel ivm = this.SimpleUploadLongListSelector.SelectedItem as ItemViewModel;
            Int32 id = ivm.ID;
            string name = ivm.Name;

            Dictionary<int, string> navUrlDict = new Dictionary<int, string>();
            navUrlDict.Add(0, string.Format("/Controls/Upload/SimpleUploadWithoutKey.xaml?selectedItem={0}", ivm.ID));
            navUrlDict.Add(1, string.Format("/Controls/Upload/SimpleUploadWithKey.xaml?selectedItem={0}", ivm.ID));
            navUrlDict.Add(2, string.Format("/Controls/Upload/SimpleUploadUseSaveKey.xaml?selectedItem={0}", ivm.ID));
            navUrlDict.Add(3, string.Format("/Controls/Upload/SimpleUploadUseSaveKeyFromXParam.xaml?selectedItem={0}", ivm.ID));
            navUrlDict.Add(4, string.Format("/Controls/Upload/SimpleUploadUseReturnBody.xaml?selectedItem={0}", ivm.ID));
            navUrlDict.Add(5, string.Format("/Controls/Upload/SimpleUploadOverwriteExistingFile.xaml?selectedItem={0}", ivm.ID));
            navUrlDict.Add(6, string.Format("/Controls/Upload/SimpleUploadUseFsizeLimit.xaml?selectedItem={0}", ivm.ID));
            string navUrl = "";
            if (navUrlDict.ContainsKey(id))
            {
                navUrl = navUrlDict[id];
            }
            if (!string.IsNullOrWhiteSpace(navUrl))
            {
                NavigationService.Navigate(new Uri(navUrl, UriKind.Relative));
            }
            this.SimpleUploadLongListSelector.SelectedItem = null;
        }

        private void AdvancedUploadLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TestCaseUploadLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = this.TestCaseUploadLongListSelector.SelectedItem;
            if (selected == null)
            {
                return;
            }

            ItemViewModel ivm = this.TestCaseUploadLongListSelector.SelectedItem as ItemViewModel;
            Int32 id = ivm.ID;
            string name = ivm.Name;

            Dictionary<int, string> navUrlDict = new Dictionary<int, string>();
            navUrlDict.Add(0, string.Format("/Controls/Upload/TestCaseUploadUseSandboxFile1.xaml?selectedItem={0}", ivm.ID));
            navUrlDict.Add(1, string.Format("/Controls/Upload/TestCaseUploadUseSandboxFile2.xaml?selectedItem={0}", ivm.ID));
            navUrlDict.Add(2, string.Format("/Controls/Upload/TestCaseUploadUseSandboxFile3.xaml?selectedItem={0}", ivm.ID));
            navUrlDict.Add(3, string.Format("/Controls/Upload/TestCaseUploadUseSandboxFile4.xaml?selectedItem={0}", ivm.ID));
            string navUrl = "";
            if (navUrlDict.ContainsKey(id))
            {
                navUrl = navUrlDict[id];
            }
            if (!string.IsNullOrWhiteSpace(navUrl))
            {
                NavigationService.Navigate(new Uri(navUrl, UriKind.Relative));
            }
            this.TestCaseUploadLongListSelector.SelectedItem = null;
        }
    }
}
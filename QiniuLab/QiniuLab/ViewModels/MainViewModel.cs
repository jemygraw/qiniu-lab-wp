using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using QiniuLab.Resources;

namespace QiniuLab.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.SimpleUploadItems = new ObservableCollection<ItemViewModel>();
            this.AdvancedUploadItems = new ObservableCollection<ItemViewModel>();
            this.TestCaseUploadItems = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> SimpleUploadItems { get; private set; }
        public ObservableCollection<ItemViewModel> AdvancedUploadItems { get; private set; }
        public ObservableCollection<ItemViewModel> TestCaseUploadItems { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            this.SimpleUploadItems.Add(new ItemViewModel() { ID = 0, Name = "无Key文件上传" });
            this.SimpleUploadItems.Add(new ItemViewModel() { ID = 1, Name = "有Key文件上传" });
            this.SimpleUploadItems.Add(new ItemViewModel() { ID = 2, Name = "使用SaveKey指定文件名" });
            this.SimpleUploadItems.Add(new ItemViewModel() { ID = 3, Name = "使用扩展参数作为SaveKey" });
            this.SimpleUploadItems.Add(new ItemViewModel() { ID = 4, Name = "使用ReturnBody自定义返回内容" });
            this.SimpleUploadItems.Add(new ItemViewModel() { ID = 5, Name = "文件同名(key)覆盖上传" });
            this.SimpleUploadItems.Add(new ItemViewModel() { ID = 6, Name = "限制文件上传大小" });
            this.SimpleUploadItems.Add(new ItemViewModel() { ID = 7, Name = "限制文件上传类型" });
            this.SimpleUploadItems.Add(new ItemViewModel() { ID = 8, Name = "指定上传文件的MimeType" });
            this.SimpleUploadItems.Add(new ItemViewModel() { ID = 9, Name = "启用上传文件CRC32校验" });
            this.SimpleUploadItems.Add(new ItemViewModel() { ID = 10, Name = "使用EndUser标注终端" });

            this.AdvancedUploadItems.Add(new ItemViewModel() { ID = 0, Name = "无Key大文件断点续传" });
            this.AdvancedUploadItems.Add(new ItemViewModel() { ID = 1, Name = "有Key大文件断点续传" });
            this.AdvancedUploadItems.Add(new ItemViewModel() { ID = 2, Name = "批量文件上传" });
            this.AdvancedUploadItems.Add(new ItemViewModel() { ID = 3, Name = "回调上传-URL参数格式" });
            this.AdvancedUploadItems.Add(new ItemViewModel() { ID = 4, Name = "回调上传-JSON参数格式" });
            this.AdvancedUploadItems.Add(new ItemViewModel() { ID = 5, Name = "上传后文件持久化操作" });

            this.TestCaseUploadItems.Add(new ItemViewModel() { ID = 0, Name = "测试沙盒文件上传(表单方式)" });
            this.TestCaseUploadItems.Add(new ItemViewModel() { ID = 1, Name = "测试沙盒文件上传(分片上传)" });
            this.TestCaseUploadItems.Add(new ItemViewModel() { ID = 2, Name = "测试沙盒文件上传(断点续传)" });
            this.TestCaseUploadItems.Add(new ItemViewModel() { ID = 3, Name = "测试沙盒文件上传(自动判断)" });
            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
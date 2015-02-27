using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.IO;
using Qiniu.Http;
using Newtonsoft.Json;
using Qiniu.Storage;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.Diagnostics;
namespace QiniuLab.Controls.Upload
{
    public partial class SimpleUploadWithoutKey : PhoneApplicationPage
    {
        private Stream uploadFileStream;
        private HttpManager httpManager;
        private string upTokenUrl;
        public SimpleUploadWithoutKey()
        {
            InitializeComponent();
            this.upTokenUrl = string.Format("{0}{1}", Config.API_HOST, Config.SIMPLE_UPLOAD_WITHOUT_KEY_UPTOKEN);
            this.UploadFileButton.IsEnabled = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext == null)
            {
                string selectedIndex = "";
                if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
                {
                    int index = int.Parse(selectedIndex);
                    DataContext = App.ViewModel.SimpleUploadItems[index];
                }
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.PhotoChooserTask t = new Microsoft.Phone.Tasks.PhotoChooserTask();
            t.Completed += SetFileName;
            t.Show();
        }

        private void SetFileName(object sender, Microsoft.Phone.Tasks.PhotoResult e)
        {
            if (e != null && e.Error == null)
            {
                //clear log
                LogTextBlock.Text = "";
                this.uploadFileStream = e.ChosenPhoto;
                writeLog("选取文件:" + e.OriginalFileName);
                this.UploadFileButton.IsEnabled = true;
            }
        }

        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.uploadFileStream == null)
            {
                return;
            }
            Task.Factory.StartNew(() =>
            {
                uploadFile();

            }).ContinueWith((state) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    //reset progress bar
                    ProgressBar.Value = 0;
                    this.UploadFileButton.IsEnabled = false;
                });
            });
        }

        private void uploadFile()
        {
            writeLog("准备上传...");
            if (this.httpManager == null)
            {
                this.httpManager = new HttpManager();
            }
            httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo getTokenRespInfo, string getTokenResponse)
            {
                if (getTokenRespInfo.StatusCode == 200)
                {
                    Dictionary<string, string> respDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(getTokenResponse);
                    if (respDict.ContainsKey("uptoken"))
                    {
                        string upToken = respDict["uptoken"];
                        writeLog("获取上传凭证:" + upToken);
                        UploadOptions uploadOptions = UploadOptions.defaultOptions();
                        uploadOptions.ProgressHandler = new UpProgressHandler(delegate(string key, double percent)
                        {
                            int progress = (int)(percent * 100);
                            Dispatcher.BeginInvoke(() =>
                            {
                                ProgressBar.Value = progress;
                            });
                        });
                        writeLog("开始上传文件...");
                        new UploadManager().uploadStream(this.uploadFileStream, null, upToken, uploadOptions,
                            new UpCompletionHandler(delegate(string key, ResponseInfo uploadRespInfo, string uploadResponse)
                        {
                            if (uploadRespInfo.isOk())
                            {
                                Dictionary<string, string> upRespDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(uploadResponse);
                                writeLog(string.Format("上传成功!\r\nKey: {0}\r\nHash: {1}", upRespDict["key"], upRespDict["hash"]));
                            }
                            else
                            {
                                writeLog("上传失败!\r\n" + uploadRespInfo.ToString());
                            }
                        }));
                    }
                    else
                    {
                        writeLog("获取凭证失败!\r\n" + getTokenRespInfo.ToString());
                    }
                }
                else
                {
                    writeLog("获取凭证失败!\r\n" + getTokenRespInfo.ToString());
                }
            });
            httpManager.post(upTokenUrl);
        }

        private void writeLog(string msg)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LogTextBlock.Text += "\r\n" + msg;
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Qiniu.Http;
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Qiniu.Storage;
using System.Threading.Tasks;
using Qiniu.Storage.Persistent;

namespace QiniuLab.Controls.Upload
{
    public partial class TestCaseUploadUseSandboxFile3 : PhoneApplicationPage
    {
        private HttpManager httpManager;
        private string upTokenUrl;
        private string filePath;
        private string fileName;
        private bool cancelSignal;
        public TestCaseUploadUseSandboxFile3()
        {
            InitializeComponent();
            this.upTokenUrl = string.Format("{0}{1}", Config.API_HOST, Config.SIMPLE_UPLOAD_WITHOUT_KEY_UPTOKEN);
            this.cancelSignal = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext == null)
            {
                string selectedIndex = "";
                if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
                {
                    int index = int.Parse(selectedIndex);
                    DataContext = App.ViewModel.TestCaseUploadItems[index];
                }
            }
        }

        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            int fileSize = 0;
            try
            {
                fileSize = int.Parse(this.FileSize.Text.Trim());
            }
            catch (Exception)
            {
                return;
            }
            if (this.FileName.Text.Trim().Length > 0)
            {
                this.fileName = this.FileName.Text.Trim();
            }
            else
            {
                return;
            }

            this.LogTextBlock.Text = "";
            Task.Factory.StartNew(() =>
            {
                //create a temp file of the length of the specified value
                this.filePath = createFile(fileSize, this.fileName);
                //reset progress bar
                Dispatcher.BeginInvoke(() =>
                {
                    ProgressBar.Value = 0;
                });
                //upload file by filePath
                uploadFile(this.filePath, this.fileName);
            });
        }

        //create file if not exists
        private string createFile(int fileSize, string fileName)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.DirectoryExists("temp") == false)
            {
                storage.CreateDirectory("temp");
            }

            byte[] fnameData = Encoding.UTF8.GetBytes(fileName);
            string filePath = Path.Combine("temp", Convert.ToBase64String(fnameData, 0, fnameData.Length));
            if (storage.FileExists(filePath))
            {
                return filePath;
            }
            writeLog("创建临时文件...");
            IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filePath, FileMode.Create, storage);
            StreamWriter sw = new StreamWriter(stream);
            for (int i = 0; i < fileSize; i++)
            {
                sw.Write("x");
            }
            try
            {
                sw.Flush();
                stream.Flush();
                sw.Close();
                stream.Close();
            }
            catch (Exception ex)
            {
                writeLog("创建临时文件失败!");
                writeLog("原因:" + ex.Message);
                return null;
            }
            return filePath;
        }

        private void uploadFile(string filePath, string fileName)
        {
            this.cancelSignal = false;
            if (filePath == null)
            {
                return;
            }
            writeLog("准备上传..." + filePath);
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
                        uploadOptions.MimeType = "text/plain";
                        uploadOptions.ExtraParams.Add("x:hello", "hello");
                        uploadOptions.ExtraParams.Add("x:world", "world");
                        uploadOptions.CheckCrc32 = true;
                        uploadOptions.ProgressHandler = new UpProgressHandler(delegate(string key, double percent)
                        {
                            int progress = (int)(percent * 100);
                            Dispatcher.BeginInvoke(() =>
                            {
                                ProgressBar.Value = progress;
                            });
                        });
                        uploadOptions.CancellationSignal = new UpCancellationSignal(delegate()
                        {
                            return this.cancelSignal;
                        });

                        writeLog("开始上传文件...");
                        UpCompletionHandler completionHandler = new UpCompletionHandler(delegate(string key, ResponseInfo uploadRespInfo, string uploadResponse)
                        {
                            this.cancelSignal = false;//reset
                            if (uploadRespInfo.isOk())
                            {
                                Dictionary<string, string> upRespDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(uploadResponse);
                                Dispatcher.BeginInvoke(() =>
                                    MessageBox.Show(string.Format("Key: {0}\r\nHash: {1}", upRespDict["key"], upRespDict["hash"]), "上传成功", MessageBoxButton.OK)
                                );
                                writeLog(string.Format("上传成功!\r\nKey: {0}\r\nHash: {1}", upRespDict["key"], upRespDict["hash"]));
                            }
                            else
                            {
                                Dispatcher.BeginInvoke(() =>
                                    MessageBox.Show(uploadRespInfo.ToString(), "上传失败", MessageBoxButton.OK)
                                );
                                writeLog("上传失败!\r\n" + uploadRespInfo.ToString());
                            }
                        });
                        //string uploadKey = null;
                        string uploadKey = fileName;
                        ResumeRecorder recorder = new ResumeRecorder("records");
                        string recorderKey = filePath;
                        ResumeUploader resumeUploader = new ResumeUploader(httpManager, recorder, recorderKey, filePath, uploadKey, upToken, uploadOptions, completionHandler);
                        resumeUploader.uploadFile();
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() =>
                            MessageBox.Show(getTokenRespInfo.ToString(), "获取上传凭证失败", MessageBoxButton.OK)
                        );
                        writeLog("获取凭证失败!\r\n" + getTokenRespInfo.ToString());
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(() =>
                        MessageBox.Show(getTokenRespInfo.ToString(), "获取上传凭证失败", MessageBoxButton.OK)
                    );
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

        private void CancelUploadButton_Click(object sender, RoutedEventArgs e)
        {
            this.cancelSignal = true;
        }
    }
}
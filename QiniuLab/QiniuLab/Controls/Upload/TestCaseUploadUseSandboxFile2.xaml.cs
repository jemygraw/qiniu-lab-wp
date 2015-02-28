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
using System.Diagnostics;

namespace QiniuLab.Controls.Upload
{
    public partial class TestCaseUploadUseSandboxFile2 : PhoneApplicationPage
    {
        private HttpManager httpManager;
        private string upTokenUrl;
        private string filePath;
        private string fileName;
        public TestCaseUploadUseSandboxFile2()
        {
            InitializeComponent();
            this.upTokenUrl = string.Format("{0}{1}", Config.API_HOST, Config.SIMPLE_UPLOAD_WITHOUT_KEY_UPTOKEN_PATH);
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
            this.LogTextBlock.Text = "";
            //delete old file due to failed upload
            if (this.filePath != null)
            {
                if (IsolatedStorageFile.GetUserStoreForApplication().FileExists(this.filePath))
                {
                    this.deleteFile(this.filePath);
                }
            }
            Task.Factory.StartNew(() =>
            {
                //create a temp file of the length of the specified value
                this.filePath = createFile(fileSize);
                //reset progress bar
                Dispatcher.BeginInvoke(() =>
                {
                    ProgressBar.Value = 0;
                });
                //upload file by filePath
                uploadFile(this.filePath, this.fileName);
            });
        }


        private string createFile(int fileSize)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.DirectoryExists("temp") == false)
            {
                storage.CreateDirectory("temp");
            }
            writeLog("创建临时文件...");
            string filePath = Path.Combine("temp", Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString())));
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
                        uploadOptions.ExtraParams.Add("cloud", "cloud");
                        uploadOptions.ExtraParams.Add("storage", "");
                        uploadOptions.ExtraParams.Add("x:qiniu", "");
                        uploadOptions.CheckCrc32 = true;
                        uploadOptions.ProgressHandler = new UpProgressHandler(delegate(string key, double percent)
                        {
                            int progress = (int)(percent * 100);
                            Dispatcher.BeginInvoke(() =>
                            {
                                ProgressBar.Value = progress;
                            });
                        });
                        writeLog("开始上传文件...");
                        UpCompletionHandler completionHandler = new UpCompletionHandler(delegate(string key, ResponseInfo uploadRespInfo, string uploadResponse)
                        {
                            if (uploadRespInfo.isOk())
                            {
                                Dictionary<string, string> upRespDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(uploadResponse);
                                writeLog(string.Format("上传成功!\r\nKey: {0}\r\nHash: {1}", upRespDict["key"], upRespDict["hash"]));
                                deleteFile(this.filePath);
                            }
                            else
                            {
                                writeLog("上传失败!\r\n" + uploadRespInfo.ToString());
                                deleteFile(this.filePath);
                            }
                        });
                        //string uploadKey = null;
                        string uploadKey = fileName;
                        ResumeUploader resumeUploader = new ResumeUploader(httpManager, null, null, filePath, uploadKey, upToken, uploadOptions, completionHandler);
                        resumeUploader.uploadFile();
                    }
                    else
                    {
                        writeLog("获取凭证失败!\r\n" + getTokenRespInfo.ToString());
                        deleteFile(this.filePath);
                    }
                }
                else
                {
                    writeLog("获取凭证失败!\r\n" + getTokenRespInfo.ToString());
                    deleteFile(this.filePath);
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

        private void deleteFile(string filePath)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                storage.DeleteFile(filePath);
                writeLog("成功删除临时文件!");
            }
            catch (Exception ex)
            {
                writeLog("删除临时文件失败!");
                writeLog("原因:" + ex.Message);
            }
        }
    }
}
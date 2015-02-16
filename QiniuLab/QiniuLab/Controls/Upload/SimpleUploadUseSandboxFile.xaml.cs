using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using Qiniu.Http;
using Qiniu.Storage;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;

namespace QiniuLab.Controls.Upload
{
    public partial class SimpleUploadUseSandboxFile : PhoneApplicationPage
    {
        private HttpManager httpManager;
        private string upTokenUrl;
        public SimpleUploadUseSandboxFile()
        {
            InitializeComponent();
            this.upTokenUrl = string.Format("{0}{1}", Config.API_HOST, Config.SIMPLE_UPLOAD_WITHOUT_KEY_UPTOKEN);
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
            this.LogTextBlock.Text = "";
            Task.Factory.StartNew(() =>
            {
                //create a temp file of the length of the specified value
                string fileName = createFile(fileSize);
                //upload file by filename
                uploadFile(fileName);
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
            string fileName = Path.Combine("temp", Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString())));
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Create, storage))
            {
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    for (int i = 0; i < fileSize; i++)
                    {
                        sw.Write("x");
                    }
                    sw.Flush();
                }
            }
            return fileName;
        }

        private void uploadFile(string fileName)
        {
            writeLog("准备上传..."+fileName);
            if (this.httpManager == null)
            {
                this.httpManager = new HttpManager();
            }
            httpManager.CompletionCallback = new CompletionCallback(delegate(ResponseInfo getTokenRespInfo, string getTokenResponse)
            {
                if (getTokenRespInfo.StatusCode == 200)
                {
                    Dictionary<string, string> respDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(getTokenResponse);
                    if (respDict.ContainsKey("uptoken"))
                    {
                        string upToken = respDict["uptoken"];
                        writeLog("获取上传凭证:" + upToken);
                        UploadOptions uploadOptions = new UploadOptions();
                        uploadOptions.ProgressCallback = new ProgressCallback(delegate(int bytesWritten, int totalBytes)
                        {
                            int progress = (bytesWritten * 100 / totalBytes);
                            Dispatcher.BeginInvoke(() =>
                            {
                                ProgressBar.Value = progress;
                            });
                        });
                        writeLog("开始上传文件...");
                        IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                        Stream uploadFileStream = new IsolatedStorageFileStream(fileName, FileMode.Open, storage);
                        FormUploader.uploadStream(httpManager, uploadFileStream, null, upToken, uploadOptions, new CompletionCallback(delegate(ResponseInfo uploadRespInfo, string uploadResponse)
                        {
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
                        }));
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
    }
}
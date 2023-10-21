using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Diagnostics;
using ZXing;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;







namespace jhtwj
{





    public partial class Form1 : Form
    {
        private HttpListener listener;
        private string currentVersion;
        private string selectedFilePath;
        private const string ReleasesUrl = "https://gitee.com/xiaobai9978/ipv6-file-sender/releases";
        private readonly HttpClient _client = new HttpClient();
        public bool IsPortOpen2 { get; private set; } = true;



        // 导入Windows API函数
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        // 定义委托类型
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // 定义RECT结构体，用于存储窗口位置信息
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // 记事本窗口句柄
        private IntPtr notepadHandle;


        public FileDropHandler FileDroper; //全局的



        string ipv6lable1 = "无 IPV6 连通";
        string ipv6lable2 = "IPV6 获取成功,端口检测中...";
        string ipv6lable3 = "IPV6 端口测试:成功";
        string ipv6lable4 = "IPV6 端口测试:失败";

        public Form1()
        {
            InitializeComponent();

            currentVersion = GetAssemblyVersion();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            updatemsg.Start();
            update.SetToolTip(label2, "点击检查更新");
            ipv6tip.SetToolTip(ipv6, "双方均支持ipv6才可以公网传输");
            //MessageBox.Show(GetAssemblyVersion());
            label2.Text = GetAssemblyVersion().Substring(0, GetAssemblyVersion().Length - 2);


            int portNumber = 11166; // 要检测的端口号

            bool isOpen = FirewallChecker.IsPortOpen(portNumber);

            if (isOpen)
            {
                ;
            }
            else
            {
                FirewallControl.OpenPort(11166);
            }


            FileDroper = new FileDropHandler(selectFileButton); //初始化
            StartServer();




            // 检测IPv6地址
            if (HasIpv6Address())
            {
                ipv6.Text = ipv6lable2;
                ipv6.ForeColor = Color.Blue;
                // 有IPv6地址

                // 创建 PortScanner 实例
                PortScanner portScanner = new PortScanner();

                // 调用异步方法并等待结果
                string portOpenStatus = await portScanner.IsPortOpenAsync(GetIpv6Address(), "", "11166");
                //MessageBox.Show(portOpenStatus);
                if (portOpenStatus == "ok")
                {
                    IsPortOpen2 = true;
                    ipv6.Text = ipv6lable3;
                    ipv6.ForeColor = Color.Green;
                }
                else if (portOpenStatus == "fail")
                {
                    IsPortOpen2 = false;
                    ipv6.Text = ipv6lable4;
                    ipv6.ForeColor = Color.Red;
                }
                else
                {
                    ipv6.Text = "IPV6 获取成功,检测网站失效..";
                    ipv6.ForeColor = Color.Brown;
                }
            }
            else
            {
                ipv6.Text = ipv6lable1;
                ipv6.ForeColor = Color.Red;
                // 没有IPv6地址
            }







            EnumWindows(new EnumWindowsProc(EnumWindowsCallback), IntPtr.Zero);

            if (notepadHandle != IntPtr.Zero)
            {
                // 启动定时器，每隔一段时间更新窗口位置
                timer1.Start();
            }
            else
            {
                //MessageBox.Show("未找到江河窗口！");
                timer1.Stop();
            }


        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopServer();
        }

        private void StartServer()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:11166/"); // 设置监听的URL，*表示监听所有可用IP地址和端口11166

            try
            {

                    listener.Start();

            }
            catch
            {
                // 忽略HttpListener异常
                MessageBox.Show("请勿重复打开本软件！" , "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Process.GetCurrentProcess().Kill();
            }




            serverStatusLabel.Text = "服务器已启动";
            serverStatusLabel.ForeColor = System.Drawing.Color.Green;

            serverStatusLabel.Visible = true;

            Listen();
        }

        private void StopServer()
        {
            if (listener != null && listener.IsListening)
            {
                listener.Stop();
                listener.Close();
            }
        }

        private async void Listen()
        {
            try
            {
                while (listener.IsListening)
                {
                    HttpListenerContext context = await listener.GetContextAsync();
                    ProcessRequest(context);
                }
            }
            catch (HttpListenerException)
            {
                // 忽略HttpListener异常
            }
        }




        private async void ProcessRequest(HttpListenerContext context)
        {
            if (selectedFilePath == null)
            {
                SendErrorResponse(context, "No file selected.");
                return;
            }

            try
            {
                FileStream fileStream = new FileStream(selectedFilePath, FileMode.Open, FileAccess.Read);

                string extension = Path.GetExtension(selectedFilePath);
                string mimeType = GetMimeType(extension);

                context.Response.ContentType = mimeType;

                byte[] buffer = new byte[16384];
                int bytesRead;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await context.Response.OutputStream.WriteAsync(buffer, 0, bytesRead);
                    await context.Response.OutputStream.FlushAsync();
                }

                fileStream.Close();
                context.Response.OutputStream.Close();
            }
            catch (FileNotFoundException)
            {
                SendErrorResponse(context, "File not found.");
            }
            catch (Exception ex)
            {
                SendErrorResponse(context, ex.Message);
            }
        }

        private string GetMimeType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".txt":
                    return "text/plain";
                case ".html":
                case ".htm":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".js":
                    return "application/javascript";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".pdf":
                    return "application/pdf";
                // 添加其他文件扩展名和对应的MIME类型
                default:
                    return "application/octet-stream";
            }
        }
        private void SendErrorResponse(HttpListenerContext context, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            try
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                // 在这里处理异常，例如记录错误日志或采取其他适当的操作
                Console.WriteLine("发生异常：" + ex.Message);
            }
        }




        [DllImport("user32.dll")]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


        private IntPtr targetWindowHandle = IntPtr.Zero;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;


        private bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            const int maxWindowClassLength = 256;
            const int maxWindowTextLength = 256;
            StringBuilder windowClass = new StringBuilder(maxWindowClassLength);
            StringBuilder windowText = new StringBuilder(maxWindowTextLength);

            // 获取窗口类名
            GetClassName(hWnd, windowClass, maxWindowClassLength);

            // 获取窗口标题
            GetWindowText(hWnd, windowText, maxWindowTextLength);

            // 判断窗口类名是否匹配
            if (windowClass.ToString().Contains("imwnd") || windowClass.ToString().Contains("CIMMgr") || (windowClass.ToString().Contains("Chrome_WidgetWin_1") && windowText.ToString().Contains("江河")))
            {
                //MessageBox.Show(windowClass.ToString());
                // 保存目标窗口句柄
                notepadHandle = hWnd;
                targetWindowHandle = notepadHandle;
                //MessageBox.Show(targetWindowHandle.ToString());

                return false; // 停止枚举
            }

            return true; // 继续枚举
        }




        private void timer1_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show(targetWindowHandle.ToString());

            if (targetWindowHandle != IntPtr.Zero)
            {
                
                // 获取记事本窗口的位置信息
                if (GetWindowRect(targetWindowHandle, out RECT rect))
                {
                    // 更新本窗口的位置
                    Location = new System.Drawing.Point(rect.Left - 300, rect.Top);
                    // 将当前窗口设置为目标窗口的上层
                    SetWindowPos(Handle, targetWindowHandle, rect.Left - 300, rect.Top, 0, 0, SWP_NOSIZE | SWP_NOMOVE);
                }
            }
        }





        private void selectFileButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "所有文件|*.*";
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
                filePathLabel.Text = selectedFilePath;
                filePathLabel.Visible = true;

                generateLinkButton.Enabled = true;
            }

            generateLinkButton.PerformClick();
            timer1.Start();
        }

        private void selectFileButton_DragEnter(object sender, DragEventArgs e)
        {
            var paths = (string[])e.Data.GetData(typeof(string[]));
            selectedFilePath = paths[0];
            filePathLabel.Text = selectedFilePath;


            label3.Text = "点击蓝色链接复制";
            generateLinkButton.PerformClick();
        }


        private void generateLinkButton_Click(object sender, EventArgs e)
        {

            if (Directory.Exists(selectedFilePath))
            {
                // 如果 selectedFilePath 是文件夹路径，显示拒绝的提示弹窗

                filePathLabel.Text = "拒绝选择文件夹！";
                label3.Text  = linkLabel1.Text ="";
                picQrCode.Image = null;
                //MessageBox.Show("拒绝选择文件夹！");


                return;
            }

            string fileName = Path.GetFileName(selectedFilePath);
            List<string> ipAddressList = GetLocalIpAddresses(IsPortOpen2);

            string ipAddress = ipAddressList.FirstOrDefault(); // 获取列表中的第一个 IP 地址

            if (ipAddress != null)
            {
                string link = "";

                // 判断是否为IPv6地址
                if (IPAddress.TryParse(ipAddress, out IPAddress parsedIpAddress) && parsedIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    link = $"http://[{ipAddress}]:11166/{fileName}";
                }
                else
                {
                    link = $"http://{ipAddress}:11166/{fileName}";
                }

                linkLabel1.Text = link;
                linkLabel1.LinkColor = System.Drawing.Color.Blue;
                linkLabel1.Visible = true;
            }
            else
            {
                // 处理找不到符合条件的 IP 地址的情况
            }


            string text = linkLabel1.Text; // 获取用户输入的文本

            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE, // 设置二维码格式
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 100, // 设置二维码的宽度
                    Height = 100, // 设置二维码的高度
                    Margin = 0 // 设置二维码的边距
                }
            };

            Bitmap qrCodeBitmap = writer.Write(text); // 生成二维码图像

            picQrCode.Image = qrCodeBitmap; // 在PictureBox中显示二维码

            label3.Text = "点击蓝色链接复制";



        }

        private List<string> GetLocalIpAddresses(bool IsPortOpen2)
        {
            List<string> ipAddressList = new List<string>();
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());

            

            // 查找以 "2" 开头的 IPv6 地址
            if (ipv6.Text == ipv6lable2 | ipv6.Text == ipv6lable3)
            {
                foreach (IPAddress ipAddress in hostEntry.AddressList)
                {
                    if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        string ipAddressString = ipAddress.ToString();

                        //MessageBox.Show(ipAddressString);
                        if (ipAddressString.StartsWith("2"))
                        {
                            ipAddressList.Add(ipAddressString);
                        }
                    }
                }
            }
            else
            {
                // 查找以 "10.1.5." 开头的 IP 地址
                foreach (IPAddress ipAddress in hostEntry.AddressList)
                {
                    if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        string ipAddressString = ipAddress.ToString();
                        if (ipAddressString.StartsWith("10.1.5."))
                        {
                            ipAddressList.Add(ipAddressString);
                        }
                    }
                }

                // 如果找不到以 "10.1.5." 开头的 IP 地址，则查找以 "10.1." 开头的 IP 地址
                if (ipAddressList.Count == 0)
                {
                    foreach (IPAddress ipAddress in hostEntry.AddressList)
                    {
                        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            string ipAddressString = ipAddress.ToString();
                            if (ipAddressString.StartsWith("10.1."))
                            {
                                ipAddressList.Add(ipAddressString);
                            }
                        }
                    }
                }
                
                // 如果找不到以 "10.1." 开头的 IP 地址，则查找以 "10." 开头的 IP 地址
                if (ipAddressList.Count == 0)
                {
                    foreach (IPAddress ipAddress in hostEntry.AddressList)
                    {
                        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            string ipAddressString = ipAddress.ToString();
                            if (ipAddressString.StartsWith("10."))
                            {
                                ipAddressList.Add(ipAddressString);
                            }
                        }
                    }
                }
            }

            // 如果仍然找不到符合条件的 IP 地址，则添加非回环地址的其他 IP 地址
            if (ipAddressList.Count == 0)
            {
                foreach (IPAddress ipAddress in hostEntry.AddressList)
                {
                    if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !IPAddress.IsLoopback(ipAddress))
                    {
                        ipAddressList.Add(ipAddress.ToString());
                    }
                }
            }

            return ipAddressList;
        }









        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var paths = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string fileName = Path.GetFileName(paths);

            List<string> ipAddressList = GetLocalIpAddresses(IsPortOpen2);

            string ipAddress = ipAddressList.FirstOrDefault(); // 获取列表中的第一个 IP 地址

            if (ipAddress != null)
            {
                //urlcode
                string linkText = linkLabel1.Text;
                string encodedLink = Regex.Replace(linkText, @"[\u4e00-\u9fa5\u3000-\u303f\uff01-\uff5e]+", m => Uri.EscapeDataString(m.Value));
                Clipboard.SetText("下载链接：" + encodedLink + "\n--此由文件传送器分享");
            }
            else
            {
                // 处理找不到符合条件的 IP 地址的情况
            }
            label3.Text = "*下载链接已复制至剪切板*";
            linkLabel1.LinkColor= System.Drawing.Color.Green;

            //label3.Text = " ";
        }





        public sealed class FileDropHandler : IMessageFilter, IDisposable
        {

            #region native members

            [DllImport("user32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint message, ChangeFilterAction action, in ChangeFilterStruct pChangeFilterStruct);

            [DllImport("shell32.dll", SetLastError = false, CallingConvention = CallingConvention.Winapi)]
            private static extern void DragAcceptFiles(IntPtr hWnd, bool fAccept);

            [DllImport("shell32.dll", SetLastError = false, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
            private static extern uint DragQueryFile(IntPtr hWnd, uint iFile, StringBuilder lpszFile, int cch);

            [DllImport("shell32.dll", SetLastError = false, CallingConvention = CallingConvention.Winapi)]
            private static extern void DragFinish(IntPtr hDrop);

            [StructLayout(LayoutKind.Sequential)]
            private struct ChangeFilterStruct
            {
                public uint CbSize;
                public ChangeFilterStatu ExtStatus;
            }

            private enum ChangeFilterAction : uint
            {
                MSGFLT_RESET,
                MSGFLT_ALLOW,
                MSGFLT_DISALLOW
            }

            private enum ChangeFilterStatu : uint
            {
                MSGFLTINFO_NONE,
                MSGFLTINFO_ALREADYALLOWED_FORWND,
                MSGFLTINFO_ALREADYDISALLOWED_FORWND,
                MSGFLTINFO_ALLOWED_HIGHER
            }

            private const uint WM_COPYGLOBALDATA = 0x0049;
            private const uint WM_COPYDATA = 0x004A;
            private const uint WM_DROPFILES = 0x0233;

            #endregion


            private const uint GetIndexCount = 0xFFFFFFFFU;

            private Control _ContainerControl;

            private readonly bool _DisposeControl;

            public Control ContainerControl { get; }

            public FileDropHandler(Control containerControl) : this(containerControl, false) { }

            public FileDropHandler(Control containerControl, bool releaseControl)
            {
                _ContainerControl = containerControl ?? throw new ArgumentNullException("control", "control is null.");

                if (containerControl.IsDisposed) throw new ObjectDisposedException("control");

                _DisposeControl = releaseControl;

                var status = new ChangeFilterStruct() { CbSize = 8 };

                if (!ChangeWindowMessageFilterEx(containerControl.Handle, WM_DROPFILES, ChangeFilterAction.MSGFLT_ALLOW, in status)) throw new Win32Exception(Marshal.GetLastWin32Error());
                if (!ChangeWindowMessageFilterEx(containerControl.Handle, WM_COPYGLOBALDATA, ChangeFilterAction.MSGFLT_ALLOW, in status)) throw new Win32Exception(Marshal.GetLastWin32Error());
                if (!ChangeWindowMessageFilterEx(containerControl.Handle, WM_COPYDATA, ChangeFilterAction.MSGFLT_ALLOW, in status)) throw new Win32Exception(Marshal.GetLastWin32Error());
                DragAcceptFiles(containerControl.Handle, true);

                Application.AddMessageFilter(this);
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (_ContainerControl == null || _ContainerControl.IsDisposed) return false;
                if (_ContainerControl.AllowDrop) return _ContainerControl.AllowDrop = false;
                if (m.Msg == WM_DROPFILES)
                {
                    var handle = m.WParam;

                    var fileCount = DragQueryFile(handle, GetIndexCount, null, 0);

                    var fileNames = new string[fileCount];

                    var sb = new StringBuilder(262);
                    var charLength = sb.Capacity;
                    for (uint i = 0; i < fileCount; i++)
                    {
                        if (DragQueryFile(handle, i, sb, charLength) > 0) fileNames[i] = sb.ToString();
                    }
                    DragFinish(handle);
                    _ContainerControl.AllowDrop = true;
                    _ContainerControl.DoDragDrop(fileNames, DragDropEffects.All);
                    _ContainerControl.AllowDrop = false;
                    return true;
                }
                return false;
            }

            public void Dispose()
            {
                if (_ContainerControl == null)
                {
                    if (_DisposeControl && !_ContainerControl.IsDisposed) _ContainerControl.Dispose();
                    Application.RemoveMessageFilter(this);
                    _ContainerControl = null;
                }
            }
        }


        public class FirewallControl
        {
            public static void OpenPort(int portNumber)
            {
                string arguments = $"advfirewall firewall add rule name=\"Open Port {portNumber}\" dir=in action=allow protocol=TCP localport={portNumber}";

                ExecuteCommand("netsh", arguments);
            }



            private static void ExecuteCommand(string command, string arguments)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(output))
                    {
                        Console.WriteLine("Output: " + output);
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Error: " + error);
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            var paths = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            selectedFilePath = paths;
            filePathLabel.Text = selectedFilePath;
            generateLinkButton.PerformClick();


            // 创建一个 LinkLabelLinkClickedEventArgs 对象
            var args = new LinkLabelLinkClickedEventArgs(linkLabel1.Links[0]);

            // 调用 protected 方法 LinkLabel.OnLinkClicked 方法
            typeof(LinkLabel).GetMethod("OnLinkClicked", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Invoke(linkLabel1, new object[] { args });


            //Clipboard.SetText(linkLabel1.Text);
            label1.Text = "已复制";

            timer2.Interval = 5000;
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label1.Text = "分享本软件";
        }

        public class FirewallChecker
        {
            public static bool IsPortOpen(int portNumber)
            {
                string arguments = $"advfirewall firewall show rule name=\"Open Port {portNumber}\"";

                string output = ExecuteCommand("netsh", arguments);

                return output.Contains("11166");
            }

            private static string ExecuteCommand(string command, string arguments)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Error: " + error);
                    }

                    return output;
                }
            }
        }

        private bool HasIpv6Address()
        {
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in entry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetworkV6 &&
                   ip.ToString().StartsWith("2"))
                {
                    return true;
                }
            }
            return false;
        }

        private string GetIpv6Address()
        {
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in entry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetworkV6 &&
                    ip.ToString().StartsWith("2"))
                {
                    return ip.ToString();
                }
            }

            return string.Empty;
        }



        private void CheckForUpdates()
        {
            try
            {
                WebClient client = new WebClient();
                string releasesPage = client.DownloadString(ReleasesUrl);

                // 解析 releasesPage 的内容以获取更新信息
                // 查找最新版本号
                string latestVersion = FindLatestVersion(releasesPage);

                if (!string.IsNullOrEmpty(latestVersion))
                {
                    if (IsNewVersionAvailable(latestVersion))
                    {
                        DialogResult result = MessageBox.Show("发现新版本：" + latestVersion + "  是否前往下载更新？", "更新提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        if (result == DialogResult.OK)
                        {
                            // 打开指定的URL链接
                            System.Diagnostics.Process.Start(ReleasesUrl);
                        }
                    }
                    else
                    {
                        if (yhdj == true)
                            MessageBox.Show("云端最新版本：" + latestVersion + "  已是最新版本，无需更新。", "更新提示", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        else
                            ;
                    }
                }
                else
                {
                    if (yhdj == true)
                        MessageBox.Show("未找到版本号。", "更新提示", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            catch (Exception ex)
            {
                if (yhdj == true)
                    MessageBox.Show("检查更新时发生错误：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private string FindLatestVersion(string html)
        {
            string pattern = @"<span>\s*(v\d+(\.\d+)+)\s*</span>";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(html);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }





        private bool IsNewVersionAvailable(string latestVersion)
        {
            // 在这里编写判断是否有新版本可用的逻辑
            // 可以比较 latestVersion 与当前安装的版本号，或者将版本号转换为数字进行比较等
            return string.Compare(latestVersion, currentVersion) > 0;
        }

        private string GetAssemblyVersion()
        {
            // 获取当前程序集的版本号
            Version version = typeof(Form1).Assembly.GetName().Version;
            return "v" + version.ToString();
        }





        private void update_Popup(object sender, PopupEventArgs e)
        {

        }
        bool yhdj=false;
        private void label2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("是否连接 Gitee 以查找更新?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            if (result == DialogResult.Yes)
            {
                // 用户点击了是，执行相应的函数
                yhdj = true;
                CheckForUpdates();
            }
            
        }




        public class PortScanner
        {
            public async Task<string> IsPortOpenAsync(string address, string portsSelected, string portsList)
            {
                try
                {
                    // 构建请求数据
                    string postData = $"addr={Uri.EscapeDataString(address)}&ports_selected={Uri.EscapeDataString(portsSelected)}&ports_list={Uri.EscapeDataString(portsList)}";
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                    // 创建请求对象
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ipv6.my-addr.com/online-ipv6-port-scan.php");
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;

                    // 发送请求数据
                    using (Stream dataStream = await request.GetRequestStreamAsync())
                    {
                        await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
                    }

                    // 获取响应
                    using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(responseStream))
                            {
                                string responseHtml = await reader.ReadToEndAsync();
                                //MessageBox.Show(responseHtml);
                                //Clipboard.SetText(responseHtml);
                                // 检查响应中是否包含检查通过的图标
                                if (responseHtml.Contains("<td><img src='/img/fail.png' ></td>"))
                                {

                                    return "fail";
                                }
                                else if (responseHtml.Contains("<td><img src='/img/check_ok.png' ></td>"))
                                {
                                    return "ok";
                                }
                                else
                                {
                                    return "error";
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 处理异常情况
                    Console.WriteLine("An error occurred: " + ex.Message);
                    return "error";
                }

            }
       
        }

        private void ipv6_Click(object sender, EventArgs e)
        {
            if (ipv6.Text == ipv6lable4)
                MessageBox.Show("虽成功获取 IPV6 地址，但端口未通过测试，只可接收不可发送。\n请尝试关闭路由器 IPv6 Session 防火墙", ipv6lable4, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void updatemsg_Tick(object sender, EventArgs e)
        {
            updatemsg.Stop();
            CheckForUpdates();
        }
    }



}


































using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProject
{
    public class LogServer
    {
        //先进先出
        public ConcurrentQueue<LogInfo> LogQueue = new ConcurrentQueue<LogInfo>();

        public static LogServer Instance { get; private set; } = new LogServer();
        private Task task;

        public string path = @"F:\log\";

        /*
         私有构造函数，防止外部实例化
         */
        private LogServer()
        {
            task = Task.Run(() => Dowork());
            //if (task.Status == TaskStatus.Running) {

            //}
        }

        //写入日志
        public void WriteLog(string logText)
        {
            string fileName = $"{path}{DateTime.Now.ToString("yyyy-MM")}\\{DateTime.Now.ToString("yyyy-MM-dd")}_LogInfo.txt";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!Directory.Exists(fileName.Substring(0, fileName.LastIndexOf("\\"))))
            {
                Directory.CreateDirectory(fileName.Substring(0, fileName.LastIndexOf("\\")));
            }

            string logContext = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} =======> {logText}";
            WriteLog(fileName, logContext);
        }

        private void WriteLog(string fileName, string logContext)
        {
            lock (this)
            {
                LogInfo logInfo = new LogInfo()
                {
                    Path = fileName,
                    Text = logContext
                };
                //添加数据
                LogQueue.Enqueue(logInfo);
            }
        }

        private void Dowork()
        {
            while (true)
            {
                int failCounet = 0;
                if (LogQueue.Count > 0)
                {
                    //取数据 并移除
                    LogQueue.TryDequeue(out LogInfo logInfo);
                LogStart: try
                    {
                        using (StreamWriter sw = new StreamWriter(logInfo.Path, true, Encoding.UTF8))
                        {
                            //写入日志
                            sw.WriteLine(logInfo.Text);
                        }
                    }
                    catch (Exception ex)
                    {
                        failCounet++;

                        if (failCounet <= 30)
                        {
                            goto LogStart;
                        }
                        else
                        {
                            //三色灯
                            //蜂鸣器
                            //邮件
                            //弹窗
                            //急停
                        }

                    }
                }
            }
        }
    }
}

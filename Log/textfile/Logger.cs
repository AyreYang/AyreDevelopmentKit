using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Log.common;
using Log.common.enums;
using System.IO;
using Log.common.messages;

namespace Log.textfile
{
    public class Logger : LogCore
    {
        private const long size = 1024 * 1024 * 100;
        private const string encode = "UTF-8";     //shift-jis, GB2312
        private const string extension = ".log";

        private object m_lock = new object();

        public string path { get; private set; }
        public string file
        {
            get
            {
                var today = DateTime.Today.Date.ToString("yyyyMMdd");
                var temp = Path.Combine(path, today);
                return Path.Combine(temp, this.id + extension);
            }
        }

        private bool full
        {
            get
            {
                if (!File.Exists(file)) return false;
                return new FileInfo(file).Length >= size;
            }
        }

        public Logger(string id) : this(id, null) { }
        public Logger(string id, string location, LEVEL level = LEVEL.ALL)
            : base(id, level)
        {
            DirectoryInfo ldi_info = null;
            if (string.IsNullOrWhiteSpace(location))
            {
                var lfi_module = new FileInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                ldi_info = lfi_module.Directory;
                var ls_root = Path.Combine(ldi_info.FullName, "log");
                if(!Directory.Exists(ls_root)) Directory.CreateDirectory(ls_root);
                ldi_info = new DirectoryInfo(ls_root);
            }
            else
            {
                ldi_info = new DirectoryInfo(location);
            }
            if (ldi_info == null || !ldi_info.Exists) throw new Exception(string.Format(Message.MSG_ERR_PARM_IS_INVALID, "location", location));
            path = ldi_info.FullName;
        }
        public override long Write(TYPE type, string message)
        {
            try
            {
                lock (m_lock)
                {
                    var lfi_info = new FileInfo(file);
                    if (!Directory.Exists(lfi_info.Directory.FullName)) Directory.CreateDirectory(lfi_info.Directory.FullName);
                    if (full) Rename();
                    using (StreamWriter lsw_temp = new StreamWriter(File.Open(file, FileMode.Append), Encoding.GetEncoding(encode)))
                    {
                        var msg = BuildMessage(type, message);
                        if (msg != null) lsw_temp.WriteLine(msg);
                    }
                }
                return 1;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return -100;
            }
        }

        private void Rename()
        {
            try
            {
                var current = new FileInfo(file);
                var filename = current.Name.Replace(current.Extension, string.Empty);
                var now = DateTime.Now;
                var hour = now.Hour.ToString().PadLeft(2, '0');
                var minute = now.Minute.ToString().PadLeft(2, '0');
                var second = now.Second.ToString().PadLeft(2, '0');
                filename = string.Format("{0}-{1}{2}{3}{4}", filename, hour, minute, second, extension);
                var newfile = new FileInfo(Path.Combine(current.Directory.FullName, filename));
                current.MoveTo(newfile.FullName);
            }
            catch (Exception err)
            { Console.WriteLine(err); }
        }
    }
}

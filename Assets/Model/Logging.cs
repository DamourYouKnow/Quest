using System;
using System.IO;

namespace Quest.Core {
    public class Logger() {
        const string LogPath = "./Logs/";
        private StreamWriter sw;

        public Logger(string path) {
            this.logPath = path;
            sw = File.AppendText(path);
        }

        public void Log(string message) {
            using sw {
                sw.WriteLine(timestamp() + " | " + message);
            }
        }

        private string timestamp() {
            return Date.Now().ToString("yyyy-MM-dd hh:mm:ss");
        }
    }
}
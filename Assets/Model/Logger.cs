using System;
using System.IO;

namespace Quest.Core {
	/// <summary>
	/// Creates a basic logging system.
	/// </summary>
	public class Logger {
		const string logFolder = "./Logs/";
		const string filePrefix = "quest";
		const string fileSuffix = ".log";

		string LogPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="Quest.Core.Logger"/> class.
		/// </summary>
		/// <note>
		/// Creates directory structure in regards to current date, and creates uniquely
		/// identified logfile.
		/// </note>
		public Logger(){
			string folderDate = date().Replace("-", "_");

			string filePath = logFolder;

			//If today's directory doesn't exist, create it.
			if (!Directory.Exists (filePath + folderDate)) {
				Directory.CreateDirectory (filePath + folderDate);
			}

			//If exists, or created in last block.
			if (Directory.Exists (filePath + folderDate)) {
				filePath += folderDate + "/" + filePrefix + folderDate + "_";

				//Append unique identifier to end of filename.
				int id = 0;
				while (File.Exists (filePath + id.ToString () + fileSuffix)) {
					id++;
				}
				filePath += id.ToString () + fileSuffix;

				//Attempt to create logfile.
				using (File.Create (filePath)) {
					this.LogPath = filePath;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Quest.Core.Logger"/> class.
		/// </summary>
		/// <param name="path">Identifies custom log file name.</param>
		/// <note>
		/// Will create file, but not directory structure.
		/// </note>
		public Logger(string path) {
			using (File.Create (path)) {
				this.LogPath = path;
			}
		}

		public void Log(string message) {
			using (StreamWriter sw = File.AppendText (LogPath)){
				sw.WriteLine("LOG " + timestamp () + " | " + message);
			}
		}

		public void Error(string message) {
			using (StreamWriter sw = File.AppendText (LogPath)) {
				sw.WriteLine ("ERR " + timestamp () + " | " + message);
			}
		}

		private string timestamp() {
			return date() + " " + time();
		}
		private string time() {
			return DateTime.Now.ToString("hh:mm:ss");
		}
		
		private string date(){
			return DateTime.Now.ToString ("yyyy-MM-dd");
		}
	}
}
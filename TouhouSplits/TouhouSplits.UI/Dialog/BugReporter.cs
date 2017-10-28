using System;
using System.IO;
using System.Windows.Forms;
using TouhouSplits.Service;

namespace TouhouSplits.UI.Dialog
{
    public class BugReporter
    {
        public bool ShowSaveBugReportDialog(Exception exception, out string error)
        {
            error = string.Empty;

            try {
                var dialog = new SaveFileDialog();
                dialog.DefaultExt = FilePaths.EXT_CRASH_FILE;
                dialog.Filter = string.Format("Crash Files ({0})|*{0}|All Files (*.*)|*.*", FilePaths.EXT_CRASH_FILE);

                if (dialog.ShowDialog() == DialogResult.OK) {
                    using (var filestream = File.Open(dialog.FileName, FileMode.Create, FileAccess.Write)) {
                        using (StreamWriter writer = new StreamWriter(filestream)) {
                            WriteStackTrace(writer, exception);
                            writer.Flush();
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e) {
                error = e.Message;
                return false;
            }
        }

        private static void WriteStackTrace(StreamWriter writer, Exception e)
        {
            if (e != null) {
                writer.WriteLine(e.Message);
                writer.WriteLine(e.StackTrace);
                writer.WriteLine();

                WriteStackTrace(writer, e.InnerException);
            }
        }
    }
}

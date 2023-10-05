using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheLostHopeEditor.EditorCode.Utils
{
    public static class FileDialog
    {
        public static void OpenFileDialog(string initialDir, string filter, Action<string> onSuccess)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = initialDir;
                openFileDialog.Filter = filter;
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    onSuccess?.Invoke(openFileDialog.FileName);
                }
            }
        }
    }
}

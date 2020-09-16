using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace folderlocker_cryptor
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public IEnumerable<string> GetFiles(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }
        public async void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) { radioButton1.Checked=false; MessageBox.Show("file name cyptoing option is not avavle yet"); }
            foreach (string file in GetFiles(textBox1.Text))
            {

                try
                {

                    var filePath = file;
                    var password = textBox2.Text;
                    // برای آن که برنامه در هنگام عملیات رمزگذاری قفل نشود
                    //  متد زیر را با استفاده از Task فراخوانی می کنیم
                    await Task.Run(() => AesCryptography.EncryptFile(filePath, password));

                    if (radioButton1.Checked)
                    {
                        try
                        {
                         //   MessageBox.Show(file);
                            string  input = Path.GetFileName(file);
                           // MessageBox.Show(input);
                            var passwordname = textBox2.Text;
                            var result = AesCryptography.EncryptText(input, passwordname);
                            Thread.Sleep(2000);
                            FileInfo fileinf = new FileInfo(file);
                            fileinf.Rename(result);
                        }
                        catch (Exception ex)
                        {
                           MessageBox.Show(ex.Message);
                        }
                    }







                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("this future is not available yet ");
            radioButton1.Checked = false;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            
        }

        private void radioButton1_MouseClick(object sender, MouseEventArgs e)
        {
           
        }

        private void radioButton1_MouseCaptureChanged(object sender, EventArgs e)
        {
        
        }
    }
}

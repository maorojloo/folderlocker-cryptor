using System;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace folderlocker_cryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Text = "cryphoing";
                var filePath = textBox1.Text;
                var password = textBox2.Text;
                // برای آن که برنامه در هنگام عملیات رمزگذاری قفل نشود
                //  متد زیر را با استفاده از Task فراخوانی می کنیم
                await Task.Run(() => AesCryptography.EncryptFile(filePath, password));
                button1.Text = "crypho";
                //hashimg name 

               if(radioButton1.Checked)
                {


                    try
                    {

                        var input = Path.GetFileName(textBox1.Text);
                        var passwordname = textBox1.Text;
                        var result = AesCryptography.EncryptText(input, password);

                        FileInfo file = new FileInfo(textBox1.Text);
                        file.Rename(result);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }





                MessageBox.Show(@"Done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       
        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}

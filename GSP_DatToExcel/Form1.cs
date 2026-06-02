using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_DatToExcel
{
    public partial class Form1 : Form
    {
        public WemadeCrypt WemadeCrypt;

        public static Form1 form1;
        public bool Runing;
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            form1 = this;
            //"res070821mir" -> "Y8vkFwSacHjCFThh"
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(GetType().Namespace + ".Properties.Resources", System.Reflection.Assembly.GetExecutingAssembly());
            byte[] encBox = (byte[])rm.GetObject("fullEncBoxes");

            using (MemoryStream ms = new MemoryStream(encBox))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    Buffer.BlockCopy(br.ReadBytes(WemadeCrypt.PBOX_INIT.Length * 4), 0, WemadeCrypt.PBOX_INIT, 0, WemadeCrypt.PBOX_INIT.Length * 4);
                    Buffer.BlockCopy(br.ReadBytes(WemadeCrypt.SBOX_INIT_1.Length * 4), 0, WemadeCrypt.SBOX_INIT_1, 0, WemadeCrypt.SBOX_INIT_1.Length * 4);
                    Buffer.BlockCopy(br.ReadBytes(WemadeCrypt.SBOX_INIT_2.Length * 4), 0, WemadeCrypt.SBOX_INIT_2, 0, WemadeCrypt.SBOX_INIT_2.Length * 4);
                    Buffer.BlockCopy(br.ReadBytes(WemadeCrypt.SBOX_INIT_3.Length * 4), 0, WemadeCrypt.SBOX_INIT_3, 0, WemadeCrypt.SBOX_INIT_3.Length * 4);
                    Buffer.BlockCopy(br.ReadBytes(WemadeCrypt.SBOX_INIT_4.Length * 4), 0, WemadeCrypt.SBOX_INIT_4, 0, WemadeCrypt.SBOX_INIT_4.Length * 4);
                }
            };


            radioButton1.Checked = true;
            AddLog("Mir2/Mir3 GSP/Kor Dat 转换工具 by Yang", Color.Red);
        }

        public void AddLog(string log, Color color)
        {
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.SelectionColor = color;
            txtLog.SelectedText = log + "\r\n";
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
            Application.DoEvents();
        }

        private void Button_Dat2Xml_Click(object sender, EventArgs e)
        {
            if (Runing)
            {
                AddLog("有任务正在执行，请稍等。。。", Color.Red);
                return;
            }
            //打开文件窗口
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Wemade加密的dat文件 (*.dat)|*.dat|All files (*.*)|*.*";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            if (ofd.FileName == "") return;

            Task.Run(() =>
            {
                Runing = true;
                AddLog("开始解密...... ", Color.Blue);
                try
                {
                    byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(ofd.FileName));
                    string hash = BitConverter.ToString(tmpHash).Replace("-", "").ToLower();

                    byte[] data = File.ReadAllBytes(ofd.FileName);
                    WemadeCrypt.BlowFish_DecryMem(data, data.Length);
                    File.WriteAllBytes(Path.ChangeExtension(ofd.FileName, ".xml"), data);
                    AddLog("解密成功 -> " + Path.ChangeExtension(ofd.FileName, ".xml"), Color.Blue);
                    Runing = false;
                }
                catch (Exception ex)
                {
                    AddLog("解密失败!!!", Color.Red);
                    AddLog("Error:" + ex.ToString(), Color.Red);
                    Runing = false;
                }
            });
            
        }

        private void Button_Xml2Dat_Click(object sender, EventArgs e)
        {
            if (Runing)
            {
                AddLog("有任务正在执行，请稍等。。。", Color.Red);
                return;
            }
            //打开文件窗口
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "解密后的Xml文件 (*.xml)|*.xml|All files (*.*)|*.*";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            if (ofd.FileName == "") return;

            Task.Run(() =>
            {
                Runing = true;
                AddLog("开始加密...... ", Color.Blue);
                try
                {
                    byte[] data = File.ReadAllBytes(ofd.FileName);
                    data = WemadeCrypt.BlowFish_EncryMem(data, data.Length);
                    File.WriteAllBytes(Path.ChangeExtension(ofd.FileName, ".dat"), data);
                    AddLog("加密成功 -> " + Path.ChangeExtension(ofd.FileName, ".dat"), Color.Blue);
                    Runing = false;
                }
                catch (Exception ex)
                {
                    AddLog("加密失败!!!", Color.Red);
                    AddLog("Error:" + ex.ToString(), Color.Red);
                    Runing = false;
                }
            });
            
        }

        private void Button_Xml2Excel_Click(object sender, EventArgs e)
        {
            if (Runing)
            {
                AddLog("有任务正在执行，请稍等。。。", Color.Red);
                return;
            }
            //打开文件窗口
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "解密后的XML文件 (*.xml)|*.xml|All files (*.*)|*.*";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            if (ofd.FileName == "") return;

            Task.Run(() =>
            {
                Runing = true;
                XML.XmlToExcel(ofd.FileName);
                Runing = false;
            });
        }

        

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (Runing) return;
            if (!radioButton1.Checked) return;
            textBox1.ReadOnly = true;
            textBox1.Text = "res070821mir";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (Runing) return;
            if (!radioButton2.Checked) return;
            textBox1.ReadOnly = true;
            textBox1.Text = "Y8vkFwSacHjCFThh";
        }

        private void Button_Excel2Xml_Click(object sender, EventArgs e)
        {
            if (Runing)
            {
                AddLog("有任务正在执行，请稍等。。。", Color.Red);
                return;
            }
            //打开文件窗口
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel文件 (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            if (ofd.FileName == "") return;

            Task.Run(() =>
            {
                Runing = true;
                XML.ExcelToXml(ofd.FileName);
                Runing = false;
            });
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text)) return;

            WemadeCrypt = new WemadeCrypt(textBox1.Text);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (Runing) return;
            textBox1.ReadOnly = false;
        }
    }
}

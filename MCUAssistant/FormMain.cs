using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using INIFILE;
using System.Text.RegularExpressions;

namespace MCUAssistant
{
    public partial class FormMain : Form
    {
        float cryFreq, clkTime;
        int perMacTime;
        int a, b, c;
        Res r1, r2, r3, r4, r5;
        SerialPort sp1 = new SerialPort();
        //sp1.ReceivedBytesThreshold = 1;//只要有1个字符送达端口时便触发DataReceived事件

        struct Res
        {
            public double value;
            public bool enable;
            public int unit;
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            
            setCal();
            initOmegaOption();
            initSerialSetup();

            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            setCal();
        }

        private void initRatio()
        {
            if (radioButton1.Checked) perMacTime=12;
            if (radioButton2.Checked) perMacTime=6;
            if (radioButton3.Checked) perMacTime=4;
            if (radioButton4.Checked) perMacTime=1;
        }

        private void setCal()
        {
            initRatio();
            cryFreq = float.Parse(comboBox1.Text);
            clkTime = 1 / cryFreq;
            labelClkTime.Text = clkTime + "us";
            labelMacTime.Text = clkTime * perMacTime + "us";
            labelMIPS.Text = 1 / (clkTime * perMacTime)*100 + "万条指令";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            setCal();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            setCal();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            setCal();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            setCal();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Calc(int.Parse(textBoxus.Text));
            labelABC.Text = a +"a"+ b+"b" + c+"c";
            textBoxRes.Text = "void delay(void)\r\n{\r\n    unsigned char a,b,c;\r\n    for(c=" + c + ";c>0;c--)\r\n        for(b=" + b + ";b>0;b--)\r\n            for(a=" + a + ";a>0;a--);\r\n}\r\n";

        }

        private void Calc(int time)
        {
            
            for (a = 1; a < 256; a++)
                for (b = 1; b < 256; b++)
                    for (c = 1; c < 256; c++)
                    {
                        if (Math.Abs(2 * a * b * c + 3 * b * c + 3 * c + 3 - time) < 2) return; 
                    }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == 0)
                webBrowser1.Url = new Uri(Application.StartupPath + @"\c51keyhelp.html");
            else if (listBox1.SelectedIndex == 1)
                webBrowser1.Url = new Uri(Application.StartupPath + @"\cjyhelp.mht");
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            initCheck();
            checkRUnit();
            checkREnabel();
            
            textBoxR.Text = calcRes().ToString();
            

        }

        private void checkREnabel()
        {
            if (textBoxR1.Text == "" || textBoxR1.Text == "0")
                r1.enable = false;
            if (textBoxR2.Text == "" || textBoxR2.Text == "0")
                r2.enable = false;
            if (textBoxR3.Text == "" || textBoxR3.Text == "0")
                r3.enable = false;
            if (textBoxR4.Text == "" || textBoxR4.Text == "0")
                r4.enable = false;
            if (textBoxR5.Text == "" || textBoxR5.Text == "0")
                r5.enable = false;
            if (r1.enable)
                r1.value = 1.0 / int.Parse(textBoxR1.Text);
            if (r2.enable)
                r2.value = 1.0 / int.Parse(textBoxR2.Text);
            if (r3.enable)
                r3.value = 1.0 / int.Parse(textBoxR3.Text);
            if (r4.enable)
                r4.value = 1.0 / int.Parse(textBoxR4.Text);
            if (r5.enable)
                r5.value = 1.0 / int.Parse(textBoxR5.Text);

        }

        private double calcRes()
        {
            calcUnitedRes(ref r1);
            calcUnitedRes(ref r2);
            calcUnitedRes(ref r3);
            calcUnitedRes(ref r4);
            calcUnitedRes(ref r5);
            double r;
            r = 1.0 / (r1.value + r2.value + r3.value + r4.value + r5.value);
            return r;
        }

        private void initCheck()
        {
            r1.enable = true;
            r2.enable = true;
            r3.enable = true;
            r4.enable = true;
            r5.enable = true;
            r1.value = 0;
            r2.value = 0;
            r3.value = 0;
            r4.value = 0;
            r5.value = 0;
        }

        private void  initOmegaOption()
        {
            comboBoxOmegaType1.SelectedIndex = 1;
            comboBoxOmegaType2.SelectedIndex = 1;
            comboBoxOmegaType3.SelectedIndex = 1;
            comboBoxOmegaType4.SelectedIndex = 1;
            comboBoxOmegaType5.SelectedIndex = 1;
        }

        private void checkRUnit()
        {
            r1.unit = comboBoxOmegaType1.SelectedIndex;
            r2.unit = comboBoxOmegaType2.SelectedIndex;
            r3.unit = comboBoxOmegaType3.SelectedIndex;
            r4.unit = comboBoxOmegaType4.SelectedIndex;
            r5.unit = comboBoxOmegaType5.SelectedIndex;
        }

        private void calcUnitedRes(ref Res inRes)
        {
            switch (inRes.unit)
            {
                case 0 :
                    inRes.value *= 1000;
                    break;
                case 1:
                    break;
                case 2:
                    inRes.value /= 1000;
                    break;
                case 3:
                    inRes.value /= 1000000;
                    break;
                default:
                    break;
            }
        }

        private void initSerialSetup()
        {
            INIFILE.Profile.LoadProfile();//加载所有

            // 预置波特率
            switch (Profile.G_BAUDRATE)
            {
                case "300":
                    cbBaudRate.SelectedIndex = 0;
                    break;
                case "600":
                    cbBaudRate.SelectedIndex = 1;
                    break;
                case "1200":
                    cbBaudRate.SelectedIndex = 2;
                    break;
                case "2400":
                    cbBaudRate.SelectedIndex = 3;
                    break;
                case "4800":
                    cbBaudRate.SelectedIndex = 4;
                    break;
                case "9600":
                    cbBaudRate.SelectedIndex = 5;
                    break;
                case "19200":
                    cbBaudRate.SelectedIndex = 6;
                    break;
                case "38400":
                    cbBaudRate.SelectedIndex = 7;
                    break;
                case "115200":
                    cbBaudRate.SelectedIndex = 8;
                    break;
                default:
                    {
                        MessageBox.Show("波特率预置参数错误。");
                        return;
                    }
            }

            //预置波特率
            switch (Profile.G_DATABITS)
            {
                case "5":
                    cbDataBits.SelectedIndex = 0;
                    break;
                case "6":
                    cbDataBits.SelectedIndex = 1;
                    break;
                case "7":
                    cbDataBits.SelectedIndex = 2;
                    break;
                case "8":
                    cbDataBits.SelectedIndex = 3;
                    break;
                default:
                    {
                        MessageBox.Show("数据位预置参数错误。");
                        return;
                    }

            }
            //预置停止位
            switch (Profile.G_STOP)
            {
                case "1":
                    cbStop.SelectedIndex = 0;
                    break;
                case "1.5":
                    cbStop.SelectedIndex = 1;
                    break;
                case "2":
                    cbStop.SelectedIndex = 2;
                    break;
                default:
                    {
                        MessageBox.Show("停止位预置参数错误。");
                        return;
                    }
            }

            //预置校验位
            switch (Profile.G_PARITY)
            {
                case "NONE":
                    cbParity.SelectedIndex = 0;
                    break;
                case "ODD":
                    cbParity.SelectedIndex = 1;
                    break;
                case "EVEN":
                    cbParity.SelectedIndex = 2;
                    break;
                default:
                    {
                        MessageBox.Show("校验位预置参数错误。");
                        return;
                    }
            }

            //检查是否含有串口
            string[] str = SerialPort.GetPortNames();
            if (str == null)
            {
                MessageBox.Show("本机没有串口！", "Error");
                return;
            }

            //添加串口项目
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {//获取有多少个COM口
                //System.Diagnostics.Debug.WriteLine(s);
                cbSerial.Items.Add(s);
            }

            //串口设置默认选择项
            cbSerial.SelectedIndex = 0;         //note：获得COM9口，但别忘修改
            //cbBaudRate.SelectedIndex = 5;
            // cbDataBits.SelectedIndex = 3;
            // cbStop.SelectedIndex = 0;
            //  cbParity.SelectedIndex = 0;
            sp1.BaudRate = 9600;

            Control.CheckForIllegalCrossThreadCalls = false;    //这个类中我们不检查跨线程的调用是否合法(因为.net 2.0以后加强了安全机制,，不允许在winform中直接跨线程访问控件的属性)
            sp1.DataReceived += new SerialDataReceivedEventHandler(sp1_DataReceived);
            //sp1.ReceivedBytesThreshold = 1;

            radio1.Checked = true;  //单选按钮默认是选中的
            rbRcvStr.Checked = true;

            //准备就绪              
            sp1.DtrEnable = true;
            sp1.RtsEnable = true;
            //设置数据读取超时为1秒
            sp1.ReadTimeout = 1000;

            sp1.Close();
        }

        void sp1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (sp1.IsOpen)     //此处可能没有必要判断是否打开串口，但为了严谨性，我还是加上了
            {
                //输出当前时间
                DateTime dt = DateTime.Now;
                txtReceive.Text += dt.GetDateTimeFormats('f')[0].ToString() + "\r\n";
                txtReceive.SelectAll();
                txtReceive.SelectionColor = Color.Blue;         //改变字体的颜色

                byte[] byteRead = new byte[sp1.BytesToRead];    //BytesToRead:sp1接收的字符个数
                if (rdSendStr.Checked)                          //'发送字符串'单选按钮
                {
                    try
                    {
                        txtReceive.Text += sp1.ReadLine() + "\r\n"; //注意：回车换行必须这样写，单独使用"\r"和"\n"都不会有效果
                    }
                    catch (System.TimeoutException)
                    {
                        txtReceive.Text += "[发生异常：接收超时]\r\n";
                    }

                    sp1.DiscardInBuffer();                      //清空SerialPort控件的Buffer 
                }
                else                                            //'发送16进制按钮'
                {
                    try
                    {
                        Byte[] receivedData = new Byte[sp1.BytesToRead];        //创建接收字节数组
                        sp1.Read(receivedData, 0, receivedData.Length);         //读取数据
                        //string text = sp1.Read();   //Encoding.ASCII.GetString(receivedData);
                        sp1.DiscardInBuffer();                                  //清空SerialPort控件的Buffer
                        //这是用以显示字符串
                        //    string strRcv = null;
                        //    for (int i = 0; i < receivedData.Length; i++ )
                        //    {
                        //        strRcv += ((char)Convert.ToInt32(receivedData[i])) ;
                        //    }
                        //    txtReceive.Text += strRcv + "\r\n";             //显示信息
                        //}
                        string strRcv = null;
                        //int decNum = 0;//存储十进制
                        for (int i = 0; i < receivedData.Length; i++) //窗体显示
                        {

                            strRcv += receivedData[i].ToString("X2");  //16进制显示
                        }
                        txtReceive.Text += strRcv + "\r\n";
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message, "出错提示");
                        txtSend.Text = "";
                    }
                }
            }
            else
            {
                MessageBox.Show("请打开某个串口", "错误提示");
            }
        }
    }
}

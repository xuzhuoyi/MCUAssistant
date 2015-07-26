using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
        float _cryFreq, _clkTime;
        int _perMacTime;
        int _a, _b, _c;
        Res _r1, _r2, _r3, _r4, _r5;
        readonly SerialPort _sp1 = new SerialPort();
        //sp1.ReceivedBytesThreshold = 1;//只要有1个字符送达端口时便触发DataReceived事件

        struct Res
        {
            public double Value;
            public bool Enable;
            public int Unit;
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            
            SetCal();
            InitOmegaOption();
            InitSerialSetup();
            comboBoxTimer.SelectedIndex = 0;


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCal();
        }

        private void InitRatio()
        {
            if (radioButton1.Checked) _perMacTime=12;
            if (radioButton2.Checked) _perMacTime=6;
            if (radioButton3.Checked) _perMacTime=4;
            if (radioButton4.Checked) _perMacTime=1;
        }

        private void SetCal()
        {
            InitRatio();
            _cryFreq = float.Parse(comboBox1.Text);
            _clkTime = 1 / _cryFreq;
            labelClkTime.Text = _clkTime + "us";
            labelMacTime.Text = _clkTime * _perMacTime + "us";
            labelMIPS.Text = 1 / (_clkTime * _perMacTime)*100 + "万条指令";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            SetCal();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            SetCal();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            SetCal();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            SetCal();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Calc(int.Parse(textBoxus.Text));
            labelABC.Text = _a +"a"+ _b+"b" + _c+"c";
            textBoxRes.Text = "void delay(void)\r\n{\r\n    unsigned char a,b,c;\r\n    for(c=" + _c + ";c>0;c--)\r\n        for(b=" + _b + ";b>0;b--)\r\n            for(a=" + _a + ";a>0;a--);\r\n}\r\n";

        }

        private void Calc(int time)
        {
            
            for (_a = 1; _a < 256; _a++)
                for (_b = 1; _b < 256; _b++)
                    for (_c = 1; _c < 256; _c++)
                    {
                        if (Math.Abs(2 * _a * _b * _c + 3 * _b * _c + 3 * _c + 3 - time) < 2) return; 
                    }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (listBox1.SelectedIndex)
            {
                case 0:
                    webBrowser1.Url = new Uri(Application.StartupPath + @"\c51keyhelp.html");
                    break;
                case 1:
                    webBrowser1.Url = new Uri(Application.StartupPath + @"\cjyhelp.mht");
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            InitCheck();
            CheckRUnit();
            CheckREnabel();

            textBoxR.Text = CalcRes().ToString(CultureInfo.InvariantCulture);
            

        }

        private void CheckREnabel()
        {
            if (textBoxR1.Text == "" || textBoxR1.Text == "0")
                _r1.Enable = false;
            if (textBoxR2.Text == "" || textBoxR2.Text == "0")
                _r2.Enable = false;
            if (textBoxR3.Text == "" || textBoxR3.Text == "0")
                _r3.Enable = false;
            if (textBoxR4.Text == "" || textBoxR4.Text == "0")
                _r4.Enable = false;
            if (textBoxR5.Text == "" || textBoxR5.Text == "0")
                _r5.Enable = false;
            if (_r1.Enable)
                _r1.Value = 1.0 / int.Parse(textBoxR1.Text);
            if (_r2.Enable)
                _r2.Value = 1.0 / int.Parse(textBoxR2.Text);
            if (_r3.Enable)
                _r3.Value = 1.0 / int.Parse(textBoxR3.Text);
            if (_r4.Enable)
                _r4.Value = 1.0 / int.Parse(textBoxR4.Text);
            if (_r5.Enable)
                _r5.Value = 1.0 / int.Parse(textBoxR5.Text);

        }

        private double CalcRes()
        {
            CalcUnitedRes(ref _r1);
            CalcUnitedRes(ref _r2);
            CalcUnitedRes(ref _r3);
            CalcUnitedRes(ref _r4);
            CalcUnitedRes(ref _r5);
            double r = 0;
            r = 1.0 / (_r1.Value + _r2.Value + _r3.Value + _r4.Value + _r5.Value);
            return r;
        }

        private void InitCheck()
        {
            _r1.Enable = true;
            _r2.Enable = true;
            _r3.Enable = true;
            _r4.Enable = true;
            _r5.Enable = true;
            _r1.Value = 0;
            _r2.Value = 0;
            _r3.Value = 0;
            _r4.Value = 0;
            _r5.Value = 0;
        }

        private void  InitOmegaOption()
        {
            comboBoxOmegaType1.SelectedIndex = 1;
            comboBoxOmegaType2.SelectedIndex = 1;
            comboBoxOmegaType3.SelectedIndex = 1;
            comboBoxOmegaType4.SelectedIndex = 1;
            comboBoxOmegaType5.SelectedIndex = 1;
        }

        private void CheckRUnit()
        {
            _r1.Unit = comboBoxOmegaType1.SelectedIndex;
            _r2.Unit = comboBoxOmegaType2.SelectedIndex;
            _r3.Unit = comboBoxOmegaType3.SelectedIndex;
            _r4.Unit = comboBoxOmegaType4.SelectedIndex;
            _r5.Unit = comboBoxOmegaType5.SelectedIndex;
        }

        private static void CalcUnitedRes(ref Res inRes)
        {
            switch (inRes.Unit)
            {
                case 0 :
                    inRes.Value *= 1000;
                    break;
                case 1:
                    break;
                case 2:
                    inRes.Value /= 1000;
                    break;
                case 3:
                    inRes.Value /= 1000000;
                    break;
                default:
                    break;
            }
        }

        private void InitSerialSetup()
        {
            INIFILE.Profile.LoadProfile();//加载所有

            // 预置波特率
            switch (Profile.GBudrate)
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
            switch (Profile.GDatabits)
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
            switch (Profile.GStop)
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
            switch (Profile.GParity)
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
            var str = SerialPort.GetPortNames();
            if (str.Length == 0)
            {
                MessageBox.Show("本机没有串口！", "Error");
                return;
            }

            //添加串口项目
            foreach (var s in System.IO.Ports.SerialPort.GetPortNames())
            {//获取有多少个COM口
                //System.Diagnostics.Debug.WriteLine(s);
                cbSerial.Items.Add(s);
            }

            //串口设置默认选择项
            if (cbSerial.Items.Count>0)
                cbSerial.SelectedIndex = 0;         //note：获得COM9口，但别忘修改
            //cbBaudRate.SelectedIndex = 5;
            // cbDataBits.SelectedIndex = 3;
            // cbStop.SelectedIndex = 0;
            //  cbParity.SelectedIndex = 0;
            _sp1.BaudRate = 9600;

            Control.CheckForIllegalCrossThreadCalls = false;    //这个类中我们不检查跨线程的调用是否合法(因为.net 2.0以后加强了安全机制,，不允许在winform中直接跨线程访问控件的属性)
            _sp1.DataReceived += new SerialDataReceivedEventHandler(sp1_DataReceived);
            //sp1.ReceivedBytesThreshold = 1;

            radio1.Checked = true;  //单选按钮默认是选中的
            rbRcvStr.Checked = true;

            //准备就绪              
            _sp1.DtrEnable = true;
            _sp1.RtsEnable = true;
            //设置数据读取超时为1秒
            _sp1.ReadTimeout = 1000;

            _sp1.Close();
        }

        void sp1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_sp1.IsOpen)     //此处可能没有必要判断是否打开串口，但为了严谨性，我还是加上了
            {
                //输出当前时间
                var dt = DateTime.Now;
                txtReceive.Text += dt.GetDateTimeFormats('f')[0].ToString() + "\r\n";
                txtReceive.SelectAll();
                txtReceive.SelectionColor = Color.Blue;         //改变字体的颜色

                var byteRead = new byte[_sp1.BytesToRead];    //BytesToRead:sp1接收的字符个数
                if (rbRcvStr.Checked)                          //'发送字符串'单选按钮
                {
                    try
                    {
                        txtReceive.Text += _sp1.ReadLine() + "\r\n"; //注意：回车换行必须这样写，单独使用"\r"和"\n"都不会有效果
                    }
                    catch (System.TimeoutException)
                    {
                        txtReceive.Text += "[发生异常：接收超时]\r\n";
                    }

                    _sp1.DiscardInBuffer();                      //清空SerialPort控件的Buffer 
                }
                else                                            //'发送16进制按钮'
                {
                    try
                    {
                        Byte[] receivedData = new Byte[_sp1.BytesToRead];        //创建接收字节数组
                        _sp1.Read(receivedData, 0, receivedData.Length);         //读取数据
                        //string text = sp1.Read();   //Encoding.ASCII.GetString(receivedData);
                        _sp1.DiscardInBuffer();                                  //清空SerialPort控件的Buffer
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
                       
                        for (var i = 0; i < receivedData.Length; i++) //窗体显示
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

        private void btnSend_Click(object sender, EventArgs e)
        {
            tmSend.Enabled = cbTimeSend.Checked ? true : false;

            if (!_sp1.IsOpen) //如果没打开
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }

            var strSend = txtSend.Text;
            if (radio1.Checked == true)	//“HEX发送” 按钮 
            {
                //处理数字转换
                var sendBuf = strSend;
                var sendnoNull = sendBuf.Trim();
                var sendNoComma = sendnoNull.Replace(',', ' ');    //去掉英文逗号
                var sendNoComma1 = sendNoComma.Replace('，', ' '); //去掉中文逗号
                var strSendNoComma2 = sendNoComma1.Replace("0x", "");   //去掉0x
                var strSendNo0X = strSendNoComma2.Replace("0X", "");   //去掉0X
                var strArray = strSendNo0X.Split(' ');

                var byteBufferLength = strArray.Length;
                var blankStr = strArray.Where(data =>
                {
                    var result = data == "";
                    return result;

                });
                foreach (var i in blankStr)
                {
                    byteBufferLength--;
                }
                /*for (int i = 0; i < strArray.Length; i++)
                {
                    if (strArray[i] == "")
                    {
                        byteBufferLength--;
                    }
                }*/
                // int temp = 0;
                var byteBuffer = new byte[byteBufferLength];
                var ii = 0;
                for (var i = 0; i < strArray.Length; i++)        //对获取的字符做相加运算
                {

                    Byte[] bytesOfStr = Encoding.Default.GetBytes(strArray[i]);

                    var decNum = 0;
                    if (strArray[i] == "")
                    {
                        //ii--;     //加上此句是错误的，下面的continue以延缓了一个ii，不与i同步
                        continue;
                    }
                    else
                    {
                        try
                        {
                            decNum = Convert.ToInt32(strArray[i], 16); //atrArray[i] == 12时，temp == 18
                        }
                        catch (System.FormatException ex)
                        {
                            MessageBox.Show(ex.Message, "出错提示");
                            return;
                        }
                         
                    }

                    try    //防止输错，使其只能输入一个字节的字符
                    {
                        byteBuffer[ii] = Convert.ToByte(decNum);
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("字节越界，请逐个字节输入！", "Error");
                        tmSend.Enabled = false;
                        return;
                    }

                    ii++;
                }
                _sp1.Write(byteBuffer, 0, byteBuffer.Length);
            }
            else		//以字符串形式发送时 
            {
                _sp1.Write(txtSend.Text);    //写入数据
            }
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            //serialPort1.IsOpen
            if (!_sp1.IsOpen)
            {
                try
                {
                    //设置串口号
                    var serialName = cbSerial.SelectedItem.ToString();
                    _sp1.PortName = serialName;

                    //设置各“串口设置”
                    var strBaudRate = cbBaudRate.Text;
                    var strDateBits = cbDataBits.Text;
                    var strStopBits = cbStop.Text;
                    var iBaudRate = Convert.ToInt32(strBaudRate);
                    var iDateBits = Convert.ToInt32(strDateBits);

                    _sp1.BaudRate = iBaudRate;       //波特率
                    _sp1.DataBits = iDateBits;       //数据位
                    switch (cbStop.Text)            //停止位
                    {
                        case "1":
                            _sp1.StopBits = StopBits.One;
                            break;
                        case "1.5":
                            _sp1.StopBits = StopBits.OnePointFive;
                            break;
                        case "2":
                            _sp1.StopBits = StopBits.Two;
                            break;
                        default:
                            MessageBox.Show("Error：参数不正确!", "Error");
                            break;
                    }
                    switch (cbParity.Text)             //校验位
                    {
                        case "无":
                            _sp1.Parity = Parity.None;
                            break;
                        case "奇校验":
                            _sp1.Parity = Parity.Odd;
                            break;
                        case "偶校验":
                            _sp1.Parity = Parity.Even;
                            break;
                        default:
                            MessageBox.Show("Error：参数不正确!", "Error");
                            break;
                    }

                    if (_sp1.IsOpen == true)//如果打开状态，则先关闭一下
                    {
                        _sp1.Close();
                    }
                    //状态栏设置
                    tsSpNum.Text = "串口号：" + _sp1.PortName + "|";
                    tsBaudRate.Text = "波特率：" + _sp1.BaudRate + "|";
                    tsDataBits.Text = "数据位：" + _sp1.DataBits + "|";
                    tsStopBits.Text = "停止位：" + _sp1.StopBits + "|";
                    tsParity.Text = "校验位：" + _sp1.Parity + "|";

                    //设置必要控件不可用
                    cbSerial.Enabled = false;
                    cbBaudRate.Enabled = false;
                    cbDataBits.Enabled = false;
                    cbStop.Enabled = false;
                    cbParity.Enabled = false;

                    _sp1.Open();     //打开串口
                    btnSwitch.Text = "关闭串口";
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message, "Error");
                    tmSend.Enabled = false;
                    return;
                }
            }
            else
            {
                //状态栏设置
                tsSpNum.Text = "串口号：未指定|";
                tsBaudRate.Text = "波特率：未指定|";
                tsDataBits.Text = "数据位：未指定|";
                tsStopBits.Text = "停止位：未指定|";
                tsParity.Text = "校验位：未指定|";
                //恢复控件功能
                //设置必要控件不可用
                cbSerial.Enabled = true;
                cbBaudRate.Enabled = true;
                cbDataBits.Enabled = true;
                cbStop.Enabled = true;
                cbParity.Enabled = true;

                _sp1.Close();                    //关闭串口
                btnSwitch.Text = "打开串口";
                tmSend.Enabled = false;         //关闭计时器
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtReceive.Text = "";       //清空文本
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            INIFILE.Profile.SaveProfile();
            _sp1.Close();
        }

        private void txtSend_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (radio1.Checked == true)
            {
                //正则匹配
                const string patten = "[0-9a-fA-F]|\b|0x|0X| "; //“\b”：退格键
                var r = new Regex(patten);
                var m = r.Match(e.KeyChar.ToString());

                e.Handled = !m.Success;
                /*if (m.Success)//&&(txtSend.Text.LastIndexOf(" ") != txtSend.Text.Length-1))
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }*/
            }//end of radio1
            else
            {
                e.Handled = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //设置各“串口设置”
            var strBaudRate = cbBaudRate.Text;
            var strDateBits = cbDataBits.Text;
            var strStopBits = cbStop.Text;
            var iBaudRate = Convert.ToInt32(strBaudRate);
            var iDateBits = Convert.ToInt32(strDateBits);

            Profile.GBudrate = iBaudRate + "";       //波特率
            Profile.GDatabits = iDateBits + "";       //数据位
            switch (cbStop.Text)            //停止位
            {
                case "1":
                    Profile.GStop = "1";
                    break;
                case "1.5":
                    Profile.GStop = "1.5";
                    break;
                case "2":
                    Profile.GStop = "2";
                    break;
                default:
                    MessageBox.Show("Error：参数不正确!", "Error");
                    break;
            }
            switch (cbParity.Text)             //校验位
            {
                case "无":
                    Profile.GParity = "NONE";
                    break;
                case "奇校验":
                    Profile.GParity = "ODD";
                    break;
                case "偶校验":
                    Profile.GParity = "EVEN";
                    break;
                default:
                    MessageBox.Show("Error：参数不正确!", "Error");
                    break;
            }

            //保存设置
            // public static string G_BAUDRATE = "1200";//给ini文件赋新值，并且影响界面下拉框的显示
            //public static string G_DATABITS = "8";
            //public static string G_STOP = "1";
            //public static string G_PARITY = "NONE";
            Profile.SaveProfile();
        }

        private void tmSend_Tick(object sender, EventArgs e)
        {
            //转换时间间隔
            var strSecond = txtSecond.Text;
            try
            {
                var isecond = int.Parse(strSecond) * 1000;//Interval以微秒为单位
                tmSend.Interval = isecond;
                if (tmSend.Enabled == true)
                {
                    btnSend.PerformClick();
                }
            }
            catch (System.Exception)
            {
                tmSend.Enabled = false;
                MessageBox.Show("错误的定时输入！", "Error");
            }
        }

        private void txtSecond_KeyPress(object sender, KeyPressEventArgs e)
        {
            const string patten = "[0-9]|\b"; //“\b”：退格键
            var r = new Regex(patten);
            var m = r.Match(e.KeyChar.ToString());

            e.Handled = !m.Success;  //没操作“过”，系统会处理事件
            
        }

        private void radioButtonS0_CheckedChanged(object sender, EventArgs e)
        {
            CalcTmod();
        }

        private void CalcTmod()
        {
            byte tmod = 0;
            if (checkBoxGate.Checked == true)
            {
                tmod |= 0x88;
            }
            if (checkBoxCounter.Checked == true)
            {
                tmod |= 0x44;
            }
            if (radioButtonS0.Checked == true)
            {
                tmod |= 0;
            }
            if (radioButtonS1.Checked == true)
            {
                tmod |= 0x11;
            }
            if (radioButtonS2.Checked == true)
            {
                tmod |= 0x22;
            }
            if (radioButtonS3.Checked == true)
            {
                tmod |= 0x51;
            }
            if (comboBoxTimer.SelectedIndex == 0)
            {
                tmod &= 0x0F;
            }
            else
            {
                tmod &= 0xF0;
            }
            textBoxTmod.Text = tmod.ToString();
        }

        private void buttonGen_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxGate_CheckedChanged(object sender, EventArgs e)
        {
            CalcTmod();
        }

        private void checkBoxCounter_CheckedChanged(object sender, EventArgs e)
        {
            CalcTmod();
        }

        private void comboBoxTimer_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcTmod();
        }

        private void radioButtonS1_CheckedChanged(object sender, EventArgs e)
        {
            CalcTmod();
        }

        private void radioButtonS2_CheckedChanged(object sender, EventArgs e)
        {
            CalcTmod();
        }

        private void radioButtonS3_CheckedChanged(object sender, EventArgs e)
        {
            CalcTmod();
        }
    }
}

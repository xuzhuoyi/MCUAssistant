using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCUAssistant
{
    public partial class FormMain : Form
    {
        float cryFreq, clkTime;
        int perMacTime;
        int a, b, c;
        Res r1, r2, r3, r4, r5;

        struct Res
        {
            public float value;
            public bool enable;
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            
            setCal();
            
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
                r1.value = 1 / int.Parse(textBoxR1.Text);
            if (r2.enable)
                r2.value = 1 / int.Parse(textBoxR2.Text);
            if (r3.enable)
                r3.value = 1 / int.Parse(textBoxR3.Text);
            if (r4.enable)
                r4.value = 1 / int.Parse(textBoxR4.Text);
            if (r5.enable)
                r5.value = 1 / int.Parse(textBoxR5.Text);

        }

        private float calcRes()
        {
            float r;
            r = 1 / (r1.value + r2.value + r3.value + r4.value + r5.value);
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
    }
}

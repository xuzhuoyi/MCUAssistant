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
    }
}

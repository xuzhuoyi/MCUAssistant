using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INIFILE
{
    class Profile
    {
        public static void LoadProfile()
        {
            var strPath = AppDomain.CurrentDomain.BaseDirectory;
            _file = new IniFile(strPath + "Cfg.ini");
            GBudrate = _file.ReadString("CONFIG", "BaudRate", "4800");    //读数据，下同
            GDatabits = _file.ReadString("CONFIG", "DataBits", "8");
            GStop = _file.ReadString("CONFIG", "StopBits", "1");
            GParity = _file.ReadString("CONFIG", "Parity", "NONE");
         
        }

        public static void SaveProfile()
        {
            var strPath = AppDomain.CurrentDomain.BaseDirectory;
            _file = new IniFile(strPath + "Cfg.ini");
            _file.WriteString("CONFIG", "BaudRate", GBudrate);            //写数据，下同
            _file.WriteString("CONFIG", "DataBits", GDatabits);
            _file.WriteString("CONFIG", "StopBits", GStop);
            _file.WriteString("CONFIG", "G_PARITY", GParity);
        }

        private static IniFile _file;//内置了一个对象

        public static string GBudrate = "1200";//给ini文件赋新值，并且影响界面下拉框的显示
        public static string GDatabits = "8";
        public static string GStop = "1";
        public static string GParity = "NONE";
        
    }
}

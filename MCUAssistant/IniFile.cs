using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace INIFILE
{
    public abstract class CustomIniFile
    {
        protected CustomIniFile(string aFileName)
        {
            _fFileName = aFileName;
        }
        private readonly string _fFileName;
        public string FileName
        {
            get { return _fFileName; }
        }
        public virtual bool SectionExists(string section)
        {
            var vStrings = new List<string>();
            ReadSections(vStrings);
            return vStrings.Contains(section);
        }
        public virtual bool ValueExists(string section, string ident)
        {
            var vStrings = new List<string>();
            ReadSection(section, vStrings);
            return vStrings.Contains(ident);
        }
        public abstract string ReadString(string section, string ident, string Default);
        public abstract bool WriteString(string section, string ident, string value);
        public abstract bool ReadSectionValues(string section, List<string> strings);
        public abstract bool ReadSection(string section, List<string> strings);
        public abstract bool ReadSections(List<string> strings);
        public abstract bool EraseSection(string section);
        public abstract bool DeleteKey(string section, string ident);
        public abstract bool UpdateFile();
    }
   /// <summary>
    /// 存储本地INI文件的类。
    /// </summary>
    public class IniFile : CustomIniFile
    {
        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileString(
            string lpAppName,    // points to section name 
            string lpKeyName,    // points to key name 
            string lpDefault,    // points to default string 
            byte[] lpReturnedString,    // points to destination buffer 
            uint nSize,    // size of destination buffer 
            string lpFileName     // points to initialization filename 
        );

        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(
            string lpAppName,    // pointer to section name 
            string lpKeyName,    // pointer to key name 
            string lpString,    // pointer to string to add 
            string lpFileName     // pointer to initialization filename 
        );

        /// <summary>
        /// 构造IniFile实例。
        /// <param name="aFileName">指定文件名</param>
        /// </summary>
        public IniFile(string aFileName)
            : base(aFileName)
        {
        }

        /// <summary>
        /// 析够IniFile实例。
        /// </summary>
        ~IniFile()
        {
            UpdateFile();
        }

        /// <summary>
        /// 读取字符串值。
        /// <param name="ident">指定变量标识。</param>
        /// <param name="section">指定所在区域。</param>
        /// <param name="Default">指定默认值。</param>
        /// <returns>返回读取的字符串。如果读取失败则返回该值。</returns>
        /// </summary>
        public override string ReadString(string section, string ident, string Default)
        {
            var vBuffer = new byte[2048];
            var vCount = GetPrivateProfileString(section,
                ident, Default, vBuffer, (uint)vBuffer.Length, FileName);
            return Encoding.Default.GetString(vBuffer, 0, (int)vCount);
        }
        /// <summary>
        /// 写入字符串值。
        /// </summary>
        /// <param name="section">指定所在区域。</param>
        /// <param name="ident">指定变量标识。</param>
        /// <param name="value">所要写入的变量值。</param>
        /// <returns>返回写入是否成功。</returns>
        public override bool WriteString(string section, string ident, string value)
        {
            return WritePrivateProfileString(section, ident, value, FileName);
        }

        /// <summary>
        /// 获得区域的完整文本。(变量名=值格式)。
        /// </summary>
        /// <param name="section">指定区域标识。</param>
        /// <param name="strings">输出处理结果。</param>
        /// <returns>返回读取是否成功。</returns>
        public override bool ReadSectionValues(string section, List<string> strings)
        {
            var vIdentList = new List<string>();
            if (!ReadSection(section, vIdentList)) return false;
            foreach (string vIdent in vIdentList)
                strings.Add(string.Format("{0}={1}", vIdent, ReadString(section, vIdent, "")));
            return true;
        }

        /// <summary>
        /// 读取区域变量名列表。
        /// </summary>
        /// <param name="Section">指定区域名。</param>
        /// <param name="Strings">指定输出列表。</param>
        /// <returns>返回获取是否成功。</returns>
        public override bool ReadSection(string Section, List<string> Strings)
        {
            var vBuffer = new byte[16384];
            var vLength = GetPrivateProfileString(Section, null, null, vBuffer,
                (uint)vBuffer.Length, FileName);
            var j = 0;
            for (var i = 0; i < vLength; i++)
                if (vBuffer[i] == 0)
                {
                    Strings.Add(Encoding.Default.GetString(vBuffer, j, i - j));
                    j = i + 1;
                }
            return true;
        }

        /// <summary>
        /// 读取区域名列表。
        /// </summary>
        /// <param name="strings">指定输出列表。</param>
        /// <returns></returns>
        public override bool ReadSections(List<string> strings)
        {
            var vBuffer = new byte[16384];
            var vLength = GetPrivateProfileString(null, null, null, vBuffer,
                (uint)vBuffer.Length, FileName);
            var j = 0;
            for (var i = 0; i < vLength; i++)
                if (vBuffer[i] == 0)
                {
                    strings.Add(Encoding.Default.GetString(vBuffer, j, i - j));
                    j = i + 1;
                }
            return true;
        }

        /// <summary>
        /// 删除指定区域。
        /// </summary>
        /// <param name="section">指定区域名。</param>
        /// <returns>返回删除是否成功。</returns>
        public override bool EraseSection(string section)
        {
            return WritePrivateProfileString(section, null, null, FileName);
        }

        /// <summary>
        /// 删除指定变量。
        /// </summary>
        /// <param name="section">变量所在区域。</param>
        /// <param name="ident">变量标识。</param>
        /// <returns>返回删除是否成功。</returns>
        public override bool DeleteKey(string section, string ident)
        {
            return WritePrivateProfileString(section, ident, null, FileName);
        }

        /// <summary>
        /// 更新文件。
        /// </summary>
        /// <returns>返回更新是否成功。</returns>
        public override bool UpdateFile()
        {
            return WritePrivateProfileString(null, null, null, FileName);
        }
    }
}

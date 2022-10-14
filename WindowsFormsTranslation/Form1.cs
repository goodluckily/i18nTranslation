using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Newtonsoft.Json;

namespace WindowsFormsTranslation
{
    public partial class Form1 : Form
    {
        //APP ID 
        public static string appId = "20210420000791175";
        //密钥
        public static string secretKey = "xhRL6tzMl5acjsZHaapC";

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //结果
            var nameText = textBox1.Text?.Trim();
            var descText = textBox2.Text?.Trim();

            var unescapeRetString = GetUnescapeRetString(descText, "zh", "en");
            var amode = JsonConvert.DeserializeObject<TranslationResult>(unescapeRetString);
            var traditionalStr = ChineseConverter.Convert(descText, ChineseConversionDirection.SimplifiedToTraditional);
            //拼接数据
            var strBudder = new StringBuilder();
            
            //strBudder.AppendLine($"{Environment.NewLine}");
            
            strBudder.AppendLine($"\"{nameText}\":\"{descText}\",");
            strBudder.AppendLine($"\"{nameText}\":\"{traditionalStr}\",");
            strBudder.AppendLine($"\"{nameText}\":\"{amode.trans_result.FirstOrDefault()?.dst}\",");
            
            //strBudder.AppendLine($"{Environment.NewLine}");

            SetRichTextBox1(strBudder.ToString(), color: Color.Black);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            richTextBox1.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void 变量名称_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 翻译 中文(zh) 英语(en) 繁体中文(cht)
        /// </summary>
        /// <param name="text">原文</param>
        /// <param name="from">源语言</param>
        /// <param name="to">目标语言</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetUnescapeRetString(string text, string from, string to)
        {
            try
            {
                Random rd = new Random();
                string salt = rd.Next(100000).ToString();

                string sign = EncryptString(appId + text + salt + secretKey);
                string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
                url += "q=" + HttpUtility.UrlEncode(text);
                url += "&from=" + from;
                url += "&to=" + to;
                url += "&appid=" + appId;
                url += "&salt=" + salt;
                url += "&sign=" + sign;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html";
                request.UserAgent = null;
                request.Timeout = 6000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream);
                string retString = myStreamReader.ReadToEnd();
                var UnescapeRetString = Regex.Unescape(retString);
                myStreamReader.Close();
                myResponseStream.Close();
                return UnescapeRetString;
            }
            catch (Exception ex)
            {
                SetRichTextBox1(ex.Message, color: Color.Red);
                throw new Exception(ex.Message);
            }
        }

        public void SetRichTextBox1(string str, Color color = default(Color))
        {
            richTextBox1.Clear();  // 清除文本内容

            richTextBox1.BulletIndent = 30;//指定文本距离控件最左边缩进30个像素
            richTextBox1.SelectionFont = new Font("Arial", 13);//设置当前插入文本的16号字体
            richTextBox1.SelectionColor = color;  // 设定插入文本的颜色
            richTextBox1.SelectedText = str;  // 插入的文本
        }

        // 计算MD5值
        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }


        /// <summary>
        /// 接收百度翻译API结果的实体类
        /// </summary>

        public class TranslationResult
        {
            public string from { get; set; }
            public string to { get; set; }
            public Trans_Result[] trans_result { get; set; }
        }

        public class Trans_Result
        {
            public string src { get; set; }
            public string dst { get; set; }
        }

        public class TranslationErrorResult
        {
            public string error_code { get; set; }
            public string error_msg { get; set; }
        }

        
    }
}

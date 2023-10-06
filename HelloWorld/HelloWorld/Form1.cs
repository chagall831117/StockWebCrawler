using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Windows.Forms.DataVisualization.Charting;

namespace HelloWorld
{
    public partial class Form1 : Form
    {
        private float TimeCount = 0;
        public List<int> StockList = new List<int>();
        public int StockCount = 0;
        public int NowPrize;
        Random ClsRoom = new Random();
        public Form1()
        {
            InitializeComponent();

            //設定ChartArea
            ChartArea chartArea = new ChartArea("ViewArea");
            DateTime t = DateTime.Parse("9:00");
            for (int i = 1; i < 300; i++)
            {
                if (i % 2 == 1)
                {
                    CustomLabel label = new CustomLabel();
                    label.Text = t.ToShortTimeString();
                    label.ToPosition = i;
                    chartArea.AxisX.CustomLabels.Add(label);
                    label.GridTicks = GridTickTypes.Gridline;
                    t = t.AddMinutes(1);
                }
            }
            chartArea.AxisX.Minimum = 0; //X軸數值從0開始
            chartArea.AxisY.Minimum = 510;
            chartArea.AxisY.Maximum = 540;
            chartArea.AxisX.ScaleView.Size = 10; //設定視窗範圍內一開始要顯示多少點
            chartArea.AxisX.Interval = 5;
            chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;//設定ScrollBar
            chart1.ChartAreas[0] = chartArea; //chart new出來內建第一個chartArea

        }
        public void RefreshChart(object sender, EventArgs e)
        {
            GetPrizeNow();
            label1.Text = "台積電現在股價為:" + NowPrize;
            //新增一個點至Series中
            chart1.Series[0].Points.AddXY(TimeCount, NowPrize);
            if (TimeCount > 10)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.Position = TimeCount - 10;
                //將視窗焦點維持在最新的點那邊
            }
            TimeCount++;
            StockCount++;
        }
        public class SearchResult
        {
            public string text { get; set; }
        }


        private void GetPrizeNow()
        {
            string ss = HttpGet("https://tw.stock.yahoo.com/quote/2330");
            Console.WriteLine("SS:" + ss);
            int FirstStringPosition = ss.IndexOf("加入自選股");
            Console.WriteLine("FirstStringPosition:" + FirstStringPosition);
            string stringBetweenTwoStrings = ss.Substring(FirstStringPosition,300);
            Console.WriteLine("stringBetweenTwoStrings:" + stringBetweenTwoStrings);
            int FirstMendPosition = stringBetweenTwoStrings.IndexOf("Mend") + 38;
            string nowprize = stringBetweenTwoStrings.Substring(FirstMendPosition, 3);
            Console.WriteLine("NowPrize:"+Convert.ToInt32(nowprize));
            NowPrize = Convert.ToInt32(nowprize);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //設定Timer
            Timer clsTimer = new Timer();
            clsTimer.Tick += new EventHandler(RefreshChart);
            clsTimer.Interval = 60 * 1000;//間隔為1秒
            clsTimer.Start();
            /*
            int FirstLowString = ss.IndexOf("\"low\"") +6;
            int FirstOpenString = ss.IndexOf("\"open\"") - 1;
            string LowOpen = ss.Substring(FirstLowString, FirstOpenString - FirstLowString);
            LowOpen = LowOpen.Substring(1, LowOpen.Length - 2); //去頭去尾
            Console.WriteLine(LowOpen);
            while(LowOpen.Length > 4)
            {
                string ThisTimePrize = LowOpen.Substring(0, LowOpen.IndexOf(","));
                Console.WriteLine("ThisTimePrize:"+ThisTimePrize);
                if (ThisTimePrize == "null")
                {
                    Console.WriteLine("ISNULL");
                    LowOpen = LowOpen.Substring(LowOpen.IndexOf(",") + 1);
                    Console.WriteLine("LO:"+LowOpen);
                    continue;
                }
                else
                {
                    Console.WriteLine("ISADD");
                    StockList.Add(Convert.ToInt32(LowOpen.Substring(0, LowOpen.IndexOf(","))));
                    Console.WriteLine("[{0}]", string.Join(", ", StockList));
                    LowOpen = LowOpen.Substring(LowOpen.IndexOf(",") + 1);
                    Console.WriteLine("LO:"+LowOpen);
                }
            }
            if (LowOpen == "null")
            {
                Console.WriteLine("ISNULL");
                return;
            }
            else
            {
                Console.WriteLine("ISADD");
                StockList.Add(Convert.ToInt32(LowOpen));
                Console.WriteLine("[{0}]", string.Join(", ", StockList));
            }
            */

        }
        public static string HttpGet(string url)
        {
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(),Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {
        }
    }
}

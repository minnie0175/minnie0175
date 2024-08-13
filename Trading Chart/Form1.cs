using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Timers;

namespace test
{
    public partial class Form1 : Form
    {
        public class Market
        {
            public string market { get; set; }
            public string korean_name { get; set; }
            public string english_name { get; set; }
            public string market_warning { get; set; }
        }

        public class Ticker
        {
            public string market { get; set; }
            public string trade_date { get; set; }
            public string trade_time { get; set; }
            public string trade_date_kst { get; set; }
            public string trade_time_kst { get; set; }
            public long trade_timestamp { get; set; }
            public double opening_price { get; set; }
            public double high_price { get; set; }
            public double low_price { get; set; }
            public double trade_price { get; set; }
            public double prev_closing_price { get; set; }
            public string change { get; set; }
            public double change_price { get; set; }
            public double change_rate { get; set; }
            public double signed_change_price { get; set; }
            public double signed_change_rate { get; set; }
            public double trade_volume { get; set; }
            public double acc_trade_price { get; set; }
            public double acc_trade_price_24h { get; set; }
            public double acc_trade_volume { get; set; }
            public double acc_trade_volume_24h { get; set; }
            public double highest_52_week_price { get; set; }
            public string highest_52_week_date { get; set; }
            public double lowest_52_week_price { get; set; }
            public string lowest_52_week_date { get; set; }
            public long timestamp { get; set; }
        }

        List<Market> markets_list = new List<Market>();
        List<Ticker> tickers_list = new List<Ticker>();
        private string m_url = "";
        private string t_url = "";
        private string site = "";
        private System.Timers.Timer fetchTimer;
        List<string> selectedColumns = new List<string> { "market", "korean_name", "trade_price", "signed_change_rate", "signed_change_price", "acc_trade_price_24h" };

        public Form1()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            fetchTimer = new System.Timers.Timer(5000); // Set the interval to 5000 milliseconds (5 seconds)
            fetchTimer.Elapsed += OnTimedEvent;
            fetchTimer.AutoReset = true;
            fetchTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Invoke on the UI thread
            this.Invoke((MethodInvoker)delegate
            {
                // Save current scroll position
                int firstDisplayedRowIndex = dataGridView1.FirstDisplayedScrollingRowIndex;
                int firstDisplayedColumnIndex = dataGridView1.FirstDisplayedScrollingColumnIndex;
                wGetData();
                UpdateDataGridView();


                // Restore scroll position
                if (firstDisplayedRowIndex >= 0)
                {
                    dataGridView1.FirstDisplayedScrollingRowIndex = firstDisplayedRowIndex;
                }
                if (firstDisplayedColumnIndex >= 0)
                {
                    dataGridView1.FirstDisplayedScrollingColumnIndex = firstDisplayedColumnIndex;
                }

            });
        }

        private void fGetData()
        {
            StreamReader sr = new StreamReader("C:\\Users\\minni\\Downloads\\market_all.json");
            string market_json = sr.ReadToEnd();
            sr = new StreamReader("C:\\Users\\minni\\Downloads\\ticker.json");
            string ticker_json = sr.ReadToEnd();
            markets_list = JsonConvert.DeserializeObject<List<Market>>(market_json);
            tickers_list = JsonConvert.DeserializeObject<List<Ticker>>(ticker_json);
        }
        private void wGetData()
        {

            var options = new RestClientOptions(m_url);
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddHeader("accept", "application/json");
            var response = client.Execute(request);
            string markets = "";
            if (response.IsSuccessful)
            {
                //Console.WriteLine("{0}", response.Content);
                markets_list = JsonConvert.DeserializeObject<List<Market>>(response.Content.ToString());

                markets = markets_list[0].market;
                for(int i = 1; i < markets_list.Count; i++)
                {
                    markets += ",";
                    markets += markets_list[i].market;
                }



            }
            else
            {
                Console.WriteLine("Error: {0}", response.ErrorMessage);
            }


            options = new RestClientOptions(t_url);
            client = new RestClient(options);
            request = new RestRequest();
            request.AddQueryParameter("markets", markets);
            request.AddHeader("accept", "application/json");
            response = client.Execute(request);

            if (response.IsSuccessful)
            {

                tickers_list = JsonConvert.DeserializeObject<List<Ticker>>(response.Content.ToString());

            }
            else
            {
                Console.WriteLine("Error: {0}", response.ErrorMessage);
            }
        }

        private DataTable ConvertToDataTable<T>(List<T> data)
        {
            DataTable dataTable = new DataTable();

            // Define columns
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Populate rows
            foreach (var item in data)
            {
                DataRow row = dataTable.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private DataTable JoinMarketAndTickerData()
        {
            var joinedData = from market in markets_list
                             join ticker in tickers_list on market.market equals ticker.market
                             orderby ticker.acc_trade_price_24h descending
                             select new
                             {
                                 market.market,
                                 market.korean_name,
                                 market.english_name,
                                 market.market_warning,
                                 ticker.trade_date,
                                 ticker.trade_time,
                                 ticker.trade_date_kst,
                                 ticker.trade_time_kst,
                                 ticker.trade_timestamp,
                                 ticker.opening_price,
                                 ticker.high_price,
                                 ticker.low_price,
                                 trade_price = ticker.trade_price < 1 ? ticker.trade_price.ToString("F8") :
                                       ticker.trade_price.ToString("F2"),
                                 ticker.prev_closing_price,
                                 ticker.change,
                                 ticker.change_price,
                                 ticker.change_rate,
                                 signed_change_price = ticker.trade_price < 1 ? ticker.signed_change_price.ToString("F8"):ticker.signed_change_price.ToString("F2"),
                                 signed_change_rate = (ticker.signed_change_rate).ToString("P2"),
                                 ticker.trade_volume,
                                 ticker.acc_trade_price,
                                 acc_trade_price_24h = market.market.StartsWith("KRW") ? (ticker.acc_trade_price_24h / 1_000_000).ToString("F2") + "M" :(ticker.acc_trade_price_24h).ToString("F2"),
                                 ticker.acc_trade_volume,
                                 ticker.acc_trade_volume_24h,
                                 ticker.highest_52_week_price,
                                 ticker.highest_52_week_date,
                                 ticker.lowest_52_week_price,
                                 ticker.lowest_52_week_date,
                                 ticker.timestamp
                             };

            return ConvertToDataTable(joinedData.ToList());
        }

        private DataTable FilterColumns(DataTable sourceTable, List<string> columns)
        {
            DataTable filteredTable = new DataTable();
            foreach (var column in columns)
            {
                filteredTable.Columns.Add(column, sourceTable.Columns[column].DataType);
            }

            foreach (DataRow row in sourceTable.Rows)
            {
                DataRow newRow = filteredTable.NewRow();
                foreach (var column in columns)
                {
                    newRow[column] = row[column];
                }
                filteredTable.Rows.Add(newRow);
            }

            return filteredTable;
        }
        private void UpdateDataGridView()
        {
            DataTable joinedDataTable = JoinMarketAndTickerData();
            DataTable final_dt = FilterColumns(joinedDataTable, selectedColumns);

            dataGridView1.DataSource = final_dt;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("bithumb");
            comboBox1.Items.Add("upbit");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fetchTimer.Stop();
            textBox1.Text = "로컬 파일에서 불러온 데이터";
            fGetData();
            UpdateDataGridView();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = site + "에서 불러온 데이터";
            UpdateDataGridView();
            fetchTimer.Start();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                m_url = "https://api.bithumb.com/v1/market/all?isDetails=false";
                t_url = "https://api.bithumb.com/v1/ticker";
                site = "bithumb";
            }
            else
            {
                m_url = "https://api.upbit.com/v1/market/all?isDetails=true";
                t_url = "https://api.upbit.com/v1/ticker";
                site = "upbit";
            }
        }
    }
}
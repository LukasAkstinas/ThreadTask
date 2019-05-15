using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Data.Sql;
using System.Configuration;
using System.Data.SqlClient;

namespace ThreadTask
{
    public partial class Form1 : Form
    {
        private Thread[] threads;
        private readonly string connectionString;


        public Form1()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["ThreadTask.Properties.Settings.Database1ConnectionString"].ConnectionString;
            btnStart.Enabled = false;
            btnStop.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            Stop();
            btnStart.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int num;
            btnStart.Enabled = int.TryParse(textBox1.Text, out num) && 2 <= num && num <= 15;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
        }

        private void Start()
        {
            listView1.Items.Clear();
            int threadCount = int.Parse(textBox1.Text);
            threads = new Thread[threadCount];
            for (int t = 0; t < threadCount; ++t)
            {
                int threadId = t + 1;
                threads[t] = new Thread(delegate () { DoWork(threadId); });
                threads[t].Start();
            }
        }

        private void Stop()
        {
            foreach (Thread t in threads)
            {
                if (t.ThreadState == ThreadState.WaitSleepJoin) t.Abort(); // only abort sleeping threads, let the rest finish
            }
        }

        private void DoWork(int threadId)
        {
            Random rng = new Random((DateTime.Now.Millisecond + threadId) * threadId);
            while (true)
            {
                Thread.Sleep(rng.Next(5, 20) * 100); // 500-2000ms
                string s = GetRandomString(rng);
                listView1.Invoke(new MethodInvoker( delegate ()
                {
                    if (listView1.Items.Count == 20) listView1.Items.RemoveAt(0);
                    listView1.Items.Add(new ListViewItem(new string[] { threadId.ToString(), s }));
                }));
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    SqlCommand com = new SqlCommand(string.Format("INSERT INTO ThreadData (ThreadID, Data) VALUES ({0},\'{1}\');", threadId, s), sqlCon);
                    com.ExecuteNonQuery();
                    sqlCon.Close();
                }
            }
        }

        private string GetRandomString(Random rng)
        {
            string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder sb = new StringBuilder();
            int length = rng.Next(5, 10);
            for (int i = 0; i < length; ++i)
            {
                sb.Append(charSet[rng.Next(charSet.Length)]);
            }
            return sb.ToString();
        }
    }
}

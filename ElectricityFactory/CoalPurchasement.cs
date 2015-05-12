using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ElectricityFactory
{
    public partial class CoalPurchasement : Form
    {
        private List<Vendor> _vendorList;
        private Planning _planning;

        public CoalPurchasement()
        {
            InitializeComponent();
            LoadPlanning();
            LoadVendorInfo();
        }

        private void LoadPlanning()
        {
            XmlSerializer xml = new XmlSerializer(typeof(Planning));
            using (FileStream fileStream = new FileStream("planning.config", FileMode.Open, FileAccess.Read))
            {
                XmlReader xmlReader = new XmlTextReader(fileStream);
                _planning = (Planning)xml.Deserialize(xmlReader);
            }

            numericUpDown_totalCoal.Value = Convert.ToDecimal(_planning.TotalCoalAmount);
            numericUpDown_totalMoney.Value = Convert.ToDecimal(_planning.AveragePrice);
            numericUpDown_qnet.Value = Convert.ToDecimal(_planning.AverageQnetThreshold);
            numericUpDown_vad.Value = Convert.ToDecimal(_planning.AverageVadThreshold);
            numericUpDown_sad.Value = Convert.ToDecimal(_planning.AverageSadThreshold);
        }

        private void LoadVendorInfo()
        {
            dataGridView2.EditMode = DataGridViewEditMode.EditOnKeystroke;

            _vendorList = new List<Vendor>();

            XmlSerializer xml = new XmlSerializer(typeof(List<Vendor>));
            using (FileStream fileStream = new FileStream("vendors.config", FileMode.Open, FileAccess.Read))
            {
                XmlReader xmlReader = new XmlTextReader(fileStream);
                _vendorList = (List<Vendor>)xml.Deserialize(xmlReader);
            }

            dataGridView2.DataSource = _vendorList;
            dataGridView2.Columns[0].HeaderText = "供应商名称";
            dataGridView2.Columns[1].HeaderText = "入厂标单（不含税）";
            dataGridView2.Columns[2].HeaderText = "发热量";
            dataGridView2.Columns[3].HeaderText = "挥发份";
            dataGridView2.Columns[4].HeaderText = "硫份";
            dataGridView2.Columns[5].HeaderText = "最小采购量(万吨)";
            dataGridView2.Columns[6].HeaderText = "最大采购量(万吨)";
            dataGridView2.Columns[7].HeaderText = "采购单位(万吨)";

            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            SavePlanning();

            PurchaseEngine engine = new PurchaseEngine(_planning.TotalCoalAmount, _vendorList);

            BackgroundWorker runEngine = new BackgroundWorker();
            runEngine.DoWork += runEngine_DoWork;

            var list = engine.Run(_planning.AveragePrice, _planning.AverageQnetThreshold, _planning.AverageVadThreshold, _planning.AverageSadThreshold);

            for (int i = 0; i < list.Count; i++)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = index;
                for (int j = 0; j < list[i].Count; j++)
                {
                    dataGridView1.Rows[index].Cells[j+1].Value = (list[i][j]).ToString("F");
                }
            }
        }

        void runEngine_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void SavePlanning()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof (Planning));
            using (FileStream fileStream = new FileStream("planning.config", FileMode.Create))
            {
                xmlSerializer.Serialize(fileStream, _planning);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _planning.TotalCoalAmount = Convert.ToSingle(numericUpDown_totalCoal.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            _planning.AveragePrice = Convert.ToSingle(numericUpDown_totalMoney.Value);
        }

        private void numericUpDown_qnet_ValueChanged(object sender, EventArgs e)
        {
            _planning.AverageQnetThreshold = Convert.ToSingle(numericUpDown_qnet.Value);
        }

        private void numericUpDown_vad_ValueChanged(object sender, EventArgs e)
        {
            _planning.AverageVadThreshold = Convert.ToSingle(numericUpDown_vad.Value);
        }

        private void numericUpDown_sad_ValueChanged(object sender, EventArgs e)
        {
            _planning.AverageSadThreshold = Convert.ToSingle(numericUpDown_sad.Value);
        }

        private void buttonVendorSave_Click(object sender, EventArgs e)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Vendor>));
            using (FileStream fileStream = new FileStream("vendors.config", FileMode.Create))
            {
                xmlSerializer.Serialize(fileStream, _vendorList);
            }
        }
    }
}

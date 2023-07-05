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
using NewspaperSellerModels;
using NewspaperSellerTesting;

namespace NewspaperSellerSimulation
{
    public partial class Form1 : Form
    {
        public SimulationSystem simulationSystem;// = new SimulationSystem();
        public Form1()
        {
            InitializeComponent();
        }

        String TestCasePath = "";
        bool CaseIsSimulated = false;
        List<decimal> DayProb = new List<decimal>();
        List<int> demandList = new List<int>();
        List<decimal> goodList = new List<decimal>();
        List<decimal> fairList = new List<decimal>();
        List<decimal> poorList = new List<decimal>();
        public void readFile(string path)
        {
            simulationSystem = new SimulationSystem();
           
            try
            {
                StreamReader sr = new StreamReader(path);
                string data = sr.ReadLine();

                while (data != null)
                {
                    if (data == "NumOfNewspapers")
                        simulationSystem.NumOfNewspapers = int.Parse(sr.ReadLine());
                    else if (data == "NumOfRecords")
                        simulationSystem.NumOfRecords = int.Parse(sr.ReadLine());
                    else if (data == "PurchasePrice")
                        simulationSystem.PurchasePrice = decimal.Parse(sr.ReadLine());
                    else if (data == "ScrapPrice")
                        simulationSystem.ScrapPrice = decimal.Parse(sr.ReadLine());
                    else if (data == "SellingPrice")
                        simulationSystem.SellingPrice = decimal.Parse(sr.ReadLine());
                    else if (data == "DayTypeDistributions")
                    {
                        string[] array = sr.ReadLine().Split(',');
                        for (int i = 0; i < 3; i++)
                            DayProb.Add(decimal.Parse(array[i]));
                        simulationSystem.fillTypeOfNewsDay(DayProb);
                    }
                    else if (data == "DemandDistributions")
                    {
                        int i = 0;
                        while (i < 7)
                        {
                            String[] prob = sr.ReadLine().Split(',');
                            demandList.Add(int.Parse(prob[0]));
                            goodList.Add(decimal.Parse(prob[1]));
                            fairList.Add(decimal.Parse(prob[2]));
                            poorList.Add(decimal.Parse(prob[3]));
                            i++;
                        }
                    }
                    data = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception x)
            {
                Console.WriteLine("Exception in reading form file");
            }
        }

        private void SimulateBtn_Click(object sender, EventArgs e)
        {
            if (TestCasePath == "")
            {
                CaseIsSimulated = false;
                MessageBox.Show("Please Select TestCase First", "Error");
            }
            else
            {
                readFile(TestCasePath);
                simulationSystem.fillDemandDistribution(demandList, goodList, fairList, poorList);
                simulationSystem.fillSimulationTable();
                simulationSystem.fillPerformanceMeasure();
                dataGridView1.DataSource = simulationSystem.SimulationTable;
                dataGridView1.Dock = DockStyle.Fill;
                Controls.Add(dataGridView1);
                string result = TestingManager.Test(simulationSystem, TestCaseName);
                MessageBox.Show(result);
                CaseIsSimulated = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e) {}

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {}

        private void performance_Click(object sender, EventArgs e)
        {
            if (CaseIsSimulated)
            {
                showPerformance performance = new showPerformance();
                performance.TLost = simulationSystem.PerformanceMeasures.TotalLostProfit;
                performance.TCost = simulationSystem.PerformanceMeasures.TotalCost;
                performance.TNetProfit = simulationSystem.PerformanceMeasures.TotalNetProfit;
                performance.TSales = simulationSystem.PerformanceMeasures.TotalSalesProfit;
                performance.TScrup = simulationSystem.PerformanceMeasures.TotalScrapProfit;
                performance.UnSold = simulationSystem.PerformanceMeasures.DaysWithUnsoldPapers;
                performance.ExDemand = simulationSystem.PerformanceMeasures.DaysWithMoreDemand;
                performance.Show();
            }
            else
                MessageBox.Show("Please Simulate case First", "Error");
        }
        private void ClearDataGridView()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Dock = DockStyle.Fill;
        }
        private void ClearBtn_Click(object sender, EventArgs e)
        {
            ClearDataGridView();
            combTestCase.SelectedIndex = -1;
            combTestCase.SelectedText = "";
            CaseIsSimulated = false;
        }
    }
}

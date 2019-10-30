using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        SimulationSystem System = new SimulationSystem();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.startsimulation("TestCase1.txt");
            dataGridView1.DataSource = System.SimulationTable;
            String testResult = TestingManager.Test(System, Constants.FileNames.TestCase1);
            MessageBox.Show(testResult);
        }

    }
}

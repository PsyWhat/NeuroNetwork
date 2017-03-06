using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EvolutionNetwork.Teacher;
using EvolutionNetwork.NeuroNet;

namespace EvolutionNetwork
{
    public partial class Form1 : Form
    {
        GenericTeacher<NeuroNetGenome,CNeuroNetWrapper> teacher;
        Random r;
        public Form1()
        {
            InitializeComponent();
            r = new Random();
        }

        

        private void generateNewBtn_Click(object sender, EventArgs e)
        {
            //try
            //{
                int popC = int.Parse(populationCount.Text);
                List<NeuroNetGenome> population = new List<NeuroNetGenome>();
                for (int i = 0;i<popC;++i)
                {
                    CNeuroNetWrapper net = new CNeuroNetWrapper(2, 1);
                    net.Connections.Add(new CNeuroNetWrapper.Connection(r.Next(0, 2), r.Next(1, 2), (r.NextDouble() - r.NextDouble()) * 2));
                    population.Add(new NeuroNetGenome(net));
                }
                _results = teacher.AddNewGeneration(population);
                IndividualSelector.Maximum = (Decimal)population.Count;
            //}
            /*catch (System.Exception ex)
            {
                MessageBox.Show("Input error.", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            status.Text = string.Format("Generating...");
            try
            {
                teacher = new GenericTeacher<NeuroNetGenome, CNeuroNetWrapper>(123456, double.Parse(smcTB.Text),
                    double.Parse(umcTB.Text),
                    double.Parse(structmcTB.Text),
                    double.Parse(complexitymcTB.Text), Tester.TestFunction);
                panel1.Visible = true;
                status.Text = string.Format("Generated...", teacher.CurrentGeneration);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Input error.", "Input error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        List<Tuple<double, NeuroNetGenome>> _results;
        List<NeuroNetGenome> _newGen;

        private void passGenerationBtn_Click(object sender, EventArgs e)
        {
            status.Text = string.Format("Passing gen:{0}", teacher.CurrentGeneration);
            _newGen = teacher.GetNewGenerationPopulation();
            _results = teacher.AddNewGeneration(_newGen);
            status.Text = string.Format("Gen:{0} have been passed", teacher.CurrentGeneration);
        }

        private void cpyGenerationBtn_Click(object sender, EventArgs e)
        {
            balanceTask2.SetAGenome(_results[(int)IndividualSelector.Value-1].Item2);
        }
    }
}

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
        GenericTeacher<NeuroNetGenome, CNeuroNetWrapper> teacher;
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
            //{ 
            status.Text = "Generating a new pop";
            int popC = int.Parse(populationCount.Text);
            List<NeuroNetGenome> population = new List<NeuroNetGenome>();
            for (int i = 0; i < popC; ++i)
            {
                CNeuroNetWrapper net = new CNeuroNetWrapper(3, 1);
                CNeuroNetWrapper.Connection c = new CNeuroNetWrapper.Connection(r.Next(net.InputsCount), r.Next(net.InputsCount,net.InputsCount+net.OutputsCount), (r.NextDouble() - r.NextDouble()) * 2);
                net.Connections.Add(c);
                population.Add(new NeuroNetGenome(net));
            }
            _results = teacher.AddNewGeneration(population);
            IndividualSelector.Maximum = (Decimal)population.Count;
            status.Text = "Generated a new pop";
            //}
            /*catch (System.Exception ex)
            {
                MessageBox.Show("Input error.", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                teacher = new GenericTeacher<NeuroNetGenome, CNeuroNetWrapper>(123456, double.Parse(smcTB.Text),
                    double.Parse(umcTB.Text),
                    double.Parse(structmcTB.Text),
                    double.Parse(complexitymcTB.Text), Tester.TestFunction);
                panel1.Visible = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Input error.", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        List<Tuple<double, NeuroNetGenome>> _results;
        List<NeuroNetGenome> _newGen;

        private void passGenerationBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < (int)generationsToPass.Value; ++i)
            {
                status.Text = string.Format("Passing gen:{0}", teacher.CurrentGeneration);
                _newGen = teacher.GetNewGenerationPopulation();
                _results = teacher.AddNewGeneration(_newGen);
                status.Text = string.Format("Gen:{0} have been passed", teacher.CurrentGeneration);
                label5.Text = string.Format("Worst is {1}, Best is {0}", _results[0].Item1, _results[_results.Count - 1].Item1);
                label5.Visible = true;
            }
        }

        private void cpyGenerationBtn_Click(object sender, EventArgs e)
        {
            Tuple<double, NeuroNetGenome> sg = _results[(int)IndividualSelector.Value - 1];
            balanceTask2.SetAGenome(sg.Item2);
            label5.Text = string.Format("Selected result is: {0}, Connections count: {1}, Nodes {2}", sg.Item1, sg.Item2.NeuroNet.Connections.Count, sg.Item2.NeuroNet.Nodes.Count);
            label5.Visible = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}

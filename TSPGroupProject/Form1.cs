using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TSP
{
    public partial class mainform : Form
    {
        private ProblemAndSolver CityData;
        public int saSource = 0;
        public int saMaxTemp = 10000;


        public mainform()
        {
            InitializeComponent();

            CityData = new ProblemAndSolver();
            this.tbSeed.Text = CityData.Seed.ToString();
        }

        /// <summary>
        /// overloaded to call the redraw method for CityData. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SetClip(new Rectangle(0,0,this.Width, this.Height - this.toolStrip1.Height-35));
            CityData.Draw(e.Graphics);
        }

        private void SetSeed()
        {
            if (Regex.IsMatch(this.tbSeed.Text, "^[0-9]+$"))
            {
                this.toolStrip1.Focus();
                CityData = new ProblemAndSolver(int.Parse(this.tbSeed.Text));
                this.Invalidate();
            }
            else
                MessageBox.Show("Seed must be an integer.");
        }

        private HardMode.Modes getMode()
        {
            return HardMode.getMode(cboMode.Text);
        }

        private int getProblemSize()
        {
            if (Regex.IsMatch(this.tbProblemSize.Text, "^[0-9]+$"))
            {
                return Int32.Parse(this.tbProblemSize.Text);
            }
            else
            {
                MessageBox.Show("Problem size must be an integer.");
                return 20;
            };
        }

        // not necessarily a new problem but resets the state using the specified settings
        private void reset()
        {
            this.SetSeed(); // also resets the CityData variable

            int size = getProblemSize();
            HardMode.Modes mode = getMode();

            CityData.GenerateProblem ( size, mode );
        }


#region GUI Event Handlers

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void tbSeed_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.reset();
//                this.SetSeed();
        }

#endregion // Event Handlers

        private void Form1_Load(object sender, EventArgs e)
        {
            // use the parameters in the GUI controls
            this.reset();
        }

        private void tbProblemSize_Leave(object sender, EventArgs e)
        {
            this.reset();
        }

        private void tbProblemSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.reset();
        }

        private void cboMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.reset();
        }

        private void newProblem_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch(this.tbProblemSize.Text, "^[0-9]+$"))
            {
                Random random = new Random();
                int seed = int.Parse(tbSeed.Text);
                
                this.reset();
                
                this.Invalidate(); 
            }
            else
            {
                MessageBox.Show("Problem size must be an integer.");
            };
        }

        private void randomProblem_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch(this.tbProblemSize.Text, "^[0-9]+$"))
            {
                Random random = new Random();
                int seed = random.Next(1000); // 3-digit random number
                this.tbSeed.Text = "" + seed;

                this.reset();

                this.Invalidate();
            }
            else
            {
                MessageBox.Show("Problem size must be an integer.");
            };
        }

        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.reset();

            CityData.solveProblem(Solver.DEFAULT);
        }

        private void greedyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Add a hook into your own implementation here.
            //throw new NotImplementedException();
            CityData.solveProblem(Solver.GREEDY);

        }

        private void bBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Add a hook into your own implementation here.
            //throw new NotImplementedException();
            
            CityData.solveProblem(Solver.BRANCH_BOUND);
            TimeSpan timeTaken = new TimeSpan(Node.timeElapsed);
            double timeEl = timeTaken.TotalMilliseconds * 10;
            tbElapsedTime.Text = timeEl.ToString("N7");
            toolStripTextBox1.Text = Node.numSolutions.ToString();
            toolStripTextBox_MaxNodes.Text = Node.maxNodesCreated.ToString();
            toolStripTextBox_Pruned.Text = Node.pruneCount.ToString();
            toolStripTextBox_StatesCreated.Text = Node.numStatesCreated.ToString();
        }

        private void randomToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // TODO: Add a hook into your own implementation here.
            //throw new NotImplementedException();

            CityData.solveProblem(Solver.RANDOM);
        }

        private void yourTSPToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // TODO: Add a hook into your own implementation here.
            //throw new NotImplementedException();
            CityData.solveProblem(Solver.SIMANNEAL);
        }

        private void AlgorithmMenu2_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            AlgorithmMenu2.Text = e.ClickedItem.Text;
            AlgorithmMenu2.Tag = e.ClickedItem;
        }

        private void AlgorithmMenu2_ButtonClick_1(object sender, EventArgs e)
        {
            if (AlgorithmMenu2.Tag != null)
            {
                (AlgorithmMenu2.Tag as ToolStripMenuItem).PerformClick();
            }
            else
            {
                AlgorithmMenu2.ShowDropDown();
            }
        }

        private void SAOptions_Click(object sender, EventArgs e)
        {
            SAOptions saOpt = new SAOptions();
            saOpt.trackBar_MaxTemp.Value = saMaxTemp / 10000;

            switch (saSource)
            {
                case 0:
                    saOpt.radioButton_Default.Checked = true;
                    break;

                case 1:
                    saOpt.radioButton_Random.Checked = true;
                    break;

                case 2:
                    saOpt.radioButton_Greedy.Checked = true;
                    break;

                case 3:
                    saOpt.radioButton_Branch.Checked = true;
                    break;
            }

            if (saOpt.ShowDialog() == DialogResult.OK)
            {
                saMaxTemp = saOpt.trackBar_MaxTemp.Value * 10000;

                if (saOpt.radioButton_Default.Checked)
                {
                    saSource = 0;
                }

                if (saOpt.radioButton_Random.Checked)
                {
                    saSource = 1;
                }

                if (saOpt.radioButton_Greedy.Checked)
                {
                    saSource = 2;
                }

                if (saOpt.radioButton_Branch.Checked)
                {
                    saSource = 3;
                }
            }
        }

        
    }
}
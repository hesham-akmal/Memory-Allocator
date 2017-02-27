using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryAllocation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SystemProg.instance.createBlocks(Convert.ToInt32(comboBox2.Text));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 4;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
                return;

            if (comboBox1.SelectedIndex == 0)
            {
                SystemProg.instance.addPfirstFit(Convert.ToInt32(textBox1.Text));
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                SystemProg.instance.addProcessFit(Convert.ToInt32(textBox1.Text) , true);
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                SystemProg.instance.addProcessFit(Convert.ToInt32(textBox1.Text), false);
            }

            textBox1.Text = "";

        }

        private void button3_Click(object sender, EventArgs e)
        {
           
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SystemProg.instance.randomSizes = true;
            }
            else if (!checkBox1.Checked)
                SystemProg.instance.randomSizes = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                SystemProg.instance.mergeHolesAuto = true;
                SystemProg.instance.mergeHoles();
            }
               else if (!checkBox2.Checked)
            {
                SystemProg.instance.mergeHolesAuto = false;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if(!comboBox3.Text.Equals(""))
            SystemProg.instance.deallocateProcess(SystemProg.instance.allProcesses[Convert.ToInt32(comboBox3.Text.ToString().Substring(8)) -1]);
        }
    }
}

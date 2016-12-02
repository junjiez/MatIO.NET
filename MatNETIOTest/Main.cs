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
using MatNETIO.io;
using MatNETIO.types;

namespace MatNETIOTest
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dRes = openFileDialog.ShowDialog();
            if (dRes == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                txtOutput.Text = txtOutput.Text + "Attempting to read the file '" + fileName + "'...";
                try
                {
                    MatFileReader mfr = new MatFileReader(fileName);
                    txtOutput.Text += "Done!\nMAT-file contains the following:\n";
                    txtOutput.Text += mfr.MatFileHeader.ToString() + "\n";

                    foreach (MLArray mla in mfr.Data)
                    {
                        txtOutput.Text = txtOutput.Text + mla.ContentToString() + "\n";
                    }
                }
                catch (IOException)
                {
                    txtOutput.Text = txtOutput1.Text + "Invalid MAT-file!\n";
                    MessageBox.Show("Invalid binary MAT-file! Please select a valid binary Mat-file.",
                        "Invalid Mat-file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }
    }
}

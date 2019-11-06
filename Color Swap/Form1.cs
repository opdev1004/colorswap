using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.IO;
using System.Drawing.Imaging;

namespace Color_Swap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Set form color based on RGB
            this.BackColor = Color.FromArgb(35, 39, 47);
            // Add default Path for saving
            textBox3.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Set Hex Color
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowDialog();
            Color col = colorDialog1.Color;
            String hex = col.R.ToString("X2") + col.G.ToString("X2") + col.B.ToString("X2");
            textBox1.Text = hex;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Set Hex Color
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowDialog();
            Color col = colorDialog1.Color;
            String hex = col.R.ToString("X2") + col.G.ToString("X2") + col.B.ToString("X2");
            textBox2.Text = hex;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Clear all items in list box
            listBox1.Items.Clear();
            MessageBox.Show("All items has been removed from the list.");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Get file path and add them to list box
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Images (*.BMP;*.PNG;*.JPG;) | *.BMP;*.PNG;*.JPG; | All files (*.*)|*.*";
            ofd.Title = "Open images";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (String file in ofd.FileNames)
                {
                    try
                    {
                        String fileName = System.IO.Path.GetFileName(file);
                        listBox1.Items.Add(file);
                    }
                    catch (SecurityException ex)
                    {
                        // The user lacks appropriate permissions to read files, discover paths, etc.
                        MessageBox.Show("Security error. Please check if this program has permission to do this task.\n\n" +
                            "Error message: " + ex.Message + "\n\n" +
                            "Details (send to Support):\n\n" + ex.StackTrace
                        );
                    }
                    catch (Exception ex)
                    {
                        // Could not load the image - probably related to Windows file system permissions.
                        MessageBox.Show("Cannot read the images: " + file.Substring(file.LastIndexOf('\\'))
                            + ". You may not have permission to read the file, or " +
                            "it may be corrupt.\n\nReported error: " + ex.Message);
                    }
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            int total = listBox1.SelectedItems.Count;
            String[] values = new String[total];
            int count = 0;
            foreach (String value in listBox1.SelectedItems)
            {
                values[count] = value;
                count++;
            }
            foreach (String value in values)
            {
                listBox1.Items.Remove(value);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int total = listBox1.Items.Count;
            int count = 1;
            double range = Convert.ToDouble(textBox4.Text);
            progressBar1.Maximum = total;
            Color oldCol = ColorTranslator.FromHtml("#" + textBox1.Text);
            Color newCol = ColorTranslator.FromHtml("#" + textBox2.Text);
            Tuple<double, double> r = GetColorRange(oldCol.R, range);
            Tuple<double, double> g = GetColorRange(oldCol.G, range);
            Tuple<double, double> b = GetColorRange(oldCol.B, range);
            double maxR = r.Item1;
            double minR = r.Item2;
            double maxG = g.Item1;
            double minG = g.Item2;
            double maxB = b.Item1;
            double minB = b.Item2;

            String path = textBox3.Text;
            double alpha = 255;
            Boolean transparency = checkBox1.Checked;
            if (transparency)
            {
                alpha = 0;
            }
            foreach (String value in listBox1.Items)
            {
                label4.Text = count.ToString() + "/" + total.ToString(); 
                String fileName = Path.GetFileNameWithoutExtension(value);
                Image<Rgba, byte> image = CvInvoke.Imread(value).ToImage<Rgba, byte>();
                // Get all the transparent part of image and set them same color
                Image<Gray, byte> transparentImage = image.InRange(new Rgba(0, 0, 0, 0), new Rgba(255, 255, 255, 0));
                Mat mat = image.Mat;
                mat.SetTo(new MCvScalar(newCol.R, newCol.G, newCol.B, alpha), transparentImage);
                mat.CopyTo(image);
                // Apply color replace and transparent color
                Image<Gray, byte> tmp = image.InRange(new Rgba(minR, minG, minB, 255), new Rgba(maxR, maxG, maxB, 255));
                mat = image.Mat;
                mat.SetTo(new MCvScalar(newCol.R, newCol.G, newCol.B, alpha), tmp);
                mat.CopyTo(image);
                image.Save(path + "\\" + fileName + ".png");
                progressBar1.Increment(1);
                count++;
            }
            label4.Text = total.ToString() + "/" + total.ToString() + ", all done!";
        }

        private Tuple<double, double> GetColorRange(double value, double range)
        {
            double result1 = value + range;
            double result2 = value - range;
            double leftOVer1 = 0;
            double leftOVer2 = 0;
            if (result1 > 255)
            {
                leftOVer1 = result1 - 255;
                result1 = 255;
                result2 = result2 + leftOVer1;
            }
            if (result2 < 0)
            {
                leftOVer2 = 0 - result2;
                result2 = 0;
                result1 = result1 + leftOVer2;
            }
            return Tuple.Create<double, double>(result1, result2);
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox3.Text = fbd.SelectedPath;
            }

        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int value = hScrollBar1.Value;
            if (value > 127) {
                value = 127;
            }
            textBox4.Text = value.ToString();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }

}

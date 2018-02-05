using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using rtChart;

namespace ReadSerialPort_S
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        kayChart serialDataChart;

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.BackColor = Color.FromArgb(32,32,32);
            listBox1.ForeColor = Color.FromArgb(0 ,255, 0);
            listBox2.BackColor = Color.FromArgb(32, 32, 32);
            listBox2.ForeColor = Color.FromArgb(0, 255, 0);
            // listBox1.Enabled = false;
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            serialDataChart = new kayChart(chart1,60);
            serialDataChart.serieName = "SerialReadData";
            serialDataChart.TriggeredUpdate(300);
           


        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("Boud Rate veya Port Name Seçilmedi...");
            }
            else
            {
                String portName = comboBox1.SelectedItem.ToString();
                String portBoudRate = comboBox2.SelectedItem.ToString();
                serialPort1.PortName = portName;
                serialPort1.BaudRate = int.Parse(portBoudRate);
                
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_dataReceived);
                try
                {
                    serialPort1.Open();
                }
                catch
                {
                    MessageBox.Show("Error.. reading data from Serial Port ... ");
                }
            }

            button1.Enabled = false;

            
        }

        private void serialPort1_dataReceived(object sender, SerialDataReceivedEventArgs e)
        {      
            
                String data = serialPort1.ReadLine();
                
                double parseData;
                bool result = Double.TryParse(data, out parseData);
                if (result)
                {
                    serialDataChart.TriggeredUpdate(parseData);

                }
            this.BeginInvoke(new LineReceivedEvent(LineReceived), data);



        }
        private delegate void LineReceivedEvent(string line);
        private void LineReceived(string line)
        {

            // label1.Text = line;
            listBox1.Items.Add(line);
            listBox1.TopIndex = listBox1.Items.Count - 1;
            if (fileName != null)
            {
                using (StreamWriter writetext = File.AppendText(fileName+".txt"))
                {
                    writetext.Write(line);
                }
            }
            else
            {
                using (StreamWriter writetext = File.AppendText("log.txt"))
                {
                    writetext.Write(line);
                }

            }
            

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                          
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                button1.Enabled = true;
            }
        }
        String fileName;
        
        private void button3_Click(object sender, EventArgs e)
        {
            fileName = textBox1.Text;
            for (int i = 0; i < 10; i++)
            {
                listBox1.Items.Add(i+"");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("FSMVU...");
            this.Close();
        }

       

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                string str = textBox2.Text;
                listBox2.Items.Add(str);
                listBox2.TopIndex = listBox2.Items.Count - 1;
                textBox2.Text = "";
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine(textBox2.Text);
        }
    }
}

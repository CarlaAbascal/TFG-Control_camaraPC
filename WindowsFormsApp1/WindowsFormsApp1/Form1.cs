using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using csDronLink;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Dron miDron = new Dron();

        // STREAMING
        private VideoCapture capPC;
        private VideoCapture capDron;
        private bool running = false;

        // TCP SERVER
        private TcpListener listener;
        private bool serverRunning = false;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            // 🔹 Aquí llamamos manualmente al método de carga
            Form1_Load(this, EventArgs.Empty);
        }

        // ==========================
        //     INICIALIZACIÓN
        // ==========================
        private void Form1_Load(object sender, EventArgs e)
        {
            IniciarServidorTCP();
        }

        // ==========================
        //     TELEMETRÍA
        // ==========================
        private void ProcesarTelemetria(byte id, List<(string nombre, float valor)> telemetria)
        {
            foreach (var t in telemetria)
            {
                if (t.nombre == "Alt")
                {
                    altLbl.Text = t.valor.ToString();
                    break;
                }
            }
        }

        // ==========================
        //     BOTONES MANUALES
        // ==========================
        private void button1_Click_1(object sender, EventArgs e)
        {
            miDron.Conectar("simulacion");
            miDron.EnviarDatosTelemetria(ProcesarTelemetria);
        }

        private void EnAire(byte id, object param)
        {
            button2.BackColor = Color.Green;
            button2.ForeColor = Color.White;
            button2.Text = (string)param;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            miDron.Despegar(20, bloquear: false, f: EnAire, param: "Volando");
            button2.BackColor = Color.Yellow;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            miDron.Aterrizar(bloquear: false);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            capPC = new VideoCapture(0);
            // capDron = new VideoCapture("tcp://127.0.0.1:5760/live");

            running = true;

            Task.Run(() =>
            {
                Mat matPC = new Mat();
                Mat matDron = new Mat();

                while (running)
                {
                    if (capPC.Read(matPC) && !matPC.Empty())
                    {
                        pictureBoxPC.Image?.Dispose();
                        pictureBoxPC.Image = BitmapConverter.ToBitmap(matPC);
                    }

                   /* if (capDron != null && capDron.Read(matDron) && !matDron.Empty())
                    {
                        pictureBoxDron.Image?.Dispose();
                        pictureBoxDron.Image = BitmapConverter.ToBitmap(matDron);
                    }*/
                }
            });
        }

        /* private void button5_Click(object sender, EventArgs e)
         {
             miDron.CambiarHeading(90, bloquear: false);
         }

         private void button6_Click(object sender, EventArgs e)
         {
             miDron.CambiarHeading(270, bloquear: false);
         }

         private void button7_Click(object sender, EventArgs e)
         {
             miDron.Mover("Forward", 10, bloquear: false);
         }
        */

        // ==========================
        //     TCP SERVER GESTOS
        // ==========================
        private void IniciarServidorTCP()
        {
            Task.Run(() =>
            {
                try
                {
                    int puerto = 5005;
                    listener = new TcpListener(IPAddress.Parse("127.0.0.1"), puerto);
                    listener.Start();
                    serverRunning = true;

                    listBox1.Items.Add($"Servidor TCP iniciado en puerto {puerto}...");

                    while (serverRunning)
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        listBox1.Items.Add("Cliente conectado desde Python.");

                        NetworkStream stream = client.GetStream();
                        byte[] buffer = new byte[1024];

                        while (client.Connected)
                        {
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0) break;

                            string mensaje = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                            listBox1.Items.Add($"Gesto recibido: {mensaje}");
                            EjecutarAccionPorGesto(mensaje);
                        }

                        client.Close();
                        listBox1.Items.Add("Cliente desconectado.");
                    }
                }
                catch (Exception ex)
                {
                    listBox1.Items.Add($"Error en servidor TCP: {ex.Message}");
                }
            });
        }

        // ==========================
        //     ACCIONES POR GESTO
        // ==========================
        private void EjecutarAccionPorGesto(string gesto)
        {
            switch (gesto.ToLower())
            {
                case "palm":
                    miDron.Despegar(20, bloquear: false, f: EnAire, param: "Volando");
                    break;

                case "puño":
                    miDron.Aterrizar(bloquear: false);
                    break;

                case "uno":
                    miDron.Mover("Forward", 10, bloquear: false);
                    break;

                case "dos":
                    miDron.CambiarHeading(90, bloquear: false);
                    break;

                case "tres":
                    miDron.CambiarHeading(270, bloquear: false);
                    break;

                default:
                    listBox1.Items.Add($"Gesto no reconocido: {gesto}");
                    break;
            }
        }

        // ==========================
        //     FORM CLOSING
        // ==========================
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            running = false;
            serverRunning = false;
            capPC?.Release();
            capDron?.Release();
            listener?.Stop();
            base.OnFormClosing(e);
        }
    }
}

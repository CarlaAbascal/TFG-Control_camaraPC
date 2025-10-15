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
using System.IO;
using System.Linq;


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
            IniciarServidorTCP();   //Gestos
            IniciarServidorVideo();   //Video
            IniciarScriptPython();  // Ejecutar el script Python

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

        private bool sistemaActivo = false;

        private void button4_Click(object sender, EventArgs e)
        {
            if (serverRunning || videoServerRunning)
            {
                listBox1.Items.Add("⚠️ Los servidores ya están activos.");
                return;
            }

            if (!sistemaActivo)
            {
                listBox1.Items.Add("Iniciando detección de gestos y vídeo...");
                IniciarServidorTCP();
                IniciarServidorVideo();
                IniciarScriptPython();
                sistemaActivo = true;
                button4.Text = "Detener";
            }
            else
            {
                listBox1.Items.Add("Deteniendo detección...");
                running = false;
                serverRunning = false;
                videoServerRunning = false;

                listener?.Stop();
                videoListener?.Stop();

                if (pythonProcess != null && !pythonProcess.HasExited)
                    pythonProcess.Kill();

                sistemaActivo = false;
                button4.Text = "Iniciar vídeo";
            }
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

                        StringBuilder sb = new StringBuilder();

                        while (client.Connected)
                        {
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0) break;

                            string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            sb.Append(data);

                            // Procesar cada línea completa (cada gesto termina con '\n')
                            while (sb.ToString().Contains("\n"))
                            {
                                string line = sb.ToString();
                                int index = line.IndexOf('\n');
                                string mensaje = line.Substring(0, index).Trim();
                                sb.Remove(0, index + 1);

                                if (!string.IsNullOrWhiteSpace(mensaje))
                                {
                                    listBox1.Items.Add($"Gesto recibido: {mensaje}");
                                    EjecutarAccionPorGesto(mensaje);
                                }
                            }
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
        //     ARRANCAR SCRIPT PYTHON
        // ==========================
        private System.Diagnostics.Process pythonProcess;

        private void IniciarScriptPython()
        {
            try
            {
                string pythonExe = "python";
                string scriptPath = System.IO.Path.Combine(Application.StartupPath, "detectar_mano.py");

                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = pythonExe,
                    Arguments = $"\"{scriptPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                pythonProcess = new System.Diagnostics.Process { StartInfo = psi };
                pythonProcess.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        listBox1.Items.Add($"[Python]: {e.Data}");
                };
                pythonProcess.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        listBox1.Items.Add($"[Python ERROR]: {e.Data}");
                };

                pythonProcess.Start();
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();

                listBox1.Items.Add("Script Python iniciado correctamente.");
            }
            catch (Exception ex)
            {
                listBox1.Items.Add($"Error al iniciar script Python: {ex.Message}");
            }
        }
        // ==========================
        //     VIDEO DESDE PYTHON
        // ==========================
        private TcpListener videoListener;
        private bool videoServerRunning = false;

        private void IniciarServidorVideo()
        {
            Task.Run(() =>
            {
                try
                {
                    int puerto = 5006; // debe coincidir con el del script Python
                    videoListener = new TcpListener(IPAddress.Parse("127.0.0.1"), puerto);
                    videoListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    videoListener.Start();
                    videoServerRunning = true;
                    listBox1.Items.Add($"Servidor de video iniciado en puerto {puerto}...");

                    TcpClient client = videoListener.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    byte[] lengthBuffer = new byte[4];

                    while (videoServerRunning)
                    {
                        // Leer los 4 bytes del tamaño
                        int bytesRead = stream.Read(lengthBuffer, 0, 4);
                        if (bytesRead == 0) break;

                        int length = BitConverter.ToInt32(lengthBuffer.Reverse().ToArray(), 0);
                        byte[] imageBuffer = new byte[length];

                        int totalBytes = 0;
                        while (totalBytes < length)
                        {
                            int read = stream.Read(imageBuffer, totalBytes, length - totalBytes);
                            if (read == 0) break;
                            totalBytes += read;
                        }

                        using (var ms = new MemoryStream(imageBuffer))
                        {
                            var bmp = new Bitmap(ms);
                            pictureBoxPC.Invoke(new Action(() =>
                            {
                                pictureBoxPC.Image?.Dispose();
                                pictureBoxPC.Image = new Bitmap(bmp);
                            }));
                        }
                    }

                    client.Close();
                }
                catch (Exception ex)
                {
                    listBox1.Items.Add($"Error en servidor de video: {ex.Message}");
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

            if (pythonProcess != null && !pythonProcess.HasExited)
                pythonProcess.Kill();  // ✅ Cierra el script al salir

            base.OnFormClosing(e);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
 }

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        // Variables para streaming
        private VideoCapture capPC;
        private VideoCapture capDron;
        private bool running = false;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // --------------------------TELEMETRÍA--------------------------
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

        // --------------------------STREAMING--------------------------
        private void button4_Click(object sender, EventArgs e)
        {
            // Abrir cámara del PC
            capPC = new VideoCapture(0);

            // Abrir streaming del dron
            capDron = new VideoCapture("tcp://127.0.0.1:57600/live");

            running = true;

            Task.Run(() =>
            {
                Mat matPC = new Mat();
                Mat matDron = new Mat();

                while (running)
                {
                    // Frame de la cámara del PC
                    if (capPC.Read(matPC) && !matPC.Empty())
                    {
                        string gesture = DetectGesture(matPC);
                        if (!string.IsNullOrEmpty(gesture))
                        {
                            // Usamos OpenCvSharp.Point explícitamente
                            Cv2.PutText(matPC, $"Gesto: {gesture}", new OpenCvSharp.Point(30, 30),
                                HersheyFonts.HersheySimplex, 1.0, Scalar.Red, 2);

                            ExecuteGestureAction(gesture);
                        }

                        pictureBoxPC.Image?.Dispose();
                        pictureBoxPC.Image = BitmapConverter.ToBitmap(matPC);
                    }

                    // Frame del dron
                    if (capDron.Read(matDron) && !matDron.Empty())
                    {
                        pictureBoxDron.Image?.Dispose();
                        pictureBoxDron.Image = BitmapConverter.ToBitmap(matDron);
                    }
                }
            });
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            running = false;
            capPC?.Release();
            capDron?.Release();
            base.OnFormClosing(e);
        }

        // --------------------------DETECCIÓN DE GESTOS--------------------------
        private string DetectGesture(Mat frame)
        {
            if (frame.Empty()) return null;

            Mat hsv = new Mat();
            Cv2.CvtColor(frame, hsv, ColorConversionCodes.BGR2HSV);

            // Color piel (ajusta si no detecta bien)
            Scalar lower = new Scalar(0, 30, 60);
            Scalar upper = new Scalar(20, 150, 255);
            Mat mask = new Mat();
            Cv2.InRange(hsv, lower, upper, mask);

            Cv2.GaussianBlur(mask, mask, new OpenCvSharp.Size(5, 5), 0);
            Cv2.Threshold(mask, mask, 127, 255, ThresholdTypes.Binary);

            Cv2.FindContours(mask, out OpenCvSharp.Point[][] contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            if (contours.Length == 0) return null;

            var cnt = contours.OrderByDescending(c => Cv2.ContourArea(c)).First();
            if (Cv2.ContourArea(cnt) < 2000) return null;

            int[] hull = Cv2.ConvexHullIndices(cnt);

            if (hull.Length > 3)
            {
                var defects = Cv2.ConvexityDefects(cnt, hull);
                int fingers = 0;
                foreach (var defect in defects)
                {
                    OpenCvSharp.Point start = cnt[defect.Item0];
                    OpenCvSharp.Point end = cnt[defect.Item1];
                    OpenCvSharp.Point far = cnt[defect.Item2];

                    double a = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
                    double b = Math.Sqrt(Math.Pow(far.X - start.X, 2) + Math.Pow(far.Y - start.Y, 2));
                    double c = Math.Sqrt(Math.Pow(end.X - far.X, 2) + Math.Pow(end.Y - far.Y, 2));
                    double angle = Math.Acos((b * b + c * c - a * a) / (2 * b * c));

                    if (angle <= Math.PI / 2)
                        fingers++;
                }

                if (fingers == 0) return "fist";      // puño -> aterrizar
                if (fingers == 1) return "one";       // un dedo -> avanzar
                if (fingers == 2) return "two";       // dos dedos -> derecha
                if (fingers == 3) return "three";     // tres dedos -> izquierda
                if (fingers >= 4) return "palm";      // palma abierta -> despegr
            }

            return null;
        }

        // --------------------------ACCIONES DEL DRON POR GESTO--------------------------
        private void ExecuteGestureAction(string gesture)
        {
            switch (gesture)
            {
                case "palm": // mano abierta → despegar
                    miDron.Despegar(20, bloquear: false, f: EnAire, param: "Volando");
                    break;

                case "fist": // puño → aterrizar
                    miDron.Aterrizar(bloquear: false);
                    break;

                case "two": // dos dedos → girar derecha
                    miDron.CambiarHeading(90, bloquear: false);
                    break;

                case "three": // tres dedos → girar izquierda
                    miDron.CambiarHeading(270, bloquear: false);
                    break;

                case "one": // un dedo → avanzar
                    miDron.Mover("Forward", 10, bloquear: false);
                    break;
            }
        }
    }
}


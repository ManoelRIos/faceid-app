using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using ComponentFactory.Krypton.Toolkit;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using Emgu.CV.CvEnum;
namespace Marerial_design_elements
{
    public partial class Form1 : KryptonForm
    {
        #region variables
        private Capture videoCapture = null ;
        private Image<Bgr, Byte> currentFrame = null;
        Mat frame = new Mat();
        private bool facesDetectionEnabled = false;

        CascadeClassifier faceCascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt.xml");

        #endregion

        public Form1()
        {
            InitializeComponent();
                                 
                   
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            videoCapture = new Capture("videoplayback.mp4");
            videoCapture.ImageGrabbed += ProcessFrame;
            videoCapture.Start();


        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            //1. Capturar vídeo
            videoCapture.Retrieve(frame, 0);
            currentFrame = frame.ToImage<Bgr, Byte>().Resize
                (
                picCapture.Width, picCapture.Height, Inter.Cubic
                );

            //2. Detectar face
            if (facesDetectionEnabled)
            {
                //Converte imagem Bgr para imagem cinza
                Mat grayImage = new Mat();
                CvInvoke.CvtColor(currentFrame, grayImage, ColorConversion.Bgr2Gray);

                //Realça a imagem para um melhor resultado
                CvInvoke.EqualizeHist(grayImage, grayImage);

                Rectangle[] faces = faceCascadeClassifier.DetectMultiScale
                    (
                    grayImage, 1.1, 3, Size.Empty, Size.Empty
                    );
                //Se detectou o rosto
                if(faces.Length > 0)
                {
                    foreach (var face in faces)
                    {
                        //Desenha retangulo no rosto detectado
                        CvInvoke.Rectangle
                            (
                            currentFrame, face, new Bgr(Color.Red).MCvScalar, 2
                            );


                    }
                }

            }

            //renderizar a captura do vídeo dentro da Picture box picCapture
            picCapture.Image = currentFrame.Bitmap;
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            facesDetectionEnabled = true;

        }

    }
}

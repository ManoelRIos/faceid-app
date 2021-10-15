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
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Marerial_design_elements
{
    public partial class Form1 : KryptonForm
    {
        #region variables
        int testid = 0; 
        private Capture videoCapture = null ;
        private Image<Bgr, Byte> currentFrame = null;
        Mat frame = new Mat();
        private bool facesDetectionEnabled = false;
        CascadeClassifier faceCascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt.xml");
        Image<Bgr, Byte> faceResult = null;
        List<Image<Gray, Byte>> TrainedFaces = new List<Image<Gray, byte>>();
        List<int> PersonLabes = new List<int>();
        bool EnabledSaveImage = false;
        private static bool isTrainded = false;
        EigenFaceRecognizer recognizer;
        List<string> PersonNames = new List<string>();
        #endregion

        public Form1()
        {
            InitializeComponent();                                                    
        }

        
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (videoCapture != null) videoCapture.Dispose();
            videoCapture = new Capture();
            videoCapture.ImageGrabbed += ProcessFrame;
            videoCapture.Start();
            //facesDetectionEnabled = true;

        }

        private void ProcessFrame(object sender, EventArgs e)
        {

            //1. Capturar vídeo
            if(videoCapture != null && videoCapture.Ptr != IntPtr.Zero)
            {
                videoCapture.Retrieve(frame, 0);
                currentFrame = frame.ToImage<Bgr, Byte>().Resize(loginPicture.Width, loginPicture.Height, Inter.Cubic);
                //2. Detectar face
                if (facesDetectionEnabled)
                {
                    //Converte imagem Bgr para imagem cinza
                    Mat grayImage = new Mat();
                    CvInvoke.CvtColor(currentFrame, grayImage, ColorConversion.Bgr2Gray);
                                  
                    //Realça a imagem para um melhor resultado
                    CvInvoke.EqualizeHist(grayImage, grayImage);
                    Rectangle[] faces = faceCascadeClassifier.DetectMultiScale(
                        grayImage, 1.1, 3, Size.Empty, Size.Empty);
                                             
                    //Se detectou o rosto
                    if(faces.Length > 0)
                    {
                        foreach (var face in faces)
                        {
                            //Desenha retangulo no rosto detectado
                            //CvInvoke.Rectangle(currentFrame, face, new Bgr(Color.Red).MCvScalar, 2);
                            //3. Adicionar pessoa
                            //Mostrar a face no picture box para adicionar
                            Image<Bgr, Byte> resultImage = currentFrame.Convert<Bgr, Byte>();
                            resultImage.ROI = face;
                            loginPicture.SizeMode = PictureBoxSizeMode.AutoSize;
                            loginPicture.Image = resultImage.Bitmap;
                            if (EnabledSaveImage)
                            {
                                //Criar diretório se não existir 
                                string path = Directory.GetCurrentDirectory() + @"\BancoDeDados";
                                if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                                //Salvar 10 imagens com delay de um segundo para cada imagem
                                Task.Factory.StartNew(() => 
                                { 
                                    for (int i = 0; i < 10; i++)
                                    {
                                        //redimensiona a imagem e salva
                                        resultImage.Resize(200, 200, Inter.Cubic)
                                        .Save(path + @"\" + textName.Text + "_" + DateTime.Now.ToString("dd-mm-yyyy-hh-mm-ss")+".jpg");
                                        Thread.Sleep(1000);
                                    }
                                });
                            }
                            EnabledSaveImage = false;
                            if (btnSignup.InvokeRequired)
                            {
                                btnSignup.Invoke(new ThreadStart(delegate
                                {
                                    btnSignup.Enabled = true;
                                }));
                            }
                            //5. Reconhecer a face
                            if (isTrainded)
                            {
                                Image<Gray, Byte> grayFaceResult = 
                                    resultImage.Convert<Gray, Byte>().Resize(200,200,Inter.Cubic);
                                CvInvoke.EqualizeHist(grayFaceResult, grayFaceResult);                          
                                var result = recognizer.Predict(grayFaceResult);
                                //pictureBox2.Image = grayFaceResult.Bitmap;
                                //pictureBox3.Image = TrainedFaces[result.Label].Bitmap;
                                Debug.WriteLine(result.Label + ". " + result.Distance);
                                if (result.Label != -1 && result.Distance < 2000)
                                {
                                    CvInvoke.PutText(currentFrame, PersonNames[result.Label],
                                        new Point(face.X - 2, face.Y - 2),
                                        FontFace.HersheyComplex, 1.0, new Bgr(Color.Orange).MCvScalar);
                                }
                                else
                                {
                                    CvInvoke.PutText(currentFrame, "unknow", new Point(face.X - 2, face.Y - 2),
                                    FontFace.HersheyComplex, 1.0, new Bgr(Color.Orange).MCvScalar);                            
                                }
                            }
                        }
                    }
                }
                //renderizar a captura do vídeo dentro da Picture box picCapture
                loginPicture.Image = currentFrame.Bitmap;
            }
            if(currentFrame != null)currentFrame.Dispose();                    
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            //btnSave.Enabled = true;
            //btnAddPerson.Enabled = false;
            EnabledSaveImage = true;
        }
    
        private void btnTrain_Click(object sender, EventArgs e)
        {
           TrainImageFromDir();
        }

        private bool TrainImageFromDir()
        {
           int imagesCount = 0;
           double Threshold = 2000;
           TrainedFaces.Clear();
           PersonLabes.Clear();
           PersonNames.Clear();
           try
           {
               string path = Directory.GetCurrentDirectory() + @"\BancoDeDados";
               string[] files = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);

               foreach(var file in files)
               {
                   Image<Gray, Byte> traindImage = new Image<Gray, byte>(file);
                   TrainedFaces.Add(traindImage);
                   PersonLabes.Add(imagesCount);
                   imagesCount++;
               }

               EigenFaceRecognizer recognizer = new EigenFaceRecognizer(imagesCount, Threshold);
               recognizer.Train(TrainedFaces.ToArray(), PersonLabes.ToArray());

               isTrainded = true;
               Debug.WriteLine(imagesCount);
               Debug.WriteLine(isTrainded);                

               return true;
           }
           catch(Exception ex)
           {
               isTrainded = false;
               MessageBox.Show("Não foi possível localizar pessoa:" + ex.Message);
               return false;
           }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
           /*loginForm login = new loginForm();
           login.TopLevel = false;
           login.Visible = true;
           Controls.Add(login);*/
        }

     
    }
}

        
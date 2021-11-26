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
    public partial class FaceRecogniton : KryptonForm
    {
        #region variables
        private static DadosForm nivelForm = new DadosForm();
        private static bool nivel1 = true;
        private static int id = 0;
        private Capture videoCapture = null;
        private Image<Bgr, Byte> currentFrame = null;
        Mat frame = new Mat();
        bool faceDetection = false;
        bool loadShow = true;
        private bool facesDetectionEnabled = false;
        CascadeClassifier faceCascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt.xml");
        Image<Bgr, Byte> faceResult = null;
        List<Image<Gray, Byte>> TrainedFaces = new List<Image<Gray, byte>>();
        List<int> PersonLabes = new List<int>();
        bool EnableSaveImage = false;
        private static bool isTrained = false;
        EigenFaceRecognizer recognizer;
        
        List<string> PersonNames = new List<string>();
        #endregion

        public FaceRecogniton()
        {
            InitializeComponent();                                                    
        }

        private void btnOpenCam_Click(object sender, EventArgs e)
        {

            facesDetectionEnabled = true;
            btnOfCam.Visible = true;
            camState.Text = "Deligar Webcam";

            if (videoCapture != null) videoCapture.Dispose();

            try 
            {
                videoCapture = new Capture();
                Application.Idle += ProcessFrame;
            }
            catch
            {
                MessageBox.Show("Não foi possível identificar uma WebCam");
            }
            
        }

        private void btnOfCam_Click(object sender, EventArgs e)
        {
            facesDetectionEnabled = false;
            btnOfCam.Visible = false;
            btnOpenCam.Visible = true;
            camState.Text = "Ligar Webcam";
            if (videoCapture != null) videoCapture.Dispose();

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            if (!faceDetection)
            {
                MessageBox.Show("Não foi detectado um rosto");
            }

            else
            {
                
                TrainImageFromDir();
                
            }
            
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {

            if (!faceDetection)
            {
                MessageBox.Show("Não foi detectado um rosto");
            }
            else
            {
                LoadingSignUp load = new LoadingSignUp();
                
                if (loadShow)
                {
                    load.Show();
                    EnableSaveImage = true;
                    
                }                
            }                     
        }

        private void ProcessFrame(object sender, EventArgs e)
        {

            //1. Capturar vídeo
            if(videoCapture != null && videoCapture.Ptr != IntPtr.Zero)
            {
                videoCapture.Retrieve(frame, 1);
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
                        faceDetection = true;
                        foreach (var face in faces)
                        {
                            //Desenha retangulo no rosto detectado
                            CvInvoke.Rectangle(currentFrame, face, new Bgr(Color.Blue).MCvScalar, 2);

                            //3. Adicionar pessoa
                            //Mostrar a face no picture box para adicionar
                            Image<Bgr, Byte> resultImage = currentFrame.Convert<Bgr, Byte>();
                            resultImage.ROI = face;
                            loginPicture.SizeMode = PictureBoxSizeMode.StretchImage;
                            loginPicture.Image = resultImage.Bitmap;

                            if (EnableSaveImage)
                            {
                               


                                //Criar diretório se não existir  
                                string path = Directory.GetCurrentDirectory() + @"\DB_APP";

                                if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                                //Salvar 10 imagens com delay de um segundo para cada imagem
                                
                                Task.Factory.StartNew(() => 
                                { 
                                    for (int i = 0; i < 10; i++)
                                    {
                                        if (nivel1) id = 1;                                       
                                        //redimensiona a imagem e salva
                                        resultImage.Resize(200, 200, Inter.Cubic).Save(path + @"\" + id + '_' + textName.Text + "_" + DateTime.Now.ToString("dd-mm-yyyy-hh-mm-ss")+".jpg");
                                        Thread.Sleep(1000);
                                    }
                                });
                                
                            }                                                        
                            
                            EnableSaveImage = false;

                            if (btnSignup.InvokeRequired)
                            {
                                btnSignup.Invoke(new ThreadStart(delegate
                                {
                                    btnSignup.Enabled = true;
                                }));
                            }
                            //5. Reconhecer a face
                            if (isTrained)
                            {

                                Image<Gray, Byte> grayFaceResult = 
                                    resultImage.Convert<Gray, Byte>().Resize(200,200,Inter.Cubic);
                                CvInvoke.EqualizeHist(grayFaceResult, grayFaceResult);                                                                                                  
                                var result = recognizer.Predict(grayFaceResult);                               
                                Debug.WriteLine(result.Label + ". " + result.Distance);

                                if (result.Label != -1 && result.Distance < 2000)
                                {                                    
                                    NivelUser(id);
                                    CvInvoke.PutText(currentFrame, PersonNames[result.Label],
                                        new Point(face.X - 2, face.Y - 2),
                                        FontFace.HersheyComplex, 1.0, new Bgr(Color.Red).MCvScalar);
                                    CvInvoke.Rectangle(currentFrame, face, new Bgr(Color.Green).MCvScalar, 2);
                                    result.Label = -1; result.Distance = 2000;
                                    
                                }
                                else
                                {                                    
                                    CvInvoke.PutText(currentFrame, "Desconhecido", new Point(face.X - 2, face.Y - 2),
                                    FontFace.HersheyComplex, 1.0, new Bgr(Color.Orange).MCvScalar);                            
                                }
                            }
                        }
                    }
                    else
                    {
                        faceDetection = false;
                    }

                   
                }
                //renderizar a captura do vídeo dentro da Picture box picCapture
                loginPicture.Image = currentFrame.Bitmap;
            }
            if(currentFrame != null)currentFrame.Dispose();                    
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
               string path = Directory.GetCurrentDirectory() + @"\DB_APP";
               string[] files = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);

               foreach(var file in files)
               {
                    Image<Gray, Byte> trainedImage = new Image<Gray, byte>(file);
                    TrainedFaces.Add(trainedImage);
                    PersonLabes.Add(imagesCount);
                    string name = file.Split('\\').Last().Split('_')[0];
                    id = Convert.ToInt32(name);
                    PersonNames.Add(name);
                    imagesCount++;
                    Debug.WriteLine(imagesCount + ". " + name);                  

               }


               if(TrainedFaces.Count() > 0)
               {                   
                    recognizer = new EigenFaceRecognizer(imagesCount, Threshold);
                    recognizer.Train(TrainedFaces.ToArray(), PersonLabes.ToArray());
                    isTrained = true;
                    return true;
               }
               else
               {
                    isTrained = false;
                    MessageBox.Show("Não foi possível localizar pessoa no Banco de Dados");
                    return false;
               }        
                              

               
           }
           catch(Exception ex)
           {
               isTrained = false; 
               MessageBox.Show("Não foi possível localizar pessoa no Banco de Dados" + ex.Message);
               return false;
           }
        }

        private static void NivelUser(int idNivel)
        {
            if (idNivel == 1)
            {
                
                FaceRecogniton form = new FaceRecogniton();                           
                nivelForm.Show();            
                UC_Info dado = new UC_Info();
                dado.InfoText = "Voce acessou o Nível 1";

            }
            else if (idNivel == 2)
            {
                MessageBox.Show("Acesso a dados de nível 2");
            }
            else if (idNivel == 3)
            {
                MessageBox.Show("Acesso a dados de nível 3");
            }
        }
        
    }
}

        
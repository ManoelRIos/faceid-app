using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marerial_design_elements
{
    class Signup
    {
        #region Variáveis
        private Capture videoCapture = null;
        #endregion

        public void CapturarVideo()
        {
            if (videoCapture != null) videoCapture.Dispose();
            videoCapture = new Capture();
            

        }
    }

  
}

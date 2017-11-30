﻿using Emgu.CV;
using Emgu.CV.Structure;

namespace Football
{
    public interface ISource
    {
        VideoCapture Capture { get; set; }
        Image<Bgr, byte> ImgOriginal { get; set; }
        Image<Gray, byte> ImgFiltered { get; set; }
        //static VideoScreen _home;
        bool TakeASource();
        bool StartVideo();
        bool StartCamera();
        void Pause();
        void Stop();        
        bool StartLastUsedVideo();
        bool Check();
        Image<Gray, byte> ColorRange(int lowBlue, int lowGreen, int lowRed, int highBlue, int highGreen, int highRed);
        Image<Gray, byte> ConvertToGray();
        Image<Gray, byte> GetFilteredImageZones(Colour clr);
        Image<Gray, byte> GetFilteredImage(Colour colour);

    }
}

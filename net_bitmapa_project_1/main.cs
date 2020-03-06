using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace net_bitmapa_project_1
{
    //added on branch VojtaVagunda
    class main
    {
        //commet local
        static void Main(string[] args)
        {
            String fileName = "d:\\dokumenty\\Vojta\\UTB\\5_LET_IT\\multimedia\\OneDrive_2020-02-12\\Zpracovani rastrovych obrazku formatu BMP & PCX\\_Obrazky_zdroj\\BMP\\jednoduchy_barevny_24.bmp";//testovaci_1_width10

            String fileNameResult = "d:\\dokumenty\\Vojta\\UTB\\5_LET_IT\\multimedia\\OneDrive_2020-02-12\\Zpracovani rastrovych obrazku formatu BMP & PCX\\_Obrazky_zdroj\\BMP\\changed\\jednoduchy_barevny_24_changed.bmp";
           
            /*BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            byte[] buff;
            buff = new byte[(int)numBytes];
            buff = br.ReadBytes((int)numBytes);*/

            BitMap Pic = new BitMap(fileName);
            BitMap PicChanged = new BitMap(fileName);
            Pic.pixelArr[0, 0].R = 0;
            Pic.pixelArr[0, 0].B = 0;
            Pic.pixelArr[0, 0].G = 255;

            Pic.pixelArr[4, 40].R = 0;
            Pic.pixelArr[4, 40].B = 0;
            Pic.pixelArr[4, 40].G = 255;
            Pic.SavePictureToFile(fileNameResult);
            //pixely ze řádku jsou vždy ukončené zarovnávacím počtem byte, tak aby byl počet byte v řádku dělitelný 4 bez zbytku
            


            //FileToByteArray("d:\\dokumenty\\dotnet\\obr\\jednoduchy1.bmp");

            //uvolnění zdrojů
            //fs.Close();

        }
    }

}

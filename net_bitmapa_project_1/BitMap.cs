﻿using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace net_bitmapa_project_1
{
    class BitMap
    {

        //puvodni inicializace a nacteni souboru
        //public static String fileName = "d:\\dokumenty\\dotnet\\obr\\jednoduchy1.bmp";
        //FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //stream.Close(); ukonceni
        //tmp
        public BitMap(String fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            streamBits = new BinaryReader(fs);

            long numbytes = new FileInfo(fileName).Length;
            buff = new byte[(int)numbytes];
            buff = streamBits.ReadBytes((int)numbytes);
            
            streamBits.BaseStream.Seek(0, SeekOrigin.Begin);
            //POKUS NAČÍTÁNÍ DO PRVKŮ POMOCÍ BYTE POLE =>je to příliš zdlouhavé a vyžaduje to vytváření dočasných byte polí =>použijeme taky byte stream
            //long numBytes = new FileInfo(fileName).Length;
            //buff = new byte[(int)numBytes];
            //buff = streamBits.ReadBytes((int)numBytes);            
            //Array.Copy(buff, 2, BM_SizeArr, 0, 4);            
            //BM_Size = Convert.ToUInt32(BitConverter.ToInt32(BM_SizeArr, 0));
            //streamBits.Close; ukoncení

            //je možno použít buď  Convert.ToUInt32() nebo (uint) 
            BM_Type = (uint)(streamBits.ReadInt16());
            BM_Size = Convert.ToUInt32(streamBits.ReadInt32());            
            Unused1 = (uint)(streamBits.ReadInt16());
            Unused2 = (uint)(streamBits.ReadInt16());
            BM_Offset = (uint)(streamBits.ReadInt32());

            //BITMAPINFOHEADER \/
            BM_NumberOfBit = (uint)(streamBits.ReadInt32());
            BM_Width = (uint)(streamBits.ReadInt32());
            BM_Height = (uint)(streamBits.ReadInt32());
            BM_Planes = (uint)(streamBits.ReadInt16());
            BM_BitsPerPixel = (uint)(streamBits.ReadInt16());   //2B-biBitCount
            BM_Compression = (uint)(streamBits.ReadInt32());  //4B-biCompression (neřešíme)
            BM_ByteSizeToCom = (uint)(streamBits.ReadInt32());    //4B-biSizeImage (asi nepotřebujeme)
            BM_XOutPerMeter = (uint)(streamBits.ReadInt32());    //4B-biXPelsPerMeter
            BM_YOutPerMeter = (uint)(streamBits.ReadInt32());   //4B-biYPelsPerMeter
            BM_ByteColorUsed = (uint)(streamBits.ReadInt32());    //4B-biClrUsed
            BM_NeededByteToColor = (uint)(streamBits.ReadInt32());    //4B-biClrImportant

            //RowLength = Convert.ToUInt32(Math.Ceiling(BM_Width * BM_BitsPerPixel / 32.0) * 4);
            //RowBitAlignment = (RowLength * 8) - (BM_Width * BM_BitsPerPixel);
            //RowByteAlignment = RowBitAlignment / 8;
            ////loading of picture 1 bit
            ////loading of color palette
            //SizeOfPallet = BM_Offset - HeadersLength;
            //ColorPalette = new Color[SizeOfPallet / 4];//4 because every color is expressed by 4 BYTES               
            //for (int col = 0; col < (SizeOfPallet / 4); col++)
            //{
            //    TempIndex = HeadersLength + col * 4 + 0;
            //    blue = buff[TempIndex];

            //    green = buff[HeadersLength + col * 4 + 1];

            //    TempIndex3 = HeadersLength + col * 4 + 2;
            //    red = buff[TempIndex3];

            //    ColorPalette[col] = Color.FromArgb(red, green, blue);
            //}

            ////loading of pixel array 1BIT picture, true = white, false=black
            //BM_OffsetMoved = BM_Offset;
            //pixelIndexArr = new uint[BM_Height, BM_Width];//pole pixelů, jednotlivé složky plole mají v sobě uloženo buď 0 or 1, indexi do barevné palety
            ////nepouzivano //uint NumByteForRow = (uint)Math.Ceiling(BM_Width / 8.0);
            //BitArray bits;
            //byte[] arr = new byte[RowLength];
            ////byte[] OneBytearr=
            //for (int r = 0; r < BM_Height; r++)
            //{
            //    Array.Copy(buff, BM_OffsetMoved, arr, 0, RowLength);                
            //    int indexArr = 0;                
            //    //bits = new BitArray(arr[0]);
            //    for (int col = 0; col < BM_Width; col++)
            //    {
            //        bits = new BitArray(new int[] { arr[indexArr] });
            //        for (int indexB = 0; indexB <8; indexB++) {                        
            //            pixelIndexArr[r, col] = Convert.ToUInt32(bits[8-indexB-1]);
            //            col = col + 1;
            //        }
            //        indexArr++;
            //    }

            //    //after first iteration we have to change Offset               
            //    BM_OffsetMoved = BM_OffsetMoved + RowLength;
            //}

            //loading of fixel array 24 bit picture
            RowLength = Convert.ToUInt32(Math.Ceiling(BM_Width * BM_BitsPerPixel / 32.0) * 4);
            RowBitAlignment = (RowLength*8) - (BM_Width * BM_BitsPerPixel); //number of bits used for aligment of row
            RowByteAlignment = RowBitAlignment / 8;                         //number of byts used for aligment of row
            pixelArr = new VColor[BM_Height,BM_Width];

            BM_OffsetMoved = BM_Offset;                      
            for (int r = 0; r < BM_Height; r++) {
                for (int col = 0; col < BM_Width; col++)
                {
                    TempIndex = BM_OffsetMoved + 3*col + 0;
                    //b
                    blue =  buff[BM_OffsetMoved + 3 * col + 0];
                    //g
                    TempIndex2 = BM_OffsetMoved + 3 * col + 1;
                    green = buff[BM_OffsetMoved + 3 * col + 1];
                    //r
                    TempIndex3 = BM_OffsetMoved + 3 * col + 2;
                    red = buff[BM_OffsetMoved + 3 * col + 2];

                    pixelArr[r,col]= new VColor(red, green, blue);
                }
                //after first iteration we have to change Offset               
                BM_OffsetMoved = Convert.ToUInt32(TempIndex3) + RowByteAlignment+1;
            }
            //uvolnění zdrojů
            fs.Close();
        }
        public void SavepixelArrToBuff()
        {
            BM_OffsetMoved = BM_Offset;
            for (int r = 0; r < BM_Height; r++)
            {
                for (int col = 0; col < BM_Width; col++)
                {
                    TempIndex = BM_OffsetMoved + 3 * col + 0;
                    //b
                    byte tempB = Convert.ToByte(pixelArr[r, col].B);
                    buff[TempIndex] = Convert.ToByte(pixelArr[r, col].B);
                    //g
                    TempIndex2 = BM_OffsetMoved + 3 * col + 1;
                    buff[TempIndex2] = Convert.ToByte(pixelArr[r, col].G);
                    //r
                    TempIndex3 = BM_OffsetMoved + 3 * col + 2;
                    buff[TempIndex3] = Convert.ToByte(pixelArr[r, col].R);
                }
                //after first iteration we have to change Offset               
                BM_OffsetMoved = Convert.ToUInt32(TempIndex3) + RowByteAlignment + 1;
            }
        }
        public void SavePictureToFile(String path) {
            SavepixelArrToBuff();
            //buff[54] = 0;
            //buff[55] = 100;
            //buff[56] = 100;
            // Delete the file if it exists.                       
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream fs = File.Create(path);
            BinaryWriter binWriter = new BinaryWriter(fs);
            binWriter.Write(buff, 0, buff.Length);
            binWriter.Close();

        }
        //other needfull variables
        public BinaryReader streamBits;
        public long TempIndex;
        public long TempIndex2;
        public long TempIndex3;
        public byte[] buff;
        public Color[] ColorPalette; 
        public const uint HeadersLength = 54; //BitmapFileHeader + delka BITMAPINFOHEADER
        public uint RowLength { get; private set; }
        public uint RowBitAlignment { get; private set; }
        public uint RowByteAlignment { get; private set; }
        public uint BM_OffsetMoved { get; private set; }
        
        public uint SizeOfPallet { get; private set; }
        public VColor[,] pixelArr;
        public uint[,] pixelIndexArr;

        public Int32 red = 0;
        public Int32 green = 0;
        public Int32 blue = 0;
                
        //BMP header\/
        public uint BM_Type { get; private set; } //2B-bfType        
        public uint BM_Size { get; private set; } //4B-bfSize
        public byte[] BM_SizeArr = new byte[4];
        //2B nic
        public uint Unused1 { get; private set; }
        public uint Unused2 { get; private set; }
        //2B nic
        public uint BM_Offset { get; private set; } //4B-bfOffBits
        //BMP header /\

        //BITMAPINFOHEADER \/
        public uint BM_NumberOfBit { get; private set; } //4B-biSize
        public uint BM_Width { get; private set; } //4B-biWidth
        public uint BM_Height { get; private set; }  //4B-biHeight
        public uint BM_Planes { get; private set; } //2B-biPlanes
        public uint BM_BitsPerPixel { get; private set; }   //2B-biBitCount
        public uint BM_Compression { get; private set; }    //4B-biCompression (neřešíme)
        public uint BM_ByteSizeToCom { get; private set; }    //4B-biSizeImage (asi nepotřebujeme)
        public uint BM_XOutPerMeter { get; private set; }    //4B-biXPelsPerMeter
        public uint BM_YOutPerMeter { get; private set; }    //4B-biYPelsPerMeter
        public uint BM_ByteColorUsed { get; private set; }    //4B-biClrUsed
        public uint BM_NeededByteToColor { get; private set; }    //4B-biClrImportant

    }



}


//https://docs.microsoft.com/cs-cz/dotnet/api/system.collections.bitarray?view=netframework-4.8
/*
 * 
 * https://www.root.cz/clanky/graficky-format-bmp-pouzivany-a-pritom-neoblibeny/
 * 
 * Hlavička souboru BMP
 * 2B (bfType) Identifikátor formátu BMP.
 * 4B (bfSize) Celková velikost souboru s obrazovými údaji.
 * 2B (bfReserved1) nic
 * 2B (bfReserved2) nic
 * 4B (bfOffBits) Posun dat od začátku souboru k datům.
 * 
 * Metainformace o uloženém rastrovém obrazu
 * 4B (biSize) Velikost datové struktury.
 * 4B (biWidth) šířka v pixelech
 * 4B (biHeight) výška n pixelech
 * 2B (biPlanes) Bitové hladiny téměř vždy 1
 * 2B (biBitCount) Počet bitů na pixel (podle barev).
 * 4B (biCompression) neřešíme
 * 4B (biSizeImage) Velikost obrazu v B, položka je nulová, pokud není použitá žádná komprese
 * 4B (biXPelsPerMeter) Udává horizontální rozlišení výstupního zařízení v pixelech na metr, většinou 0
 * 4B (biYPelsPerMeter) Udává vertikální rozlišení výstupního zařízení v pixelech na metr většinou 0
 * 4B (biClrUsed) Celkový počet barev použitých v bitmapě, pokud je 0 jsou použité všechny, protože počet barev lze zjistit z biSize
 * 4B (biClrImportant) Počet barev potřebný pro vykreslení obrázku, většinou bývá 0, protože všechny barvy jsou potřebné, využíváno u zařízení s barevným omezení
 * 
 * Barvová paleta
 * 4B na jeden bit obrazku na jednu barvu RGB model, ve struktuře je poskládaný jako BGR -> B-Blue 1B, G-Green 1B, R-Red 1B, poslední B je vždy nulový, možná byl vyhrazen pro alfa kanál
 * 1bit bmp má barevnou paletu 4B, 2bit má 8B, 4bit má 16B, 8bit má 32B a 24bit má 96B velikost barevné palety
 * 
 * 
 */


//naprogramovat načtení bytů pomocí streamu a provést jednotkový test na otestování funkčnosti na pole, použit na malou bitmapu třeba 4x4


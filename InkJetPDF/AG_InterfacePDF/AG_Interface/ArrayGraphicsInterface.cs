using System;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AG_Interface
{
    class ArrayGraphicsInterface
    {
        private TcpClient tcpClient;
        private TcpClient tcpClientExtra;
        internal NetworkStream networkStream;
        internal NetworkStream networkStreamExtra;
        public bool connected=false;
        private ListBox infobox;
       
        public ArrayGraphicsInterface(ListBox listBox1)
        {
            // TODO: Complete member initialization
            this.infobox = listBox1;
            _ArrayGraphicsInterface();
        }
        public void _ArrayGraphicsInterface()
        {
            connected = ConnectToServer(10001, "192.168.0.2");//was 10001
            if (!connected) //implemented for bad network
            {
                Thread.Sleep(1000);
                connected = ConnectToServer(10001, "192.168.2.2");
            }      
        }

        


        public void  SendBuffer()
        {


        }
        public int Pack = 1;

        public byte[] RASTER_BUFFER_CREATE(byte NumBuffers, int BufferSize, byte PrintDirFwd, byte HorizontalDPIEnum, byte NumPens, int NumberOfLongWordsInLine, int NumberOfLinesInImage, int HorizontalDpi, bool nBufferPrefill)
        {
            RASTER_BUFFER_CREATE_MSG msg = new RASTER_BUFFER_CREATE_MSG();
            msg.ucNumBuffers = (byte)NumBuffers;
            msg.uliBufferSize = (uint)IPAddress.HostToNetworkOrder((int)BufferSize);
            msg.ucPrintDirFwdRev = (byte)PrintDirFwd;
            msg.ucHorizontalDpiEnum = (byte)HorizontalDPIEnum;
            msg.ucNumPens = (byte)NumPens;
            msg.uiNumberOfLongWordsInLine = (uint)IPAddress.HostToNetworkOrder((int)NumberOfLongWordsInLine);
            msg.uiNumberOfLinesInImage = (uint)IPAddress.HostToNetworkOrder((int)NumberOfLinesInImage);
            msg.uiHorizontalDpi = (uint)IPAddress.HostToNetworkOrder((int)HorizontalDpi);
            msg.noBufferPrefill = (byte)(nBufferPrefill ? 1 : 0);
            byte[] message = StructToArray(msg);
            return message;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RASTER_BUFFER_CREATE_MSG
        {
            public byte ucNumBuffers;
            public uint uliBufferSize;
            public byte ucPrintDirFwdRev;
            public byte ucHorizontalDpiEnum;
            public byte ucNumPens;
            public uint uiNumberOfLongWordsInLine;
            public uint uiNumberOfLinesInImage;
            public uint uiHorizontalDpi;
            public byte noBufferPrefill;
        }


        public byte[] RASTER_BUFFER_DATA(byte BufferNum, byte ReadyToPrint, uint RasterDataSize, uint StartingByteNum, uint NumBytesToCopyBeforeJumping, uint JumpAmount, byte AddToClearList, uint PageNumber, byte GeneratePulse_1,byte GeneratePulse_2,byte[] PixelData)
        {
            RASTER_BUFFER_DATA_MSG msg = new RASTER_BUFFER_DATA_MSG();
            msg.ucBufferNum= BufferNum;
            msg.ucReadyToPrint= ReadyToPrint;
            msg.uliRasterDataSize= (uint)IPAddress.HostToNetworkOrder((int)RasterDataSize);
            msg.uliStartingByteNum= (uint)IPAddress.HostToNetworkOrder((int)StartingByteNum);
            msg.uliNumBytesToCopyBeforeJumping= (uint)IPAddress.HostToNetworkOrder((int)NumBytesToCopyBeforeJumping);
            msg.uliJumpAmount= (uint)IPAddress.HostToNetworkOrder((int)JumpAmount);
            msg.ucAddToClearList= AddToClearList;
            msg.uliPageNumber= (uint)IPAddress.HostToNetworkOrder((int)PageNumber);
            msg.ucGeneratePulse_1= GeneratePulse_1;
            msg.ucGeneratePulse_2= GeneratePulse_2;
            byte[] messagehead = StructToArray(msg);
            byte[] message = new byte[messagehead.Length + PixelData.Length];
            int p = 0;
            for (int i=0; i< messagehead.Length;i++)
            {
                message[p++] = messagehead[i];
            }
            for (int i = 0; i < PixelData.Length; i++)
            {
                message[p++] = PixelData[i];
            }
            return message;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RASTER_BUFFER_DATA_MSG
        {

            public byte ucBufferNum;
            public byte ucReadyToPrint;
            public uint uliRasterDataSize;
            public uint uliStartingByteNum;
            public uint uliNumBytesToCopyBeforeJumping;
            public uint uliJumpAmount;
            public byte ucAddToClearList;
            public uint uliPageNumber;
            public byte ucGeneratePulse_1;
            public byte ucGeneratePulse_2;
        }


        byte[] StructToArray(object str)
        {
            int size = Marshal.SizeOf(str);
            byte[] array = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, array, 0, size);
            Marshal.FreeHGlobal(ptr);
            return array;
        }

        int paddedcolums = 0;
        int columnheightboudrary = 0;
        int columnheightbytesize = 0;


        void Setpixel(int col,int row, byte[] buffer)
        {
            int i = (col * columnheightbytesize) + (row >> 3);
            byte pixelbyte = (byte)( 0x80 >> (row & 7));
            buffer[i] |= pixelbyte;
        }
        bool Getpixel(int col, int row, byte[] buffer)
        {
            int i = (col * columnheightbytesize) + (row >> 3);
            byte pixelbyte = (byte)(0x80 >> (row & 7));
            return (( buffer[i] & pixelbyte) !=0 );
        }
        private Bitmap FullBitmap;
        internal int FullBitmapHeight;
        internal int FullBitmapWidth;
        private byte[] BufferPixelData;
        private int BufferPixelDataColumsize=0;
        private int BufferPixelDataColumcount=0;
       
        public double FullBitmapScaleX = 1.0;
        public double FullBitmapScaleY = 1.0;
        private bool printable = true;
        public int WhiteTreshhold=500;

        bool GetScaledPixelfromFullBitmap(int x, int y)
        {
            double xi = FullBitmapScaleX * x;
            double yi = FullBitmapScaleY * y;
            if ((xi < 0) || (yi<0) || (xi>= FullBitmapWidth) || (yi>=FullBitmapHeight)) return true;//white 
            printable = true;
          //  if ((x & 0xff) == 0xff ) return false;//white      
            Color c = FullBitmap.GetPixel((int)xi, (int)yi);
            int intensity =  c.R + c.G + c.B;
            return (intensity > WhiteTreshhold); //white>50%
        }
         
      

        public bool[] PalleteEntryIsBlack;//array containing indexed pallete items that are black

        public void CreatePaletteTranslator(Bitmap FullBitmap)
        {
            int plaetteentries = FullBitmap.Palette.Entries.Length;
            Color[] PalleteEntries = FullBitmap.Palette.Entries;
            PalleteEntryIsBlack = new bool[plaetteentries];
            for (int i = 0; i < plaetteentries; i++)
            {
                Color c = PalleteEntries[i];
                int intensity = c.R + c.G + c.B;
                PalleteEntryIsBlack[i] = intensity < WhiteTreshhold;
            }
        }
        public void SetXcalibration(double factor)
        {
            xcalibrationfactor = factor;
        }

        public bool MirrorredX = true;
        public double xcalibrationfactor = 1.01;//bigger factor is smaller image
        unsafe public bool FillBufferFromImage(double lane, int buffernr,bool calibrationlines)
        {
            if (FullBitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                FillBufferFromImageOld(lane, buffernr);
            }
            else
            {
                BitmapData imageData = FullBitmap.LockBits(new Rectangle(0, 0, FullBitmap.Width, FullBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
                byte* scan0 = (byte*)imageData.Scan0.ToPointer();
                int stride = imageData.Stride;
                int basex = buffernr * BufferPixelDataColumcount;
                int basey = (int)(lane * BufferPixelDataColumsize);
                byte pixelR=0;
                for (int i = 0; i < BufferPixelDataColumsize ; i++)
                {
                    int y = basey + i;
                    if ((y >= 0) && (y < FullBitmapHeight))
                    {
                        for (int j = 0; j < BufferPixelDataColumcount; j++)
                        {
                            double x = basex + j;
                            x = x * xcalibrationfactor;

                            if ((x >= 0) && (x < FullBitmapWidth))
                            {
                                byte* row = scan0 + (y * stride);
                                if (!MirrorredX) pixelR=row[(int)x]; else pixelR = row[FullBitmapWidth - (int) x - 1];
                                if (PalleteEntryIsBlack[pixelR]) Setpixel(BufferPixelDataColumcount - j + 1 , i, BufferPixelData);
                            }
                            if (calibrationlines)
                            {
                                if ((y % 150) == 0x00) Setpixel(BufferPixelDataColumcount - j + 1, i, BufferPixelData);    //debug
                                if ((x % 150) == 0x00) Setpixel(BufferPixelDataColumcount - j + 1, i, BufferPixelData);    //debug           
                            }
                        }
                    }
                }
                FullBitmap.UnlockBits(imageData);
            }
            return true;
        }

        public bool FillBufferFromImageOld(double lane, int buffernr)
        {
            printable = false;
            for (int i = 0; i < BufferPixelData.Length; i++) BufferPixelData[i] = 0;//clear buffer
            int basex = buffernr * BufferPixelDataColumcount;
            int basey = (int)(lane * BufferPixelDataColumsize);
            for (int i = 0; i < BufferPixelDataColumsize; i++)
            {
                for (int j = 0; j < BufferPixelDataColumcount; j++)
                {
                    bool p = GetScaledPixelfromFullBitmap(basex + j, basey + i);
                    if (!p) Setpixel(j, i, BufferPixelData);
                }
            }
            return printable;
        }


        internal void DirectPrintBufferNoWait(ListBox statuslistbox)
        {
            SendMessage(0x16);//stop printing
            Thread.Sleep(100);
            byte heads = 4;
            int columnheight = 600;//450
            int columns = FullBitmapWidth;
            paddedcolums = columns + 50;//add 50
            if (columnheight <= 512) columnheightboudrary = 512; else columnheightboudrary = 1024;
            columnheightbytesize = columnheightboudrary / 8;
            int buffersize = columnheightbytesize * paddedcolums;
            SendMessage(0x02, RASTER_BUFFER_CREATE(2, buffersize, 1, 0, heads, columnheightboudrary/32, columns, 300, false));
         
            BufferPixelData = new byte[buffersize];
            BufferPixelDataColumsize = columnheight;
            BufferPixelDataColumcount = columns;
           

            //data word sequentieel per colom aargeleverd (voor alle nozzles 1 bit)
            //colom is veelvoud 512 bit (dus 1024 voor 4 koppen)
            for (int i = 0; i < BufferPixelData.Length; i++) //512 bits per vert row
            {
                BufferPixelData[i] = 0; //white all
            }
            
            int lane = 0;
            int buffer = 0;
            FillBufferFromImage(lane,buffer,true);

            int col = 0;
            for (col = 0; col < 150; col += 16) //512 bits per vert row
            {
                for (int i = 0; i < columnheightbytesize; i++) //512 bits per vert row
                {
                    BufferPixelData[i + (col * columnheightbytesize)] = (byte)255; //dummy data

                }
            }

            for (int x = 0; x < 150; x ++) //512 bits per vert row
            { 
                Setpixel(x % columns,  x , BufferPixelData);
            }

            for (int i = 0; i < 2; i++)
            {
                SendMessage(0x03, RASTER_BUFFER_DATA((byte)((i % 4) + 1), 1, (uint) buffersize, 0,(uint) columnheightbytesize,(uint) columnheightbytesize, 0, 1, 0, 0, BufferPixelData));
                Thread.Sleep(2000);
                FillBufferFromImage(lane, i,true); //test; 2nd and up only with mage data
            }

            SendMessage(0x25);//Enable printing; should trigger  RASTER_BUFFER_READY_FOR_DATA (0x06)
        }

        internal Image GetLastBufferDateAsImage()
        {
            Bitmap BufferImage = new Bitmap(BufferPixelDataColumcount,BufferPixelDataColumsize);
            
            //  BufferImage.SetPixel()
      
            for (int i = 0; i < BufferPixelDataColumsize; i++)
            {
                for (int j = 0; j < BufferPixelDataColumcount; j++)
                {
                    if (Getpixel(j, i, BufferPixelData)) BufferImage.SetPixel(j, i, Color.FromArgb(0, 0, 255)); else BufferImage.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                }
            }
            return BufferImage;

        }

        internal void InitializePrint()
        {
            byte heads = 4;
            int columnheight = 600;
            int columns = FullBitmapWidth;// Debug Here: 6500 works.  7000 does not work
            paddedcolums = columns + 50;

            if (columnheight <= 512) columnheightboudrary = 512; else columnheightboudrary = 640;
            columnheightbytesize = columnheightboudrary / 8;
            int buffersize = columnheightbytesize * paddedcolums;
            SendMessage(0x02, RASTER_BUFFER_CREATE(Buffers, buffersize, 0, 0, heads, columnheightboudrary / 32, columns, 300, false));
            Thread.Sleep(200);

            BufferPixelData = new byte[buffersize];
            BufferPixelDataColumsize = columnheight;
            BufferPixelDataColumcount = columns;
            BufferPixelDataSize = buffersize;
            CurrentBufferNumber = 0;
            CurrentLaneNumber = 0;          
          //  SendMessage(0x25);//stond uitEnable printing; should trigger  RASTER_BUFFER_READY_FOR_DATA (0x06)       
        }

        internal void StartPrint()
        {
            SendMessage(0x25);//Enable printing; 
        }

        internal void DeactivateHead()
        {
            byte[] stoparg = new byte[1];
            stoparg[0] = 2;//stond op 2
            Thread.Sleep(100);
            SendMessage(0x16, stoparg);//stop printing
/*
            Thread.Sleep(100);
            SendMessage(0x86);//empty buffers
            stoparg[0] = 2;
            Thread.Sleep(100);
            SendMessage(0x16, stoparg);//stop printing
            Thread.Sleep(100);
            SendMessage(0x86);//empty buffers
*/
        }
        internal void SetContrast(byte val)
        {
            //Contrast 0..15 (0 = 6.25%, 15=100%)
            byte[] dataarg = new byte[1];
            dataarg[0] = val;
            Thread.Sleep(100);
            SendMessage(0x89, dataarg);
        }

        internal void ProgramXML(int programxmltype, string programxmlpath)
        {
            //Open XML file and send it to the InkJet
            //Type is te command code for the corresponding file type.
            //0xAD=config;0xAE=pen config;0xAF=stich
            string readText = File.ReadAllText(programxmlpath);
            byte[] XMLbytes = Encoding.ASCII.GetBytes(readText);
            Thread.Sleep(100);
            SendMessage((byte) programxmltype, XMLbytes);
        }

        internal void TriggerPulseGenerator(byte generator)
        {
            //gererator:  0 or 1 for generator 0 or generator 1
            byte[] dataarg = new byte[1];
            dataarg[0] = generator;
            Thread.Sleep(100);
            SendMessage(0x67, dataarg);
        }
        public int CurrentBufferNumber = 0;
        private double  CurrentLaneNumber = 0;
        private int BufferPixelDataSize = 0;
        private byte Buffers = 2;
        public void UploadLaneNoWait(double lane,bool cal)
        {          
           //data word sequentieel per colom aangeleverd (voor alle nozzles 1 bit)
           //colom is veelvoud 128 bit (dus 640 voor 4 koppen)
           CurrentBufferNumber = 0;
           CurrentLaneNumber = lane;          
     //     for (int i = 0; i < Buffers; i++)
            {           
                FillNextPrintBufferRLE(cal);
                Thread.Sleep(200);          
            }
           Thread.Sleep(100);         
   
        }

        internal void FillNextPrintBuffer(bool calibrationlines)
        {
            for (int i = 0; i < BufferPixelData.Length; i++) BufferPixelData[i] = 0; //white all
            bool printabledata = FillBufferFromImage(CurrentLaneNumber, CurrentBufferNumber, calibrationlines); //test; 2nd and up only with mage data

            if (printabledata)
            {
                SendMessage(0x03, RASTER_BUFFER_DATA((byte)((CurrentBufferNumber % Buffers) + 1), 1, (uint)BufferPixelDataSize, 0, (uint)columnheightbytesize, (uint)columnheightbytesize, 0, 1, 0, 0, BufferPixelData));
                CurrentBufferNumber++;
            }
        }

        internal void FillNextPrintBufferRLE(bool calibrationlines)
        {
            for (int i = 0; i < BufferPixelData.Length; i++) BufferPixelData[i] = 0; //white all 
            bool printabledata = true;
        //    if ((CurrentBufferNumber % Buffers) == 0) 
            FillBufferFromImage(CurrentLaneNumber, 0, calibrationlines/* CurrentBufferNumber % Buffers*/); //test; 2nd and up only with mage data
            byte [] BufferPixelDataRLE=CompressBufferRLE32(BufferPixelData);
            if (printabledata)
            {
                SendMessage(0x81, RASTER_BUFFER_DATA((byte)((CurrentBufferNumber % Buffers) + 1), 1, (uint)BufferPixelDataSize, 0, (uint)columnheightbytesize, (uint)columnheightbytesize, 0, 1, 0, 0, BufferPixelDataRLE));
                CurrentBufferNumber++;
            }
            CurrentLaneNumber++;
        }

        private byte [] CompressBufferRLE32(byte[] inbuffer)
        {
            //make 32 bit int, msb in first byte
            uint[] intArray = new uint[(inbuffer.Length / 4)];
            for (int i = 0; i < inbuffer.Length; i += 4) intArray[i / 4] = (uint)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(inbuffer, i));


            uint trackindex = 0;
            uint t_trackindex = 0;
            uint newdatavalue;
            //start new sequence
            uint countffffffff = 0;
            uint count00000000 = 0;

            //restart RLE buffer
            RLE32Buffer_subhead = 0xffffffff;
            RLE32Buffer_current = 0;
            for (int i = 0; i < RLE32Buffer.Length; i++) RLE32Buffer[i] = 0; //zero all

            while (trackindex < intArray.Length)
            {
                t_trackindex = trackindex;
                newdatavalue = intArray[t_trackindex];

                t_trackindex = trackindex;
                countffffffff = 0;
                while ((t_trackindex < intArray.Length) && (intArray[t_trackindex++] == 0xffffffff)) countffffffff++;

                t_trackindex = trackindex;
                count00000000 = 0;
                while ((t_trackindex < intArray.Length) && (intArray[t_trackindex++] == 0x00000000)) count00000000++;


                if (countffffffff >= 3)
                {
                    //compress as ffffffff
                    trackindex += countffffffff;
                    addRLE32Array(0xC0000000 | countffffffff, true);
                }
                else
                    if (count00000000 >= 3)
                    {
                        //compress as 00000000
                        trackindex += count00000000;
                        addRLE32Array(0x80000000 | count00000000, true);
                    }
                    else
                    {
                        //add to uncompressed
                        addRLE32Array(newdatavalue, false);
                        trackindex++;
                    }

            }
            byte[] CompressedByteArray = new byte[RLE32BufferLen*4];
            uint CompressedByteArrayIndex=0;
            byte[] IntBytes=new byte[4];
            for(int i=0;i<RLE32BufferLen;i++)
            { 
                IntBytes = BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder((int)RLE32Buffer[i]));
                for(int j=0;j<4;j++)
                {
                    CompressedByteArray[CompressedByteArrayIndex++] = IntBytes[j];
                }
            }

            

            return CompressedByteArray;
        }


        uint[] RLE32Buffer = new uint[10000000];
        uint RLE32Buffer_subhead = 0xffffffff;
        uint RLE32Buffer_current = 0;
        uint RLE32BufferLen = 0;
        private void addRLE32Array(uint val, bool compressed)
        {
            if (compressed)
            {
                //add direct, but add compress subbuffer fi
                RLE32Buffer_subhead = 0xffffffff;//kill uncompressed head
                RLE32Buffer[RLE32Buffer_current++] = val;
            }
            else
            {
                //ADD to uncomp
                if (RLE32Buffer_subhead == 0xffffffff)
                {
                    RLE32Buffer_subhead = RLE32Buffer_current++;//create new subhead
                    RLE32Buffer[RLE32Buffer_subhead] = 0; //count = 0;bit 31=0;
                }
                RLE32Buffer[RLE32Buffer_subhead]++;
                RLE32Buffer[RLE32Buffer_current++] = val;
            }
            RLE32BufferLen = RLE32Buffer_current;
        }

        private bool ConnectToServer(int nPort, string nServer)
        {
            try
            {
                tcpClient = new TcpClient(nServer, nPort);
                tcpClient.NoDelay = true;
                tcpClient.ReceiveTimeout = 200;
                tcpClient.SendTimeout = 200;
                tcpClient.ReceiveBufferSize = 4000;
                //tcpClient.SendBufferSize = 400000;
                networkStream = tcpClient.GetStream();
/*
               tcpClientExtra = new TcpClient(nServer, nPort-1);
                tcpClientExtra.NoDelay = true;
                tcpClientExtra.ReceiveTimeout = 200;
                tcpClientExtra.SendTimeout = 200;
                tcpClientExtra.ReceiveBufferSize = 4000;
                //tcpClient.SendBufferSize = 400000;
                networkStreamExtra = tcpClientExtra.GetStream();
                return tcpClient.Connected && tcpClientExtra.Connected;*/
               return tcpClient.Connected;
            }
            catch
            { 
            }            
            return false;

        }

       
        AGMessage currentmessage = null;
        private byte[] Header = new byte[8];
        public int MessageNumOfBytes = 0;
        public int MessageEtx = 0;
        internal AGMessage ReadMessage(NetworkStream t_networkStream)
        {
            
            if (!CanReadByte()) return null;
            
          
            t_networkStream.Read(Header, 0, 8);//Always 8

            if (Header[0] == 0x02)
            {
                currentmessage = new AGMessage();               
                currentmessage.MessageChecksum = Header[5];
                currentmessage.MessageSeqNum = Header[6];
                currentmessage.MessageMsgID = Header[7];
                MessageNumOfBytes = (Header[1] << 24) | (Header[2] << 16) | (Header[3] << 8) | (Header[4]);
                int DataLen = MessageNumOfBytes - 4;

                if (DataLen > 0)
                {
                    currentmessage.CreateMessageData(DataLen);
                    t_networkStream.Read(currentmessage.MessageData, 0, DataLen);
                }
                int MessageEtx = t_networkStream.ReadByte();


                if (MessageEtx == 0x03)
                {
                    if (ListenForMessage == currentmessage.MessageMsgID)  FoundMessage = true;
                    return currentmessage;
                }
                else
                { //"AOS";
                    return null;
                }
            }

            else
            {
                
                //out of sync?
                return null;
            }
        }


        int ListenForMessage = -1;
        bool FoundMessage = false;

        private void SetListenForMessage(int messageid)
        {
            ListenForMessage = messageid;
            FoundMessage = false;
        }


        public void SendToAll(byte[] b)
        {
            _SendToAll(b, b.Length);
        }
        private void _SendToAll(byte[] b, int iLength)
        {
            try
            {
                networkStream.Write(b, 0, iLength);
            }
            catch(Exception err)
            {
                string exception = err.ToString();
            }
        }

        public bool CanReadByte()
        {
            return networkStream.DataAvailable;
        }
      
        public int ReadByte()
        {
            if (networkStream.DataAvailable)
                return networkStream.ReadByte();
            else
                return -1;
        }
        private byte TransmitSequence = 0;
        private ListBox listBox1;

        public Bitmap SetFullBitmap
        {
            get
            {
                return FullBitmap;
            }

            set
            {
                FullBitmap = value;
                FullBitmapHeight = FullBitmap.Height;
                FullBitmapWidth = FullBitmap.Width;
                CreatePaletteTranslator(FullBitmap);
            }
        }

     

        public byte[] CodeMessage(byte ID,byte[] data)
        {
            int n = 0;
            if(data!=null) n= data.Length;
            int messagelen = n + 4;
            byte[] message = new byte[5 + messagelen];
            byte checksum = 0;

            int i = 0;
            message[i++] = 0x02;
            message[i++] = (byte)(messagelen >> 24);
            message[i++] = (byte)(messagelen >> 16);
            message[i++] = (byte)(messagelen >>  8);
            message[i++] = (byte)(messagelen      );
            message[i++] = checksum;
            message[i++] = TransmitSequence++;
            message[i++] = ID;
            if (n > 0)
            {
                int j = 0;
                while ((n--) > 0) message[i++] = data[j++];
            }
            message[i++] = 0x03;
            return message;
        }

        public void SendMessage(byte ID,byte[] messagedata)
        {
            SendToAll(CodeMessage(ID,messagedata));
            string MessageString = "Sent ID:" + ID.ToString("X") + ":data";
            infobox.Items.Add(MessageString);
        }

        public void SendMessage(byte ID)
        {
            SendToAll(CodeMessage(ID, null));
            string MessageString = "Sent ID:" + ID.ToString("X");
            infobox.Items.Add(MessageString);
           
        }



     
    }

    public class AGMessage
    {
        public byte[] MessageData;
       
        public int MessageChecksum = 0;
        public int MessageSeqNum = 0;
        public int MessageMsgID = 0;
        public int MessageDataLen = 0;
        
        public AGMessage()
        {
            // Late create MessageData           
        }

        public void CreateMessageData(int iMessageDataLen)
        {
            MessageDataLen = iMessageDataLen;
            MessageData = new byte[MessageDataLen];
        }

        public AGMessage(int size)
        {
            MessageData = new byte[size];
        }
    }
}

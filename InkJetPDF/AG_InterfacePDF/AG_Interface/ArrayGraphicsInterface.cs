using System;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace AG_Interface
{
    class ArrayGraphicsInterface
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        public bool connected=false;
        
        public ArrayGraphicsInterface()
        {
            connected = ConnectToServer(10000, "192.168.1.2");
           
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

        internal void DirectPrintBuffer(ListBox statuslistbox)
        {
            SetListenForMessage(0x26);
            SendMessage(0x02, RASTER_BUFFER_CREATE(4, 12800, 0, 0, 3, 16, 150, 300, false));
            bool Received_Ready_to_print = false;
            AGMessage receivedmessage = null;
            int retrycount = 0;
            while ((!Received_Ready_to_print) && (retrycount<10))
            {
                receivedmessage = ReadMessage();
                if (receivedmessage == null)
                {
                    Thread.Sleep(1000);
                    retrycount++;
                }
                if (receivedmessage != null)
                {
                    string MessageString = "ID:" + receivedmessage.MessageMsgID.ToString("X") + ":";
                    statuslistbox.Items.Add(MessageString);
                    statuslistbox.Invalidate();
                    Thread.Sleep(10);

                    if (receivedmessage.MessageMsgID == 0x26)
                    {
                        Received_Ready_to_print = true;
                    }
                }
            }
            if(Received_Ready_to_print ) statuslistbox.Items.Add("Proceed"); else statuslistbox.Items.Add("Proceed, no rtp");
            byte[] PixelData = new byte[64 * 200];
            for (int i = 0; i < PixelData.Length; i++) //512 bits per vert row
            {
                PixelData[i] = 0; //white all

            }
            int row = 0;
            for (row = 0; row < 180; row += 16) //512 bits per vert row
            {
                for (int i = 0; i < 64; i++) //512 bits per vert row
                {
                    PixelData[i + (row * 64)] = (byte)255; //dummy data

                }
            }
          //  SendMessage(0x25);//Enable printing; should trigger  RASTER_BUFFER_READY_FOR_DATA (0x06)
            //opm : sensor + encoder actie vereist
            int buffernum = 1;
            for (int i = 0; i < 4; i++)
            {
                SendMessage(0x03, RASTER_BUFFER_DATA((byte)((i%4)+1), 1, 12800, 0, 64, 64, 0, 1, 0, 0, PixelData));
                Thread.Sleep(500);
            }
            SendMessage(0x25);//Enable printing; should trigger  RASTER_BUFFER_READY_FOR_DATA (0x06)
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
        bool GetScaledPixelfromFullBitmap(int x, int y)
        {
            double xi = FullBitmapScaleX * x;
            double yi = FullBitmapScaleY * y;
            if ((xi < 0) || (yi<0) || (xi>= FullBitmapWidth) || (yi>=FullBitmapHeight)) return true;//white
            printable = true;
            Color c = FullBitmap.GetPixel((int) xi,(int) yi);
            int intensity = c.R + c.G + c.B;
            return (intensity > 500); //white>50%
        }
         
        public bool FillBufferFromImage(double lane, int buffernr)
        {
            printable = false;
            for (int i = 0; i < BufferPixelData.Length; i++) BufferPixelData[i] = 0;//clear buffer
            int basex = buffernr * BufferPixelDataColumcount;
            int basey =(int) (lane * BufferPixelDataColumsize);
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
            FillBufferFromImage(lane,buffer);

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
                FillBufferFromImage(lane, i); //test; 2nd and up only with mage data
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

        public int CurrentBufferNumber = 0;
        private double  CurrentLaneNumber = 0;
        private int BufferPixelDataSize = 0;
        private byte Buffers = 2;
        public void StartPrintLaneNoWait(double lane)
        {
            byte[] stoparg=new byte[1];
            stoparg[0]=2;
            SendMessage(0x16, stoparg);//stop printing
            Thread.Sleep(3);
          //  SendMessage(0x86);//reset buffers
            Thread.Sleep(100);
            byte heads = 4;
            int columnheight = 600;
            int columns = FullBitmapWidth;// Debug Here: 6500 works.  7000 does not work
            paddedcolums = columns + 50;
           
            if (columnheight <= 512) columnheightboudrary = 512; else columnheightboudrary = 640;
            columnheightbytesize = columnheightboudrary / 8;
            int buffersize = columnheightbytesize * paddedcolums;
            SendMessage(0x02, RASTER_BUFFER_CREATE(Buffers, buffersize, 0, 0, heads, columnheightboudrary / 32, columns, 300, true));         
            Thread.Sleep(1000);  
   
            BufferPixelData = new byte[buffersize];
            BufferPixelDataColumsize = columnheight;
            BufferPixelDataColumcount = columns;
            BufferPixelDataSize = buffersize;
            //data word sequentieel per colom aangeleverd (voor alle nozzles 1 bit)
            //colom is veelvoud 128 bit (dus 640 voor 4 koppen)
            CurrentBufferNumber = 0;
            CurrentLaneNumber = lane;
            stop_afer_next_buffer = false;
            for (int i = 0; i < Buffers; i++)
            {
                FillNextPrintBuffer();
                Thread.Sleep(6000);          
            }
           Thread.Sleep(1000);         
           SendMessage(0x25);//Enable printing; should trigger  RASTER_BUFFER_READY_FOR_DATA (0x06)
        }
        bool stop_afer_next_buffer = false;

        public void DebugStartPrintLaneNoWait(double lane)
        {
            byte[] stoparg = new byte[1];
            stoparg[0] = 2;
            SendMessage(0x16, stoparg);//stop printing
            Thread.Sleep(3);
            //  SendMessage(0x86);//reset buffers
            Thread.Sleep(100);
            byte heads = 4;
            int columnheight = 600;
            int columns = 108*300;// 108 inch= 2743mm
            paddedcolums = columns + 50;

            if (columnheight <= 512) columnheightboudrary = 512; else columnheightboudrary = 640;
            columnheightbytesize = columnheightboudrary / 8;
            int buffersize = columnheightbytesize * paddedcolums;
            SendMessage(0x02, RASTER_BUFFER_CREATE(Buffers, buffersize, 0, 0, heads, columnheightboudrary / 32, columns, 300, true));
            Thread.Sleep(1000);

            BufferPixelData = new byte[buffersize];
            BufferPixelDataColumsize = columnheight;
            BufferPixelDataColumcount = columns;
            BufferPixelDataSize = buffersize;
            //data word sequentieel per colom aangeleverd (voor alle nozzles 1 bit)
            //colom is veelvoud 128 bit (dus 640 voor 4 koppen)
            CurrentBufferNumber = 0;
            CurrentLaneNumber = lane;
            stop_afer_next_buffer = false;
            for (int i = 0; i < Buffers; i++)
            {
                FillNextPrintBuffer();
                Thread.Sleep(12000);
            }
            Thread.Sleep(1000);
            SendMessage(0x25);//Enable printing; should trigger  RASTER_BUFFER_READY_FOR_DATA (0x06)
        }

        internal void FillNextPrintBuffer()
        {
            if (stop_afer_next_buffer)
            {
                //SendMessage(0x16);//stop printing
               // return;
            }

            for (int i = 0; i < BufferPixelData.Length; i++) BufferPixelData[i] = 0; //white all
            bool printabledata =FillBufferFromImage(CurrentLaneNumber, CurrentBufferNumber); //test; 2nd and up only with mage data
         //   bool printabledata = true;
           if (printabledata)
            {
               /*
                int col = 0;
                for (col = 0; col < BufferPixelDataColumcount; col += 10) //512 bits per vert row
                {
                    for (int i = 0; i < columnheightbytesize; i++) //512 bits per vert row
                    {
                        BufferPixelData[i + (col * columnheightbytesize)] = (byte)0x33; //dummy data

                    }
                }
            */
               SendMessage(0x03, RASTER_BUFFER_DATA((byte)((CurrentBufferNumber % Buffers) + 1), 1, (uint)BufferPixelDataSize, 0, (uint)columnheightbytesize, (uint)columnheightbytesize, 0, 1, 0, 0, BufferPixelData));
              
                CurrentBufferNumber++;
            }
            else
            {
                stop_afer_next_buffer = true;
            }
        }

        private bool ConnectToServer(int nPort, string nServer)
        {
            try
            {
                tcpClient = new TcpClient(nServer, nPort);
                tcpClient.NoDelay = true;
                tcpClient.ReceiveTimeout = 800;
                tcpClient.SendTimeout = 800;
                tcpClient.ReceiveBufferSize = 400000;
                tcpClient.SendBufferSize = 400000;
                networkStream = tcpClient.GetStream();
                return tcpClient.Connected;
            }
            catch{ }
            return false;

        }

       
        AGMessage currentmessage = null;
        private byte[] Header = new byte[8];
        public int MessageNumOfBytes = 0;
        public int MessageEtx = 0;
        internal AGMessage ReadMessage()
        {
            
            if (!CanReadByte()) return null;
            
          
            networkStream.Read(Header, 0, 8);//Always 8

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
                    networkStream.Read(currentmessage.MessageData, 0, DataLen);
                }
                int MessageEtx = networkStream.ReadByte();


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
            networkStream.Write(b, 0, iLength);
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
         
        }
        public void SendMessage(byte ID)
        {
            SendToAll(CodeMessage(ID, null));
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

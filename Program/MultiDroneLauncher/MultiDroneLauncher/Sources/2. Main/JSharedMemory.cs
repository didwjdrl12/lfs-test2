using System;
using System.IO.MemoryMappedFiles;
//using System.IO.MemoryMappedFiles;

namespace J2y.MultiDrone
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JSharedMemory
    //
    //  1. �ʱ�ȭ
    //      _memory_command = JSharedMemory.Create("SharedMemory_command", 1024);
    //
    //  2. ����
    //      _memory_command.Dispose();
    //
    //  3. ������ ����
    //      using (var stream = new MemoryStream(_memory_command._buffer))
    //      {
    //      	using (var writer = new BinaryWriter(stream))
    //      	{
    //      		writer.Write(some_data1);
    //      		writer.Write(some_data2);
    //      	}
    //      }
    //      _memory_command.WriteToSharedMemory();		
    //
    //  4. ������ �б�
    //      _memory_base.ReadFromSharedMemory();
    //      using (var stream = new MemoryStream(_memory_base._buffer))
    //      {
    //      	using (var reader = new BinaryReader(stream))
    //      	{
    //      		var some_data1 = reader.ReadInt32();
    //      		var some_data2 = reader.ReadInt32();
    //          }
    //      }
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JSharedMemory : IDisposable
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ����
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [����] Base
        public MemoryMappedFile _map_file;
        public byte[] _buffer;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [����] Property
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int Size
        {
            get { return (_buffer != null) ? _buffer.Length : 0; }
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ���� �Լ�
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [����] Create (Name, Size)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JSharedMemory Create(string name, int size, bool log = true)
        {
            var sm = new JSharedMemory();

            try
            {
                var map_file = MemoryMappedFile.CreateNew(name, size, MemoryMappedFileAccess.ReadWrite);
                sm._map_file = map_file;
            }
            catch (Exception e)
            {
                if(log)
                    Console.WriteLine("[Error]" + e.Message);
            }

            sm._buffer = new byte[size];
            return sm;
        }
        #endregion

        #region [����] Open (Name, Size)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JSharedMemory Open(string name, int size, bool log = true, int count = 0)
        {            
            try
            {
                var map_file = MemoryMappedFile.OpenExisting(name, MemoryMappedFileRights.ReadWrite);
                var sm = new JSharedMemory();
                sm._buffer = new byte[size];
                sm._map_file = map_file;
                return sm;
            }
            catch (Exception e)
            {
                if (log)
                    Console.WriteLine("[Error]" + e.Message);
            }
            return null;
        }
        #endregion

        #region [����] Dispose
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            if (_map_file != null)
                _map_file.Dispose();
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ���ۺ���
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [����] [����] Buffer -> SharedMemory
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void WriteToSharedMemory()
        {
            using (var accessor = _map_file.CreateViewAccessor())
            {
                accessor.WriteArray(0, _buffer, 0, _buffer.Length);
            }
        }
        #endregion
        
        #region [�б�] [����] SharedMemory -> Buffer
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ReadFromSharedMemory(int length = 0)
        {
            using (var view = _map_file.CreateViewStream())
            {
                view.Position = 0;
                view.Read(_buffer, 0, (length > 0) ? length : _buffer.Length);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ReadFromSharedMemory(int startIndex, int length)
        {
            using (var view = _map_file.CreateViewStream())
            {
                view.Position = startIndex;
                view.Read(_buffer, startIndex, length);
            }
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // IO
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [����] Write (int)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void WriteTo(int startIndex, int value)
        {
            using (var accessor = _map_file.CreateViewAccessor())
            {
                accessor.Write(startIndex, value);
            }
        }
        #endregion


        #region [�б�] Read (int)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int ReadInt32(int startIndex)
        {
            ReadFromSharedMemory(startIndex, 4);          // ������ �ϸ� �տ��� ���� �������� ��찡 �ִ�.
            int result = BitConverter.ToInt32(_buffer, startIndex);
            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int ReadInt32(byte[] bytes, int startIndex)
        {
            int result = 0;
            result |= (int)((bytes[startIndex + 0] << 0) & 0xFF);
            result |= (int)((bytes[startIndex + 1] << 8) & 0xFF);
            result |= (int)((bytes[startIndex + 2] << 16) & 0xFF);
            result |= (int)((bytes[startIndex + 3] << 24) & 0xFF);
            return result;
        }
        #endregion


        #region [�б�] Read (int[])
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int[] ReadInt32s(int count)
        {
            int size = sizeof(int) * count;
            ReadFromSharedMemory(0, size);

            return ReadInt32s(_buffer, 0, count);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int[] ReadInt32s(byte[] buffer, int start, int count)
        {
            int[] results = new int[count];

            for (int i = 0; i < count; ++i)
            {
                int startIndex = start + i * sizeof(int);
                //results[i] = readInt(buffer, startIndex);
                results[i] = BitConverter.ToInt32(buffer, startIndex);
            }
            return results;
        }
        #endregion

        #region [�б�] Read (float[])
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public float[] ReadFloats(int count)
        {
            int size = sizeof(float) * count;
            ReadFromSharedMemory(0, size);

            return ReadFloats(_buffer, 0, count);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public float[] ReadFloats(byte[] buffer, int start, int count)
        {
            float[] results = new float[count];

            for (int i = 0; i < count; ++i)
            {
                int startIndex = start + i * sizeof(float);
                results[i] = BitConverter.ToSingle(buffer, startIndex);
            }
            return results;
        }
        #endregion

        #region [�б�] Read (double[])
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public double[] ReadDoubles(byte[] buffer, int start, int count)
        {
            double[] results = new double[count];

            for (int i = 0; i < count; ++i)
            {
                int startIndex = start + i * sizeof(double);
                results[i] = BitConverter.ToDouble(buffer, startIndex);
            }
            return results;
        }
        #endregion        

    }

}

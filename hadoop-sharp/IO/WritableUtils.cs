using System;
using System.IO;
using System.Text;

namespace Hadoop.IO
{
    public static class WritableUtils
    {
        private static Encoding encoding = new UTF8Encoding();

        public static long ReadVLong(Stream stream)
        {
            sbyte firstByte = (sbyte)stream.ReadByte();
            int len = DecodeVIntSize(firstByte);
            if (len == 1)
            {
                return firstByte;
            }
            long i = 0;
            for (int idx = 0; idx < len - 1; idx++)
            {
                sbyte b = (sbyte)stream.ReadByte();
                i = i << 8;
                i = i | (b & 0xFF);
            }
            return (IsNegativeVInt(firstByte) ? (i ^ -1L) : i);
        }

        public static bool IsNegativeVInt(sbyte b)
        {
            return b < -120 || (b >= -112 && b < 0);
        }

        public static int DecodeVIntSize(sbyte i)
        {
            if (i >= -112)
            {
                return 1;
            }
            else if (i < -120)
            {
                return -119 - i;
            }
            return -111 - i;
        }

        public static int ReadVInt(Stream stream)
        {
            return (int)ReadVLong(stream);
        }

        public static string ReadString(Stream stream)
        {
            return encoding.GetString(ReadBytes(stream));
        }

        public static byte[] ReadBytes(Stream stream)
        {
            int length = ReadVInt(stream);
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return buffer;
        }

        public static void WriteString(Stream stream, string s)
        {
            WriteBytes(stream, encoding.GetBytes(s));
        }

        public static void WriteBytes(Stream stream, byte[] bytes)
        {
            stream.WriteByte((byte)bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void WriteLong(Stream stream, long v)
        {
            byte[] buffer = new byte[8];
            buffer[0] = (byte)(v >> 56);
            buffer[1] = (byte)(v >> 48);
            buffer[2] = (byte)(v >> 40);
            buffer[3] = (byte)(v >> 32);
            buffer[4] = (byte)(v >> 24);
            buffer[5] = (byte)(v >> 16);
            buffer[6] = (byte)(v >> 8);
            buffer[7] = (byte)(v >> 0);
            stream.Write(buffer, 0, 8);
        }

        public static long ReadLong(Stream stream)
        {
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, buffer.Length);

            return (((long)buffer[0] << 56) +
                   ((long)(buffer[1] & 255) << 48) +
                   ((long)(buffer[2] & 255) << 40) +
                   ((long)(buffer[3] & 255) << 32) +
                   ((long)(buffer[4] & 255) << 24) +
                   ((buffer[5] & 255) << 16) +
                   ((buffer[6] & 255) << 8) +
                   ((buffer[7] & 255) << 0));
        }

        public static void WriteVLong(Stream stream, long i)
        {
            if (i >= -112 && i <= 127)
            {
                stream.WriteByte((byte)i);
                return;
            }

            int len = -112;
            if (i < 0)
            {
                i ^= -1L;
                len = -120;
            }

            long tmp = i;
            while (tmp != 0)
            {
                tmp = tmp >> 8;
                len--;
            }

            stream.WriteByte((byte)len);

            len = (len < -120) ? -(len + 120) : -(len + 112);

            for (int idx = len; idx != 0; idx--)
            {
                int shiftbits = (idx - 1) * 8;
                long mask = 0xFFL << shiftbits;
                stream.WriteByte((byte)((i & mask) >> shiftbits));
            }
        }

        public static void WriteVInt(Stream stream, long i)
        {
            WriteVLong(stream, (long)i);
        }

        public static object Read<T>(byte[] data)
        {
            Type type = typeof(T);
            MemoryStream buffer = new MemoryStream(data);

            if (type == typeof(string))
            {
                return encoding.GetString(data);
            }
            else if (type == typeof(long))
            {
                return ReadLong(buffer);
            }
			
            return default(T);
        }
    }
}

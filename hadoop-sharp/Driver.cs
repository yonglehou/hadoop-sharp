using System;
using System.Net;
using System.Net.Sockets;

using Hadoop.IO;
using Hadoop.Pipes;

namespace Hadoop
{
    public class Driver
    {
        public static void RunTask(Factory factory)
        {
            TaskContextImpl context = new TaskContextImpl(factory);

            Protocol connection;

            string port = Environment.GetEnvironmentVariable("hadoop.pipes.command.port");

            if (port != null)
            {
                TcpClient client = new TcpClient("localhost", Convert.ToInt32(port));
                NetworkStream stream = client.GetStream();
                connection = new BinaryProtocol(stream, context, stream);
            }
            else
            {
                throw new Exception("No pipes command port variable found");
            }

            context.SetProtocol(connection, connection.Uplink);
            context.WaitForTask();

            while (!context.Done)
            {
                context.NextKey();
            }

            context.Close();

            connection.Uplink.Done();
        }
    }
}

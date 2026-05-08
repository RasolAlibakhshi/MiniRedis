using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System;

class Program
{
    static async Task Main()
    {
        var listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();

        Console.WriteLine("MiniRedis started on port 5000...");
        Console.WriteLine("");
        Console.WriteLine("SET => set name rasol");
        Console.WriteLine("GET => get name");
        Console.WriteLine("DEL => del name");
        Console.WriteLine("KEYS=>keys \n" +
                          "show all key");
        Console.WriteLine("EXIT=>close app");
        Console.WriteLine("");



        var dbString = new ConcurrentDictionary<string, string>();

        // while (true)
        // {
        //     var client = await listener.AcceptTcpClientAsync();
        //     Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);
        //     _ =HandleClientAsync(client, dbString);
        // }


        var cts = new CancellationTokenSource();

        var acceptTask = AcceptLoopAsync(listener, cts.Token, dbString);

        
        Console.ReadLine();
        cts.Cancel();
        listener.Stop(); 

        await acceptTask;
    }



    static async Task AcceptLoopAsync(
        TcpListener listener,
        CancellationToken token,
        ConcurrentDictionary<string, string> db)
    {
        while (!token.IsCancellationRequested)
        {
            TcpClient client;
            try
            {
                client = await listener.AcceptTcpClientAsync();
            }
            catch (ObjectDisposedException) when (token.IsCancellationRequested)
            {
                break;
            }
            catch (SocketException) when (token.IsCancellationRequested)
            {
                break;
            }

            Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);

            _ = HandleClientAsync(client, db); 
        }
    }


    static async Task HandleClientAsync(TcpClient client, ConcurrentDictionary<string, string> db)
    {
        using (client)
        using (var stream = client.GetStream())
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
        {
            await writer.WriteLineAsync("Welcome to MiniRedis");

            while (true)
            {
                string line = null;
                try
                {
                    line = await reader.ReadLineAsync();
                }
                catch
                {
                    break;
                }

                if (line == null)
                    break;
                
                line = line.Trim();
                if (line.Length == 0)
                    continue;
                
                var parts = line.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                var command = parts[0].ToUpperInvariant();

                switch (command)
                {
                    case "SET":
                        if (parts.Length < 3)
                        {
                            await writer.WriteLineAsync("ERR usage: SET key value");
                            break;
                        }

                        db[parts[1]] = parts[2];
                        await writer.WriteLineAsync("OK");
                        break;

                    case "GET":
                        if (parts.Length < 2)
                        {
                            await writer.WriteLineAsync("ERR usage: GET key");
                            break;
                        }

                        if (db.TryGetValue(parts[1], out var value))
                            await writer.WriteLineAsync(value);
                        else
                            await writer.WriteLineAsync("(nil)");
                        break;

                    case "DEL":
                        if (parts.Length < 2)
                        {
                            await writer.WriteLineAsync("ERR usage: DEL key");
                            break;
                        }

                        if (db.TryRemove(parts[1], out _))
                            await writer.WriteLineAsync("1");
                        else
                            await writer.WriteLineAsync("0");
                        break;

                    case "KEYS":
                        await writer.WriteLineAsync(string.Join(", ", db.Keys));
                        break;

                    case "EXIT":
                        await writer.WriteLineAsync("Bye");
                        return;

                    default:
                        await writer.WriteLineAsync("ERR unknown command");
                        break;
                }
            }
        }
    }
}
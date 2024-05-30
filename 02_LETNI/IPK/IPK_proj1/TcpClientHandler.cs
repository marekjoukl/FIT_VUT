using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPK_proj1
{
    public enum MessageType
    {
        ERR,
        REPLY,
        AUTH,
        JOIN,
        MSG,
        BYE
    }
    public class TcpClientHandler
    {
        private StreamWriter? _writer;
        private Socket? _socket;
        private StreamReader? _reader;
        private NetworkStream? _stream;
        private string _displayName = "";
        private CancellationTokenSource _cts = new CancellationTokenSource();
        
        public TcpClientHandler()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(this.OnCancelKeyPress);
        }
        
        private async void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            if (_writer != null)
            {
                await _writer.WriteLineAsync("BYE\r");
            }
            await _cts.CancelAsync();
            await TerminateConnection();
            e.Cancel = false;
        }

        public async Task OpenTcpConnection(IPAddress ipAddress, int port)
        {
            try
            {   
                // Create socket and connect to the server
                _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
                await _socket.ConnectAsync(endPoint);
                
                _stream = new NetworkStream(_socket);

                // Create a StreamWriter to write data to the stream
                _writer = new StreamWriter(_stream) { AutoFlush = true };

                // Create a StreamReader to read data from the stream
                _reader = new StreamReader(_stream);
                if (_reader == null)
                {
                    await Console.Error.WriteLineAsync("Error creating StreamReader");
                }
                
                await Authenticate(_cts.Token);
                
                await Task.WhenAll(ReceiveMessagesFromServer(_cts.Token), HandleUserInput(false, _cts.Token));
                
                await _cts.CancelAsync();
                await TerminateConnection();
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync("Error: " + e.Message);
                await TerminateConnection();
                Environment.Exit(0);
            }
        }

        private async Task Authenticate(CancellationToken ct)
        {
            try
            {
                // Send the authentication message to the server
                await HandleUserInput(true, _cts.Token);
                
                while (true)
                {
                    ct.ThrowIfCancellationRequested();
                    if (_reader == null) continue;
                    string receivedMessage = await _reader.ReadLineAsync(ct) ?? throw new InvalidOperationException();
                    if (receivedMessage.StartsWith("REPLY"))
                    {
                        string[] parts = receivedMessage.Split(' ');
                        string permission = parts[1];
                        string msg = string.Join(' ', parts[3..]);
                        if (permission == "OK")
                        {
                            await Console.Error.WriteLineAsync("Success: " + msg);
                            break;
                        }
                        if (permission == "NOK")
                        {
                            await Console.Error.WriteLineAsync("Failure: " + msg);
                            await HandleUserInput(true, _cts.Token);
                        }
                    } else if (receivedMessage.StartsWith(MessageType.ERR.ToString()))
                    {
                        string[] parts = receivedMessage.Split(' ');
                        string name = parts[2];
                        string message = string.Join(" ", parts[4..]);
                        await Console.Error.WriteLineAsync("ERR FROM " + name + ": " + message);
                        // Terminate the connection when an ERR message is received
                        await SendMessageToServer("BYE\r").WaitAsync(ct);
                        ct.ThrowIfCancellationRequested();
                        await _cts.CancelAsync();
                        break; // Exit the loop
                    }
                }
            }
            catch (OperationCanceledException)
            {
                await _cts.CancelAsync();
                await TerminateConnection();
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync("Error performing authentication: " + e.Message);
                await TerminateConnection();
                Environment.Exit(0);
            }
        }

        private async Task SendMessageToServer(string message)
        {
            try
            {
                if (_writer != null) await _writer.WriteLineAsync(message);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync("Error sending message to server: " + e.Message);
                await TerminateConnection();
                Environment.Exit(0);
            }
        }

        private async Task HandleUserInput(bool isAuthMsg, CancellationToken ct)
        {
            try
            {
                while (true) {
                    ct.ThrowIfCancellationRequested();
                    string? userInput = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(userInput))
                    {
                        if (userInput == null)
                        {
                            await SendMessageToServer("BYE\r").WaitAsync(ct);
                            await _cts.CancelAsync();
                            await TerminateConnection();
                        }
                    }
                    if (userInput != null)
                    {
                        if (userInput.StartsWith("/join") && !isAuthMsg)
                        {
                            string[] parts = userInput.Split(' ');
                            if (parts.Length == 2)
                            {
                                string channel = parts[1];
                                if (Regex.IsMatch(channel, @"^[a-zA-Z0-9\.\-]{1,20}$"))
                                {
                                    string joinMessage = $"JOIN {channel} AS {_displayName}\r";
                                    await SendMessageToServer(joinMessage);
                                    continue;
                                }
                            }
                            await Console.Error.WriteLineAsync("ERR: Invalid format for /join command. Usage: /join channel\r");
                        }
                        else if (userInput.StartsWith("/rename") && !isAuthMsg)
                        {
                            string[] parts = userInput.Split(' ');
                            if (parts.Length == 2)
                            {
                                string newDisplayName = parts[1];
                                _displayName = newDisplayName;
                                continue;
                            }
                            await Console.Error.WriteLineAsync("ERR: Invalid format for /rename command. Usage: /rename new_display_name\r");
                        }   
                        else if (userInput.StartsWith("/auth")  && isAuthMsg)
                        {
                            string[] parts = userInput.Split(' ');
                            if (parts.Length == 4)
                            {
                                string username = parts[1];
                                string secret = parts[2];
                                _displayName = parts[3];
                                if (ValidateUserData(username,secret,_displayName))
                                {
                                    string authMessage = $"AUTH {username} AS {_displayName} USING {secret}\r";
                                    await SendMessageToServer(authMessage).WaitAsync(ct);
                                    break;
                                }
                            }
                            await Console.Error.WriteLineAsync("ERR: Invalid format for /auth command. Usage: /auth username secret display_name\r");
                        }
                        else if (userInput.StartsWith("/help") && !isAuthMsg)
                        {
                            Console.WriteLine("Available commands:");
                            Console.WriteLine("/join ChannelID - join a channel");
                            Console.WriteLine("/auth username secret display_name - authenticate with the server");
                            Console.WriteLine("/bye - disconnect from the server");
                            Console.WriteLine("/rename new_display_name - change your display name");
                        }
                        else if (!isAuthMsg)
                        {
                            if (!userInput.StartsWith("/"))
                            {
                                string message = $"MSG FROM {_displayName} IS {userInput}\r";
                                await SendMessageToServer(message);
                            }
                            else
                            {
                                await Console.Error.WriteLineAsync("ERR: Invalid command. Type /help for a list of available commands.\r");
                            }
                        }
                        else if(!isAuthMsg && userInput.StartsWith("/auth"))
                        {
                            await Console.Error.WriteLineAsync("ERR: You are already authenticated");
                        }
                        else
                        {
                            await Console.Error.WriteLineAsync("ERR: You must authenticate first. Usage: /auth username secret display_name\r");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                await _cts.CancelAsync();
                await TerminateConnection();
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync("Error handling user input: " + e.Message);
                await TerminateConnection();
                Environment.Exit(0);
            }
        }

        private async Task ReceiveMessagesFromServer(CancellationToken ct)
        {
            try
            {
                while(true)
                {
                    ct.ThrowIfCancellationRequested();
                    if (_reader == null) continue;
                    string receivedMessage = await _reader.ReadLineAsync(ct) ?? throw new InvalidOperationException();
                    if (receivedMessage.StartsWith(MessageType.MSG.ToString()))
                    {
                        string[] parts = receivedMessage.Split(' ');
                        string name = parts[2];
                        string message = string.Join(" ", parts[4..]);
                        Console.WriteLine(name + ": " + message);
                    }
                    else if (receivedMessage.StartsWith(MessageType.ERR.ToString()))
                    {
                        string[] parts = receivedMessage.Split(' ');
                        string name = parts[2];
                        string message = string.Join(" ", parts[4..]);
                        await Console.Error.WriteLineAsync("ERR FROM " + name + ": " + message);
                        // Terminate the connection when an ERR message is received
                        await SendMessageToServer("BYE\r").WaitAsync(ct);
                        ct.ThrowIfCancellationRequested();
                        await _cts.CancelAsync();
                        Environment.Exit(1);
                    }
                    else if (receivedMessage.StartsWith(MessageType.BYE.ToString()))
                    {
                        ct.ThrowIfCancellationRequested();
                        await _cts.CancelAsync();
                    }
                    else if (receivedMessage.StartsWith(MessageType.REPLY.ToString()))
                    {
                        string[] parts = receivedMessage.Split(' ');
                        string permission = parts[1];
                        string msg = string.Join(' ', parts[3..]);
                        if (permission == "OK")
                        {
                            await Console.Error.WriteLineAsync("Success: " + msg);
                        }
                        else if (permission == "NOK")
                        {
                            await Console.Error.WriteLineAsync("Failure: " + msg);
                        }
                        else
                        {
                            await Console.Error.WriteLineAsync("ERR: Invalid response from server\r");
                        }
                    }
                    else
                    {
                        await Console.Error.WriteLineAsync("ERR: Not recognized message: " + receivedMessage + "\r");
                        await SendMessageToServer($"ERR FROM {_displayName} IS Not recognised message\r").WaitAsync(ct);
                        await _cts.CancelAsync();
                        await TerminateConnection();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                await _cts.CancelAsync();
                await TerminateConnection();
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync("Error receiving message from server: " + e.Message);
                await TerminateConnection();
                Environment.Exit(0);
            }
        }

        private async Task TerminateConnection()
        {
            try
            {
                // Close the StreamWriter
                if (_writer != null)
                {
                    await _writer.DisposeAsync();
                    _writer = null;
                }

                // Close the StreamReader
                if (_reader != null)
                {
                    _reader.Dispose();
                    _reader = null;
                }

                // Close the NetworkStream
                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }

                // Close the Socket
                if (_socket != null)
                {
                    _socket.Dispose();
                    _socket = null;
                }
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync("Error terminating connection: " + e.Message);
                await TerminateConnection();
                Environment.Exit(0);
            }
        }
        
        static bool ValidateUserData(string username, string secret, string displayName)
        {
            // Regular expressions for validation
            string usernamePattern = @"^[a-zA-Z0-9\-]{1,20}$";
            string secretPattern = @"^[a-zA-Z0-9\-]{1,128}$";
            string displayNamePattern = @"^[\x21-\x7E]{1,20}$";

            // Validate each parameter against its corresponding pattern
            if (!Regex.IsMatch(username, usernamePattern))
            {
                Console.Error.WriteLine("Invalid username format.");
                return false;
            }

            if (!Regex.IsMatch(secret, secretPattern))
            {
                Console.Error.WriteLine("Invalid secret format.");
                return false;
            }

            if (!Regex.IsMatch(displayName, displayNamePattern))
            {
                Console.Error.WriteLine("Invalid display name format.");
                return false;
            }
            return true;
        }
    }
}

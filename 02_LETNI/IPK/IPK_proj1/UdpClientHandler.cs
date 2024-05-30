using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.BitConverter;
using static System.Buffers.Binary.BinaryPrimitives;

namespace IPK_proj1;

public class UdpClientHandler
{
    private Socket? _socket;
    private IPEndPoint? _endPoint;
    private string _displayName = "";
    private ushort _messageId = 0;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    
    public UdpClientHandler()
    {
        Console.CancelKeyPress += new ConsoleCancelEventHandler(this.OnCancelKeyPress);
    }
    private async void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        if (_endPoint != null)
            _socket?.SendTo(new byte[] { 0xFF }.Concat(GetBytes(ReverseEndianness(_messageId))).ToArray(), 0, 3,
                SocketFlags.None, _endPoint);
        _messageId++;
        await _cts.CancelAsync();
        _socket?.Close();
        e.Cancel = false;
    }
    public async Task OpenUdpConnection(IPAddress ipAddress, int port, int retryCount, UInt16 timeout)
    { 
        _socket = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        _endPoint = new IPEndPoint(ipAddress, port);
        
        await Authenticate(retryCount, timeout, _cts.Token);
        
        await Task.WhenAll(ReceiveMessagesFromServer(_cts.Token), SendUserInputToServer(_cts.Token));
        
        _socket.Close();
    }

    private async Task Authenticate(int retryCount, UInt16 timeout, CancellationToken ct)
    {
        try
        {
            string authMessageContent;
            ushort authMessageId;
            byte[] recBuffer;
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                string? userInput = Console.ReadLine();
                if (userInput != null)
                {
                    if (userInput.StartsWith("/auth"))
                    {
                        string[] parts = userInput.Split(" ");
                        if (parts.Length != 4)
                        {
                            await Console.Error.WriteLineAsync("Invalid /auth command. Usage: /auth {Username} {Secret} {DisplayName}");
                            continue;
                        }
                        authMessageContent = string.Join(" ", parts[1..]);
                        authMessageId = _messageId;
                        await SendMessage("AUTH", authMessageContent);
                        
                        // auth message sent, receive messages from server
            
                        recBuffer = new byte[1024];
                        // Recieve the confirmation message from the server
                        if (_endPoint == null)
                        {
                            continue;
                        }

                        if (_socket == null) continue;
                        await _socket.ReceiveFromAsync(new ArraySegment<byte>(recBuffer), SocketFlags.None,
                            _endPoint);
                        if (recBuffer[0] == 0x00)
                        {
                            // converts the message ID from the server to a short
                            ushort messageId = ToUInt16(recBuffer, 1);
                            if (messageId != authMessageId)
                            {
                                await Console.Error.WriteLineAsync("ERR: Confirm message ID is not 0");
                            }
                        }
                        else
                        {
                            await Console.Error.WriteLineAsync(
                                "ERR: Received message is not a confirmation message.");
                        }

                        // Receive the reply message from the server
                        recBuffer = new byte[1024];
                        await _socket.ReceiveFromAsync(new ArraySegment<byte>(recBuffer), SocketFlags.None,
                            _endPoint);
                        ushort messageId2 = ReverseEndianness(ToUInt16(recBuffer, 1));

                        if (recBuffer[0] == 0xFE && messageId2 == authMessageId)
                        {
                            int startIndex = 3;
                            int endIndex = Array.IndexOf(recBuffer, (byte)0, startIndex);

                            string displayName = Encoding.UTF8.GetString(recBuffer[startIndex..endIndex]);

                            int startIndex2 = endIndex + 1;
                            int endIndex2 = Array.IndexOf(recBuffer, (byte)0, startIndex2);
                            string errorMessage = Encoding.UTF8.GetString(recBuffer[startIndex2..endIndex2]);

                            await Console.Error.WriteLineAsync($"ERR FROM {displayName}: {errorMessage}");
                            if (_endPoint != null)
                                _socket.SendTo(
                                    new byte[] { 0x00 }.Concat(GetBytes(ReverseEndianness(messageId2))).ToArray(),
                                    0, 3, SocketFlags.None, _endPoint);
                            if (_endPoint != null)
                                _socket?.SendTo(
                                    new byte[] { 0xFF }.Concat(GetBytes(ReverseEndianness(_messageId))).ToArray(),
                                    0, 3, SocketFlags.None, _endPoint);
                            ct.ThrowIfCancellationRequested();
                            await _cts.CancelAsync();
                        }
                        // If the reply message is received
                        else if (recBuffer[0] == 0x01 && messageId2 == authMessageId)
                        {
                            int result = recBuffer[3];
                            string messageContent = Encoding.UTF8.GetString(recBuffer[6..]);
                            if (result == 0)
                            {
                                await Console.Error.WriteLineAsync($"Failure: " + messageContent);
                            }
                            else if (result == 1)
                            {
                                await Console.Error.WriteLineAsync($"Success: {messageContent}");
                                if (_endPoint != null)
                                    _socket.SendTo(
                                        new byte[] { 0x00 }.Concat(GetBytes(ReverseEndianness(messageId2)))
                                            .ToArray(), 0, 3, SocketFlags.None, _endPoint);
                                break;
                            }
                            else
                            {
                                await Console.Error.WriteLineAsync("ERR: Invalid reply message result.");
                            }

                            if (_endPoint != null)
                                _socket.SendTo(
                                    new byte[] { 0x00 }.Concat(GetBytes(ReverseEndianness(messageId2))).ToArray(),
                                    0, 3, SocketFlags.None, _endPoint);
                        }
                        else
                        {
                            int startIndex = 3;
                            int endIndex = Array.IndexOf(recBuffer, (byte)0, startIndex);

                            string displayName = Encoding.UTF8.GetString(recBuffer[startIndex..endIndex]);

                            int startIndex2 = endIndex + 1;
                            int endIndex2 = Array.IndexOf(recBuffer, (byte)0, startIndex2);
                            string errorMessage = Encoding.UTF8.GetString(recBuffer[startIndex2..endIndex2]);

                            await Console.Error.WriteLineAsync($"ERR FROM {displayName}: {errorMessage}");
                            if (_endPoint != null)
                                _socket.SendTo(
                                    new byte[] { 0x00 }.Concat(GetBytes(ReverseEndianness(messageId2))).ToArray(),
                                    0, 3, SocketFlags.None, _endPoint);
                            if (_endPoint != null)
                                _socket?.SendTo(
                                    new byte[] { 0xFF }.Concat(GetBytes(ReverseEndianness(_messageId))).ToArray(),
                                    0, 3, SocketFlags.None, _endPoint);
                            ct.ThrowIfCancellationRequested();
                            await _cts.CancelAsync();
                        }
                    }
                    else if (userInput.StartsWith("/help"))
                    {
                        Console.WriteLine("Supported commands:");
                        Console.WriteLine("/auth {Username} {Secret} {DisplayName} - Sends AUTH message with the data provided from the command to the server");
                        Console.WriteLine("/join {ChannelID} - Sends JOIN message with channel name from the command to the server");
                        Console.WriteLine("/rename {DisplayName} - Locally changes the display name of the user to be sent with new messages/selected commands");
                        Console.WriteLine("/help - Prints out supported local commands with their parameters and a description");
                    }
                    else
                    {
                        await Console.Error.WriteLineAsync("ERR: You must authenticate first. Usage: /auth {Username} {Secret} {DisplayName}");
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            await _cts.CancelAsync();
            _socket?.Close();
            Environment.Exit(0);
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR:" + e);
            throw;
        }
    }
    
    private async Task SendUserInputToServer(CancellationToken ct)
    {
        try
        {
            Console.WriteLine("XXXXXXXX");
            string? userInput;
            while ((userInput = Console.ReadLine()) != null)
            {
                ct.ThrowIfCancellationRequested();
                string[] parts = userInput.Split(" ");
                string messageType = parts[0];
                if (messageType.StartsWith("/"))
                {
                    switch (messageType)
                    {
                        case "/auth":
                            await Console.Error.WriteLineAsync("ERR: Already authenticated.");
                            break;

                        case "/join":
                            if (parts.Length != 2)
                            {
                                Console.WriteLine("Invalid /join command. Usage: /join {ChannelID}");
                                continue;
                            }
                            await SendMessage("JOIN", parts[1]);
                            break;

                        case "/rename":
                            if (parts.Length != 2)
                            {
                                Console.WriteLine("Invalid /rename command. Usage: /rename {DisplayName}");
                                continue;
                            }
                            // Locally changes the display name of the user
                            _displayName = parts[1];
                            break;

                        case "/help":
                            Console.WriteLine("Supported commands:");
                            Console.WriteLine("/auth {Username} {Secret} {DisplayName} - Sends AUTH message with the data provided from the command to the server");
                            Console.WriteLine("/join {ChannelID} - Sends JOIN message with channel name from the command to the server");
                            Console.WriteLine("/rename {DisplayName} - Locally changes the display name of the user to be sent with new messages/selected commands");
                            Console.WriteLine("/help - Prints out supported local commands with their parameters and a description");
                            break;

                        default:
                            Console.WriteLine("ERR: Invalid command. Type /help for a list of supported commands.");
                            break;
                    }
                }
                else
                {
                    await SendMessage("MSG", userInput);
                }
            }
        }
        catch (OperationCanceledException)
        {
            await _cts.CancelAsync();
            _socket?.Close();
            Environment.Exit(0);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    private async Task SendMessage(string messageType, string messageContent)
    {
        try
        {
            byte[] messageBytes = ConstructMessage(messageType, messageContent);
            if (_endPoint != null) _socket?.SendTo(messageBytes, 0, messageBytes.Length, SocketFlags.None, _endPoint);
            _messageId++;
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Error sending message: {e.Message}");
        }
    }
    private byte[] ConstructMessage(string messageType, string messageContent)
    {
        byte[] messageBytes = [];
        byte[] messageIdBytes = GetBytes(_messageId);
        int currentIndex = 3;
        byte[] nameBytes = Encoding.UTF8.GetBytes(_displayName);
        byte[] msgBytes;
        
        // Map the message type to the corresponding field value
        switch (messageType)
        {
            case "CONFIRM":
                // messageBytes[0] = 0x00;
                break;
            case "AUTH":
                string[] parts = messageContent.Split(" ");
                string username = parts[0];
                string displayName = parts[1];
                _displayName = parts[2];
                string secret = parts[2];
                
                byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
                byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
                byte[] displayNameBytes = Encoding.UTF8.GetBytes(displayName);
                
                messageBytes = new byte[usernameBytes.Length + secretBytes.Length + displayNameBytes.Length + 3 + 3];
        
                //Adds the message ID to the message 
                
                messageBytes[0] = 0x02;
                messageBytes[1] = messageIdBytes[0];
                messageBytes[2] = messageIdBytes[1];
                
                Array.Copy(usernameBytes, 0, messageBytes, currentIndex, usernameBytes.Length);
                currentIndex += usernameBytes.Length;
                messageBytes[currentIndex++] = 0; // zero byte

                Array.Copy(secretBytes, 0, messageBytes, currentIndex, secretBytes.Length);
                currentIndex += secretBytes.Length;
                messageBytes[currentIndex++] = 0; // zero byte

                Array.Copy(displayNameBytes, 0, messageBytes, currentIndex, displayNameBytes.Length);
                currentIndex += displayNameBytes.Length;
                messageBytes[currentIndex] = 0; // zero byte
                break;
            case "JOIN":
                byte[] joinBytes = Encoding.UTF8.GetBytes(messageContent);
                
                messageBytes = new byte[joinBytes.Length + nameBytes.Length + 3 + 2];
                
                messageBytes[0] = 0x03;
                messageBytes[1] = messageIdBytes[0];
                messageBytes[2] = messageIdBytes[1];
                
                Array.Copy(joinBytes, 0, messageBytes, currentIndex, joinBytes.Length);
                currentIndex += joinBytes.Length;
                messageBytes[currentIndex++] = 0; // zero byte
                
                Array.Copy(nameBytes, 0, messageBytes, currentIndex, nameBytes.Length);
                currentIndex += nameBytes.Length;
                messageBytes[currentIndex] = 0; // zero byte
                
                break;
            case "MSG":
                msgBytes = Encoding.UTF8.GetBytes(messageContent);
                messageBytes = new byte[msgBytes.Length + nameBytes.Length + 3 + 2];
                
                messageBytes[0] = 0x04;
                messageBytes[1] = messageIdBytes[0];
                messageBytes[2] = messageIdBytes[1];
                
                Array.Copy(nameBytes, 0, messageBytes, currentIndex, nameBytes.Length);
                currentIndex += nameBytes.Length;
                messageBytes[currentIndex++] = 0; // zero byte
                
                Array.Copy(msgBytes, 0, messageBytes, currentIndex, msgBytes.Length);
                currentIndex += msgBytes.Length;
                messageBytes[currentIndex] = 0; // zero byte
                
                break;
            case "ERR":
                msgBytes = Encoding.UTF8.GetBytes(messageContent);
                
                messageBytes = new byte[msgBytes.Length + nameBytes.Length + 3 + 2];
                messageBytes[0] = 0xFE;
                messageBytes[1] = messageIdBytes[0];
                messageBytes[2] = messageIdBytes[1];
                
                Array.Copy(nameBytes, 0, messageBytes, currentIndex, nameBytes.Length);
                currentIndex += nameBytes.Length;
                messageBytes[currentIndex++] = 0; // zero byte
                
                Array.Copy(msgBytes, 0, messageBytes, currentIndex, msgBytes.Length);
                currentIndex += msgBytes.Length;
                messageBytes[currentIndex] = 0; // zero byte
                
                break;
            case "BYE":
                messageBytes[0] = 0xFF;
                break;
            default:
                throw new ArgumentException("ERR: Invalid message type");
        }
        
        string messageWithNewLine = Encoding.UTF8.GetString(messageBytes) + "\r\n";
        messageBytes = Encoding.UTF8.GetBytes(messageWithNewLine);
        
        return messageBytes;
    }

    private async Task ReceiveMessagesFromServer(CancellationToken ct)
    {
        try
        {
            Console.WriteLine("YYYYYYYY");
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                byte[] recBuffer = new byte[1024];
                if (_endPoint == null)
                {
                    continue;
                }

                if (_socket != null)
                {
                    await _socket.ReceiveFromAsync(new ArraySegment<byte>(recBuffer), SocketFlags.None, _endPoint);
                    ushort messageId2 = ReverseEndianness(ToUInt16(recBuffer, 1));
                    if (recBuffer[0] == 0x00)
                    {
                        // Confirm message
                    }
                    else if (recBuffer[0] == 0x01)
                    {
                        // Reply message
                        int result = recBuffer[3];
                        string messageContent = Encoding.UTF8.GetString(recBuffer[6..]);
                        if (result == 0)
                        {
                            await Console.Error.WriteLineAsync($"Failure: " + messageContent);
                        }
                        else if (result == 1)
                        {
                            await Console.Error.WriteLineAsync($"Success: {messageContent}");
                            if (_endPoint != null)
                                _socket.SendTo(
                                    new byte[] { 0x00 }.Concat(GetBytes(ReverseEndianness(messageId2))).ToArray(), 0, 3,
                                    SocketFlags.None, _endPoint);
                            break;
                        }
                        else
                        {
                            await Console.Error.WriteLineAsync("ERR: Invalid reply message result.");
                        }

                        if (_endPoint != null)
                            _socket.SendTo(
                                new byte[] { 0x00 }.Concat(GetBytes(ReverseEndianness(messageId2))).ToArray(), 0, 3,
                                SocketFlags.None, _endPoint);
                    }
                    else if (recBuffer[0] == 0x04)
                    {
                        // Message message
                        ushort messageId = ReverseEndianness(ToUInt16(recBuffer, 1));
                        int startIndex = 3;
                        int endIndex = Array.IndexOf(recBuffer, (byte)0, startIndex);
                        string displayName = Encoding.UTF8.GetString(recBuffer[startIndex..endIndex]);
                        int startIndex2 = endIndex + 1;
                        int endIndex2 = Array.IndexOf(recBuffer, (byte)0, startIndex2);
                        string content = Encoding.UTF8.GetString(recBuffer[startIndex2..endIndex2]);

                        await Console.Out.WriteLineAsync($"{displayName}: {content}");
                        if (_endPoint != null)
                            _socket.SendTo(new byte[] { 0x00 }.Concat(GetBytes(ReverseEndianness(messageId))).ToArray(),
                                0, 3, SocketFlags.None, _endPoint);
                    }
                    else if (recBuffer[0] == 0xFE)
                    {
                        // Error message
                        int startIndex = 3;
                        int endIndex = Array.IndexOf(recBuffer, (byte)0, startIndex);

                        string displayName = Encoding.UTF8.GetString(recBuffer[startIndex..endIndex]);

                        int startIndex2 = endIndex + 1;
                        int endIndex2 = Array.IndexOf(recBuffer, (byte)0, startIndex2);
                        string errorMessage = Encoding.UTF8.GetString(recBuffer[startIndex2..endIndex2]);

                        await Console.Error.WriteLineAsync($"ERR FROM {displayName}: {errorMessage}");
                        if (_endPoint != null)
                            _socket.SendTo(
                                new byte[] { 0x00 }.Concat(GetBytes(ReverseEndianness(messageId2))).ToArray(), 0, 3,
                                SocketFlags.None, _endPoint);
                        if (_endPoint != null)
                            _socket?.SendTo(
                                new byte[] { 0xFF }.Concat(GetBytes(ReverseEndianness(_messageId))).ToArray(), 0, 3,
                                SocketFlags.None, _endPoint);
                        ct.ThrowIfCancellationRequested();
                        await _cts.CancelAsync();
                    }
                    else if (recBuffer[0] == 0xFF)
                    {
                        // Bye message
                        ct.ThrowIfCancellationRequested();
                        await _cts.CancelAsync();
                    }
                    else
                    {
                        await Console.Error.WriteLineAsync("ERR: Invalid message type");
                        if (_endPoint != null)
                            _socket.SendTo(
                                new byte[] { 0x00 }.Concat(GetBytes(ReverseEndianness(messageId2))).ToArray(), 0, 3,
                                SocketFlags.None, _endPoint);
                        await SendMessage("ERR", "Invalid message type");
                    }
                }
            }
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Error receiving message: {e.Message}");
        }
    }
}
# IPK Project 1 2024 - Client for a chat server
## Autor: Marek Joukl (xjouk00)

## Popis funkcionality
Cílem projektu bylo vytvořit program pro komunikaci mezi klientem a serverem pomocí protokolů UDP (binární) a TCP (textová). Program umožňuje připojení na vzdálený server, autentifikaci, přepínání mezi kanály a zasílání zpráv. Program lze přeložit příkazem `make` a zavoláním příkazu `./ipk24chat-client`. Pro připojení je nutné zavolat program s těmito argumenty: 
- `-t` [tcp / udp]
- `-s` [IP adresa nebo hostname]

Dále je možné upřesnit volání těmito volitelnými argumenty:

- `-p` [port serveru]
- `-d` [specifikace doby pro odeslání potvrzení]
- `-r` [počet znovuodeslání]
- `-h` zobrazí nápovědu

### Použití příkazů
Po navázání spojení může uživatel využít tyto příkazy
- `/auth {Username} {Secret} {DisplayName}`
- `/join {ChannelID}`
- `/rename`
- `/help`

### Binární varianta
Binárni varianta využívá protokol UDP. Zde neprobíhá připojení klienta k serveru a spojení je nespolehlivé. Klient zasílá zprávy ve formátu HLAVIČKA - obsahující typ zprávy a identifikátor, OBSAH - růžní se dle typu zprávy. Při posílání zpráv je nutné potvrzování zprávou typu CONFIRM.

### Textová varianta
Textová varianta využívá protokol TCP. Po navánání spojení je nejdříve nutná autentifikace klienta příkazem AUTH. Po zaslání této zprávy klient čeká na odpověd typu REPLY. Při pozitivní odpovědi je klient připojen k serveru a může začít posílat další příkazy. Má k dispozici příkazy typu MSG pro zaslání zprávy a JOIN pro připojení do kanálu.

## Implementace
Program je psán v jazyce C#. Při spuštění je nejdříve zavolána funkce main v souboru **Program.cs**, která je zodpovědná za zpracování argumentů pomocí funkce **ParseArgs()** ve třídě **ArgsParser()** a dále rozhoduje, zda zavolat **OpenTcpConnection()** ve třídě TcpClientHandler pro TCP variantu nebo **OpenUdpConnection()** ve třídě TcpClientHandler pro UDP variantu.

### TcpClientHandler
Nejdříve se vytvoří spojení inicializováním TCP socketu a vytvořením IPEndPoint pro zadaný port a ip adresu. Následuje vytvoření TCP streamu pro daný socket a vytvoření StreamReaderu a StreamWriteru pro čtení a zápis. Dále je zavolána funkce **Authenticate()**, která očekává autentifikaci od klienta. Ta probíhá, dokud od serveru nejdojde zpráva typu REPLY OK. Po úspěšném ověření může klient posílat i přijímat zprávy díky asynchronnímu běhu. Jakmile je od serveru obdržena zpráva BYE, komunikace se  ukončí. Uživatel může také ukončit komunikaci, a to signálem C-c a následným odesláním BYE. Vždy je zavolána funkce **TerminateConnection()**, která řeší ukončení programu.

### UdpClientHandler

Opět se začíná vytvořením socketu, tentokrát pro UDP a vytvořením IPEndPoint pro zadaný port a ip adresu a je zavolána funkce **Authenticate()**. Ta nejprve vynutí zaslání zprávy AUTH a čeká na CONFIRM a REPLY a posílá CONFIRM zprávu zpět. Při úspěšném ověření začíná komunikace. Zprávy od klienta jsou odesílány ve funkci **SendUserInputToServer()**, která využívá pomocné funkce **ConstructMessage()** pro převedení jednotlivých zpráv k odeslání dle protokolu a **SendMessage()**. Pro příjem je volána funkce **ReceiveMessagesFromServer()**. Funkce pro příjem a odesílání běží asynchronně ve smyčce. Ukončení je zajištěno zavřením socketu.

## Testování

Testování obou variant probíhalo ručně vytvořením lokálního serveru pomocí programu `netcat` a sledováním probíhající konverzace ve `Wireshark`.

using System;
using System.Collections.Generic; 
using System.Threading; 
using Sys = Cosmos.Kernel.System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Cosmos.Kernel.System.Network;
using Cosmos.Kernel.System.Network.Config;
using Cosmos.Kernel.System.Network.DNS;
using Cosmos.Kernel.System.Network.IPv4;
using Cosmos.Kernel.System.Network.IPv4.DHCP;
using Cosmos.Kernel.System.Timer;


namespace SimpleOS;

public class Kernel : Sys.Kernel
{

    private Dictionary<string, string> FileStorage = new Dictionary<string, string>();

    protected override void BeforeRun()
    {
        if (NetworkManager.DeviceCount > 0)
{
    Console.WriteLine("Wifi Info:");    
    Console.WriteLine("Device:  " + NetworkManager.Name);
    Console.WriteLine("MAC:     " + NetworkManager.MacAddress?.ToString());
    Console.WriteLine("Link up: " + NetworkManager.LinkUp);
    Console.WriteLine("Ready:   " + NetworkManager.Ready);
}
DhcpClient dhcpClient = new();

if (dhcpClient.SendDiscoverPacket() != -1)
{
    IPConfig? config = NetworkManager.Primary.IPConfig;
    if (config is not null)
    {
        Console.WriteLine("IP address: " + config.Address.ToString());
        Console.WriteLine("Subnet:     " + config.SubnetMask.ToString());
        Console.WriteLine("Gateway:    " + config.DefaultGateway.ToString());
    }
}
else
{
    Console.WriteLine("DHCP timed out");
}

        Console.WriteLine("SimpleOS booted successfully!");
        Console.WriteLine("Type a command to get it executed.");
    }

    protected override void Run()
    {
        Console.Write("> ");
        var input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
            return;

        switch (input.ToLower())
        {
            case "help":
                Console.WriteLine("Available commands:");
                Console.WriteLine("  help     - Show this help message");
                Console.WriteLine("  clear    - Clear the screen");
                Console.WriteLine("  halt     - Halt the system");
                Console.WriteLine("  hello    - Say Hello");
                Console.WriteLine("  timer    - Run a 10-second timer");
                Console.WriteLine("  version  - Find SimpleOS version");
                Console.WriteLine("  pmcalc   - Basic addition and subtraction calculator");
                Console.WriteLine("  dmcalc   - Basic Multiplication and Division");
                Console.WriteLine("  edit     - Create a text file in RAM");
                Console.WriteLine("  ls       - List all files stored in RAM");
                Console.WriteLine("  open     - Open and read a text file from RAM");
                Console.WriteLine("  rm       - Remove/Delete a text file from RAM");
                Console.WriteLine("  about    - What is this OS");
                Console.WriteLine("  ram      - Figure out the free ram amount.");
                Console.WriteLine("  echo     - Because Every OS needs it");
                Console.WriteLine("  ip       - Figure out IP adress of a website");
                break;

            case "clear":
                Console.Clear();
                break;
            case "ip":
                Console.WriteLine();
                Console.Write("What website?");
                var ip = Console.ReadLine();
                DnsConfig.Add(new Address4(1, 1, 1, 1));   // Cloudflare public DNS

IPAddress[] addresses = Dns.GetHostAddresses(ip);
Console.WriteLine(ip + " resolved to " + addresses[0].ToString());
                break;

            case "echo":
                Console.WriteLine();
                Console.Write("What should I echo?");
                var echo = Console.ReadLine();
                Console.WriteLine(echo);
                break;

            case "ram":
               
                long estimatedFreeRAM = 128 - (FileStorage.Count * 2); 
                Console.WriteLine($"Estimated Available RAM: {estimatedFreeRAM} MB");
                break;

            case "about":
                Console.WriteLine("A simple bad little OS made with COSMOS");
                break;

            case "halt":
                Console.WriteLine("Halting system...");
                Stop();
                break;

            case "hello":
                Console.WriteLine("hello");
                break;

            case "timer":
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("tick " + i);
                    Thread.Sleep(500); 
                }
                break;

            case "version":
                Console.WriteLine("SimpleOS 1.2 Cashew Kernel 3.0");
                Console.WriteLine("   $$$   ");
                Console.WriteLine("  $$$$   ");
                Console.WriteLine("    $$   ");
                Console.WriteLine("    $$   ");
                Console.WriteLine("    $$   ");
                Console.WriteLine("    $$   ");
                Console.WriteLine("    $$   ");
                Console.WriteLine("  $$$$$$ ");
                Console.WriteLine(" $$$$$$$$");
                break;

             case "dmcalc":
                try
                {
                    Console.Write("Enter first number: ");
                    string num1Input = Console.ReadLine();
                    int num1 = int.Parse(num1Input);

                    Console.Write("Enter operator (div or mult): ");
                    string op = Console.ReadLine();

                    Console.Write("Enter second number: ");
                    string num2Input = Console.ReadLine();
                    int num2 = int.Parse(num2Input);

                    if (op == "mult")
                    {
                        int result = num1 * num2;
                        Console.WriteLine($"Result: {num1} * {num2} = {result}");
                    }
                    else if (op == "div")
                    {
                        int result = num1 / num2;
                        Console.WriteLine($"Result: {num1} / {num2} = {result}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid operator! Only 'mult' and 'div' are supported here.");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error: Please enter valid relatively small numbers (and do not divide by zero).");
                }
                break;

            case "pmcalc":
                try
                {
                    Console.Write("Enter first number: ");
                    string num1Input = Console.ReadLine();
                    int num1 = int.Parse(num1Input);

                    Console.Write("Enter operator (plus or minus): ");
                    string op = Console.ReadLine();

                    Console.Write("Enter second number: ");
                    string num2Input = Console.ReadLine();
                    int num2 = int.Parse(num2Input);

                    if (op == "plus")
                    {
                        int result = num1 + num2;
                        Console.WriteLine($"Result: {num1} + {num2} = {result}");
                    }
                    else if (op == "minus")
                    {
                        int result = num1 - num2;
                        Console.WriteLine($"Result: {num1} - {num2} = {result}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid operator! Only 'plus' and 'minus' are supported here.");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error: Please enter valid relatively small numbers.");
                }
                break;

            case "edit":
                Console.Write("Enter filename to create (e.g., notes.txt): ");
                string filename = Console.ReadLine();

                if (string.IsNullOrEmpty(filename))
                {
                    Console.WriteLine("Error: Filename cannot be empty.");
                    break;
                }

                Console.Clear();
                Console.WriteLine($"=== Editing: {filename} ===");
                Console.WriteLine("Type your text below. To save and close, type 'exit' on an empty line.");
                Console.WriteLine("---------------------------------------------------\n");

                string document = "";
                while (true)
                {
                    string line = Console.ReadLine();
                    if (line.Trim().ToLower() == "exit")
                    {
                        break;
                    }
                    document += line + "\n";
                }

                if (FileStorage.ContainsKey(filename))
                {
                    FileStorage[filename] = document;
                }
                else
                {
                    FileStorage.Add(filename, document);
                }

                Console.WriteLine($"\nFile '{filename}' successfully saved to RAM storage.");
                break;

            case "ls":
                Console.WriteLine("=== Files in RAM Storage ===");
                if (FileStorage.Count == 0)
                {
                    Console.WriteLine("(No files found)");
                }
                else
                {
                    foreach (var file in FileStorage.Keys)
                    {
                        Console.WriteLine($"  {file}");
                    }
                }
                Console.WriteLine("----------------------------");
                break;

            case "open":
                Console.Write("Enter filename to open: ");
                string openFile = Console.ReadLine();

                if (FileStorage.ContainsKey(openFile))
                {
                    Console.Clear();
                    Console.WriteLine($"=== Content of: {openFile} ===");
                    Console.WriteLine("---------------------------------------------------");
                    Console.Write(FileStorage[openFile]);
                    Console.WriteLine("---------------------------------------------------");
                }
                else
                {
                    Console.WriteLine($"Error: File '{openFile}' not found in RAM.");
                }
                break;

            case "rm":
                Console.Write("Enter filename to delete: ");
                string deleteFile = Console.ReadLine();

                if (FileStorage.ContainsKey(deleteFile))
                {
                    FileStorage.Remove(deleteFile);
                    Console.WriteLine($"File '{deleteFile}' successfully removed from RAM.");
                }
                else
                {
                    Console.WriteLine($"Error: File '{deleteFile}' not found in RAM.");
                }
                break;

            default:
                Console.WriteLine($"\"{input}\" is not a command, run help for a list of commands");
                break;
        }
    }
}


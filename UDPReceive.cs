using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour {

    public static UDPReceive instance;

	Thread receiveThread;
	UdpClient client;
    
	public int port = 5000;
    public string IP; // = "192.168.1.193";
	
    // infos
	public string lastReceivedUDPPacket="";
	public string allReceivedUDPPackets=""; // clean up this from time to time!

    //public VideoPlayerTest VPT;

    public string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    private void Awake()
    {
        instance = this;

        string host = System.Net.Dns.GetHostName();
        IP = System.Net.Dns.GetHostEntry(host).AddressList[0].ToString();
        IP = LocalIPAddress();
        Debug.Log("Super Brand new IP: " + IP);
    }
    
    public void Start()
    {

        init();
    }
    
    private void init()
    {
        print("UDPSend.init()");

        // define port
        port = 5000;

        // status
        print("Sending to " + IP + " : " + port);
        print("Test-Sending to this Port: nc -u \"" + IP + port + "");

        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void OnDisable()
	{
		if ( receiveThread!= null)
			receiveThread.Abort();

		//client.Close();
	} 

	// start from shell
	private static void Main()
	{
		UDPReceive receiveObj=new UDPReceive();
		receiveObj.init();

		string text="";
		do
		{
			text = Console.ReadLine();
		}
		while(!text.Equals("exit"));
	}

	// OnGUI
	void OnGUI()
	{
		Rect rectObj=new Rect(40,10,200,400);
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		GUI.Box(rectObj,"# UDPReceive\n+"+IP+port+" #\n"
			+ "shell> nc -u "+IP+port+" \n"
			+ "\nLast Packet: \n"+ lastReceivedUDPPacket
			+ "\n\nAll Messages: \n"+allReceivedUDPPackets
			,style);
	}
    
	// receive thread
	private  void ReceiveData()
	{

		client = new UdpClient(port);
		while (true)
		{

			try
			{

				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);

				print(">> " + text);

				lastReceivedUDPPacket = text;

                MainUI.instance.ProcessCommand(lastReceivedUDPPacket);

			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}

	public string getLatestUDPPacket()
	{
		allReceivedUDPPackets="";
		return lastReceivedUDPPacket;
	}
}
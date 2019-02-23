using UnityEngine;
using System.Security.Cryptography;
using System.Numerics;

public class ClientKeyExchange : MonoBehaviour
{
    public int ClientKeyLength = 5; // more than 5 can be hard on system requirements!!
    public static ClientKeyExchange Constructor;
    private int clientkey;
    private BigInteger G, P;

    private string FinallySecureKey;
    private BigInteger _KeyToExchange; 
    public BigInteger KeyToExchange
    {
        get => _KeyToExchange; set
        {
            _KeyToExchange = value;
            PublishToServer();
        }
    }

    void Start()
    {
        Constructor = this;
        int i = CryptographicallyGenerate();
        string value = i.ToString();
        value = value.Substring(1, ClientKeyLength);
        clientkey = int.Parse(value);
    }

    //TODO:rework to eventhandler
    public void ReceivePublishedCommons(BigInteger g, BigInteger p)
    {
        Constructor.G = g;
        Constructor.P = p;
        KeyToExchange = CalcSecretKey(Constructor.G, clientkey, Constructor.P);
    }
    //TODO:rework to eventhandler
    public void ReceiveFromServer(BigInteger bigi)
    {
        string rawholder = CalcSecretKey(bigi, clientkey, P).ToString();
        FinallySecureKey = Sha256(rawholder);
        print("client použiva heslo: " + FinallySecureKey);
    }

    //TODO:rework to sendmessage
    void PublishToServer()
    {
        ServerKeyExchange.Constructor.ReceiveFromClient(KeyToExchange);
    }


    BigInteger CalcSecretKey(BigInteger g, int a, BigInteger p)
    {
        BigInteger bigi = (BigInteger.Pow(g, a) % p);
        return bigi;
    }

    int CryptographicallyGenerate()
    {
        int i;
        var numberGenerator = new RNGCryptoServiceProvider();
        var byteArray = new byte[4];
        numberGenerator.GetBytes(byteArray);
        i = System.BitConverter.ToInt32(byteArray, 0);
        return i;
    }

    static string Sha256(string randomString)
    {
        var crypt = new System.Security.Cryptography.SHA256Managed();
        var hash = new System.Text.StringBuilder();
        byte[] crypto = crypt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(randomString));
        foreach (byte theByte in crypto)
        {
            hash.Append(theByte.ToString("x2"));
        }
        return hash.ToString();
    }
}
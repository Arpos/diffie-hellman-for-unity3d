using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using UnityEngine;

public class ServerKeyExchange : MonoBehaviour
{
    public int ServerKeyLength = 5; // more than 5 can be hard on system requirements!!
    public static ServerKeyExchange Constructor;
    private int serverkey;
    private BigInteger G, P;

    private string FinallySecureKey;
    BigInteger _KeyToExchange;
    public BigInteger KeyToExchange
    {
        get => _KeyToExchange; set
        {
            _KeyToExchange = value;
            PublishToClient();
        }
    }

    void Start()
    {
        Constructor = this;

        int i = CryptographicallyGenerate();
        string value = i.ToString();
        value = value.Substring(1, ServerKeyLength);
        serverkey = int.Parse(value);

        G = CryptographicallyGenerate();
        P = CryptographicallyGenerate();

        PublishCommonToClient();

        KeyToExchange = CalcSecretKey(G, serverkey, P);
    }

    //TODO:rework to eventhandler
    public void ReceiveFromClient(BigInteger bigi)
    {
        string rawholder = CalcSecretKey(bigi, serverkey, P).ToString();
        FinallySecureKey = Sha256(rawholder);
        print("server použiva heslo: " + FinallySecureKey);
    }

    //TODO:rework to sendmessage
    void PublishCommonToClient()
    {
        if (G != 0 && P != 0 && ClientKeyExchange.Constructor != null)
        {
            ClientKeyExchange.Constructor.ReceivePublishedCommons(G, P);
        }
    }

    //TODO:rework to sendmessage
    void PublishToClient()
    {
        ClientKeyExchange.Constructor.ReceiveFromServer(KeyToExchange);
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

using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class LeaderboardHandle : MonoBehaviour
{
    private static string hostAddress = "gwcleaderboard.000webhostapp.com";
    private static string secretKey = sha256_hash("freebyte doesn't know how to use this shit");
    public static bool RegisterAccount(string _username, string _password)
    {
        string _url = $"https://{hostAddress}/register.php?nickname={_username}&password={_password}&key={secretKey}";
        string requestResult = Get(_url);
        bool result = false;
        switch (requestResult)
        {
            case "taken":
                result = false;
                break;
            case "success":
                result = true;
                break;
        }
        return result;
    }
    public static bool LoginAccount(string _username, string _password)
    {
        string _url = $"https://{hostAddress}/login.php?nickname={_username}&password={_password}";
        string requestResult = Get(_url);
        bool result = false;
        switch (requestResult)
        {
            case "success":
                result = true;
                break;
            case "error":
                result = false;
                break;
        }
        return result;
    }
    public static string GetTimesFromServer(string _username, string _password)
    {
        string _url = $"https://{hostAddress}/gettime.php?key={secretKey}&username={_username}&password={_password}";
        return Get(_url);
    }
    public static string hashPassword(string _password)
    {
        return sha256_hash(_password);
    }
    public static string SendTimeToServer(string _username, string _password,
        string _time, string _sec1, string _sec2, string _sec3, int _carEngine, int _level)
    {
        string _json = "{" + $"\"{_level}\":" + "{" + $"\"time\":\"{_time}\",\"sec1\":\"{_sec1}\",\"sec2\":\"{_sec2}\",\"sec3\":\"{_sec3}\",\"engine\":\"{_carEngine}\"" + "}}";
        string _url = $"https://{hostAddress}/save.php?key={secretKey}&username={_username}&password={_password}&json_data={_json}&version=" + Constants.physicsVersion;
        return _url;
    }

    public static string SendDataToServer(string _username, string _password, string _jsonData, int _carColor, int _carModel)
    {

        string _url = $"https://{hostAddress}/save.php?key={secretKey}&username={_username}&password={_password}&json_data={_jsonData}&version=" + Constants.physicsVersion;
        string _urlCar = $"https://{hostAddress}/updatecolorandmodel.php?key={secretKey}&nickname={_username}&password={_password}&color={_carColor}&model={_carModel}";
        string CarUpdateResult = Get(_urlCar);
        return Get(_url) + " " + CarUpdateResult;
    }

    public static string UpdateCar(string _username, string _password, int _color, int _model)
    {
        string _url = $"https://{hostAddress}/updatecolorandmodel.php?key={secretKey}&nickname={_username}&password={_password}&color={_color}&model={_model}";
        return _url;
    }
    public static string UpdatePassword(string _username, string _password, string _newPassword)
    {
        string _url = $"https://{hostAddress}/changepassword.php?key={secretKey}&username={_username}&password={_password}&newpassword={_newPassword}";
        return Get(_url);
    }
    public static String sha256_hash(String value)
    {
        StringBuilder Sb = new StringBuilder();
        using (SHA256 hash = SHA256Managed.Create())
        {
            Encoding enc = Encoding.UTF8;
            Byte[] result = hash.ComputeHash(enc.GetBytes(value));

            foreach (Byte b in result)
                Sb.Append(b.ToString("x2"));
        }
        return Sb.ToString();
    }
    public static string Get(string uri)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

}

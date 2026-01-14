using System;
using System.IO;
using System.Text;
using System.Web.Services.Protocols;

[AttributeUsage(AttributeTargets.Method)]
public class SoapLoggerAttribute : SoapExtensionAttribute
{
    public string FileName { get; set; }

    public override Type ExtensionType => typeof(SoapLogger);

    public override int Priority { get; set; }
}

public class SoapLogger : SoapExtension
{
    private Stream _oldStream;
    private Stream _newStream;
    private string _fileName;

    public override object GetInitializer(Type serviceType)
    {
        // Default-Dateiname, falls nichts angegeben wird
        return "soap.log";
    }

    public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
    {
        return ((SoapLoggerAttribute)attribute).FileName;
    }

    public override void Initialize(object initializer)
    {
        _fileName = (string)initializer;
    }

    public override Stream ChainStream(Stream stream)
    {
        _oldStream = stream;
        _newStream = new MemoryStream();
        return _newStream;
    }

    public override void ProcessMessage(SoapMessage message)
    {
        switch (message.Stage)
        {
            case SoapMessageStage.AfterSerialize:
                // → Request ist in _newStream
                LogRequest();
                break;

            case SoapMessageStage.BeforeDeserialize:
                // → Response ist in _oldStream
                LogResponse();
                break;

            case SoapMessageStage.BeforeSerialize:
            case SoapMessageStage.AfterDeserialize:
                // nichts zu tun
                break;
        }
    }

    private void LogRequest()
    {
        // Request wurde in _newStream geschrieben
        _newStream.Position = 0;

        string soap;
        using (var reader = new StreamReader(_newStream, Encoding.UTF8, true, 1024, true))
        {
            soap = reader.ReadToEnd();
        }

        // in Datei loggen (anhängen, nicht überschreiben)
        File.AppendAllText(
            _fileName,
            $"----- REQUEST {DateTime.Now:O} -----{Environment.NewLine}{soap}{Environment.NewLine}");

        // Request muss weiter zum echten Stream (_oldStream)
        _newStream.Position = 0;
        CopyStream(_newStream, _oldStream);
    }

    private void LogResponse()
    {
        // Response liegt in _oldStream (vom Transport),
        // wir müssen ihn in _newStream kopieren, weil der Deserializer _newStream liest.
        _newStream.SetLength(0);
        _oldStream.Position = 0;
        CopyStream(_oldStream, _newStream);

        _newStream.Position = 0;

        string soap;
        using (var reader = new StreamReader(_newStream, Encoding.UTF8, true, 1024, true))
        {
            soap = reader.ReadToEnd();
        }

        File.AppendAllText(
            _fileName,
            $"----- RESPONSE {DateTime.Now:O} -----{Environment.NewLine}{soap}{Environment.NewLine}");

        // ganz wichtig: wieder auf Anfang setzen,
        // damit der SOAP-Stack den Response aus _newStream deserialisieren kann
        _newStream.Position = 0;
    }

    private static void CopyStream(Stream from, Stream to)
    {
        byte[] buffer = new byte[4096];
        int bytes;
        while ((bytes = from.Read(buffer, 0, buffer.Length)) > 0)
            to.Write(buffer, 0, bytes);
        to.Flush();
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QuickFix;
using QuickFix.DataDictionary;
using Serilog;
using Log = Serilog.Log;


namespace FixAcceptor
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("serilogfile.txt").WriteTo.Console().Enrich.FromLogContext().CreateLogger();

            var doc = new XmlDocument();
            doc.Load("C:/FixGenium.xml");

         
            var settings = new SessionSettings("fixconfig.ini");
            var application = new EmptyQuickfixApp();
            var storeFactory = new MemoryStoreFactory();
            var logFactory = new FileLogFactory(settings);
            var acceptor = new ThreadedSocketAcceptor(application, storeFactory, settings, logFactory);
        
            acceptor.Start();


            Console.WriteLine("Started");

            Console.ReadLine();

        }
    }

    


    public class EmptyQuickfixApp : IApplication
    {
        public void ToAdmin(Message message, SessionID sessionID)
        {
            Log.Information("Sending {message}", message.ToString());
        }

        public void FromAdmin(Message message, SessionID sessionID)
        {
            Log.Information("Got {message}", message.ToString());
        }

        public void ToApp(Message message, SessionID sessionId)
        {
            Log.Information("Sending {message}", message.ToString());
        }

        public void FromApp(Message message, SessionID sessionID)
        {
            Log.Information("Got {message}", message.ToString());
            SendFixMessages(sessionID);

        }

        public void OnCreate(SessionID sessionID)
        {
            Log.Information("Creating session {sessionID}", sessionID);

        }

   

        public void OnLogout(SessionID sessionID)
        {
            Log.Information("Logging out session {sessionID}", sessionID);

        }

        public void OnLogon(SessionID sessionID)
        {
            Log.Information("Logging on session {sessionID}", sessionID);

        }

        private static void SendFixMessages(SessionID sessionId)
        {
            var message = new Message("8=FIX.4.49=00034335=849=GENIUM56=CODDB34=229457=A02052=20171002-08:08:57.52337=5FCE4C820006920D11=CODNDK01IV453=2448=CODDB447=D452=1448=47034447=D452=1217=12830150=039=01=BETS70=CODNDK01IV55=2NYK01E_A3248=7468922=M54=138=50000.000000040=244=100.000000059=6432=2017100918=G528=A151=50000.000000014=06=060=20171002-08:08:57.52210=020");
            Session.SendToTarget(message, sessionId);
        }
    }


    public class SerilogLogFactory : ILogFactory
    {
        public ILog Create(SessionID sessionID)
        {
            return new SerilogILogWrapper(Log.Logger.ForContext("Session", sessionID));
        }
    }

    public class SerilogILogWrapper : ILog
    {
        private readonly ILogger _logger;

        public SerilogILogWrapper(ILogger logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {
            
        }

        public void Clear()
        {
            
        }

        public void OnIncoming(string msg)
        {
            _logger.Information("Got message {msg}", msg);
        }

        public void OnOutgoing(string msg)
        {
            _logger.Information("Sending message {msg}", msg);
        }

        public void OnEvent(string s)
        {
            _logger.Information("Event: {s}", s);

        }
    }
}

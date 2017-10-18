using System;
using System.Collections.Generic;
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

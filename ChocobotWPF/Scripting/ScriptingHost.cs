using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace Chocobot.Scripting
{
    class ScriptingHost
    {

        private readonly Session _session;
        private readonly ScriptEngine _engine;


        public ScriptingHost()
            : this(null)
        {
            //Just fall back to the below constructor
        }


        public ScriptingHost(dynamic context)
        {

            if (context == null)
                context = this;

            //Create the script engine
            _engine = new ScriptEngine();

            //Let us use engine's Addreference for adding some common
            //assemblies
            new[]
                {
                    typeof (Type).Assembly,
                    typeof (ICollection).Assembly,
                    typeof (ListDictionary).Assembly,
                    typeof (Console).Assembly,
                    typeof (ScriptingHost).Assembly,
                    typeof (IEnumerable<>).Assembly,
                    typeof (IQueryable).Assembly,
                    GetType().Assembly
                }.ToList().ForEach(asm => _engine.AddReference(asm));

            //Import common namespaces
            new[]
                {
                    "System", "System.Linq",
                    "System.Collections",
                    "System.Collections.Generic",
                    "Chocobot",
                    "Chocobot.CombatAI.Classes"
                }.ToList().ForEach(ns => _engine.ImportNamespace(ns));

            _session = _engine.CreateSession(context);

        }

        //public object Execute(string code)
        //{
        //    return _session.Execute(code);
        //}

        public T ExecuteFunction<T>(string code)
        {
            return _session.Execute<T>(code);
        }

        public object Execute(string code)
        {
            return _session.Execute(code);
        }
        public void ExecuteFile(string path)
        {
            _session.ExecuteFile(path);
        }

        public void ImportNamespace(string ns)
        {
            _session.ImportNamespace(ns);
        }

        public void AddReference(Assembly asm)
        {
            _session.AddReference(asm);
        }
 

    }
}

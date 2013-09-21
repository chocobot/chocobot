using System;
using System.Threading;
using System.Windows.Forms;
using Chocobot.Datatypes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;
using Chocobot.Scripting;
using System.Diagnostics;

namespace Chocobot.CombatAI.Classes
{
    public class ScriptAI : GenericAI
    {

        public class ScriptingObject
        {
            private Character _currentMonster;
            private Character _currentUser;
            private Recast _cooldowns;
            private int _attackStep;
            private bool _isRanged;

            public ScriptingObject(Character user, Character monster, Recast recast)
            {
                CurrentMonster = monster;
                CurrentUser = user;
                Cooldowns = recast;
                AttackStep = 0;
 
            }
            public ScriptingObject()
            {
                IsRanged = true;
                AttackStep = 0;
            }

            public int AttackStep
            {
                get { return _attackStep; }
                set { _attackStep = value; }
            }


            public Character CurrentMonster
            {
                get { return _currentMonster; }
                set { _currentMonster = value; }
            }

            public Character CurrentUser
            {
                get { return _currentUser; }
                set { _currentUser = value; }
            }

            public Recast Cooldowns
            {
                get { return _cooldowns; }
                set { _cooldowns = value; }
            }

            public bool IsRanged
            {
                get { return _isRanged; }
                set { _isRanged = value; }
            }

            public void DisplayMessage(string input)
            {
                MessageBox.Show(input, "Chocobot", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private ScriptingObject _scriptingObject;
        private ScriptingHost _host;
        private bool _valid = false;

        public ScriptAI(): base()
        {
            IsRanged = true;
            Name = "Scripting";

            OpenFileDialog openfiledialog = new OpenFileDialog();
            openfiledialog.Filter = "Class AI Script (*.csx)|*.csx";

            if (openfiledialog.ShowDialog() == DialogResult.Cancel)
                return;

            _scriptingObject = new ScriptingObject();
            _host = new ScriptingHost(_scriptingObject);

            _host.ImportNamespace("Chocobot.Utilities.Keyboard");
            _host.ImportNamespace("Chocobot.Datatypes");
            _host.ImportNamespace("Chocobot.MemoryStructures.Abilities.Recast");

            try
            {
                _host.ExecuteFile(openfiledialog.FileName);
            }
            catch (Roslyn.Compilers.CompilationErrorException ex)
            {
                MessageBox.Show(ex.Diagnostics.ToString(), "Scripting Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.Print(ex.Diagnostics.ToString());
                return;
            }


            try
            {
                var res = _host.Execute("Initialize();");

            }
            catch (Roslyn.Compilers.CompilationErrorException ex)
            {
                Console.WriteLine("{0}{1}", Environment.NewLine,
                                            ex.Diagnostics);
                throw;
            }


            IsRanged = _scriptingObject.IsRanged;
            _valid = true;

        }

        public override void Reset()
        {

            if (_valid == false)
                return;

            try
            {

                var res = _host.Execute("Reset();");

                if (res != null)
                    Console.WriteLine(" = " + res.ToString());
            }
            catch (Roslyn.Compilers.CompilationErrorException ex)
            {
                Console.WriteLine("{0}{1}", Environment.NewLine,
                                            ex.Diagnostics);
                throw;
            }

            base.Reset();
        }

        public override void Fight(Character user, Character monster, Recast recast)
        {

            base.Fight(user, monster, recast);

            if (_valid == false)
                return;

            recast.Refresh();

            _scriptingObject.Cooldowns = recast;
            _scriptingObject.CurrentMonster = monster;
            _scriptingObject.CurrentUser = user;

            
            //Debug.Print("Fighting");
            try
            {

                var res = _host.Execute("Fight();");

                if (res != null)
                    Console.WriteLine(" = " + res.ToString());
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            

        }

    }
}

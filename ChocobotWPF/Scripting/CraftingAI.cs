using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chocobot.CombatAI.Classes;
using Chocobot.MemoryStructures.Abilities;
using Chocobot.MemoryStructures.Character;
using Chocobot.Properties;
using Chocobot.MemoryStructures.UIWindows.Crafting;

namespace Chocobot.Scripting
{
    public class CraftingAI
    {


        public class ScriptingObject
        {
            private Character _currentUser;
            private int _craftStep;
            private CraftWindow _craftWindow;

            public ScriptingObject(Character user)
            {
                CurrentUser = user;
                Craftwindow = null;
                CraftStep = 0;

            }
            public ScriptingObject(CraftWindow craftWindow)
            {
                Craftwindow = craftWindow;
                CraftStep = 0;
            }

            public int CraftStep
            {
                get { return _craftStep; }
                set { _craftStep = value; }
            }


            public Character CurrentUser
            {
                get { return _currentUser; }
                set { _currentUser = value; }
            }

            public CraftWindow Craftwindow
            {
                get { return _craftWindow; }
                set { _craftWindow = value; }
            }


            public void DisplayMessage(string input)
            {
                MessageBox.Show(input, "Chocobot", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            public void UseAbility(Datatypes.Keys key, bool useControl = false)
            {

                _currentUser.Refresh();
                Stopwatch timer = new Stopwatch();

                timer.Reset();
                timer.Start();

                Debug.Print("starting ability..");


                while ((_currentUser.UsingAbility) && _currentUser.IsCrafting && timer.Elapsed.Seconds < 3)
                {
                    _currentUser.Refresh();
                }


                while ((_currentUser.UsingAbility == false) && _currentUser.IsCrafting && timer.Elapsed.Seconds < 3)
                {
                    if (useControl)
                        Utilities.Keyboard.KeyBoardHelper.Ctrl(key);
                    else
                        Utilities.Keyboard.KeyBoardHelper.KeyPress(key);
                        
                    
                    _currentUser.Refresh();
                }

                Debug.Print("Waiting for start: " + timer.Elapsed.Milliseconds.ToString());
                timer.Restart();

                while ((_currentUser.UsingAbility == true) && _currentUser.IsCrafting && timer.Elapsed.Seconds < 3)
                {
                    _currentUser.Refresh();
                }

                Debug.Print("Ability Finished: " + timer.Elapsed.Milliseconds.ToString());
                Thread.Sleep(300);
                _currentUser.Refresh();
                _craftWindow.Refresh();
            }
          




        }

        private ScriptingObject _scriptingObject;
        private ScriptingHost _host;
        private bool _valid = false;
        private CraftWindow _craftwindow;

        public CraftWindow Craftwindow
        {
            get { return _craftwindow; }
            set { 
                _craftwindow = value;
                _scriptingObject.Craftwindow = _craftwindow;
            }
        }

        public void Reset()
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

        }


        public void Synth()
        {

            if (_valid == false)
                return;

            try
            {
                
                var res = _host.Execute("Synth();");

                if (res != null)
                    Console.WriteLine(" = " + res.ToString());
            }
            catch (Roslyn.Compilers.CompilationErrorException ex)
            {
                Console.WriteLine("{0}{1}", Environment.NewLine,
                                            ex.Diagnostics);
                throw;
            }

        }

        public void Initialize()
        {

            if (_valid == false)
                return;

            try
            {

                var res = _host.Execute("Initialize();");

                if (res != null)
                    Console.WriteLine(" = " + res.ToString());
            }
            catch (Roslyn.Compilers.CompilationErrorException ex)
            {
                Console.WriteLine("{0}{1}", Environment.NewLine,
                                            ex.Diagnostics);
                throw;
            }

        }


        public CraftingAI(Character user)
        {
            OpenFileDialog openfiledialog = new OpenFileDialog();
            openfiledialog.Filter = Resources.CraftingAI_CraftingAI_Crafting_AI_Script____csx____csx;

            if (openfiledialog.ShowDialog() == DialogResult.Cancel)
                return;

            _scriptingObject = new ScriptingObject(user);
            _host = new ScriptingHost(_scriptingObject);

            _host.ImportNamespace("Chocobot.Utilities.Keyboard");
            _host.ImportNamespace("Chocobot.Datatypes");


            try
            {
                _host.ExecuteFile(openfiledialog.FileName);
            }
            catch (Roslyn.Compilers.CompilationErrorException ex)
            {
                MessageBox.Show(ex.Diagnostics.ToString(), Resources.CraftingAI_CraftingAI_Scripting_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            _valid = true;
        }

    }
}

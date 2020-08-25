using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;

namespace klib
{
    public class AstyleToolWindow : EditorWindow
    {

        private const string PACKAGE_PATH = "Packages/jp.tatsuya-kosuda.klib/Editor/Tools/astyle/Win";

        private const string PS_FILE = "astyle.ps1";

        [MenuItem("Klib/AstyleWindow")]
        private new static void Show()
        {
            EditorWindow.GetWindow<AstyleToolWindow>("Astyle");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Init"))
            {
                ExecuteShell("-Init");
            }

            if (GUILayout.Button("Run"))
            {
                ExecuteShell("-Run");
            }
        }

        private void ExecuteShell(string arg1)
        {
            var p = new Process();
            p.StartInfo.FileName = "powershell";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            string absolute = System.IO.Path.GetFullPath(PACKAGE_PATH);
            string args = absolute + "/" + PS_FILE + " " + arg1 + " " + absolute;
            p.StartInfo.Arguments = args;
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            p.Close();
            UnityEngine.Debug.Log(output);
        }

    }
}

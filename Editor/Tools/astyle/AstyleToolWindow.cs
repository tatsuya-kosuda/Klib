using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;

namespace klib
{
    public class AstyleToolWindow : EditorWindow
    {

#if UNITY_EDITOR_WIN
        private const string PACKAGE_PATH = "Packages/jp.tatsuya-kosuda.klib/Editor/Tools/astyle/Win";
#elif UNITY_EDITOR_OSX
        private const string PACKAGE_PATH = "Packages/jp.tatsuya-kosuda.klib/Editor/Tools/astyle/Mac";
#endif

#if UNITY_EDITOR_WIN
        private const string SH_FILE = "astyle.ps1";
#elif UNITY_EDITOR_OSX
        private const string SH_FILE = "astyle.sh";
#endif

        [MenuItem("Klib/AstyleWindow")]
        private new static void Show()
        {
            EditorWindow.GetWindow<AstyleToolWindow>("Astyle");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Init"))
            {
#if UNITY_EDITOR_WIN
                ExecuteShell("-Init");
#elif UNITY_EDITOR_OSX
                ExecuteShell("init");
#endif
            }

            if (GUILayout.Button("Run"))
            {
#if UNITY_EDITOR_WIN
                ExecuteShell("-Run");
#elif UNITY_EDITOR_OSX
                ExecuteShell("run");
#endif
            }
        }

        private void ExecuteShell(string arg1)
        {
            var p = new Process();
#if UNITY_EDITOR_WIN
            p.StartInfo.FileName = "powershell";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            string absolute = System.IO.Path.GetFullPath(PACKAGE_PATH);
            string args = "-ExecutionPolicy Bypass " + absolute + "/" + SH_FILE + " " + arg1 + " " + absolute;
            p.StartInfo.Arguments = args;
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            p.Close();
#elif UNITY_EDITOR_OSX
            p.StartInfo.FileName = "/bin/bash";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.EnvironmentVariables["PATH"] += ":/usr/local/bin";
            string absolute = System.IO.Path.GetFullPath(PACKAGE_PATH);
            p.StartInfo.Arguments = absolute + "/" + SH_FILE + " " + arg1 + " " + absolute;
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            p.Close();
#endif
            UnityEngine.Debug.Log(output);
        }

    }
}

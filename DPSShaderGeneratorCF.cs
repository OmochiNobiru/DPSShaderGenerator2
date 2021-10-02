#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;

namespace DPSGen
{
    internal static class MenuStringsCF
    {
        public const string BASE_PATH_TOOLS = "Tools/DPS Shader Generator/";
        public const string BASE_PATH_ASSETS = "Assets/DPS Shader Generator/";

        public const string TOOLS_GENERATE_CE = BASE_PATH_TOOLS + "Generate CensorEffect Shaders";
        public const string ASSETS_GENERATE_CE = BASE_PATH_ASSETS + "Generate CensorEffect Shaders";
    }

    public class CFGenWindow : EditorWindow
    {
        [MenuItem(MenuStringsCF.TOOLS_GENERATE_CE)]
        [MenuItem(MenuStringsCF.TOOLS_GENERATE_CE)]
        private static void OpenWindowFromMenu()
        {
            GetWindow<CFGenWindow>("DPS Shader Generator/Generate CensorEffect Shaders");
        }

        Vector2 scroll = Vector2.zero;
        string log = "";

        private void OnEnable()
        {
            minSize = new Vector2(480, 640);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("DPS Shader Generator/Generate CensorEffect Shaders");
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Generate DPS CensorEffect Shaders");
            EditorGUILayout.HelpBox("You need to import DPS and CensorEffect before running this script.", MessageType.Info);
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate"))
            {
                RunCEGen();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(420));
            log = EditorGUILayout.TextArea(log, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void RunCEGen()
        {
            log = "";
            string pathCensor = null;
            string pathCensorMC = null;
            string pathCensorMS = null;
            string pathXSOrifice = null;
            string pathXSPenetrator = null;
            string pathOrifice = null;
            string[] guids1 = AssetDatabase.FindAssets("t:Shader");
            foreach (string guid in guids1)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (pathCensor == null && sp.EndsWith("/CensorShader.shader"))
                {
                    pathCensor = sp;
                    log += "Censor: " + sp + "\n";
                }
                if (pathCensorMC == null && sp.EndsWith("/CensorShaderMaskedCutout.shader"))
                {
                    pathCensorMC = sp;
                    log += "CensorMaskedCutout: " + sp + "\n";
                }
                if (pathCensorMS == null && sp.EndsWith("/CensorShaderMaskedSmooth.shader"))
                {
                    pathCensorMS = sp;
                    log += "CensorMaskedSmooth: " + sp + "\n";
                }

                if (pathXSOrifice == null && sp.EndsWith("/XSToon2.0 Orifice.shader"))
                {
                    pathXSOrifice = sp;
                    log += "XSOrifice: " + sp + "\n";
                }
                if (pathXSPenetrator == null && sp.EndsWith("/XSToon2.0 Penetrator.shader"))
                {
                    pathXSPenetrator = sp;
                    log += "XSPenetrator: " + sp + "\n";
                }
                if (pathOrifice == null && sp.EndsWith("/Orifice.shader"))
                {
                    pathOrifice = sp;
                    log += "Orifice: " + sp + "\n";
                }
            }

            if (pathCensor == null || pathCensorMC == null || pathCensorMS == null)
            {
                log += "Error: CensorEffect not found. Please import CensorEffect.";
                return;
            }
            if (pathXSOrifice == null || pathXSPenetrator == null || pathOrifice == null)
            {
                log += "Error: DPS not found. Please import DPS.";
                return;
            }

            string dpsCGincDirPath = Directory.GetParent(Path.GetDirectoryName(pathXSOrifice)) + "/CGInc";
            string path_xs_OD = null;
            string path_xs_PD = null;
            string path_xs_OVD = null;
            string path_xs_VO = null;
            string path_xs_VP = null;
            log += "DPS_XS_CGInc_Directory: " + dpsCGincDirPath + "\n";
            string[] guids3 = AssetDatabase.FindAssets("", new string[] { dpsCGincDirPath });
            foreach (string guid in guids3)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (path_xs_OD == null && sp.EndsWith("/OrificeDefines.cginc"))
                {
                    path_xs_OD = sp;
                    log += "OrificeDefines: " + sp + "\n";
                }
                if (path_xs_PD == null && sp.EndsWith("/PenetratorDefines.cginc"))
                {
                    path_xs_PD = sp;
                    log += "PenetratorDefines: " + sp + "\n";
                }
                if (path_xs_OVD == null && sp.EndsWith("/XSDefinesOrifice.cginc"))
                {
                    path_xs_OVD = sp;
                    log += "DefinesOrificeVertexData: " + sp + "\n";
                }
                if (path_xs_VO == null && sp.EndsWith("/XSVertOrifice.cginc"))
                {
                    path_xs_VO = sp;
                    log += "DPSFunctions: " + sp + "\n";
                }
                if (path_xs_VP == null && sp.EndsWith("/XSVert.cginc"))
                {
                    path_xs_VP = sp;
                    log += "XSPenetratorVert: " + sp + "\n";
                }
            }

            string dpsPluginDirPath = Directory.GetParent(Path.GetDirectoryName(pathOrifice)) + "/Plugins";
            string path_DPS_func = null;
            log += "DPS_CGInc_Directory: " + dpsPluginDirPath + "\n";
            string[] guids4 = AssetDatabase.FindAssets("", new string[] { dpsPluginDirPath });
            foreach (string guid in guids4)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (path_DPS_func == null && sp.EndsWith("/RalivDPS_Functions.cginc"))
                {
                    path_DPS_func = sp;
                    log += "DPSFunctions: " + sp + "\n";
                }
            }

            log += "\n";
            string[] selfguids = AssetDatabase.FindAssets("DPSShaderGenerator t:script");
            if (selfguids.Length == 0)
            {
                log += "Error: Could not find this script from the asset database. Did you rename this script?";
                return;
            }
            string selfpath = AssetDatabase.GUIDToAssetPath(selfguids[0]);
            string outputPath = Directory.GetParent(Path.GetDirectoryName(selfpath)) + "/ShaderCensor";
            log += "OutputPath: " + outputPath + "\n";
            Directory.CreateDirectory(outputPath);

            List<string> newAssets = new List<string>();

            AssetDatabase.DeleteAsset(outputPath + "/PenetratorDefines.cginc");
            AssetDatabase.CopyAsset(path_xs_PD, outputPath + "/PenetratorDefines.cginc");

            if (path_DPS_func != null)
            {
                string opath = outputPath + "/PenetratorFunctions.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_DPS_func);
                for (int i = 0; i <= 41; i++)
                    writer.WriteLine(lines[i]);
                for (int i = 43; i <= 118; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }


            if (pathCensor != null)
            {
                string opath = outputPath + "/CensorShader_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathCensor);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                string[] vplines = File.ReadAllLines(path_xs_VP);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                lines[0] = "Shader \"FX/DPS/Censor Penetrator\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 5)
                    {
                        // Penetrator Properties
                        for (int j = 11; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    if (i == 9)
                        writer.WriteLine("\"LightMode\" = \"ForwardBase\"");
                    if (i == 24)
                    {
                        writer.WriteLine("#include \"PenetratorDefines.cginc\"");
                        writer.WriteLine("#include \"PenetratorFunctions.cginc\"");
                    }
                    if (i == 28)
                    {
                        //add normal and tangent
                        writer.WriteLine(ovdlines[10]);
                        writer.WriteLine(ovdlines[11]);
                    }
                    if (i == 41)
                    {
                        // Penetrator vert
                        writer.WriteLine(vplines[119]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (pathCensorMC != null)
            {
                string opath = outputPath + "/CensorShaderMaskedCutout_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathCensorMC);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                string[] vplines = File.ReadAllLines(path_xs_VP);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                lines[0] = "Shader \"FX/DPS/Censor (Masked Cutout) Penetrator\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 6)
                    {
                        // Penetrator Properties
                        for (int j = 11; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    if (i == 10)
                        writer.WriteLine("\"LightMode\" = \"ForwardBase\"");
                    if (i == 25)
                    {
                        writer.WriteLine("#include \"PenetratorDefines.cginc\"");
                        writer.WriteLine("#include \"PenetratorFunctions.cginc\"");
                    }
                    if (i == 28)
                    {
                        //add normal and tangent
                        writer.WriteLine(ovdlines[10]);
                        writer.WriteLine(ovdlines[11]);
                    }
                    if (i == 45)
                    {
                        // Penetrator vert
                        writer.WriteLine(vplines[119]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (pathCensorMS != null)
            {
                string opath = outputPath + "/CensorShaderMaskedSmooth_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathCensorMS);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                string[] vplines = File.ReadAllLines(path_xs_VP);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                lines[0] = "Shader \"FX/DPS/Censor (Masked Smooth) Penetrator\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 6)
                    {
                        // Penetrator Properties
                        for (int j = 11; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    if (i == 10)
                        writer.WriteLine("\"LightMode\" = \"ForwardBase\"");
                    if (i == 25)
                    {
                        writer.WriteLine("#include \"PenetratorDefines.cginc\"");
                        writer.WriteLine("#include \"PenetratorFunctions.cginc\"");
                    }
                    if (i == 28)
                    {
                        //add normal and tangent
                        writer.WriteLine(ovdlines[10]);
                        writer.WriteLine(ovdlines[11]);
                    }
                    if (i == 45)
                    {
                        // Penetrator vert
                        writer.WriteLine(vplines[119]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            log += "Done.\n";
        }
    }
}
#endif
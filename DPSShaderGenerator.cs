#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;

namespace DPSGen
{
    internal static class MenuStrings
    {
        public const string BASE_PATH_TOOLS = "Tools/DPS Shader Generator/";
        public const string BASE_PATH_ASSETS = "Assets/DPS Shader Generator/";

        public const string TOOLS_GENERATE_UTS = BASE_PATH_TOOLS + "Generate UTS";
        public const string ASSETS_GENERATE_UTS = BASE_PATH_ASSETS + "Generate UTS";

        public const string TOOLS_GENERATE_WF = BASE_PATH_TOOLS + "Generate UnlitWF Shaders";
        public const string ASSETS_GENERATE_WF = BASE_PATH_ASSETS + "Generate UnlitWF Shaders";

        public const string TOOLS_GENERATE_CE = BASE_PATH_TOOLS + "Generate CensorEffect Shaders";
        public const string ASSETS_GENERATE_CE = BASE_PATH_ASSETS + "Generate CensorEffect Shaders";
    }

    public class CFGenWindow : EditorWindow
    {
        [MenuItem(MenuStrings.TOOLS_GENERATE_CE)]
        [MenuItem(MenuStrings.TOOLS_GENERATE_CE)]
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
            }

            if (pathCensor == null || pathCensorMC == null || pathCensorMS == null)
            {
                log += "Error: CensorEffect not found. Please import CensorEffect.";
                return;
            }
            if (pathXSOrifice == null || pathXSPenetrator == null)
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

            if (path_xs_VO != null)
            {
                string opath = outputPath + "/PenetratorFunctions.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_xs_VO);
                for (int i = 0; i < 41; i++)
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
                        for (int j = 43; j <= 106; j++)
                            writer.WriteLine(vplines[j]);
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
                        for (int j = 43; j <= 106; j++)
                            writer.WriteLine(vplines[j]);
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
                        for (int j = 43; j <= 106; j++)
                            writer.WriteLine(vplines[j]);
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


    public class WFGenWindow : EditorWindow
    {
        [MenuItem(MenuStrings.TOOLS_GENERATE_WF)]
        [MenuItem(MenuStrings.TOOLS_GENERATE_WF)]
        private static void OpenWindowFromMenu()
        {
            GetWindow<WFGenWindow>("DPS Shader Generator/Generate UnlitWF Shaders");
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
            EditorGUILayout.LabelField("DPS Shader Generator/Generate UnlitWF Shaders");
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Generate DPS UnlitWF Shaders");
            EditorGUILayout.HelpBox("You need to import DPS and UnlitWF before running this script.", MessageType.Info);
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate"))
            {
                RunWFGen();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(420));
            log = EditorGUILayout.TextArea(log, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void RunWFGen()
        {
            log = "";
            string pathOpaque = null;
            string pathOpaqueOutline = null;
            string pathOutlineOnly = null;
            string pathXSOrifice = null;
            string pathXSPenetrator = null;
            string[] guids1 = AssetDatabase.FindAssets("t:Shader");
            foreach (string guid in guids1)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (pathOpaque == null && sp.EndsWith("/Unlit_WF_UnToon_Opaque.shader"))
                {
                    pathOpaque = sp;
                    log += "Opaque: " + sp + "\n";
                }
                if (pathOpaqueOutline == null && sp.EndsWith("/Unlit_WF_UnToon_Outline_Opaque.shader"))
                {
                    pathOpaqueOutline = sp;
                    log += "OpaqueOutline: " + sp + "\n";
                }
                if (pathOutlineOnly == null && sp.EndsWith("/Unlit_WF_UnToon_OutlineOnly_Opaque.shader"))
                {
                    pathOutlineOnly = sp;
                    log += "OutlineOnly: " + sp + "\n";
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
            }

            if (pathOpaque == null || pathOpaqueOutline == null || pathOutlineOnly == null)
            {
                log += "Error: UnlitWF not found. Please import UnlitWF.";
                return;
            }
            if (pathXSOrifice == null || pathXSPenetrator == null)
            {
                log += "Error: DPS not found. Please import DPS.";
                return;
            }

            string wfDirPath = Path.GetDirectoryName(pathOpaque);
            string path_inc_meta = null;
            string path_inc_untoon = null;
            string path_inc_input = null;
            string path_inc_func = null;
            string path_inc_uniform = null;
            string path_inc_sc = null;
            string path_inc_com = null;
            string path_inc_comL = null;
            string path_inc_comB = null;
            log += "WFDirectory: " + wfDirPath + "\n";
            string[] guids2 = AssetDatabase.FindAssets("WF_", new string[] { wfDirPath });
            foreach (string guid in guids2)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (path_inc_untoon == null && sp.EndsWith("/WF_UnToon.cginc"))
                {
                    path_inc_untoon = sp;
                    log += "inc_untoon: " + sp + "\n";
                }
                if (path_inc_meta == null && sp.EndsWith("/WF_UnToon_Meta.cginc"))
                {
                    path_inc_meta = sp;
                    log += "inc_meta: " + sp + "\n";
                }
                if (path_inc_sc == null && sp.EndsWith("/WF_UnToon_ShadowCaster.cginc"))
                {
                    path_inc_sc = sp;
                    log += "inc_sc: " + sp + "\n";
                }
                if (path_inc_input == null && sp.EndsWith("/WF_INPUT_UnToon.cginc"))
                {
                    path_inc_input = sp;
                    log += "inc_input: " + sp + "\n";
                }
                if (path_inc_func == null && sp.EndsWith("/WF_UnToon_Function.cginc"))
                {
                    path_inc_func = sp;
                    log += "inc_func: " + sp + "\n";
                }
                if (path_inc_uniform == null && sp.EndsWith("/WF_UnToon_Uniform.cginc"))
                {
                    path_inc_uniform = sp;
                    log += "inc_uniform: " + sp + "\n";
                }
                if (path_inc_com == null && sp.EndsWith("/WF_Common.cginc"))
                {
                    path_inc_com = sp;
                    log += "inc_common: " + sp + "\n";
                }
                if (path_inc_comL == null && sp.EndsWith("/WF_Common_LightweightRP.cginc"))
                {
                    path_inc_comL = sp;
                    log += "inc_commonL: " + sp + "\n";
                }
                if (path_inc_comB == null && sp.EndsWith("/WF_Common_BuiltinRP.cginc"))
                {
                    path_inc_comB = sp;
                    log += "inc_commonB: " + sp + "\n";
                }
            }

            string[] guids_sc = AssetDatabase.FindAssets("WF_ShaderCustomEditor t:Script");
            string path_customEditor = null;
            foreach (string guid in guids_sc)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (sp.EndsWith("/WF_ShaderCustomEditor.cs"))
                {
                    path_customEditor = sp;
                    log += "CustomEditor: " + sp + "\n";
                    break;
                }
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

            log += "\n";
            string[] selfguids = AssetDatabase.FindAssets("DPSShaderGenerator t:script");
            if (selfguids.Length == 0)
            {
                log += "Error: Could not find this script from the asset database. Did you rename this script?";
                return;
            }
            string selfpath = AssetDatabase.GUIDToAssetPath(selfguids[0]);
            string outputPath = Directory.GetParent(Path.GetDirectoryName(selfpath)) + "/ShaderWF";
            log += "OutputPath: " + outputPath + "\n";
            Directory.CreateDirectory(outputPath);

            List<string> newAssets = new List<string>();

            AssetDatabase.DeleteAsset(outputPath + "/WF_Common.cginc");
            AssetDatabase.CopyAsset(path_inc_com, outputPath + "/WF_Common.cginc");

            AssetDatabase.DeleteAsset(outputPath + "/WF_Common_LightweightRP.cginc");
            AssetDatabase.CopyAsset(path_inc_comL, outputPath + "/WF_Common_LightweightRP.cginc");

            AssetDatabase.DeleteAsset(outputPath + "/WF_Common_BuiltinRP.cginc");
            AssetDatabase.CopyAsset(path_inc_comB, outputPath + "/WF_Common_BuiltinRP.cginc");

            AssetDatabase.DeleteAsset(outputPath + "/WF_INPUT_UnToon.cginc");
            AssetDatabase.CopyAsset(path_inc_input, outputPath + "/WF_INPUT_UnToon.cginc");

            AssetDatabase.DeleteAsset(outputPath + "/WF_UnToon_Function.cginc");
            AssetDatabase.CopyAsset(path_inc_func, outputPath + "/WF_UnToon_Function.cginc");

            AssetDatabase.DeleteAsset(outputPath + "/WF_UnToon_Uniform.cginc");
            AssetDatabase.CopyAsset(path_inc_uniform, outputPath + "/WF_UnToon_Uniform.cginc");

            AssetDatabase.DeleteAsset(outputPath + "/OrificeDefines.cginc");
            AssetDatabase.CopyAsset(path_xs_OD, outputPath + "/OrificeDefines.cginc");

            AssetDatabase.DeleteAsset(outputPath + "/PenetratorDefines.cginc");
            AssetDatabase.CopyAsset(path_xs_PD, outputPath + "/PenetratorDefines.cginc");

            if (path_inc_untoon != null)
            {
                string opath = outputPath + "/WF_UnToon_Orifice.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_inc_untoon);
                string[] xslines = File.ReadAllLines(path_xs_VO);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                lines[38] = "";
                lines[40] = ovdlines[13];
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 25)
                        writer.WriteLine("#include \"OrificeDefines.cginc\"");
                    if (i == 72)
                        writer.WriteLine("#include \"OrificeFunctions.cginc\"");
                    if (i == 78 || i == 203)
                    {
                        // Orifice vert
                        for (int j = 54; j <= 81; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_inc_untoon != null)
            {
                string opath = outputPath + "/WF_UnToon_Penetrator.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_inc_untoon);
                string[] xslines = File.ReadAllLines(path_xs_VP);
                lines[38] = "";
                lines[40] = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 25)
                        writer.WriteLine("#include \"PenetratorDefines.cginc\"");
                    if (i == 72)
                        writer.WriteLine("#include \"PenetratorFunctions.cginc\"");
                    if (i == 78 || i == 203)
                    {
                        // Penetrator vert
                        for (int j = 43; j <= 106; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_inc_meta != null)
            {
                string opath = outputPath + "/WF_UnToon_Meta_Orifice.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_inc_meta);
                string[] xslines = File.ReadAllLines(path_xs_VO);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 25)
                        writer.WriteLine("#include \"OrificeDefines.cginc\"");
                    if (i == 57)
                        writer.WriteLine("#include \"OrificeFunctions.cginc\"");
                    if (i == 38)
                    {
                        //add normal, tangent and vertexId
                        writer.WriteLine(ovdlines[10]);
                        writer.WriteLine(ovdlines[11]);
                        writer.WriteLine(ovdlines[13]);
                    }
                    if (i == 63)
                    {
                        // Orifice vert
                        for (int j = 54; j <= 81; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_inc_meta != null)
            {
                string opath = outputPath + "/WF_UnToon_Meta_Penetrator.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_inc_meta);
                string[] xslines = File.ReadAllLines(path_xs_VP);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 25)
                        writer.WriteLine("#include \"PenetratorDefines.cginc\"");
                    if (i == 57)
                        writer.WriteLine("#include \"PenetratorFunctions.cginc\"");
                    if (i == 38)
                    {
                        //add normal and tangent
                        writer.WriteLine(ovdlines[10]);
                        writer.WriteLine(ovdlines[11]);
                    }
                    if (i == 63)
                    {
                        // Penetrator vert
                        for (int j = 43; j <= 106; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_inc_sc != null)
            {
                string opath = outputPath + "/WF_UnToon_ShadowCaster_Orifice.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_inc_sc);
                string[] xslines = File.ReadAllLines(path_xs_VO);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                lines[47] = "v2f_shadow vert_shadow(appdata v) {";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 25)
                        writer.WriteLine("#include \"OrificeDefines.cginc\"");
                    if (i == 42)
                        writer.WriteLine("#include \"OrificeFunctions.cginc\"");
                    if (i == 29)
                    {
                        //add vert structure
                        writer.WriteLine("struct appdata {");
                        writer.WriteLine("float4 vertex : POSITION;");
                        writer.WriteLine("float4 texcoord : TEXCOORD0;");
                        writer.WriteLine(ovdlines[10]);
                        writer.WriteLine(ovdlines[11]);
                        writer.WriteLine(ovdlines[13]);
                        writer.WriteLine("};");
                    }
                    if (i == 48)
                    {
                        // Orifice vert
                        for (int j = 54; j <= 81; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_inc_sc != null)
            {
                string opath = outputPath + "/WF_UnToon_ShadowCaster_Penetrator.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_inc_sc);
                string[] xslines = File.ReadAllLines(path_xs_VP);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                lines[47] = "v2f_shadow vert_shadow(appdata v) {"; 
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 25)
                        writer.WriteLine("#include \"PenetratorDefines.cginc\"");
                    if (i == 42)
                        writer.WriteLine("#include \"PenetratorFunctions.cginc\"");
                    if (i == 29)
                    {
                        //add vert structure
                        writer.WriteLine("struct appdata {");
                        writer.WriteLine("float4 vertex : POSITION;");
                        writer.WriteLine("float4 texcoord : TEXCOORD0;");
                        writer.WriteLine(ovdlines[10]);
                        writer.WriteLine(ovdlines[11]);
                        writer.WriteLine(ovdlines[13]);
                        writer.WriteLine("};");
                    }
                    if (i == 48)
                    {
                        // Penetrator vert
                        for (int j = 43; j <= 106; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_xs_VO != null)
            {
                string opath = outputPath + "/OrificeFunctions.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_xs_VO);
                for (int i = 0; i < 51; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_xs_VO != null)
            {
                string opath = outputPath + "/PenetratorFunctions.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_xs_VO);
                for (int i = 0; i < 41; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (pathOpaque != null)
            {
                string opath = outputPath + "/Unlit_WF_UnToon_Opaque_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathOpaque);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[16] = "Shader \"UnlitWF/DPS/WF_UnToon_Opaque_Orifice\" {";
                lines[324] = "#include \"WF_UnToon_Orifice.cginc\"";
                lines[343] = "#include \"WF_UnToon_ShadowCaster_Orifice.cginc\"";
                lines[363] = "#include \"WF_UnToon_Meta_Orifice.cginc\"";
                lines[371] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 29)
                    {
                        // Orifice Properties
                        writer.WriteLine("[WFHeader(Orifice Dynamic Penetration System)]");
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (pathOpaque != null)
            {
                string opath = outputPath + "/Unlit_WF_UnToon_Opaque_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathOpaque);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[16] = "Shader \"UnlitWF/DPS/WF_UnToon_Opaque_Penetrator\" {";
                lines[324] = "#include \"WF_UnToon_Penetrator.cginc\"";
                lines[343] = "#include \"WF_UnToon_ShadowCaster_Penetrator.cginc\"";
                lines[363] = "#include \"WF_UnToon_Meta_Penetrator.cginc\"";
                lines[371] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 29)
                    {
                        // Penetrator Properties
                        writer.WriteLine("[WFHeader(Orifice Dynamic Penetration System)]");
                        for (int j = 11; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (pathOpaqueOutline != null)
            {
                string opath = outputPath + "/Unlit_WF_UnToon_Outline_Opaque_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathOpaqueOutline);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[16] = "Shader \"UnlitWF/DPS/WF_UnToon_Outline_Opaque_Orifice\" {";
                lines[346] = "#include \"WF_UnToon_Orifice.cginc\"";
                lines[383] = "#include \"WF_UnToon_Orifice.cginc\"";
                lines[388] = "UsePass \"UnlitWF/DPS/WF_UnToon_Opaque_Orifice/SHADOWCASTER\"";
                lines[389] = "UsePass \"UnlitWF/DPS/WF_UnToon_Opaque_Orifice/META\"";
                lines[394] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 29)
                    {
                        // Orifice Properties
                        writer.WriteLine("[WFHeader(Orifice Dynamic Penetration System)]");
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (pathOpaqueOutline != null)
            {
                string opath = outputPath + "/Unlit_WF_UnToon_Outline_Opaque_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathOpaqueOutline);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[16] = "Shader \"UnlitWF/DPS/WF_UnToon_Outline_Opaque_Penetrator\" {";
                lines[346] = "#include \"WF_UnToon_Penetrator.cginc\"";
                lines[383] = "#include \"WF_UnToon_Penetrator.cginc\"";
                lines[388] = "UsePass \"UnlitWF/DPS/WF_UnToon_Opaque_Penetrator/SHADOWCASTER\"";
                lines[389] = "UsePass \"UnlitWF/DPS/WF_UnToon_Opaque_Penetrator/META\"";
                lines[394] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 29)
                    {
                        // Penetrator Properties
                        writer.WriteLine("[WFHeader(Orifice Dynamic Penetration System)]");
                        for (int j = 11; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
/*
            if (pathOutlineOnly != null)
            {
                string opath = outputPath + "/WF_UnToon_OutlineOnly_Opaque_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathOutlineOnly);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[16] = "Shader \"UnlitWF/DPS/WF_UnToon_OutlineOnly_Opaque_Orifice\" {";
                lines[96] = "#pragma vertex vert_outline";
                lines[97] = "";
                lines[101] = "";
                lines[111] = "#include \"WF_UnToon_Orifice.cginc\"";
                lines[121] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 27)
                    {
                        // Orifice Properties
                        writer.WriteLine("[WFHeader(Orifice Dynamic Penetration System)]");
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (pathOutlineOnly != null)
            {
                string opath = outputPath + "/WF_UnToon_OutlineOnly_Opaque_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathOutlineOnly);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[16] = "Shader \"UnlitWF/DPS/WF_UnToon_OutlineOnly_Opaque_Penetrator\" {";
                lines[96] = "#pragma vertex vert_outline";
                lines[97] = "";
                lines[101] = "";
                lines[111] = "#include \"WF_UnToon_Penetrator.cginc\"";
                lines[121] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 27)
                    {
                        // Penetrator Properties
                        writer.WriteLine("[WFHeader(Orifice Dynamic Penetration System)]");
                        for (int j = 11; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
*/
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (path_customEditor != null)
            {
                string opath = Directory.GetParent(Path.GetDirectoryName(selfpath)) + "/Editor" + "/WF_ShaderCustomEditorDPS.cs";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_customEditor);
                lines[28] = "public class ShaderCustomEditorDPS : ShaderGUI";
                lines[208] = "";
                for (int i = 0; i < 992; i++)
                {
                    writer.WriteLine(lines[i]);
                }
                writer.WriteLine("}");
                writer.WriteLine("#endif");
                writer.Flush();
                writer.Close();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            log += "Done.\n";
        }
    }

    public class UTSGenWindow : EditorWindow
    {
        [MenuItem(MenuStrings.TOOLS_GENERATE_UTS)]
        [MenuItem(MenuStrings.TOOLS_GENERATE_UTS)]
        private static void OpenWindowFromMenu()
        {
            GetWindow<UTSGenWindow>("DPS Shader Generator/Generate UTS");
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
            EditorGUILayout.LabelField("DPS Shader Generator / Generate UTS");
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Generate DPS UnityChanShaders");
            EditorGUILayout.HelpBox("You need to import DPS and UTS before running this script.", MessageType.Info);
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate"))
            {
                RunUTSGen();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(420));
            log = EditorGUILayout.TextArea(log, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void RunUTSGen()
        {
            log = "";
            string pathToonDSF = null;
            string pathToonSGM = null;
            string pathToonColDSF = null;
            string pathToonColSGM = null;
            string pathXSOrifice = null;
            string pathXSPenetrator = null;
            string[] guids1 = AssetDatabase.FindAssets("t:Shader");
            foreach (string guid in guids1)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (pathToonDSF == null && sp.EndsWith("/Toon_DoubleShadeWithFeather.shader"))
                {
                    pathToonDSF = sp;
                    log += "ToonDSF: " + sp + "\n";
                }
                if (pathToonSGM == null && sp.EndsWith("/Toon_ShadingGradeMap.shader"))
                {
                    pathToonSGM = sp;
                    log += "ToonSGM: " + sp + "\n";
                }
                if (pathToonColDSF == null && sp.EndsWith("/ToonColor_DoubleShadeWithFeather.shader"))
                {
                    pathToonColDSF = sp;
                    log += "ToonColDSF: " + sp + "\n";
                }
                if (pathToonColSGM == null && sp.EndsWith("/ToonColor_ShadingGradeMap.shader"))
                {
                    pathToonColSGM = sp;
                    log += "ToonColSGM: " + sp + "\n";
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
            }

            if (pathToonDSF == null || pathToonSGM == null || pathToonColDSF == null || pathToonColSGM == null)
            {
                log += "Error: UTS not found. Please import UTS.";
                return;
            }
            if (pathXSOrifice == null || pathXSPenetrator == null)
            {
                log += "Error: DPS not found. Please import DPS.";
                return;
            }

            string utsDirPath = Path.GetDirectoryName(pathToonDSF);
            string path_ucts_DSF = null;
            string path_ucts_SGM = null;
            string path_ucts_OT = null;
            string path_ucts_SC = null;
            log += "UTSDirectory: " + utsDirPath + "\n";
            string[] guids2 = AssetDatabase.FindAssets("UCTS_", new string[] { utsDirPath });
            foreach (string guid in guids2)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (path_ucts_DSF == null && sp.EndsWith("/UCTS_DoubleShadeWithFeather.cginc"))
                {
                    path_ucts_DSF = sp;
                    log += "UCTS_DSF: " + sp + "\n";
                }
                if (path_ucts_SGM == null && sp.EndsWith("/UCTS_ShadingGradeMap.cginc"))
                {
                    path_ucts_SGM = sp;
                    log += "UCTS_SGM: " + sp + "\n";
                }
                if (path_ucts_OT == null && sp.EndsWith("/UCTS_Outline.cginc"))
                {
                    path_ucts_OT = sp;
                    log += "UCTS_OT: " + sp + "\n";
                }
                if (path_ucts_SC == null && sp.EndsWith("/UCTS_ShadowCaster.cginc"))
                {
                    path_ucts_SC = sp;
                    log += "UCTS_SC: " + sp + "\n";
                }
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

            log += "\n";
            string[] selfguids = AssetDatabase.FindAssets("DPSShaderGenerator t:script");
            if (selfguids.Length == 0)
            {
                log += "Error: Could not find this script from the asset database. Did you rename this script?";
                return;
            }
            string selfpath = AssetDatabase.GUIDToAssetPath(selfguids[0]);
            string outputPath = Directory.GetParent(Path.GetDirectoryName(selfpath)) + "/ShaderUTS";
            log += "OutputPath: " + outputPath + "\n";
            Directory.CreateDirectory(outputPath);

            List<string> newAssets = new List<string>();

            if (path_xs_VO != null)
            {
                string opath = outputPath + "/OrificeFunctions.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_xs_VO);
                for (int i = 0; i < 51; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_xs_VO != null)
            {
                string opath = outputPath + "/PenetratorFunctions.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_xs_VO);
                for (int i = 0; i < 41; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_ucts_DSF != null)
            {
                string opath = outputPath + "/UCTS_DoubleShadeWithFeather_Orifice.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_ucts_DSF);
                string[] xslines = File.ReadAllLines(path_xs_VO);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 149)
                    {
                        //add vertexId
                        writer.WriteLine(ovdlines[13]);
                    }
                    if (i == 164)
                    {
                        // Orifice vert
                        for (int j = 54; j <= 81; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_ucts_DSF != null)
            {
                string opath = outputPath + "/UCTS_DoubleShadeWithFeather_Penetrator.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_ucts_DSF);
                string[] xslines = File.ReadAllLines(path_xs_VP);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 164)
                    {
                        // Penetrator vert
                        for (int j = 43; j <= 106; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_ucts_SGM != null)
            {
                string opath = outputPath + "/UCTS_ShadingGradeMap_Orifice.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_ucts_SGM);
                string[] xslines = File.ReadAllLines(path_xs_VO);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 159)
                    {
                        //add vertexId
                        writer.WriteLine(ovdlines[13]);
                    }
                    if (i == 194)
                    {
                        // Orifice vert
                        for (int j = 54; j <= 81; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_ucts_SGM != null)
            {
                string opath = outputPath + "/UCTS_ShadingGradeMap_Penetrator.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_ucts_SGM);
                string[] xslines = File.ReadAllLines(path_xs_VP);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 194)
                    {
                        // Penetrator vert
                        for (int j = 43; j <= 106; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_ucts_OT != null)
            {
                string opath = outputPath + "/UCTS_Outline_Orifice.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_ucts_OT);
                string[] xslines = File.ReadAllLines(path_xs_VO);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 49)
                    {
                        //add vertexId
                        writer.WriteLine(ovdlines[13]);
                    }
                    if (i == 58)
                    {
                        // Orifice vert
                        for (int j = 54; j <= 81; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_ucts_OT != null)
            {
                string opath = outputPath + "/UCTS_Outline_Penetrator.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_ucts_OT);
                string[] xslines = File.ReadAllLines(path_xs_VP);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 58)
                    {
                        // Penetrator vert
                        for (int j = 43; j <= 106; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_ucts_SC != null)
            {
                string opath = outputPath + "/UCTS_ShadowCaster_Orifice.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_ucts_SC);
                string[] xslines = File.ReadAllLines(path_xs_VO);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 25)
                    {
                        //add normal, tangent and vertexId
                        writer.WriteLine(ovdlines[10]);
                        writer.WriteLine(ovdlines[11]);
                        writer.WriteLine(ovdlines[13]);
                    }
                    if (i == 48)
                    {
                        // Orifice vert
                        for (int j = 54; j <= 81; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_ucts_SC != null)
            {
                string opath = outputPath + "/UCTS_ShadowCaster_Penetrator.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_ucts_SC);
                string[] xslines = File.ReadAllLines(path_xs_VP);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 25)
                    {
                        //add normal and tangent
                        writer.WriteLine(ovdlines[10]);
                        writer.WriteLine(ovdlines[11]);
                    }
                    if (i == 48)
                    {
                        // Penetrator vert
                        for (int j = 43; j <= 106; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            
            AssetDatabase.DeleteAsset(outputPath + "/OrificeDefines.cginc");
            AssetDatabase.CopyAsset(path_xs_OD, outputPath + "/OrificeDefines.cginc");

            AssetDatabase.DeleteAsset(outputPath + "/PenetratorDefines.cginc");
            AssetDatabase.CopyAsset(path_xs_PD, outputPath + "/PenetratorDefines.cginc");

            if (pathToonDSF != null)
            {
                string opath = outputPath + "/Toon_DoubleShadeWithFeather_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonDSF);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[5] = "Shader \"UnityChanToonShader/DPS/Toon_DoubleShadeWithFeather_Orifice\" {";
                lines[174] = "#include \"UCTS_Outline_Orifice.cginc\"";
                lines[205] = "#include \"UCTS_DoubleShadeWithFeather_Orifice.cginc\"";
                lines[235] = "#include \"UCTS_DoubleShadeWithFeather_Orifice.cginc\"";
                lines[260] = "#include \"UCTS_ShadowCaster_Orifice.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }

                    if (i == 158)
                        writer.WriteLine("\"LightMode\" = \"ForwardBase\"");

                    if (i == 170 || i == 199 || i == 232 || i == 259)
                    {
                        writer.WriteLine("#include \"OrificeDefines.cginc\"");
                        writer.WriteLine("#include \"OrificeFunctions.cginc\"");
                    }

                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (pathToonDSF != null)
            {
                string opath = outputPath + "/Toon_DoubleShadeWithFeather_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonDSF);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[5] = "Shader \"UnityChanToonShader/DPS/Toon_DoubleShadeWithFeather_Penetrator\" {";
                lines[174] = "#include \"UCTS_Outline_Penetrator.cginc\"";
                lines[205] = "#include \"UCTS_DoubleShadeWithFeather_Penetrator.cginc\"";
                lines[235] = "#include \"UCTS_DoubleShadeWithFeather_Penetrator.cginc\"";
                lines[260] = "#include \"UCTS_ShadowCaster_Penetrator.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Penetrator Properties
                        for (int j = 11; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }

                    if (i == 158)
                        writer.WriteLine("\"LightMode\" = \"ForwardBase\"");

                    if (i == 170 || i == 199 || i == 232 || i == 259)
                    {
                        writer.WriteLine("#include \"PenetratorDefines.cginc\"");
                        writer.WriteLine("#include \"PenetratorFunctions.cginc\"");
                    }

                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (pathToonSGM != null)
            {
                string opath = outputPath + "/Toon_ShadingGradeMap_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonSGM);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[5] = "Shader \"UnityChanToonShader/DPS/Toon_ShadingGradeMap_Orifice\" {";
                lines[176] = "#include \"UCTS_Outline_Orifice.cginc\"";
                lines[207] = "#include \"UCTS_ShadingGradeMap_Orifice.cginc\"";
                lines[237] = "#include \"UCTS_ShadingGradeMap_Orifice.cginc\"";
                lines[262] = "#include \"UCTS_ShadowCaster_Orifice.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }

                    if (i == 160)
                        writer.WriteLine("\"LightMode\" = \"ForwardBase\"");

                    if (i == 172 || i == 200 || i == 233 || i == 260)
                    {
                        writer.WriteLine("#include \"OrificeDefines.cginc\"");
                        writer.WriteLine("#include \"OrificeFunctions.cginc\"");
                    }

                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (pathToonSGM != null)
            {
                string opath = outputPath + "/Toon_ShadingGradeMap_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonSGM);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[5] = "Shader \"UnityChanToonShader/DPS/Toon_ShadingGradeMap_Penetrator\" {";
                lines[176] = "#include \"UCTS_Outline_Penetrator.cginc\"";
                lines[207] = "#include \"UCTS_ShadingGradeMap_Penetrator.cginc\"";
                lines[237] = "#include \"UCTS_ShadingGradeMap_Penetrator.cginc\"";
                lines[262] = "#include \"UCTS_ShadowCaster_Penetrator.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Penetrator Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }

                    if (i == 160)
                        writer.WriteLine("\"LightMode\" = \"ForwardBase\"");

                    if (i == 172 || i == 200 || i == 233 || i == 260)
                    {
                        writer.WriteLine("#include \"PenetratorDefines.cginc\"");
                        writer.WriteLine("#include \"PenetratorFunctions.cginc\"");
                    }
                    
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (pathToonColDSF != null)
            {
                string opath = outputPath + "/ToonColor_DoubleShadeWithFeather_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonColDSF);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[5] = "Shader \"UnityChanToonShader/DPS/ToonColor_DoubleShadeWithFeather_Orifice\" {";
                lines[139] = "UsePass \"UnityChanToonShader/DPS/Toon_DoubleShadeWithFeather_Orifice/FORWARD\"";
                lines[140] = "UsePass \"UnityChanToonShader/DPS/Toon_DoubleShadeWithFeather_Orifice/FORWARD_DELTA\"";
                lines[141] = "UsePass \"UnityChanToonShader/DPS/Toon_DoubleShadeWithFeather_Orifice/SHADOWCASTER\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (pathToonColDSF != null)
            {
                string opath = outputPath + "/ToonColor_DoubleShadeWithFeather_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonColDSF);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[5] = "Shader \"UnityChanToonShader/DPS/ToonColor_DoubleShadeWithFeather_Penetrator\" {";
                lines[139] = "UsePass \"UnityChanToonShader/DPS/Toon_DoubleShadeWithFeather_Penetrator/FORWARD\"";
                lines[140] = "UsePass \"UnityChanToonShader/DPS/Toon_DoubleShadeWithFeather_Penetrator/FORWARD_DELTA\"";
                lines[141] = "UsePass \"UnityChanToonShader/DPS/Toon_DoubleShadeWithFeather_Penetrator/SHADOWCASTER\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Penetrator Properties
                        for (int j = 11; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (pathToonColSGM != null)
            {
                string opath = outputPath + "/ToonColor_ShadingGradeMap_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonColSGM);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[5] = "Shader \"UnityChanToonShader/DPS/ToonColor_ShadingGradeMap_Orifice\" {";
                lines[141] = "UsePass \"UnityChanToonShader/DPS/Toon_ShadingGradeMap_Orifice/FORWARD\"";
                lines[142] = "UsePass \"UnityChanToonShader/DPS/Toon_ShadingGradeMap_Orifice/FORWARD_DELTA\"";
                lines[143] = "UsePass \"UnityChanToonShader/DPS/Toon_ShadingGradeMap_Orifice/SHADOWCASTER\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (pathToonColSGM != null)
            {
                string opath = outputPath + "/ToonColor_ShadingGradeMap_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonColSGM);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[5] = "Shader \"UnityChanToonShader/DPS/ToonColor_ShadingGradeMap_Penetrator\" {";
                lines[141] = "UsePass \"UnityChanToonShader/DPS/Toon_ShadingGradeMap_Penetrator/FORWARD\"";
                lines[142] = "UsePass \"UnityChanToonShader/DPS/Toon_ShadingGradeMap_Penetrator/FORWARD_DELTA\"";
                lines[143] = "UsePass \"UnityChanToonShader/DPS/Toon_ShadingGradeMap_Penetrator/SHADOWCASTER\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Penetrator Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            log += "Done.\n";

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

#endif
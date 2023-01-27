#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;

namespace DPSGen
{
    internal static class MenuStringsWF
    {
        public const string BASE_PATH_TOOLS = "Tools/DPS Shader Generator/";
        public const string BASE_PATH_ASSETS = "Assets/DPS Shader Generator/";

        public const string TOOLS_GENERATE_WF = BASE_PATH_TOOLS + "Generate UnlitWF Shaders";
        public const string ASSETS_GENERATE_WF = BASE_PATH_ASSETS + "Generate UnlitWF Shaders";
    }

    public class WFGenWindow : EditorWindow
    {
        [MenuItem(MenuStringsWF.TOOLS_GENERATE_WF)]
        [MenuItem(MenuStringsWF.TOOLS_GENERATE_WF)]
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

            if (GUILayout.Button("Remove UnlitWFDPS"))
            {
                log = "";
                RemoveUnlitWFDPS();
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
            string pathTransparent = null;
            string pathXSOrifice = null;
            string pathXSPenetrator = null;
            string pathOrifice = null;
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
                if (pathTransparent == null && sp.EndsWith("/Unlit_WF_UnToon_Transparent.shader"))
                {
                    pathTransparent = sp;
                    log += "Transparent: " + sp + "\n";
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

            if (pathOpaque == null || pathOpaqueOutline == null || pathOutlineOnly == null || pathTransparent == null)
            {
                log += "Error: UnlitWF not found. Please import UnlitWF.";
                return;
            }
            if (pathXSOrifice == null || pathXSPenetrator == null || pathOrifice == null)
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
                    log += "XSVertOrifice: " + sp + "\n";
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


            if (path_DPS_func != null)
            {
                string opath = outputPath + "/OrificeFunctions.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_DPS_func);
                for (int i = 0; i <= 41; i++)
                    writer.WriteLine(lines[i]);
                for (int i = 122; i <= 160; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
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
                    if (i == 78 || i == 222)
                    {
                        // Orifice vert
                        writer.WriteLine(xslines[86]);
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
                    if (i == 78 || i == 222)
                    {
                        // Penetrator vert
                        writer.WriteLine(xslines[119]);
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
                        writer.WriteLine(xslines[86]);
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
                        writer.WriteLine(xslines[119]);
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
                        writer.WriteLine(xslines[86]);
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
                        writer.WriteLine(xslines[119]);
                    }
                    writer.WriteLine(lines[i]);
                }
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
                lines[409] = "#include \"WF_UnToon_Orifice.cginc\"";
                lines[429] = "#include \"WF_UnToon_ShadowCaster_Orifice.cginc\"";
                lines[450] = "#include \"WF_UnToon_Meta_Orifice.cginc\"";
                lines[458] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
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
                lines[409] = "#include \"WF_UnToon_Penetrator.cginc\"";
                lines[429] = "#include \"WF_UnToon_ShadowCaster_Penetrator.cginc\"";
                lines[450] = "#include \"WF_UnToon_Meta_Penetrator.cginc\"";
                lines[458] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
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
            if (pathOpaque != null)
            {
                string opath = outputPath + "/Unlit_WF_UnToon_Opaque_Penetrator_Overlay.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathOpaque);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[16] = "Shader \"UnlitWF/DPS/WF_UnToon_Opaque_Penetrator_Overlay\" {";
                lines[360] = "\"RenderType\" = \"Transparent\"";
                lines[361] = "\"Queue\" = \"Transparent+590\"";
                lines[368] = "ZWrite ON";
                lines[370] = "ZTest LEqual";
                lines[409] = "#include \"WF_UnToon_Penetrator.cginc\"";
                lines[417] = "ZWrite ON";
                lines[419] = "ZTest LEqual";
                lines[429] = "#include \"WF_UnToon_ShadowCaster_Penetrator.cginc\"";
                lines[437] = "ZWrite ON";
                lines[439] = "ZTest LEqual";
                lines[450] = "#include \"WF_UnToon_Meta_Penetrator.cginc\"";
                lines[458] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";

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
                lines[423] = "#include \"WF_UnToon_Orifice.cginc\"";
                lines[473] = "#include \"WF_UnToon_Orifice.cginc\"";
                lines[478] = "UsePass \"UnlitWF/DPS/WF_UnToon_Opaque_Orifice/SHADOWCASTER\"";
                lines[479] = "UsePass \"UnlitWF/DPS/WF_UnToon_Opaque_Orifice/META\"";
                lines[484] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
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
                lines[423] = "#include \"WF_UnToon_Penetrator.cginc\"";
                lines[473] = "#include \"WF_UnToon_Penetrator.cginc\"";
                lines[478] = "UsePass \"UnlitWF/DPS/WF_UnToon_Opaque_Penetrator/SHADOWCASTER\"";
                lines[479] = "UsePass \"UnlitWF/DPS/WF_UnToon_Opaque_Penetrator/META\"";
                lines[484] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
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

            if (pathTransparent != null)
            {
                string opath = outputPath + "/Unlit_WF_UnToon_Transparent_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathTransparent);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[16] = "Shader \"UnlitWF/DPS/WF_UnToon_Transparent_Orifice\" {";
                lines[437] = "#include \"WF_UnToon_Orifice.cginc\"";
                lines[490] = "#include \"WF_UnToon_Orifice.cginc\"";
                lines[512] = "#include \"WF_UnToon_ShadowCaster_Orifice.cginc\"";
                lines[535] = "#include \"WF_UnToon_Meta_Orifice.cginc\"";
                lines[543] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 26)
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
            if (pathTransparent != null)
            {
                string opath = outputPath + "/Unlit_WF_UnToon_Transparent_Orifice_Overlay.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathTransparent);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[16] = "Shader \"UnlitWF/DPS/WF_UnToon_Transparent_Orifice_Overlay\" {";
                lines[384] = "\"Queue\" = \"Transparent+580\"";
                lines[391] = "ZTest Always";
                lines[437] = "#include \"WF_UnToon_Orifice.cginc\"";
                lines[447] = "ZWrite ON";
                lines[449] = "ZTest Always";
                lines[490] = "#include \"WF_UnToon_Orifice.cginc\"";
                lines[512] = "#include \"WF_UnToon_ShadowCaster_Orifice.cginc\"";
                lines[535] = "#include \"WF_UnToon_Meta_Orifice.cginc\"";
                lines[543] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 26)
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
            if (pathTransparent != null)
            {
                string opath = outputPath + "/Unlit_WF_UnToon_Transparent_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathTransparent);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[16] = "Shader \"UnlitWF/DPS/WF_UnToon_Transparent_Penetrator\" {";
                lines[437] = "#include \"WF_UnToon_Penetrator.cginc\"";
                lines[490] = "#include \"WF_UnToon_Penetrator.cginc\"";
                lines[512] = "#include \"WF_UnToon_ShadowCaster_Penetrator.cginc\"";
                lines[535] = "#include \"WF_UnToon_Meta_Penetrator.cginc\"";
                lines[543] = "CustomEditor \"UnlitWF.ShaderCustomEditorDPS\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 26)
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

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (path_customEditor != null)
            {
                string opath = Directory.GetParent(Path.GetDirectoryName(selfpath)) + "/Editor" + "/WF_ShaderCustomEditorDPS.cs";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_customEditor);
                lines[28] = "public class ShaderCustomEditorDPS : ShaderGUI";
                lines[326] = "";
                for (int i = 0; i < 1386; i++)
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


            var dpsWFOpaqueOutlineShader = Shader.Find("UnlitWF/DPS/WF_UnToon_Outline_Opaque_Orifice");
            string[] mat_guids = AssetDatabase.FindAssets("BodyMat", new string[] { "Assets/Silvino/Materials" });
            foreach (string guid in mat_guids)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (sp.EndsWith("/BodyMat Orifice.mat") ||
                    sp.EndsWith("/BodyMat Orifice deleted.mat") ||
                    sp.EndsWith("/BodyMat Orifice deleted boko.mat") ||
                    sp.EndsWith("/BodyMat Orifice boko.mat"))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (material != null)
                        material.shader = dpsWFOpaqueOutlineShader;
                }
            }

        }

        private void RemoveUnlitWFDPS()
        {

            AssetDatabase.StartAssetEditing();

            AssetDatabase.DeleteAsset("Assets/DPSShaderGenerator/ShaderWF");
            AssetDatabase.DeleteAsset("Assets/DPSShaderGenerator/Editor/WF_ShaderCustomEditorDPS.cs");

            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();

            log += "Done.\n";
        }
    }
}
#endif
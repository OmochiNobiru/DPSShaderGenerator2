#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;

namespace DPSGen
{
    internal static class MenuStringsLT
    {
        public const string BASE_PATH_TOOLS = "Tools/DPS Shader Generator/";
        public const string BASE_PATH_ASSETS = "Assets/DPS Shader Generator/";

        public const string TOOLS_GENERATE_LT = BASE_PATH_TOOLS + "Generate lilToon";
        public const string ASSETS_GENERATE_LT = BASE_PATH_ASSETS + "Generate lilToon";
    }

    public class LTGenWindow : EditorWindow
    {
        [MenuItem(MenuStringsLT.TOOLS_GENERATE_LT)]
        [MenuItem(MenuStringsLT.ASSETS_GENERATE_LT)]
        private static void OpenWindowFromMenu()
        {
            GetWindow<LTGenWindow>("DPS Shader Generator/Generate lilToon");
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
            EditorGUILayout.LabelField("DPS Shader Generator / Generate lilToon");
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Generate DPS lilToon");
            EditorGUILayout.HelpBox("You need to import DPS and lilToon before running this script.", MessageType.Info);
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate"))
            {
                RunLILGen();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(420));
            log = EditorGUILayout.TextArea(log, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void RunLILGen()
        {
            log = "";

            string[] selfguids = AssetDatabase.FindAssets("DPSShaderGenerator t:script");
            if (selfguids.Length == 0)
            {
                log += "Error: Could not find this script from the asset database. Did you rename this script?";
                return;
            }
            string selfpath = AssetDatabase.GUIDToAssetPath(selfguids[0]);
            string outputPath = Directory.GetParent(Path.GetDirectoryName(selfpath)) + "/lilToonDPS";
            log += "OutputPath: " + outputPath + "\n";
            Directory.CreateDirectory(outputPath);
            Directory.CreateDirectory(outputPath + "/Includes");

            string pathLil = null;
            string pathLilOpaque = null;
            string pathLilOutline = null;
            string pathXSOrifice = null;
            string pathXSPenetrator = null;
            string pathOrifice = null;
            string[] guids1 = AssetDatabase.FindAssets("t:Shader");
            foreach (string guid in guids1)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (pathLil == null && sp.EndsWith("/lts.shader"))
                {
                    pathLil = sp;
                    log += "lil: " + sp + "\n";
                }
                if (pathLilOutline == null && sp.EndsWith("/lts_o.shader"))
                {
                    pathLilOutline = sp;
                    log += "lilOutline: " + sp + "\n";
                }
                if (pathLilOpaque == null && sp.EndsWith("/ltspass_opaque.shader"))
                {
                    pathLilOpaque = sp;
                    log += "lilOpaque: " + sp + "\n";
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

            if (pathLil == null || pathLilOutline == null || pathLilOpaque == null)
            {
                log += "Error: lilToon not found. Please import lilToon.";
                return;
            }
            if (pathXSOrifice == null || pathXSPenetrator == null || pathOrifice == null)
            {
                log += "Error: DPS not found. Please import DPS.";
                return;
            }
            
            string lilDirPath = Path.GetDirectoryName(pathLil) + "/Includes";
            string path_forward = null;
            string path_normal = null;
            string path_shadow = null;
            string path_meta = null;
            string path_common = null;
            string path_struct = null;
            string path_vert = null;
            log += "lilDirectory: " + lilDirPath + "\n";
            string[] guids2 = AssetDatabase.FindAssets("lil", new string[] { lilDirPath });
            foreach (string guid in guids2)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (path_forward == null && sp.EndsWith("/lil_pass_forward.hlsl"))
                {
                    path_forward = sp;
                    log += "path_forward: " + sp + "\n";
                }
                if (path_normal == null && sp.EndsWith("/lil_pass_forward_normal.hlsl"))
                {
                    path_normal = sp;
                    log += "pass_forward_normal: " + sp + "\n";
                }
                else if (path_shadow == null && sp.EndsWith("/lil_pass_shadowcaster.hlsl"))
                {
                    path_shadow = sp;
                    log += "pass_shadowcaster: " + sp + "\n";
                }
                else if (path_meta == null && sp.EndsWith("/lil_pass_meta.hlsl"))
                {
                    path_meta = sp;
                    log += "pass_meta: " + sp + "\n";
                }
                else if (path_common == null && sp.EndsWith("/lil_common.hlsl"))
                {
                    path_common = sp;
                    log += "lil_common: " + sp + "\n";
                }
                else if (path_struct == null && sp.EndsWith("/lil_common_appdata.hlsl"))
                {
                    path_struct = sp;
                    log += "lil_common_appdata: " + sp + "\n";
                }
                else if (path_vert == null && sp.EndsWith("/lil_common_vert.hlsl"))
                {
                    path_vert = sp;
                    log += "lil_common_vert: " + sp + "\n";
                }
                else
                {
                    string filename = Path.GetFileName(sp);
                    AssetDatabase.DeleteAsset(outputPath + "/Includes/" + filename);
                    AssetDatabase.CopyAsset(sp, outputPath + "/Includes/" + filename);
                }
            }

            string lilSettingDirPath = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(pathLil)) + "") + "/lilToonSetting";
            string path_setting = null;
            log += "lilSettingDir: " + lilSettingDirPath + "\n";
            string[] guids2_1 = AssetDatabase.FindAssets("lil", new string[] { lilSettingDirPath });
            foreach (string guid in guids2_1)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (path_setting == null && sp.EndsWith("/lil_setting.hlsl"))
                {
                    log += "lilSetting: " + sp + "\n";
                    string filename = Path.GetFileName(sp);
                    AssetDatabase.DeleteAsset(outputPath + "/Includes/" + filename);
                    AssetDatabase.CopyAsset(sp, outputPath + "/Includes/" + filename);
                }
            }

            string[] guids_sc = AssetDatabase.FindAssets("lilInspector t:Script");
            string path_customEditor = null;
            foreach (string guid in guids_sc)
            {
                string sp = AssetDatabase.GUIDToAssetPath(guid);
                if (sp.EndsWith("/lilInspector.cs"))
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

            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.StartAssetEditing();

            List<string> newAssets = new List<string>();

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

            if (path_common != null)
            {
                string opath = outputPath + "/Includes" + "/lil_common.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_common);
                lines[20] = "#include \"Includes/lil_setting.hlsl\"";
                for (int i = 0; i < lines.Length; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_struct != null)
            {
                string opath = outputPath + "/Includes" + "/lil_common_appdata.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_struct);
                string[] ovdlines = File.ReadAllLines(path_xs_OVD);
                lines[48] = "";
                lines[50] = "";
                lines[52] = "";
                lines[54] = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 115)
                    {
                        //add vertexId
                        writer.WriteLine(ovdlines[13]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            string orifice_vert = "";
            if (path_xs_VO != null)
            {
                string[] xslines = File.ReadAllLines(path_xs_VO);
                orifice_vert = xslines[86].Substring(0, xslines[86].IndexOf('('))
                    + "(input.positionOS, input.normalOS, input.tangentOS.xyz, input"
                    + xslines[86].Substring(xslines[86].LastIndexOf('.'));
            }
            string penetrator_vert = "";
            if (path_xs_VP != null)
            {
                string[] xslines = File.ReadAllLines(path_xs_VP);
                penetrator_vert = xslines[119].Substring(0, xslines[119].IndexOf('('))
                    + "(input.positionOS, input.normalOS);";
            }

            if (path_vert != null)
            {
                string opath = outputPath + "/Includes" + "/lil_common_vert_orifice.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_vert);
                string[] xslines = File.ReadAllLines(path_xs_VO);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 3)
                    {
                        writer.WriteLine("#include \"OrificeDefines.cginc\"");
                        writer.WriteLine("#include \"OrificeFunctions.cginc\"");
                    }
                    if (i == 54)
                    {
                        // Orifice vert
                        writer.WriteLine(orifice_vert);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_vert != null)
            {
                string opath = outputPath + "/Includes" + "/lil_common_vert_penetrator.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_vert);
                string[] xslines = File.ReadAllLines(path_xs_VP);
                for (int i = 0; i < lines.Length; i++)
                {
                    if(i == 3)
                    {
                        writer.WriteLine("#include \"PenetratorDefines.cginc\"");
                        writer.WriteLine("#include \"PenetratorFunctions.cginc\"");
                    }
                    if (i == 54)
                    {
                        // Penetrator vert
                        writer.WriteLine(penetrator_vert);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_normal != null)
            {
                string opath = outputPath + "/Includes" + "/lil_pass_forward_normal_orifice.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_normal);
                lines[108] = "#include \"Includes/lil_common_vert_orifice.hlsl\"";
                for (int i = 0; i < lines.Length; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_normal != null)
            {
                string opath = outputPath + "/Includes" + "/lil_pass_forward_normal_penetrator.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_normal);
                lines[108] = "#include \"Includes/lil_common_vert_penetrator.hlsl\"";
                for (int i = 0; i < lines.Length; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_forward != null)
            {
                string opath = outputPath + "/Includes" + "/lil_pass_forward_orifice.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_forward);
                lines[6] = "#include \"Includes/lil_pass_forward_normal_orifice.hlsl\"";
                for (int i = 0; i < lines.Length; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_forward != null)
            {
                string opath = outputPath + "/Includes" + "/lil_pass_forward_penetrator.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_forward);
                lines[6] = "#include \"Includes/lil_pass_forward_normal_penetrator.hlsl\"";
                for (int i = 0; i < lines.Length; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_shadow != null)
            {
                string opath = outputPath + "/Includes" + "/lil_pass_shadowcaster_orifice.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_shadow);
                lines[36] = "#include \"Includes/lil_common_vert_orifice.hlsl\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_shadow != null)
            {
                string opath = outputPath + "/Includes" + "/lil_pass_shadowcaster_penetrator.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_shadow);
                lines[36] = "#include \"Includes/lil_common_vert_penetrator.hlsl\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (path_meta != null)
            {
                string opath = outputPath + "/Includes" + "/lil_pass_meta_orifice.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_meta);
                lines[47] = "#include \"Includes/lil_common_vert_orifice.hlsl\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_meta != null)
            {
                string opath = outputPath + "/Includes" + "/lil_pass_meta_penetrator.hlsl";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_meta);
                lines[47] = "#include \"Includes/lil_common_vert_penetrator.hlsl\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (pathLilOpaque != null)
            {
                string opath = outputPath + "/ltspass_opaque_orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathLilOpaque);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[0] = "Shader \"Hidden/ltspass_opaque_orifice\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].IndexOf("lil_pass_forward.hlsl") >= 0)
                        lines[i] = lines[i].Replace("lil_pass_forward.hlsl", "lil_pass_forward_orifice.hlsl");
                    if (lines[i].IndexOf("lil_pass_shadowcaster.hlsl") >= 0)
                        lines[i] = lines[i].Replace("lil_pass_shadowcaster.hlsl", "lil_pass_shadowcaster_orifice.hlsl");
                    if (lines[i].IndexOf("lil_pass_meta.hlsl") >= 0)
                        lines[i] = lines[i].Replace("lil_pass_meta.hlsl", "lil_pass_meta_orifice.hlsl");

                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (pathLilOpaque != null)
            {
                string opath = outputPath + "/ltspass_opaque_penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathLilOpaque);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[0] = "Shader \"Hidden/ltspass_opaque_penetrator\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].IndexOf("lil_pass_forward.hlsl") >= 0)
                        lines[i] = lines[i].Replace("lil_pass_forward.hlsl", "lil_pass_forward_penetrator.hlsl");
                    if (lines[i].IndexOf("lil_pass_shadowcaster.hlsl") >= 0)
                        lines[i] = lines[i].Replace("lil_pass_shadowcaster.hlsl", "lil_pass_shadowcaster_penetrator.hlsl");
                    if (lines[i].IndexOf("lil_pass_meta.hlsl") >= 0)
                        lines[i] = lines[i].Replace("lil_pass_meta.hlsl", "lil_pass_meta_penetrator.hlsl");

                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (pathLilOutline != null)
            {
                string opath = outputPath + "/lts_o_orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathLilOutline);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[0] = "Shader \"Hidden/lilToonOutline_Orifice\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 13)
                    {
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }

                    if (lines[i].IndexOf("ltspass_opaque") >= 0)
                        lines[i] = lines[i].Replace("ltspass_opaque", "ltspass_opaque_orifice");

                    if (lines[i].IndexOf("lilToon.lilToonInspector") >= 0)
                        lines[i] = lines[i].Replace("lilToon.lilToonInspector", "lilToon.lilToonInspectorDPS_Orifice");

                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (pathLilOutline != null)
            {
                string opath = outputPath + "/lts_o_penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathLilOutline);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[0] = "Shader \"Hidden/lilToonOutline_Penetrator\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 13)
                    {
                        // Penetrator Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }

                    if (lines[i].IndexOf("ltspass_opaque") >= 0)
                        lines[i] = lines[i].Replace("ltspass_opaque", "ltspass_opaque_penetrator");

                    if (lines[i].IndexOf("lilToon.lilToonInspector") >= 0)
                        lines[i] = lines[i].Replace("lilToon.lilToonInspector", "lilToon.lilToonInspectorDPS_Penetrator");

                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            if (pathLil != null)
            {
                string opath = outputPath + "/lts_orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathLil);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[0] = "Shader \"lilToon_Orifice\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 13)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }

                    if (lines[i].IndexOf("ltspass_opaque") >= 0)
                        lines[i] = lines[i].Replace("ltspass_opaque", "ltspass_opaque_orifice");

                    if (lines[i].IndexOf("lilToon.lilToonInspector") >= 0)
                        lines[i] = lines[i].Replace("lilToon.lilToonInspector", "lilToon.lilToonInspectorDPS_Orifice");

                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (pathLil != null)
            {
                string opath = outputPath + "/lts_penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathLil);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[0] = "Shader \"lilToon_Penetrator\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 13)
                    {
                        // Penetrator Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }

                    if (lines[i].IndexOf("ltspass_opaque") >= 0)
                        lines[i] = lines[i].Replace("ltspass_opaque", "ltspass_opaque_penetrator");

                    if (lines[i].IndexOf("lilToon.lilToonInspector") >= 0)
                        lines[i] = lines[i].Replace("lilToon.lilToonInspector", "lilToon.lilToonInspectorDPS_Penetrator");

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

            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.StartAssetEditing();

            if (path_customEditor != null)
            {
                string opath = Directory.GetParent(Path.GetDirectoryName(pathLil)) + "/Editor" + "/lilInspectorDPS_Orifice.cs";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_customEditor);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[14] = "    public class lilToonInspectorDPS_Orifice : ShaderGUI";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 20)
                    {
                        for (int j = 12; j <= 22; j++)
                        {
                            int stpos, edpos;
                            stpos = xslines[j].IndexOf('_') + 1;
                            edpos = xslines[j].IndexOf('(');
                            string prop_name = "dps" + xslines[j].Substring(stpos, edpos - stpos);
                            writer.WriteLine("MaterialProperty " + prop_name + ";");
                        }
                    }

                    if (i == 36)
                    {
                        int stpos, edpos;
                        stpos = xslines[12].IndexOf('(') + 1;
                        edpos = xslines[12].IndexOf(',');
                        string label = xslines[12].Substring(stpos, edpos - stpos);
                        writer.WriteLine("GUIContent dpsod = new GUIContent(" + label + ");");
                    }

                    if (i == 36)
                    {
                        for (int j = 12; j <= 22; j++)
                        {
                            int stpos, edpos;
                            stpos = xslines[j].IndexOf('_') + 1;
                            edpos = xslines[j].IndexOf('(');
                            string prop_name = "dps" + xslines[j].Substring(stpos, edpos - stpos);

                            stpos = xslines[j].IndexOf('(') + 1;
                            edpos = xslines[j].IndexOf(',');
                            string label = xslines[j].Substring(stpos, edpos - stpos);
                            if (j == 12)
                                writer.WriteLine("materialEditor.TexturePropertySingleLine(dpsod, " + prop_name + ");");
                            else
                                writer.WriteLine("materialEditor.ShaderProperty(" + prop_name + ", " + label + ");");
                        }
                    }

                    if (i == 23)
                    {
                        for (int j = 12; j <= 22; j++)
                        {
                            int stpos, edpos;
                            stpos = xslines[j].IndexOf('_') + 1;
                            edpos = xslines[j].IndexOf('(');
                            string prop_name = xslines[j].Substring(stpos, edpos - stpos);
                            writer.WriteLine("dps" + prop_name + " = FindProperty(\"_" + prop_name + "\", props);");
                        }
                    }

                    if (lines[i].IndexOf("isCustomShader  = material.shader.name.Contains") >= 0)
                        lines[i] = lines[i].Replace("Optional", "Orifice");

                    if (lines[i].IndexOf("(lilPresetCategory)") >= 0)
                        lines[i] = lines[i].Replace("(lilPresetCategory)", "(lilToonInspector.lilPresetCategory)");

                    if (lines[i].IndexOf("\"lilToon\"") >= 0)
                        lines[i] = lines[i].Replace("\"lilToon\"", "\"lilToon_Orifice\"");
                    if (lines[i].IndexOf("\"Hidden/lilToonOutline\"") >= 0)
                        lines[i] = lines[i].Replace("\"Hidden/lilToonOutline\"", "\"Hidden/lilToonOutline_Orifice\"");
                    if (lines[i].IndexOf("\"Hidden/ltspass_opaque\"") >= 0)
                        lines[i] = lines[i].Replace("\"Hidden/ltspass_opaque\"", "\"Hidden/ltspass_opaque_orifice\"");

                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                writer.Close();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }
            if (path_customEditor != null)
            {
                string opath = Directory.GetParent(Path.GetDirectoryName(pathLil)) + "/Editor" + "/lilInspectorDPS_Penetrator.cs";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_customEditor);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[14] = "    public class lilToonInspectorDPS_Penetrator : ShaderGUI";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 20)
                    {
                        for (int j = 12; j <= 22; j++)
                        {
                            int stpos, edpos;
                            stpos = xslines[j].IndexOf('_') + 1;
                            edpos = xslines[j].IndexOf('(');
                            string prop_name = "dps" + xslines[j].Substring(stpos, edpos - stpos);
                            writer.WriteLine("MaterialProperty " + prop_name + ";");
                        }
                    }

                    if (i == 36)
                    {
                        for (int j = 12; j <= 22; j++)
                        {
                            int stpos, edpos;
                            stpos = xslines[j].IndexOf('_') + 1;
                            edpos = xslines[j].IndexOf('(');
                            string prop_name = "dps" + xslines[j].Substring(stpos, edpos - stpos);

                            stpos = xslines[j].IndexOf('(') + 1;
                            edpos = xslines[j].IndexOf(',');
                            string label = xslines[j].Substring(stpos, edpos - stpos);
                            writer.WriteLine("materialEditor.ShaderProperty(" + prop_name + ", " + label + ");");
                        }
                    }

                    if (i == 23)
                    {
                        for (int j = 12; j <= 22; j++)
                        {
                            int stpos, edpos;
                            stpos = xslines[j].IndexOf('_') + 1;
                            edpos = xslines[j].IndexOf('(');
                            string prop_name = xslines[j].Substring(stpos, edpos - stpos);
                            writer.WriteLine("dps" + prop_name + " = FindProperty(\"_" + prop_name + "\", props);");
                        }
                    }

                    if (lines[i].IndexOf("isCustomShader  = material.shader.name.Contains") >= 0)
                        lines[i] = lines[i].Replace("Optional", "Penetrator");

                    if (lines[i].IndexOf("(lilPresetCategory)") >= 0)
                        lines[i] = lines[i].Replace("(lilPresetCategory)", "(lilToonInspector.lilPresetCategory)");

                    if (lines[i].IndexOf("\"lilToon\"") >= 0)
                        lines[i] = lines[i].Replace("\"lilToon\"", "\"lilToon_Penetrator\"");
                    if (lines[i].IndexOf("\"Hidden/lilToonOutline\"") >= 0)
                        lines[i] = lines[i].Replace("\"Hidden/lilToonOutline\"", "\"Hidden/lilToonOutline_Penetrator\"");
                    if (lines[i].IndexOf("\"Hidden/ltspass_opaque\"") >= 0)
                        lines[i] = lines[i].Replace("\"Hidden/ltspass_opaque\"", "\"Hidden/ltspass_opaque_penetrator\"");

                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                writer.Close();
                newAssets.Add(opath);
                log += "Generated: " + opath + "\n";
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            log += "Done.\n";
        }
    }
}

#endif
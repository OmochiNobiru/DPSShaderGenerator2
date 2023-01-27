#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

using System.IO;
using System;
using System.Threading;

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
        int curstep = 0;
        int totalstep = 92;
        string template_path = "";

        private void OnEnable()
        {
            minSize = new Vector2(480, 640);
            template_path = EditorPrefs.GetString("DPSShaderGeneratorLT", "");
        }

        protected void OnDisable()
        {
            EditorPrefs.SetString("DPSShaderGeneratorLT", template_path);
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

            EditorGUILayout.HelpBox("This script requires the lilToon custom shader template. Please download and unzip it.", MessageType.Info);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(" LilToon Custom Shader Template ", GUILayout.Width(200f));
                template_path = GUILayout.TextField(template_path);
                if (GUILayout.Button(" Browse ", GUILayout.Width(55f)))
                {
                    template_path = EditorUtility.OpenFolderPanel(" Choose the root directory of the lilToon custom shader template ", "", "");
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate"))
            {
                log = "";
                curstep = 0;
                AssetDatabase.DisallowAutoRefresh();
                GeneratelilToonDPS();
                AssetDatabase.AllowAutoRefresh();
            }

            if (GUILayout.Button("Remove lilToonDPS"))
            {
                log = "";
                RemovelilToonDPS();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(420));
            log = EditorGUILayout.TextArea(log, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void GeneratelilToonDPS()
        {
            RemovelilToonDPS();

            AssetDatabase.StartAssetEditing();

            string[] selfguids = AssetDatabase.FindAssets("DPSShaderGenerator t:script");
            if (selfguids.Length == 0)
            {
                log += "Error: Could not find this script from the asset database. Did you rename this script?";
                EditorUtility.DisplayProgressBar($"Error: ", "Could not find this script from the asset database. Did you rename this script?", (float) curstep++ / totalstep);
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                return;
            }

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
                    EditorUtility.DisplayProgressBar($"Found LilToon Shader: ", sp, (float) curstep++ / totalstep);
                }
                if (pathLilOutline == null && sp.EndsWith("/lts_o.shader"))
                {
                    pathLilOutline = sp;
                    log += "lilOutline: " + sp + "\n";
                    EditorUtility.DisplayProgressBar($"Found LilToon Outline Shader: ", sp, (float) curstep++ / totalstep);
                }
                if (pathLilOpaque == null && sp.EndsWith("/ltspass_opaque.shader"))
                {
                    pathLilOpaque = sp;
                    log += "lilOpaque: " + sp + "\n";
                    EditorUtility.DisplayProgressBar($"Found LilToon Opaque Shader: ", sp, (float) curstep++ / totalstep);
                }
                if (pathXSOrifice == null && sp.EndsWith("/XSToon2.0 Orifice.shader"))
                {
                    pathXSOrifice = sp;
                    log += "XSOrifice: " + sp + "\n";
                    EditorUtility.DisplayProgressBar($"Found XSOrifice Shader: ", sp, (float) curstep++ / totalstep);
                }
                if (pathXSPenetrator == null && sp.EndsWith("/XSToon2.0 Penetrator.shader"))
                {
                    pathXSPenetrator = sp;
                    log += "XSPenetrator: " + sp + "\n";
                    EditorUtility.DisplayProgressBar($"Found XSPenetrator Shader: ", sp, (float) curstep++ / totalstep);
                }
                if (pathOrifice == null && sp.EndsWith("/Orifice.shader"))
                {
                    pathOrifice = sp;
                    log += "Orifice: " + sp + "\n";
                    EditorUtility.DisplayProgressBar($"Found Orifice Shader: ", sp, (float) curstep++ / totalstep);
                }
            }
            string outputPath = "Assets/lilToonDPS";
            string lilOrificePath = outputPath + "/lilToonOrifice";
            string lilPenetratorPath = outputPath + "/lilToonPenetrator";
            log += "OutputPath: " + outputPath + "\n";
            Directory.CreateDirectory(outputPath);
            Directory.CreateDirectory(lilOrificePath);
            Directory.CreateDirectory(lilOrificePath + "/Editor");
            Directory.CreateDirectory(lilOrificePath + "/Shaders");
            Directory.CreateDirectory(lilPenetratorPath);
            Directory.CreateDirectory(lilPenetratorPath + "/Editor");
            Directory.CreateDirectory(lilPenetratorPath + "/Shaders");

            EditorUtility.DisplayProgressBar($"Creating Output Path: ", outputPath, (float) curstep++ / totalstep);

            if (pathLil == null || pathLilOutline == null || pathLilOpaque == null)
            {
                log += "Error: lilToon not found. Please import lilToon.";
                EditorUtility.DisplayProgressBar($"Error: ", "lilToon not found. Please import lilToon.", (float) curstep++ / totalstep);
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                return;
            }
            if (pathXSOrifice == null || pathXSPenetrator == null || pathOrifice == null)
            {
                log += "Error: DPS not found. Please import DPS.";
                EditorUtility.DisplayProgressBar($"Error: ", "DPS not found. Please import DPS.", (float) curstep++ / totalstep);
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                return;
            }
            
            string path_custom = null;
            string path_custom_insert = null;
            string path_shader_data = null;
            string path_shader_prop = null;
            string path_shader_insert = null;
            List<string> copy_files = new List<string>();
            log += "LilTemplateDirectory: " + template_path + "\n";
            string[] files = Directory.GetFiles(template_path + "/Shaders");
            foreach (string fpath in files)
            {
                string sp = fpath.Replace('\\', '/');
                if (sp.EndsWith("/custom.hlsl"))
                {
                    if (path_custom == null)
                    {
                        path_custom = sp;
                        log += "custom.hlsl: " + sp + "\n";
                        EditorUtility.DisplayProgressBar($"Found Liltoon custom.hlsl: ", sp, (float)curstep++ / totalstep);
                    }
                }
                else if (sp.EndsWith("/custom_insert.hlsl"))
                {
                    if (path_custom_insert == null)
                    {
                        path_custom_insert = sp;
                        log += "custom_insert.hlsl: " + sp + "\n";
                        EditorUtility.DisplayProgressBar($"Found Liltoon custom_insert.hlsl: ", sp, (float)curstep++ / totalstep);
                    }
                }
                else if (sp.EndsWith("/lilCustomShaderDatas.lilblock"))
                {
                    if (path_shader_data == null)
                    {
                        path_shader_data = sp;
                        log += "lilCustomShaderDatas: " + sp + "\n";
                        EditorUtility.DisplayProgressBar($"Found Liltoon lilCustomShaderDatas: ", sp, (float)curstep++ / totalstep);
                    }
                }
                else if (sp.EndsWith("/lilCustomShaderProperties.lilblock"))
                {
                    if (path_shader_prop == null)
                    {
                        path_shader_prop = sp;
                        log += "lilCustomShaderProperties: " + sp + "\n";
                        EditorUtility.DisplayProgressBar($"Found Liltoon lilCustomShaderProperties: ", sp, (float)curstep++ / totalstep);
                    }
                }
                else if (sp.EndsWith("/lilCustomShaderInsert.lilblock"))
                {
                    if (path_shader_insert == null)
                    {
                        path_shader_insert = sp;
                        log += "lilCustomShaderInsert: " + sp + "\n";
                        EditorUtility.DisplayProgressBar($"Found Liltoon lilCustomShaderInsert: ", sp, (float)curstep++ / totalstep);
                        string fname = Path.GetFileName(sp);
                        File.Copy(sp, lilOrificePath + "/Shaders/" + fname, true);
                        File.Copy(sp, lilPenetratorPath + "/Shaders/" + fname, true);
                    }
                }
                else
                {
                    string fname = Path.GetFileName(sp);
                    copy_files.Add(sp);
                }
            }

            string[] files2 = Directory.GetFiles(template_path + "/Editor");
            string path_custom_inspector = null;
            foreach (string fpath in files2)
            {
                string sp = fpath.Replace('\\', '/');
                if (sp.EndsWith("/CustomInspector.cs"))
                {
                    path_custom_inspector = sp;
                    log += "CustomInspector: " + sp + "\n";
                    EditorUtility.DisplayProgressBar($"Found Liltoon Custom Inspector: ", sp, (float)curstep++ / totalstep);
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
                    EditorUtility.DisplayProgressBar($"Found OrificeDefines: ", sp, (float) curstep++ / totalstep);
                }
                if (path_xs_PD == null && sp.EndsWith("/PenetratorDefines.cginc"))
                {
                    path_xs_PD = sp;
                    log += "PenetratorDefines: " + sp + "\n";
                    EditorUtility.DisplayProgressBar($"Found PenetratorDefines: ", sp, (float) curstep++ / totalstep);
                }
                if (path_xs_OVD == null && sp.EndsWith("/XSDefinesOrifice.cginc"))
                {
                    path_xs_OVD = sp;
                    log += "DefinesOrificeVertexData: " + sp + "\n";
                    EditorUtility.DisplayProgressBar($"Found DefinesOrificeVertexData: ", sp, (float) curstep++ / totalstep);
                }
                if (path_xs_VO == null && sp.EndsWith("/XSVertOrifice.cginc"))
                {
                    path_xs_VO = sp;
                    log += "XSVertOrifice: " + sp + "\n";
                    EditorUtility.DisplayProgressBar($"Found XSVertOrifice: ", sp, (float) curstep++ / totalstep);
                }
                if (path_xs_VP == null && sp.EndsWith("/XSVert.cginc"))
                {
                    path_xs_VP = sp;
                    log += "XSPenetratorVert: " + sp + "\n";
                    EditorUtility.DisplayProgressBar($"Found XSPenetratorVert: ", sp, (float) curstep++ / totalstep);
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
                    EditorUtility.DisplayProgressBar($"Found DPSFunctions: ", sp, (float) curstep++ / totalstep);
                }
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();

            if (path_shader_data != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilOrificePath + "/Shaders" + "/lilCustomShaderDatas.lilblock";
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_shader_data);
                lines[0] = "ShaderName \"lilToonDPSOrfice\"";
                lines[1] = "EditorName \"lilToon.OrificeInspector\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated lilCustomShaderDatas: ", opath, (float)curstep++ / totalstep);
            }
            if (path_shader_data != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilPenetratorPath + "/Shaders" + "/lilCustomShaderDatas.lilblock";
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_shader_data);
                lines[0] = "ShaderName \"lilToonDPSPenetrator\"";
                lines[1] = "EditorName \"lilToon.PenetratorInspector\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated lilCustomShaderDatas: ", opath, (float)curstep++ / totalstep);
            }

            if (path_shader_prop != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilOrificePath + "/Shaders" + "/lilCustomShaderProperties.lilblock";
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_shader_prop);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 0)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated lilCustomShaderProperties: ", opath, (float)curstep++ / totalstep);
            }
            if (path_shader_prop != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilPenetratorPath + "/Shaders" + "/lilCustomShaderProperties.lilblock";
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_shader_prop);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 0)
                    {
                        // Penetrator Properties
                        for (int j = 11; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated lilCustomShaderProperties: ", opath, (float)curstep++ / totalstep);
            }

            string orifice_vert = "";
            if (path_xs_VO != null)
            {
                string[] xslines = File.ReadAllLines(path_xs_VO);
                orifice_vert = xslines[86].Substring(0, xslines[86].IndexOf('('))
                    + "(positionOS, input.normalOS, input.tangentOS.xyz, input.vertexID);";
            }
            string penetrator_vert = "";
            if (path_xs_VP != null)
            {
                string[] xslines = File.ReadAllLines(path_xs_VP);
                penetrator_vert = xslines[119].Substring(0, xslines[119].IndexOf('('))
                    + "(positionOS, input.normalOS);";
            }

            if (path_custom != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilOrificePath + "/Shaders" + "/custom.hlsl";
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_custom);
                string[] xslines = File.ReadAllLines(path_xs_OD);
                lines[6] = "#define LIL_CUSTOM_PROPERTIES  \\";
                lines[9] = "#define LIL_CUSTOM_TEXTURES  \\";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 7)
                    {
                        // Orifice Properties
                        writer.WriteLine(xslines[1] + " \\");
                        for (int j = 3; j < xslines.Length-1; j++)
                            writer.WriteLine(xslines[j] + " \\");
                        writer.WriteLine(xslines[xslines.Length - 1]);
                    }
                    if (i == 10)
                    {
                        // Orifice Texture
                        writer.WriteLine(xslines[2]);
                    }
                    if (i == 25)
                    {
                        writer.WriteLine("#define LIL_REQUIRE_APP_NORMAL");
                        writer.WriteLine("#define LIL_REQUIRE_APP_TANGENT");
                        writer.WriteLine("#define LIL_REQUIRE_APP_VERTEXID");
                    }
                    if (i == 43)
                    {
                        writer.WriteLine("#define LIL_CUSTOM_VERTEX_OS " + orifice_vert);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated custom.hlsl: ", opath, (float)curstep++ / totalstep);
            }
            if (path_custom != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilPenetratorPath + "/Shaders" + "/custom.hlsl";
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_custom);
                string[] xslines = File.ReadAllLines(path_xs_PD);
                lines[6] = "#define LIL_CUSTOM_PROPERTIES  \\";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 7)
                    {
                        // Penetrator Properties
                        for (int j = 1; j < xslines.Length - 1; j++)
                            writer.WriteLine(xslines[j] + " \\");
                        writer.WriteLine(xslines[xslines.Length - 1]);
                    }
                    if (i == 25)
                    {
                        writer.WriteLine("#define LIL_REQUIRE_APP_NORMAL");
                        writer.WriteLine("#define LIL_REQUIRE_APP_TANGENT");
                        writer.WriteLine("#define LIL_REQUIRE_APP_VERTEXID");
                    }
                    if (i == 43)
                    {
                        writer.WriteLine("#define LIL_CUSTOM_VERTEX_OS " + penetrator_vert);
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated custom.hlsl: ", opath, (float)curstep++ / totalstep);
            }

            if (path_DPS_func != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilOrificePath + "/Shaders" + "/OrificeFunctions.cginc";
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_DPS_func);
                for (int i = 0; i <= 41; i++)
                    writer.WriteLine(lines[i]);
                for (int i = 122; i <= 160; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated OrificeFunctions: ", opath, (float)curstep++ / totalstep);
            }
            if (path_DPS_func != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilPenetratorPath + "/Shaders" + "/PenetratorFunctions.cginc";
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_DPS_func);
                for (int i = 0; i <= 41; i++)
                    writer.WriteLine(lines[i]);
                for (int i = 43; i <= 118; i++)
                    writer.WriteLine(lines[i]);
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated PenetratorFunctions: ", opath, (float)curstep++ / totalstep);
            }

            if (path_custom_insert != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilOrificePath + "/Shaders" + "/custom_insert.hlsl";
                StreamWriter writer = new StreamWriter(opath);
                writer.WriteLine("#include \"OrificeFunctions.cginc\"");
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated custom_insert.hlsl: ", opath, (float)curstep++ / totalstep);
            }
            if (path_custom_insert != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilPenetratorPath + "/Shaders" + "/custom_insert.hlsl";
                StreamWriter writer = new StreamWriter(opath);
                writer.WriteLine("#include \"PenetratorFunctions.cginc\"");
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated custom_insert.hlsl: ", opath, (float)curstep++ / totalstep);
            }

            AssetDatabase.Refresh();

            if (path_custom_inspector != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilOrificePath + "/Editor" + "/CustomInspector.cs";
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_custom_inspector);
                string[] xslines = File.ReadAllLines(path_xs_OD);
                lines[6] = "public class OrificeInspector : lilToonInspector";
                lines[12] = "private const string shaderName = \"lilToonDPSOrfice\";";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 10)
                    {
                        // Orifice Properties
                        for (int j = 1; j < xslines.Length - 1; j++)
                        {
                            int st = xslines[j].LastIndexOf(" ") + 1;
                            int ed = xslines[j].IndexOf(";");
                            string propname = xslines[j].Substring(st, ed - st);
                            writer.WriteLine("MaterialProperty " + propname + ";");
                        }
                    }
                    if (i == 27)
                    {
                        // Orifice Properties
                        for (int j = 1; j < xslines.Length - 1; j++)
                        {
                            int st = xslines[j].LastIndexOf(" ") + 1;
                            int ed = xslines[j].IndexOf(";");
                            string propname = xslines[j].Substring(st, ed - st);
                            writer.WriteLine(propname + " = FindProperty(\"" + propname + "\", props);");
                        }
                    }
                    if (i == 47)
                    {
                        // Orifice Properties
                        for (int j = 1; j < xslines.Length - 1; j++)
                        {
                            int st = xslines[j].LastIndexOf(" ");
                            int ed = xslines[j].IndexOf(";");
                            string propname = xslines[j].Substring(st, ed - st);
                            string proptitle = xslines[j].Substring(st+1, ed - (st+1));
                            if (j == 2)
                                writer.WriteLine("m_MaterialEditor.TexturePropertySingleLine(new GUIContent(\"" + proptitle + "\"), " + propname + ");");
                            else
                                writer.WriteLine("m_MaterialEditor.ShaderProperty(" + propname + ", \"" + proptitle + "\");");
                        }
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated CustomInspector: ", opath, (float)curstep++ / totalstep);
            }
            if (path_custom_inspector != null)
            {
                AssetDatabase.StartAssetEditing();
                string opath = lilPenetratorPath + "/Editor" + "/CustomInspector.cs";
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_custom_inspector);
                string[] xslines = File.ReadAllLines(path_xs_PD);
                lines[6] = "public class PenetratorInspector : lilToonInspector";
                lines[12] = "private const string shaderName = \"lilToonDPSPenetrator\";";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 10)
                    {
                        // Penetrator Properties
                        for (int j = 1; j < xslines.Length - 1; j++)
                        {
                            int st = xslines[j].LastIndexOf(" ") + 1;
                            int ed = xslines[j].IndexOf(";");
                            string propname = xslines[j].Substring(st, ed - st);
                            writer.WriteLine("MaterialProperty " + propname + ";");
                        }
                    }
                    if (i == 27)
                    {
                        // Penetrator Properties
                        for (int j = 1; j < xslines.Length - 1; j++)
                        {
                            int st = xslines[j].LastIndexOf(" ") + 1;
                            int ed = xslines[j].IndexOf(";");
                            string propname = xslines[j].Substring(st, ed - st);
                            writer.WriteLine(propname + " = FindProperty(\"" + propname + "\", props);");
                        }
                    }
                    if (i == 47)
                    {
                        // Penetrator Properties
                        for (int j = 1; j < xslines.Length - 1; j++)
                        {
                            int st = xslines[j].LastIndexOf(" ");
                            int ed = xslines[j].IndexOf(";");
                            string propname = xslines[j].Substring(st, ed - st);
                            string proptitle = xslines[j].Substring(st + 1, ed - (st + 1));
                            writer.WriteLine("m_MaterialEditor.ShaderProperty(" + propname + ", \"" + proptitle + "\");");
                        }
                    }
                    writer.WriteLine(lines[i]);
                }
                writer.Flush();
                AssetDatabase.StopAssetEditing();
                writer.Close();
                AssetDatabase.SaveAssets();
                log += "Generated: " + opath + "\n";
                EditorUtility.DisplayProgressBar($"Generated CustomInspector: ", opath, (float)curstep++ / totalstep);
            }

            AssetDatabase.Refresh();

            AssetDatabase.StartAssetEditing();
            foreach (string sp in copy_files)
            {
                string fname = Path.GetFileName(sp);
                File.Copy(sp, lilOrificePath + "/Shaders/" + fname, true);
                File.Copy(sp, lilPenetratorPath + "/Shaders/" + fname, true);
                EditorUtility.DisplayProgressBar($"Copied ShaderContainers: ", fname, (float)curstep++ / totalstep);
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
            log += "Done.\n";
        }

        private void RemovelilToonDPS()
        {

            AssetDatabase.StartAssetEditing();

            AssetDatabase.DeleteAsset("Assets/lilToonDPS");

            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();

            log += "Done.\n";
        }
    }
}

#endif

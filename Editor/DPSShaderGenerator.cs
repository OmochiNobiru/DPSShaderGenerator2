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
            string pathToonDSFTCP = null;
            string pathToonSGMTCP = null;
            string pathToonColDSFTR = null;
            string pathToonColSGMTR = null;
            string pathXSOrifice = null;
            string pathXSPenetrator = null;
            string pathOrifice = null;
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
                if (pathToonDSFTCP == null && sp.EndsWith("/Toon_DoubleShadeWithFeather_TransClipping.shader"))
                {
                    pathToonDSFTCP = sp;
                    log += "ToonDSFTCP: " + sp + "\n";
                }
                if (pathToonSGMTCP == null && sp.EndsWith("/Toon_ShadingGradeMap_TransClipping.shader"))
                {
                    pathToonSGMTCP = sp;
                    log += "ToonSGMTCP: " + sp + "\n";
                }
                if (pathToonColDSFTR == null && sp.EndsWith("/ToonColor_DoubleShadeWithFeather_Transparent.shader"))
                {
                    pathToonColDSFTR = sp;
                    log += "ToonColDSFTR: " + sp + "\n";
                }
                if (pathToonColSGMTR == null && sp.EndsWith("/ToonColor_ShadingGradeMap_Transparent.shader"))
                {
                    pathToonColSGMTR = sp;
                    log += "ToonColSGMTR: " + sp + "\n";
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

            if (pathToonDSF == null || pathToonSGM == null || pathToonColDSF == null || pathToonColSGM == null)
            {
                log += "Error: UTS not found. Please import UTS.";
                return;
            }
            if (pathXSOrifice == null || pathXSPenetrator == null || pathOrifice == null)
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
            string outputPath = Directory.GetParent(Path.GetDirectoryName(selfpath)) + "/ShaderUTS";
            log += "OutputPath: " + outputPath + "\n";
            Directory.CreateDirectory(outputPath);

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
                    if (i == 168)
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
            if (path_ucts_DSF != null)
            {
                string opath = outputPath + "/UCTS_DoubleShadeWithFeather_Penetrator.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_ucts_DSF);
                string[] xslines = File.ReadAllLines(path_xs_VP);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 168)
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
                    if (i == 199)
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
            if (path_ucts_SGM != null)
            {
                string opath = outputPath + "/UCTS_ShadingGradeMap_Penetrator.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_ucts_SGM);
                string[] xslines = File.ReadAllLines(path_xs_VP);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 199)
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
            if (path_ucts_OT != null)
            {
                string opath = outputPath + "/UCTS_Outline_Penetrator.cginc";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(path_ucts_OT);
                string[] xslines = File.ReadAllLines(path_xs_VP);
                for (int i = 0; i < lines.Length; i++)
                {
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
                    if (i == 51)
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
                    if (i == 51)
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
                lines[175] = "#include \"UCTS_Outline_Orifice.cginc\"";
                lines[206] = "#include \"UCTS_DoubleShadeWithFeather_Orifice.cginc\"";
                lines[236] = "#include \"UCTS_DoubleShadeWithFeather_Orifice.cginc\"";
                lines[261] = "#include \"UCTS_ShadowCaster_Orifice.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }

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
                lines[175] = "#include \"UCTS_Outline_Penetrator.cginc\"";
                lines[206] = "#include \"UCTS_DoubleShadeWithFeather_Penetrator.cginc\"";
                lines[236] = "#include \"UCTS_DoubleShadeWithFeather_Penetrator.cginc\"";
                lines[261] = "#include \"UCTS_ShadowCaster_Penetrator.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Penetrator Properties
                        for (int j = 11; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }

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
                lines[177] = "#include \"UCTS_Outline_Orifice.cginc\"";
                lines[208] = "#include \"UCTS_ShadingGradeMap_Orifice.cginc\"";
                lines[238] = "#include \"UCTS_ShadingGradeMap_Orifice.cginc\"";
                lines[263] = "#include \"UCTS_ShadowCaster_Orifice.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }

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
                lines[177] = "#include \"UCTS_Outline_Penetrator.cginc\"";
                lines[208] = "#include \"UCTS_ShadingGradeMap_Penetrator.cginc\"";
                lines[238] = "#include \"UCTS_ShadingGradeMap_Penetrator.cginc\"";
                lines[263] = "#include \"UCTS_ShadowCaster_Penetrator.cginc\"";
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

            if (pathToonColDSFTR != null)
            {
                string opath = outputPath + "/ToonColor_DoubleShadeWithFeather_Transparent_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonColDSFTR);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[5] = "Shader \"UnityChanToonShader/DPS/ToonColor_DoubleShadeWithFeather_Transparent_Orifice\" {";
                lines[186] = "#include \"UCTS_DoubleShadeWithFeather_Orifice.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    if (i == 180)
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
            if (pathToonColDSFTR != null)
            {
                string opath = outputPath + "/ToonColor_DoubleShadeWithFeather_Transparent_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonColDSFTR);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[5] = "Shader \"UnityChanToonShader/DPS/ToonColor_DoubleShadeWithFeather_Transparent_Penetrator\" {";
                lines[186] = "#include \"UCTS_DoubleShadeWithFeather_Penetrator.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Penetrator Properties
                        for (int j = 11; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    if (i == 180)
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

            if (pathToonColSGMTR != null)
            {
                string opath = outputPath + "/ToonColor_ShadingGradeMap_Transparent_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonColSGMTR);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[5] = "Shader \"UnityChanToonShader/DPS/ToonColor_ShadingGradeMap_Transparent_Orifice\" {";
                lines[188] = "#include \"UCTS_ShadingGradeMap_Orifice.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    if (i == 180)
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
            if (pathToonColSGMTR != null)
            {
                string opath = outputPath + "/ToonColor_ShadingGradeMap_Transparent_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonColSGMTR);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[5] = "Shader \"UnityChanToonShader/DPS/ToonColor_ShadingGradeMap_Transparent_Penetrator\" {";
                lines[188] = "#include \"UCTS_ShadingGradeMap_Penetrator.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Penetrator Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    if (i == 180)
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

            if (pathToonDSFTCP != null)
            {
                string opath = outputPath + "/Toon_DoubleShadeWithFeather_TransClipping_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonDSFTCP);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[5] = "Shader \"UnityChanToonShader/DPS/Toon_DoubleShadeWithFeather_TransClipping_Orifice\" {";
                lines[184] = "#include \"UCTS_Outline_Orifice.cginc\"";
                lines[215] = "#include \"UCTS_DoubleShadeWithFeather_Orifice.cginc\"";
                lines[244] = "#include \"UCTS_DoubleShadeWithFeather_Orifice.cginc\"";
                lines[269] = "#include \"UCTS_ShadowCaster_Orifice.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    if (i == 208 || i == 240 || i == 267)
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
            if (pathToonDSFTCP != null)
            {
                string opath = outputPath + "/Toon_DoubleShadeWithFeather_TransClipping_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonDSFTCP);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[5] = "Shader \"UnityChanToonShader/DPS/Toon_DoubleShadeWithFeather_TransClipping_Penetrator\" {";
                lines[184] = "#include \"UCTS_Outline_Penetrator.cginc\"";
                lines[215] = "#include \"UCTS_DoubleShadeWithFeather_Penetrator.cginc\"";
                lines[244] = "#include \"UCTS_DoubleShadeWithFeather_Penetrator.cginc\"";
                lines[269] = "#include \"UCTS_ShadowCaster_Penetrator.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Penetrator Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    if (i == 208 || i == 240 || i == 267)
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

            if (pathToonSGMTCP != null)
            {
                string opath = outputPath + "/Toon_ShadingGradeMap_TransClipping_Orifice.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonSGMTCP);
                string[] xslines = File.ReadAllLines(pathXSOrifice);
                lines[5] = "Shader \"UnityChanToonShader/DPS/Toon_ShadingGradeMap_TransClipping_Orifice\" {";
                lines[186] = "#include \"UCTS_Outline_Orifice.cginc\"";
                lines[218] = "#include \"UCTS_ShadingGradeMap_Orifice.cginc\"";
                lines[248] = "#include \"UCTS_ShadingGradeMap_Orifice.cginc\"";
                lines[273] = "#include \"UCTS_ShadowCaster_Orifice.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Orifice Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    if (i == 210 || i == 243 || i == 271)
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
            if (pathToonSGMTCP != null)
            {
                string opath = outputPath + "/Toon_ShadingGradeMap_TransClipping_Penetrator.shader";
                AssetDatabase.DeleteAsset(opath);
                StreamWriter writer = new StreamWriter(opath);
                string[] lines = File.ReadAllLines(pathToonSGMTCP);
                string[] xslines = File.ReadAllLines(pathXSPenetrator);
                lines[5] = "Shader \"UnityChanToonShader/DPS/Toon_ShadingGradeMap_TransClipping_Penetrator\" {";
                lines[186] = "#include \"UCTS_Outline_Penetrator.cginc\"";
                lines[218] = "#include \"UCTS_ShadingGradeMap_Penetrator.cginc\"";
                lines[248] = "#include \"UCTS_ShadingGradeMap_Penetrator.cginc\"";
                lines[273] = "#include \"UCTS_ShadowCaster_Penetrator.cginc\"";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 11)
                    {
                        // Penetrator Properties
                        for (int j = 12; j <= 23; j++)
                            writer.WriteLine(xslines[j]);
                    }
                    if (i == 210 || i == 243 || i == 271)
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

            log += "Done.\n";

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

#endif
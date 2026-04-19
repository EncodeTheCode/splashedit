using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;

using SplashEdit.RuntimeCode;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SplashEdit.EditorCode
{
#if UNITY_EDITOR
    // ================================
    // AUTO DEFINE SYMBOL BOOTSTRAP
    // ================================
    [InitializeOnLoad]
    internal static class ScriptingDefinesBootstrap
    {
        private const string LUA = "LUA_SUPPORT";
        private const string PYTHON = "PYTHON_SUPPORT";

        static ScriptingDefinesBootstrap()
        {
            var group = EditorUserBuildSettings.selectedBuildTargetGroup;

            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

            defines = Add(defines, LUA);
            defines = Add(defines, PYTHON);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
        }

        private static string Add(string defines, string symbol)
        {
            if (!defines.Contains(symbol))
            {
                if (defines.Length > 0)
                    defines += ";";

                defines += symbol;
            }
            return defines;
        }
    }
#endif

#if LUA_SUPPORT && PYTHON_SUPPORT
    [ScriptedImporter(2, new[] { "lua", "py" })]
    public class ScriptImporter : ScriptedImporter
#elif LUA_SUPPORT
    [ScriptedImporter(2, "lua")]
    public class ScriptImporter : ScriptedImporter
#elif PYTHON_SUPPORT
    [ScriptedImporter(2, "py")]
    public class ScriptImporter : ScriptedImporter
#else
    // No importer compiled
#endif

#if LUA_SUPPORT || PYTHON_SUPPORT
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string ext = Path.GetExtension(ctx.assetPath).ToLowerInvariant();
            string code = File.ReadAllText(ctx.assetPath);
            string fileName = Path.GetFileName(ctx.assetPath);

            var text = new TextAsset(code);
            text.name = fileName;
            ctx.AddObjectToAsset("Text", text);

#if LUA_SUPPORT
            if (ext == ".lua")
            {
                var asset = ScriptableObject.CreateInstance<LuaFile>();
                asset.Init(code);
                asset.name = fileName;

                ctx.AddObjectToAsset("Script", asset);
                ctx.SetMainObject(asset);
                return;
            }
#endif

#if PYTHON_SUPPORT
            if (ext == ".py")
            {
                var asset = ScriptableObject.CreateInstance<PythonFile>();
                asset.Init(code);
                asset.name = fileName;

                ctx.AddObjectToAsset("Script", asset);
                ctx.SetMainObject(asset);
                return;
            }
#endif

            ctx.SetMainObject(text);
        }
    }
#endif
}

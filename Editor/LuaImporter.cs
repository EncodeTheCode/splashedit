using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;

using SplashEdit.RuntimeCode;

namespace SplashEdit.EditorCode
{
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
    // Nothing enabled, no importer compiled, zero risk. In this specific else case do not declare ScriptImporter at all.
    // The if elif statements determine whether you allow either Lua or Python support or just one of them. Python support might need to be added incase Python were to be allowed. I hope that Python support will be added eventually, it'd be greatly beneficial to this project.
#endif
#if LUA_SUPPORT || PYTHON_SUPPORT
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string ext = Path.GetExtension(ctx.assetPath).ToLowerInvariant();
            string code = File.ReadAllText(ctx.assetPath);
            string fileName = Path.GetFileName(ctx.assetPath);

            // Always safe fallback
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

            // Absolute safety fallback (should never hit, but prevents import failure)
            ctx.SetMainObject(text);
        }
    }
#endif
}

using System.Linq;
using UnityEditor;

namespace AppCore.EditorUtilities
{
    public static class PreprocessorManager
    {
        public static class Consts
        {
            public static readonly string PREPROCESSOR_APP_METRICA = "APCR_APPMETRICA";
        }

        public static string[] GetAll(BuildTargetGroup buildTargetGroup)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';');
        }

        public static bool Has(BuildTargetGroup buildTargetGroup, string preprocessor)
        {
            return GetAll(buildTargetGroup).Contains(preprocessor);
        }

        public static void Add(BuildTargetGroup buildTargetGroup, string preprocessor)
        {
            var list = GetAll(buildTargetGroup).ToList();

            if (list.Contains(preprocessor))
                return;

            list.Add(preprocessor);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", list));
        }

        public static void Remove(BuildTargetGroup buildTargetGroup, string preprocessor)
        {
            var list = GetAll(buildTargetGroup).ToList();

            if (list.Remove(preprocessor))
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", list));
        }
    }
}
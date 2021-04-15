#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

namespace AppCore.EditorUtilities
{
    [Serializable]
    public class EnumInfo
    {
        [SerializeField] string _name = "";
        [SerializeField] List<string> _elements = new List<string>();

        public EnumInfo(string name)
        {
            _name = name;
            _elements = new List<string>();
        }

        string _path;

        public string Name { get { return _name; } }
        public List<string> Elements { get { return _elements; } }
        public string Path { get { return $"{ClassfileGenerator.GENERATED_CLASSFILES_PATH}{_name}.cs"; } }

        public bool HasToBeUpdated()
        {
            Type type = TypeUtility.GetTypeByName(_name);

            if (type != null)
            {
                var names = Enum.GetNames(type);
                var elements = Elements.ToArray();

                bool hasToBeReplaced = elements.Length != names.Length;

                if (!hasToBeReplaced)
                {
                    for (int i = 0; i < elements.Length; i++)
                    {
                        if (names[i] != elements[i])
                        {
                            hasToBeReplaced = true;
                            break;
                        }
                    }
                }

                return hasToBeReplaced;
            }

            return true;
        }
    }

    [Serializable]
    public class ClassInfo
    {
        public string[] usings;
        public string nspace;
        public string name;
        public string parentClass;
        public string[] interfaces;
        public string[] body;
        public string Path { get { return $"{ClassfileGenerator.GENERATED_CLASSFILES_PATH}{name}.cs"; } }
    }

    public static class ClassfileGenerator
    {
        public static readonly string GENERATED_CLASSFILES_PATH = "Assets/_AppCore/Scripts/Generated/";



        public static void CreateEnum(EnumInfo enumInfo)
        {
            try
            {
                string path = enumInfo.Path;
                Debug.Log("Writing to Classfile: " + path);

                using (StreamWriter outfile = new StreamWriter(path))
                {
                    outfile.WriteLine($"public enum {enumInfo.Name}");
                    outfile.WriteLine("{");
                    foreach (var element in enumInfo.Elements)
                    {
                        if (!string.IsNullOrEmpty(element))
                            outfile.WriteLine($"    {element},");
                    }
                    outfile.WriteLine("}");
                }

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public static void CreateClass(ClassInfo classInfo)
        {
            try
            {
                string path = classInfo.Path;
                Debug.Log("Creating Classfile: " + path);

                using (StreamWriter outfile = new StreamWriter(path))
                {
                    if (classInfo.usings != null && classInfo.usings.Length > 0)
                    {
                        foreach (var str in classInfo.usings)
                            outfile.WriteLine($"using {str};");

                        outfile.WriteLine("");
                    }

                    string parentClass = string.IsNullOrEmpty(classInfo.parentClass) ? string.Empty : classInfo.parentClass;
                    string interfaces = classInfo.interfaces == null ? "" : string.Join(", ", classInfo.interfaces);
                    string nestingFrom = string.Empty;

                    if (!string.IsNullOrEmpty(parentClass))
                    {
                        nestingFrom += $" : {parentClass}";
                    }
                    else if (!string.IsNullOrEmpty(interfaces))
                    {
                        if (!string.IsNullOrEmpty(nestingFrom))
                            nestingFrom += ", ";
                        else
                            nestingFrom += " : ";

                        nestingFrom += interfaces;
                    }

                    bool hasNamespace = !string.IsNullOrEmpty(classInfo.nspace);

                    if (hasNamespace)
                        outfile.WriteLine($"namespace {classInfo.nspace}\n{{");

                    outfile.WriteLine($"public class {classInfo.name} {nestingFrom}\n{{");
                    
                    foreach (var str in classInfo.body)
                        outfile.WriteLine(str);

                    if (hasNamespace)
                        outfile.WriteLine("}");

                    outfile.WriteLine("}");
                }

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}

#endif
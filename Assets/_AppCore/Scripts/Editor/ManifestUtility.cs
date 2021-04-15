using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AppCore
{
    public static class ManifestUtility
    {
        public static readonly string ManifestPath = "Assets/Plugins/Android/AndroidManifest.xml";

        public enum Result
        {
            ErrorReadingFile,
            Success,
            FileWasCorrect,
            ErrorWritingFile,
        }

        public static bool HasManifest()
        {
            return File.Exists(ManifestPath);
        }

        public static bool IsDebuggable()
        {
            if (!HasManifest())
                return false;

            string fileString = null;

            try
            {
                StreamReader streamReader = new StreamReader(ManifestPath, true);

                fileString = streamReader.ReadToEnd();

                streamReader.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e);

                return false;
            }

            string searchingValue = "android:debuggable=\"";

            int index = fileString.IndexOf(searchingValue) + searchingValue.Length;

            if (index != -1)
            {
                var substring = fileString.Substring(index, 4);

                return substring == "true";
            }

            return false;
        }

        public static bool SetDebuggableValue(bool value)
        {
            if (!HasManifest())
                return false;

            string fileString = null;

            try
            {
                StreamReader streamReader = new StreamReader(ManifestPath, true);

                fileString = streamReader.ReadToEnd();

                streamReader.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e);

                return false;
            }

            string searchingValue = "android:debuggable=\"";

            int index = fileString.IndexOf(searchingValue) + searchingValue.Length;

            if (index != -1)
            {
                if (value)
                {
                    if (fileString.Substring(index, 5) == "false")
                    {
                        fileString = fileString.Remove(index, 5).Insert(index, "true");
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (fileString.Substring(index, 4) == "true")
                    {
                        fileString = fileString.Remove(index, 4).Insert(index, "false");
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            try
            {
                var streamWriter = new StreamWriter(ManifestPath);

                streamWriter.Write(fileString);

                streamWriter.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e);

                return false;
            }

            return true;
        }
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace Atavism
{

    public class AtavismUnityUtility
    {

        /// <summary>
        /// Used to get an asset with a specified name.
        /// </summary>
        /// <param name="fileName">The name of the Asset</param>
        /// <returns>An Object array of assets.</returns>
        public static string GetAssetPath(string fileName)
        {
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            FileInfo[] goFileInfo = directory.GetFiles(fileName, SearchOption.AllDirectories);

            int i = 0;
            int goFileInfoLength = goFileInfo.Length;
            FileInfo tempGoFileInfo;
            string tempFilePath;
            //	Object tempGO;
            for (; i < goFileInfoLength; i++)
            {
                tempGoFileInfo = goFileInfo[i];
                if (tempGoFileInfo == null)
                    continue;

#if UNITY_EDITOR_WIN
                tempFilePath = tempGoFileInfo.Directory.FullName.Remove(0, Application.dataPath.Length - 6);
                return tempFilePath + "\\";
#endif
#if UNITY_EDITOR_OSX
			tempFilePath = tempGoFileInfo.Directory.FullName.Remove(0, Application.dataPath.Length-6);
			return tempFilePath + "/";
#endif
#if UNITY_EDITOR_LINUX
			tempFilePath = tempGoFileInfo.Directory.FullName.Remove(0, Application.dataPath.Length-6);
			return tempFilePath + "/";
#endif

            }

            return null;
        }

    }
}
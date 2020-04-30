using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectHelper
{
    public class ProjectFileFilter
    {
        public static string[] SearchForDirectory (string directoryPath, bool searchingHidden, IEnumerable<string> matchPattern = null, IEnumerable<string> exceptPattern = null)
        {
            return SearchForDirectory (new DirectoryInfo (directoryPath), searchingHidden, matchPattern, exceptPattern);
        }
        public static string[] SearchForDirectory (DirectoryInfo directoryInfo, bool searchingHidden, IEnumerable<string> matchPattern = null, IEnumerable<string> exceptPattern = null)
        {
            ConcurrentBag<string> filesPathBag = new ConcurrentBag<string> ();
            searchingFiles (filesPathBag, searchingHidden, directoryInfo, matchPattern, exceptPattern);
            return filesPathBag.ToArray ();

        }
        static void searchingFiles (ConcurrentBag<string> filesPathCollection, bool searchingHidden, DirectoryInfo dir, IEnumerable<string> matchPattern, IEnumerable<string> exceptPattern)
        {
            Task.WaitAll (Task.Run (() =>
            {
                Parallel.ForEach (dir.GetFiles (), (fileInfo) =>
                {
                    if (!searchingHidden && ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                    {
                        return;
                    }
                    bool isMatch = false;

                    if (matchPattern != null)
                    {

                        foreach (var pattern in matchPattern)
                        {
                            if (Regex.IsMatch (fileInfo.FullName, pattern))
                            {
                                isMatch = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        isMatch = true;
                    }

                    if (isMatch && exceptPattern != null)
                    {

                        foreach (var pattern in exceptPattern)
                        {
                            if (Regex.IsMatch (fileInfo.FullName, pattern))
                            {
                                isMatch = false;
                                break;
                            }
                        }
                    }
                    if (isMatch)
                    {
                        filesPathCollection.Add (fileInfo.FullName);
                    }

                });
            }), Task.Run (() =>
            {
                Parallel.ForEach (dir.GetDirectories (), (dirInfo) =>
                {
                    if (!searchingHidden && ((dirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                    {
                        return;
                    }
                    searchingFiles (filesPathCollection, searchingHidden, dirInfo, matchPattern, exceptPattern);
                });
            }));

        }
    }
}
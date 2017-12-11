using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.DAL
{
    public static class DALManager
    {
        private static IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForAssembly();

        /// <summary>
        /// Returns a list of all items of type T, or one item if itemID is specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public static ObservableCollection<T> Load<T>(object itemID = null)
        {
            var result = new ObservableCollection<T>();
            //var binaryFormatter = new SoapFormatter();
            var binaryFormatter = new BinaryFormatter();
            string directoryName = GetStorageDirectoryName<T>();

            if (!isoStorage.DirectoryExists(directoryName))
            {
                isoStorage.CreateDirectory(directoryName);
                return result;
            }

            var fileNames = isoStorage.GetFileNames(GetFilePattern<T>(itemID?.ToString(), directoryName));

            foreach (var fileName in fileNames)
            {
                using (var fileStream = new IsolatedStorageFileStream(
                    Path.Combine(directoryName, fileName), FileMode.Open,
                    FileAccess.Read, FileShare.Read, isoStorage))
                {
                    result.Add((T)binaryFormatter.Deserialize(fileStream));
                }
            }
  
            return result;
        }

        /// <summary>
        /// Inserts an item of type T, using itemID if specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="itemID"></param>
        public static void Insert<T>(this T obj, object itemID)
        {
            //var binaryFormatter = new SoapFormatter();
            var binaryFormatter = new BinaryFormatter();
            string directoryName = GetStorageDirectoryName<T>();

            if (!isoStorage.DirectoryExists(directoryName))
                isoStorage.CreateDirectory(directoryName);

            string fileName = GetFilePattern<T>(itemID?.ToString() ?? typeof(T).Name, directoryName);

            using (var fileStream = new IsolatedStorageFileStream(
                fileName, FileMode.Create, FileAccess.Write, FileShare.None, isoStorage))
            {
                binaryFormatter.Serialize(fileStream, obj);
            }
        }

        public static void Update<T>(this T obj, object itemID)
        {
            obj.Insert(itemID);
        }

        /// <summary>
        /// Delete all items of type T, or one item if itemID is specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="itemID"></param>
        public static void Delete<T>(this T obj, object itemID)
        {
            string directoryName = GetStorageDirectoryName<T>();
            var fileNames = isoStorage.GetFileNames(GetFilePattern<T>(itemID?.ToString(), directoryName));

            foreach (var fileName in fileNames)
            {
                isoStorage.DeleteFile(GetFilePattern<T>(fileName, directoryName));
            }
        }

        private static string GetFilePattern<T>(string fileName, string directoryName = null)
        {
            return Path.Combine(directoryName ?? GetStorageDirectoryName<T>(), fileName ?? "*");
        }

        private static string GetStorageDirectoryName<T>()
        {
            return typeof(T).Name.PluralForm();
        }
    }
}

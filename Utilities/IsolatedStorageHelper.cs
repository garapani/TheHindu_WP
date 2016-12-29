﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Utilities
{
    public static class IsolatedStorageHelper
    {
        #region Methods

        private readonly static object ReadLock = new object();

        public static T GetObject<T>(string key, IEnumerable<Type> types = null)
        {
            try
            {
                lock (ReadLock)
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                    {
                        string serializedObject = IsolatedStorageSettings.ApplicationSettings[key].ToString();
                        return Deserialize<T>(serializedObject, types);
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("IsolatedStorageHelper:" + exception);
                }
            }
            return default(T);
        }

        public static float GetFloat(string key, float defaultValue = 0.0f)
        {
            try
            {
                lock (ReadLock)
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                    {
                        return Convert.ToSingle(IsolatedStorageSettings.ApplicationSettings[key]);
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("IsolatedStorageHelper:" + exception);
                }
            }
            return defaultValue;
        }

        public static string GetString(string key, string defaultValue = "")
        {
            try
            {
                lock (ReadLock)
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                    {
                        return Convert.ToString(IsolatedStorageSettings.ApplicationSettings[key]);
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("IsolatedStorageHelper:" + exception);
                }
            }
            return defaultValue;
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            try
            {
                lock (ReadLock)
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                    {
                        return Convert.ToBoolean(IsolatedStorageSettings.ApplicationSettings[key]);
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("IsolatedStorageHelper:" + exception);
                }
            }
            return defaultValue;
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            try
            {
                lock (ReadLock)
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                    {
                        return Convert.ToInt32(IsolatedStorageSettings.ApplicationSettings[key]);
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("IsolatedStorageHelper:" + exception);
                }
            }
            return defaultValue;
        }

        public static DateTime GetDateTime(string key, DateTime defaultValue)
        {
            try
            {
                lock (ReadLock)
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                    {
                        return (DateTime)IsolatedStorageSettings.ApplicationSettings[key];
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("IsolatedStorageHelper:" + exception);
                }
            }
            return defaultValue;
        }

        public static void SaveObject<T>(string key, T objectToSave, IEnumerable<Type> types = null) where T : class
        {
            if (objectToSave != null)
            {
                try
                {
                    lock (ReadLock)
                    {
                        string serializedObject = Serialize(objectToSave, types);
                        IsolatedStorageSettings.ApplicationSettings[key] = serializedObject;
                        IsolatedStorageSettings.ApplicationSettings.Save();
                    }
                }
                catch (Exception exception)
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine("IsolatedStorageHelper:" + exception);
                    }
                }
            }
        }

        public static void DeleteObject(string key)
        {
            try
            {
                lock (ReadLock)
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove(key);
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("IsolatedStorageHelper:" + exception);
                }
            }
        }

        public static void SaveSettingValueImmediately(string propertyName, object value)
        {
            try
            {
                lock (ReadLock)
                {
                    IsolatedStorageSettings.ApplicationSettings[propertyName] = value;
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
            catch (Exception exception)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("IsolatedStorageHelper:" + exception);
                }
            }
        }

        #endregion Methods

        #region Private Methods

        private static string Serialize(object objectToSerialize, IEnumerable<Type> types)
        {
            string result = null;

            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(objectToSerialize.GetType(), types);
                serializer.WriteObject(memoryStream, objectToSerialize);
                memoryStream.Position = 0;
                using (StreamReader reader = new StreamReader(memoryStream))
                {
                    result = reader.ReadToEnd();
                    memoryStream = null;
                }
            }
            finally
            {
                //if (memoryStream != null)
                //    memoryStream.Dispose();
            }

            return result;
        }

        private static T Deserialize<T>(string jsonString, IEnumerable<Type> types)
        {
            using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), types);
                return (T)serializer.ReadObject(memoryStream);
            }
        }

        #endregion Private Methods
    }
}
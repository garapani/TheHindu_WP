using System;
using System.IO;
using System.Xml.Serialization;
using TheHindu.Utils;

namespace TheHindu.Helper
{
    public class CategoriesHelper
    {
        private ListOfCategories _listOfcategories;

        public ListOfCategories ListOfCategories
        {
            get
            {
                return _listOfcategories;
            }
        }

        public CategoriesHelper()
        {
            _listOfcategories = new ListOfCategories();
            ReadCategoriesXml();
        }

        public async void ReadCategoriesXml()
        {
            try
            {
                string configFileUri = "ms-appx:///Config/AllCategories.xml";

                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(configFileUri));
                Stream fs = await file.OpenStreamForReadAsync();
                var serializer = new XmlSerializer(typeof(ListOfCategories));
                _listOfcategories = (ListOfCategories)serializer.Deserialize(fs);
            }
            catch (Exception)
            { }
        }
    }
}
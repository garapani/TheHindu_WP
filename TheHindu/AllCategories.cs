//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.6407
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TheHindu.Common;

//
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
//
namespace TheHindu.Utils
{
    public partial class ListOfCategories : BindableBase
    {
        private List<CategoryDetails> _categories;

        private ListOfCategoriesGroup _groups;

        public ListOfCategories()
        {
            _categories = new List<CategoryDetails>();
            _groups = new ListOfCategoriesGroup();
        }

        [System.Xml.Serialization.XmlElementAttribute("Category")]
        public List<CategoryDetails> Categories
        {
            get
            {
                return this._categories;
            }
            set
            {
                SetProperty(ref this._categories, value);
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute("Group")]
        //public ListOfCategoriesGroup Groups
        //{
        //    get
        //    {
        //        return this._groups;
        //    }
        //    set
        //    {
        //        SetProperty(ref this._groups, value);
        //    }
        //}
    }

    public partial class CategoryDetails : BindableBase
    {
        private string categoryNameField;

        private string isPinnedField;

        private string isGroupField;

        private bool StrEQYes(string str)
        {
            return string.Equals(str, "yes", StringComparison.OrdinalIgnoreCase);
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CategoryName")]
        public string CategoryName
        {
            get
            {
                return this.categoryNameField;
            }
            set
            {
                SetProperty(ref this.categoryNameField, value);
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("IsPinned")]
        public string IsPinnedString
        {
            get
            {
                return this.isPinnedField;
            }
            set
            {
                SetProperty(ref this.isPinnedField, value);
            }
        }

        [XmlIgnore]
        public bool IsPinned
        {
            get
            {
                return StrEQYes(this.IsPinnedString);
            }
            set
            {
                this.SetProperty(ref this.isPinnedField, value ? "yes" : "no");
                //SetProperty(ref this.IsPinnedString, value ? "yes" : "no");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("IsGroup")]
        public string IsGroupString
        {
            get
            {
                return this.isGroupField;
            }
            set
            {
                //this.isGroupField = value;
                this.SetProperty(ref this.isGroupField, value);
            }
        }

        [XmlIgnore]
        public bool IsGroup
        {
            get
            {
                return StrEQYes(this.IsGroupString);
            }
            set
            {
                //this.IsGroupString = value ? "yes" : "no";
                SetProperty(ref this.isGroupField, value ? "yes" : "no");
            }
        }

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute("GroupName")]
        //public string GroupName
        //{
        //    get
        //    {
        //        return this.groupNameField;
        //    }
        //    set
        //    {
        //        //this.groupNameField = value;
        //        SetProperty(ref this.groupNameField, value);
        //    }
        //}

        //private CategoryDetails _group;
        //[System.Xml.Serialization.XmlElementAttribute("Group")]
        //public CategoryDetails Group
        //{
        //    get
        //    {
        //        return this._group;
        //    }
        //    set
        //    {
        //        //this._group = value;
        //        SetProperty(ref this._group, value);
        //    }
        //}

        private ListOfCategoriesGroup _group;

        [System.Xml.Serialization.XmlElementAttribute("Group")]
        public ListOfCategoriesGroup Group
        {
            get
            {
                return this._group;
            }
            set
            {
                SetProperty(ref this._group, value);
            }
        }
    }

    public partial class ListOfCategoriesGroup : BindableBase
    {
        private List<CategoryDetails> _categories;

        public ListOfCategoriesGroup()
        {
            _categories = new List<CategoryDetails>();
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Category")]
        public List<CategoryDetails> Categories
        {
            get
            {
                return this._categories;
            }
            set
            {
                SetProperty(ref this._categories, value);
            }
        }

        ///// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute("Name")]
        //public string Name
        //{
        //    get
        //    {
        //        return this._name;
        //    }
        //    set
        //    {
        //        SetProperty(ref this._name, value);
        //    }
        //}
    }
}
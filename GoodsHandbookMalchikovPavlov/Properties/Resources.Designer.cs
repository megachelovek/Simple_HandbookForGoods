﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoodsHandbookMalchikovPavlov.Properties {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GoodsHandbookMalchikovPavlov.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на usage: add-count &lt;product id&gt; &lt;count&gt;.
        /// </summary>
        internal static string ADDCOUNT_USAGE {
            get {
                return ResourceManager.GetString("ADDCOUNT_USAGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Product type \&quot;{0}\&quot; does not exist
        ///            List of available product types:.
        /// </summary>
        internal static string CREATE_PRODUCTTYPEDOESNTEXIST {
            get {
                return ResourceManager.GetString("CREATE_PRODUCTTYPEDOESNTEXIST", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на usage: delete &lt;product id&gt;.
        /// </summary>
        internal static string DELETE_USAGE {
            get {
                return ResourceManager.GetString("DELETE_USAGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на usage: get-item &lt;product id&gt;.
        /// </summary>
        internal static string GETITEM_USAGE {
            get {
                return ResourceManager.GetString("GETITEM_USAGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Handbook is a product catalog program
        ///            Available commands:
        ///            create &lt;product type&gt; - creates new product and adds it to a catalog
        ///                                                       &quot;product type&quot; can be specified inline
        ///                                                       with command via optional parameter
        ///            list &lt;product type&gt;      - lists products presented in catalog
        ///                                                       to filter output by product type
        ///       [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string HELP {
            get {
                return ResourceManager.GetString("HELP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Product type \&quot;{0}\&quot; does not exist
        ///            List of available product types:.
        /// </summary>
        internal static string LIST_PRODUCTTYPEDOESNTEXIST {
            get {
                return ResourceManager.GetString("LIST_PRODUCTTYPEDOESNTEXIST", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на list &lt;product type&gt; [-full].
        /// </summary>
        internal static string LIST_USAGE {
            get {
                return ResourceManager.GetString("LIST_USAGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на usage: sub-count &lt;product id&gt; &lt;count&gt;.
        /// </summary>
        internal static string SUBSTRACTCOUNT_USAGE {
            get {
                return ResourceManager.GetString("SUBSTRACTCOUNT_USAGE", resourceCulture);
            }
        }
    }
}
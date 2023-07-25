using FairyGUI.Utils;

namespace FUIEditor
{
    public class VariableInfo
    {
        public string TypeName { get; set; }

        public string PackageId { get; set; }

        public ComponentInfo ComponentInfo { get; set; }
        
        public string VariableName { get; set; }
        
        /// <summary>
        /// 是否是默认的名称，比如 n0, n1, n2
        /// </summary>
        public bool IsDefaultName { get; set; }
        
        /// <summary>
        /// 是否是编辑器内部指定的名称，比如 title, icon
        /// </summary>
        public bool IsAppointName { get; set; }
        
        public XML displayXML { get; set; }

        public string LanguageKey { get; set; }
        
        public bool IsExported { get; set; }
    }
}
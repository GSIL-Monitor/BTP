using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销商身份设置值 
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributionIdentityResultDTO
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public int ValueType { get; set; }

        [DataMember]
        public int NameCategory { get; set; }

        [DataMember]
        public bool IsRequired { get; set; }

        public DistributionIdentityResultDTO()
        {
            
        }
        public DistributionIdentityResultDTO(string name,string value,int valueType,int nameCategory,bool isRequired)
        {
            Name = name;
            Value = value;
            ValueType = valueType;
            NameCategory = nameCategory;
            IsRequired = isRequired;

            //要求：value为空是，都显示“无”
            var textValueType = 1;
            if (string.IsNullOrWhiteSpace(value))
            {
                if (valueType != textValueType)
                    ValueType = textValueType;
                Value = "无";
            }
        }
    }
}

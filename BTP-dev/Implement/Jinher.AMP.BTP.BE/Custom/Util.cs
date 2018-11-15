using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BE.Custom
{
    public static class Util
    {
        public static List<SimpleAttributeDTO> ConvertToAttributeList(string attributes)
        {
            if (string.IsNullOrEmpty(attributes))
            {
                return null;
            }

            List<SimpleAttributeDTO> attributeList = new List<SimpleAttributeDTO>();
            string[] attributesArray = attributes.Split('，');

            for (var i = 0; i < attributesArray.Count(); i++)
            {
                SimpleAttributeDTO simpleAttributeDTO = new Deploy.CustomDTO.SimpleAttributeDTO();
                string attributesString = attributesArray[i];
                
                int point = attributesString.IndexOf('：');
                if (point > -1)
                {
                    simpleAttributeDTO.AttrName = attributesString.Substring(0, point);
                    simpleAttributeDTO.AttrValue = attributesString.Substring(point + 1);
                    attributeList.Add(simpleAttributeDTO);
                }
               
            }

            return attributeList;
        }

        public static List<Attribute> GetColorSizes(Guid appId)
        {
            List<Attribute> attrs = new List<Attribute>();
            attrs.Add(new Attribute() { Id = Guid.Parse("324244CB-8E9F-45B3-A1E4-53FC1A25A11C"), Name = "颜色", SubId = Guid.Empty, AppId = appId, IsDel = false });
            attrs.Add(new Attribute() { Id = Guid.Parse("844D8816-1692-45CB-9FE5-F82C061A30E7"), Name = "尺寸", SubId = Guid.Empty, AppId = appId, IsDel = false });
            return attrs;
        }
        public static List<Guid> GetColorSizesId()
        {
            List<Guid> attrs = new List<Guid>();
            attrs.Add(Guid.Parse("324244CB-8E9F-45B3-A1E4-53FC1A25A11C"));
            attrs.Add(Guid.Parse("844D8816-1692-45CB-9FE5-F82C061A30E7"));
            return attrs;
        }
    }
}

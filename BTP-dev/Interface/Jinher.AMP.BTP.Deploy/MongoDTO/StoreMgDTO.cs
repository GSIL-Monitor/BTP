using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Jinher.AMP.BTP.Deploy.MongoDTO
{
    [Serializable()]
    [DataContract]
    [BsonIgnoreExtraElements(true)]
    public class StoreMgDTO
    {
        /// <summary>
        /// 门店详细地址
        /// </summary>
        [DataMember]
        [BsonId]
        //[BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        [DataMember]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 提交人ID
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }
        /// <summary>
        /// 门店详细地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 门店电话
        /// </summary>
        [DataMember]
        public string Phone { get; set; }
        /// <summary>
        /// 门店图片
        /// </summary>
        [DataMember]
        public string picture { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        [DataMember]
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        [DataMember]
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        [DataMember]
        public string District { get; set; }
        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 省份CODE
        /// </summary>
        [DataMember]
        public string ProvinceCode { get; set; }
        /// <summary>
        /// 城市CODE
        /// </summary>
        [DataMember]
        public string CityCode { get; set; }
        /// <summary>
        /// 县区CODE
        /// </summary>
        [DataMember]
        public string DistrictCode { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [DataMember]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// 坐标
        /// </summary>
        [DataMember]
        public double[] Location { get; set; }


    }

}

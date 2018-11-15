using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// app关税
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppDutyDTO
    {
        /// <summary>
        /// 店铺Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 店铺商品关税总和
        /// </summary>
        [DataMember]
        public decimal TotalDuty { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember]
        public List<ComDutyReDTO> Coms { get; set; }

    }
    /// <summary>
    /// 关税
    /// </summary>
    [Serializable]
    [DataContract]
    public class CreateOrderDutyResultDTO
    {
        /// <summary>
        /// 关税
        /// </summary>
        public CreateOrderDutyResultDTO()
        {
            List = new List<AppDutyDTO>();
        }

        /// <summary>
        /// 关税
        /// </summary>
        public CreateOrderDutyResultDTO(Dictionary<Guid, List<ComDutyReDTO>> dict)
            : this()
        {
            if (dict == null || !dict.Any())
                return;

            foreach (var kv in dict)
            {
                var appDutyDTO = new AppDutyDTO
                    {
                        AppId = kv.Key,
                        Coms = kv.Value
                    };
                if (kv.Value != null && kv.Value.Any())
                    appDutyDTO.TotalDuty = kv.Value.Sum(c => c.Duty * c.Num);
                TotalDuty += appDutyDTO.TotalDuty;
                List.Add(appDutyDTO);
            }
        }

        /// <summary>
        /// 关税总额
        /// </summary>
        [DataMember]
        public decimal TotalDuty { get; set; }

        /// <summary>
        /// 支持7天无理由退货
        /// </summary>
        [DataMember]
        public bool Isnsupport { get; set; }

        /// <summary>
        /// 列表
        /// </summary>
        [DataMember]
        public List<AppDutyDTO> List { get; set; }
    }


    [Serializable]
    [DataContract]
    public class ComDutyReDTO : ComScoreCheckDTO
    {

        public static ComDutyReDTO FromRequest(ComScoreCheckDTO request, Guid appId)
        {
            ComDutyReDTO result = new ComDutyReDTO();
            result.ItemId = request.ItemId;
            result.CommodityId = request.CommodityId;
            result.RealPrice = request.RealPrice;
            result.ColorAndSize = request.ColorAndSize;
            result.CommodityStockId = request.CommodityStockId;
            result.Num = request.Num;
            result.AppId = appId;
            return result;
        }
        /// <summary>
        /// 关税
        /// </summary>
        [DataMember]
        public decimal Duty { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        [DataMember]
        public string DeliveryTime { get; set; }

        /// <summary>
        /// 七天无理由退货
        /// </summary>
        [DataMember]
        public bool IsAssurance { get; set; }

        /// <summary>
        /// 赠品信息
        /// </summary>
        [DataMember]
        public CommodiyPresentDTO Present { get; set; }
    }
}

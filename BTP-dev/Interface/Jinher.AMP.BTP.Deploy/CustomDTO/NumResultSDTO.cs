using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 统计
    /// </summary>
    [Serializable()]
    [DataContract]
   public class NumResultSDTO
    {
       /// <summary>
       /// 收藏商品数量
       /// </summary>
       [DataMemberAttribute()]
       public int CollectNum { get; set; }
       /// <summary>
       /// 购物车商品数
       /// </summary>
       [DataMemberAttribute()]
       public int  ShopCartNum { get; set; }
       /// <summary>
       /// 评价数
       /// </summary>
       [DataMemberAttribute()]
       public int TotalReview { get; set; }
        
    }
}

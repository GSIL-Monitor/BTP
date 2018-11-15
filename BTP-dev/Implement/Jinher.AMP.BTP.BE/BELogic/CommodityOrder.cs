

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
using Jinher.JAP.PL;
namespace Jinher.AMP.BTP.BE
{
    public partial class CommodityOrder
    {
        #region 基类抽象方法重载

        public override void BusinessRuleValidate()
        {
        }
        #endregion
        #region 基类虚方法重写
        public override void SetDefaultValue()
        {
            base.SetDefaultValue();
        }
        #endregion

        private static readonly Dictionary<int, int[]> StateCanChangeDict = new Dictionary<int, int[]>()
            {
                { 0, new int[] { 1, 4, 5, 6, 11} },
                { 1, new int[] { 2, 3, 8, 13 } },
                { 2, new int[] { 3, 9 } },
                { 3, new int[] {  } },
                { 4, new int[] { 1 } },
                { 5, new int[] { 1 } },
                { 6, new int[] { 1 } },
                { 7, new int[] {  } },
                { 8, new int[] { 1, 2, 7, 12 } },
                { 9, new int[] { 2, 7, 10, 12 } },
                { 10, new int[] { 7, 12 } },
                { 11, new int[] { 0, 1 } },
                { 12, new int[] { 7 } },
                { 13, new int[] {  2, 3 ,14 } },
                { 14, new int[] { 10,13 } }

            };
        private static readonly Dictionary<int, int[]> StateCanChangeHdfkDict = new Dictionary<int, int[]>()
            {
                { 0, new int[] { 1, 4, 5, 6 } },
                { 1, new int[] { 2, 3, 4, 5, 13 } },
                { 2, new int[] { 3, 9 } },
                { 3, new int[] {  } },
                { 4, new int[] {  } },
                { 5, new int[] {  } },
                { 6, new int[] {  } },
                { 7, new int[] {  } },
                { 8, new int[] {  } },
                { 9, new int[] {  } },
                { 10, new int[] {  } },
                { 11, new int[] {  } },
                { 12, new int[] {  } },
                { 13, new int[] {  2, 3 } },
                { 14, new int[] {  } }

            };
        /// <summary>
        /// 判断订单状态是否可以变为目标状态
        /// </summary>
        /// <param name="newState">订单目标状态</param>
        /// <returns></returns>
        public bool CanChangeState(int newState)
        {
            //TODO ghc 需要通过订单状态，支付方式，是否自提 三个标志判断
            Dictionary<int, int[]> stateDict = Payment == 1 ? StateCanChangeHdfkDict : StateCanChangeDict;
            if (stateDict != null && stateDict.ContainsKey(State))
            {
                return stateDict[State].Contains(newState);
            }
            return false;
        }

        /// <summary>
        /// 根据订单状态获取状态小类
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static List<int> GetOrderStateList(int? state)
        {
            switch (state)
            {
                case 0:
                    return new List<int>() { 0 };   //未支付=0
                case 1:
                    return new List<int>() { 1, 11 };   //未发货=1，付款中=11
                case 2:
                    return new List<int>() { 2, 13 };  //已发货=2，出库中=13
                case 3:
                    return new List<int>() { 3 };       //确认收货=3
                case 4:
                    return new List<int>() { 18 };       //舌尖待处理=18
                case 5:
                    return new List<int>() { 19 };       //舌尖已处理=18
                case 6:
                    return new List<int>() { 18, 19, 7 };       //舌尖：18待处理 19:已处理   7已退款
                case -1:
                    return new List<int>() { 4, 5, 6, 7, 8, 9, 10, 12, 14 };  //商家取消订单=4，客户取消订单=5，超时交易关闭=6，已退款=7，待发货退款中=8，已发货退款中=9,已发货退款中商家同意退款，商家未收到货=10,金和处理退款中=12,出库中退款中=14
                default:
                    return new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }; //所有状态
            }
        }
        /// <summary>
        /// 生成订单流水号
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static string GenerateOrderBatch(Guid appId)
        {
            var num = RedisHelper.HashIncr(getOrderBatchKey(), appId.ToString());
            return num < 10 ? string.Format("W{0:D2}", num) : string.Format("W{0}", num);
        }
        /// <summary>
        /// 取订单流水号redis的key
        /// </summary>
        /// <returns></returns>
        private static string getOrderBatchKey()
        {
            return RedisKeyConst.OrderBatch + ":" + DateTime.Today.ToString("yyyyMMdd");
        }
    }
}




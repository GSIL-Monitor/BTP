
/***************
功能描述: BTP-OPTSV
作    者: 
创建时间: 2015/7/17 18:14:18
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BAC.IBP.Facade;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class UserSpreaderSV : BaseSv, IUserSpreader
    {

        /// <summary>
        /// 保存买家微信和推广者（推广码）之间的关系。
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.UserSpreaderSV.svc/SaveSpreaderAndBuyerWxRel
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [Obsolete("已过期，目前不使用这种推广方式", false)]
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSpreaderAndBuyerWxRelExt(Jinher.AMP.BTP.Deploy.CustomDTO.SpreaderAndBuyerWxDTO sbwxDto)
        {
            ResultDTO result = new ResultDTO();

            if (sbwxDto == null)
            {
                result.ResultCode = 1;
                result.Message = "参数错误，参数不能为空!";
                return result;
            }
            if (sbwxDto.SpreadCode == Guid.Empty)
            {
                result.ResultCode = 2;
                result.Message = "参数错误，推广码不能为空!";
                return result;
            }

            if (string.IsNullOrWhiteSpace(sbwxDto.WxOpenId))
            {
                result.ResultCode = 2;
                result.Message = "参数错误，微信OpenId不能为空!";
                return result;
            }

            try
            {
                //一个买家只能属于一个推广主。
                var usWxs = from us in UserSpreader.ObjectSet()
                            where us.WxOpenId == sbwxDto.WxOpenId
                            select us;
                if (usWxs.Any())
                {
                    result.ResultCode = 0;
                    result.Message = "已存在当前买家和推广主信息!";
                    return result;
                }

                Guid spreaderId = Guid.Empty;
                //TODO 构造 bac不过。。。
                //try
                //{
                //    SpreadInfoFacade spreadFacade = new SpreadInfoFacade();
                //    spreaderId = spreadFacade.GetSpreadIdByid(sbwxDto.SpreadCode);
                //}
                //catch (Exception ex)
                //{
                //    LogHelper.Error(string.Format("UserSpreaderSVExt中调用Jinher.AMP.BAC.IBP.Facade.GetSpreadIdByid接口异常。id：{0}", sbwxDto.SpreadCode), ex);

                //    result.ResultCode = 3;
                //    result.Message = "接口异常，请稍后重试!";
                //    return result;
                //}



                UserSpreader uSpreaderNew = new UserSpreader();
                uSpreaderNew.Id = Guid.NewGuid();
                uSpreaderNew.UserId = new Guid("00000000-0000-0000-0000-000000000000");
                uSpreaderNew.SpreaderId = spreaderId;
                uSpreaderNew.SpreadCode = sbwxDto.SpreadCode;
                uSpreaderNew.IsDel = false;
                uSpreaderNew.CreateOrderId = new Guid("00000000-0000-0000-0000-000000000000");
                uSpreaderNew.SubTime = DateTime.Now;
                uSpreaderNew.ModifiedOn = DateTime.Now;
                uSpreaderNew.WxOpenId = sbwxDto.WxOpenId;
                uSpreaderNew.EntityState = System.Data.EntityState.Added;

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                contextSession.SaveObject(uSpreaderNew);
                contextSession.SaveChanges();

                result.ResultCode = 0;
                result.Message = "成功!";
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("UserSpreaderSVExt异常。sbwxDto：{0}", JsonHelper.JsonSerializer(sbwxDto)), ex);
                result.ResultCode = 4;
                result.Message = "接口异常，请稍后重试!";
                return result;
            }
        }
        /// <summary>
        /// 更新订单推广者信息。
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.UserSpreaderSV.svc/UpdateOrderSpreader
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [Obsolete("已过期，目前不使用这种推广方式", false)]
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderSpreaderExt(Jinher.AMP.BTP.Deploy.CustomDTO.SpreaderAndBuyerWxDTO sbwxDto)
        {
            ResultDTO result = new ResultDTO();

            if (sbwxDto == null)
            {
                result.ResultCode = 1;
                result.Message = "参数错误，参数不能为空!";
                return result;
            }
            if (string.IsNullOrWhiteSpace(sbwxDto.WxOpenId))
            {
                result.ResultCode = 2;
                result.Message = "参数错误，微信OpenId不能为空!";
                return result;
            }

            if (sbwxDto.BuyerId == Guid.Empty)
            {
                result.ResultCode = 3;
                result.Message = "参数错误，买家用户Id不能为空!";
                return result;
            }

            if (sbwxDto.OrderId == Guid.Empty)
            {
                result.ResultCode = 4;
                result.Message = "参数错误，订单Id不能为空!";
                return result;
            }

            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                //用户id相关 买家-推广者对照表。
                var usQuery = from us in UserSpreader.ObjectSet()
                              where us.UserId == sbwxDto.BuyerId
                              select us;

                //微信Id相关 买家-推广者对照表。
                var wxQuery = from u in UserSpreader.ObjectSet()
                              where u.WxOpenId == sbwxDto.WxOpenId
                              select u;
                //推广者用户id.
                Guid spreaderId = Guid.Empty;
                Guid spreadCode = Guid.Empty;
                //当前买家已有归属的推广主信息。
                if (usQuery.Any())
                {
                    //没有wxopenId信息，则更新。
                    var usFirst = usQuery.FirstOrDefault();
                    spreaderId = usFirst.SpreaderId;
                    spreadCode = usFirst.SpreadCode;
                    if (string.IsNullOrWhiteSpace(usFirst.WxOpenId))
                    {
                        usFirst.WxOpenId = sbwxDto.WxOpenId;
                        usFirst.ModifiedOn = DateTime.Now;
                    }
                    //删除其它和当前wxOpenId相关的记录。
                    foreach (var uu in usQuery)
                    {
                        if (uu.Id == usFirst.Id)
                        {
                            continue;
                        }
                        uu.EntityState = System.Data.EntityState.Deleted;
                    }
                }
                else
                {
                    if (wxQuery.Any())
                    {
                        UserSpreader usRecord = wxQuery.FirstOrDefault();
                        usRecord.UserId = sbwxDto.BuyerId;
                        usRecord.ModifiedOn = DateTime.Now;
                        spreaderId = usRecord.SpreaderId;
                        spreadCode = usRecord.SpreadCode;

                    }
                }

                //推广分成相关订单Id.
                List<Guid> orderIds = new List<Guid>();

                var moQuery = from mo in MainOrder.ObjectSet()
                              where mo.MainOrderId == sbwxDto.OrderId
                              select mo.SubOrderId;

                if (moQuery.Any())
                {
                    //传入的OrderId为主订单id，找出所有子订单。
                    orderIds = moQuery.ToList();
                }
                else
                {
                    orderIds.Add(sbwxDto.OrderId);
                }

                var coQuery = from co in CommodityOrder.ObjectSet()
                              where orderIds.Contains(co.Id)
                              select co;
                foreach (var cOrder in coQuery)
                {
                    cOrder.SpreaderId = spreaderId;
                    cOrder.ModifiedOn = DateTime.Now;
                    cOrder.SpreadCode = spreadCode;
                }

                contextSession.SaveChanges();

                result.ResultCode = 0;
                result.Message = "成功!";
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("UpdateOrderSpreaderExt异常。sbwxDto：{0}", JsonHelper.JsonSerializer(sbwxDto)), ex);

                result.ResultCode = 10;
                result.Message = "接口异常，请稍后重试!";
                return result;
            }
        }

        /// <summary>
        /// 更新用户为推广主
        /// </summary>
        /// <param name="spreaderDto">推广者dto</param>
        /// <returns></returns>
        public ResultDTO UpdateToSpreaderExt(SpreaderAndBuyerWxDTO spreaderDto)
        {
            ResultDTO result = new ResultDTO { Message = "Success" };
            if (spreaderDto == null || spreaderDto.SpreaderId == Guid.Empty || spreaderDto.SpreadCode == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "id为空或推广码为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //插入SpreadInfo数据库表  
                var oldSpreadInfo = SpreadInfo.ObjectSet().FirstOrDefault(c => c.SpreadId == spreaderDto.SpreaderId && c.SpreadCode == spreaderDto.SpreadCode && c.IsDel == 0);
                if (oldSpreadInfo == null)
                {
                    SpreadInfo newSpreadInfo = SpreadInfo.CreateSpreadInfo();
                    newSpreadInfo.SpreadId = spreaderDto.SpreaderId;
                    newSpreadInfo.SpreadCode = spreaderDto.SpreadCode;
                    newSpreadInfo.IsDel = 0;
                    newSpreadInfo.SpreadType = 0;
                    contextSession.SaveObject(newSpreadInfo);
                    contextSession.SaveChanges();
                }
                var createUser = EBCSV.GetOrgCreateUser(spreaderDto.SpreaderId);
                var oldUserSpreader = UserSpreader.ObjectSet().FirstOrDefault(c => c.UserId == createUser);
                if (oldUserSpreader != null)
                {
                    var createUsers = EBCSV.GetMyCreateAccountList(createUser);
                    if (createUsers != null && createUsers.Contains(oldUserSpreader.SpreaderId))
                        return result;
                    oldUserSpreader.EntityState = EntityState.Deleted;
                }
                UserSpreader newUserSpreader = UserSpreader.CreateUserSpreader();
                newUserSpreader.SpreaderId = spreaderDto.SpreaderId;
                newUserSpreader.UserId = createUser;
                newUserSpreader.SpreadCode = spreaderDto.SpreadCode;
                contextSession.SaveObject(newUserSpreader);
                contextSession.SaveChanges();

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("UserSpreaderSV.UpdateToSpreaderExt异常：spreaderDto：{0}", JsonHelper.JsonSerializer(spreaderDto)), ex);
                return result;
            }
            return result;
        }

        /// <summary>
        /// 绑定关系
        /// </summary>
        /// <param name="userSpreaderBindDTO">参数只传SpreadCode、UserID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUserSpreaderCodeExt(Jinher.AMP.BTP.Deploy.CustomDTO.UserSpreaderBindDTO userSpreaderBindDTO)
        {
            ResultDTO result = new ResultDTO();

            if (userSpreaderBindDTO == null)
            {
                result.ResultCode = 1;
                result.Message = "参数错误，参数不能为空!";
                return result;
            }
            if (userSpreaderBindDTO.SpreadCode == Guid.Empty)
            {
                result.ResultCode = 2;
                result.Message = "参数错误，推广码不能为空!";
                return result;
            }

            if (userSpreaderBindDTO.UserId == Guid.Empty)
            {
                result.ResultCode = 2;
                result.Message = "参数错误，买家ID不能为空!";
                return result;
            }

            try
            {
                //一个买家只能属于一个推广主。
                var usCount = (from us in UserSpreader.ObjectSet()
                               where us.UserId == userSpreaderBindDTO.UserId
                               select us).Count();
                if (usCount > 0)
                {
                    result.ResultCode = 0;
                    result.Message = "已存在当前买家和推广主信息!";
                    return result;
                }

                var spreadInfo = SpreadInfo.ObjectSet().FirstOrDefault(t => t.SpreadCode == userSpreaderBindDTO.SpreadCode && t.IsDel == 0);
                if (spreadInfo == null)
                {
                    result.ResultCode = 3;
                    result.Message = "没有此推广码的信息!";
                    return result;
                }

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                UserSpreader uSpreaderNew = new UserSpreader();
                uSpreaderNew.Id = Guid.NewGuid();
                uSpreaderNew.UserId = userSpreaderBindDTO.UserId;
                uSpreaderNew.SpreaderId = spreadInfo.SpreadId;
                uSpreaderNew.SpreadCode = userSpreaderBindDTO.SpreadCode;
                uSpreaderNew.IsDel = false;
                uSpreaderNew.CreateOrderId = new Guid("00000000-0000-0000-0000-000000000000");
                uSpreaderNew.SubTime = DateTime.Now;
                uSpreaderNew.ModifiedOn = DateTime.Now;
                uSpreaderNew.WxOpenId = "";

                uSpreaderNew.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(uSpreaderNew);
                contextSession.SaveChanges();

                result.ResultCode = 0;
                result.Message = "成功!";
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("UserSpreaderSVExt异常。userSpreaderBindDTO：{0}", JsonHelper.JsonSerializer(userSpreaderBindDTO)), ex);
                result.ResultCode = 4;
                result.Message = "接口异常，请稍后重试!";
                return result;
            }
        }

    }
}

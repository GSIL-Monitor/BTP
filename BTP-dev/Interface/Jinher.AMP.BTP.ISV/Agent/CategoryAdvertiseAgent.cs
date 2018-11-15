
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/6/15 17:13:41
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class CategoryAdvertiseAgent : BaseBpAgent<ICategoryAdvertise>, ICategoryAdvertise
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO> getBrandWallSpecialByCateID(System.Guid CategoryID)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.getBrandWallSpecialByCateID(CategoryID);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
    }
}

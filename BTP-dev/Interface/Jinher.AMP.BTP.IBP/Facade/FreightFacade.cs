
/***************
功能描述: BTP-OPTFacade
作    者: 
创建时间: 2015/7/30 17:59:53
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class FreightFacade : BaseFacade<IFreight>
    {

        /// <summary>
        /// 获取运费模板列表
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页查询几条数据</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO> GetFreightTemplateListByAppId(System.Guid appId, int pageIndex, int pageSize, out int rowCount)
        {
            base.Do();
            return this.Command.GetFreightTemplateListByAppId(appId, pageIndex, pageSize, out rowCount);
        }
        /// <summary>
        /// 获取运费模板详细数据
        /// </summary>
        /// <param name="freightTemplateId">运费模板编号</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.FreightTemplateDetailDTO> GetFreightTemplateDetailListByTemId(System.Guid freightTemplateId)
        {
            base.Do();
            return this.Command.GetFreightTemplateDetailListByTemId(freightTemplateId);
        }
        /// <summary>
        /// 增加一条运费模板详细信息
        /// </summary>
        /// <param name="freightDetail"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddFreightDetail(Jinher.AMP.BTP.Deploy.FreightTemplateDetailDTO freightDetail)
        {
            base.Do();
            return this.Command.AddFreightDetail(freightDetail);
        }
        /// <summary>
        /// 删除一条运费模板明细信息
        /// </summary>
        /// <param name="freightDetailId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteFreightDetail(System.Guid freightDetailId)
        {
            base.Do();
            return this.Command.DeleteFreightDetail(freightDetailId);
        }
        /// <summary>
        /// 删除一条运费模板信息
        /// </summary>
        /// <param name="Id">运费模板编号</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteFreight(System.Guid Id)
        {
            base.Do();
            return this.Command.DeleteFreight(Id);
        }
        /// <summary>
        /// 保存运费模板及其明细
        /// </summary>
        /// <param name="freight"></param>
        /// <param name="freightDetailList"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddFreightAndFreightDetail(Jinher.AMP.BTP.Deploy.FreightTemplateDTO freight, System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.FreightTemplateDetailDTO> freightDetailList)
        {
            base.Do();
            return this.Command.AddFreightAndFreightDetail(freight, freightDetailList);
        }
        /// <summary>
        /// 更新运费模板和运费详细信息列表
        /// </summary>
        /// <param name="freight"></param>
        /// <param name="freightDetailList"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateFreightAndFreightDetail(Jinher.AMP.BTP.Deploy.FreightTemplateDTO freight, System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.FreightTemplateDetailDTO> freightDetailList)
        {
            base.Do();
            return this.Command.UpdateFreightAndFreightDetail(freight, freightDetailList);
        }
        /// <summary>
        /// 获取一条运费记录
        /// </summary>
        /// <param name="freightTemplateId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO GetOneFreight(System.Guid freightTemplateId)
        {
            base.Do();
            return this.Command.GetOneFreight(freightTemplateId);
        }
        /// <summary>
        /// 获取运费模板列表
        ///  </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO> GetFreightListByAppId(System.Guid appId)
        {
            base.Do();
            return this.Command.GetFreightListByAppId(appId);
        }
        /// <summary>
        /// 运费模板是否关联了商品
        /// </summary>
        /// <param name="freightTemplateId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO IsContactCommodity(System.Guid freightTemplateId)
        {
            base.Do();
            return this.Command.IsContactCommodity(freightTemplateId);
        }
        /// <summary>
        /// 保存运费模板及其明细
        /// </summary>
        /// <param name="freight"></param>
        /// <param name="freightDetailList"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveFreightTemplateFull(Jinher.AMP.BTP.Deploy.CustomDTO.FreightTempFullDTO freightDTO)
        {
            base.Do();
            return this.Command.SaveFreightTemplateFull(freightDTO);
        }

        /// <summary>
        /// 保存运费模板及其明细
        /// </summary>
        /// <param name="freight"></param>
        /// <param name="freightDetailList"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveRangeFreightTemplate(Jinher.AMP.BTP.Deploy.CustomDTO.RangeFreightTemplateInputDTO inputDTO)
        {
            base.Do();
            return this.Command.SaveRangeFreightTemplate(inputDTO);
        }

        /// <summary>
        /// 建立运费模板与选定商品的关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public ResultDTO JoinCommodity(FreightTemplateAssociationCommodityInputDTO inputDTO)
        {
            base.Do();
            return this.Command.JoinCommodity(inputDTO);
        }

        /// <summary>
        /// 解除运费模板与选定商品的关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public ResultDTO UnjoinCommodity(FreightTemplateAssociationCommodityInputDTO inputDTO)
        {
            base.Do();
            return this.Command.UnjoinCommodity(inputDTO);
        }
    }
}

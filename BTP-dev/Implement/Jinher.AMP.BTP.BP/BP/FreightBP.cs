
/***************
功能描述: BTP-OPTBP
作    者: 
创建时间: 2015/7/30 17:59:54
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class FreightBP : BaseBP, IFreight
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
            return this.GetFreightTemplateListByAppIdExt(appId, pageIndex, pageSize, out rowCount);
        }
        /// <summary>
        /// 获取运费模板详细数据
        /// </summary>
        /// <param name="freightTemplateId">运费模板编号</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.FreightTemplateDetailDTO> GetFreightTemplateDetailListByTemId(System.Guid freightTemplateId)
        {
            base.Do();
            return this.GetFreightTemplateDetailListByTemIdExt(freightTemplateId);
        }
        /// <summary>
        /// 增加一条运费模板详细信息
        /// </summary>
        /// <param name="freightDetail"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddFreightDetail(Jinher.AMP.BTP.Deploy.FreightTemplateDetailDTO freightDetail)
        {
            base.Do();
            return this.AddFreightDetailExt(freightDetail);
        }
        /// <summary>
        /// 删除一条运费模板明细信息
        /// </summary>
        /// <param name="freightDetailId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteFreightDetail(System.Guid freightDetailId)
        {
            base.Do();
            return this.DeleteFreightDetailExt(freightDetailId);
        }
        /// <summary>
        /// 删除一条运费模板信息
        /// </summary>
        /// <param name="Id">运费模板编号</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteFreight(System.Guid Id)
        {
            base.Do();
            return this.DeleteFreightExt(Id);
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
            return this.AddFreightAndFreightDetailExt(freight, freightDetailList);
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
            return this.UpdateFreightAndFreightDetailExt(freight, freightDetailList);
        }
        /// <summary>
        /// 获取一条运费记录
        /// </summary>
        /// <param name="freightTemplateId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO GetOneFreight(System.Guid freightTemplateId)
        {
            base.Do();
            return this.GetOneFreightExt(freightTemplateId);
        }
        /// <summary>
        /// 获取运费模板列表
        ///  </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO> GetFreightListByAppId(System.Guid appId)
        {
            base.Do();
            return this.GetFreightListByAppIdExt(appId);
        }
        /// <summary>
        /// 运费模板是否关联了商品
        /// </summary>
        /// <param name="freightTemplateId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO IsContactCommodity(System.Guid freightTemplateId)
        {
            base.Do();
            return this.IsContactCommodityExt(freightTemplateId);
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
            return this.SaveFreightTemplateFullExt(freightDTO);
        }

        /// <summary>
        /// 保存区间运费模板及其明细
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO SaveRangeFreightTemplate(Deploy.CustomDTO.RangeFreightTemplateInputDTO inputDTO)
        {
            base.Do();
            return this.SaveRangeFreightTemplateExt(inputDTO);
        }

        /// <summary>
        /// 建立运费模板与选定商品的关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO JoinCommodity(FreightTemplateAssociationCommodityInputDTO inputDTO)
        {
            base.Do();
            return JoinCommodityExt(inputDTO);
        }

        /// <summary>
        /// 解除运费模板与选定商品的关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO UnjoinCommodity(FreightTemplateAssociationCommodityInputDTO inputDTO)
        {
            base.Do();
            return UnjoinCommodityExt(inputDTO);
        }
    }
}

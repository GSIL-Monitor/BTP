
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/5/31 18:12:30
***************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class SpreadCategoryBP : BaseBP, ISpreadCategory
    {

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Add(Jinher.AMP.BTP.Deploy.SpreadCategoryDTO dto)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.AddExt(dto);
            timer.Stop();
            LogHelper.Debug(string.Format("SpreadCategoryBP.Save：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(dto), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        ///  删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Delete(System.Guid id)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.DeleteExt(id);
            timer.Stop();
            LogHelper.Debug(string.Format("SpreadCategoryBP.Delete：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, id, JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 获取推广主类型
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SpreadCategoryDTO>> GetSpreadCategoryList(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadSearchDTO search)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetSpreadCategoryListExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("SpreadCategoryBP.GetSpreadCategoryList：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }
    }
}
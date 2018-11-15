using System;
using System.Data;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.BE
{
    public partial class HaiXinMqJournal
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        public void Create(HaiXinMqJournalDTO dto)
        {
            try
            {
                var entity = HaiXinMqJournal.FromDTO(dto);
                entity.EntityState = EntityState.Added;
                ContextFactory.CurrentThreadContext.SaveObject(entity);
                ContextFactory.CurrentThreadContext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("BE.HaiXinMqJournal.Create异常", ex);
            }
        }
    }
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExpandFunc
    {
        /// <summary>
        /// 更新方法
        /// </summary>
        /// <param name="mqJournal">参数</param>
        public static void Update(this HaiXinMqJournal mqJournal)
        {
            try
            {
                mqJournal.EntityState = EntityState.Modified;
                ContextFactory.CurrentThreadContext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("BE.HaiXinMqJournal.Create异常", ex);
            }
        }
    }
}




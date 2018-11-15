using System;
using System.Data;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.BE
{
	public  partial class YKBigDataMqJournal
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
        public void Create(YKBigDataMqJournalDTO dto)
        {
            try
            {
                var entity = YKBigDataMqJournal.FromDTO(dto);
                entity.EntityState = EntityState.Added;
                ContextFactory.CurrentThreadContext.SaveObject(entity);
                ContextFactory.CurrentThreadContext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("BE.YKBigDataMqJournal.Create异常", ex);
            }
        }
	}
}




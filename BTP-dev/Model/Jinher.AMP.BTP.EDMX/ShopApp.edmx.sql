
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 11/09/2018 18:51:00
-- Generated from EDMX file: D:\Project\01-AMP\开发库\08-Program\Code\BizNew\BTP\Model\Jinher.AMP.BTP.EDMX\ShopApp.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [AMP.BTP];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_FreightTemplateFreightTemplateDetail]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FreightTemplateDetail] DROP CONSTRAINT [FK_FreightTemplateFreightTemplateDetail];
GO
IF OBJECT_ID(N'[dbo].[FK_CateringSettingCateringBusinessHours]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CateringBusinessHours] DROP CONSTRAINT [FK_CateringSettingCateringBusinessHours];
GO
IF OBJECT_ID(N'[dbo].[FK_CateringSettingCateringShiftTime]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CateringShiftTime] DROP CONSTRAINT [FK_CateringSettingCateringShiftTime];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Category]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Category];
GO
IF OBJECT_ID(N'[dbo].[Commodity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Commodity];
GO
IF OBJECT_ID(N'[dbo].[SecondAttribute]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SecondAttribute];
GO
IF OBJECT_ID(N'[dbo].[ProductDetailsPicture]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductDetailsPicture];
GO
IF OBJECT_ID(N'[dbo].[Attribute]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Attribute];
GO
IF OBJECT_ID(N'[dbo].[Promotion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Promotion];
GO
IF OBJECT_ID(N'[dbo].[PromotionItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PromotionItems];
GO
IF OBJECT_ID(N'[dbo].[CommodityOrder]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityOrder];
GO
IF OBJECT_ID(N'[dbo].[OrderItem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderItem];
GO
IF OBJECT_ID(N'[dbo].[Message]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Message];
GO
IF OBJECT_ID(N'[dbo].[ShoppingCartItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ShoppingCartItems];
GO
IF OBJECT_ID(N'[dbo].[Review]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Review];
GO
IF OBJECT_ID(N'[dbo].[Store]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Store];
GO
IF OBJECT_ID(N'[dbo].[DeliveryAddress]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeliveryAddress];
GO
IF OBJECT_ID(N'[dbo].[Collection]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Collection];
GO
IF OBJECT_ID(N'[dbo].[ComAttibute]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ComAttibute];
GO
IF OBJECT_ID(N'[dbo].[CommodityUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityUser];
GO
IF OBJECT_ID(N'[dbo].[CommodityCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityCategory];
GO
IF OBJECT_ID(N'[dbo].[Reply]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Reply];
GO
IF OBJECT_ID(N'[dbo].[Journal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Journal];
GO
IF OBJECT_ID(N'[dbo].[Payments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Payments];
GO
IF OBJECT_ID(N'[dbo].[AllPayment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AllPayment];
GO
IF OBJECT_ID(N'[dbo].[HotCommodity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HotCommodity];
GO
IF OBJECT_ID(N'[dbo].[TodayPromotion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TodayPromotion];
GO
IF OBJECT_ID(N'[dbo].[GenUserPrizeRecord]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GenUserPrizeRecord];
GO
IF OBJECT_ID(N'[dbo].[OrderRefund]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderRefund];
GO
IF OBJECT_ID(N'[dbo].[CommodityStock]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityStock];
GO
IF OBJECT_ID(N'[dbo].[FreightTemplate]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FreightTemplate];
GO
IF OBJECT_ID(N'[dbo].[FreightTemplateDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FreightTemplateDetail];
GO
IF OBJECT_ID(N'[dbo].[UserLimited]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserLimited];
GO
IF OBJECT_ID(N'[dbo].[OrderShareMess]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderShareMess];
GO
IF OBJECT_ID(N'[dbo].[Invoice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Invoice];
GO
IF OBJECT_ID(N'[dbo].[RelationCommodity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RelationCommodity];
GO
IF OBJECT_ID(N'[dbo].[ShareDividend]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ShareDividend];
GO
IF OBJECT_ID(N'[dbo].[ShareDividendDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ShareDividendDetail];
GO
IF OBJECT_ID(N'[dbo].[UserRedEnvelope]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserRedEnvelope];
GO
IF OBJECT_ID(N'[dbo].[APPManage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[APPManage];
GO
IF OBJECT_ID(N'[dbo].[RuleDescription]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RuleDescription];
GO
IF OBJECT_ID(N'[dbo].[UserCrowdfunding]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserCrowdfunding];
GO
IF OBJECT_ID(N'[dbo].[CfDividend]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CfDividend];
GO
IF OBJECT_ID(N'[dbo].[CfOrderDividend]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CfOrderDividend];
GO
IF OBJECT_ID(N'[dbo].[CfOrderDividendDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CfOrderDividendDetail];
GO
IF OBJECT_ID(N'[dbo].[UserCrowdfundingDaily]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserCrowdfundingDaily];
GO
IF OBJECT_ID(N'[dbo].[CrowdfundingDaily]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CrowdfundingDaily];
GO
IF OBJECT_ID(N'[dbo].[CrowdfundingCount]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CrowdfundingCount];
GO
IF OBJECT_ID(N'[dbo].[Crowdfunding]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Crowdfunding];
GO
IF OBJECT_ID(N'[dbo].[CrowdfundingStatistics]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CrowdfundingStatistics];
GO
IF OBJECT_ID(N'[dbo].[AppSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppSet];
GO
IF OBJECT_ID(N'[dbo].[SetCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SetCategory];
GO
IF OBJECT_ID(N'[dbo].[SetCommodityCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SetCommodityCategory];
GO
IF OBJECT_ID(N'[dbo].[MainOrder]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MainOrder];
GO
IF OBJECT_ID(N'[dbo].[BehaviorRecord]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BehaviorRecord];
GO
IF OBJECT_ID(N'[dbo].[OrderShipping]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderShipping];
GO
IF OBJECT_ID(N'[dbo].[SetCollection]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SetCollection];
GO
IF OBJECT_ID(N'[dbo].[OrderExpirePay]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderExpirePay];
GO
IF OBJECT_ID(N'[dbo].[UserSpreader]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSpreader];
GO
IF OBJECT_ID(N'[dbo].[OrderPayDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderPayDetail];
GO
IF OBJECT_ID(N'[dbo].[SettlingAccount]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SettlingAccount];
GO
IF OBJECT_ID(N'[dbo].[CommodityOrderException]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityOrderException];
GO
IF OBJECT_ID(N'[dbo].[FreightPartialFree]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FreightPartialFree];
GO
IF OBJECT_ID(N'[dbo].[SpreadCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SpreadCategory];
GO
IF OBJECT_ID(N'[dbo].[SpreadInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SpreadInfo];
GO
IF OBJECT_ID(N'[dbo].[SelfTakeStation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SelfTakeStation];
GO
IF OBJECT_ID(N'[dbo].[SelfTakeStationManager]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SelfTakeStationManager];
GO
IF OBJECT_ID(N'[dbo].[OrderPickUp]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderPickUp];
GO
IF OBJECT_ID(N'[dbo].[CommodityJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityJournal];
GO
IF OBJECT_ID(N'[dbo].[CommodityOrderService]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityOrderService];
GO
IF OBJECT_ID(N'[dbo].[OrderRefundAfterSales]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderRefundAfterSales];
GO
IF OBJECT_ID(N'[dbo].[AppExtension]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppExtension];
GO
IF OBJECT_ID(N'[dbo].[ErrorCommodityOrder]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ErrorCommodityOrder];
GO
IF OBJECT_ID(N'[dbo].[OrderExpressRoute]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderExpressRoute];
GO
IF OBJECT_ID(N'[dbo].[ExpressTrace]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExpressTrace];
GO
IF OBJECT_ID(N'[dbo].[ExpressCode]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExpressCode];
GO
IF OBJECT_ID(N'[dbo].[OrderPayee]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderPayee];
GO
IF OBJECT_ID(N'[dbo].[ScoreSetting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ScoreSetting];
GO
IF OBJECT_ID(N'[dbo].[Distributor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Distributor];
GO
IF OBJECT_ID(N'[dbo].[CommodityDistribution]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityDistribution];
GO
IF OBJECT_ID(N'[dbo].[CommodityDistributionJounal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityDistributionJounal];
GO
IF OBJECT_ID(N'[dbo].[OrderItemShare]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderItemShare];
GO
IF OBJECT_ID(N'[dbo].[OrderShare]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderShare];
GO
IF OBJECT_ID(N'[dbo].[DiyGroup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DiyGroup];
GO
IF OBJECT_ID(N'[dbo].[DiyGroupOrder]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DiyGroupOrder];
GO
IF OBJECT_ID(N'[dbo].[PaySource]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PaySource];
GO
IF OBJECT_ID(N'[dbo].[VatInvoiceProof]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VatInvoiceProof];
GO
IF OBJECT_ID(N'[dbo].[InvoiceJounal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InvoiceJounal];
GO
IF OBJECT_ID(N'[dbo].[AppSelfTakeStation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppSelfTakeStation];
GO
IF OBJECT_ID(N'[dbo].[AppStsManager]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppStsManager];
GO
IF OBJECT_ID(N'[dbo].[AppStsOfficeTime]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppStsOfficeTime];
GO
IF OBJECT_ID(N'[dbo].[AppOrderPickUp]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppOrderPickUp];
GO
IF OBJECT_ID(N'[dbo].[CateringSetting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CateringSetting];
GO
IF OBJECT_ID(N'[dbo].[WeChatQRCode]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WeChatQRCode];
GO
IF OBJECT_ID(N'[dbo].[CateringComdtyXData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CateringComdtyXData];
GO
IF OBJECT_ID(N'[dbo].[CateringBusinessHours]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CateringBusinessHours];
GO
IF OBJECT_ID(N'[dbo].[CateringShiftTime]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CateringShiftTime];
GO
IF OBJECT_ID(N'[dbo].[ShiftTimeLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ShiftTimeLog];
GO
IF OBJECT_ID(N'[dbo].[OrderRefundInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderRefundInfo];
GO
IF OBJECT_ID(N'[dbo].[ExpressOrderTemplate]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExpressOrderTemplate];
GO
IF OBJECT_ID(N'[dbo].[ExpressOrderTemplateProperty]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExpressOrderTemplateProperty];
GO
IF OBJECT_ID(N'[dbo].[OrderPrintLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderPrintLog];
GO
IF OBJECT_ID(N'[dbo].[OrderPrintDetailLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderPrintDetailLog];
GO
IF OBJECT_ID(N'[dbo].[DistributionRule]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DistributionRule];
GO
IF OBJECT_ID(N'[dbo].[DistributionIdentitySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DistributionIdentitySet];
GO
IF OBJECT_ID(N'[dbo].[DistributionIdentity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DistributionIdentity];
GO
IF OBJECT_ID(N'[dbo].[DistributionApply]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DistributionApply];
GO
IF OBJECT_ID(N'[dbo].[Microshop]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Microshop];
GO
IF OBJECT_ID(N'[dbo].[MicroshopCom]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MicroshopCom];
GO
IF OBJECT_ID(N'[dbo].[DistributionApplyAudit]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DistributionApplyAudit];
GO
IF OBJECT_ID(N'[dbo].[WeChatQrCodeType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WeChatQrCodeType];
GO
IF OBJECT_ID(N'[dbo].[MallApply]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MallApply];
GO
IF OBJECT_ID(N'[dbo].[BaseCommission]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BaseCommission];
GO
IF OBJECT_ID(N'[dbo].[CategoryCommission]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CategoryCommission];
GO
IF OBJECT_ID(N'[dbo].[CommodityCommission]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityCommission];
GO
IF OBJECT_ID(N'[dbo].[SettleAccounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SettleAccounts];
GO
IF OBJECT_ID(N'[dbo].[SettleAccountsDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SettleAccountsDetails];
GO
IF OBJECT_ID(N'[dbo].[SettleAccountsOrderItem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SettleAccountsOrderItem];
GO
IF OBJECT_ID(N'[dbo].[SettleAccountsPeriod]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SettleAccountsPeriod];
GO
IF OBJECT_ID(N'[dbo].[CommoditySettleAmount]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommoditySettleAmount];
GO
IF OBJECT_ID(N'[dbo].[HTJSInvoice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HTJSInvoice];
GO
IF OBJECT_ID(N'[dbo].[YJBJOrderItem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YJBJOrderItem];
GO
IF OBJECT_ID(N'[dbo].[CommodityTaxRate]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityTaxRate];
GO
IF OBJECT_ID(N'[dbo].[OrderField]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderField];
GO
IF OBJECT_ID(N'[dbo].[InnerCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InnerCategory];
GO
IF OBJECT_ID(N'[dbo].[Supplier]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Supplier];
GO
IF OBJECT_ID(N'[dbo].[CommodityCodeSeq]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityCodeSeq];
GO
IF OBJECT_ID(N'[dbo].[CommodityInnerCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityInnerCategory];
GO
IF OBJECT_ID(N'[dbo].[ExpressCollection]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExpressCollection];
GO
IF OBJECT_ID(N'[dbo].[ExpressTemplateCollection]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExpressTemplateCollection];
GO
IF OBJECT_ID(N'[dbo].[SupplierMain]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SupplierMain];
GO
IF OBJECT_ID(N'[dbo].[JdOrderItem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JdOrderItem];
GO
IF OBJECT_ID(N'[dbo].[JdJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JdJournal];
GO
IF OBJECT_ID(N'[dbo].[ServiceSettings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ServiceSettings];
GO
IF OBJECT_ID(N'[dbo].[YJBJCard]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YJBJCard];
GO
IF OBJECT_ID(N'[dbo].[Jdlogs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Jdlogs];
GO
IF OBJECT_ID(N'[dbo].[PresentPromotion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PresentPromotion];
GO
IF OBJECT_ID(N'[dbo].[PresentPromotionCommodity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PresentPromotionCommodity];
GO
IF OBJECT_ID(N'[dbo].[PresentPromotionGift]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PresentPromotionGift];
GO
IF OBJECT_ID(N'[dbo].[OrderItemPresent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderItemPresent];
GO
IF OBJECT_ID(N'[dbo].[SettleAccountsException]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SettleAccountsException];
GO
IF OBJECT_ID(N'[dbo].[OrderStatistics]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderStatistics];
GO
IF OBJECT_ID(N'[dbo].[CommodityChange]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityChange];
GO
IF OBJECT_ID(N'[dbo].[JdOrderRefundAfterSales]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JdOrderRefundAfterSales];
GO
IF OBJECT_ID(N'[dbo].[LongisticsTrack]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LongisticsTrack];
GO
IF OBJECT_ID(N'[dbo].[JdExpressTrace]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JdExpressTrace];
GO
IF OBJECT_ID(N'[dbo].[AuditCommodity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AuditCommodity];
GO
IF OBJECT_ID(N'[dbo].[AuditCommodityStock]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AuditCommodityStock];
GO
IF OBJECT_ID(N'[dbo].[AuditManage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AuditManage];
GO
IF OBJECT_ID(N'[dbo].[AuditMode]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AuditMode];
GO
IF OBJECT_ID(N'[dbo].[JDAuditMode]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JDAuditMode];
GO
IF OBJECT_ID(N'[dbo].[JdAuditCommodity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JdAuditCommodity];
GO
IF OBJECT_ID(N'[dbo].[JdAuditCommodityStock]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JdAuditCommodityStock];
GO
IF OBJECT_ID(N'[dbo].[YJEmployee]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YJEmployee];
GO
IF OBJECT_ID(N'[dbo].[Specifications]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Specifications];
GO
IF OBJECT_ID(N'[dbo].[CommoditySpecifications]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommoditySpecifications];
GO
IF OBJECT_ID(N'[dbo].[JdCommodity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JdCommodity];
GO
IF OBJECT_ID(N'[dbo].[JDStockJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JDStockJournal];
GO
IF OBJECT_ID(N'[dbo].[JDEclpOrder]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JDEclpOrder];
GO
IF OBJECT_ID(N'[dbo].[JDEclpOrderJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JDEclpOrderJournal];
GO
IF OBJECT_ID(N'[dbo].[JDEclpOrderRefundAfterSales]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JDEclpOrderRefundAfterSales];
GO
IF OBJECT_ID(N'[dbo].[JDEclpOrderRefundAfterSalesItem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JDEclpOrderRefundAfterSalesItem];
GO
IF OBJECT_ID(N'[dbo].[JDEclpOrderRefundAfterSalesJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JDEclpOrderRefundAfterSalesJournal];
GO
IF OBJECT_ID(N'[dbo].[UserCodeForHaiXin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserCodeForHaiXin];
GO
IF OBJECT_ID(N'[dbo].[HaiXinMqJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HaiXinMqJournal];
GO
IF OBJECT_ID(N'[dbo].[RefundExpressTrace]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RefundExpressTrace];
GO
IF OBJECT_ID(N'[dbo].[FreightRangeDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FreightRangeDetails];
GO
IF OBJECT_ID(N'[dbo].[YKBigDataMqJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YKBigDataMqJournal];
GO
IF OBJECT_ID(N'[dbo].[YXOrder]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YXOrder];
GO
IF OBJECT_ID(N'[dbo].[YXOrderJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YXOrderJournal];
GO
IF OBJECT_ID(N'[dbo].[YXOrderPackage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YXOrderPackage];
GO
IF OBJECT_ID(N'[dbo].[YXOrderSku]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YXOrderSku];
GO
IF OBJECT_ID(N'[dbo].[YXExpressDetailInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YXExpressDetailInfo];
GO
IF OBJECT_ID(N'[dbo].[YXExpressDetailInfoSku]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YXExpressDetailInfoSku];
GO
IF OBJECT_ID(N'[dbo].[YXComInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YXComInfo];
GO
IF OBJECT_ID(N'[dbo].[YXOrderPackageJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YXOrderPackageJournal];
GO
IF OBJECT_ID(N'[dbo].[YXOrderErrorLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YXOrderErrorLog];
GO
IF OBJECT_ID(N'[dbo].[YXExpressTrace]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YXExpressTrace];
GO
IF OBJECT_ID(N'[dbo].[AppMetaData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AppMetaData];
GO
IF OBJECT_ID(N'[dbo].[CommodityPriceFloat]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityPriceFloat];
GO
IF OBJECT_ID(N'[dbo].[CategoryAdvertise]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CategoryAdvertise];
GO
IF OBJECT_ID(N'[dbo].[CommodityInnerBrand]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityInnerBrand];
GO
IF OBJECT_ID(N'[dbo].[Brandwall]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Brandwall];
GO
IF OBJECT_ID(N'[dbo].[CategoryInnerBrand]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CategoryInnerBrand];
GO
IF OBJECT_ID(N'[dbo].[CommodityOrderRefund]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityOrderRefund];
GO
IF OBJECT_ID(N'[dbo].[SNOrderItem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SNOrderItem];
GO
IF OBJECT_ID(N'[dbo].[SNExpressTrace]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SNExpressTrace];
GO
IF OBJECT_ID(N'[dbo].[SNPackageTrace]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SNPackageTrace];
GO
IF OBJECT_ID(N'[dbo].[SNOrderRefundAfterSales]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SNOrderRefundAfterSales];
GO
IF OBJECT_ID(N'[dbo].[ThirdECommerce]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdECommerce];
GO
IF OBJECT_ID(N'[dbo].[ThirdECOrder]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdECOrder];
GO
IF OBJECT_ID(N'[dbo].[ThirdECOrderJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdECOrderJournal];
GO
IF OBJECT_ID(N'[dbo].[ThirdECOrderPackage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdECOrderPackage];
GO
IF OBJECT_ID(N'[dbo].[ThirdECOrderPackageSku]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdECOrderPackageSku];
GO
IF OBJECT_ID(N'[dbo].[ThirdECOrderErrorLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdECOrderErrorLog];
GO
IF OBJECT_ID(N'[dbo].[ThirdECExpressTrace]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdECExpressTrace];
GO
IF OBJECT_ID(N'[dbo].[ThirdECStockJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdECStockJournal];
GO
IF OBJECT_ID(N'[dbo].[ThirdECService]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdECService];
GO
IF OBJECT_ID(N'[dbo].[ThirdECServiceJournal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdECServiceJournal];
GO
IF OBJECT_ID(N'[dbo].[YJEmTemp]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YJEmTemp];
GO
IF OBJECT_ID(N'[dbo].[YJBDSFOrderInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YJBDSFOrderInfo];
GO
IF OBJECT_ID(N'[dbo].[YJBCarInsuranceRebate]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YJBCarInsuranceRebate];
GO
IF OBJECT_ID(N'[dbo].[YJBCarInsuranceReport]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YJBCarInsuranceReport];
GO
IF OBJECT_ID(N'[dbo].[YJBCarInsReportDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[YJBCarInsReportDetail];
GO
IF OBJECT_ID(N'[dbo].[SNOrder]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SNOrder];
GO
IF OBJECT_ID(N'[dbo].[CouponRefundDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CouponRefundDetail];
GO
IF OBJECT_ID(N'[dbo].[FangZhengOrder]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FangZhengOrder];
GO
IF OBJECT_ID(N'[dbo].[FangZhengLogistics]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FangZhengLogistics];
GO
IF OBJECT_ID(N'[dbo].[ThirdMapStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdMapStatus];
GO
IF OBJECT_ID(N'[dbo].[ThirdMerchant]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdMerchant];
GO
IF OBJECT_ID(N'[dbo].[SNBill]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SNBill];
GO
IF OBJECT_ID(N'[dbo].[InsuranceCompanyActivity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InsuranceCompanyActivity];
GO
IF OBJECT_ID(N'[dbo].[InsuranceCompany]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InsuranceCompany];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Category'
CREATE TABLE [dbo].[Category] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [ParentId] uniqueidentifier  NULL,
    [CurrentLevel] int  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Sort] int  NOT NULL,
    [IsDel] bit  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [icon] nvarchar(4000)  NULL,
    [IsUse] bit  NULL,
    [ParentCategoryPath] nvarchar(4000)  NULL
);
GO

-- Creating table 'Commodity'
CREATE TABLE [dbo].[Commodity] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [No_Number] int  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [Stock] int  NOT NULL,
    [PicturesPath] nvarchar(4000)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [State] int  NOT NULL,
    [IsDel] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [No_Code] nvarchar(512)  NOT NULL,
    [TotalCollection] int  NOT NULL,
    [TotalReview] int  NOT NULL,
    [Salesvolume] int  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [GroundTime] datetime  NULL,
    [ComAttribute] nvarchar(max)  NULL,
    [CategoryName] nvarchar(max)  NULL,
    [SortValue] int  NOT NULL,
    [FreightTemplateId] uniqueidentifier  NULL,
    [MarketPrice] decimal(18,2)  NULL,
    [IsEnableSelfTake] int  NOT NULL,
    [Weight] decimal(18,5)  NULL,
    [PricingMethod] tinyint  NOT NULL,
    [SaleAreas] nvarchar(max)  NULL,
    [SharePercent] decimal(18,5)  NULL,
    [CommodityType] int  NOT NULL,
    [HtmlVideoPath] nvarchar(3000)  NULL,
    [MobileVideoPath] nvarchar(3000)  NULL,
    [VideoPic] nvarchar(3000)  NULL,
    [VideoName] nvarchar(512)  NULL,
    [ScorePercent] decimal(18,5)  NULL,
    [Duty] decimal(18,2)  NULL,
    [SpreadPercent] decimal(18,5)  NULL,
    [ScoreScale] decimal(18,5)  NULL,
    [TaxRate] decimal(18,2)  NULL,
    [TaxClassCode] nvarchar(64)  NULL,
    [Unit] nvarchar(max)  NULL,
    [InputRax] decimal(18,2)  NULL,
    [Barcode] nvarchar(128)  NULL,
    [JDCode] nvarchar(128)  NULL,
    [CostPrice] decimal(18,2)  NULL,
    [IsAssurance] bit  NULL,
    [TechSpecs] nvarchar(max)  NULL,
    [SaleService] nvarchar(max)  NULL,
    [IsReturns] bit  NULL,
    [ServiceSettingId] nvarchar(400)  NULL,
    [Type] int  NULL,
    [YJCouponActivityId] nvarchar(32)  NULL,
    [YJCouponType] nvarchar(10)  NULL,
    [ModifieId] uniqueidentifier  NULL,
    [Isnsupport] bit  NULL,
    [ErQiCode] nvarchar(50)  NULL,
    [RefundFreightTemplateId] uniqueidentifier  NULL,
    [BasketCount] int  NULL,
    [OrderWeight] decimal(18,2)  NULL,
    [Assurance] bit  NULL,
    [YoukaPercent] decimal(18,0)  NULL,
    [SpuId] nvarchar(128)  NULL
);
GO

-- Creating table 'SecondAttribute'
CREATE TABLE [dbo].[SecondAttribute] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL,
    [AttributeId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'ProductDetailsPicture'
CREATE TABLE [dbo].[ProductDetailsPicture] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [PicturesPath] nvarchar(4000)  NOT NULL,
    [Sort] int  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'Attribute'
CREATE TABLE [dbo].[Attribute] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'Promotion'
CREATE TABLE [dbo].[Promotion] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [PicturesPath] nvarchar(4000)  NOT NULL,
    [StartTime] datetime  NOT NULL,
    [EndTime] datetime  NOT NULL,
    [Intensity] decimal(18,2)  NOT NULL,
    [IsEnable] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [IsAll] bit  NULL,
    [CommodityNames] nvarchar(max)  NULL,
    [DiscountPrice] decimal(18,2)  NULL,
    [PromotionType] int  NOT NULL,
    [ChannelId] uniqueidentifier  NULL,
    [OutsideId] uniqueidentifier  NULL,
    [IsDel] bit  NOT NULL,
    [PresellStartTime] datetime  NULL,
    [PresellEndTime] datetime  NULL,
    [ModifiedOn] datetime  NOT NULL,
    [GroupMinVolume] int  NULL,
    [ExpireSecond] int  NULL,
    [Description] nvarchar(400)  NULL,
    [IsSell] bit  NULL,
    [IsStatis] bit  NOT NULL,
    [LimitBuyTotal] int  NULL
);
GO

-- Creating table 'PromotionItems'
CREATE TABLE [dbo].[PromotionItems] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [PromotionId] uniqueidentifier  NOT NULL,
    [CommodityName] nvarchar(max)  NULL,
    [LimitBuyEach] int  NULL,
    [LimitBuyTotal] int  NULL,
    [DiscountPrice] decimal(18,2)  NULL,
    [Intensity] decimal(18,2)  NULL,
    [SurplusLimitBuyTotal] int  NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'CommodityOrder'
CREATE TABLE [dbo].[CommodityOrder] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [State] int  NOT NULL,
    [PaymentTime] datetime  NULL,
    [RealPrice] decimal(18,2)  NULL,
    [ConfirmTime] datetime  NULL,
    [ShipmentsTime] datetime  NULL,
    [ReceiptUserName] nvarchar(512)  NOT NULL,
    [ReceiptPhone] nvarchar(512)  NOT NULL,
    [ReceiptAddress] nvarchar(4000)  NOT NULL,
    [Details] nvarchar(4000)  NULL,
    [Payment] int  NOT NULL,
    [MessageToBuyer] nvarchar(4000)  NULL,
    [Province] nvarchar(512)  NOT NULL,
    [City] nvarchar(512)  NOT NULL,
    [District] nvarchar(512)  NOT NULL,
    [IsModifiedPrice] bit  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [PaymentState] bit  NOT NULL,
    [RecipientsZipCode] nvarchar(6)  NULL,
    [SrcType] int  NULL,
    [SrcTagId] uniqueidentifier  NULL,
    [CPSId] nvarchar(max)  NULL,
    [CancelReason] nvarchar(128)  NULL,
    [ShipExpCo] nvarchar(50)  NULL,
    [ExpOrderNo] nvarchar(128)  NULL,
    [IsDelayConfirmTime] bit  NOT NULL,
    [Freight] decimal(18,2)  NOT NULL,
    [IsRefund] bit  NULL,
    [IsDel] int  NOT NULL,
    [RefundTime] datetime  NULL,
    [AgreementTime] datetime  NULL,
    [SellersRemark] nvarchar(512)  NULL,
    [Commission] decimal(18,2)  NOT NULL,
    [IsCrowdfunding] int  NOT NULL,
    [CrowdfundingPrice] decimal(18,2)  NOT NULL,
    [GoldPrice] decimal(18,2)  NOT NULL,
    [GoldCoupon] decimal(18,2)  NOT NULL,
    [SelfTakeFlag] int  NOT NULL,
    [SpreaderId] uniqueidentifier  NULL,
    [SpreadGold] bigint  NOT NULL,
    [SpreadCode] uniqueidentifier  NULL,
    [OwnerShare] decimal(18,2)  NOT NULL,
    [SrcAppId] uniqueidentifier  NULL,
    [ScoreState] int  NOT NULL,
    [EsAppId] uniqueidentifier  NULL,
    [OrderType] int  NOT NULL,
    [ServiceId] uniqueidentifier  NULL,
    [DistributorId] uniqueidentifier  NULL,
    [DistributeMoney] decimal(18,2)  NOT NULL,
    [PicturesPath] nvarchar(4000)  NULL,
    [SaiCode] nvarchar(100)  NULL,
    [ChannelShareMoney] decimal(18,2)  NOT NULL,
    [Batch] nvarchar(50)  NULL,
    [MealBoxFee] decimal(18,2)  NULL,
    [Duty] decimal(18,2)  NOT NULL,
    [ExpressPrintCount] int  NULL,
    [InvoicePrintCount] int  NULL,
    [FirstContent] nvarchar(50)  NOT NULL,
    [SecondContent] nvarchar(50)  NOT NULL,
    [ThirdContent] nvarchar(50)  NOT NULL,
    [Street] nvarchar(50)  NULL,
    [CancelReasonCode] smallint  NULL,
    [SetMealId] uniqueidentifier  NULL,
    [AppName] nvarchar(512)  NULL,
    [SupplierName] nvarchar(512)  NULL,
    [SupplierCode] nvarchar(128)  NULL,
    [SupplierType] smallint  NULL,
    [ShipperType] smallint  NULL,
    [AppType] smallint  NULL,
    [JcActivityId] uniqueidentifier  NULL,
    [HasStatisYJInfo] bit  NULL,
    [TechSpecs] nvarchar(max)  NULL,
    [SaleService] nvarchar(max)  NULL,
    [DeliveryTime] datetime  NULL,
    [DeliveryDays] int  NULL,
    [CustomerInfo] uniqueidentifier  NULL,
    [YJCouponPrice] decimal(18,2)  NULL,
    [YJCardPrice] decimal(18,2)  NULL
);
GO

-- Creating table 'OrderItem'
CREATE TABLE [dbo].[OrderItem] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Number] int  NOT NULL,
    [CurrentPrice] decimal(18,2)  NOT NULL,
    [PicturesPath] nvarchar(4000)  NOT NULL,
    [CommodityOrderId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [PromotionId] uniqueidentifier  NULL,
    [ComAttributeIds] nvarchar(max)  NULL,
    [CategoryNames] nvarchar(max)  NULL,
    [CommodityAttributes] nvarchar(500)  NULL,
    [RealPrice] decimal(18,2)  NULL,
    [Intensity] decimal(18,2)  NULL,
    [AlreadyReview] bit  NOT NULL,
    [DiscountPrice] decimal(18,2)  NULL,
    [CommodityStockId] uniqueidentifier  NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ShareMoney] decimal(18,2)  NOT NULL,
    [PromotionDesc] nvarchar(200)  NULL,
    [PromotionType] int  NULL,
    [VipLevelId] uniqueidentifier  NULL,
    [ComCategoryId] uniqueidentifier  NULL,
    [ComCategoryName] nvarchar(512)  NULL,
    [ScorePrice] decimal(18,2)  NOT NULL,
    [Duty] decimal(18,2)  NOT NULL,
    [TaxRate] decimal(18,2)  NULL,
    [InputRax] decimal(18,2)  NULL,
    [CostPrice] decimal(18,2)  NULL,
    [Barcode] nvarchar(128)  NULL,
    [No_Code] nvarchar(128)  NULL,
    [InnerCatetoryIds] nvarchar(512)  NULL,
    [Unit] nvarchar(256)  NULL,
    [YouKaPercent] decimal(18,2)  NULL,
    [DeliveryTime] datetime  NULL,
    [DeliveryDays] int  NULL,
    [Type] int  NULL,
    [YJCouponActivityId] nvarchar(32)  NULL,
    [YJCouponType] nvarchar(10)  NULL,
    [HasPresent] bit  NULL,
    [CouponPrice] decimal(18,2)  NULL,
    [FreightPrice] decimal(18,2)  NULL,
    [YjbPrice] decimal(18,2)  NULL,
    [ChangeFreightPrice] decimal(18,2)  NULL,
    [ChangeRealPrice] decimal(18,2)  NULL,
    [State] int  NULL,
    [RefundExpCo] nvarchar(max)  NULL,
    [RefundExpOrderNo] nvarchar(max)  NULL,
    [Specifications] int  NULL,
    [ErQiCode] nvarchar(50)  NULL,
    [YJCouponPrice] decimal(18,2)  NULL,
    [AppId] uniqueidentifier  NULL,
    [JDCode] nvarchar(128)  NULL,
    [State_Value] nvarchar(max)  NULL,
    [SNCode] nvarchar(200)  NULL,
    [YJCardPrice] decimal(18,2)  NULL
);
GO

-- Creating table 'Message'
CREATE TABLE [dbo].[Message] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Status] int  NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'ShoppingCartItems'
CREATE TABLE [dbo].[ShoppingCartItems] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [CommodityNumber] int  NOT NULL,
    [ComAttributeIds] nvarchar(max)  NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [EsAppId] uniqueidentifier  NULL,
    [CommodityStockId] uniqueidentifier  NOT NULL,
    [JcActivityId] uniqueidentifier  NULL,
    [Specifications] int  NULL
);
GO

-- Creating table 'Review'
CREATE TABLE [dbo].[Review] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [UserName] nvarchar(512)  NOT NULL,
    [UserHeader] nvarchar(4000)  NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CommodityName] nvarchar(500)  NULL,
    [CommodityAttributes] nvarchar(500)  NULL,
    [CommodityPicture] nvarchar(500)  NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'Store'
CREATE TABLE [dbo].[Store] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Address] nvarchar(4000)  NOT NULL,
    [Phone] nvarchar(512)  NOT NULL,
    [picture] nvarchar(4000)  NOT NULL,
    [Province] nvarchar(512)  NOT NULL,
    [City] nvarchar(512)  NOT NULL,
    [District] nvarchar(512)  NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [ProvinceCode] nvarchar(max)  NULL,
    [CityCode] nvarchar(max)  NULL,
    [DistrictCode] nvarchar(max)  NULL,
    [ModifiedOn] datetime  NOT NULL,
    [XAxis] decimal(10,6)  NOT NULL,
    [YAxis] decimal(10,6)  NOT NULL
);
GO

-- Creating table 'DeliveryAddress'
CREATE TABLE [dbo].[DeliveryAddress] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [RecipientsUserName] nvarchar(512)  NOT NULL,
    [RecipientsPhone] nvarchar(512)  NOT NULL,
    [RecipientsAddress] nvarchar(4000)  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [IsDefault] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Province] nvarchar(512)  NOT NULL,
    [City] nvarchar(512)  NOT NULL,
    [District] nvarchar(512)  NOT NULL,
    [RecipientsZipCode] nvarchar(6)  NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ProvinceCode] nvarchar(12)  NOT NULL,
    [CityCode] nvarchar(12)  NOT NULL,
    [DistrictCode] nvarchar(12)  NOT NULL,
    [StreetCode] nvarchar(12)  NULL,
    [Street] nvarchar(12)  NULL
);
GO

-- Creating table 'Collection'
CREATE TABLE [dbo].[Collection] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'ComAttibute'
CREATE TABLE [dbo].[ComAttibute] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [AttributeName] nvarchar(512)  NOT NULL,
    [SecondAttributeName] nvarchar(512)  NOT NULL,
    [AttributeId] uniqueidentifier  NOT NULL,
    [SecondAttributeId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'CommodityUser'
CREATE TABLE [dbo].[CommodityUser] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [HeadPic] nvarchar(4000)  NOT NULL,
    [UserName] nvarchar(512)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [Sex] int  NOT NULL,
    [Details] nvarchar(max)  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'CommodityCategory'
CREATE TABLE [dbo].[CommodityCategory] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [CategoryId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [MaxSort] float  NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CrcAppId] bigint  NOT NULL,
    [IsDel] bit  NULL,
    [CategoryPath] nvarchar(4000)  NULL
);
GO

-- Creating table 'Reply'
CREATE TABLE [dbo].[Reply] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [ReplyDetails] nvarchar(max)  NULL,
    [ReplyerId] uniqueidentifier  NOT NULL,
    [PreUserId] uniqueidentifier  NOT NULL,
    [UserName] nvarchar(512)  NULL,
    [UserHeader] nvarchar(4000)  NULL,
    [ReviewId] uniqueidentifier  NOT NULL,
    [ParentId] uniqueidentifier  NULL,
    [Type] int  NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'Journal'
CREATE TABLE [dbo].[Journal] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Details] nvarchar(max)  NOT NULL,
    [CommodityOrderId] uniqueidentifier  NOT NULL,
    [StateFrom] int  NULL,
    [StateTo] int  NOT NULL,
    [IsPush] bit  NOT NULL,
    [OrderType] int  NOT NULL,
    [CommodityOrderItemId] uniqueidentifier  NULL
);
GO

-- Creating table 'Payments'
CREATE TABLE [dbo].[Payments] (
    [Id] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NULL,
    [Name] nvarchar(512)  NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [IsOnuse] bit  NOT NULL,
    [PaymentName] nvarchar(max)  NOT NULL,
    [PaymentId] uniqueidentifier  NOT NULL,
    [AliPayPartnerId] nvarchar(1000)  NULL,
    [AliPaySeller] nvarchar(1000)  NULL,
    [AliPayPrivateKey] nvarchar(2000)  NULL,
    [AliPayPublicKey] nvarchar(2000)  NULL,
    [AliPayVerifyCode] nvarchar(max)  NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'AllPayment'
CREATE TABLE [dbo].[AllPayment] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [PaymentName] nvarchar(max)  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'HotCommodity'
CREATE TABLE [dbo].[HotCommodity] (
    [Id] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [PicturesPath] nvarchar(4000)  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [TotalReview] int  NOT NULL,
    [TotalCollection] int  NOT NULL,
    [State] int  NOT NULL,
    [Stock] int  NOT NULL,
    [Salesvolume] int  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'TodayPromotion'
CREATE TABLE [dbo].[TodayPromotion] (
    [Id] uniqueidentifier  NOT NULL,
    [PromotionId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [Intensity] decimal(18,2)  NOT NULL,
    [StartTime] datetime  NOT NULL,
    [EndTime] datetime  NOT NULL,
    [DiscountPrice] decimal(18,2)  NULL,
    [LimitBuyEach] int  NULL,
    [LimitBuyTotal] int  NULL,
    [SurplusLimitBuyTotal] int  NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [PromotionType] int  NOT NULL,
    [ChannelId] uniqueidentifier  NULL,
    [OutsideId] uniqueidentifier  NULL,
    [PresellStartTime] datetime  NULL,
    [PresellEndTime] datetime  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [GroupMinVolume] int  NULL,
    [ExpireSecond] int  NULL,
    [Description] nvarchar(400)  NULL,
    [ExpireSeconds] bigint  NOT NULL,
    [PromtionLimitBuyTotal] int  NULL
);
GO

-- Creating table 'GenUserPrizeRecord'
CREATE TABLE [dbo].[GenUserPrizeRecord] (
    [Id] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [ValTime] datetime  NOT NULL,
    [PromotionId] uniqueidentifier  NOT NULL,
    [IsBuyed] bit  NOT NULL,
    [OrderId] uniqueidentifier  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'OrderRefund'
CREATE TABLE [dbo].[OrderRefund] (
    [Id] uniqueidentifier  NOT NULL,
    [RefundType] int  NOT NULL,
    [RefundReason] nvarchar(128)  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL,
    [RefundDesc] nvarchar(225)  NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [State] int  NOT NULL,
    [ReceiverAccount] nvarchar(128)  NULL,
    [Receiver] nvarchar(max)  NULL,
    [RefundExpCo] nvarchar(50)  NULL,
    [RefundExpOrderNo] nvarchar(max)  NULL,
    [OrderRefundImgs] nvarchar(max)  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [DataType] nvarchar(max)  NULL,
    [OrderItemId] uniqueidentifier  NULL,
    [RefuseTime] datetime  NULL,
    [IsFullRefund] bit  NULL,
    [RefuseReason] nvarchar(max)  NULL,
    [NotReceiveTime] datetime  NULL,
    [RefundExpOrderTime] datetime  NULL,
    [IsDelayConfirmTimeAfterSales] bit  NULL,
    [AgreeFlag] int  NULL,
    [RefundScoreMoney] decimal(18,2)  NOT NULL,
    [SalerRemark] nvarchar(max)  NULL,
    [RefundYJBMoney] decimal(18,2)  NOT NULL,
    [RefundFreightPrice] decimal(18,2)  NULL,
    [RefundChangeFreightPrice] decimal(18,2)  NULL,
    [RefundChangeRealPrice] decimal(18,2)  NULL,
    [RefundDuty] decimal(18,2)  NULL,
    [RejectFreightMoney] decimal(18,2)  NULL,
    [ApplyId] nvarchar(128)  NULL,
    [RefundCouponPirce] decimal(18,2)  NULL,
    [OrderRefundMoneyAndCoupun] decimal(18,2)  NULL,
    [RefundYJCouponMoney] decimal(18,2)  NULL,
    [RefundYJCardMoney] decimal(18,2)  NULL
);
GO

-- Creating table 'CommodityStock'
CREATE TABLE [dbo].[CommodityStock] (
    [Id] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [ComAttribute] nvarchar(max)  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [Stock] int  NOT NULL,
    [MarketPrice] decimal(18,0)  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [Duty] decimal(18,2)  NULL,
    [Barcode] nvarchar(128)  NULL,
    [No_Code] nvarchar(128)  NULL,
    [JDCode] nvarchar(128)  NULL,
    [CostPrice] decimal(18,2)  NULL,
    [ThumImg] nvarchar(1000)  NULL,
    [CarouselImgs] nvarchar(4000)  NULL,
    [State] int  NULL,
    [ErQiCode] nvarchar(50)  NULL,
    [ComAttrType] int  NULL,
    [IsDel] bit  NULL,
    [IsSell] bit  NULL
);
GO

-- Creating table 'FreightTemplate'
CREATE TABLE [dbo].[FreightTemplate] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [FreightMethod] int  NOT NULL,
    [FreightTo] nvarchar(max)  NOT NULL,
    [FirstCount] decimal(18,5)  NOT NULL,
    [FirstCountPrice] decimal(18,2)  NOT NULL,
    [NextCount] decimal(18,5)  NOT NULL,
    [NextCountPrice] decimal(18,2)  NOT NULL,
    [IsFreeExp] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [PricingMethod] tinyint  NOT NULL,
    [ExpressType] tinyint  NOT NULL,
    [IsDefault] int  NULL
);
GO

-- Creating table 'FreightTemplateDetail'
CREATE TABLE [dbo].[FreightTemplateDetail] (
    [Id] uniqueidentifier  NOT NULL,
    [FreightTo] nvarchar(2000)  NOT NULL,
    [FirstCount] decimal(18,5)  NOT NULL,
    [FirstCountPrice] decimal(18,2)  NOT NULL,
    [NextCount] decimal(18,5)  NOT NULL,
    [NextCountPrice] decimal(18,2)  NOT NULL,
    [FreightTemplateId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [DestinationCodes] nvarchar(2000)  NOT NULL
);
GO

-- Creating table 'UserLimited'
CREATE TABLE [dbo].[UserLimited] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [PromotionId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [Count] int  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [CommodityOrderId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'OrderShareMess'
CREATE TABLE [dbo].[OrderShareMess] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [ShareId] nvarchar(max)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'Invoice'
CREATE TABLE [dbo].[Invoice] (
    [Id] uniqueidentifier  NOT NULL,
    [CommodityOrderId] uniqueidentifier  NOT NULL,
    [InvoiceTitle] nvarchar(max)  NULL,
    [InvoiceContent] nvarchar(max)  NULL,
    [InvoiceType] int  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ReceiptPhone] nvarchar(50)  NULL,
    [ReceiptEmail] nvarchar(200)  NULL,
    [State] int  NOT NULL,
    [Remark] nvarchar(max)  NULL,
    [Category] int  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Code] nvarchar(200)  NULL
);
GO

-- Creating table 'RelationCommodity'
CREATE TABLE [dbo].[RelationCommodity] (
    [Id] uniqueidentifier  NOT NULL,
    [CommodityName] nvarchar(max)  NOT NULL,
    [CommodityPicturesPath] nvarchar(max)  NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [RelationCommodityId] uniqueidentifier  NOT NULL,
    [No_Code] nvarchar(max)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'ShareDividend'
CREATE TABLE [dbo].[ShareDividend] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [Money] decimal(18,2)  NOT NULL,
    [SettlementDate] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [State] int  NOT NULL,
    [CommodityOrderId] uniqueidentifier  NOT NULL,
    [SharerMoney] bigint  NOT NULL,
    [ShareType] int  NOT NULL
);
GO

-- Creating table 'ShareDividendDetail'
CREATE TABLE [dbo].[ShareDividendDetail] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [Money] bigint  NOT NULL,
    [SettlementDate] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [RoleType] int  NOT NULL,
    [ShareDivedendId] uniqueidentifier  NOT NULL,
    [State] int  NOT NULL,
    [Description] nvarchar(200)  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'UserRedEnvelope'
CREATE TABLE [dbo].[UserRedEnvelope] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [State] int  NOT NULL,
    [DueDate] datetime  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [GoldCount] bigint  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [RedEnvelopeType] int  NOT NULL,
    [RoleType] int  NOT NULL
);
GO

-- Creating table 'APPManage'
CREATE TABLE [dbo].[APPManage] (
    [Id] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(64)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Remark] nvarchar(512)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ModifiedId] uniqueidentifier  NULL
);
GO

-- Creating table 'RuleDescription'
CREATE TABLE [dbo].[RuleDescription] (
    [Id] uniqueidentifier  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NULL
);
GO

-- Creating table 'UserCrowdfunding'
CREATE TABLE [dbo].[UserCrowdfunding] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [UserName] nvarchar(200)  NULL,
    [UserCode] nvarchar(100)  NULL,
    [CrowdfundingId] uniqueidentifier  NOT NULL,
    [Money] decimal(18,2)  NOT NULL,
    [OrderCount] int  NOT NULL,
    [CurrentShareCount] bigint  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [TotalDividend] bigint  NOT NULL,
    [RealGetDividend] bigint  NOT NULL,
    [OrdersMoney] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'CfDividend'
CREATE TABLE [dbo].[CfDividend] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(max)  NOT NULL,
    [Gold] bigint  NOT NULL,
    [ShareCount] bigint  NOT NULL,
    [State] int  NOT NULL,
    [SettlementDate] datetime  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'CfOrderDividend'
CREATE TABLE [dbo].[CfOrderDividend] (
    [Id] uniqueidentifier  NOT NULL,
    [Gold] bigint  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [State] int  NOT NULL,
    [CommodityOrderId] uniqueidentifier  NOT NULL,
    [CurrentShareCount] bigint  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'CfOrderDividendDetail'
CREATE TABLE [dbo].[CfOrderDividendDetail] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [OrderDividendId] uniqueidentifier  NOT NULL,
    [SettlementDate] datetime  NOT NULL,
    [Gold] bigint  NOT NULL,
    [ShareCount] bigint  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'UserCrowdfundingDaily'
CREATE TABLE [dbo].[UserCrowdfundingDaily] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(200)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [ShareCount] bigint  NOT NULL,
    [Money] decimal(18,2)  NOT NULL,
    [DailyMoney] decimal(18,2)  NOT NULL,
    [SettlementDate] datetime  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'CrowdfundingDaily'
CREATE TABLE [dbo].[CrowdfundingDaily] (
    [Id] uniqueidentifier  NOT NULL,
    [CrowdfundingId] uniqueidentifier  NOT NULL,
    [DividendPercent] decimal(18,10)  NOT NULL,
    [CurrentShareCount] bigint  NOT NULL,
    [SettlementDate] datetime  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'CrowdfundingCount'
CREATE TABLE [dbo].[CrowdfundingCount] (
    [Id] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CrowdfundingId] uniqueidentifier  NOT NULL,
    [CurrentShareCount] bigint  NOT NULL,
    [ShareCount] bigint  NOT NULL,
    [TotalDividend] bigint  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'Crowdfunding'
CREATE TABLE [dbo].[Crowdfunding] (
    [Id] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(200)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [PerShareMoney] decimal(18,2)  NOT NULL,
    [DividendPercent] decimal(18,10)  NOT NULL,
    [ShareCount] bigint  NOT NULL,
    [StartTime] datetime  NOT NULL,
    [State] int  NOT NULL,
    [Slogan] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'CrowdfundingStatistics'
CREATE TABLE [dbo].[CrowdfundingStatistics] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [CrowdfundingCount] bigint  NOT NULL,
    [Total] bigint  NOT NULL,
    [UnReceive] bigint  NOT NULL,
    [Week] bigint  NOT NULL,
    [UnReceiveWeek] bigint  NOT NULL,
    [LastOneDay] bigint  NOT NULL,
    [LastTwoDay] bigint  NOT NULL,
    [LastThreeDay] bigint  NOT NULL,
    [LastFourDay] bigint  NOT NULL,
    [LastFiveDay] bigint  NOT NULL,
    [LastSixDay] bigint  NOT NULL,
    [LastSevenDay] bigint  NOT NULL,
    [SettlementDate] datetime  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'AppSet'
CREATE TABLE [dbo].[AppSet] (
    [Id] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NULL,
    [AppSetType] int  NOT NULL,
    [AppName] nvarchar(max)  NOT NULL,
    [AppIcon] nvarchar(max)  NOT NULL,
    [AppAccount] nvarchar(max)  NOT NULL,
    [AppCreateOn] datetime  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [AppSetId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'SetCategory'
CREATE TABLE [dbo].[SetCategory] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [ParentId] uniqueidentifier  NOT NULL,
    [CurrentLevel] int  NOT NULL,
    [IsDel] bit  NOT NULL,
    [Sort] int  NOT NULL,
    [PicturesPath] nvarchar(4000)  NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'SetCommodityCategory'
CREATE TABLE [dbo].[SetCommodityCategory] (
    [Id] uniqueidentifier  NOT NULL,
    [SetCategoryId] uniqueidentifier  NOT NULL,
    [SetCategoryName] nvarchar(128)  NOT NULL,
    [SetCategorySort] float  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'MainOrder'
CREATE TABLE [dbo].[MainOrder] (
    [Id] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NULL,
    [MainOrderId] uniqueidentifier  NOT NULL,
    [SubOrderId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'BehaviorRecord'
CREATE TABLE [dbo].[BehaviorRecord] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [Method] nvarchar(200)  NOT NULL,
    [Params] nvarchar(300)  NOT NULL,
    [BehaviorType] int  NOT NULL,
    [BehaviorKey] nvarchar(500)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'OrderShipping'
CREATE TABLE [dbo].[OrderShipping] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [ShipExpCo] nvarchar(50)  NOT NULL,
    [ExpOrderNo] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'SetCollection'
CREATE TABLE [dbo].[SetCollection] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [ChannelId] uniqueidentifier  NOT NULL,
    [ColKey] uniqueidentifier  NOT NULL,
    [ColType] int  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'OrderExpirePay'
CREATE TABLE [dbo].[OrderExpirePay] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [ExpirePayTime] datetime  NOT NULL,
    [State] int  NOT NULL,
    [PromotionId] uniqueidentifier  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'UserSpreader'
CREATE TABLE [dbo].[UserSpreader] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [SpreaderId] uniqueidentifier  NOT NULL,
    [SpreadCode] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL,
    [CreateOrderId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [WxOpenId] nvarchar(100)  NULL
);
GO

-- Creating table 'OrderPayDetail'
CREATE TABLE [dbo].[OrderPayDetail] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [ObjectType] tinyint  NOT NULL,
    [Amount] decimal(18,2)  NOT NULL,
    [CommodityId] uniqueidentifier  NULL,
    [ObjectId] uniqueidentifier  NOT NULL,
    [CommodityIds] nvarchar(max)  NULL,
    [ObjectIds] nvarchar(max)  NULL,
    [UseType] int  NOT NULL,
    [CouponType] int  NOT NULL,
    [ScoreCost] int  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'SettlingAccount'
CREATE TABLE [dbo].[SettlingAccount] (
    [Id] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [ManufacturerClearingPrice] decimal(18,2)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Effectable] int  NOT NULL,
    [EffectiveTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubName] nvarchar(max)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [UserCode] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'CommodityOrderException'
CREATE TABLE [dbo].[CommodityOrderException] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(max)  NOT NULL,
    [OrderRealPrice] decimal(18,2)  NULL,
    [ClearingPrice] decimal(18,2)  NULL,
    [ExceptionType] int  NOT NULL,
    [ExceptionReason] nvarchar(max)  NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(max)  NULL,
    [State] int  NOT NULL,
    [Note] nvarchar(max)  NULL,
    [ExceptionTime] datetime  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'FreightPartialFree'
CREATE TABLE [dbo].[FreightPartialFree] (
    [Id] uniqueidentifier  NOT NULL,
    [DestinationCodes] nvarchar(2000)  NOT NULL,
    [FreeType] tinyint  NOT NULL,
    [FreePrice] decimal(18,2)  NOT NULL,
    [FreeCount] decimal(18,5)  NOT NULL,
    [FreightTemplateId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'SpreadCategory'
CREATE TABLE [dbo].[SpreadCategory] (
    [Id] uniqueidentifier  NOT NULL,
    [SpreadType] int  NOT NULL,
    [SpreaderPercent] decimal(18,5)  NOT NULL,
    [Priority] int  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [CategoryDesc] nvarchar(200)  NOT NULL
);
GO

-- Creating table 'SpreadInfo'
CREATE TABLE [dbo].[SpreadInfo] (
    [Id] uniqueidentifier  NOT NULL,
    [SpreadId] uniqueidentifier  NOT NULL,
    [SpreadUrl] nvarchar(300)  NOT NULL,
    [SpreadCode] uniqueidentifier  NOT NULL,
    [SpreadDesc] nvarchar(200)  NULL,
    [SpreadType] int  NOT NULL,
    [IsDel] int  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [QrCodeUrl] nvarchar(2000)  NOT NULL,
    [SpreadAppId] uniqueidentifier  NOT NULL,
    [HotshopId] uniqueidentifier  NOT NULL,
    [UserCode] nvarchar(200)  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [IWId] uniqueidentifier  NOT NULL,
    [SubSpreadCount] int  NOT NULL,
    [DividendPercent] decimal(18,5)  NOT NULL,
    [IWCode] nvarchar(200)  NOT NULL
);
GO

-- Creating table 'SelfTakeStation'
CREATE TABLE [dbo].[SelfTakeStation] (
    [Id] uniqueidentifier  NOT NULL,
    [CityOwnerId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Province] nvarchar(20)  NOT NULL,
    [City] nvarchar(20)  NOT NULL,
    [District] nvarchar(20)  NOT NULL,
    [Address] nvarchar(200)  NOT NULL,
    [SpreadUrl] nvarchar(500)  NOT NULL,
    [Remark] nvarchar(max)  NOT NULL,
    [QRCodeUrl] nvarchar(1000)  NOT NULL,
    [SpreadCode] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [IsDel] bit  NOT NULL,
    [SelfTakeStationType] int  NOT NULL,
    [AppId] uniqueidentifier  NULL
);
GO

-- Creating table 'SelfTakeStationManager'
CREATE TABLE [dbo].[SelfTakeStationManager] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [UserCode] nvarchar(512)  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [SelfTakeStationId] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL
);
GO

-- Creating table 'OrderPickUp'
CREATE TABLE [dbo].[OrderPickUp] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [SelfTakeStationId] uniqueidentifier  NOT NULL,
    [SelfTakeAddress] nvarchar(300)  NOT NULL,
    [PickUpCode] nvarchar(50)  NOT NULL,
    [PickUpQrCodeUrl] nvarchar(1000)  NOT NULL,
    [PickUpManagerId] uniqueidentifier  NULL,
    [PickUpTime] datetime  NULL,
    [SelfTakeProvince] nvarchar(20)  NULL,
    [SelfTakeCity] nvarchar(20)  NULL
);
GO

-- Creating table 'CommodityJournal'
CREATE TABLE [dbo].[CommodityJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [No_Number] int  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [Stock] int  NOT NULL,
    [PicturesPath] nvarchar(4000)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [State] int  NOT NULL,
    [IsDel] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [No_Code] nvarchar(512)  NOT NULL,
    [TotalCollection] int  NOT NULL,
    [TotalReview] int  NOT NULL,
    [Salesvolume] int  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [GroundTime] datetime  NULL,
    [ComAttribute] nvarchar(max)  NULL,
    [CategoryName] nvarchar(max)  NULL,
    [SortValue] int  NOT NULL,
    [FreightTemplateId] uniqueidentifier  NULL,
    [MarketPrice] decimal(18,2)  NULL,
    [IsEnableSelfTake] int  NOT NULL,
    [Weight] decimal(18,5)  NULL,
    [PricingMethod] tinyint  NOT NULL,
    [SaleAreas] nvarchar(max)  NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [SharePercent] decimal(18,5)  NULL,
    [HtmlVideoPath] nvarchar(3000)  NULL,
    [MobileVideoPath] nvarchar(3000)  NULL,
    [VideoPic] nvarchar(3000)  NULL,
    [VideoName] nvarchar(512)  NULL,
    [ScorePercent] decimal(18,5)  NULL,
    [Duty] decimal(18,2)  NULL
);
GO

-- Creating table 'CommodityOrderService'
CREATE TABLE [dbo].[CommodityOrderService] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [State] int  NOT NULL,
    [EndTime] datetime  NULL,
    [SelfTakeFlag] int  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [IsDelayConfirmTimeAfterSales] bit  NOT NULL
);
GO

-- Creating table 'OrderRefundAfterSales'
CREATE TABLE [dbo].[OrderRefundAfterSales] (
    [Id] uniqueidentifier  NOT NULL,
    [RefundType] int  NOT NULL,
    [RefundReason] nvarchar(128)  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL,
    [RefundDesc] nvarchar(225)  NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [State] int  NOT NULL,
    [ReceiverAccount] nvarchar(128)  NULL,
    [Receiver] nvarchar(max)  NULL,
    [RefundExpCo] nvarchar(50)  NULL,
    [RefundExpOrderNo] nvarchar(max)  NULL,
    [OrderRefundImgs] nvarchar(max)  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [DataType] nvarchar(max)  NOT NULL,
    [OrderItemId] uniqueidentifier  NULL,
    [RefuseTime] datetime  NULL,
    [IsFullRefund] int  NULL,
    [RefuseReason] nvarchar(max)  NULL,
    [NotReceiveTime] datetime  NULL,
    [RefundExpOrderTime] datetime  NULL,
    [RefundScoreMoney] decimal(18,2)  NOT NULL,
    [SalerRemark] nvarchar(max)  NULL,
    [RefundYJBMoney] decimal(18,2)  NOT NULL,
    [RefundFreightPrice] decimal(18,2)  NULL,
    [RefundChangeFreightPrice] decimal(18,2)  NULL,
    [RefundChangeRealPrice] decimal(18,2)  NULL,
    [RefundDuty] decimal(18,2)  NULL,
    [PickUpFreightMoney] decimal(18,2)  NULL,
    [PickwareType] int  NULL,
    [JDEclpOrderRefundAfterSalesId] uniqueidentifier  NULL,
    [PickwareAddress] nvarchar(256)  NULL,
    [CustomerTel] nvarchar(50)  NULL,
    [CustomerContactName] nvarchar(50)  NULL,
    [AuditUserId] uniqueidentifier  NULL,
    [AuditUserName] nvarchar(50)  NULL,
    [SendBackFreightMoney] decimal(18,2)  NULL,
    [ApplyId] nvarchar(128)  NULL,
    [RefundReceiveFullAddress] nvarchar(1024)  NULL,
    [RefundReceiveName] nvarchar(1024)  NULL,
    [RefundReceiveMobile] nvarchar(1024)  NULL,
    [RefundReturnType] int  NULL,
    [RefundCouponPirce] decimal(18,2)  NULL,
    [OrderRefundMoneyAndCoupun] decimal(18,2)  NULL,
    [RefundYJCouponMoney] decimal(18,2)  NULL
);
GO

-- Creating table 'AppExtension'
CREATE TABLE [dbo].[AppExtension] (
    [Id] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(64)  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [IsShowSearchMenu] bit  NOT NULL,
    [IsShowAddCart] bit  NOT NULL,
    [IsDividendAll] bit  NULL,
    [SharePercent] decimal(18,5)  NOT NULL,
    [IsCashForScore] bit  NOT NULL,
    [DistributeL1Percent] decimal(18,5)  NULL,
    [DistributeL2Percent] decimal(18,5)  NULL,
    [DistributeL3Percent] decimal(18,5)  NULL,
    [InvoiceDefault] int  NOT NULL,
    [InvoiceValues] int  NOT NULL,
    [ChannelSharePercent] decimal(18,2)  NULL,
    [IsScoreAll] bit  NULL,
    [ScorePercent] decimal(18,5)  NOT NULL,
    [IsSpreadAll] bit  NULL,
    [SpreadPercent] decimal(18,5)  NOT NULL
);
GO

-- Creating table 'ErrorCommodityOrder'
CREATE TABLE [dbo].[ErrorCommodityOrder] (
    [Id] uniqueidentifier  NOT NULL,
    [ErrorOrderId] uniqueidentifier  NOT NULL,
    [ResourceType] int  NOT NULL,
    [Source] int  NOT NULL,
    [State] int  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(50)  NOT NULL,
    [CouponId] uniqueidentifier  NOT NULL,
    [Score] int  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ScoreType] int  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'OrderExpressRoute'
CREATE TABLE [dbo].[OrderExpressRoute] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ShipExpCo] nvarchar(50)  NOT NULL,
    [ExpOrderNo] nvarchar(128)  NOT NULL,
    [ShipperCode] nvarchar(max)  NOT NULL,
    [State] tinyint  NOT NULL,
    [Deliverystatus] nvarchar(max)  NULL
);
GO

-- Creating table 'ExpressTrace'
CREATE TABLE [dbo].[ExpressTrace] (
    [Id] uniqueidentifier  NOT NULL,
    [ExpRouteId] uniqueidentifier  NOT NULL,
    [AcceptTime] datetime  NOT NULL,
    [AcceptStation] nvarchar(max)  NOT NULL,
    [Remark] nvarchar(max)  NULL
);
GO

-- Creating table 'ExpressCode'
CREATE TABLE [dbo].[ExpressCode] (
    [Id] uniqueidentifier  NOT NULL,
    [ExpCode] nvarchar(50)  NOT NULL,
    [ExpCompanyName] nvarchar(150)  NOT NULL,
    [OrderNum] int  NOT NULL
);
GO

-- Creating table 'OrderPayee'
CREATE TABLE [dbo].[OrderPayee] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [PayeeId] uniqueidentifier  NOT NULL,
    [PayeeType] int  NOT NULL,
    [Description] nvarchar(200)  NULL,
    [IsJHSelfUseAccount] bit  NOT NULL,
    [IsVoucherBuyGold] bit  NOT NULL,
    [PayMoney] decimal(18,2)  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ReCheckFlag] int  NOT NULL
);
GO

-- Creating table 'ScoreSetting'
CREATE TABLE [dbo].[ScoreSetting] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [ScoreCost] int  NULL,
    [Extract] nvarchar(250)  NULL
);
GO

-- Creating table 'Distributor'
CREATE TABLE [dbo].[Distributor] (
    [Id] uniqueidentifier  NOT NULL,
    [UserName] nvarchar(128)  NULL,
    [UserCode] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [ParentId] uniqueidentifier  NOT NULL,
    [Level] int  NOT NULL,
    [Key] nvarchar(max)  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [UserSubTime] datetime  NOT NULL,
    [PicturePath] nvarchar(max)  NULL,
    [Remarks] nvarchar(200)  NULL
);
GO

-- Creating table 'CommodityDistribution'
CREATE TABLE [dbo].[CommodityDistribution] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [L1Percent] decimal(18,5)  NULL,
    [L2Percent] decimal(18,5)  NULL,
    [L3Percent] decimal(18,5)  NULL
);
GO

-- Creating table 'CommodityDistributionJounal'
CREATE TABLE [dbo].[CommodityDistributionJounal] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [L1Percent] decimal(18,5)  NULL,
    [L2Percent] decimal(18,5)  NULL,
    [L3Percent] decimal(18,5)  NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL
);
GO

-- Creating table 'OrderItemShare'
CREATE TABLE [dbo].[OrderItemShare] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SharePrice] decimal(18,2)  NOT NULL,
    [Commission] decimal(18,2)  NOT NULL,
    [SharePercent] decimal(18,5)  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [PayeeType] int  NOT NULL,
    [PayeeId] uniqueidentifier  NOT NULL,
    [ShareKey] nvarchar(200)  NULL,
    [ShouldCommission] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'OrderShare'
CREATE TABLE [dbo].[OrderShare] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SharePrice] decimal(18,2)  NOT NULL,
    [Commission] decimal(18,2)  NOT NULL,
    [SharePercent] decimal(18,5)  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [PayeeType] int  NOT NULL,
    [PayeeId] uniqueidentifier  NOT NULL,
    [ShareKey] nvarchar(max)  NULL,
    [ShouldCommission] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'DiyGroup'
CREATE TABLE [dbo].[DiyGroup] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [PromotionId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ExpireTime] datetime  NOT NULL,
    [State] int  NOT NULL,
    [JoinNumber] int  NOT NULL,
    [SuccessProcessorId] uniqueidentifier  NULL,
    [SuccessTime] datetime  NULL,
    [FailProcessorId] uniqueidentifier  NULL,
    [FailTime] datetime  NULL,
    [EsAppId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'DiyGroupOrder'
CREATE TABLE [dbo].[DiyGroupOrder] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(50)  NOT NULL,
    [Role] int  NOT NULL,
    [DiyGroupId] uniqueidentifier  NOT NULL,
    [SubCode] nvarchar(128)  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [State] int  NOT NULL
);
GO

-- Creating table 'PaySource'
CREATE TABLE [dbo].[PaySource] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [Payment] int  NOT NULL,
    [TradeType] int  NOT NULL
);
GO

-- Creating table 'VatInvoiceProof'
CREATE TABLE [dbo].[VatInvoiceProof] (
    [Id] uniqueidentifier  NOT NULL,
    [CompanyName] nvarchar(300)  NOT NULL,
    [IdentifyNo] nvarchar(200)  NOT NULL,
    [Address] nvarchar(500)  NOT NULL,
    [Phone] nvarchar(50)  NOT NULL,
    [BankName] nvarchar(200)  NOT NULL,
    [BankCode] nvarchar(100)  NOT NULL,
    [BusinessLicence] nvarchar(2000)  NOT NULL,
    [TaxRegistration] nvarchar(2000)  NOT NULL,
    [PersonalProof] nvarchar(2000)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'InvoiceJounal'
CREATE TABLE [dbo].[InvoiceJounal] (
    [Id] uniqueidentifier  NOT NULL,
    [InvoiceId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [StateFrom] int  NOT NULL,
    [StateTo] int  NOT NULL
);
GO

-- Creating table 'AppSelfTakeStation'
CREATE TABLE [dbo].[AppSelfTakeStation] (
    [Id] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [Province] nvarchar(20)  NOT NULL,
    [City] nvarchar(20)  NOT NULL,
    [District] nvarchar(20)  NOT NULL,
    [Address] nvarchar(200)  NOT NULL,
    [Phone] nvarchar(100)  NULL,
    [DelayDay] int  NOT NULL,
    [MaxBookDay] int  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [IsDel] bit  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AppStsManager'
CREATE TABLE [dbo].[AppStsManager] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [UserCode] nvarchar(512)  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [SelfTakeStationId] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AppStsOfficeTime'
CREATE TABLE [dbo].[AppStsOfficeTime] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [WeekDays] int  NOT NULL,
    [StartTime] time  NOT NULL,
    [EndTime] time  NOT NULL,
    [SelfTakeStationId] uniqueidentifier  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AppOrderPickUp'
CREATE TABLE [dbo].[AppOrderPickUp] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [Phone] nvarchar(100)  NULL,
    [BookDate] datetime  NULL,
    [BookStartTime] time  NULL,
    [BookEndTime] time  NULL,
    [PickUpCode] nvarchar(50)  NOT NULL,
    [PickUpQrCodeUrl] nvarchar(1000)  NOT NULL,
    [SelfTakeStationId] uniqueidentifier  NOT NULL,
    [StsProvince] nvarchar(20)  NULL,
    [StsCity] nvarchar(20)  NULL,
    [StsDistrict] nvarchar(20)  NOT NULL,
    [StsAddress] nvarchar(300)  NOT NULL,
    [StsPhone] nvarchar(100)  NULL,
    [StsName] nvarchar(100)  NOT NULL,
    [PickUpTime] datetime  NULL,
    [PickUpManagerId] uniqueidentifier  NULL,
    [UserId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'CateringSetting'
CREATE TABLE [dbo].[CateringSetting] (
    [Id] uniqueidentifier  NOT NULL,
    [Unit] nvarchar(10)  NOT NULL,
    [Specification] nvarchar(50)  NOT NULL,
    [DeliveryAmount] decimal(18,2)  NOT NULL,
    [DeliveryRange] float  NOT NULL,
    [DeliveryFee] decimal(18,2)  NOT NULL,
    [MostCoupon] decimal(18,2)  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [StoreId] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL,
    [FreeAmount] decimal(18,2)  NULL,
    [DeliveryFeeStartT] datetime  NULL,
    [DeliveryFeeEndT] datetime  NULL,
    [DeliveryFeeDiscount] float  NULL
);
GO

-- Creating table 'WeChatQRCode'
CREATE TABLE [dbo].[WeChatQRCode] (
    [Id] uniqueidentifier  NOT NULL,
    [WeChatPublicCode] nvarchar(100)  NOT NULL,
    [WeChatAppId] nvarchar(100)  NOT NULL,
    [WeChatSecret] nvarchar(100)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [QRNo] int IDENTITY(1,1) NOT NULL,
    [QRType_Value] int  NOT NULL,
    [WeChatTicket] nvarchar(200)  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [StoreId] uniqueidentifier  NOT NULL,
    [IsDel] int  NOT NULL,
    [IsUse] bit  NOT NULL,
    [SpreadInfoId] uniqueidentifier  NOT NULL,
    [Description] nvarchar(500)  NULL,
    [Name] nvarchar(512)  NOT NULL
);
GO

-- Creating table 'CateringComdtyXData'
CREATE TABLE [dbo].[CateringComdtyXData] (
    [Id] uniqueidentifier  NOT NULL,
    [Unit] nvarchar(10)  NOT NULL,
    [MealBoxAmount] decimal(18,2)  NOT NULL,
    [MealBoxNum] int  NOT NULL,
    [MinPurchaseQuantity] int  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ComdtyId] uniqueidentifier  NOT NULL,
    [IsValid] bit  NULL
);
GO

-- Creating table 'CateringBusinessHours'
CREATE TABLE [dbo].[CateringBusinessHours] (
    [Id] uniqueidentifier  NOT NULL,
    [OpeningTime] datetime  NOT NULL,
    [ClosingTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [IsValid] bit  NOT NULL,
    [CateringSettingId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'CateringShiftTime'
CREATE TABLE [dbo].[CateringShiftTime] (
    [Id] uniqueidentifier  NOT NULL,
    [ShiftTime] datetime  NOT NULL,
    [IsValid] bit  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [CateringSettingId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NULL
);
GO

-- Creating table 'ShiftTimeLog'
CREATE TABLE [dbo].[ShiftTimeLog] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [ShiftTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [StoreId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'OrderRefundInfo'
CREATE TABLE [dbo].[OrderRefundInfo] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [ItemId] uniqueidentifier  NOT NULL,
    [Refund] decimal(18,2)  NOT NULL,
    [isDelivery] bit  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [OrderRefundId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ExpressOrderTemplate'
CREATE TABLE [dbo].[ExpressOrderTemplate] (
    [Id] uniqueidentifier  NOT NULL,
    [ExpressCode] nvarchar(50)  NOT NULL,
    [TemplateName] nvarchar(30)  NOT NULL,
    [ExpressImage] nvarchar(300)  NOT NULL,
    [Top] float  NOT NULL,
    [Left] float  NOT NULL,
    [Width] float  NOT NULL,
    [Height] float  NOT NULL,
    [TemplateType] int  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NULL,
    [CreateTime] datetime  NOT NULL,
    [Status] int  NOT NULL
);
GO

-- Creating table 'ExpressOrderTemplateProperty'
CREATE TABLE [dbo].[ExpressOrderTemplateProperty] (
    [Id] uniqueidentifier  NOT NULL,
    [TemplateId] uniqueidentifier  NOT NULL,
    [PropertyName] nvarchar(50)  NULL,
    [PropertyType] int  NOT NULL,
    [Top] float  NOT NULL,
    [Left] float  NOT NULL,
    [Width] float  NOT NULL,
    [Height] float  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [PropertyText] nvarchar(30)  NULL
);
GO

-- Creating table 'OrderPrintLog'
CREATE TABLE [dbo].[OrderPrintLog] (
    [Id] uniqueidentifier  NOT NULL,
    [PrintUserId] uniqueidentifier  NOT NULL,
    [PrintType] int  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'OrderPrintDetailLog'
CREATE TABLE [dbo].[OrderPrintDetailLog] (
    [Id] uniqueidentifier  NOT NULL,
    [PrintId] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [ExpressNo] nvarchar(30)  NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'DistributionRule'
CREATE TABLE [dbo].[DistributionRule] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [ModifiedId] uniqueidentifier  NOT NULL,
    [HasCondition] bit  NOT NULL,
    [NeedIdentity] bit  NOT NULL,
    [OrderTimeLimit] int  NOT NULL,
    [OrderAmountLimit] decimal(18,2)  NOT NULL,
    [HasCustomComs] bit  NOT NULL,
    [Title] nvarchar(40)  NOT NULL,
    [ApprovalType_Value] int  NOT NULL,
    [StartCalcState_Value] int  NOT NULL,
    [RuleDesc] nvarchar(max)  NULL
);
GO

-- Creating table 'DistributionIdentitySet'
CREATE TABLE [dbo].[DistributionIdentitySet] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [ModifiedId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(20)  NOT NULL,
    [IsRequired] bit  NOT NULL,
    [RuleId] uniqueidentifier  NOT NULL,
    [ValueType_Value] int  NOT NULL,
    [IsDel] bit  NOT NULL,
    [NameCategory] int  NOT NULL
);
GO

-- Creating table 'DistributionIdentity'
CREATE TABLE [dbo].[DistributionIdentity] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [RuleId] uniqueidentifier  NOT NULL,
    [IdentitySetId] uniqueidentifier  NOT NULL,
    [ApplyId] uniqueidentifier  NOT NULL,
    [Value] nvarchar(3000)  NULL,
    [ValueType_Value] int  NOT NULL,
    [NameCategory] int  NOT NULL
);
GO

-- Creating table 'DistributionApply'
CREATE TABLE [dbo].[DistributionApply] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [HasIdentity] bit  NOT NULL,
    [RuleId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [UserCode] nvarchar(200)  NOT NULL,
    [UserName] nvarchar(500)  NOT NULL,
    [PicturePath] nvarchar(2000)  NOT NULL,
    [Remarks] nvarchar(200)  NULL,
    [State_Value] int  NOT NULL,
    [RefuseReason] nvarchar(200)  NULL,
    [AuditorId] uniqueidentifier  NULL,
    [AuditTime] datetime  NULL,
    [ParentId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Microshop'
CREATE TABLE [dbo].[Microshop] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [Logo] nvarchar(2000)  NULL,
    [QRCodeUrl] nvarchar(2000)  NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Url] nvarchar(2000)  NULL,
    [Type_Value] int  NOT NULL,
    [Key] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'MicroshopCom'
CREATE TABLE [dbo].[MicroshopCom] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [MicroshopId] uniqueidentifier  NULL,
    [IsDel] bit  NOT NULL
);
GO

-- Creating table 'DistributionApplyAudit'
CREATE TABLE [dbo].[DistributionApplyAudit] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Details] nvarchar(max)  NULL,
    [ApplyId] uniqueidentifier  NOT NULL,
    [RefuseReason] nvarchar(1000)  NULL,
    [IsPass] bit  NOT NULL
);
GO

-- Creating table 'WeChatQrCodeType'
CREATE TABLE [dbo].[WeChatQrCodeType] (
    [Id] uniqueidentifier  NOT NULL,
    [Type] int  NOT NULL,
    [Description] nvarchar(500)  NULL,
    [UseTye] int  NOT NULL
);
GO

-- Creating table 'MallApply'
CREATE TABLE [dbo].[MallApply] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [ApplyContent] nvarchar(2000)  NULL,
    [State_Value] int  NOT NULL,
    [Type] smallint  NOT NULL,
    [ThirdECommerceId] uniqueidentifier  NULL,
    [CrcAppId] bigint  NOT NULL
);
GO

-- Creating table 'BaseCommission'
CREATE TABLE [dbo].[BaseCommission] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [MallApplyId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [EffectiveTime] datetime  NOT NULL,
    [Commission] decimal(18,2)  NOT NULL,
    [IsDel] bit  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'CategoryCommission'
CREATE TABLE [dbo].[CategoryCommission] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [MallApplyId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [EffectiveTime] datetime  NOT NULL,
    [Commission] decimal(18,2)  NOT NULL,
    [CategoryId] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL
);
GO

-- Creating table 'CommodityCommission'
CREATE TABLE [dbo].[CommodityCommission] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [MallApplyId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [EffectiveTime] datetime  NOT NULL,
    [Commission] decimal(18,2)  NOT NULL,
    [CategoryId] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [CommodityName] nvarchar(200)  NULL
);
GO

-- Creating table 'SettleAccounts'
CREATE TABLE [dbo].[SettleAccounts] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NULL,
    [SellerType] smallint  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NULL,
    [OrderAmount] decimal(18,2)  NOT NULL,
    [OrderYJBAmount] decimal(18,2)  NOT NULL,
    [SellerAmount] decimal(18,2)  NOT NULL,
    [AmountDate] datetime  NOT NULL,
    [IsAmount] bit  NOT NULL,
    [CouponAmount] decimal(18,2)  NOT NULL,
    [RefundAmount] decimal(18,2)  NOT NULL,
    [PromotionCommissionAmount] decimal(18,2)  NOT NULL,
    [PromotionAmount] decimal(18,2)  NOT NULL,
    [Remark] nvarchar(2000)  NULL,
    [SettleStatue] bit  NOT NULL,
    [State_Value] int  NOT NULL,
    [OrderRealAmount] decimal(18,2)  NOT NULL,
    [Code] nvarchar(18)  NOT NULL,
    [IsPaySuccess] bit  NULL,
    [AmountStartDate] datetime  NULL
);
GO

-- Creating table 'SettleAccountsDetails'
CREATE TABLE [dbo].[SettleAccountsDetails] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SAId] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [OrderSubTime] datetime  NOT NULL,
    [OrderAmount] decimal(18,2)  NOT NULL,
    [OrderRealAmount] decimal(18,2)  NOT NULL,
    [OrderFreight] decimal(18,2)  NOT NULL,
    [OrderRefundAmount] decimal(18,2)  NOT NULL,
    [OrderCouponAmount] decimal(18,2)  NOT NULL,
    [OrderYJBAmount] decimal(18,2)  NOT NULL,
    [OrderPromotionCommissionAmount] decimal(18,2)  NOT NULL,
    [PromotionAmount] decimal(18,2)  NOT NULL,
    [SellerAmount] decimal(18,2)  NOT NULL,
    [IsMallCoupon] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [IsSettled] bit  NOT NULL,
    [SettleAmount] decimal(18,2)  NOT NULL,
    [IsSetSettleAmount] bit  NOT NULL,
    [OrderSpreadAmount] decimal(18,2)  NULL,
    [OrderConfirmTime] datetime  NULL,
    [AfterSalesEndTime] datetime  NULL
);
GO

-- Creating table 'SettleAccountsOrderItem'
CREATE TABLE [dbo].[SettleAccountsOrderItem] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [OrderItemRefundAmount] decimal(18,2)  NOT NULL,
    [OrderItemYJBAmount] decimal(18,2)  NOT NULL,
    [OrderItemPromotionCommissionAmount] decimal(18,2)  NOT NULL,
    [OrderItemName] nvarchar(512)  NOT NULL,
    [OrderItemNumber] int  NOT NULL,
    [OrderItemPrice] decimal(18,2)  NOT NULL,
    [SettleAmount] decimal(18,2)  NOT NULL,
    [BaseCommission] decimal(18,2)  NULL,
    [CategoryCommission] decimal(18,2)  NULL,
    [CommodityCommission] decimal(18,2)  NULL,
    [PromotionAmount] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'SettleAccountsPeriod'
CREATE TABLE [dbo].[SettleAccountsPeriod] (
    [Id] uniqueidentifier  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [NumOfDay] int  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [UseAfterSalesEndTime] bit  NULL
);
GO

-- Creating table 'CommoditySettleAmount'
CREATE TABLE [dbo].[CommoditySettleAmount] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [UserName] nvarchar(128)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [CommodityType] smallint  NOT NULL,
    [CommodityAttrName] nvarchar(256)  NULL,
    [CommoditySecAttrName] nvarchar(256)  NULL,
    [CommodityAttrJson] nvarchar(max)  NULL,
    [EffectiveTime] datetime  NOT NULL
);
GO

-- Creating table 'HTJSInvoice'
CREATE TABLE [dbo].[HTJSInvoice] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SwNo] nvarchar(max)  NOT NULL,
    [FMsgCode] nvarchar(max)  NOT NULL,
    [FMsg] nvarchar(max)  NOT NULL,
    [Fpdm] nvarchar(max)  NULL,
    [Fphm] nvarchar(max)  NULL,
    [Kprq] datetime  NULL,
    [PdfContent] nvarchar(max)  NULL,
    [PdfMd5] nvarchar(max)  NULL,
    [SMsgCode] nvarchar(max)  NULL,
    [SMsg] nvarchar(max)  NULL,
    [RefundType] int  NOT NULL
);
GO

-- Creating table 'YJBJOrderItem'
CREATE TABLE [dbo].[YJBJOrderItem] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [Number] int  NOT NULL,
    [RealPrice] decimal(18,2)  NOT NULL,
    [ShouldMoney] decimal(18,2)  NOT NULL,
    [DiscountMoney] decimal(18,2)  NOT NULL,
    [PayMoney] decimal(18,2)  NOT NULL,
    [TaxMoney] decimal(18,2)  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL,
    [CommodityName] nvarchar(512)  NOT NULL,
    [CategoryName] nvarchar(max)  NOT NULL,
    [TaxRate] decimal(18,2)  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [FreightMoney] decimal(18,2)  NOT NULL,
    [TradeId] nvarchar(60)  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [AppType] smallint  NOT NULL,
    [RefundFreightMoney] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'CommodityTaxRate'
CREATE TABLE [dbo].[CommodityTaxRate] (
    [Code] nvarchar(255)  NOT NULL,
    [Name] nvarchar(255)  NULL,
    [Content] nvarchar(255)  NULL,
    [TaxRate] float  NULL,
    [KeyWord] nvarchar(255)  NULL,
    [IsCombine] nvarchar(255)  NULL,
    [VersionCode] nvarchar(255)  NULL,
    [IsUse] nvarchar(255)  NULL,
    [DutyState] nvarchar(255)  NULL,
    [Policy] nvarchar(255)  NULL,
    [PolicyCode] nvarchar(255)  NULL,
    [ConsumeState] nvarchar(255)  NULL,
    [ConsumePolicy] nvarchar(255)  NULL,
    [ConsumePolicyCode] nvarchar(255)  NULL,
    [TradeCode] float  NULL,
    [CIQCategory] nvarchar(255)  NULL,
    [FiringTime] nvarchar(255)  NULL,
    [EndTime] nvarchar(255)  NULL,
    [UpdateTime] nvarchar(255)  NULL
);
GO

-- Creating table 'OrderField'
CREATE TABLE [dbo].[OrderField] (
    [Id] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [FirstField] nvarchar(50)  NULL,
    [SecondField] nvarchar(50)  NULL,
    [ThirdField] nvarchar(50)  NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'InnerCategory'
CREATE TABLE [dbo].[InnerCategory] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [ParentId] uniqueidentifier  NULL,
    [CurrentLevel] int  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Sort] int  NOT NULL,
    [IsDel] bit  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'Supplier'
CREATE TABLE [dbo].[Supplier] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(512)  NOT NULL,
    [SupplierName] nvarchar(512)  NOT NULL,
    [SupplierCode] nvarchar(128)  NOT NULL,
    [SupplierType] smallint  NOT NULL,
    [ShipperType] smallint  NOT NULL,
    [SupplierMainId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'CommodityCodeSeq'
CREATE TABLE [dbo].[CommodityCodeSeq] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [Code] nvarchar(256)  NOT NULL,
    [Offset] int  NOT NULL
);
GO

-- Creating table 'CommodityInnerCategory'
CREATE TABLE [dbo].[CommodityInnerCategory] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [CategoryId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [MaxSort] float  NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CrcAppId] bigint  NOT NULL,
    [IsDel] bit  NULL
);
GO

-- Creating table 'ExpressCollection'
CREATE TABLE [dbo].[ExpressCollection] (
    [Id] uniqueidentifier  NOT NULL,
    [ExpCode] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ExpressTemplateCollection'
CREATE TABLE [dbo].[ExpressTemplateCollection] (
    [Id] uniqueidentifier  NOT NULL,
    [TemplateId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NULL
);
GO

-- Creating table 'SupplierMain'
CREATE TABLE [dbo].[SupplierMain] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [AppIds] nvarchar(512)  NOT NULL,
    [AppNames] nvarchar(512)  NOT NULL,
    [SupplierName] nvarchar(512)  NOT NULL,
    [SupplierCode] nvarchar(128)  NOT NULL,
    [SupplierType] smallint  NOT NULL,
    [ShipperType] smallint  NOT NULL
);
GO

-- Creating table 'JdOrderItem'
CREATE TABLE [dbo].[JdOrderItem] (
    [Id] uniqueidentifier  NOT NULL,
    [JdPorderId] nvarchar(100)  NOT NULL,
    [TempId] uniqueidentifier  NOT NULL,
    [JdOrderId] nvarchar(100)  NULL,
    [MainOrderId] nvarchar(100)  NULL,
    [CommodityOrderId] nvarchar(100)  NULL,
    [State] int  NOT NULL,
    [StateContent] nvarchar(100)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [CommoditySkuId] nvarchar(128)  NULL,
    [CommodityOrderItemId] uniqueidentifier  NULL,
    [IsRefund] bit  NULL,
    [JdTrackState] int  NULL,
    [JdTrackStateContent] nvarchar(max)  NULL
);
GO

-- Creating table 'JdJournal'
CREATE TABLE [dbo].[JdJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [JdPorderId] nvarchar(100)  NOT NULL,
    [TempId] uniqueidentifier  NOT NULL,
    [JdOrderId] nvarchar(100)  NULL,
    [MainOrderId] nvarchar(100)  NULL,
    [CommodityOrderId] nvarchar(100)  NULL,
    [Name] nvarchar(100)  NULL,
    [Details] nvarchar(100)  NULL,
    [SubTime] datetime  NOT NULL
);
GO

-- Creating table 'ServiceSettings'
CREATE TABLE [dbo].[ServiceSettings] (
    [Id] uniqueidentifier  NOT NULL,
    [Title] nvarchar(50)  NULL,
    [Content] nvarchar(50)  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Isdisable] bit  NOT NULL,
    [OrderNo] int  NULL
);
GO

-- Creating table 'YJBJCard'
CREATE TABLE [dbo].[YJBJCard] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [CardName] nvarchar(50)  NOT NULL,
    [CardNo] nvarchar(50)  NOT NULL,
    [CheckCode] nvarchar(50)  NOT NULL,
    [EndTime] datetime  NOT NULL,
    [CouponUrl] nvarchar(256)  NOT NULL,
    [Status] int  NOT NULL,
    [Message] nvarchar(200)  NOT NULL,
    [SubTime] datetime  NOT NULL
);
GO

-- Creating table 'Jdlogs'
CREATE TABLE [dbo].[Jdlogs] (
    [Id] uniqueidentifier  NOT NULL,
    [Content] nvarchar(100)  NULL,
    [Remark] nvarchar(100)  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Isdisable] bit  NOT NULL,
    [ThirdECommerceType] int  NULL
);
GO

-- Creating table 'PresentPromotion'
CREATE TABLE [dbo].[PresentPromotion] (
    [Id] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(256)  NOT NULL,
    [BeginTime] datetime  NOT NULL,
    [EndTime] datetime  NOT NULL,
    [Limit] int  NULL,
    [IsEnd] bit  NOT NULL
);
GO

-- Creating table 'PresentPromotionCommodity'
CREATE TABLE [dbo].[PresentPromotionCommodity] (
    [Id] uniqueidentifier  NOT NULL,
    [PresentPromotionId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [CommodityCode] nvarchar(128)  NULL,
    [CommodityName] nvarchar(256)  NOT NULL,
    [CommoditySKUId] uniqueidentifier  NOT NULL,
    [CommoditySKU] nvarchar(256)  NULL,
    [CommoditySKUCode] nvarchar(128)  NULL,
    [CommodityPrice] decimal(18,0)  NOT NULL,
    [Limit] int  NOT NULL
);
GO

-- Creating table 'PresentPromotionGift'
CREATE TABLE [dbo].[PresentPromotionGift] (
    [Id] uniqueidentifier  NOT NULL,
    [PresentPromotionId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [CommodityCode] nvarchar(128)  NULL,
    [CommodityName] nvarchar(256)  NOT NULL,
    [CommoditySKUId] uniqueidentifier  NOT NULL,
    [CommoditySKU] nvarchar(256)  NULL,
    [CommoditySKUCode] nvarchar(128)  NULL,
    [CommodityPrice] decimal(18,0)  NOT NULL,
    [Number] int  NOT NULL
);
GO

-- Creating table 'OrderItemPresent'
CREATE TABLE [dbo].[OrderItemPresent] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Number] int  NOT NULL,
    [CurrentPrice] decimal(18,2)  NOT NULL,
    [PicturesPath] nvarchar(4000)  NOT NULL,
    [CommodityOrderId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [PromotionId] uniqueidentifier  NULL,
    [ComAttributeIds] nvarchar(max)  NULL,
    [CategoryNames] nvarchar(max)  NULL,
    [CommodityAttributes] nvarchar(500)  NULL,
    [RealPrice] decimal(18,2)  NULL,
    [Intensity] decimal(18,2)  NULL,
    [AlreadyReview] bit  NOT NULL,
    [DiscountPrice] decimal(18,2)  NULL,
    [CommodityStockId] uniqueidentifier  NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ShareMoney] decimal(18,2)  NOT NULL,
    [PromotionDesc] nvarchar(200)  NULL,
    [PromotionType] int  NULL,
    [VipLevelId] uniqueidentifier  NULL,
    [ComCategoryId] uniqueidentifier  NULL,
    [ComCategoryName] nvarchar(512)  NULL,
    [ScorePrice] decimal(18,2)  NOT NULL,
    [Duty] decimal(18,2)  NOT NULL,
    [TaxRate] decimal(18,2)  NULL,
    [InputRax] decimal(18,2)  NULL,
    [CostPrice] decimal(18,2)  NULL,
    [Barcode] nvarchar(128)  NULL,
    [No_Code] nvarchar(128)  NULL,
    [InnerCatetoryIds] nvarchar(512)  NULL,
    [Unit] nvarchar(256)  NULL,
    [YouKaPercent] decimal(18,2)  NULL,
    [DeliveryTime] datetime  NULL,
    [DeliveryDays] int  NULL,
    [Type] int  NULL,
    [YJCouponActivityId] nvarchar(32)  NULL,
    [YJCouponType] nvarchar(10)  NULL,
    [OrderItemId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'SettleAccountsException'
CREATE TABLE [dbo].[SettleAccountsException] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [OrderRealPrice] decimal(18,0)  NULL,
    [ClearingPrice] decimal(18,0)  NULL,
    [ExceptionInfo] nvarchar(max)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NULL
);
GO

-- Creating table 'OrderStatistics'
CREATE TABLE [dbo].[OrderStatistics] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SumRealPrice] decimal(18,0)  NOT NULL,
    [OrderCount] int  NOT NULL,
    [LastSubTime] datetime  NOT NULL
);
GO

-- Creating table 'CommodityChange'
CREATE TABLE [dbo].[CommodityChange] (
    [CommodityId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [No_Number] int  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [Stock] int  NOT NULL,
    [PicturesPath] nvarchar(4000)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [State] int  NOT NULL,
    [IsDel] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [No_Code] nvarchar(512)  NULL,
    [TotalCollection] int  NOT NULL,
    [TotalReview] int  NOT NULL,
    [Salesvolume] int  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [GroundTime] datetime  NULL,
    [ComAttribute] nvarchar(max)  NULL,
    [CategoryName] nvarchar(max)  NULL,
    [SortValue] int  NOT NULL,
    [FreightTemplateId] uniqueidentifier  NULL,
    [MarketPrice] decimal(18,2)  NULL,
    [IsEnableSelfTake] int  NOT NULL,
    [Weight] decimal(18,5)  NULL,
    [PricingMethod] tinyint  NOT NULL,
    [SaleAreas] nvarchar(max)  NULL,
    [SharePercent] decimal(18,5)  NULL,
    [CommodityType] int  NOT NULL,
    [HtmlVideoPath] nvarchar(3000)  NULL,
    [MobileVideoPath] nvarchar(3000)  NULL,
    [VideoPic] nvarchar(3000)  NULL,
    [VideoName] nvarchar(512)  NULL,
    [ScorePercent] decimal(18,5)  NULL,
    [Duty] decimal(18,2)  NULL,
    [SpreadPercent] decimal(18,5)  NULL,
    [ScoreScale] decimal(18,5)  NULL,
    [TaxRate] decimal(18,2)  NULL,
    [TaxClassCode] nvarchar(64)  NULL,
    [Unit] nvarchar(max)  NULL,
    [InputRax] decimal(18,2)  NULL,
    [Barcode] nvarchar(128)  NULL,
    [JDCode] nvarchar(128)  NULL,
    [CostPrice] decimal(18,2)  NULL,
    [IsAssurance] bit  NULL,
    [TechSpecs] nvarchar(max)  NULL,
    [SaleService] nvarchar(max)  NULL,
    [IsReturns] bit  NULL,
    [ServiceSettingId] nvarchar(400)  NULL,
    [Type] int  NULL,
    [YJCouponActivityId] nvarchar(32)  NULL,
    [YJCouponType] nvarchar(10)  NULL,
    [ModifiedId] uniqueidentifier  NULL,
    [AuditState] int  NOT NULL,
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL
);
GO

-- Creating table 'JdOrderRefundAfterSales'
CREATE TABLE [dbo].[JdOrderRefundAfterSales] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [OrderRefundAfterSalesId] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [JdOrderId] nvarchar(128)  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [SkuId] nvarchar(128)  NOT NULL,
    [AfsServiceId] nvarchar(128)  NULL,
    [AfsServiceStep] int  NOT NULL,
    [AfsServiceStepName] nvarchar(128)  NOT NULL,
    [Cancel] smallint  NOT NULL,
    [PickwareType] int  NOT NULL,
    [CustomerContactName] nvarchar(128)  NOT NULL,
    [CustomerTel] nvarchar(128)  NOT NULL,
    [PickwareAddress] nvarchar(256)  NOT NULL,
    [CommodityNum] int  NULL,
    [AfsServiceIds] nvarchar(512)  NULL
);
GO

-- Creating table 'LongisticsTrack'
CREATE TABLE [dbo].[LongisticsTrack] (
    [Id] uniqueidentifier  NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [AppName] nvarchar(max)  NULL,
    [SupplierName] nvarchar(max)  NULL,
    [SupplierType] nvarchar(max)  NULL,
    [CommodityDetail] nvarchar(max)  NULL,
    [Ordertime] datetime  NULL,
    [UploadExpresstime] datetime  NULL,
    [Expressdeliverytime] datetime  NULL,
    [ExpressSdtime] datetime  NULL,
    [Confirmtime] datetime  NULL,
    [SubTime] datetime  NOT NULL,
    [IsDel] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [SupplierCode] nvarchar(max)  NULL
);
GO

-- Creating table 'JdExpressTrace'
CREATE TABLE [dbo].[JdExpressTrace] (
    [Id] uniqueidentifier  NOT NULL,
    [ExpRouteId] uniqueidentifier  NOT NULL,
    [AcceptTime] datetime  NOT NULL,
    [AcceptStation] nvarchar(max)  NULL,
    [Remark] nvarchar(max)  NULL
);
GO

-- Creating table 'AuditCommodity'
CREATE TABLE [dbo].[AuditCommodity] (
    [Id] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [No_Number] int  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [Stock] int  NOT NULL,
    [PicturesPath] nvarchar(4000)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [State] int  NOT NULL,
    [IsDel] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [No_Code] nvarchar(512)  NOT NULL,
    [TotalCollection] int  NOT NULL,
    [TotalReview] int  NOT NULL,
    [Salesvolume] int  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [GroundTime] datetime  NULL,
    [ComAttribute] nvarchar(max)  NULL,
    [CategoryName] nvarchar(max)  NULL,
    [SortValue] int  NOT NULL,
    [FreightTemplateId] uniqueidentifier  NULL,
    [MarketPrice] decimal(18,2)  NULL,
    [IsEnableSelfTake] int  NOT NULL,
    [Weight] decimal(18,5)  NULL,
    [PricingMethod] tinyint  NOT NULL,
    [SaleAreas] nvarchar(max)  NULL,
    [SharePercent] decimal(18,5)  NULL,
    [CommodityType] int  NOT NULL,
    [HtmlVideoPath] nvarchar(3000)  NULL,
    [MobileVideoPath] nvarchar(3000)  NULL,
    [VideoPic] nvarchar(3000)  NULL,
    [VideoName] nvarchar(512)  NULL,
    [ScorePercent] decimal(18,5)  NULL,
    [Duty] decimal(18,2)  NULL,
    [SpreadPercent] decimal(18,5)  NULL,
    [ScoreScale] decimal(18,5)  NULL,
    [TaxRate] decimal(18,2)  NULL,
    [TaxClassCode] nvarchar(64)  NULL,
    [Unit] nvarchar(max)  NULL,
    [InputRax] decimal(18,2)  NULL,
    [Barcode] nvarchar(128)  NULL,
    [JDCode] nvarchar(128)  NULL,
    [CostPrice] decimal(18,2)  NULL,
    [IsAssurance] bit  NULL,
    [TechSpecs] nvarchar(max)  NULL,
    [SaleService] nvarchar(max)  NULL,
    [IsReturns] bit  NULL,
    [ServiceSettingId] nvarchar(400)  NULL,
    [Type] int  NULL,
    [YJCouponActivityId] nvarchar(32)  NULL,
    [YJCouponType] nvarchar(10)  NULL,
    [ModifieId] uniqueidentifier  NULL,
    [FieldName] nvarchar(4000)  NULL,
    [RowNo] int  NOT NULL
);
GO

-- Creating table 'AuditCommodityStock'
CREATE TABLE [dbo].[AuditCommodityStock] (
    [Id] uniqueidentifier  NOT NULL,
    [CommodityStockId] uniqueidentifier  NOT NULL,
    [AuditId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [ComAttribute] nvarchar(max)  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [Stock] int  NOT NULL,
    [MarketPrice] decimal(18,0)  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [Duty] decimal(18,2)  NULL,
    [Barcode] nvarchar(128)  NULL,
    [No_Code] nvarchar(128)  NULL,
    [JDCode] nvarchar(128)  NULL,
    [CostPrice] decimal(18,2)  NULL,
    [ThumImg] nvarchar(1000)  NULL,
    [CarouselImgs] nvarchar(4000)  NULL
);
GO

-- Creating table 'AuditManage'
CREATE TABLE [dbo].[AuditManage] (
    [Id] uniqueidentifier  NOT NULL,
    [Status] int  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [ApplyUserId] uniqueidentifier  NOT NULL,
    [AuditUserId] uniqueidentifier  NULL,
    [ApplyTime] datetime  NOT NULL,
    [AuditTime] datetime  NULL,
    [AuditRemark] nvarchar(max)  NULL,
    [Action] int  NOT NULL
);
GO

-- Creating table 'AuditMode'
CREATE TABLE [dbo].[AuditMode] (
    [Id] uniqueidentifier  NOT NULL,
    [ModeStatus] int  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedId] uniqueidentifier  NULL,
    [ModifiedOn] datetime  NULL
);
GO

-- Creating table 'JDAuditMode'
CREATE TABLE [dbo].[JDAuditMode] (
    [Id] uniqueidentifier  NOT NULL,
    [PriceModeState] int  NOT NULL,
    [CostModeState] int  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedId] uniqueidentifier  NULL,
    [ModifiedOn] datetime  NULL,
    [StockModeState] int  NOT NULL
);
GO

-- Creating table 'JdAuditCommodity'
CREATE TABLE [dbo].[JdAuditCommodity] (
    [Id] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [No_Number] int  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [Stock] int  NOT NULL,
    [PicturesPath] nvarchar(4000)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [State] int  NOT NULL,
    [IsDel] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [No_Code] nvarchar(512)  NOT NULL,
    [TotalCollection] int  NOT NULL,
    [TotalReview] int  NOT NULL,
    [Salesvolume] int  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [GroundTime] datetime  NULL,
    [ComAttribute] nvarchar(max)  NULL,
    [CategoryName] nvarchar(max)  NULL,
    [SortValue] int  NOT NULL,
    [FreightTemplateId] uniqueidentifier  NULL,
    [MarketPrice] decimal(18,2)  NULL,
    [IsEnableSelfTake] int  NOT NULL,
    [Weight] decimal(18,5)  NULL,
    [PricingMethod] tinyint  NOT NULL,
    [SaleAreas] nvarchar(max)  NULL,
    [SharePercent] decimal(18,5)  NULL,
    [CommodityType] int  NOT NULL,
    [HtmlVideoPath] nvarchar(3000)  NULL,
    [MobileVideoPath] nvarchar(3000)  NULL,
    [VideoPic] nvarchar(3000)  NULL,
    [VideoName] nvarchar(512)  NULL,
    [ScorePercent] decimal(18,5)  NULL,
    [Duty] decimal(18,2)  NULL,
    [SpreadPercent] decimal(18,5)  NULL,
    [ScoreScale] decimal(18,5)  NULL,
    [TaxRate] decimal(18,2)  NULL,
    [TaxClassCode] nvarchar(64)  NULL,
    [Unit] nvarchar(max)  NULL,
    [InputRax] decimal(18,2)  NULL,
    [Barcode] nvarchar(128)  NULL,
    [JDCode] nvarchar(128)  NULL,
    [CostPrice] decimal(18,2)  NULL,
    [IsAssurance] bit  NULL,
    [TechSpecs] nvarchar(max)  NULL,
    [SaleService] nvarchar(max)  NULL,
    [IsReturns] bit  NULL,
    [ServiceSettingId] nvarchar(400)  NULL,
    [Type] int  NULL,
    [YJCouponActivityId] nvarchar(32)  NULL,
    [YJCouponType] nvarchar(10)  NULL,
    [ModifieId] uniqueidentifier  NULL,
    [FieldName] nvarchar(4000)  NULL,
    [RowNo] int  NOT NULL
);
GO

-- Creating table 'JdAuditCommodityStock'
CREATE TABLE [dbo].[JdAuditCommodityStock] (
    [Id] uniqueidentifier  NOT NULL,
    [CommodityStockId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [ComAttribute] nvarchar(max)  NOT NULL,
    [Price] decimal(18,2)  NOT NULL,
    [Stock] int  NOT NULL,
    [MarketPrice] decimal(18,0)  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [Duty] decimal(18,2)  NULL,
    [Barcode] nvarchar(128)  NULL,
    [No_Code] nvarchar(128)  NULL,
    [JDCode] nvarchar(128)  NULL,
    [CostPrice] decimal(18,2)  NULL,
    [ThumImg] nvarchar(1000)  NULL,
    [CarouselImgs] nvarchar(4000)  NULL,
    [JdPrice] decimal(18,2)  NULL,
    [JdCostPrice] decimal(18,2)  NULL,
    [JdStatus] int  NOT NULL,
    [JdAuditCommodityId] uniqueidentifier  NOT NULL,
    [AuditType] int  NOT NULL
);
GO

-- Creating table 'YJEmployee'
CREATE TABLE [dbo].[YJEmployee] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [UserAccount] nvarchar(20)  NOT NULL,
    [UserName] nvarchar(50)  NULL,
    [IdentityNum] nvarchar(50)  NULL,
    [Phone] nvarchar(20)  NULL,
    [Area] nvarchar(50)  NULL,
    [StationCode] nvarchar(50)  NULL,
    [StationName] nvarchar(50)  NULL,
    [IsDel] int  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubOn] datetime  NOT NULL,
    [ModifiedId] uniqueidentifier  NULL,
    [ModifiedOn] datetime  NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [IsManager] int  NULL,
    [Department] nvarchar(300)  NULL,
    [Station] nvarchar(300)  NULL,
    [UserCode] nvarchar(30)  NULL
);
GO

-- Creating table 'Specifications'
CREATE TABLE [dbo].[Specifications] (
    [Id] uniqueidentifier  NOT NULL,
    [Attribute] int  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [IsDel] bit  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'CommoditySpecifications'
CREATE TABLE [dbo].[CommoditySpecifications] (
    [Id] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [Attribute] int  NULL,
    [SubTime] datetime  NOT NULL,
    [IsDel] bit  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'JdCommodity'
CREATE TABLE [dbo].[JdCommodity] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [No_Number] int  NULL,
    [Price] decimal(18,2)  NULL,
    [Stock] int  NOT NULL,
    [PicturesPath] nvarchar(4000)  NULL,
    [Description] nvarchar(max)  NULL,
    [State] int  NOT NULL,
    [IsDel] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [No_Code] nvarchar(512)  NOT NULL,
    [TotalCollection] int  NULL,
    [TotalReview] int  NULL,
    [Salesvolume] int  NULL,
    [ModifiedOn] datetime  NULL,
    [GroundTime] datetime  NULL,
    [ComAttribute] nvarchar(max)  NULL,
    [CategoryName] nvarchar(max)  NULL,
    [SortValue] int  NULL,
    [FreightTemplateId] uniqueidentifier  NULL,
    [MarketPrice] decimal(18,2)  NULL,
    [IsEnableSelfTake] int  NULL,
    [Weight] decimal(18,5)  NULL,
    [PricingMethod] tinyint  NULL,
    [SaleAreas] nvarchar(max)  NULL,
    [SharePercent] decimal(18,5)  NULL,
    [CommodityType] int  NULL,
    [HtmlVideoPath] nvarchar(3000)  NULL,
    [MobileVideoPath] nvarchar(3000)  NULL,
    [VideoPic] nvarchar(3000)  NULL,
    [VideoName] nvarchar(512)  NULL,
    [ScorePercent] decimal(18,5)  NULL,
    [Duty] decimal(18,2)  NULL,
    [SpreadPercent] decimal(18,5)  NULL,
    [ScoreScale] decimal(18,5)  NULL,
    [TaxRate] decimal(18,2)  NULL,
    [TaxClassCode] nvarchar(64)  NULL,
    [Unit] nvarchar(max)  NULL,
    [InputRax] decimal(18,2)  NULL,
    [Barcode] nvarchar(128)  NULL,
    [JDCode] nvarchar(128)  NULL,
    [CostPrice] decimal(18,2)  NULL,
    [IsAssurance] bit  NULL,
    [TechSpecs] nvarchar(max)  NULL,
    [SaleService] nvarchar(max)  NULL,
    [IsReturns] bit  NULL,
    [ServiceSettingId] nvarchar(400)  NULL,
    [Type] int  NULL,
    [YJCouponActivityId] nvarchar(32)  NULL,
    [YJCouponType] nvarchar(10)  NULL,
    [ModifieId] uniqueidentifier  NULL,
    [Isnsupport] bit  NULL,
    [ErQiCode] nvarchar(50)  NULL
);
GO

-- Creating table 'JDStockJournal'
CREATE TABLE [dbo].[JDStockJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [CommodityStockId] uniqueidentifier  NOT NULL,
    [CommodityErQiCode] nvarchar(50)  NULL,
    [CommodityStockErQiCode] nvarchar(50)  NULL,
    [CommodityOldStock] int  NOT NULL,
    [CommodityNewStock] int  NOT NULL,
    [CommodityStockOldStock] int  NOT NULL,
    [CommodityStockNewStock] int  NOT NULL,
    [Code] nvarchar(50)  NOT NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'JDEclpOrder'
CREATE TABLE [dbo].[JDEclpOrder] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [EclpOrderNo] nvarchar(50)  NOT NULL,
    [EclpOrderState] int  NOT NULL,
    [EclpOrderStateName] nvarchar(50)  NOT NULL,
    [OrderSubTime] datetime  NOT NULL,
    [PayTime] datetime  NOT NULL,
    [CancelTime] datetime  NULL,
    [ShipTime] datetime  NULL,
    [ReceiveTime] datetime  NULL,
    [RejectTime] datetime  NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(512)  NULL,
    [AppType] smallint  NULL,
    [SupplierName] nvarchar(512)  NULL,
    [SupplierCode] nvarchar(128)  NULL,
    [SupplierType] smallint  NULL,
    [ShipperType] smallint  NULL
);
GO

-- Creating table 'JDEclpOrderJournal'
CREATE TABLE [dbo].[JDEclpOrderJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [EclpOrderNo] nvarchar(50)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Details] nvarchar(max)  NULL,
    [StateFrom] int  NULL,
    [StateTo] int  NOT NULL,
    [Code] nvarchar(50)  NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'JDEclpOrderRefundAfterSales'
CREATE TABLE [dbo].[JDEclpOrderRefundAfterSales] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [OrderRefundAfterSalesId] uniqueidentifier  NOT NULL,
    [EclpOrderNo] nvarchar(50)  NOT NULL,
    [EclpServicesNo] nvarchar(50)  NOT NULL,
    [EclpServicesState] int  NOT NULL,
    [EclpServicesStateName] nvarchar(50)  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [CustomerContactName] nvarchar(50)  NULL,
    [CustomerTel] nvarchar(50)  NULL,
    [PickwareAddress] nvarchar(256)  NULL,
    [PickwareType] int  NOT NULL
);
GO

-- Creating table 'JDEclpOrderRefundAfterSalesItem'
CREATE TABLE [dbo].[JDEclpOrderRefundAfterSalesItem] (
    [Id] uniqueidentifier  NOT NULL,
    [JDEclpOrderRefundAfterSalesId] uniqueidentifier  NOT NULL,
    [IsvGoodsNo] nvarchar(50)  NOT NULL,
    [SpareCode] nvarchar(50)  NOT NULL,
    [PartReceiveType] int  NOT NULL,
    [GoodsStatus] int  NOT NULL,
    [WareType] int  NOT NULL,
    [ApproveNotes] nvarchar(200)  NOT NULL
);
GO

-- Creating table 'JDEclpOrderRefundAfterSalesJournal'
CREATE TABLE [dbo].[JDEclpOrderRefundAfterSalesJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [OrderRefundAfterSalesId] uniqueidentifier  NOT NULL,
    [EclpOrderNo] nvarchar(50)  NOT NULL,
    [EclpServicesNo] nvarchar(50)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Details] nvarchar(max)  NULL,
    [StateFrom] int  NULL,
    [StateTo] int  NOT NULL,
    [JDEclpOrderRefundAfterSalesId] uniqueidentifier  NOT NULL,
    [WarehouseName] nvarchar(50)  NULL,
    [WarehouseNo] nvarchar(50)  NULL,
    [Code] nvarchar(50)  NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'UserCodeForHaiXin'
CREATE TABLE [dbo].[UserCodeForHaiXin] (
    [UserId] uniqueidentifier  NOT NULL,
    [UserCode] nvarchar(10)  NOT NULL
);
GO

-- Creating table 'HaiXinMqJournal'
CREATE TABLE [dbo].[HaiXinMqJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [Source] nvarchar(50)  NOT NULL,
    [Result] nvarchar(50)  NOT NULL,
    [Message] nvarchar(max)  NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'RefundExpressTrace'
CREATE TABLE [dbo].[RefundExpressTrace] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [RefundExpOrderNo] nvarchar(max)  NULL,
    [UploadExpOrderTime] datetime  NULL,
    [ExpressDeliveryTime] datetime  NULL,
    [ExpressSDTime] datetime  NULL,
    [RefundExpCo] nvarchar(200)  NULL,
    [AgreeRefundTime] datetime  NULL,
    [OrderItemId] uniqueidentifier  NULL
);
GO

-- Creating table 'FreightRangeDetails'
CREATE TABLE [dbo].[FreightRangeDetails] (
    [Id] uniqueidentifier  NOT NULL,
    [TemplateId] uniqueidentifier  NOT NULL,
    [Min] decimal(18,2)  NOT NULL,
    [Max] decimal(18,2)  NOT NULL,
    [Cost] decimal(18,2)  NOT NULL,
    [IsSpecific] bit  NOT NULL,
    [ProvinceCodes] varchar(1000)  NULL
);
GO

-- Creating table 'YKBigDataMqJournal'
CREATE TABLE [dbo].[YKBigDataMqJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [Source] nvarchar(50)  NOT NULL,
    [Result] nvarchar(50)  NOT NULL,
    [Message] nvarchar(max)  NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'YXOrder'
CREATE TABLE [dbo].[YXOrder] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [OrderState] int  NOT NULL,
    [OrderStateName] nvarchar(50)  NOT NULL,
    [OrderSubTime] datetime  NOT NULL,
    [PayTime] datetime  NOT NULL,
    [CancelTime] datetime  NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(512)  NULL,
    [AppType] smallint  NULL,
    [SupplierName] nvarchar(512)  NULL,
    [SupplierCode] nvarchar(128)  NULL,
    [SupplierType] smallint  NULL,
    [ShipperType] smallint  NULL
);
GO

-- Creating table 'YXOrderJournal'
CREATE TABLE [dbo].[YXOrderJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Details] nvarchar(max)  NULL,
    [StateFrom] int  NULL,
    [StateTo] int  NOT NULL,
    [Code] nvarchar(50)  NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'YXOrderPackage'
CREATE TABLE [dbo].[YXOrderPackage] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [PackageId] nvarchar(128)  NOT NULL,
    [ExpressCompany] nvarchar(max)  NOT NULL,
    [ExpressNo] nvarchar(max)  NOT NULL,
    [ExpCreateTime] datetime  NULL,
    [ConfirmTime] datetime  NULL,
    [PackageState] int  NOT NULL,
    [PackageStateName] nvarchar(50)  NOT NULL,
    [SubExpressNos] nvarchar(1024)  NULL
);
GO

-- Creating table 'YXOrderSku'
CREATE TABLE [dbo].[YXOrderSku] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [CommodityStockId] uniqueidentifier  NOT NULL,
    [PackageId] nvarchar(128)  NOT NULL,
    [SkuId] nvarchar(32)  NOT NULL,
    [ProductName] nvarchar(512)  NOT NULL,
    [SaleCount] int  NOT NULL,
    [OriginPrice] decimal(18,2)  NOT NULL,
    [SubtotalAmount] decimal(18,2)  NOT NULL,
    [CouponTotalAmount] decimal(18,2)  NOT NULL,
    [ActivityTotalAmount] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'YXExpressDetailInfo'
CREATE TABLE [dbo].[YXExpressDetailInfo] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [PackageId] nvarchar(128)  NOT NULL,
    [ExpressCompany] nvarchar(max)  NOT NULL,
    [ExpressNo] nvarchar(max)  NOT NULL,
    [SubExpressNos] nvarchar(1024)  NULL
);
GO

-- Creating table 'YXExpressDetailInfoSku'
CREATE TABLE [dbo].[YXExpressDetailInfoSku] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [CommodityStockId] uniqueidentifier  NOT NULL,
    [PackageId] nvarchar(128)  NOT NULL,
    [SkuId] nvarchar(32)  NOT NULL,
    [ProductName] nvarchar(512)  NOT NULL,
    [SaleCount] int  NOT NULL,
    [OriginPrice] decimal(18,2)  NOT NULL,
    [SubtotalAmount] decimal(18,2)  NOT NULL,
    [CouponTotalAmount] decimal(18,2)  NOT NULL,
    [ActivityTotalAmount] decimal(18,2)  NOT NULL,
    [YXExpressDetailInfoId] uniqueidentifier  NOT NULL,
    [ExpressCompany] nvarchar(max)  NOT NULL,
    [ExpressNo] nvarchar(max)  NOT NULL,
    [SubExpressNos] nvarchar(1024)  NULL
);
GO

-- Creating table 'YXComInfo'
CREATE TABLE [dbo].[YXComInfo] (
    [Id] uniqueidentifier  NOT NULL,
    [SPU] nvarchar(max)  NOT NULL,
    [SKU] nvarchar(max)  NULL,
    [Price] decimal(18,2)  NULL,
    [CostPrice] decimal(18,0)  NULL,
    [Discount] decimal(18,0)  NULL,
    [SubTime] datetime  NOT NULL
);
GO

-- Creating table 'YXOrderPackageJournal'
CREATE TABLE [dbo].[YXOrderPackageJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [PackageId] nvarchar(128)  NOT NULL,
    [ExpCreateTime] datetime  NULL,
    [ConfirmTime] datetime  NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Details] nvarchar(max)  NULL,
    [StateFrom] int  NULL,
    [StateTo] int  NOT NULL,
    [Code] nvarchar(50)  NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'YXOrderErrorLog'
CREATE TABLE [dbo].[YXOrderErrorLog] (
    [Id] uniqueidentifier  NOT NULL,
    [Content] nvarchar(max)  NULL,
    [SubTime] datetime  NOT NULL,
    [Isdisable] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [AppName] nvarchar(512)  NULL,
    [CommodityNames] nvarchar(512)  NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'YXExpressTrace'
CREATE TABLE [dbo].[YXExpressTrace] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [PackageId] nvarchar(128)  NOT NULL,
    [ExpressCompany] nvarchar(max)  NOT NULL,
    [ExpressNo] nvarchar(max)  NOT NULL,
    [Time] datetime  NOT NULL,
    [Desc] nvarchar(max)  NULL,
    [SubTime] datetime  NOT NULL
);
GO

-- Creating table 'AppMetaData'
CREATE TABLE [dbo].[AppMetaData] (
    [Id] uniqueidentifier  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [Key] nvarchar(128)  NOT NULL,
    [Value] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'CommodityPriceFloat'
CREATE TABLE [dbo].[CommodityPriceFloat] (
    [Id] uniqueidentifier  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [AppIds] nvarchar(1024)  NULL,
    [FloatPrice] decimal(18,2)  NOT NULL,
    [IsDel] bit  NOT NULL
);
GO

-- Creating table 'CategoryAdvertise'
CREATE TABLE [dbo].[CategoryAdvertise] (
    [Id] uniqueidentifier  NOT NULL,
    [AdvertiseName] nvarchar(max)  NOT NULL,
    [PutTime] datetime  NOT NULL,
    [PushTime] datetime  NOT NULL,
    [CategoryId] uniqueidentifier  NOT NULL,
    [State] int  NOT NULL,
    [spreadEnum] int  NOT NULL,
    [FreeUrl] nvarchar(max)  NOT NULL,
    [AdvertiseImg] nvarchar(max)  NOT NULL,
    [AdvertiseMedia] nvarchar(max)  NOT NULL,
    [AdvertiseType] int  NOT NULL,
    [AdverID] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ModifiedId] uniqueidentifier  NOT NULL,
    [UserService] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'CommodityInnerBrand'
CREATE TABLE [dbo].[CommodityInnerBrand] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(512)  NOT NULL,
    [Code] nvarchar(128)  NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [BrandId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [MaxSort] float  NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CrcAppId] bigint  NOT NULL,
    [IsDel] bit  NULL
);
GO

-- Creating table 'Brandwall'
CREATE TABLE [dbo].[Brandwall] (
    [Id] uniqueidentifier  NOT NULL,
    [Brandname] nvarchar(max)  NOT NULL,
    [Brandstatu] int  NOT NULL,
    [BrandLogo] nvarchar(max)  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'CategoryInnerBrand'
CREATE TABLE [dbo].[CategoryInnerBrand] (
    [Id] uniqueidentifier  NOT NULL,
    [Brandname] nvarchar(max)  NOT NULL,
    [BrandId] uniqueidentifier  NOT NULL,
    [CategoryId] uniqueidentifier  NOT NULL,
    [CategoryName] nvarchar(max)  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'CommodityOrderRefund'
CREATE TABLE [dbo].[CommodityOrderRefund] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [ModifiedOn] datetime  NULL,
    [ModifiedBy] nvarchar(max)  NULL,
    [CommodityOrderId] uniqueidentifier  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [RefundDate] datetime  NOT NULL,
    [RefundType] int  NOT NULL,
    [RefundAmount] decimal(18,2)  NOT NULL,
    [Remark] nvarchar(150)  NULL
);
GO

-- Creating table 'SNOrderItem'
CREATE TABLE [dbo].[SNOrderItem] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(150)  NULL,
    [CustomOrderId] nvarchar(200)  NOT NULL,
    [CustomOrderItemId] nvarchar(200)  NOT NULL,
    [CustomSkuId] nvarchar(200)  NULL,
    [SubTime] datetime  NOT NULL,
    [Status] int  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [DeliveryType] int  NULL,
    [ExpressStatus] int  NOT NULL
);
GO

-- Creating table 'SNExpressTrace'
CREATE TABLE [dbo].[SNExpressTrace] (
    [Id] uniqueidentifier  NOT NULL,
    [OperateState] nvarchar(max)  NOT NULL,
    [OperateTime] datetime  NULL,
    [OrderId] nvarchar(200)  NOT NULL,
    [PackageId] nvarchar(30)  NOT NULL
);
GO

-- Creating table 'SNPackageTrace'
CREATE TABLE [dbo].[SNPackageTrace] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] nvarchar(200)  NOT NULL,
    [OrderItemId] nvarchar(200)  NOT NULL,
    [SkuId] nvarchar(200)  NOT NULL,
    [PackageId] nvarchar(30)  NOT NULL,
    [ReceiveTime] datetime  NULL,
    [ShippingTime] datetime  NULL,
    [CommodityOrderId] uniqueidentifier  NULL
);
GO

-- Creating table 'SNOrderRefundAfterSales'
CREATE TABLE [dbo].[SNOrderRefundAfterSales] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [OrderRefundAfterSalesId] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [CustomOrderId] nvarchar(128)  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [CustomSkuId] nvarchar(128)  NOT NULL,
    [AfsServiceId] nvarchar(128)  NULL,
    [AfsServiceStep] int  NOT NULL,
    [AfsServiceStepName] nvarchar(128)  NULL,
    [Cancel] smallint  NOT NULL,
    [PickwareType] int  NOT NULL,
    [CustomerContactName] nvarchar(128)  NULL,
    [CustomerTel] nvarchar(128)  NULL,
    [PickwareAddress] nvarchar(256)  NULL,
    [CommodityNum] int  NULL
);
GO

-- Creating table 'ThirdECommerce'
CREATE TABLE [dbo].[ThirdECommerce] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [OpenApiCallerId] nvarchar(50)  NOT NULL,
    [OpenApiKey] nvarchar(50)  NOT NULL,
    [OrderCreateUrl] nvarchar(500)  NOT NULL,
    [OrderCancelUrl] nvarchar(500)  NOT NULL,
    [ServiceCreateUrl] nvarchar(500)  NOT NULL,
    [ServiceCancelUrl] nvarchar(500)  NOT NULL,
    [Type] int  NOT NULL
);
GO

-- Creating table 'ThirdECOrder'
CREATE TABLE [dbo].[ThirdECOrder] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [OrderSubTime] datetime  NOT NULL,
    [PayTime] datetime  NOT NULL,
    [CancelApplyTime] datetime  NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(512)  NULL,
    [AppType] smallint  NULL,
    [SupplierName] nvarchar(512)  NULL,
    [SupplierCode] nvarchar(128)  NULL,
    [SupplierType] smallint  NULL,
    [ShipperType] smallint  NULL,
    [StateName] nvarchar(50)  NOT NULL,
    [CancelResultTime] datetime  NULL,
    [CancelCallBackTime] datetime  NULL,
    [StateDesc] nvarchar(max)  NULL
);
GO

-- Creating table 'ThirdECOrderJournal'
CREATE TABLE [dbo].[ThirdECOrderJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Details] nvarchar(max)  NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'ThirdECOrderPackage'
CREATE TABLE [dbo].[ThirdECOrderPackage] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ExpressCompany] nvarchar(max)  NOT NULL,
    [ExpressNo] nvarchar(max)  NOT NULL,
    [ExpCreateTime] datetime  NULL,
    [ConfirmTime] datetime  NULL
);
GO

-- Creating table 'ThirdECOrderPackageSku'
CREATE TABLE [dbo].[ThirdECOrderPackageSku] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [CommodityStockId] uniqueidentifier  NOT NULL,
    [PackageId] uniqueidentifier  NOT NULL,
    [SkuId] nvarchar(32)  NOT NULL,
    [ExpressCompany] nvarchar(max)  NOT NULL,
    [ExpressNo] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ThirdECOrderErrorLog'
CREATE TABLE [dbo].[ThirdECOrderErrorLog] (
    [Id] uniqueidentifier  NOT NULL,
    [Content] nvarchar(max)  NULL,
    [SubTime] datetime  NOT NULL,
    [Isdisable] bit  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [AppName] nvarchar(512)  NULL,
    [CommodityNames] nvarchar(512)  NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'ThirdECExpressTrace'
CREATE TABLE [dbo].[ThirdECExpressTrace] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [PackageId] uniqueidentifier  NOT NULL,
    [ExpressCompany] nvarchar(max)  NOT NULL,
    [ExpressNo] nvarchar(max)  NOT NULL,
    [Time] datetime  NOT NULL,
    [Desc] nvarchar(max)  NULL
);
GO

-- Creating table 'ThirdECStockJournal'
CREATE TABLE [dbo].[ThirdECStockJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [CommodityStockId] uniqueidentifier  NOT NULL,
    [CommoditySkuId] nvarchar(50)  NULL,
    [CommodityStockSkuId] nvarchar(50)  NULL,
    [CommodityOldStock] int  NOT NULL,
    [CommodityNewStock] int  NOT NULL,
    [CommodityStockOldStock] int  NOT NULL,
    [CommodityStockNewStock] int  NOT NULL,
    [Code] nvarchar(50)  NOT NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'ThirdECService'
CREATE TABLE [dbo].[ThirdECService] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [OrderRefundAfterSalesId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SkuId] nvarchar(32)  NOT NULL,
    [Number] int  NOT NULL,
    [StateName] nvarchar(50)  NOT NULL,
    [UserCancelTime] datetime  NULL,
    [RejectApplyTime] datetime  NULL,
    [AgreeApplyTime] datetime  NULL,
    [UserMailTime] datetime  NULL,
    [MailNotReceiveTime] datetime  NULL,
    [MailReceiveTime] datetime  NULL,
    [RejectRefundTime] datetime  NULL,
    [AgreeRefundTime] datetime  NULL,
    [StateDesc] nvarchar(max)  NULL
);
GO

-- Creating table 'ThirdECServiceJournal'
CREATE TABLE [dbo].[ThirdECServiceJournal] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [OrderRefundAfterSalesId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Details] nvarchar(max)  NULL,
    [Json] nvarchar(max)  NULL
);
GO

-- Creating table 'YJEmTemp'
CREATE TABLE [dbo].[YJEmTemp] (
    [id] int  NOT NULL,
    [stationcode] nvarchar(512)  NOT NULL,
    [station] nvarchar(max)  NULL,
    [UserAccount] nvarchar(30)  NULL,
    [UserCode] nvarchar(30)  NULL
);
GO

-- Creating table 'YJBDSFOrderInfo'
CREATE TABLE [dbo].[YJBDSFOrderInfo] (
    [Id] uniqueidentifier  NOT NULL,
    [PlatformName] nvarchar(max)  NULL,
    [OrderPayDate] datetime  NULL,
    [OrderNo] nvarchar(max)  NULL,
    [OrderPayState] nvarchar(max)  NULL,
    [OrderPayMoney] decimal(18,2)  NULL,
    [Commodity] nvarchar(max)  NULL,
    [UserID] uniqueidentifier  NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [CollectGoodsName] nvarchar(max)  NOT NULL,
    [CollectGoodsPhone] nvarchar(max)  NOT NULL,
    [Freight] decimal(18,2)  NOT NULL,
    [PlaceOrderName] nvarchar(max)  NOT NULL,
    [PlaceOrderPhone] nvarchar(max)  NOT NULL,
    [Status] int  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'YJBCarInsuranceRebate'
CREATE TABLE [dbo].[YJBCarInsuranceRebate] (
    [Id] uniqueidentifier  NOT NULL,
    [RebateDate] datetime  NULL,
    [RebateNum] int  NULL,
    [RebateMoney] decimal(18,2)  NULL,
    [OrderNo] nvarchar(max)  NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [RemittanceNo] nvarchar(max)  NULL,
    [PhoneNum] nvarchar(max)  NULL,
    [RebateState] int  NULL,
    [AuditDate] datetime  NULL,
    [InsuranceAmount] decimal(18,2)  NULL,
    [AfterTaxMoney] decimal(18,2)  NOT NULL,
    [AuditFlag] int  NULL,
    [DouRebateMoney] decimal(18,2)  NOT NULL,
    [CompanyRebateMoney] decimal(18,2)  NOT NULL,
    [DouRebatePersent] decimal(18,2)  NOT NULL,
    [CompanyRebatePersent] decimal(18,2)  NOT NULL,
    [InsuranceCompanyCode] nvarchar(max)  NOT NULL,
    [BusinessInsuranceAmount] decimal(18,2)  NOT NULL,
    [StrongInsuranceAmount] decimal(18,2)  NOT NULL,
    [CarShipAmount] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'YJBCarInsuranceReport'
CREATE TABLE [dbo].[YJBCarInsuranceReport] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderNo] nvarchar(max)  NULL,
    [MemberPhone] nvarchar(max)  NULL,
    [CustomPhone] nvarchar(max)  NULL,
    [InsuranceAmount] decimal(18,2)  NULL,
    [DetailId] uniqueidentifier  NULL,
    [InsuranceTime] nvarchar(max)  NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [State] nvarchar(max)  NULL,
    [RecommendName] nvarchar(max)  NULL,
    [RecommendAmount] decimal(18,2)  NULL,
    [CustomAmount] decimal(18,2)  NULL,
    [SinopecAmount] decimal(18,2)  NULL,
    [RebateState] smallint  NULL,
    [InsuranceCompanyCode] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'YJBCarInsReportDetail'
CREATE TABLE [dbo].[YJBCarInsReportDetail] (
    [Id] uniqueidentifier  NOT NULL,
    [StrongInsuranceAmount] decimal(18,2)  NULL,
    [BusinessAmount] decimal(18,2)  NULL,
    [BusinessFreeAmount] decimal(18,2)  NULL,
    [StrongInsuranceOrderId] nvarchar(max)  NULL,
    [StrongInsuranceStartTime] nvarchar(max)  NULL,
    [BusinessOrderId] nvarchar(max)  NULL,
    [BusinessStartTime] nvarchar(max)  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [PlateNumber] nvarchar(max)  NULL,
    [ChassisNumber] nvarchar(max)  NULL,
    [EngineNumber] nvarchar(max)  NULL,
    [CarTypeName] nvarchar(max)  NULL,
    [RegisterTime] nvarchar(max)  NULL,
    [IsTransfer] nvarchar(max)  NULL,
    [CarOwnerName] nvarchar(max)  NULL,
    [CarOwnerIdType] nvarchar(max)  NULL,
    [CarOwnerId] nvarchar(max)  NULL,
    [CarOwnerAddress] nvarchar(max)  NULL,
    [CarOwnerPhone] nvarchar(max)  NULL,
    [PolicyHolderName] nvarchar(max)  NULL,
    [PolicyHolderIdType] nvarchar(max)  NULL,
    [PolicyHolderId] nvarchar(max)  NULL,
    [PolicyHolderPhone] nvarchar(max)  NULL,
    [PolicyHolderAddress] nvarchar(max)  NULL,
    [StrongInsurance_SI] nvarchar(max)  NULL,
    [StrongInsurance_Car] nvarchar(max)  NULL,
    [Business_Three] nvarchar(max)  NULL,
    [Business_Driver] nvarchar(max)  NULL,
    [Business_Passenger] nvarchar(max)  NULL,
    [Business_AllCar] nvarchar(max)  NULL,
    [Business_Glass] nvarchar(max)  NULL,
    [Business_Body] nvarchar(max)  NULL,
    [Business_Engine] nvarchar(max)  NULL,
    [Business_Natural] nvarchar(max)  NULL,
    [Business_Garage] nvarchar(max)  NULL,
    [Business_Third] nvarchar(max)  NULL,
    [Business_Spirit] nvarchar(max)  NULL,
    [NoDeductibles_Car] nvarchar(max)  NULL,
    [Business_Car] nvarchar(max)  NULL,
    [NoDeductibles_Three] nvarchar(max)  NULL,
    [NoDeductibles_Driver] nvarchar(max)  NULL,
    [NoDeductibles_Passenger] nvarchar(max)  NULL,
    [NoDeductibles_AllCar] nvarchar(max)  NULL,
    [NoDeductibles_Body] nvarchar(max)  NULL,
    [NoDeductibles_Engine] nvarchar(max)  NULL,
    [NoDeductibles_Natural] nvarchar(max)  NULL,
    [NoDeductibles__Spirit] nvarchar(max)  NULL,
    [BusinessEndTime] nvarchar(max)  NOT NULL,
    [StrongInsuranceEndTime] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SNOrder'
CREATE TABLE [dbo].[SNOrder] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [CustomOrderId] nvarchar(50)  NOT NULL,
    [RequestFee] decimal(18,2)  NOT NULL,
    [ResponseFee] decimal(18,2)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'CouponRefundDetail'
CREATE TABLE [dbo].[CouponRefundDetail] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [RefundTime] datetime  NULL,
    [ReceiveAccount] nvarchar(128)  NULL,
    [ReceiveName] nvarchar(max)  NULL,
    [CommodityCouponMoney] decimal(18,2)  NULL,
    [FreightCouponMoney] decimal(18,2)  NULL,
    [CommodityRefundMoney] decimal(18,2)  NULL,
    [FreightRefundMoney] decimal(18,2)  NULL,
    [RefundTotalMoney] decimal(18,2)  NULL,
    [ShopName] nvarchar(max)  NULL,
    [OrderNo] nvarchar(max)  NULL,
    [CommoidtyName] nvarchar(max)  NULL,
    [ReceivePhone] nvarchar(11)  NULL,
    [ConsigneeName] nvarchar(max)  NULL,
    [Remark] nvarchar(max)  NULL
);
GO

-- Creating table 'FangZhengOrder'
CREATE TABLE [dbo].[FangZhengOrder] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderItemId] uniqueidentifier  NOT NULL,
    [OrderStatus] int  NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'FangZhengLogistics'
CREATE TABLE [dbo].[FangZhengLogistics] (
    [Id] uniqueidentifier  NOT NULL,
    [LogisticsCompany] nvarchar(50)  NOT NULL,
    [LogisticsOrderId] nvarchar(50)  NOT NULL,
    [StatusName] nvarchar(10)  NOT NULL,
    [StatusCode] nvarchar(10)  NOT NULL,
    [DataInfo] nvarchar(1000)  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'ThirdMapStatus'
CREATE TABLE [dbo].[ThirdMapStatus] (
    [Id] uniqueidentifier  NOT NULL,
    [Status] int  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [StatusName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ThirdMerchant'
CREATE TABLE [dbo].[ThirdMerchant] (
    [Id] uniqueidentifier  NOT NULL,
    [Secret] nvarchar(max)  NOT NULL,
    [MerchantName] nvarchar(max)  NOT NULL,
    [IsDisable] bit  NOT NULL,
    [MobileUrl] nvarchar(max)  NOT NULL,
    [ComputerUrl] nvarchar(max)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL
);
GO

-- Creating table 'SNBill'
CREATE TABLE [dbo].[SNBill] (
    [ID] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NULL,
    [CustomOrderId] nvarchar(32)  NULL,
    [SNOrderTotalMoney] decimal(18,2)  NULL,
    [SNFreight] decimal(18,2)  NULL,
    [SNSubtime] datetime  NULL,
    [IsHandle] int  NULL,
    [YjOrderId] uniqueidentifier  NULL,
    [OrderCode] nvarchar(50)  NULL,
    [State] int  NULL,
    [OrderDetail] nvarchar(2000)  NULL,
    [YjOrderTotal] decimal(18,2)  NULL,
    [YJFreight] decimal(18,2)  NULL,
    [IsEqualMoney] nvarchar(20)  NULL,
    [IsEqualFreight] nvarchar(20)  NULL,
    [SNState] nvarchar(100)  NULL,
    [SnWuLiuState] nvarchar(100)  NULL
);
GO

-- Creating table 'InsuranceCompanyActivity'
CREATE TABLE [dbo].[InsuranceCompanyActivity] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [CompanyId] uniqueidentifier  NOT NULL,
    [BeginTime] nvarchar(max)  NOT NULL,
    [EndTime] nvarchar(max)  NOT NULL,
    [IsAvailable] int  NOT NULL,
    [BusinessRate] decimal(18,2)  NOT NULL,
    [StrongRate] decimal(18,2)  NOT NULL,
    [CarShipRate] decimal(18,2)  NOT NULL,
    [TaxRate] decimal(18,2)  NOT NULL,
    [InsuranceCompanyCode] nvarchar(2)  NOT NULL
);
GO

-- Creating table 'InsuranceCompany'
CREATE TABLE [dbo].[InsuranceCompany] (
    [Id] uniqueidentifier  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubId] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [PicUrl] nvarchar(max)  NOT NULL,
    [Status] int  NOT NULL,
    [ContactName] nvarchar(max)  NOT NULL,
    [ContactPhone] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [InsuranceCompanyCode] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Category'
ALTER TABLE [dbo].[Category]
ADD CONSTRAINT [PK_Category]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Commodity'
ALTER TABLE [dbo].[Commodity]
ADD CONSTRAINT [PK_Commodity]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SecondAttribute'
ALTER TABLE [dbo].[SecondAttribute]
ADD CONSTRAINT [PK_SecondAttribute]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProductDetailsPicture'
ALTER TABLE [dbo].[ProductDetailsPicture]
ADD CONSTRAINT [PK_ProductDetailsPicture]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Attribute'
ALTER TABLE [dbo].[Attribute]
ADD CONSTRAINT [PK_Attribute]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Promotion'
ALTER TABLE [dbo].[Promotion]
ADD CONSTRAINT [PK_Promotion]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PromotionItems'
ALTER TABLE [dbo].[PromotionItems]
ADD CONSTRAINT [PK_PromotionItems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityOrder'
ALTER TABLE [dbo].[CommodityOrder]
ADD CONSTRAINT [PK_CommodityOrder]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderItem'
ALTER TABLE [dbo].[OrderItem]
ADD CONSTRAINT [PK_OrderItem]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Message'
ALTER TABLE [dbo].[Message]
ADD CONSTRAINT [PK_Message]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ShoppingCartItems'
ALTER TABLE [dbo].[ShoppingCartItems]
ADD CONSTRAINT [PK_ShoppingCartItems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Review'
ALTER TABLE [dbo].[Review]
ADD CONSTRAINT [PK_Review]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Store'
ALTER TABLE [dbo].[Store]
ADD CONSTRAINT [PK_Store]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DeliveryAddress'
ALTER TABLE [dbo].[DeliveryAddress]
ADD CONSTRAINT [PK_DeliveryAddress]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Collection'
ALTER TABLE [dbo].[Collection]
ADD CONSTRAINT [PK_Collection]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ComAttibute'
ALTER TABLE [dbo].[ComAttibute]
ADD CONSTRAINT [PK_ComAttibute]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityUser'
ALTER TABLE [dbo].[CommodityUser]
ADD CONSTRAINT [PK_CommodityUser]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityCategory'
ALTER TABLE [dbo].[CommodityCategory]
ADD CONSTRAINT [PK_CommodityCategory]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Reply'
ALTER TABLE [dbo].[Reply]
ADD CONSTRAINT [PK_Reply]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Journal'
ALTER TABLE [dbo].[Journal]
ADD CONSTRAINT [PK_Journal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Payments'
ALTER TABLE [dbo].[Payments]
ADD CONSTRAINT [PK_Payments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AllPayment'
ALTER TABLE [dbo].[AllPayment]
ADD CONSTRAINT [PK_AllPayment]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'HotCommodity'
ALTER TABLE [dbo].[HotCommodity]
ADD CONSTRAINT [PK_HotCommodity]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TodayPromotion'
ALTER TABLE [dbo].[TodayPromotion]
ADD CONSTRAINT [PK_TodayPromotion]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GenUserPrizeRecord'
ALTER TABLE [dbo].[GenUserPrizeRecord]
ADD CONSTRAINT [PK_GenUserPrizeRecord]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderRefund'
ALTER TABLE [dbo].[OrderRefund]
ADD CONSTRAINT [PK_OrderRefund]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityStock'
ALTER TABLE [dbo].[CommodityStock]
ADD CONSTRAINT [PK_CommodityStock]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FreightTemplate'
ALTER TABLE [dbo].[FreightTemplate]
ADD CONSTRAINT [PK_FreightTemplate]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FreightTemplateDetail'
ALTER TABLE [dbo].[FreightTemplateDetail]
ADD CONSTRAINT [PK_FreightTemplateDetail]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserLimited'
ALTER TABLE [dbo].[UserLimited]
ADD CONSTRAINT [PK_UserLimited]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderShareMess'
ALTER TABLE [dbo].[OrderShareMess]
ADD CONSTRAINT [PK_OrderShareMess]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Invoice'
ALTER TABLE [dbo].[Invoice]
ADD CONSTRAINT [PK_Invoice]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RelationCommodity'
ALTER TABLE [dbo].[RelationCommodity]
ADD CONSTRAINT [PK_RelationCommodity]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ShareDividend'
ALTER TABLE [dbo].[ShareDividend]
ADD CONSTRAINT [PK_ShareDividend]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ShareDividendDetail'
ALTER TABLE [dbo].[ShareDividendDetail]
ADD CONSTRAINT [PK_ShareDividendDetail]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserRedEnvelope'
ALTER TABLE [dbo].[UserRedEnvelope]
ADD CONSTRAINT [PK_UserRedEnvelope]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'APPManage'
ALTER TABLE [dbo].[APPManage]
ADD CONSTRAINT [PK_APPManage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RuleDescription'
ALTER TABLE [dbo].[RuleDescription]
ADD CONSTRAINT [PK_RuleDescription]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserCrowdfunding'
ALTER TABLE [dbo].[UserCrowdfunding]
ADD CONSTRAINT [PK_UserCrowdfunding]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CfDividend'
ALTER TABLE [dbo].[CfDividend]
ADD CONSTRAINT [PK_CfDividend]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CfOrderDividend'
ALTER TABLE [dbo].[CfOrderDividend]
ADD CONSTRAINT [PK_CfOrderDividend]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CfOrderDividendDetail'
ALTER TABLE [dbo].[CfOrderDividendDetail]
ADD CONSTRAINT [PK_CfOrderDividendDetail]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserCrowdfundingDaily'
ALTER TABLE [dbo].[UserCrowdfundingDaily]
ADD CONSTRAINT [PK_UserCrowdfundingDaily]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CrowdfundingDaily'
ALTER TABLE [dbo].[CrowdfundingDaily]
ADD CONSTRAINT [PK_CrowdfundingDaily]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CrowdfundingCount'
ALTER TABLE [dbo].[CrowdfundingCount]
ADD CONSTRAINT [PK_CrowdfundingCount]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Crowdfunding'
ALTER TABLE [dbo].[Crowdfunding]
ADD CONSTRAINT [PK_Crowdfunding]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CrowdfundingStatistics'
ALTER TABLE [dbo].[CrowdfundingStatistics]
ADD CONSTRAINT [PK_CrowdfundingStatistics]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AppSet'
ALTER TABLE [dbo].[AppSet]
ADD CONSTRAINT [PK_AppSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SetCategory'
ALTER TABLE [dbo].[SetCategory]
ADD CONSTRAINT [PK_SetCategory]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SetCommodityCategory'
ALTER TABLE [dbo].[SetCommodityCategory]
ADD CONSTRAINT [PK_SetCommodityCategory]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MainOrder'
ALTER TABLE [dbo].[MainOrder]
ADD CONSTRAINT [PK_MainOrder]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BehaviorRecord'
ALTER TABLE [dbo].[BehaviorRecord]
ADD CONSTRAINT [PK_BehaviorRecord]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderShipping'
ALTER TABLE [dbo].[OrderShipping]
ADD CONSTRAINT [PK_OrderShipping]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SetCollection'
ALTER TABLE [dbo].[SetCollection]
ADD CONSTRAINT [PK_SetCollection]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderExpirePay'
ALTER TABLE [dbo].[OrderExpirePay]
ADD CONSTRAINT [PK_OrderExpirePay]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserSpreader'
ALTER TABLE [dbo].[UserSpreader]
ADD CONSTRAINT [PK_UserSpreader]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderPayDetail'
ALTER TABLE [dbo].[OrderPayDetail]
ADD CONSTRAINT [PK_OrderPayDetail]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SettlingAccount'
ALTER TABLE [dbo].[SettlingAccount]
ADD CONSTRAINT [PK_SettlingAccount]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityOrderException'
ALTER TABLE [dbo].[CommodityOrderException]
ADD CONSTRAINT [PK_CommodityOrderException]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FreightPartialFree'
ALTER TABLE [dbo].[FreightPartialFree]
ADD CONSTRAINT [PK_FreightPartialFree]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SpreadCategory'
ALTER TABLE [dbo].[SpreadCategory]
ADD CONSTRAINT [PK_SpreadCategory]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SpreadInfo'
ALTER TABLE [dbo].[SpreadInfo]
ADD CONSTRAINT [PK_SpreadInfo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SelfTakeStation'
ALTER TABLE [dbo].[SelfTakeStation]
ADD CONSTRAINT [PK_SelfTakeStation]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SelfTakeStationManager'
ALTER TABLE [dbo].[SelfTakeStationManager]
ADD CONSTRAINT [PK_SelfTakeStationManager]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderPickUp'
ALTER TABLE [dbo].[OrderPickUp]
ADD CONSTRAINT [PK_OrderPickUp]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityJournal'
ALTER TABLE [dbo].[CommodityJournal]
ADD CONSTRAINT [PK_CommodityJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityOrderService'
ALTER TABLE [dbo].[CommodityOrderService]
ADD CONSTRAINT [PK_CommodityOrderService]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderRefundAfterSales'
ALTER TABLE [dbo].[OrderRefundAfterSales]
ADD CONSTRAINT [PK_OrderRefundAfterSales]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AppExtension'
ALTER TABLE [dbo].[AppExtension]
ADD CONSTRAINT [PK_AppExtension]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ErrorCommodityOrder'
ALTER TABLE [dbo].[ErrorCommodityOrder]
ADD CONSTRAINT [PK_ErrorCommodityOrder]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderExpressRoute'
ALTER TABLE [dbo].[OrderExpressRoute]
ADD CONSTRAINT [PK_OrderExpressRoute]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExpressTrace'
ALTER TABLE [dbo].[ExpressTrace]
ADD CONSTRAINT [PK_ExpressTrace]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExpressCode'
ALTER TABLE [dbo].[ExpressCode]
ADD CONSTRAINT [PK_ExpressCode]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderPayee'
ALTER TABLE [dbo].[OrderPayee]
ADD CONSTRAINT [PK_OrderPayee]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ScoreSetting'
ALTER TABLE [dbo].[ScoreSetting]
ADD CONSTRAINT [PK_ScoreSetting]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Distributor'
ALTER TABLE [dbo].[Distributor]
ADD CONSTRAINT [PK_Distributor]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityDistribution'
ALTER TABLE [dbo].[CommodityDistribution]
ADD CONSTRAINT [PK_CommodityDistribution]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityDistributionJounal'
ALTER TABLE [dbo].[CommodityDistributionJounal]
ADD CONSTRAINT [PK_CommodityDistributionJounal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderItemShare'
ALTER TABLE [dbo].[OrderItemShare]
ADD CONSTRAINT [PK_OrderItemShare]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderShare'
ALTER TABLE [dbo].[OrderShare]
ADD CONSTRAINT [PK_OrderShare]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DiyGroup'
ALTER TABLE [dbo].[DiyGroup]
ADD CONSTRAINT [PK_DiyGroup]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DiyGroupOrder'
ALTER TABLE [dbo].[DiyGroupOrder]
ADD CONSTRAINT [PK_DiyGroupOrder]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PaySource'
ALTER TABLE [dbo].[PaySource]
ADD CONSTRAINT [PK_PaySource]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VatInvoiceProof'
ALTER TABLE [dbo].[VatInvoiceProof]
ADD CONSTRAINT [PK_VatInvoiceProof]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'InvoiceJounal'
ALTER TABLE [dbo].[InvoiceJounal]
ADD CONSTRAINT [PK_InvoiceJounal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AppSelfTakeStation'
ALTER TABLE [dbo].[AppSelfTakeStation]
ADD CONSTRAINT [PK_AppSelfTakeStation]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AppStsManager'
ALTER TABLE [dbo].[AppStsManager]
ADD CONSTRAINT [PK_AppStsManager]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AppStsOfficeTime'
ALTER TABLE [dbo].[AppStsOfficeTime]
ADD CONSTRAINT [PK_AppStsOfficeTime]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AppOrderPickUp'
ALTER TABLE [dbo].[AppOrderPickUp]
ADD CONSTRAINT [PK_AppOrderPickUp]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CateringSetting'
ALTER TABLE [dbo].[CateringSetting]
ADD CONSTRAINT [PK_CateringSetting]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WeChatQRCode'
ALTER TABLE [dbo].[WeChatQRCode]
ADD CONSTRAINT [PK_WeChatQRCode]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CateringComdtyXData'
ALTER TABLE [dbo].[CateringComdtyXData]
ADD CONSTRAINT [PK_CateringComdtyXData]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CateringBusinessHours'
ALTER TABLE [dbo].[CateringBusinessHours]
ADD CONSTRAINT [PK_CateringBusinessHours]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CateringShiftTime'
ALTER TABLE [dbo].[CateringShiftTime]
ADD CONSTRAINT [PK_CateringShiftTime]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ShiftTimeLog'
ALTER TABLE [dbo].[ShiftTimeLog]
ADD CONSTRAINT [PK_ShiftTimeLog]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderRefundInfo'
ALTER TABLE [dbo].[OrderRefundInfo]
ADD CONSTRAINT [PK_OrderRefundInfo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExpressOrderTemplate'
ALTER TABLE [dbo].[ExpressOrderTemplate]
ADD CONSTRAINT [PK_ExpressOrderTemplate]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExpressOrderTemplateProperty'
ALTER TABLE [dbo].[ExpressOrderTemplateProperty]
ADD CONSTRAINT [PK_ExpressOrderTemplateProperty]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderPrintLog'
ALTER TABLE [dbo].[OrderPrintLog]
ADD CONSTRAINT [PK_OrderPrintLog]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderPrintDetailLog'
ALTER TABLE [dbo].[OrderPrintDetailLog]
ADD CONSTRAINT [PK_OrderPrintDetailLog]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DistributionRule'
ALTER TABLE [dbo].[DistributionRule]
ADD CONSTRAINT [PK_DistributionRule]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DistributionIdentitySet'
ALTER TABLE [dbo].[DistributionIdentitySet]
ADD CONSTRAINT [PK_DistributionIdentitySet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DistributionIdentity'
ALTER TABLE [dbo].[DistributionIdentity]
ADD CONSTRAINT [PK_DistributionIdentity]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DistributionApply'
ALTER TABLE [dbo].[DistributionApply]
ADD CONSTRAINT [PK_DistributionApply]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Microshop'
ALTER TABLE [dbo].[Microshop]
ADD CONSTRAINT [PK_Microshop]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MicroshopCom'
ALTER TABLE [dbo].[MicroshopCom]
ADD CONSTRAINT [PK_MicroshopCom]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DistributionApplyAudit'
ALTER TABLE [dbo].[DistributionApplyAudit]
ADD CONSTRAINT [PK_DistributionApplyAudit]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WeChatQrCodeType'
ALTER TABLE [dbo].[WeChatQrCodeType]
ADD CONSTRAINT [PK_WeChatQrCodeType]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MallApply'
ALTER TABLE [dbo].[MallApply]
ADD CONSTRAINT [PK_MallApply]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BaseCommission'
ALTER TABLE [dbo].[BaseCommission]
ADD CONSTRAINT [PK_BaseCommission]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CategoryCommission'
ALTER TABLE [dbo].[CategoryCommission]
ADD CONSTRAINT [PK_CategoryCommission]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityCommission'
ALTER TABLE [dbo].[CommodityCommission]
ADD CONSTRAINT [PK_CommodityCommission]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SettleAccounts'
ALTER TABLE [dbo].[SettleAccounts]
ADD CONSTRAINT [PK_SettleAccounts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SettleAccountsDetails'
ALTER TABLE [dbo].[SettleAccountsDetails]
ADD CONSTRAINT [PK_SettleAccountsDetails]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SettleAccountsOrderItem'
ALTER TABLE [dbo].[SettleAccountsOrderItem]
ADD CONSTRAINT [PK_SettleAccountsOrderItem]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SettleAccountsPeriod'
ALTER TABLE [dbo].[SettleAccountsPeriod]
ADD CONSTRAINT [PK_SettleAccountsPeriod]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommoditySettleAmount'
ALTER TABLE [dbo].[CommoditySettleAmount]
ADD CONSTRAINT [PK_CommoditySettleAmount]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'HTJSInvoice'
ALTER TABLE [dbo].[HTJSInvoice]
ADD CONSTRAINT [PK_HTJSInvoice]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YJBJOrderItem'
ALTER TABLE [dbo].[YJBJOrderItem]
ADD CONSTRAINT [PK_YJBJOrderItem]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Code] in table 'CommodityTaxRate'
ALTER TABLE [dbo].[CommodityTaxRate]
ADD CONSTRAINT [PK_CommodityTaxRate]
    PRIMARY KEY CLUSTERED ([Code] ASC);
GO

-- Creating primary key on [Id] in table 'OrderField'
ALTER TABLE [dbo].[OrderField]
ADD CONSTRAINT [PK_OrderField]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'InnerCategory'
ALTER TABLE [dbo].[InnerCategory]
ADD CONSTRAINT [PK_InnerCategory]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Supplier'
ALTER TABLE [dbo].[Supplier]
ADD CONSTRAINT [PK_Supplier]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityCodeSeq'
ALTER TABLE [dbo].[CommodityCodeSeq]
ADD CONSTRAINT [PK_CommodityCodeSeq]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityInnerCategory'
ALTER TABLE [dbo].[CommodityInnerCategory]
ADD CONSTRAINT [PK_CommodityInnerCategory]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExpressCollection'
ALTER TABLE [dbo].[ExpressCollection]
ADD CONSTRAINT [PK_ExpressCollection]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExpressTemplateCollection'
ALTER TABLE [dbo].[ExpressTemplateCollection]
ADD CONSTRAINT [PK_ExpressTemplateCollection]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SupplierMain'
ALTER TABLE [dbo].[SupplierMain]
ADD CONSTRAINT [PK_SupplierMain]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JdOrderItem'
ALTER TABLE [dbo].[JdOrderItem]
ADD CONSTRAINT [PK_JdOrderItem]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JdJournal'
ALTER TABLE [dbo].[JdJournal]
ADD CONSTRAINT [PK_JdJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ServiceSettings'
ALTER TABLE [dbo].[ServiceSettings]
ADD CONSTRAINT [PK_ServiceSettings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YJBJCard'
ALTER TABLE [dbo].[YJBJCard]
ADD CONSTRAINT [PK_YJBJCard]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Jdlogs'
ALTER TABLE [dbo].[Jdlogs]
ADD CONSTRAINT [PK_Jdlogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PresentPromotion'
ALTER TABLE [dbo].[PresentPromotion]
ADD CONSTRAINT [PK_PresentPromotion]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PresentPromotionCommodity'
ALTER TABLE [dbo].[PresentPromotionCommodity]
ADD CONSTRAINT [PK_PresentPromotionCommodity]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PresentPromotionGift'
ALTER TABLE [dbo].[PresentPromotionGift]
ADD CONSTRAINT [PK_PresentPromotionGift]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderItemPresent'
ALTER TABLE [dbo].[OrderItemPresent]
ADD CONSTRAINT [PK_OrderItemPresent]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SettleAccountsException'
ALTER TABLE [dbo].[SettleAccountsException]
ADD CONSTRAINT [PK_SettleAccountsException]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderStatistics'
ALTER TABLE [dbo].[OrderStatistics]
ADD CONSTRAINT [PK_OrderStatistics]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityChange'
ALTER TABLE [dbo].[CommodityChange]
ADD CONSTRAINT [PK_CommodityChange]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JdOrderRefundAfterSales'
ALTER TABLE [dbo].[JdOrderRefundAfterSales]
ADD CONSTRAINT [PK_JdOrderRefundAfterSales]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LongisticsTrack'
ALTER TABLE [dbo].[LongisticsTrack]
ADD CONSTRAINT [PK_LongisticsTrack]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JdExpressTrace'
ALTER TABLE [dbo].[JdExpressTrace]
ADD CONSTRAINT [PK_JdExpressTrace]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AuditCommodity'
ALTER TABLE [dbo].[AuditCommodity]
ADD CONSTRAINT [PK_AuditCommodity]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AuditCommodityStock'
ALTER TABLE [dbo].[AuditCommodityStock]
ADD CONSTRAINT [PK_AuditCommodityStock]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AuditManage'
ALTER TABLE [dbo].[AuditManage]
ADD CONSTRAINT [PK_AuditManage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AuditMode'
ALTER TABLE [dbo].[AuditMode]
ADD CONSTRAINT [PK_AuditMode]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JDAuditMode'
ALTER TABLE [dbo].[JDAuditMode]
ADD CONSTRAINT [PK_JDAuditMode]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JdAuditCommodity'
ALTER TABLE [dbo].[JdAuditCommodity]
ADD CONSTRAINT [PK_JdAuditCommodity]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JdAuditCommodityStock'
ALTER TABLE [dbo].[JdAuditCommodityStock]
ADD CONSTRAINT [PK_JdAuditCommodityStock]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YJEmployee'
ALTER TABLE [dbo].[YJEmployee]
ADD CONSTRAINT [PK_YJEmployee]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Specifications'
ALTER TABLE [dbo].[Specifications]
ADD CONSTRAINT [PK_Specifications]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommoditySpecifications'
ALTER TABLE [dbo].[CommoditySpecifications]
ADD CONSTRAINT [PK_CommoditySpecifications]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JdCommodity'
ALTER TABLE [dbo].[JdCommodity]
ADD CONSTRAINT [PK_JdCommodity]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JDStockJournal'
ALTER TABLE [dbo].[JDStockJournal]
ADD CONSTRAINT [PK_JDStockJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JDEclpOrder'
ALTER TABLE [dbo].[JDEclpOrder]
ADD CONSTRAINT [PK_JDEclpOrder]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JDEclpOrderJournal'
ALTER TABLE [dbo].[JDEclpOrderJournal]
ADD CONSTRAINT [PK_JDEclpOrderJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JDEclpOrderRefundAfterSales'
ALTER TABLE [dbo].[JDEclpOrderRefundAfterSales]
ADD CONSTRAINT [PK_JDEclpOrderRefundAfterSales]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JDEclpOrderRefundAfterSalesItem'
ALTER TABLE [dbo].[JDEclpOrderRefundAfterSalesItem]
ADD CONSTRAINT [PK_JDEclpOrderRefundAfterSalesItem]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JDEclpOrderRefundAfterSalesJournal'
ALTER TABLE [dbo].[JDEclpOrderRefundAfterSalesJournal]
ADD CONSTRAINT [PK_JDEclpOrderRefundAfterSalesJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [UserId] in table 'UserCodeForHaiXin'
ALTER TABLE [dbo].[UserCodeForHaiXin]
ADD CONSTRAINT [PK_UserCodeForHaiXin]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- Creating primary key on [Id] in table 'HaiXinMqJournal'
ALTER TABLE [dbo].[HaiXinMqJournal]
ADD CONSTRAINT [PK_HaiXinMqJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'RefundExpressTrace'
ALTER TABLE [dbo].[RefundExpressTrace]
ADD CONSTRAINT [PK_RefundExpressTrace]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'FreightRangeDetails'
ALTER TABLE [dbo].[FreightRangeDetails]
ADD CONSTRAINT [PK_FreightRangeDetails]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YKBigDataMqJournal'
ALTER TABLE [dbo].[YKBigDataMqJournal]
ADD CONSTRAINT [PK_YKBigDataMqJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YXOrder'
ALTER TABLE [dbo].[YXOrder]
ADD CONSTRAINT [PK_YXOrder]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YXOrderJournal'
ALTER TABLE [dbo].[YXOrderJournal]
ADD CONSTRAINT [PK_YXOrderJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YXOrderPackage'
ALTER TABLE [dbo].[YXOrderPackage]
ADD CONSTRAINT [PK_YXOrderPackage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YXOrderSku'
ALTER TABLE [dbo].[YXOrderSku]
ADD CONSTRAINT [PK_YXOrderSku]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YXExpressDetailInfo'
ALTER TABLE [dbo].[YXExpressDetailInfo]
ADD CONSTRAINT [PK_YXExpressDetailInfo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YXExpressDetailInfoSku'
ALTER TABLE [dbo].[YXExpressDetailInfoSku]
ADD CONSTRAINT [PK_YXExpressDetailInfoSku]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YXComInfo'
ALTER TABLE [dbo].[YXComInfo]
ADD CONSTRAINT [PK_YXComInfo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YXOrderPackageJournal'
ALTER TABLE [dbo].[YXOrderPackageJournal]
ADD CONSTRAINT [PK_YXOrderPackageJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YXOrderErrorLog'
ALTER TABLE [dbo].[YXOrderErrorLog]
ADD CONSTRAINT [PK_YXOrderErrorLog]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YXExpressTrace'
ALTER TABLE [dbo].[YXExpressTrace]
ADD CONSTRAINT [PK_YXExpressTrace]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AppMetaData'
ALTER TABLE [dbo].[AppMetaData]
ADD CONSTRAINT [PK_AppMetaData]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityPriceFloat'
ALTER TABLE [dbo].[CommodityPriceFloat]
ADD CONSTRAINT [PK_CommodityPriceFloat]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CategoryAdvertise'
ALTER TABLE [dbo].[CategoryAdvertise]
ADD CONSTRAINT [PK_CategoryAdvertise]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityInnerBrand'
ALTER TABLE [dbo].[CommodityInnerBrand]
ADD CONSTRAINT [PK_CommodityInnerBrand]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Brandwall'
ALTER TABLE [dbo].[Brandwall]
ADD CONSTRAINT [PK_Brandwall]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CategoryInnerBrand'
ALTER TABLE [dbo].[CategoryInnerBrand]
ADD CONSTRAINT [PK_CategoryInnerBrand]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityOrderRefund'
ALTER TABLE [dbo].[CommodityOrderRefund]
ADD CONSTRAINT [PK_CommodityOrderRefund]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SNOrderItem'
ALTER TABLE [dbo].[SNOrderItem]
ADD CONSTRAINT [PK_SNOrderItem]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SNExpressTrace'
ALTER TABLE [dbo].[SNExpressTrace]
ADD CONSTRAINT [PK_SNExpressTrace]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SNPackageTrace'
ALTER TABLE [dbo].[SNPackageTrace]
ADD CONSTRAINT [PK_SNPackageTrace]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SNOrderRefundAfterSales'
ALTER TABLE [dbo].[SNOrderRefundAfterSales]
ADD CONSTRAINT [PK_SNOrderRefundAfterSales]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdECommerce'
ALTER TABLE [dbo].[ThirdECommerce]
ADD CONSTRAINT [PK_ThirdECommerce]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdECOrder'
ALTER TABLE [dbo].[ThirdECOrder]
ADD CONSTRAINT [PK_ThirdECOrder]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdECOrderJournal'
ALTER TABLE [dbo].[ThirdECOrderJournal]
ADD CONSTRAINT [PK_ThirdECOrderJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdECOrderPackage'
ALTER TABLE [dbo].[ThirdECOrderPackage]
ADD CONSTRAINT [PK_ThirdECOrderPackage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdECOrderPackageSku'
ALTER TABLE [dbo].[ThirdECOrderPackageSku]
ADD CONSTRAINT [PK_ThirdECOrderPackageSku]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdECOrderErrorLog'
ALTER TABLE [dbo].[ThirdECOrderErrorLog]
ADD CONSTRAINT [PK_ThirdECOrderErrorLog]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdECExpressTrace'
ALTER TABLE [dbo].[ThirdECExpressTrace]
ADD CONSTRAINT [PK_ThirdECExpressTrace]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdECStockJournal'
ALTER TABLE [dbo].[ThirdECStockJournal]
ADD CONSTRAINT [PK_ThirdECStockJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdECService'
ALTER TABLE [dbo].[ThirdECService]
ADD CONSTRAINT [PK_ThirdECService]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdECServiceJournal'
ALTER TABLE [dbo].[ThirdECServiceJournal]
ADD CONSTRAINT [PK_ThirdECServiceJournal]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [id] in table 'YJEmTemp'
ALTER TABLE [dbo].[YJEmTemp]
ADD CONSTRAINT [PK_YJEmTemp]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id] in table 'YJBDSFOrderInfo'
ALTER TABLE [dbo].[YJBDSFOrderInfo]
ADD CONSTRAINT [PK_YJBDSFOrderInfo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YJBCarInsuranceRebate'
ALTER TABLE [dbo].[YJBCarInsuranceRebate]
ADD CONSTRAINT [PK_YJBCarInsuranceRebate]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YJBCarInsuranceReport'
ALTER TABLE [dbo].[YJBCarInsuranceReport]
ADD CONSTRAINT [PK_YJBCarInsuranceReport]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'YJBCarInsReportDetail'
ALTER TABLE [dbo].[YJBCarInsReportDetail]
ADD CONSTRAINT [PK_YJBCarInsReportDetail]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SNOrder'
ALTER TABLE [dbo].[SNOrder]
ADD CONSTRAINT [PK_SNOrder]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CouponRefundDetail'
ALTER TABLE [dbo].[CouponRefundDetail]
ADD CONSTRAINT [PK_CouponRefundDetail]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FangZhengOrder'
ALTER TABLE [dbo].[FangZhengOrder]
ADD CONSTRAINT [PK_FangZhengOrder]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FangZhengLogistics'
ALTER TABLE [dbo].[FangZhengLogistics]
ADD CONSTRAINT [PK_FangZhengLogistics]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdMapStatus'
ALTER TABLE [dbo].[ThirdMapStatus]
ADD CONSTRAINT [PK_ThirdMapStatus]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdMerchant'
ALTER TABLE [dbo].[ThirdMerchant]
ADD CONSTRAINT [PK_ThirdMerchant]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'SNBill'
ALTER TABLE [dbo].[SNBill]
ADD CONSTRAINT [PK_SNBill]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'InsuranceCompanyActivity'
ALTER TABLE [dbo].[InsuranceCompanyActivity]
ADD CONSTRAINT [PK_InsuranceCompanyActivity]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'InsuranceCompany'
ALTER TABLE [dbo].[InsuranceCompany]
ADD CONSTRAINT [PK_InsuranceCompany]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [FreightTemplateId] in table 'FreightTemplateDetail'
ALTER TABLE [dbo].[FreightTemplateDetail]
ADD CONSTRAINT [FK_FreightTemplateFreightTemplateDetail]
    FOREIGN KEY ([FreightTemplateId])
    REFERENCES [dbo].[FreightTemplate]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FreightTemplateFreightTemplateDetail'
CREATE INDEX [IX_FK_FreightTemplateFreightTemplateDetail]
ON [dbo].[FreightTemplateDetail]
    ([FreightTemplateId]);
GO

-- Creating foreign key on [CateringSettingId] in table 'CateringBusinessHours'
ALTER TABLE [dbo].[CateringBusinessHours]
ADD CONSTRAINT [FK_CateringSettingCateringBusinessHours]
    FOREIGN KEY ([CateringSettingId])
    REFERENCES [dbo].[CateringSetting]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CateringSettingCateringBusinessHours'
CREATE INDEX [IX_FK_CateringSettingCateringBusinessHours]
ON [dbo].[CateringBusinessHours]
    ([CateringSettingId]);
GO

-- Creating foreign key on [CateringSettingId] in table 'CateringShiftTime'
ALTER TABLE [dbo].[CateringShiftTime]
ADD CONSTRAINT [FK_CateringSettingCateringShiftTime]
    FOREIGN KEY ([CateringSettingId])
    REFERENCES [dbo].[CateringSetting]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CateringSettingCateringShiftTime'
CREATE INDEX [IX_FK_CateringSettingCateringShiftTime]
ON [dbo].[CateringShiftTime]
    ([CateringSettingId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
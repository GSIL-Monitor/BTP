
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 05/03/2018 11:35:11
-- Generated from EDMX file: D:\work\01-AMP\08-Program\Code\Biz\BTP-DEV\Model\Jinher.AMP.BTP.EDMX\YJBJ.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [YJBJ];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[MoneySummary]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MoneySummary];
GO
IF OBJECT_ID(N'[dbo].[DiscountSummary]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DiscountSummary];
GO
IF OBJECT_ID(N'[dbo].[CommodityOrder1]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityOrder1];
GO
IF OBJECT_ID(N'[dbo].[PayTransaction1]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PayTransaction1];
GO
IF OBJECT_ID(N'[dbo].[OrderItem1]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderItem1];
GO
IF OBJECT_ID(N'[dbo].[TaxSummary]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaxSummary];
GO
IF OBJECT_ID(N'[dbo].[SettleAccounts1]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SettleAccounts1];
GO
IF OBJECT_ID(N'[dbo].[SettleAccountsDetails1]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SettleAccountsDetails1];
GO
IF OBJECT_ID(N'[dbo].[WeiXinBill]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WeiXinBill];
GO
IF OBJECT_ID(N'[dbo].[WeiXinBillSummary]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WeiXinBillSummary];
GO
IF OBJECT_ID(N'[dbo].[AliPayBill]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AliPayBill];
GO
IF OBJECT_ID(N'[dbo].[AliPayBillSummary]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AliPayBillSummary];
GO
IF OBJECT_ID(N'[dbo].[OrderDiscount]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderDiscount];
GO
IF OBJECT_ID(N'[dbo].[CommodityDiscount]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommodityDiscount];
GO
IF OBJECT_ID(N'[dbo].[UnionPayBill]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UnionPayBill];
GO
IF OBJECT_ID(N'[dbo].[UnionPayBillSummary]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UnionPayBillSummary];
GO
IF OBJECT_ID(N'[dbo].[JinCaiMoneySummary]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JinCaiMoneySummary];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'MoneySummary'
CREATE TABLE [dbo].[MoneySummary] (
    [Id] uniqueidentifier  NOT NULL,
    [TradeDate] nvarchar(8)  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [AppType] smallint  NOT NULL,
    [DeliveryType] nvarchar(10)  NOT NULL,
    [PayType] nvarchar(10)  NOT NULL,
    [OrderState] nvarchar(10)  NOT NULL,
    [OrderCount] int  NOT NULL,
    [ShouldMoney] decimal(18,2)  NOT NULL,
    [DiscountMoney] decimal(18,2)  NOT NULL,
    [PayMoney] decimal(18,2)  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL,
    [FreightMoney] decimal(18,2)  NOT NULL,
    [RefundFreightMoney] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'DiscountSummary'
CREATE TABLE [dbo].[DiscountSummary] (
    [Id] uniqueidentifier  NOT NULL,
    [TradeDate] nvarchar(8)  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [AppType] smallint  NOT NULL,
    [DeliveryType] nvarchar(10)  NOT NULL,
    [PayType] nvarchar(10)  NOT NULL,
    [OrderState] nvarchar(10)  NOT NULL,
    [DiscountType] nvarchar(10)  NOT NULL,
    [OrderCount] int  NOT NULL,
    [DiscountMoney] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'CommodityOrder1'
CREATE TABLE [dbo].[CommodityOrder1] (
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [AppType] smallint  NOT NULL,
    [Payment] int  NOT NULL,
    [State] int  NOT NULL,
    [OrderState] nvarchar(10)  NOT NULL,
    [SubTime] datetime  NOT NULL,
    [PayTime] datetime  NOT NULL,
    [RefundTime] datetime  NULL,
    [ReceiptUserName] nvarchar(512)  NOT NULL,
    [ReceiptPhone] nvarchar(512)  NOT NULL,
    [ReceiptAddress] nvarchar(4000)  NOT NULL,
    [RecipientsZipCode] nvarchar(6)  NULL,
    [Details] nvarchar(4000)  NULL,
    [ShouldMoney] decimal(18,2)  NOT NULL,
    [DiscountMoney] decimal(18,2)  NOT NULL,
    [PayMoney] decimal(18,2)  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL,
    [FreightMoney] decimal(18,2)  NOT NULL,
    [MainOrderId] uniqueidentifier  NULL,
    [SubId] uniqueidentifier  NULL
);
GO

-- Creating table 'PayTransaction1'
CREATE TABLE [dbo].[PayTransaction1] (
    [TradeId] nvarchar(60)  NOT NULL,
    [TradeDate] nvarchar(8)  NOT NULL,
    [TradeTime] datetime  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [TradeNum] nvarchar(50)  NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [AppType] smallint  NOT NULL,
    [PayType] int  NOT NULL,
    [TradeResult] int  NOT NULL,
    [OrderState] nvarchar(10)  NOT NULL,
    [ShouldMoney] decimal(18,2)  NOT NULL,
    [DiscountMoney] decimal(18,2)  NOT NULL,
    [PayMoney] decimal(18,2)  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL,
    [FreightMoney] decimal(18,2)  NOT NULL,
    [RefundFreightMoney] decimal(18,2)  NOT NULL,
    [NotifyData] nvarchar(max)  NULL,
    [RefundNotifyData] nvarchar(max)  NULL,
    [RefundTradeNum] nvarchar(50)  NULL,
    [RealTradeTime] datetime  NULL,
    [MainOrderId] uniqueidentifier  NULL,
    [PayorId] uniqueidentifier  NULL,
    [ThirdUserId] nvarchar(50)  NULL,
    [SupplierCode] nvarchar(128)  NULL,
    [SupplierName] nvarchar(512)  NULL,
    [SupplierType] smallint  NULL,
    [JcActivityId] uniqueidentifier  NULL,
    [JcCustomerId] uniqueidentifier  NULL,
    [JcCustomerCode] nvarchar(30)  NULL,
    [JcCustomerName] nvarchar(30)  NULL,
    [JcCustomerProperty] nvarchar(10)  NULL,
    [YJCouponMoney] decimal(18,2)  NULL
);
GO

-- Creating table 'OrderItem1'
CREATE TABLE [dbo].[OrderItem1] (
    [Id] uniqueidentifier  NOT NULL,
    [TradeId] nvarchar(60)  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [AppType] smallint  NOT NULL,
    [CategoryName] nvarchar(max)  NULL,
    [CommodityName] nvarchar(512)  NOT NULL,
    [Number] int  NOT NULL,
    [RealPrice] decimal(18,2)  NOT NULL,
    [TaxRate] decimal(18,2)  NOT NULL,
    [ShouldMoney] decimal(18,2)  NOT NULL,
    [DiscountMoney] decimal(18,2)  NOT NULL,
    [PayMoney] decimal(18,2)  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL,
    [FreightMoney] decimal(18,2)  NOT NULL,
    [RefundFreightMoney] decimal(18,2)  NOT NULL,
    [TaxMoney] decimal(18,2)  NOT NULL,
    [CostPrice] decimal(18,2)  NULL,
    [InputRax] decimal(18,2)  NULL,
    [OrderItemId] uniqueidentifier  NULL,
    [SubTime] datetime  NULL,
    [CommodityStockId] uniqueidentifier  NULL
);
GO

-- Creating table 'TaxSummary'
CREATE TABLE [dbo].[TaxSummary] (
    [Id] uniqueidentifier  NOT NULL,
    [TradeDate] nvarchar(8)  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [TaxRate] nvarchar(10)  NOT NULL,
    [OrderState] nvarchar(10)  NOT NULL,
    [OrderCount] int  NOT NULL,
    [ShouldMoney] decimal(18,2)  NOT NULL,
    [DiscountMoney] decimal(18,2)  NOT NULL,
    [PayMoney] decimal(18,2)  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL,
    [FreightMoney] decimal(18,2)  NOT NULL,
    [RefundFreightMoney] decimal(18,2)  NOT NULL,
    [TaxMoney] decimal(18,2)  NOT NULL,
    [AppType] smallint  NOT NULL
);
GO

-- Creating table 'SettleAccounts1'
CREATE TABLE [dbo].[SettleAccounts1] (
    [Id] uniqueidentifier  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [AppType] smallint  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [SubDate] nvarchar(8)  NOT NULL,
    [BeginDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [OrderAmount] decimal(18,2)  NOT NULL,
    [OrderRealAmount] decimal(18,2)  NOT NULL,
    [CouponAmount] decimal(18,2)  NOT NULL,
    [RefundAmount] decimal(18,2)  NOT NULL,
    [PromotionAmount] decimal(18,2)  NOT NULL,
    [SellerAmount] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'SettleAccountsDetails1'
CREATE TABLE [dbo].[SettleAccountsDetails1] (
    [Id] uniqueidentifier  NOT NULL,
    [SAId] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [AppType] smallint  NOT NULL,
    [OrderAmount] decimal(18,2)  NOT NULL,
    [OrderRealAmount] decimal(18,2)  NOT NULL,
    [OrderCouponAmount] decimal(18,2)  NOT NULL,
    [OrderRefundAmount] decimal(18,2)  NOT NULL,
    [PromotionAmount] decimal(18,2)  NOT NULL,
    [SellerAmount] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'WeiXinBill'
CREATE TABLE [dbo].[WeiXinBill] (
    [Id] uniqueidentifier  NOT NULL,
    [TradeDate] nvarchar(8)  NOT NULL,
    [TradeTime] datetime  NOT NULL,
    [WeiXinAppId] nvarchar(50)  NOT NULL,
    [MchId] nvarchar(50)  NOT NULL,
    [SubMchId] nvarchar(50)  NOT NULL,
    [DeviceInfo] nvarchar(50)  NOT NULL,
    [WeiXinOrderCode] nvarchar(50)  NOT NULL,
    [TempId] nvarchar(50)  NOT NULL,
    [WeiXinUserId] nvarchar(50)  NOT NULL,
    [TradeType] nvarchar(50)  NOT NULL,
    [TradeState] nvarchar(50)  NOT NULL,
    [Bank] nvarchar(50)  NOT NULL,
    [MoneyType] nvarchar(50)  NOT NULL,
    [PayMoney] decimal(18,2)  NOT NULL,
    [HongBaoMoney] decimal(18,2)  NOT NULL,
    [WeiXinRefundOrderCode] nvarchar(50)  NOT NULL,
    [RefundTempId] nvarchar(50)  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL,
    [RefundHongBaoMoney] decimal(18,2)  NOT NULL,
    [RefundType] nvarchar(50)  NOT NULL,
    [RefundState] nvarchar(50)  NOT NULL,
    [CommodityName] nvarchar(512)  NOT NULL,
    [MchData] nvarchar(max)  NOT NULL,
    [Commission] decimal(18,5)  NOT NULL,
    [CommissionRate] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'WeiXinBillSummary'
CREATE TABLE [dbo].[WeiXinBillSummary] (
    [Id] uniqueidentifier  NOT NULL,
    [WeiXinAppId] nvarchar(50)  NOT NULL,
    [MchId] nvarchar(50)  NOT NULL,
    [TradeDate] nvarchar(8)  NOT NULL,
    [TradeTime] datetime  NOT NULL,
    [TradeCount] int  NOT NULL,
    [PayMoney] decimal(18,2)  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL,
    [RefundHongBaoMoney] decimal(18,2)  NOT NULL,
    [Commission] decimal(18,5)  NOT NULL
);
GO

-- Creating table 'AliPayBill'
CREATE TABLE [dbo].[AliPayBill] (
    [Id] uniqueidentifier  NOT NULL,
    [PId] nvarchar(50)  NOT NULL,
    [TradeDate] nvarchar(8)  NOT NULL,
    [TradeTime] datetime  NOT NULL,
    [AliPayTradeNo] nvarchar(50)  NOT NULL,
    [MerchantOrderCodeFull] nvarchar(50)  NOT NULL,
    [MerchantOrderCode] uniqueidentifier  NOT NULL,
    [TradeState] nvarchar(50)  NOT NULL,
    [CommodityName] nvarchar(512)  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [CompleteTime] datetime  NOT NULL,
    [StoreCode] nvarchar(50)  NOT NULL,
    [StoreName] nvarchar(50)  NOT NULL,
    [Operator] nvarchar(50)  NOT NULL,
    [TerminalNumber] nvarchar(50)  NOT NULL,
    [ReceiptAccount] nvarchar(50)  NOT NULL,
    [OrderMoney] decimal(18,2)  NOT NULL,
    [ReceiptMoney] decimal(18,2)  NOT NULL,
    [HongBaoMoney] decimal(18,2)  NOT NULL,
    [JiFenBaoMoney] decimal(18,2)  NOT NULL,
    [AliPayDiscountMoney] decimal(18,2)  NOT NULL,
    [MerchantDiscountMoney] decimal(18,2)  NOT NULL,
    [QuanHeXiaoMoney] decimal(18,2)  NOT NULL,
    [QuanName] nvarchar(50)  NOT NULL,
    [MerchantHongBaoMoney] decimal(18,2)  NOT NULL,
    [CardConsumptionMoney] decimal(18,2)  NOT NULL,
    [AliPayRefundTradeNo] nvarchar(50)  NOT NULL,
    [ServiceCharge] decimal(18,2)  NOT NULL,
    [FenRunMoney] decimal(18,2)  NOT NULL,
    [Remark] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'AliPayBillSummary'
CREATE TABLE [dbo].[AliPayBillSummary] (
    [Id] uniqueidentifier  NOT NULL,
    [PId] nvarchar(50)  NOT NULL,
    [TradeDate] nvarchar(8)  NOT NULL,
    [TradeTime] datetime  NOT NULL,
    [StoreCode] nvarchar(50)  NOT NULL,
    [StoreName] nvarchar(50)  NOT NULL,
    [PayCount] int  NOT NULL,
    [RefundCount] int  NOT NULL,
    [TradeCount] int  NOT NULL,
    [OrderMoney] decimal(18,2)  NOT NULL,
    [ReceiptMoney] decimal(18,2)  NOT NULL,
    [AliPayDiscountMoney] decimal(18,2)  NOT NULL,
    [MerchantDiscountMoney] decimal(18,2)  NOT NULL,
    [CardConsumptionMoney] decimal(18,2)  NOT NULL,
    [ServiceCharge] decimal(18,2)  NOT NULL,
    [FenRunMoney] decimal(18,2)  NOT NULL,
    [ShiShouMoney] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'OrderDiscount'
CREATE TABLE [dbo].[OrderDiscount] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [AppType] smallint  NOT NULL,
    [DiscountType] nvarchar(10)  NOT NULL,
    [DiscountMoney] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'CommodityDiscount'
CREATE TABLE [dbo].[CommodityDiscount] (
    [Id] uniqueidentifier  NOT NULL,
    [OrderId] uniqueidentifier  NOT NULL,
    [OrderCode] nvarchar(128)  NOT NULL,
    [CommodityId] uniqueidentifier  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [AppType] smallint  NOT NULL,
    [DiscountType] nvarchar(10)  NOT NULL,
    [DiscountMoney] decimal(18,2)  NOT NULL
);
GO

-- Creating table 'UnionPayBill'
CREATE TABLE [dbo].[UnionPayBill] (
    [Id] uniqueidentifier  NOT NULL,
    [TradeDate] nvarchar(8)  NOT NULL,
    [TradeTime] datetime  NOT NULL,
    [TradeCode] nvarchar(3)  NOT NULL,
    [AcqInsCode] nvarchar(11)  NOT NULL,
    [SendInsCode] nvarchar(11)  NOT NULL,
    [TraceNo] nvarchar(6)  NOT NULL,
    [TxnTime] nvarchar(10)  NOT NULL,
    [PayCardNo] nvarchar(19)  NOT NULL,
    [TxnAmt] nvarchar(12)  NOT NULL,
    [MerCatCode] nvarchar(4)  NOT NULL,
    [TermType] nvarchar(2)  NOT NULL,
    [QueryId] nvarchar(21)  NOT NULL,
    [PayCode] nvarchar(2)  NOT NULL,
    [OrderId] nvarchar(32)  NOT NULL,
    [OrderIdGuid] uniqueidentifier  NOT NULL,
    [PayCardType] nvarchar(11)  NOT NULL,
    [OriginalTraceNo] nvarchar(6)  NOT NULL,
    [OriginalTxnTime] nvarchar(10)  NOT NULL,
    [Commission] nvarchar(13)  NOT NULL,
    [SettleMoney] nvarchar(13)  NOT NULL,
    [PayType] nvarchar(4)  NOT NULL,
    [TxnType] nvarchar(2)  NOT NULL,
    [BizType] nvarchar(6)  NOT NULL,
    [OrigQryId] nvarchar(21)  NOT NULL,
    [MerId] nvarchar(15)  NOT NULL,
    [SubMerId] nvarchar(15)  NOT NULL,
    [SubMerMoney] nvarchar(13)  NOT NULL,
    [SettleNetAmount] nvarchar(13)  NOT NULL,
    [TermId] nvarchar(8)  NOT NULL,
    [MerReserved] nvarchar(32)  NOT NULL,
    [DiscountMoney] nvarchar(13)  NOT NULL,
    [InvoiceMoney] nvarchar(13)  NOT NULL,
    [InstallmentCommission] nvarchar(12)  NOT NULL,
    [InstallmentCount] nvarchar(2)  NOT NULL,
    [OriOrderId] nvarchar(32)  NOT NULL,
    [OriOrderIdGuid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'UnionPayBillSummary'
CREATE TABLE [dbo].[UnionPayBillSummary] (
    [Id] uniqueidentifier  NOT NULL,
    [TradeDate] nvarchar(8)  NOT NULL,
    [TradeTime] datetime  NOT NULL,
    [TradeCount] int  NOT NULL,
    [TxnAmtD] decimal(18,2)  NOT NULL,
    [TxnAmtC] decimal(18,2)  NOT NULL,
    [CommissionMoneyD] decimal(18,2)  NOT NULL,
    [CommissionMoneyC] decimal(18,2)  NOT NULL,
    [SettleMoneyD] decimal(18,2)  NOT NULL,
    [SettleMoneyC] decimal(18,2)  NOT NULL,
    [MerId] nvarchar(15)  NOT NULL
);
GO

-- Creating table 'JinCaiMoneySummary'
CREATE TABLE [dbo].[JinCaiMoneySummary] (
    [Id] uniqueidentifier  NOT NULL,
    [TradeDate] nvarchar(8)  NOT NULL,
    [EsAppId] uniqueidentifier  NOT NULL,
    [EsAppName] nvarchar(50)  NOT NULL,
    [CustomerCode] nvarchar(30)  NOT NULL,
    [CustomerName] nvarchar(30)  NOT NULL,
    [CustomerProperty] nvarchar(10)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] nvarchar(50)  NOT NULL,
    [AppType] smallint  NOT NULL,
    [DeliveryType] nvarchar(10)  NOT NULL,
    [OrderState] nvarchar(10)  NOT NULL,
    [OrderCount] int  NOT NULL,
    [ShouldMoney] decimal(18,2)  NOT NULL,
    [DiscountMoney] decimal(18,2)  NOT NULL,
    [PayMoney] decimal(18,2)  NOT NULL,
    [RefundMoney] decimal(18,2)  NOT NULL,
    [FreightMoney] decimal(18,2)  NOT NULL,
    [RefundFreightMoney] decimal(18,2)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'MoneySummary'
ALTER TABLE [dbo].[MoneySummary]
ADD CONSTRAINT [PK_MoneySummary]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DiscountSummary'
ALTER TABLE [dbo].[DiscountSummary]
ADD CONSTRAINT [PK_DiscountSummary]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [OrderId] in table 'CommodityOrder1'
ALTER TABLE [dbo].[CommodityOrder1]
ADD CONSTRAINT [PK_CommodityOrder1]
    PRIMARY KEY CLUSTERED ([OrderId] ASC);
GO

-- Creating primary key on [TradeId] in table 'PayTransaction1'
ALTER TABLE [dbo].[PayTransaction1]
ADD CONSTRAINT [PK_PayTransaction1]
    PRIMARY KEY CLUSTERED ([TradeId] ASC);
GO

-- Creating primary key on [Id] in table 'OrderItem1'
ALTER TABLE [dbo].[OrderItem1]
ADD CONSTRAINT [PK_OrderItem1]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TaxSummary'
ALTER TABLE [dbo].[TaxSummary]
ADD CONSTRAINT [PK_TaxSummary]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SettleAccounts1'
ALTER TABLE [dbo].[SettleAccounts1]
ADD CONSTRAINT [PK_SettleAccounts1]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SettleAccountsDetails1'
ALTER TABLE [dbo].[SettleAccountsDetails1]
ADD CONSTRAINT [PK_SettleAccountsDetails1]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WeiXinBill'
ALTER TABLE [dbo].[WeiXinBill]
ADD CONSTRAINT [PK_WeiXinBill]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WeiXinBillSummary'
ALTER TABLE [dbo].[WeiXinBillSummary]
ADD CONSTRAINT [PK_WeiXinBillSummary]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AliPayBill'
ALTER TABLE [dbo].[AliPayBill]
ADD CONSTRAINT [PK_AliPayBill]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AliPayBillSummary'
ALTER TABLE [dbo].[AliPayBillSummary]
ADD CONSTRAINT [PK_AliPayBillSummary]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrderDiscount'
ALTER TABLE [dbo].[OrderDiscount]
ADD CONSTRAINT [PK_OrderDiscount]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommodityDiscount'
ALTER TABLE [dbo].[CommodityDiscount]
ADD CONSTRAINT [PK_CommodityDiscount]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UnionPayBill'
ALTER TABLE [dbo].[UnionPayBill]
ADD CONSTRAINT [PK_UnionPayBill]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UnionPayBillSummary'
ALTER TABLE [dbo].[UnionPayBillSummary]
ADD CONSTRAINT [PK_UnionPayBillSummary]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JinCaiMoneySummary'
ALTER TABLE [dbo].[JinCaiMoneySummary]
ADD CONSTRAINT [PK_JinCaiMoneySummary]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
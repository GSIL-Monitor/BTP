using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinher.AMP.News.SV.Test;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO;


namespace Jinher.AMP.BTP.SV.Test
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class CommodityOrderTest
    {
        public CommodityOrderTest()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性:
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestSaveCommodityOrder()
        {
            OrderSDTO orderSDTO = new OrderSDTO();
            orderSDTO.AppId = Guid.Parse(Setting.AppId);
            orderSDTO.City = "北京";
            orderSDTO.District = "海淀区";
            orderSDTO.Price = 50;
            orderSDTO.Province = "北京";
            orderSDTO.ReceiptAddress = "上地东路1号盈创动力大厦A座北厅401室";
            orderSDTO.ReceiptPhone = "15810819038";
            orderSDTO.ReceiptUserName = "李玲";
            orderSDTO.State = 0;
            orderSDTO.UserId = Guid.Parse(Setting.UserId);
            orderSDTO.ShoppingCartItemSDTO = new List<ShoppingCartItemSDTO>();

            ShoppingCartItemSDTO good = new ShoppingCartItemSDTO();
            good.Id = Guid.Parse("3AAE2EBB-25F2-4743-A173-93DA170D0A21");
            good.CommodityNumber = 2;
            good.SizeAndColorId = "cbdd46a6-08ec-45fc-98ae-a7f65da3f96f,2802acc0-9a02-4d2f-880f-cf48ac0d30dc";
            //good.SizeAndColorId

            orderSDTO.ShoppingCartItemSDTO.Add(good);



            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"orderSDTO\":" + JsonHelper.JsonSerializer<OrderSDTO>(orderSDTO) + "}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.CommodityOrderSV.svc/SaveCommodityOrder");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestUpdateCommodityOrder()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\",\"userId\":\"2fc486f8-a775-49f8-91e7-05b7de72a003\", \"orderId\":\"cd29a16e-2e4c-4c94-940b-be2a5b5ef3c7\", \"state\":\"1\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.CommodityOrderSV.svc/UpdateCommodityOrder");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestGetAllCommodityOrder()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\",\"userId\":\"2fc486f8-a775-49f8-91e7-05b7de72a003\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetAllCommodityOrder");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestGetOrderItems()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\",\"userId\":\"2fc486f8-a775-49f8-91e7-05b7de72a003\", \"commodityorderId\":\"cd29a16e-2e4c-4c94-940b-be2a5b5ef3c7\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetOrderItems");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestGetCommodityOrderByState()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\",\"userId\":\"2fc486f8-a775-49f8-91e7-05b7de72a003\", \"state\":\"\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetCommodityOrderByState");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }
    }
}

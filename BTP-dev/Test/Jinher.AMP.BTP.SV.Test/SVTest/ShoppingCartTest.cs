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
    public class ShoppingCartTest
    {
        public ShoppingCartTest()
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
        public void TestGetShoppingCartNum()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"userId\":\"2fc486f8-a775-49f8-91e7-05b7de72a003\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ShoppingCartSV.svc/GetShoppingCartNum");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestGetShoppongCartItems()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"userId\":\"2fc486f8-a775-49f8-91e7-05b7de72a003\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ShoppingCartSV.svc/GetShoppongCartItems");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestSaveShoppingCart()
        {
            ShoppingCartItemSDTO shoppingCartItemSDTO = new ShoppingCartItemSDTO();
            shoppingCartItemSDTO.Id = Guid.Parse("3AAE2EBB-25F2-4743-A173-93DA170D0A21");
            shoppingCartItemSDTO.CommodityNumber = 2;
            shoppingCartItemSDTO.SizeAndColorId = "cbdd46a6-08ec-45fc-98ae-a7f65da3f96f,2802acc0-9a02-4d2f-880f-cf48ac0d30dc";

            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"userId\":\"2fc486f8-a775-49f8-91e7-05b7de72a003\",\"shoppingCartItemsSDTO\":" + JsonHelper.JsonSerializer<ShoppingCartItemSDTO>(shoppingCartItemSDTO) + "}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ShoppingCartSV.svc/SaveShoppingCart");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestUpdateShoppingCart()
        {
            List<ShopCartCommodityUpdateDTO> list = new List<ShopCartCommodityUpdateDTO>();
            ShopCartCommodityUpdateDTO shoppingCartItemSDTO = new ShopCartCommodityUpdateDTO();
            shoppingCartItemSDTO.ShopCartItemId = Guid.Parse("598FCC6D-6939-4863-A2B9-19A5F7093651");
            shoppingCartItemSDTO.Number = 2;
            list.Add(shoppingCartItemSDTO);

            ShopCartCommodityUpdateDTO shoppingCartItemSDTO2 = new ShopCartCommodityUpdateDTO();
            shoppingCartItemSDTO2.ShopCartItemId = Guid.Parse("B7BA0FAE-622C-4BAA-8061-194DB893665D");
            shoppingCartItemSDTO2.Number = 1;
            list.Add(shoppingCartItemSDTO2);

            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"userId\":\"2fc486f8-a775-49f8-91e7-05b7de72a003\", \"shopCartCommodityUpdateDTOs\":" + JsonHelper.JsonSerializer<List<ShopCartCommodityUpdateDTO>>(list) + "}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ShoppingCartSV.svc/UpdateShoppingCart");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestDeleteShoppingCart()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"userId\":\"2fc486f8-a775-49f8-91e7-05b7de72a003\", \"shopCartItemId\":\"941063CF-490D-4CDE-8169-860537ADE06C\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ShoppingCartSV.svc/DeleteShoppingCart");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        //[TestMethod]
        //public void TestGetStoreByProvince()
        //{
        //    string requestData = "{\"province\":\"北京\",\"appId\":\"" + Setting.AppId + "\",\"pageIndex\":1,\"pageSize\":10}";
        //    var rest = new RestRequestTest("Jinher.AMP.BTP.SV.StoreSV.svc/GetStoreByProvince");
        //    string retJson = rest.Execute(requestData);
        //    //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        //}

        //[TestMethod]
        //public void TestGetShoppongCartItemsNew3()
        //{
        //    var obj = new Jinher.AMP.BTP.SV.ShoppingCartSV();
        //    var ret = obj.GetShoppongCartItemsNew3(Guid.NewGuid(), Guid.NewGuid());
        //    var retStr = JsonHelper.JsonSerializer(ret);
        //    Assert.IsFalse(false);
        //}
    }
}

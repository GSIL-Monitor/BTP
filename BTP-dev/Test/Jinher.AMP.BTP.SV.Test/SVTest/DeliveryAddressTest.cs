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
    public class DeliveryAddressTest
    {
        public DeliveryAddressTest()
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
        public void TestSaveDeliveryAddress()
        {
            AddressSDTO addressSDTO = new AddressSDTO();
            addressSDTO.AppId = Guid.Parse(Setting.AppId);
            addressSDTO.UserId = Guid.Parse(Setting.UserId);
            addressSDTO.City = "北京";
            addressSDTO.District = "海淀区";
            addressSDTO.Province = "北京";
            addressSDTO.ReceiptAddress = "上地东路1号盈创动力大厦A座北厅401室";
            addressSDTO.ReceiptPhone = "15810819038";
            addressSDTO.ReceiptUserName = "李玲";
            string requestData = "{\"addressDTO\":" + JsonHelper.JsonSerializer<AddressSDTO>(addressSDTO) + "}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/SaveDeliveryAddress");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestGetDeliveryAddress()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\",\"userId\":\"" + Setting.UserId + "\"}"; ;
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/GetDeliveryAddress");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestDeleteDeliveryAddress()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\",\"addressId\":\"" + Setting.UserId + "\"}"; ;
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/DeleteDeliveryAddress");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestUpdateDeliveryAddress()
        {
            AddressSDTO addressSDTO = new AddressSDTO();
            addressSDTO.AppId = Guid.Parse(Setting.AppId);
            addressSDTO.UserId = Guid.Parse(Setting.UserId);
            addressSDTO.City = "北京";
            addressSDTO.District = "昌平区";
            addressSDTO.Province = "北京";
            addressSDTO.ReceiptAddress = "沙河地铁";
            addressSDTO.ReceiptPhone = "15810819038";
            addressSDTO.ReceiptUserName = "李玲";

            string requestData = "{\"addressDTO\":" + JsonHelper.JsonSerializer<AddressSDTO>(addressSDTO) + "}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/UpdateDeliveryAddress");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestGetDeliveryAddressByAddressId()
        {
            AddressSDTO addressSDTO = new AddressSDTO();
            string requestData = "{\"addressId\":\"\",\"appId\":\"" + Setting.AppId + "\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.DeliveryAddressSV.svc/GetDeliveryAddressByAddressId");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }
    }
}

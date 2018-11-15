using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jinher.AMP.News.SV.Test;
using Jinher.AMP.CBC.Deploy.CustomDTO;


namespace Jinher.AMP.BTP.SV.Test
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class CommodityTest
    {
        public CommodityTest()
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
        public void TestGetCommodity()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"pageIndex\":1}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodity");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestGetWantCommodity()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"pageIndex\":1, \"want\":\"衣\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.CommoditySV.svc/GetWantCommodity");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestGetCommodityByCategory()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"pageIndex\":1, \"categoryId\":\"\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodityByCategory");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestGetCommodityDetails()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"commodityId\":\"\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodityDetails");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }

        [TestMethod]
        public void TestGetCommodityColor()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"commodityId\":\"\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodityColor");
            string retJson = rest.Execute(requestData);
            //ReturnDTO returnDTO = JsonHelper.JsonDeserialize<ReturnDTO>(retJson);
        }
    }
}

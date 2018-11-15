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
    public class ReviewTest
    {
        public ReviewTest()
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
        public void TestSaveReview()
        {
            ReviewSDTO reviewSDTO = new Deploy.CustomDTO.ReviewSDTO();
            reviewSDTO.AppId = Guid.Parse(Setting.AppId);
           // reviewSDTO.CommodityColor = "黑色";
            reviewSDTO.OrderItemId = Guid.Parse("3AAE2EBB-25F2-4743-A173-93DA170D0A21");
           // reviewSDTO.CommoditySize = "XL";
            reviewSDTO.Details = "很漂亮";
            reviewSDTO.Name = "评论人昵称";
            reviewSDTO.UserId = Guid.Parse(Setting.UserId);
            //reviewSDTO.SelectedComAttibutes = null;
            reviewSDTO.Replays = null;
            reviewSDTO.SubTime = DateTime.Now;
            reviewSDTO.UserHead = "";
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"reviewSDTO\":" + JsonHelper.JsonSerializer<ReviewSDTO>(reviewSDTO) + "}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ReviewSV.svc/SaveReview");
            string retJson = rest.Execute(requestData);
            ResultDTO returnDTO = JsonHelper.JsonDeserialize<ResultDTO>(retJson);
        }

        [TestMethod]
        public void TestReplyReview()
        {
            ReplySDTO replySDTO = new Deploy.CustomDTO.ReplySDTO();
            replySDTO.Details = "这是回复内容";
            replySDTO.PreId = Guid.Parse(Setting.UserId);
            replySDTO.ReplyerHead = "";
            replySDTO.ReplyerId = Guid.Parse(Setting.AppId);
            replySDTO.ReplyerName = "商家";
            replySDTO.ReviewId = Guid.Parse("43F62B79-F860-43CF-9B17-30EBA057AD73");
            replySDTO.SubTime = DateTime.Now;            

            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"replySDTO\":" + JsonHelper.JsonSerializer<ReplySDTO>(replySDTO) + "}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ReviewSV.svc/ReplyReview");
            string retJson = rest.Execute(requestData);
            ResultDTO returnDTO = JsonHelper.JsonDeserialize<ResultDTO>(retJson);
        }

        [TestMethod]
        public void TestGetReviewByCommodityId()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"commodityId\":\"3AAE2EBB-25F2-4743-A173-93DA170D0A21\",\"lastReviewTime\":" + JsonHelper.JsonSerializer<DateTime>(DateTime.Now.AddYears(30)) + "}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ReviewSV.svc/GetReviewByCommodityId");
            string retJson = rest.Execute(requestData);
            List<ReviewSDTO> returnDTO = JsonHelper.JsonDeserialize<List<ReviewSDTO>>(retJson);
        }

        [TestMethod]
        public void TestGetReviewByUserId()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"userId\":\"" + Setting.UserId + "\",\"lastReviewTime\":" + JsonHelper.JsonSerializer<DateTime>(DateTime.Now) + "}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ReviewSV.svc/GetReviewByUserId");
            string retJson = rest.Execute(requestData);
            List<ReviewSDTO> returnDTO = JsonHelper.JsonDeserialize<List<ReviewSDTO>>(retJson);
        }

        [TestMethod]
        public void TestGetReviewNum()
        {
            string requestData = "{\"appId\":\"" + Setting.AppId + "\", \"commodityId\":\"3AAE2EBB-25F2-4743-A173-93DA170D0A21\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ReviewSV.svc/GetReviewNum");
            string retJson = rest.Execute(requestData);
            NumResultSDTO returnDTO = JsonHelper.JsonDeserialize<NumResultSDTO>(retJson);
        }

        [TestMethod]
        public void TestUpdateReview()
        {
            string requestData = "{\"userId\":\"" + Setting.UserId + "\", \"reviewId\":\"43F62B79-F860-43CF-9B17-30EBA057AD73\", \"content\":\"修改后内容\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ReviewSV.svc/UpdateReview");
            string retJson = rest.Execute(requestData);
            ResultDTO returnDTO = JsonHelper.JsonDeserialize<ResultDTO>(retJson);
        }

        [TestMethod]
        public void TestGetReplyByReviewId()
        {
            string requestData = "{\"userId\":\"" + Setting.UserId + "\", \"reviewId\":\"43F62B79-F860-43CF-9B17-30EBA057AD73\"}";
            var rest = new RestRequestTest("Jinher.AMP.BTP.SV.ReviewSV.svc/GetReplyByReviewId");
            string retJson = rest.Execute(requestData);
            List<ReplySDTO> returnDTO = JsonHelper.JsonDeserialize<List<ReplySDTO>>(retJson);
        }
    }
}

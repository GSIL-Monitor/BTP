var flex = function(){
    var deviceWidth = document.documentElement.clientWidth>500 ? 500 : document.documentElement.clientWidth;
    document.documentElement.style.fontSize = deviceWidth / 7.5 + 'px';
};
flex();
window.onresize = function(){
    flex();
};
var vm = new Vue({
    el: '#app',
    mounted: function(){
        this.$nextTick(function(){
            this.loadDate();
            window.addEventListener('scroll',this.handleEvent,false);
        });
    },
    data: function(){
        return {
            pageIndex: 1,//页码
            pageSize: 10,//每页数量
            isEndPage: false,//是否是最后一页
            allowScroll: true,
            orderList:[],
            tips: false,
            empty: false
        }
    },
    methods:{
        loadDate: function(){
            this.$http({
                url: '/Mobile/GetRefundOrderList',
                method: 'GET',
                params:{
                    userId: getUserId(),
                    pageIndex: this.pageIndex,
                    pageSize: this.pageSize,
                    esAppId:getEsAppId()
                },
                before:function(){
                    this.$loading.open();
                }
            }).then(function(response){
                this.$loading.close();
                var data = response.data;
                //测试代码
//                var data = [
//                    {
//                        "CommodityOrderId":"08ae71c0-6a82-4239-bbd6-e33fe1012eb9",
//                        "Price":211.76,
//                        "ShoppingCartItemSDTO":[
//                            {"OrderId":"08ae71c0-6a82-4239-bbd6-e33fe1012eb9","Id":"65789bed-9813-4e2b-b870-ec4e90357a08","Name":"【双节特惠】 贝斯特花茶30克 盒装 满200包邮","Price":105.88,"Intensity":10.00,"CommodityNumber":2,"Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017092011/c6e2a896-0dcd-4d33-abd8-998b9db60208_398fe51f-5e71-4528-8270-20c73ab194c7.jpg","Size":",30克","HasReview":false,"DiscountPrice":-1.00,"RealPrice":0,"CommodityId":"01c0737f-1b1e-4ca1-b6fa-88c7cdabec22","ComCategoryName":null,"CommodityAttributes":",30克","Duty":0}],"totalState0":0,"totalState1":0,"totalState2":0,"totalState3":0,"totalStateTui":0,"State":3,"Freight":0.00,"IsModifiedPrice":false,"OriginPrice":211.76,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","UserId":"baa8e821-c8e3-45d8-8c0b-dac91bd0935f","AppName":"景泓苑","PayType":0,"SelfTakeFlag":0,"StateAfterSales":15,"OrderRefundState":-1,"OrderRefundAfterSalesState":-1,"OrderType":0,"Batch":null,"PaymentTime":"\/Date(1506053693863)\/"},
//                    {"CommodityOrderId":"22c87e98-5737-4c9a-a9f2-a42725252577","Price":211.76,"ShoppingCartItemSDTO":[{"OrderId":"22c87e98-5737-4c9a-a9f2-a42725252577","Id":"41bbd581-cf3b-464d-9380-25fe6f412836","Name":"【双节特惠】 贝斯特花茶30克 盒装 满200包邮","Price":105.88,"Intensity":10.00,"CommodityNumber":2,"Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017092011/c6e2a896-0dcd-4d33-abd8-998b9db60208_398fe51f-5e71-4528-8270-20c73ab194c7.jpg","Size":",30克","HasReview":false,"DiscountPrice":-1.00,"RealPrice":0,"CommodityId":"01c0737f-1b1e-4ca1-b6fa-88c7cdabec22","ComCategoryName":null,"CommodityAttributes":",30克","Duty":0}],"totalState0":0,"totalState1":0,"totalState2":0,"totalState3":0,"totalStateTui":0,"State":6,"Freight":0.00,"IsModifiedPrice":false,"OriginPrice":211.76,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","UserId":"baa8e821-c8e3-45d8-8c0b-dac91bd0935f","AppName":"景泓苑","PayType":0,"SelfTakeFlag":0,"StateAfterSales":-1,"OrderRefundState":-1,"OrderRefundAfterSalesState":-1,"OrderType":0,"Batch":null,"PaymentTime":null},
//                    {"CommodityOrderId":"a52d582b-f9eb-46eb-914b-0a18c89bf84a","Price":204.52,"ShoppingCartItemSDTO":[{"OrderId":"a52d582b-f9eb-46eb-914b-0a18c89bf84a","Id":"ad6bb931-8d60-4190-90cc-0eed5281a990","Name":"【双节特惠】 贝斯特玫瑰酱200ml装 满200包邮","Price":32.88,"Intensity":10.00,"CommodityNumber":3,"Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017092014/4915ae09-11a1-4f0e-b36a-d4c9c19c5b99_ab245827-0d97-4266-a26a-25c73b542b41.jpg","Size":",200毫升","HasReview":false,"DiscountPrice":-1.00,"RealPrice":0,"CommodityId":"5fae31a9-2a22-440d-9a9b-2294afd3cea7","ComCategoryName":null,"CommodityAttributes":",200毫升","Duty":0}],"totalState0":0,"totalState1":0,"totalState2":0,"totalState3":0,"totalStateTui":0,"State":0,"Freight":0.00,"IsModifiedPrice":false,"OriginPrice":204.52,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","UserId":"baa8e821-c8e3-45d8-8c0b-dac91bd0935f","AppName":"景泓苑","PayType":0,"SelfTakeFlag":0,"StateAfterSales":15,"OrderRefundState":-1,"OrderRefundAfterSalesState":-1,"OrderType":0,"Batch":null,"PaymentTime":"\/Date(1505971097453)\/"}];
                if(data.length){
                    this.isEndPage = data.length - this.pageSize < 0 ? true : false;
                    if(this.isEndPage){
                        this.tips = true;
                    }
                    this.orderList = this.orderList.concat(data);
                }else{
                    if(this.pageIndex == 1){
                        this.orderList = [];
                        this.empty = true;
                    }else{
                        this.tips = true;
                    }
                }
            },function(response){
                this.$loading.close();
                this.$toast('服务器繁忙，请稍后再试');
            });
        },
        //滚动条滚动到底部加载数据
        handleEvent:function(){
            var scrollTope = document.body.scrollTop;
            var scrollHeight = document.body.scrollHeight;
            var clientHeight = document.documentElement.clientHeight;
            if(scrollHeight - scrollTope - clientHeight <= 10){
                if(!this.isEndPage){
                    this.pageIndex++;
                    this.loadDate();
                }
            }
        },
        goPage: function(state,data){
            if (state == "orderDetail") {
                //跳转订单详情
                window.location.href = urlAppendCommonParams("/Mobile/MyOrderDetail?orderId=" + data.OrderId + "&shopId=" + data.AppId + "&orderState=" + getQueryString("orderState") + "&sessionId=" + getQueryString("SessionId"));
            } else if (state == "refundInfo") {
                //var datai = data.ShoppingCartItemSDTO[0];
                var btnArr = getButtonByState(data, data.OrderState, data.Payment, data.OrderItemState);
                var _allDisplayBtns = btnArr.join(",");
                var viewRefundDetailUrl = urlAppendCommonParams("/Mobile/RefundInfo?orderId=" + data.OrderId + "&shopId=" + data.AppId);
                document.location.href = viewRefundDetailUrl + "&isAfterSale=0&allDisplayBtns=" + _allDisplayBtns + "&RefundExpCo=" + escape(data.RefundExpCo) + "&RefundExpNo=" + escape(data.RefundExpOrderNo) + "&orderItemId=" + data.ItemID;
                //跳转售后详情
                //window.location.href = '';
            }
        }
    },
    filters:{
        sizeFilter: function(value){
            if(!value) return;
            return value.split(',').join(' ');
        }
    }
});
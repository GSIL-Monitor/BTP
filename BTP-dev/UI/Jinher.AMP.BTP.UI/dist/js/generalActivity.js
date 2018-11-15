var vm = new Vue({
    el: '#app',
    data:{
        isEmpty: false,
        shopLists:[],
        goodsLists:[],
        horizontal: false,
        backTopShow: false,
        curFilterBar: 0,
        pageSize: 20,
        currentPage: 1,
        tips: '',
        fieldSort: 0, //0: 默认 1：价格 2：销量
        order: '', //1：升序（默认） 2：降序
        filterStock: true, //仅看有货
        filterMinPrice: '', //最低价
        filterMaxPrice: '', //最高价
        filterAppId: '' //所选的店铺App Id
    },
    mounted:function(){
        this.$nextTick(function(){
            this.loadData();
            window.addEventListener('scroll',this.handleEvent,false);
        });
    },
    methods:{
        handleEvent:function(){
            var scrollTope = document.body.scrollTop;
            var scrollHeight = document.body.scrollHeight;
            var clientHeight = document.documentElement.clientHeight;
            this.backTopShow = scrollTope ? true : false;
            if(scrollHeight - scrollTope - clientHeight <= 10){
                if(!this.endPage){
                    this.currentPage++;
                    this.loadData();
                }
            }
        },
        loadData:function(){
            if(this.currentPage == 1){
                this.goodsLists = [];
            }
            this.$loading.open();
            this.tips = "";
            var postdata = {
                "actId": getQueryString('actId'),
                "appId": getQueryString('esAppId'),
                "userId": getQueryString('userId'),
                "isAnnon": getQueryString('isAnnon'),
                "pageIndex": this.currentPage,//当前页数
                "pageSize": this.pageSize, //每页数据条数
                "saleArea": getCookie("selectCityCode"),
                "fieldSort": this.fieldSort,//排序类型
                "orderBy": this.order,//升降序
                "filter.isHasStock": this.filterStock,//仅看有货
                "filter.minPrice": this.filterMinPrice,//最低价
                "filter.maxPrice": this.filterMaxPrice,//最高价
                "filter.appId": this.filterAppId //所选的店铺App Id
            };
            this.$http.post("/GeneralActivity/GetActComdityList4App?r=" + Math.random(),postdata,{emulateJSON:true}).then(function(response){
                this.$loading.close();
                this.shopList = response.data.appInfoList;
                if(response.data.comdtyList.length){
                    this.isEmpty = false;
                    this.goodsLists.push.apply(this.goodsLists, response.data.comdtyList);
                    for(var i = 0; i < this.goodsLists.length; i++){
                        if(this.goodsLists[i].Id){
                            this.goodsLists[i].href =  url + "?commodityId=" + this.goodsLists[i].Id + RequestUrlParam();
                        }
                    }
                    if(response.data.comdtyList.length < this.pageSize){
                        this.tips = "已经看到最后一页了~";
                        this.endPage = true;
                    }
                }else{
                    if(this.goodsLists.length == 0){
                        this.isEmpty = true;
                    }else{
                        this.isEmpty = false;
                        this.tips = "已经看到最后一页了~";
                    }
                    this.endPage = true;
                }
            },function(err){
                this.$toast('服务器繁忙，请稍后再试!');
                this.$loading.close();

                //测试代码
                var data = {"realCount":0,"comdtyList":[{"RankNo":1,"Name":"【中洋鱼天下】中洋河豚经典套餐 江鲜包河豚鱼 食用熟食 速冻冷冻食品营养方便","Price":386.00,"Id":"98972e49-a360-40dc-94eb-b6e6474349a4","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017040516/0c99808f-179c-4b23-9086-94dc1e5a9609_d4d9b74b-2099-401c-b7a0-b055d31b75dd.jpg","Intensity":10,"State":0,"Stock":9946,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"68311c5f-823f-434d-8f59-64ef3141323d","IsActiveCrowdfunding":false,"AppName":"中洋鱼天下","MarketPrice":398.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":2,"Name":"【中洋鱼天下】红烧河豚鱼 食用已处理 速冻冷冻食品 熟食营养方便 两条装700g","Price":192.00,"Id":"553d8182-9859-46b4-b352-841fa723c7a3","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017040515/aa26909a-022b-4b9a-9d3e-c1676787b52a_b2ba5bba-cefe-46dc-8038-89ec8600c724.jpg","Intensity":10,"State":0,"Stock":9976,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"68311c5f-823f-434d-8f59-64ef3141323d","IsActiveCrowdfunding":false,"AppName":"中洋鱼天下","MarketPrice":198.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":3,"Name":"【中洋鱼天下】白汁河豚鱼 食用已处理 速冻冷冻食品 熟食营养方便 两条装800g","Price":192.00,"Id":"f45bcaf5-067e-4bdb-ba95-9fedfc307960","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017040515/617f5a65-bf58-4773-9c47-e073e0a5879d_c2e60902-820b-43d2-8e8f-5c5b11ba703f.jpg","Intensity":10,"State":0,"Stock":9995,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"68311c5f-823f-434d-8f59-64ef3141323d","IsActiveCrowdfunding":false,"AppName":"中洋鱼天下","MarketPrice":198.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0}],"appInfoList":[{"appId":"00000000-0000-0000-0000-000000000000","appName":"全部","icon":""},{"appId":"68311c5f-823f-434d-8f59-64ef3141323d","appName":"中洋鱼天下","icon":""}],"isSuccess":true,"Code":0,"Message":"Read Success"}
                this.shopList =data.appInfoList;
                if(data.comdtyList.length){
                    this.isEmpty = false;
                    this.goodsLists.push.apply(this.goodsLists,data.comdtyList);
                    for(var i = 0; i < this.goodsLists.length; i++){
                        if(this.goodsLists[i].Id){
                            this.goodsLists[i].href =  url + "?commodityId=" + this.goodsLists[i].Id + RequestUrlParam();
                        }
                    }
                    if(data.comdtyList.length < this.pageSize){
                        this.tips = "已经看到最后一页了~";
                        this.endPage = true;
                    }
                }else{
                    if(this.goodsLists.length == 0){
                        this.isEmpty = true;
                    }else{
                        this.isEmpty = false;
                        this.tips = "已经看到最后一页了~";
                    }
                    this.endPage = true;
                }
            });
        },
        /*排序类型*/
        sortTypeChange:function($index,type){
            if(this.curFilterBar !== $index || $index == 2){
                this.curFilterBar = $index;
                switch ($index){
                    case 0://默认
                        this.fieldSort = 0;
                        this.order = '';
                        break;
                    case 1://销量
                        this.fieldSort = 2;
                        this.order = '';
                        break;
                    case 2://价格
                        this.fieldSort = 1;
                        this.order = type ? 2 : 1;
                        break;
                }
                this.currentPage = 1;
                this.loadData();
            }
        },
        layoutClick:function(state){
            this.horizontal = state;
        },
        /*确定筛选*/
        filterOk:function(param){
            this.filterStock = param.showStock;
            this.filterMinPrice = Number(param.minPrice);
            this.filterMaxPrice = Number(param.maxPrice);
            this.filterAppId = param.selectAppId || '';
            this.currentPage = 1;
            this.loadData();
        }
    }
});
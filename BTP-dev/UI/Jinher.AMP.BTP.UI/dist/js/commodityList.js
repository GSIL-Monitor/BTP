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
        order: '', //1：升序（默认） 0：降序
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
        handleEvent(){
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
                "areaCode": getCookie("selectCityCode") || '',
                "AppId": getQueryString('shopId'),
                "PageIndex": this.currentPage,//当前页数
                "PageSize": this.pageSize, //每页数据条数
                "FieldSort": this.fieldSort,//排序类型
                "OrderState": this.order,//升降序
                "IsHasStock": this.filterStock,//仅看有货
                "MinPrice": this.filterMinPrice,//最低价
                "MaxPrice": this.filterMaxPrice,//最高价
                "filter.appId": this.filterAppId //所选的店铺App Id
            };
            this.$http.post("/Mobile/GetCommodityListV2ForCoupon?r=" + Math.random(),postdata,{emulateJSON:true}).then(function(response){
                this.$loading.close();
                this.shopList = response.data.appInfoList;
                if(response.data.comdtyList && response.data.comdtyList.length){
                    this.isEmpty = false;
                    this.goodsLists.push.apply(this.goodsLists, response.data.comdtyList);
                    for(var i = 0; i < this.goodsLists.length; i++){
                        if(this.goodsLists[i].Id){
                            this.goodsLists[i].href =  urlAppendCommonParams('/Mobile/CommodityDetail?commodityId=' + this.goodsLists[i].Id + '&shopId=' + getQueryString('shopId'))
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
                var data = {"isSuccess":true,"Code":0,"Message":"Success","realCount":11,"comdtyList":[{"Name":"禾典长粒香编织袋5kg","Price":38.80,"Id":"54655efe-437f-498e-800d-66b91e1fa25d","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/99910596-4cc9-442a-ac3b-6c6024bb7b83_3548cec1-3b27-4525-9448-70cc50fba9ef.jpg","Intensity":10,"State":0,"Stock":92,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":"禾典大米","MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":"[]","IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":38.8,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0,"SpreadPercent":0},{"Name":"禾典长粒香米5kg","Price":49.00,"Id":"6176c88f-dabf-4248-ac68-547dad9ba5a8","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/2eab5855-f470-418f-b162-91696c17ca60_3c0df46b-3b96-4cc8-830c-a2a36cf1f288.jpg","Intensity":10,"State":0,"Stock":439,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":"禾典大米","MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":"[]","IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":49.0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0,"SpreadPercent":0},{"Name":"禾典珍珠米5kg","Price":46.00,"Id":"367e5f3f-deb2-4532-abd7-e39b77b09a03","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/821bd682-2ffd-48c0-9a61-9e9b0ec68720_501f64ad-0c50-44b1-b532-380b5257f19a.jpg","Intensity":10,"State":0,"Stock":622,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":"禾典大米","MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":"[]","IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":46.0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0,"SpreadPercent":0},{"Name":"禾典稻花香米5kg","Price":78.00,"Id":"37f09bb1-0fda-485d-8f54-ff3decd20e20","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/0703b6f9-a734-4af2-8f6f-7a16949ade05_9eb30470-21c1-41a4-b5e0-b75a5fb47588.jpg","Intensity":10,"State":0,"Stock":13,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":"禾典大米","MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":"[]","IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":78.0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0,"SpreadPercent":0},{"Name":"禾典长粒香米908g","Price":12.80,"Id":"0751fd04-324b-4969-bd1e-a676132dd74c","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/a55abd20-c815-4759-a2ee-2020c79a7855_c953a40f-9fbf-4c7b-bc30-4dea4abb3795.jpg","Intensity":10,"State":0,"Stock":14,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":"禾典大米","MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":"[]","IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":12.8,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0,"SpreadPercent":0},{"Name":"杂粮礼盒10种真空杂粮4KG","Price":98.00,"Id":"0d2c5919-ce18-4ba8-bbe5-ce62c7f138df","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/1fd737b4-2776-4a97-bbc6-d7b90223c94f_5371cd93-79d2-4240-b92e-034429c522d7.jpg","Intensity":10,"State":0,"Stock":44,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":"禾典大米","MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":"[]","IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":98.0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0,"SpreadPercent":0},{"Name":"2016年秋收新米黑龙江大米禾典5KG长粒香礼盒装","Price":98.00,"Id":"83c7a65e-fb5a-49b8-85d0-e42f3005b145","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/a38c8bc8-bfb5-4394-9942-886e185fb292_620fe0c6-42f4-40b2-ad98-6a042d7a922b.jpg","Intensity":10,"State":0,"Stock":45,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":"禾典大米","MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":"[]","IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":98.0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0,"SpreadPercent":0},{"Name":"禾典一品真空米砖5kg","Price":99.00,"Id":"36671e5e-15d2-4faa-8284-aa0f3b5dcf13","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/45d8aa75-2664-45dd-98fe-61d1dc99e1ad_f77d64f0-831e-4ffd-bec5-9e8c531e95ba.jpg","Intensity":10,"State":0,"Stock":64,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":"禾典大米","MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":"[]","IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":99.0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0,"SpreadPercent":0},{"Name":"禾典一品真空米砖2.5kg","Price":59.00,"Id":"377b6ea1-5d83-4e4a-b684-e5eec85c213f","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/a7722f8b-ea68-46bf-a7eb-ca80fbd77dce_093b23a0-83ac-4648-90f1-b26541cda24b.jpg","Intensity":10,"State":0,"Stock":86,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":"禾典大米","MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":"[]","IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":59.0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0,"SpreadPercent":0},{"Name":"鸭稻米1kg小盒装+长粒香1kg小盒装组合","Price":79.20,"Id":"d30e7566-affa-4035-9e0b-d43b8f8338a7","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/8e9f2f08-76ab-48c1-ae58-896d38d0480c_6098941a-96ae-4263-917f-ea7a6f41e3fb.jpg","Intensity":10,"State":0,"Stock":96,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":"禾典大米","MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":"[]","IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":79.2,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0,"SpreadPercent":0},{"Name":"2016年秋收新米黑龙江大米禾典5KG鸭稻米礼盒装","Price":298.00,"Id":"f100f31a-ac79-4e32-9826-3100a7339943","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/ebfcfa11-7f30-4047-8c81-3432bbbce633_189fea2c-cc84-44e6-a489-719455d61525.jpg","Intensity":10,"State":0,"Stock":100,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":-1,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":"禾典大米","MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":"[]","IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":298.0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0,"SpreadPercent":0}],"appInfoList":[{"appId":"17e9581b-0a32-4a4e-8190-aab74bd57708","appName":"禾典大米","icon":"http://scfileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29E54E46-3E17-4CA4-8F03-DB71FB8F9663/CustomImg/e00b8004-a286-40a0-9f82-d7146eb637ba_96.png"}]}
                this.shopList = data.appInfoList;
                if(data.comdtyList &&data.comdtyList.length){
                    this.isEmpty = false;
                    this.goodsLists.push.apply(this.goodsLists, data.comdtyList);
                    for(var i = 0; i < this.goodsLists.length; i++){
                        if(this.goodsLists[i].Id){
                            this.goodsLists[i].href =  urlAppendCommonParams('/Mobile/CommodityDetail?commodityId=' + this.goodsLists[i].Id + '&shopId=' + getQueryString('shopId'))
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
                        this.order = type ? 0 : 1;
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
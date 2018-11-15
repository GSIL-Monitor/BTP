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
        filterStock: false, //仅看有货
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
                "cgyId": getQueryString('cgyId') || '',
                "userId": getQueryString('userId') || '',
                "pageIndex": this.currentPage,//当前页数
                "pageSize": this.pageSize, //每页数据条数
                "saleArea": getCookie("selectCityCode"),
                "fieldSort": this.fieldSort,//排序类型
                "orderBy": this.order,//升降序
                "isHasStock": this.filterStock,//仅看有货
                "minPrice": this.filterMinPrice,//最低价
                "maxPrice": this.filterMaxPrice,//最高价
                "appRowId": this.filterAppId //所选的店铺App Id
            };
            this.$http.post("/Category/GetTLevelCategoryCommodityList?r=" + Math.random(),postdata,{emulateJSON:true}).then(function(response){
                this.$loading.close();
                this.shopLists = response.data.comdtyAppList;
                if(response.data.Data.length){
                    this.isEmpty = false;
                    this.goodsLists.push.apply(this.goodsLists, response.data.Data);
                    for(var i = 0; i < this.goodsLists.length; i++){
                        if(this.goodsLists[i].Id){
                            this.goodsLists[i].href =  url + "?commodityId=" + this.goodsLists[i].Id + RequestUrlParam();
                        }
                    }
                    if(response.data.Data.length < this.pageSize){
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
                var data = {"Success":true,"Data":[{"RankNo":0,"Name":"禾典长粒香编织袋5kg","Price":38.80,"Id":"54655efe-437f-498e-800d-66b91e1fa25d","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/99910596-4cc9-442a-ac3b-6c6024bb7b83_3548cec1-3b27-4525-9448-70cc50fba9ef.jpg","Intensity":10,"State":0,"Stock":92,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"禾典长粒香米5kg","Price":49.00,"Id":"6176c88f-dabf-4248-ac68-547dad9ba5a8","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/2eab5855-f470-418f-b162-91696c17ca60_3c0df46b-3b96-4cc8-830c-a2a36cf1f288.jpg","Intensity":10,"State":0,"Stock":439,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"【双节特惠】景泓苑黑蒜浆石磨挂面有机杂粮面条300g 河北农家特产 满108包邮","Price":6.20,"Id":"d368518d-d2ba-4ee7-a354-cfc7326b5173","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017042615/f042c403-6205-4528-bbac-981ff9a21687_d40af1fb-cd16-4718-9e66-38880e3ced92.jpg","Intensity":10,"State":0,"Stock":309,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":6.50,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"禾典珍珠米5kg","Price":46.00,"Id":"367e5f3f-deb2-4532-abd7-e39b77b09a03","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/821bd682-2ffd-48c0-9a61-9e9b0ec68720_501f64ad-0c50-44b1-b532-380b5257f19a.jpg","Intensity":10,"State":0,"Stock":622,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"禾典稻花香米5kg","Price":78.00,"Id":"37f09bb1-0fda-485d-8f54-ff3decd20e20","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/0703b6f9-a734-4af2-8f6f-7a16949ade05_9eb30470-21c1-41a4-b5e0-b75a5fb47588.jpg","Intensity":10,"State":0,"Stock":13,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"【双节特惠】河北景泓苑石磨黑小麦面粉 饺子粉无任何添加剂5公斤装 满108包邮","Price":70.00,"Id":"2894f64b-fc5a-443b-b183-f510bcd1ee3b","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017042615/aab4f7bc-2b99-491f-b281-391b657ed3c4_b3540bae-771a-4a35-ac96-cc34d115c610.jpg","Intensity":10,"State":0,"Stock":48,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":72.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"禾典长粒香米908g","Price":12.80,"Id":"0751fd04-324b-4969-bd1e-a676132dd74c","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/a55abd20-c815-4759-a2ee-2020c79a7855_c953a40f-9fbf-4c7b-bc30-4dea4abb3795.jpg","Intensity":10,"State":0,"Stock":14,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"【双节特惠】御嘉谷小米 贡米 礼盒装2公斤装 一件包邮","Price":60.00,"Id":"70ea6a49-893d-4b30-ab87-27943badd88e","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017092011/990d7b52-95d6-4c44-b10e-fb36bf6dcd78_10a2f136-9b2b-4a8c-b11d-975193f23342.jpg","Intensity":10,"State":0,"Stock":156,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":78.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":3,"ScorePercent":0},{"RankNo":0,"Name":"【双节特惠】新疆和田塔里木明珠骏枣二级 500g 满5袋包邮 自提免邮费 限地区","Price":48.00,"Id":"d3cc9a7b-12e5-465e-aa45-b96e779fdf6a","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017092016/806d4045-5479-4ba5-891f-d5de8c31339d_0364803a-561a-42e8-b5c0-db9ffd7b5ab8.jpg","Intensity":10,"State":0,"Stock":70,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":60.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"杂粮礼盒10种真空杂粮4KG","Price":98.00,"Id":"0d2c5919-ce18-4ba8-bbe5-ce62c7f138df","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/1fd737b4-2776-4a97-bbc6-d7b90223c94f_5371cd93-79d2-4240-b92e-034429c522d7.jpg","Intensity":10,"State":0,"Stock":44,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"威龙有机干红葡萄酒 月亮系书写心意 官方正品","Price":142.80,"Id":"24fd62c7-4beb-4fbe-8c7e-b11c3e9f95dc","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017060114/d6f771c7-2ad8-4b0b-9812-c72efe39652f_29e3b01f-59f5-4df3-bbc7-82bac6bede6e.jpg","Intensity":10,"State":0,"Stock":115,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"20f36d0e-0f2d-448c-9561-b46294ed9f83","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":168.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"2016年秋收新米黑龙江大米禾典5KG长粒香礼盒装","Price":98.00,"Id":"83c7a65e-fb5a-49b8-85d0-e42f3005b145","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/a38c8bc8-bfb5-4394-9942-886e185fb292_620fe0c6-42f4-40b2-ad98-6a042d7a922b.jpg","Intensity":10,"State":0,"Stock":45,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"【双节特惠】新疆和田塔里木明珠骏枣四级 500g 满5袋包邮 自提免邮费 限地区","Price":28.00,"Id":"637beca3-998a-4566-ada0-40c0f174bcdf","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017092016/55434997-c73d-4fe0-bfb6-94f4d447f375_69561193-bd54-4a48-ad36-b10b7d1768cf.jpg","Intensity":10,"State":0,"Stock":103,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":40.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"【双节特惠】五常稻花香大米 礼盒内装5块真空米砖（5公斤）满10公斤11大省包邮（京津苏浙皖沪鲁冀黑辽吉）","Price":135.00,"Id":"f392e61c-cd87-4471-aefe-fa741e26ef0e","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017092010/e3ef28eb-4fae-4814-838b-73f53a439884_0982911b-3c4f-4ec6-a382-9d617e011de0.jpg","Intensity":10,"State":0,"Stock":56,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":140.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"【双节特惠】 贝斯特玫瑰酱200ml装 满200包邮","Price":40.00,"Id":"5fae31a9-2a22-440d-9a9b-2294afd3cea7","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017092014/4915ae09-11a1-4f0e-b36a-d4c9c19c5b99_ab245827-0d97-4266-a26a-25c73b542b41.jpg","Intensity":10,"State":0,"Stock":56,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":40.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":3,"ScorePercent":0},{"RankNo":0,"Name":"威龙葡萄酒 冰川白 夏日必备 官方正品","Price":68.00,"Id":"da19f6dc-f309-4c60-a9bc-53592a2f19da","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017041909/0efa7127-b81b-4758-809a-997492184ab4_153814fd-f506-4e94-9486-fa75e98436e9.jpg","Intensity":10,"State":0,"Stock":107,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"20f36d0e-0f2d-448c-9561-b46294ed9f83","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":128.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":3,"ScorePercent":0},{"RankNo":0,"Name":"【双节特惠】金蒜能 独头黑蒜 350g 罐装 河北特产 满108包邮","Price":62.00,"Id":"f3593ea3-6158-4adf-aa25-55c1752b9e3c","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017042614/25bf5a3d-a786-40b0-987d-7356dd5646f7_d6d5c8ac-2028-4195-9781-5220afe28c4f.jpg","Intensity":10,"State":0,"Stock":164,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":65.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"【双节特惠】新疆和田塔里木明珠灰枣 500g 满5袋包邮 自提免运费 限地区","Price":48.00,"Id":"0b78f9f5-d0b2-4831-9fa9-ed3d89e962db","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017092016/6c57d17c-9194-4ae3-815d-3d457068176b_baed7a87-0b3e-4e8f-bfae-5e64e49ad6aa.jpg","Intensity":10,"State":0,"Stock":109,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":60.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"禾典一品真空米砖5kg","Price":99.00,"Id":"36671e5e-15d2-4faa-8284-aa0f3b5dcf13","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017021416/45d8aa75-2664-45dd-98fe-61d1dc99e1ad_f77d64f0-831e-4ffd-bec5-9e8c531e95ba.jpg","Intensity":10,"State":0,"Stock":64,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"17e9581b-0a32-4a4e-8190-aab74bd57708","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":null,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0},{"RankNo":0,"Name":"【双节特惠】新疆和田塔里木明珠骏枣三级 500g 满5袋包邮 自提免运费 限地区","Price":38.00,"Id":"9cafa59c-1533-44a2-a314-83c02288a7d5","Pic":"http://btp.fileserver.iuoooo.com/Jinher.JAP.BaseApp.FileServer.UI/FileManage/GetFile?fileURL=29e54e46-3e17-4ca4-8f03-db71fb8f966e/2017092016/1bbbde16-6b41-47b5-8db6-c0b72e868836_f28f96b2-8404-4dfa-b734-48aaef4963b3.jpg","Intensity":10,"State":0,"Stock":114,"DiscountPrice":-1,"LimitBuyEach":-1,"LimitBuyTotal":-1,"SurplusLimitBuyTotal":0,"AppId":"7fdcf05a-780a-44ae-a914-d554e13d211f","IsActiveCrowdfunding":false,"AppName":null,"MarketPrice":50.00,"IsEnableSelfTake":0,"ComAttribute":null,"IsMultAttribute":false,"SharePercent":0,"CategoryId":"00000000-0000-0000-0000-000000000000","CommodityStocks":null,"ComAttibutes":null,"RealPrice":0,"OriPrice":null,"ComPromotionStatusEnum":0,"PromotionType":9999,"ComAttrType":1,"ScorePercent":0}],"comdtyAppList":[{"appId":"00000000-0000-0000-0000-000000000000","appName":"全部","icon":""},{"appId":"17e9581b-0a32-4a4e-8190-aab74bd57708","appName":"禾典大米","icon":null},{"appId":"7fdcf05a-780a-44ae-a914-d554e13d211f","appName":"景泓苑","icon":null},{"appId":"20f36d0e-0f2d-448c-9561-b46294ed9f83","appName":"威龙葡萄酒","icon":null}]}
                this.shopLists =data.comdtyAppList;
                if(data.Data.length){
                    this.isEmpty = false;
                    this.goodsLists.push.apply(this.goodsLists, data.Data);
                    for(var i = 0; i < this.goodsLists.length; i++){
                        if(this.goodsLists[i].Id){
                            this.goodsLists[i].href =  url + "?commodityId=" + this.goodsLists[i].Id + RequestUrlParam();
                        }
                    }
                    if(data.Data.length < this.pageSize){
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
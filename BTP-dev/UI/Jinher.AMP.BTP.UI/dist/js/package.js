window.SYS = (function(mod,undefined){
    /*根据宽度计算字体大小*/
    mod.calculateFont = function(){
        var deviceWidth = document.documentElement.clientWidth >=500 ? 500: document.documentElement.clientWidth;
        document.documentElement.style.fontSize = deviceWidth / 6.4 +'px';//如果设计图是320的话就除以3.2
    };
    return mod;
})(window.SYS ||{});
var vm = new Vue({
    el: '#app',
    mounted:function(){
        this.$nextTick(function(){
            SYS.calculateFont();
            this.loadDate();
            window.onresize = function () {
                SYS.calculateFont();
            };
        });
    },
    data: function(){
        return{
            popupShow: false,
            attributes: {},//商品属性
            selectSize: {},//选择的商品属性
            color:[],//color属性
            size: [],//size属性
            colorTitle: '',//color属性title
            sizeTitle: '',//size属性title
            colorSelect: null,
            sizeSelect: null,
            curSelectPrice: 0,
            defaultPrice: 0,
            curSelectPic: '',
            singleAttribute: null,//true：单属性 ;false: 多属性
            lists:[], //优惠套餐列表
            currentPage: 1,
            pageSize: 20,
            seckillLists: [], //套餐数据
            tabLists:[] //套餐标题
       }
    },
    methods:{
        /**
         * 获取数据
         */
        loadDate: function(){
            $.ajax({
                url: '/Mobile/GetSetMealActivitysByCommodityId?r=' + Math.random(),
                type: 'GET',
                dataType: 'json',
                data: {
                    appId: getEsAppId(),
                    commodityId: getQueryString("commodityId")
                },
                async: true
            }).done(function (data) {
                if (data.Result) {
                    if (data.Result.length) {
                        var html = "";
                        this.lists = data.Result;
                        for (var b = 0; b < this.lists.length; b++) {
                            if (b === 0) {
                                html += '<li id=' + b + ' class="tap-list active"  onclick="changeTab(' + b + ')">' + this.lists[b].Theme + '</li>';
                            } else {
                                html += '<li id=' + b + ' class="tap-list"  onclick="changeTab(' + b + ')">' + this.lists[b].Theme + '</li>';
                            }
                        }

                        var shtml = "";
                        var curData = this.lists[0];
                        var commodityIds = new Array();
                        var scommodityIds = new Array();
                        for (var b = 0; b < curData.SetMealItemsCdtos.length; b++) {
                            if ($.inArray(curData.SetMealItemsCdtos[b].CommodityId, commodityIds) === -1) {
                                shtml += "<div class=\"package-list\"><div class=\"package-list-left\"><img src=" + curData.SetMealItemsCdtos[b].CommodityDto.Pic + "></div>";
                                shtml += "<div class=\"package-list-right\"><div class=\"package-list-title\">" + curData.SetMealItemsCdtos[b].CommodityDto.Name + "</div>";
                                shtml += "<div id=" + curData.SetMealItemsCdtos[b].CommodityId + " class=\"package-list-size\" style=\"display:none\">选择商品属性</div>";
                                shtml += "<div class=\"package-list-price\"><span class=\"text-red\">¥ <em class=\"price\">" + (curData.SetMealItemsCdtos[b].CommodityDto.Price - curData.SetMealItemsCdtos[b].PreferentialPrice).toFixed(2) + "</em></span> <span class=\"pull-right\">数量：1</span></div></div></div>";
                                commodityIds.push(curData.SetMealItemsCdtos[b].CommodityId);
                            } else {
                                scommodityIds.push(curData.SetMealItemsCdtos[b].CommodityId);
                            }
                        }
                        $('#packageList').html(shtml);
                        $('#lists').html(html);

                        for (var b = 0; b < scommodityIds.length; b++) {
                            $('#' + scommodityIds[b]).show();
                        }
                        $('#oprice').text(curData.OPrice.toFixed(2));  
                        $('#preferentialPrice').text(curData.PreferentialPrice.toFixed(2));
                    }
                } 
                $('#mask').addClass('hide');
            }).fail(function (err) {
                //获取数据失败的操作
                toast('服务器繁忙，请稍后重试!');
                $('#mask').addClass('hide');
            });
        },
        showPrice: function(price){
            if(typeof price == undefined || price === '' || price == null){return};
            var arr = (price - 0).toFixed(2).split('.');
            var str = '&#165; <em class="price">' + arr[0] +'.</em>' + arr[1];
            return str;
        },
        /**
         *点击属性弹出属性选择弹窗
         */
        sizeClick: function(list){
            this.popupShow = true;
            this.curSelectPic = list.Pic;
            this.defaultPrice = list.Price;
            this.transformAttributeTitle(list);
            this.color = [];
            this.size = [];
            if(this.singleAttribute == false){//两个属性
                for(var i = 0; i < list.CommodityStocks.length;i++){
                    var commodityStocks = list.CommodityStocks[i];
                    if(commodityStocks.ComAttribute[0].Attribute == this.colorTitle){
                        this.transformAttribute(this.color,commodityStocks.ComAttribute[0],commodityStocks.ComAttribute[1],commodityStocks);
                        this.transformAttribute(this.size,commodityStocks.ComAttribute[1],commodityStocks.ComAttribute[0],commodityStocks);
                    }else{
                        this.transformAttribute(this.color,commodityStocks.ComAttribute[1],commodityStocks.ComAttribute[0],commodityStocks);
                        this.transformAttribute(this.size,commodityStocks.ComAttribute[0],commodityStocks.ComAttribute[1],commodityStocks);
                    }
                }
            }else if(this.singleAttribute == true){//单属性
                if(list.CommodityStocks && list.CommodityStocks.length){
                    for(var i = 0; i < list.CommodityStocks.length;i++){
                        var commodityStocks = list.CommodityStocks[i];
                        this.transformAttribute(this.color,commodityStocks.ComAttribute[0],null,commodityStocks);
                    }
                }
            }
            var size = list.Size.split(',');
            this.initAttr(size);
        },
        initAttr: function(size){
            if(this.singleAttribute){
                var flag = false;
                for(var i = 0; i < this.color.length; i++){
                    var title = size[0] || size[1];
                    if(this.color[i].title == title && !this.color[i].disabled){
                        this.colorSelect = title;
                        this.curSelectPrice = this.color[i].RealPrice;
                        flag = true;
                    }
                }
                if(!flag){
                    this.curSelectPrice = this.defaultPrice;
                }
            }else if(!this.singleAttribute){
                var flag = false;
                var otherAttr = [];
                for(var i = 0; i < this.color.length; i++){
                    var index = size.indexOf(this.color[i].title);
                    if(index > -1){
                        if(this.color[i].disabled){
                            flag = true;
                        }else{
                            this.updateAttribue(size[index],this.color[i].attribute,this.size);
                            this.colorSelect = size[index];
                            otherAttr = this.color[i];
                        }
                    }
                }
                for(var i = 0; i < this.size.length; i++){
                    var index = size.indexOf(this.size[i].title);
                    if(index > -1){
                        if(this.size[i].disabled){
                            flag = true;
                        }else{
                            this.updateAttribue(size[index],this.size[i].attribute,this.color);
                            this.sizeSelect = size[index];
                        }
                    }
                }
                if(!flag && otherAttr.attribute.length){
                    for(var i = 0; i < otherAttr.attribute.length; i++){
                        if(otherAttr.attribute[i].title == size[1]){
                            this.curSelectPrice = otherAttr.attribute[i].RealPrice;
                        }
                    }
                }
            }
        },
        transformAttribute(data,comAttribute1,comAttribute2,commodityStocks){
            if(comAttribute2 == null){//单属性
                var flag = commodityStocks.Stock > 0 ? false : true;
                var obj = {'title': comAttribute1.SecondAttribute,disabled: flag,'RealStock': commodityStocks.Stock,'RealPrice': commodityStocks.DiscountPrice};
                this.color.push(obj);
            }else{//多属性
                var flag = false;
                var attriObj = {'title': comAttribute2.SecondAttribute,'RealStock': commodityStocks.Stock,'RealPrice': commodityStocks.DiscountPrice};
                var len = data.length;
                for(var j = 0; j< len;j++){
                    if(data[j].title == comAttribute1.SecondAttribute){
                        data[j].attribute.push(attriObj);
                        flag = true;
                        break;
                    }
                }
                if(!flag){
                    data[len] = {'title': comAttribute1.SecondAttribute,'attribute':[attriObj]};
                }
                for(var i = 0; i < data.length; i++){
                    var attribute = data[i].attribute;
                    var flag = true;
                    for(var k = 0; k < attribute.length; k++){
                        if(attribute[k].RealStock > 0){
                            flag = false;
                            break;
                        }
                    }
                    this.disabled(data,data[i].title,flag);
                }
            }
        },
        transformAttributeTitle: function(attri){
            var arr =[];
            if(attri.ComAttibutes && attri.ComAttibutes.length){
                for(var i = 0; i < attri.ComAttibutes.length; i++){
                    var title = attri.ComAttibutes[i].Attribute;
                    if(arr.indexOf(title) < 0){
                        arr.push(title);
                    }
                }
            }
            if(arr.length >=2){ //两个属性
                this.colorTitle = arr[0];
                this.sizeTitle = arr[1];
                this.singleAttribute = false;
            }else{ //一个属性
                this.singleAttribute = true;
                this.colorTitle = arr[0];
                this.sizeTitle = '';
            }
        },
        //点击属性按钮
        attributeClick: function(data,state){
            if(!data.disabled){
                if(state == 'color'){
                    if(data.title == this.colorSelect){
                        this.colorSelect = null;
                    }else{
                        this.colorSelect = data.title;
                    }
                    if(this.singleAttribute){
                        this.curSelectPrice = data.RealPrice;
                    }else{
                        this.updateAttribue(this.colorSelect,data.attribute,this.size);
                        if(this.colorSelect !== null && this.sizeSelect !== null){
                            this.updatePrice(this.color,this.colorSelect,this.sizeSelect);
                        }
                    }
                }else{
                    if(data.title == this.sizeSelect){
                        this.sizeSelect = null;
                    }else{
                        this.sizeSelect = data.title;
                    }
                    this.updateAttribue(this.sizeSelect,data.attribute,this.color);
                    if(this.colorSelect !== null && this.sizeSelect !== null){
                        this.updatePrice(this.size,this.sizeSelect,this.colorSelect);
                    }
                }
            }
        },
        updateAttribue: function(stateSelect,attri,otherAttri){
            if(stateSelect == null){//取消选中
                for(var i = 0; i < otherAttri.length; i++){
                    var flag = true;
                    for(var j = 0; j < otherAttri[i].attribute.length;j++){
                        if(otherAttri[i].attribute[j].RealStock > 0){
                            flag = false;
                            break;
                        }
                    }
                    this.disabled(otherAttri,otherAttri[i].title,flag);
                    this.curSelectPrice = this.defaultPrice;
                }
            }else{ //选中
                for(var i = 0; i < attri.length; i++){
                    if(attri[i].RealStock <=0){
                        this.disabled(otherAttri,attri[i].title,true);
                    }else{
                        this.disabled(otherAttri,attri[i].title,false);
                    }
                }
            }
        },
        updatePrice :function(attr,selectAttr1,selectAttr2){
            for(var i = 0; i < attr.length; i++){
                if(attr[i].title == selectAttr1){
                    var attribute = attr[i].attribute;
                    for(var j =0; j < attribute.length; j++){
                        if(attribute[j].title == selectAttr2){
                            this.curSelectPrice = attribute[j].RealPrice;
                        }
                    }
                }
            }
        },
        //设置禁用状态
        disabled: function(data,cur,state){
            for(var i =0; i < data.length; i++){
                if(data[i].title == cur){
                    data[i].disabled = state;
                }
            }
        },
        //点击确定按钮
        submit: function(){
            if(this.colorSelect == null){
                this.$toast('请选择' + this.colorTitle);
                return;
            }
            if(!this.singleAttribute && this.sizeSelect == null){
                this.$toast('请选择' + this.sizeTitle);
                return;
            }

            this.closeModal();
        },
        closeModal: function(){
            this.popupShow = false;
            this.colorSelect = null;
            this.sizeSelect = null;
        }
    },
    filters:{
        priceFilter: function(value){
            if(typeof value == 'undefined' || value == null) return;
            return '￥' + eval(value).toFixed(2);
        },
        sizeFilter:function(value){
            if(value == ""){
                return '选择商品属性'
            }else{
                var arr = value.split(',');
                return arr.join(' ');
            }
        }
    }
});
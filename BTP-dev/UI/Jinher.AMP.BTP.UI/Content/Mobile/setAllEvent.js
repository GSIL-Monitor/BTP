$.fn.clicks = function () {
    var tag;
    var callback;
    if (arguments.length > 1) {
        tag = arguments[0];
        callback = arguments[1];
    } else {
        tag = '';
        callback = arguments[0];
    }

    var startSize;
    this.on('touchstart', tag, function (e) {
        startSize = e.changedTouches[0].pageY;
        e.stopPropagation();
        e.stopPropagation();
        //		ajaxLoading('111', '');
    }).on('touchend', tag, function (e) {
        e.stopPropagation();
        e.preventDefault();
        var self = $(this);
        //			if(self.hasClass('clicks_lock')) {
        ////				return this;
        //			} else {
        if (Math.abs(startSize - e.changedTouches[0].pageY) < 10) {
            //					self.addClass('clicks_lock');
            callback(this, e);
        }
        //			}
        //			ajaxLoading('111', '');
    });
    return this;
};

var goTop; //回到顶部按钮
var showGoTop = false; //回到顶部按钮状态
var goTopAdvance = 1500; //回到顶部按钮显示时机
var scrollLoading = false;
var advance = 500; //距离底部30px时开始加载
var hasLazyLoadSet = false;
var hasWantLazyLoadSet = false;


var appId = '11111111-1111-1111-1111-111111111111';
sessionStorage.page = !sessionStorage.page ? sessionStorage.page = 1 : '';
sessionStorage.categoryId = '';
sessionStorage.categoryLock = '';
sessionStorage.want = '';
sessionStorage.wantLock = '';
var maxHeight = 0;

if (getQueryString('pageIndex') != "" && getQueryString('pageIndex') != null) {
    sessionStorage.page = getQueryString('pageIndex');
} else {
    sessionStorage.page = 1;
}

if (getQueryString('pageSize') != "" && getQueryString('pageSize') != null) {
    sessionStorage.pageSize = getQueryString('pageSize');
} else {
    sessionStorage.pageSize = 10;
}

if (getQueryString('fieldSort') != "" && getQueryString('fieldSort') != null) {
    sessionStorage.fieldSort = getQueryString('fieldSort');
} else {
    sessionStorage.fieldSort = 0;
}
if (getQueryString('order') != "" && getQueryString('order') != null) {
    sessionStorage.order = getQueryString('order');
} else {
    sessionStorage.order = 1;
}

$(function () {
    goTop = $(".u-backtop"); //回到顶部按钮
    //设置元素盒子最小高度
    $('#main').css({
        minHeight: $(window).height() + $('#footer').height()
    });
});


/**
* ajax loading 图片
* @param id                默认基础上添加的区别ID
* @param insertElement     插入那个Element元素名称.可输入   #id   .class  不传入该值时.默认插入body内.并生成一个蒙版.
*/
function ajaxLoading(id, insertElement) {
    //ajaxLoading盒子ID对象
    var loading = $('#ajaxLoading_' + id);
    //蒙版
    var blind = $('<div></div>');
    //是否有插入对象.有插入对象则不显示蒙版
    var insertElements = insertElement ? $(insertElement) : blind;

    //蒙版相关css
    blind.css({
        'position': 'fixed',
        'z-index': '98',
        'opacity': 0.2,
        'backgroundColor': '#ccc',
        'height': '100%',
        'width': '100%',
        'top': 0,
        'left': 0
    });

    //蒙版ID值
    blind.attr('id', 'ajaxLoadBlind');

    //判断是否有自定义插入对象.当没有是插入body最后位置
    if (!insertElement) {
        !$('#ajaxLoadBlind')[0] ? $('body').append(blind) : '';
    }

    //生成loading图片对象
    if (!loading.attr('id')) {
        loading = $('<div></div>');
        loading.attr('id', 'ajaxLoading_' + id);
        loading.css({
            'position': 'absolute',
            'z-index': '99',
            'left': '50%',
            'margin-left': '-16px',
            'top': '50%',
            'margin-top': '-16px'
        });

        loading.append(createImg());

        insertElement ? insertElements.css({ 'position': 'relative' }) : '';
        insertElements.append(loading);
        var windowParent = $(window.parent);
        //		!insertElement ? loading.css({
        //			top: loading.css({ top: (windowParent.height() / 2) - 16 + windowParent.scrollTop() })
        //		}) : '';

    } else {
        !insertElement ? $('#ajaxLoadBlind').remove() : loading.remove();
    }

    //创建图片对象.
    function createImg() {
        var img = $('<img>');
        img.attr('src', '/Content/images/ajax-loader.gif');
        return img;
    }
}


//获取URL某值
function getQueryString(name) {
    var r;
    if (arguments.length > 1) {
        r = arguments[1].split('?')[1];
    } else {
        r = window.location.search.substr(1);
    }
    //	var r = str ? str.split('?')[1] : '' || window.location.search.substr(1);
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    r = r.match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

//获取商品详细信息
function getCommodityInfo(document, callback) {
    sessionStorage.commodityUpInfo = '';
    //判断是否缓存了appId值.当没有时从地址栏获取值并缓存
    //if(!sessionStorage.appId) {
    sessionStorage.appId = getQueryString('appId');
    //}

    //判断是否缓存了商品ID值.当没有时从地址栏获取并缓存
    //if(!sessionStorage.commodityId_2) {
    sessionStorage.commodityId_2 = getQueryString('commodityId');
    //}

    //设置获取商品详情请求参数
    var data = {
        appId: sessionStorage.appId || getQueryString('appId'),
        commodityId: sessionStorage.commodityId_2 || getQueryString('commodityId'),
        userId: sessionStorage.userId
    };

    //判断当前商品是否有缓存.有则从缓存中读取.
    var commodityInfo = sessionStorage.commodityInfo ? JSON.parse(sessionStorage.commodityInfo) : {};

    //判断当前商品ID是否和缓存的商品id相同.
    //判断是否获取最新的或读取缓存
    if (commodityInfo.Id == sessionStorage.commodityId_2 && sessionStorage.commodityId_2) {
        //读取缓存
        setData(JSON.parse(sessionStorage.commodityInfo));
    } else {
        //获取数据.
        getDataAjax({
            url: '/Mobile/GetCommodityDetails',
            data: data,
            callback: function (data) {
                ajaxLoading('22', '');
                setData(data);
            },
            beforeSend: function () {
                ajaxLoading('22', '');
            }
        });
    }

    //设置商品详情使用函数
    function setData(data) {
        sessionStorage.appId = data.AppId;

        var img = $('<img/>');                                  //生成一个img对象
        var imgBox = document.find('#img');                     //获取传入对象内的img对象.
        var imgList = imgBox.find('ul');                        //获取ul
        var property = document.find('#property');              //获取商品属性盒子
        var content_box = document.find('#content_box');        //商品详情与留言盒子
        var content_2_clone = document.find('#content_2_clone'); //留言盒子单个对象
        var addCommodity = document.find('#addCommodity');      //点击购买时弹出商品尺寸与颜色盒子
        var tmpImg;                                             //临时img对象
        var colorAndSize = (function () {                      //颜色与尺寸对象.
            var tmp = { '尺寸': [], '颜色': [] };
            if (data.ComAttibutes) {
                for (var i = 0; i < data.ComAttibutes.length; i++) {
                    if (data.ComAttibutes[i].SecondAttribute) {
                        tmp[data.ComAttibutes[i].Attribute].push(data.ComAttibutes[i].SecondAttribute);
                    }
                }
            }
            return tmp;
        })();
        var LiBox = $('<li></li>');                             //li对象
        var tmpObj = {};                                        //临时obj

        sessionStorage.Stock = data.Stock;
        sessionStorage.State = data.State;

        //设置尺寸盒子对象
        var sizeBox = addCommodity.find('.size').find('ul').empty().on("click",'li', function (e) {
            var self = $(e);
            self.parent().find('li').removeClass('li_focus_1');
            self.addClass('li_focus_1');
            //设置初始值
            setSessionStorage('commodityUpInfo', 'size', self.text());
        });
        //设置颜色盒子对象
        var colorBox = addCommodity.find('.color').find('ul').empty().on("click",'li', function (e) {
            var self = $(e);
            self.parent().find('li').removeClass('li_focus_1');
            self.addClass('li_focus_1');
            //设置初始值
            setSessionStorage('commodityUpInfo', 'color', self.text());
        });

        //缓存当前数据.
        sessionStorage.commodityInfo = JSON.stringify(data);

        //设置商品名称
        imgBox.find('.title').find('p').text(data.Name);

        //设置价格 优惠价显示优惠价
        sessionStorage.DiscountPrice = -1;
        if (data.DiscountPrice > 0) {
            sessionStorage.DiscountPrice = data.DiscountPrice;
            property.find('.type_1').text(getCookie('Currency') + Math.abs(data.DiscountPrice).toFixed(2));
        }
        else {
            property.find('.type_1').text(getCookie('Currency') + round2((data.Price * (data.Intensity / 10)), 2));
        }

        //设置颜色
        if (colorAndSize['颜色'].length) {
            property.find('.type_2').text(colorAndSize['颜色']);
            //设置购买时颜色框
            for (var i = 0; i < colorAndSize['颜色'].length; i++) {
                tmpObj = LiBox.clone().text(colorAndSize['颜色'][i]);
                if (!i) {
                    tmpObj.addClass('li_focus_1');
                    setSessionStorage('commodityUpInfo', 'color', tmpObj.text());
                }
                colorBox.append(tmpObj);
            }
        } else {
            property.find('.type_2').parent('li').hide();
            addCommodity.find('.color').hide();
        }
        //设置尺寸
        if (colorAndSize['尺寸'].length) {
            property.find('.type_3').text(colorAndSize['尺寸']);
            //设置购买时尺寸框
            for (var i = 0; i < colorAndSize['尺寸'].length; i++) {
                tmpObj = LiBox.clone().text(colorAndSize['尺寸'][i]);
                if (!i) {
                    tmpObj.addClass('li_focus_1');
                    setSessionStorage('commodityUpInfo', 'size', tmpObj.text());
                }
                sizeBox.append(tmpObj);
            }
        } else {
            property.find('.type_3').parent('li').hide();
            addCommodity.find('.size').hide();
        }
        //设置库存 需要判断是否促销的逻辑
        if (data.LimitBuyTotal > -1 && data.LimitBuyTotal) {
            sessionStorage.Stock = parseInt(data.LimitBuyTotal) - parseInt(data.SurplusLimitBuyTotal);
        }
        else {
            sessionStorage.Stock = data.Stock > 0 ? data.Stock : '已售完';

        }
        property.find('.type_4').text(sessionStorage.Stock);
        //设置出售数量
        property.find('.type_5').text(data.Total);
        //设置收藏数
        property.find('.type_6').text(data.CollectNum);
        //设置商品详情
        var contents = $(data.Description);
        contents.find('img').attr('width', '').attr('height', '').css('width', '100%');
        content_box.find('.content_1').html(contents);
        //设置评价数
        document.find('#nav').find('b').text(data.ReviewNum);
        window.ReviewNum = data.ReviewNum;

        //遍历头部图片.
        for (var i = 0; i < data.Pictures.length; i++) {
            //			if (i == 0) {
            //				tmpImg = img.clone().attr('src', data.Pictures[i].PicturesPath);
            //				tmpImg[0].onload = function () {
            //					callback();
            //				};
            //				imgList.append(tmpImg);
            //			} else {
            imgList.append(img.clone().attr('src', data.Pictures[i].PicturesPath))
            //			}
        }


        //设置图片高度与宽度
        tmpImg = img.clone().attr('src', '/Content/Mobile/2.jpg?' + new Date().getTime());
        tmpImg[0].onload = function () {
            callback();
        };

        //将商品名称.价格.折扣.图片缓存到commodityUpInfo内
        setSessionStorage('commodityUpInfo', 'name', data.Name);
        if (data.DiscountPrice > 0 && data.DiscountPrice) {
            setSessionStorage('commodityUpInfo', 'price', Math.abs(data.DiscountPrice).toFixed(2));
        }
        else {
            setSessionStorage('commodityUpInfo', 'price', Math.abs(data.Price * (data.Intensity / 10)).toFixed(2));
        }
        setSessionStorage('commodityUpInfo', 'oldPrice', data.Price);
        setSessionStorage('commodityUpInfo', 'Intensity', data.Intensity);
        setSessionStorage('commodityUpInfo', 'pic', data.Pic);

        //设置加减数量按钮事件
        numberEvent();

    }
}

//设置.sessionStorage值
function setSessionStorage(key, p_key, value) {
    //临时对象
    var data;
    //判断是否有当前缓存值
    if (!sessionStorage[key]) {
        //没有则生成一个空对象
        sessionStorage[key] = '{}';
        //将data赋值为空对象
        data = {};
    } else {
        //有则将缓存的值赋值为data
        data = JSON.parse(sessionStorage[key]);
    }

    //重新赋值
    data[p_key] = value;
    //更新缓存
    sessionStorage[key] = JSON.stringify(data);
}

//数量加减事件
function numberEvent() {
    //加减盒子对象
    var box = $('#number');
    //数字对象
    var number = box.find('.span_number');
    //当前数字
    var text = parseInt(number.text());
    //获取传入参数
    var f_arguments = arguments;
    //更新缓存数据
    setSessionStorage('commodityUpInfo', 'number', text);

    box.on("click",'.span_2', function () {//设置减事件,并更新缓存
        if (text - 1 <= 0) {
            text = 1;
        } else {
            text -= 1;
        }

        text = sessionStorage.Stock != '已售完' ? text : '已售完';

        number.text(text);
        setSessionStorage('commodityUpInfo', 'number', text);
        f_arguments.length ? f_arguments[0](text) : '';
    }).on("click",'.span_3', function () {//设置加事件,并更新缓存
        number.text(text < sessionStorage.Stock ? text += 1 : sessionStorage.Stock);
        setSessionStorage('commodityUpInfo', 'number', text);
        f_arguments.length ? f_arguments[0](text) : '';
    });
}

//获取商品留言
function getCommodityInfoReplays(obj) {
    !window.reviewByCommodity ? window.reviewByCommodity = [] : '';
    var content_box = $('#content_box');                    //留言盒子父对象
    var content_2 = content_box.find('.content_2');         //留言盒子对象
    var content_2_clone = $('#content_2_clone');            //留言盒子克隆对象
    var replay_clone = content_2_clone.find('.replays__');  //留言盒子留言条数对象
    var tmpClone;                                           //临时克隆对象
    var tmpReplayClone;                                     //临时留言条目对象

    if (obj.type) {
        content_2.empty()
    }
    //提交查询数据
    var data = {
        appId: sessionStorage.appId || getQueryString('appId'),
        commodityId: sessionStorage.commodityId_2 || getQueryString('commodityId'),
        lastReviewTime: ''
    };

    //是否有缓存过当前商品评价对象
    //没有则获服务器数据
    if (window.reviewByCommodity && window.reviewByCommodity.length == window.ReviewNum) {
        content_2.find('.content_2_clones').remove();
        setData(window.reviewByCommodity);
    } else {
        if (window.getAjax_2) {
            window.getAjax_2.abort();
            getAjax();
        } else {
            getAjax();
        }
    }

    function getAjax() {
        window.getAjax_2 = getDataAjax({
            url: '/Mobile/GetReviewByCommodityId',
            data: obj.inData || data,
            beforeSend: function () {
            },
            callback: function (data) {
                if (data.length) {
                    window.reviewByCommodity = window.reviewByCommodity.concat(data);
                    setData(data);
                }
            }
        });
    }

    //设置盒子内容方法
    function setData(data) {
        //遍历设置留言数据
        for (var i = 0; i < data.length; i++) {
            tmpClone = content_2_clone.clone().removeClass('noDisplay').removeAttr('id');
            tmpClone.find('.replays_1').text(data[i].Name.replace(/null/gi, ''));
            tmpClone.find('.replays_2').text(data[i].Details);
            tmpClone.find('.replays_3').text(data[i].ShowTime);
            tmpClone.find('.replays_4').text('尺码/颜色 | ' + data[i].Size ? data[i].Size.replace(/null/gi, '') : '');
            for (var b = 0; b < data[i].Replays.length; b++) {
                tmpReplayClone = replay_clone.clone().removeClass('noDisplay').removeAttr('id');
                tmpReplayClone.find('.replays_5').text(data[i].Replays[b].ReplyerName + ': ');
                tmpReplayClone.find('.replays_6').text(data[i].Replays[b].Details);
                tmpReplayClone.find('.replays_7').find('span').text(data[i].Replays[b].ShowTime);
                tmpClone.find('.left_3').append(tmpReplayClone);
            }
            data[i].UserHead ?
				data[i].UserHead != 'null' ? tmpClone.find('.replays_8').attr('src', data[i].UserHead) : ''
			: '';
            content_2.append(tmpClone);
        }

        if (window.ReviewNum >= 10 && window.reviewByCommodity.length != window.ReviewNum) {
            var timeData = data.pop();
            var time = timeData.SubTime.match(/\d/g).join('');
            time = new Date(Number(time));
            //			var div = $('#content_2_').show();
            var div = $('<div id="content_2_" style="display: block">展开更多>></div>');
            div.data('time', time.getUTCFullYear() + '-' + timeData.ShowTime);
            content_2.append(div);
            //			obj.className = 'focus';
        }
    }

    content_2.on("click",'#content_2_', function (e) {
        var div = $(e);
        getCommodityInfoReplays({ 'inData': {
            appId: sessionStorage.appId || getQueryString('appId'),
            commodityId: sessionStorage.commodityId_2 || getQueryString('commodityId'),
            lastReviewTime: div.data('time')
        }
        });
        div.remove();
    });
}

//封装ajax方法 get
function getDataAjax(obj) {
    return $.ajax({
        url: obj.url,
        type: 'get',
        contentType: "application/json",
        data: obj.data,
        success: obj.callback,
        beforeSend: obj.beforeSend,
        error: obj.error,
        dataType: 'json'
    })
}

//复制        post
function getDataAjax2(obj) {
    $.ajax({
        url: obj.url,
        type: 'post',
        contentType: "application/json",
        data: obj.data,
        success: obj.callback,
        beforeSend: obj.beforeSend,
        error: obj.error,
        dataType: 'json'
    })
}

function getDataAjax3(obj) {
    return $.ajax({
        url: obj.url,
        type: 'post',
        //		contentType: "application/json",
        data: obj.data,
        success: obj.success,
        beforeSend: obj.beforeSend,
        error: obj.error,
        dataType: 'json'
    })
}

//获取商品列表 ajax
function getCommodity(obj) {

    csson($("#cdefault"));
    var data = {
        setCategoryId: sessionStorage.categoryId,
        pageIndex: sessionStorage.page,
        pageSize: 20,
        fieldSort: sessionStorage.fieldSort,
        order: sessionStorage.order
    };
    return getDataAjax({
        url: '/SetMobile/GetCommodity',
        //		data: '{"appId":"' + obj.appId + '","pageIndex":' + obj.pageIndex + ',"pageSize":' + obj.pageSize + '}',
        data: data,
        beforeSend: obj.beforeSend,
        callback: obj.callback
    });
}

//商品列表中设置各个商品相关属性
function setItemData(obj) {
    var clone_item = obj.element;
    var data = obj.data;
    if (window.itemIdList) {
        if (window.itemIdList[data.Id]) {
            return '';
        }
        window.itemIdList[data.Id] = 'on'
    } else {
        window.itemIdList = {};
        window.itemIdList[data.Id] = 'on'
    }
    clone_item.css({
        //height: obj.height
    }).attr('data-id', data.Id);
    clone_item.css({
        //		height: obj.height
    }).attr('appid', data.AppId);
    //	clone_item.find('a')[0].href += '?id=' + data.Id;
    clone_item.find('img').parent().css({ 'height': Math.round(sessionStorage.img_height) + 'px', 'overflow': 'hidden' }).
		find('img').css('height', Math.round(sessionStorage.img_height) + 'px').attr('data-original', data.Pic)
		.addClass('LazyLoad')[0].onerror = function () {
		    console.log(self.css({ background: 'none' }).attr('src', '/Content/Mobile/b_2.png'));
		};
    clone_item.find('.item_title').text(data.Name);
    var isyou = false;
    if (data.IsActiveCrowdfunding === true) {
        clone_item.find('.zcz').show();
    }
    if (data.DiscountPrice > 0) {
        clone_item.find('.price_1').text(getCookie('Currency') + Math.abs(data.DiscountPrice).toFixed(2));
        clone_item.find('.price_2').text(getCookie('Currency') + Math.abs(data.Price).toFixed(2));
        //clone_item.find('.zk').text('优惠价')
        isyou = true;
    }
    else {
        if (data.Intensity == 10) {
            clone_item.find('.price_1').text(getCookie('Currency') + Math.abs(data.Price).toFixed(2));
            if (data.MarketPrice && data.MarketPrice != null && data.MarketPrice != 'null') {
                clone_item.find('.price_2').text(getCookie('Currency') + data.MarketPrice);
            } else {
                clone_item.find('.price_2').hide();
            }
        } else {
            clone_item.find('.price_1').text(getCookie('Currency') + Math.abs(data.Price * (data.Intensity / 10)).toFixed(2));
            clone_item.find('.price_2').text(getCookie('Currency') + Math.abs(data.Price).toFixed(2));

            isyou = true;
        }
        //data.Intensity != 10 ? clone_item.find('.zk').text(data.Intensity + '折') : '';
    }

    var lim = "";
    if (isyou == true) {
        if (data.LimitBuyEach > -1 && data.LimitBuyEach) {
            lim += "每人限购" + data.LimitBuyEach + "件,";
        }
        else {
            lim += "不限购,";
        }
        if (data.LimitBuyTotal > -1 && data.LimitBuyTotal) {
            lim += "还剩" + (parseInt(data.LimitBuyTotal) - parseInt(data.SurplusLimitBuyTotal)) + "件";
        }
        else {
            lim += "还剩" + data.Stock + "件";
        }
        clone_item.find('.yhzknum').text(lim);
    }
    !(data.Stock > 0) ? clone_item.find('.mb_box').removeClass('noDisplay') : '';
    data.State == 1 ? clone_item.find('.mb_box').removeClass('noDisplay').find('.mb_1').text('已下架') : '';
    clone_item.css({ height: sessionStorage.item_height + 'px', overflow: 'hidden' });

    return clone_item;
}

function csson(obj) {

    $(".toporder ul li").each(function (i) {
        $(this).removeClass("topon");
    });
    $(obj).addClass("topon");
};
//商品列表按销量排序
function ComOrderList(fieldSort, order, state) {
    var querydata = {
        setCategoryId: sessionStorage.categoryId,
        pageIndex: sessionStorage.page,
        pageSize: sessionStorage.pageSize,
        fieldSort: fieldSort,
        order: order
    };

    getDataAjax({
        url: '/Mobile/GetOrByCommodity',
        data: querydata,
        callback: function (data) {
            window.itemIdList = {};
            if (state == 1) {
                $("#items").html("");
            }
            sessionStorage.data = JSON.stringify(data);
            sessionStorage.commodityList = JSON.stringify(data);
            var items = $('#items');
            var item = $('#parent_item');
            var height = item.height();
            var img_height = item.find('img').height();
            var clone_item;

            for (var i = 0; i < data.length; i++) {
                clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                //alert(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
                items.append(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
            }
            //					items.append(div);
            $("#ajaxLoadBlind").remove();

            showLazyLoadImg();
            if (data.length >= 10)
            // $('#footer_loading').show().find('span').text('获取更多信息');
                scrollLoading = false;
        },
        beforeSend: function () {
            //                    ajaxLoading('22', '');
        },
        error: function () {
            $("#ajaxLoadBlind").remove();
            scrollLoading = false;
        }
    });
}

//获取商品分类列表
function getNavList() {
    var l_length = 0;
    var data = {
        appId: sessionStorage.appId
    };
    var li = $('<li></li>');
    var nav_list = $('#nav_list');
    if (nav_list.length) {
        getNavObj();
    }

    function getNavObj() {
        nav_list = $('#nav_list');
        setTimeout(function () {
            if (nav_list.length) {
                getNavObj();
            }
        }, 500);
    }

    if (sessionStorage.setCommodityNavList) {
        if (sessionStorage.setCommodityNavList.length) {
            setItem(JSON.parse(sessionStorage.setCommodityNavList));
        } else {
            getAjax();
        }
    } else {
        getAjax();
    }

    function getAjax() {
        getDataAjax({
            url: '/SetMobile/GetCategory',
            data: data,
            callback: function (data) {
                sessionStorage.setCommodityNavList = JSON.stringify(data);

                setItem(data);
            },
            error: function () {
                if (l_length < 3) {
                    l_length++;
                    getAjax();
                }
            }
        });
    }

    function setItem(data) {
        for (var i = 0; i < data.length; i++) {
            var show_item_key = 0;
            nav_list.append(li.clone().addClass('nav_one').
				text(data[i].Name).data('category-id', data[i].Id));

            for (var b = 0; b < data[i].SecondCategory.length; b++) {
                nav_list.append(li.clone().addClass('nav_two nav_inner').
					text(data[i].SecondCategory[b].Name).data('parent-name', data[i].Name).data('show-item', show_item_key)
					.data('category-id', data[i].SecondCategory[b].Id));

                show_item_key++;
            }
            if (show_item_key > 0) {
                $('#nav_list li').each(function () {
                    if ($(this).data('category-id') == data[i].Id)
                        $(this).addClass('no_nav_focus');
                });
            }
        }
    }
}

//关键字查询商品
function getWantCommodityData(want) {
    var items = $('#items');
    var item = $('#parent_item');
    var height = item.height();
    var img_height = item.find('img').height();
    var clone_item;
    var $footer_loading = $('#footer_loading');
    var $top_loading = $('#top_loading');
    var type = false;
    $("#main").show();
    $("#guanggao").hide();
    window.itemIdList = {};

    if (sessionStorage.wantLock != 'true') {
        items.empty();

    }
    sessionStorage.wantLock = '';
    sessionStorage.want = want;
    var data2 = {
        want: want,
        pageIndex: sessionStorage.page,
        pageSize: 10
    };

    getDataAjax({
        url: '/SetMobile/GetWantCommodity',
        data: data2,
        callback: function (data) {
            if (data.length) {
                for (var i = 0; i < data.length; i++) {
                    clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                    items.append(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
                }

                showLazyLoadImg();
                sessionStorage.page++;
            }
            if (data.length < 10) {
                $('#footer_loading').find('span').text('没有更多商品');
            }
            type = true;


            if (!hasWantLazyLoadSet) {
                $("body").picLazyLoad();
                hasWantLazyLoadSet = true;
            }


        }
    });

    return type;
}

//商品列表页首次加载使用函数
//涉及到页面加载时获取第一分页信息.
//设置侧边栏分类信息.
//侧边栏相关事件.
function CommodityList(categoryOnly) {
    $(function () {
        $(window).scroll(function () {
            backToTop();
            scrollEvent();
        });
        //回到顶部
        function backToTop() {

            var scrollTop = $(window).scrollTop();
            //回到顶部
            if (scrollTop > goTopAdvance && showGoTop == false) {
                goTop.addClass("e-backtop-fixed");
                showGoTop = true;
            } else if (scrollTop < goTopAdvance && showGoTop == true) {
                goTop.removeClass("e-backtop-fixed");
                showGoTop = false;
            }
        }
        //向下设置列表分页数据
        function setDownListItem() {

            if (Math.abs(parseInt($('#Commodity_nav').css('left'))) + 5 < Math.round($(window).width() * 0.6)) {
                return 0;
            }
            //window.ajax;
            if (!sessionStorage.want) {
                sessionStorage.page == 1 ? sessionStorage.page++ : '';

                window.ajax = getCommodity({
                    appId: sessionStorage.appId,
                    pageIndex: sessionStorage.page,
                    pageSize: 10,
                    beforeSend: function () {

                    },
                    callback: function (data) {

                        var items = $('#items');
                        var item = $('#parent_item');
                        var height = item.height();
                        var img_height = item.width() / item.find('img').width();
                        var clone_item;
                        sessionStorage.commodityList =
								JSON.stringify(JSON.parse(sessionStorage.commodityList).concat(data));

                        setTimeout(function () {
                            if (data.length) {
                                for (var i = 0; i < data.length; i++) {
                                    clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                                    items.append(setItemData({
                                        element: clone_item,
                                        data: data[i],
                                        height: height,
                                        img_height: img_height
                                    }));
                                }

                                showLazyLoadImg();
                                sessionStorage.page++;
                            } else {
                                $('#footer_loading').find('span').text('没有更多商品');
                            }
                        }, 500);
                        scrollLoading = false;
                    }
                })


            } else if (sessionStorage.want) {
                sessionStorage.wantLock = true;
                getWantCommodityData(sessionStorage.want);
            }
        }
        //滚动逻辑函数
        function scrollEvent() {
            if (scrollLoading == false) {

                var scrollHeight = $(document).height() > $(window).height() ? $(document).height() - $(window).height() : 0;
                var scrollTop = $(window).scrollTop();
                var scrollBottom = scrollHeight - scrollTop;

                //滚动加载
                if (scrollBottom <= advance && maxHeight < scrollHeight) {
                    maxHeight = scrollHeight;

                    scrollLoading = true;


                    var bnext = false;
                    switch (sessionStorage.ComTypeSearch) {
                        case "2":
                            bnext = true;
                            sessionStorage.page++;
                            ComOrderList(1, sessionStorage.PriceState, 0);
                            break;
                        case "3":
                            bnext = true;
                            sessionStorage.page++;
                            ComOrderList(0, 0, 0);
                            break;
                        case "4":
                            bnext = true;
                            sessionStorage.page++;
                            ComOrderList(2, 0, 0);
                            break;
                    }
                    if (bnext == false) {
                        setDownListItem();
                    }
                }
            }
        }

        new TouchMoveEvent();
        //		newTouchMoveEvent();
        navEvent();

        if (sessionStorage.appId && sessionStorage.appId != "undefined" && sessionStorage.appId != "null") {
        }
        else {
            sessionStorage.appId = getQueryString('appId') || appId;
        }

        if (sessionStorage.userId && sessionStorage.userId != "undefined") {
        }
        else {
            if (JsVilaDataNull(getQueryString("user")) && JsVilaDataNull(getQueryString("sessionId"))) {
                sessionStorage.userId = getQueryString('user') || '';
            }
        }

        getNavList();

        if (getQueryString("sortType") == "New" && sessionStorage.ComTypeSearch != 1) {
            sessionStorage.appId = getQueryString('appId');
            csson($("#cnewcom"));
            ajaxLoading('22', '')
            sessionStorage.page = 1;
            sessionStorage.ComTypeSearch = 4;
            ComOrderList(2, 0, 1);
        }
        else {
            if (!categoryOnly) {
                getAjaxData();
            }
        }
        function getAjaxData() {
            getCommodity({
                appId: sessionStorage.appId,
                pageIndex: sessionStorage.page,
                pageSize: 10,
                callback: function (data) {
                    if (data.length === 0) alert('此分类下没有商品');
                    sessionStorage.data = JSON.stringify(data);
                    sessionStorage.commodityList = JSON.stringify(data);
                    var items = $('#items');
                    var item = $('#parent_item');
                    var height = item.height();
                    var img_height = item.find('img').height();
                    var clone_item;

                    for (var i = 0; i < data.length; i++) {
                        clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                        items.append(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
                    }
                    //					items.append(div);
                    showLazyLoadImg();
                    if (data.length < 10) {
                        $('#footer_loading').find('span').text('没有更多商品');
                    }
                    if (getQueryString('promotionId') != '' && getQueryString('promotionId') != undefined) {
                        $('.toporder').addClass('noDisplay');
                    }
                    if (!hasLazyLoadSet) {
                        $("body").picLazyLoad();
                        hasLazyLoadSet = true;
                    }
                }
            });
        }

    });

    var touchLock = true;

    /**
    * 手势事件
    *
    */
    function newTouchMoveEvent() {
        var nav = document.getElementById('Commodity_nav'),
			$nav = $(nav),
			navStartLeft,
			navWidth = $nav.css({ position: "absolute", visibility: "hidden", display: "block" }).width(),
			navMoveAutoMinSize = 20,
			starSize;

        $nav.css({ position: "fixed", visibility: "inherit", display: "none", left: -navWidth });

        touch.config({
            tap: true,                  //tap类事件开关, 默认为true
            doubleTap: true,            //doubleTap事件开关， 默认为true
            hold: true,                 //hold事件开关, 默认为true
            holdTime: 650,              //hold时间长度
            swipe: true,                //swipe事件开关
            swipeTime: 300,             //触发swipe事件的最大时长
            swipeMinDistance: 18,       //swipe移动最小距离
            swipeFactor: 5,             //加速因子, 值越大变化速率越快
            drag: true,                 //drag事件开关
            pinch: true                //pinch类事件开关
        });

        touch.on('body', 'swipestart', function (ev) {
            starSize = ev.x;
            navStartLeft = parseInt(nav.style.left);
        });

        touch.on('body', 'touchmove', function (e) {
            //			e.preventDefault();
        });

        touch.on('body', 'swiping', function (ev) {
            if (Math.abs(ev.x) > navMoveAutoMinSize) {
                nav.style.left = navStartLeft + ev.x - navMoveAutoMinSize + 'px';
                nav.style.display = 'block';
                nav.style.zIndex = '1000';
            }
        });

        touch.on('body', 'swipeend', function (ev) {
            var left = Math.abs(parseInt(nav.style.left));

            if (starSize - ev.x < 0) {
                if (left > navWidth - navMoveAutoMinSize) {
                    nav.style.left = -navWidth + 'px';
                } else {
                    nav.style.left = '0px';
                }
            } else {
                if (left < navMoveAutoMinSize) {
                    nav.style.left = '0px';
                } else {
                    nav.style.left = -navWidth + 'px';
                }
            }
        });




        //获取样式表信息
        function getStyle(obj, styleName) {
            return document.defaultView.getComputedStyle(obj, false)[styleName];
        }
    }

    function TouchMoveEvent() {
        var body = document.getElementsByTagName('body')[0]; //缓存body元素
        var main = document.getElementById('main'); //缓存main元素
        var box = document.getElementById('box'); //缓存box元素
        var nav = document.getElementById('Commodity_nav'); //缓存nav元素
        var top_loading = document.getElementById('top_loading'); //头部加载文字盒子
        var footer_loading = document.getElementById('footer_loading'); //底部加载文字盒子
        var $main = $(main);
        var $body = $(body);
        var $nav = $(nav);
        var $box = $(box);
        var $top_loading = $(top_loading);
        var $footer_loading = $(footer_loading);
        var startPageX = 0; //横轴开始值
        var startPageY = 0; //纵轴开始值
        var top_loading_start_size = 0;
        var moveSize = 0; //横轴移动量
        var moveSizeY = 0; //纵轴移动量
        var moveYMax = 10; //纵轴阀值
        var moveXMax = 10; //横轴阀值
        var startY = document.body.scrollTop; //记录滚动条距离
        var startX = document.body.scrollLeft;
        var startNavLeft = 0;
        var boxLeft = 0; //盒子左边距
        var touchMoveMinSize = 20; //最小移动距离.当到达这个值时将自动移动到touchMoveMaxSize设置的值
        var touchMoveMaxSize = parseInt($nav.width());
        //		var touchMoveMaxSize = $nav.css({position: "absolute", visibility: "hidden", display: "block" }).width(); //最大移动距离.
        //		$nav.css({position: "fixed", visibility: "inherit", display: "block" });
        var lockY = true; //纵轴锁
        var lockX = false; //横走锁
        var lockInnerX = true; //纵轴内部锁.该值为真时才能横向移动
        var mainLock = true; //main元素横向锁
        var navScrollStart = 0; //nav开始值
        var navScrollTop = 0; //nav滚动值
        var navMove = 0; //nav移动量
        var footerLoadingLock = false;
        var topLoadingLock = false;
        var type = {};



        var testBox = $('<div id="testBox"></div>').css({
            position: 'fixed',
            top: 0,
            height: '200px',
            width: '300px',
            left: 0,
            zIndex: 999,
            backgroundColor: '#fff'
        }
		);

        $nav.css({
            //			left: '-' + touchMoveMaxSize + 'px',
            zIndex: 1000
        });

        body.addEventListener('touchstart', function (e) {
            var touch = e.changedTouches[0];
            startPageX = touch.screenX;
            startPageY = touch.screenY;
            boxLeft = parseInt($(box).css('left'));
            box.style.left = boxLeft + 'px';
            startNavLeft = parseInt($nav.css('left'));
            lockX = false;
            lockY = true;
            startY = body.scrollTop;
            top_loading_start_size = parseInt($top_loading.css('margin-top'));
            footerLoadingLock = false;
            topLoadingLock = false;
            //$nav.css({ height: ($(window).height() - 200) });

            if (parseInt($nav.css('left')) > -20) {
                lockY = false;
                //				e.preventDefault();
                lockX = true;
            }

            if ($(document).height() > $(window).height()) {
                type.h = $(document).height() - $(window).height();
            } else {
                type.h = $(window).height() - $(document).height();
            }

            touchMoveMaxSize = parseInt($nav.width());
        });
        body.addEventListener('touchmove', function (e) {
            var touch = e.changedTouches[0];
            moveSize = touch.screenX - startPageX;
            moveSizeY = touch.screenY - startPageY;

            //判断当前纵轴移动量是否超过阀值.则将横轴锁打开
            //				if (Math.abs(moveSizeY) > moveYMax && lockX && lockY) {
            //					lockX = false;
            //				}

            //判断当前横轴移动量是否超过阀值.则将纵轴锁打开
            if (Math.abs(moveSize) > moveXMax && Math.abs(moveSizeY) < moveYMax) {
                lockY = false;
                e.preventDefault();
                lockX = true;
            }

            //纵移动相关
            if (lockY) {
                if (startNavLeft == 0) {
                    $nav.css({
                        left: 0
                    });
                } else {
                    $nav.css({
                        left: -touchMoveMaxSize + 'px'
                    });
                }

                if (startY == 0 && moveSizeY > 0) {
                    var top_move = top_loading_start_size + moveSizeY;
                    if (top_move <= 0) {
                        e.preventDefault();
                        $top_loading.css({
                            marginTop: top_move
                        })
                    } else {
                        $top_loading.css({
                            marginTop: 0
                        }).find('span').text('松开可更新');
                        topLoadingLock = true;
                    }
                }

                if ($body.scrollTop() == type.h) {
                    if (moveSizeY < -10 && $footer_loading.find('span').text() != '正在获取信息...') {
                        //$footer_loading.find('span').text('松开可更新');
                        e.preventDefault();
                        footerLoadingLock = true;
                    }
                } else {
                    //						$footer_loading.find('span').text('获取更多信息');
                }
            }

            //横移动相关
            if (lockX) {
                if (Math.abs(moveSizeY) < 10) {
                    e.preventDefault();
                    $nav.show();
                    var ll = startNavLeft + moveSize;
                    if (ll < 0) {
                        $nav.css({
                            left: startNavLeft + moveSize + 'px'
                        })
                    }
                    body.scrollTop = startY;
                }
            }

        });
        body.addEventListener('touchend', function (e) {
            touchMoveMaxSize = parseInt($nav.width());
            type.moveEnd = new Date().getTime();
            var touch = e.changedTouches[0];
            var mo = startPageX - touch.screenX;
            //				moveSizeY = startPageY - touch.screenY;

            //				if (lockY) {
            //					var item = $('#main').find('.item_1').eq(0);
            var items = $('#items');

            //            if (topLoadingLock) {
            //                sessionStorage.commodityList = '';
            //                setUpListItem();
            //            } else {
            //                $top_loading.css('margin-top', '-' + $top_loading.css('height'));
            //            }

            //            if (footerLoadingLock) {
            //                var bnext = false;
            //                switch (sessionStorage.ComTypeSearch) {
            //                    case "2":
            //                        bnext = true;
            //                        sessionStorage.page++;
            //                        ComOrderList(1, sessionStorage.PriceState, 0);
            //                        break;
            //                    case "3":
            //                        bnext = true;
            //                        sessionStorage.page++;
            //                        ComOrderList(0, 0, 0);
            //                        break;
            //                    case "4":
            //                        bnext = true;
            //                        sessionStorage.page++;
            //                        ComOrderList(2, 0, 0);
            //                        break;
            //                }
            //                if (bnext == false) {
            //                    setDownListItem();
            //                }
            //            }
            //				}

            //横移动相关  nav相关
            if (lockX) {
                var n_left = parseInt($nav.css('left'));

                if (mo > 0) {
                    if (Math.abs(n_left) > 20) {
                        $nav.css('left', -touchMoveMaxSize + 'px')
                        $("#navBackground").remove();
                    } else {
                        $nav.css('left', 0);
                        navBackground();
                    }
                } else {
                    if (Math.abs(n_left) > touchMoveMaxSize - 20) {
                        $nav.css('left', -touchMoveMaxSize + 'px')
                        $("#navBackground").remove();
                    } else {
                        $nav.css('left', 0);
                        navBackground();
                    }

                }
            }

            moveSizeY = 0;
            moveSize = 0;
        });

        //向上获取数据
        function setUpListItem() {
            if (sessionStorage.want) {
                $top_loading.find('span').text('正在获取信息...');
                $('#items').empty();
                sessionStorage.page = 1;
                sessionStorage.wantLock = true;
                getWantCommodityData(sessionStorage.want);

                setTimeout(function () {
                    $top_loading.css('margin-top', '-' + $top_loading.css('height'))
						.find('span').text('下拉可刷新');
                }, 1000);

            } else {
                sessionStorage.page = 1;
                getCommodity({
                    appId: sessionStorage.appId,
                    pageIndex: 1,
                    pageSize: 10,
                    beforeSend: function () {
                        $top_loading.find('span').text('正在获取信息...');
                    },
                    callback: function (data) {
                        //					if (sessionStorage.data != JSON.stringify(data)) {
                        var items = $('#items').empty();
                        var item = $('#parent_item');
                        var height = item.height();
                        var img_height = item.find('img').height();
                        var clone_item;

                        window.itemIdList = {};

                        for (var i = 0; i < data.length; i++) {
                            clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                            items.append(setItemData({
                                element: clone_item,
                                data: data[i],
                                height: height,
                                img_height: img_height
                            }));
                        }

                        $top_loading.find('span').text('已更新');
                        sessionStorage.data = JSON.stringify(data);
                        sessionStorage.commodityList = JSON.stringify(data);
                        sessionStorage.page = 1;
                        setTimeout(function () {
                            $top_loading.css('margin-top', '-' + $top_loading.css('height'))
								.find('span').text('下拉可刷新');
                        }, 1000);
                        $footer_loading.show();
                        //					} else {
                        //						$top_loading.find('span').text('暂无更新');
                        //						setTimeout(function () {
                        //							$top_loading.css('margin-top', '-' + $top_loading.css('height'))
                        //								.find('span').text('下拉可刷新');
                        //						}, 1000);
                        //					}
                    }
                });
            }
        }

        //        $footer_loading.on("click",function () {
        //            if ($footer_loading.find('span').text() != '正在获取信息...') {
        //                var bnext = false;

        //                switch (sessionStorage.ComTypeSearch) {
        //                    case "2":
        //                        bnext = true;
        //                        sessionStorage.page++;
        //                        ComOrderList(1, sessionStorage.PriceState, 0);
        //                        break;
        //                    case "3":
        //                        bnext = true;
        //                        sessionStorage.page++;
        //                        ComOrderList(0, 0, 0);
        //                        break;
        //                    case "4":
        //                        bnext = true;
        //                        sessionStorage.page++;
        //                        ComOrderList(2, 0, 0);
        //                        break;
        //                }
        //                if (bnext == false) {
        //                    setDownListItem();
        //                }
        //            }
        //        });

        //        //向下设置列表分页数据
        //        function setDownListItem() {

        //            if (Math.abs(parseInt($('#Commodity_nav').css('left'))) + 5 < Math.round($(window).width() * 0.7)) {
        //                return 0;
        //            }
        //            window.ajax;
        //            if (!sessionStorage.want) {
        //                sessionStorage.page == 1 ? sessionStorage.page++ : '';
        //                if (lock) {
        //                    if (window.ajax) {
        //                        window.ajax.abort();
        //                        sendAjax();
        //                    } else {
        //                        sendAjax();
        //                    }
        //                }
        //                function sendAjax() {
        //                    window.ajax = getCommodity({
        //                        appId: sessionStorage.appId,
        //                        pageIndex: sessionStorage.page,
        //                        pageSize: 10,
        //                        beforeSend: function () {
        //                            $footer_loading.find('span').text('正在获取信息...');
        //                        },
        //                        callback: function (data) {
        //                            lock = true;
        //                            var items = $('#items');
        //                            var item = $('#parent_item');
        //                            var height = item.height();
        //                            var img_height = item.width() / item.find('img').width();
        //                            var clone_item;
        //                            sessionStorage.commodityList =
        //								JSON.stringify(JSON.parse(sessionStorage.commodityList).concat(data));

        //                            setTimeout(function () {
        //                                if (data.length) {
        //                                    for (var i = 0; i < data.length; i++) {
        //                                        clone_item = item.clone().removeClass('noDisplay').attr('id', '');
        //                                        items.append(setItemData({
        //                                            element: clone_item,
        //                                            data: data[i],
        //                                            height: height,
        //                                            img_height: img_height
        //                                        }));
        //                                    }

        //                                    showLazyLoadImg();

        //                                    sessionStorage.page++;
        //                                    $footer_loading.removeClass('clicks_lock').find('span').text('获取更多信息');
        //                                } else {
        //                                    $footer_loading.removeClass('clicks_lock').find('span').text('没有更多商品');
        //                                }
        //                            }, 500);
        //                            loading = false;
        //                        }
        //                    })
        //                }

        //                lock = false;

        //            } else if (sessionStorage.want) {
        //                sessionStorage.wantLock = true;
        //                getWantCommodityData(sessionStorage.want);
        //            }
        //        }

        /**
        * nav触摸开始事件
        * 设置初始值
        */
        nav.addEventListener('touchstart', function (e) {
            var touch = e.changedTouches[0];
            navScrollStart = touch.pageY;
        });

        /**
        * nav触摸移动事件.
        * 将浏览器默认事件重置.
        * 并模拟内部滚动.
        */
        nav.addEventListener('touchmove', function (e) {
            e.preventDefault();
            var touch = e.changedTouches[0];
            navMove = touch.pageY - navScrollStart;

            nav.scrollTop = navScrollTop - navMove;
        });

        /**
        * nav触摸结束时.将当前高度记录.供下次移动时使用.
        * 并设置全局touchLock锁.
        */
        nav.addEventListener('touchend', function (e) {
            navScrollTop = nav.scrollTop;
            touchLock = !navMove;
            navMove = 0;
        });

        $box.on('click', '.item_1', function (e) {
            if (Math.abs(moveSizeY) < 10 && !lockX) {
                ajaxLoading('22', '');
                var self = $(this);
                var box_2 = $('#box_2');
                sessionStorage.commodityId_2 = self.data('id');
                //				sessionStorage.commodityListScrollTop = document.body.scrollTop;
                //window.location.href = '/Mobile/CommodityDetail?commodityId=' +
                //self.data('id') + '&user=' + sessionStorage.userId + '&type=show&source=' + sessionStorage.source + '&SrcType=' + sessionStorage.SrcType + '&SrcTagId=' + sessionStorage.SrcTagId + '&ShareId=' + sessionStorage.ShareId;

                setTimeout(function () {
                    $("#ajaxLoadBlind").remove();
                }, 1000); 
                window.location.href = '/Mobile/CommodityDetail?commodityId=' +
                    self.data('id') + '&appId=' + self.attr('appid') + '&user=' + sessionStorage.userId + '&type=show&source=' + sessionStorage.source + '&SrcType=' + sessionStorage.SrcType + '&SrcTagId=' + sessionStorage.SrcTagId + '&share=' + getShareId();
            }
        });


    }

    /**
    * nav相关事件
    */
    function navEvent() {
        var psource = sessionStorage.source == "share" ? "&source=share" : "";
        var parentUl;
        var parentUlClone = {};
        $('#Commodity_nav').on('click', '.title', function () {
            window.itemIdList = {};
            if (parentUl) {
                var ul_parent = parentUl.parent();
                ul_parent.find('ul').remove();
                parentUlClone.find('li').removeClass('nav_focus');
                ul_parent.append(parentUlClone);
            } else {
                $(this).parent().find('li').removeClass('nav_focus');
            }
            var items = $('#items').empty();

            getCommodity({
                appId: sessionStorage.appId,
                pageIndex: 1,
                pageSize: 10,
                callback: function (data) {
                    sessionStorage.data = JSON.stringify(data);
                    sessionStorage.categoryId = '';
                    sessionStorage.categoryLock = '';
                    sessionStorage.want = '';
                    sessionStorage.wantLock = '';
                    sessionStorage.commodityList = JSON.stringify(data);
                    sessionStorage.page = 1;
                    var item = $('#parent_item');
                    var height = item.height();
                    var img_height = item.find('img').height();
                    var clone_item;

                    for (var i = 0; i < data.length; i++) {
                        clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                        items.append(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
                    }

                    showLazyLoadImg();

                    if (data.length < 10)
                        $('#footer_loading').find('span').text('没有更多商品');
                    parentUl = '';
                }
            });


        }).on('click', 'li', function () {
            if (touchLock) {
                var self = $(this);
                var parent = self.parent();
                if (!self.hasClass('no_nav_focus')) {
                    parent.find('li').removeClass('nav_focus');
                    if (!self.hasClass('no_nav_focus')) {
                        self.addClass('nav_focus');
                    }

                    sessionStorage.page = 1;
                    sessionStorage.ComTypeSearch = 0;
                    sessionStorage.categoryLock = '';
                    location.href = '/SetMobile/CommodityList?pageIndex=1&pageSize=10&fieldSort=0&order=1&setCategoryId=' + self.data('category-id') + psource;
                    //getCommodityByCategory(self.data('category-id'));
                }
            }
        }).on('keydown', 'input', function (e) {
            var self = $(this);
            if (e.keyCode == 13) {
                sessionStorage.page = 1;
                sessionStorage.categoryLock = '';
                getWantCommodityData(self.val());
                self[0].blur();
            }
        });


    }
}

function showLazyLoadImg() {
    //	$('.LazyLoad').each(function () {
    //		var self = $(this);
    //		self.attr('src', self.attr('data-src')).removeClass('LazyLoad')[0].onerror = function () {
    //			console.log(self.css({background: 'none'}).attr('src', '/Content/Mobile/b_2.png'));
    //		};
    //	});
}

//下订单使用
function createOrder(documents) {
    if (sessionStorage.userId == undefined || sessionStorage.userId == "null") {
        sessionStorage.userId = getQueryString('userId');
    }
    $('#addCommodity').height($(window).height()).find('.addCommodity_1').on("click",function () {
        $('#addCommodity').hide();
    });
    $('#buttonLogin').on("click",function (e) {
        var data = {};
        data.username = $('#username').val();
        data.password = $('#userPassword').val();

        $.get('/mobile/Login', data, function (data) {
            if (data.IsSuccess) {
                sessionStorage.loginUser = data.ContextDTO.LoginUserID;
                upData();
            } else {
                alert(data.Message);
            }
        }, 'json');
    });

    //    var option = $('<option></option>');
    //    var tmpOption;
    if (getQueryString('type') != "gouwuche") {
        var commodityUpInfo = JSON.parse(sessionStorage.commodityUpInfo);
        var comTitle = '';
        if (commodityUpInfo.name != '' && commodityUpInfo.name != undefined) {
            comTitle += commodityUpInfo.name;
        }
        if (commodityUpInfo.color != '' && commodityUpInfo.color != undefined) {
            comTitle += ' ' + commodityUpInfo.color
        }
        if (commodityUpInfo.size != '' && commodityUpInfo.size != undefined) {
            comTitle += ' ' + commodityUpInfo.size
        }
        documents.find('h2').text(comTitle);
        documents.find('.price_make_1').text(Math.abs(commodityUpInfo.price).toFixed(2));
        documents.find('.span_number').text(commodityUpInfo.number);
        var price_make_2 = documents.find('.price_make_2').text(Math.abs(commodityUpInfo.price * commodityUpInfo.number).toFixed(2));
    }
    //    var text_2 = documents.find('.text_2').empty();
    //    var text_3 = documents.find('.text_3').empty();
    //    var text_1 = documents.find('.text_1').empty();
    //    var forNumber = 1;

    var searchData = new GetElementData({
        element: '#main',
        find_class_name: '.search',
        find_class_data_name: 'search'
    });

    //    text_1.on('change', function () {
    //        forNumber = 1;
    //        text_1.find('option').each(function () {
    //            var self = $(this);
    //            var value;
    //            if (self.hasClass('option_clone')) {
    //                self.remove();
    //            }
    //            if (self.prop('selected')) {
    //                value = self.data('code');
    //                var upData = {
    //                    url: '/mobile/PartialProvince',
    //                    data: { provinceCode: value },
    //                    beforeSend: function () {
    //                        text_3.empty();
    //                        text_2.prop('disabled', true).empty().append(option.clone().text('正在获取数据..'));
    //                    },
    //                    success: function (data) {
    //                        text_2.empty();
    //                        setOption(text_2, data);
    //                    },
    //                    error: function () {
    //                        if (forNumber < 3) {
    //                            getDataAjax3(upData);
    //                            forNumber++;
    //                        } else {
    //                            text_2.prop('disabled', true).empty().append(option.clone().text('暂无数据').val('noData'));
    //                        }
    //                    }
    //                };

    //                if (value) {
    //                    getDataAjax3(upData);
    //                } else {
    //                    text_2.empty().prop('disabled', true);
    //                    text_3.empty().prop('disabled', true);
    //                }
    //            }
    //        });
    //    });

    //    text_2.on('change', function () {
    //        forNumber = 1;
    //        $(this).find('option').each(function () {
    //            var value;
    //            if ($(this).prop('selected')) {
    //                value = $(this).data('code');
    //                var upData = {
    //                    url: '/mobile/PartialCity',
    //                    data: { cityCode: value },
    //                    beforeSend: function () {
    //                        text_3.empty();
    //                        text_3.prop('disabled', true).append(option.clone().text('正在获取数据..'));
    //                    },
    //                    success: function (data) {
    //                        text_3.empty();
    //                        setOption(text_3, data);
    //                    },
    //                    error: function () {
    //                        if (forNumber < 3) {
    //                            getDataAjax3(upData);
    //                            forNumber++; D
    //                        } else {
    //                            text_3.prop('disabled', true).empty().append(option.clone().text('暂无数据').val('noData'));
    //                        }
    //                    }
    //                };
    //                if (value) {
    //                    getDataAjax3(upData);
    //                } else {
    //                    text_3.empty().prop('disabled', true);
    //                }
    //                //				$.post('/mobile/PartialCity', {cityCode: value}, function (data) {
    //                //					text_3.empty();
    //                //					setOption(text_3, data);
    //                //				}, 'json');
    //                return false;
    //            }
    //        });
    //    });

    numberEvent(function (text) {
        var number = commodityUpInfo.price * text;
        documents.find('.price_make_2').text(Math.abs(number).toFixed(2));
    });

    //    getDataAjax3({
    //        url: '/mobile/GetProvince',
    //        success: function (data) {
    //            ajaxLoading('111', '');
    //            setOption(text_1, data, function () {
    //                setOneSelectOption();
    //            });
    //        },
    //        beforeSend: function () {
    //            ajaxLoading('111', '');
    //        }
    //    });

    //    function setOption(element, data) {
    //        element.prop('disabled', false);
    //        if (data.length) {
    //            for (var i = 0; i < data.length; i++) {
    //                if (data[i].Name) {
    //                    tmpOption = option.clone().val(data[i].Name).text(data[i].Name).data('code', data[i].Code);
    //                    if (data[i].Name == '请选择') {
    //                        tmpOption.prop('selected', true);
    //                    }
    //                    element.append(tmpOption);
    //                }
    //            }
    //        }

    //        if (arguments.length > 2) {
    //            arguments[2]();
    //        }
    //    }

    //    searchData.verificationData(function (data, index) {
    //        switch (index) {
    //            case 'number':
    //                return data == 0 ? '数量不能为0' : '';
    //            case 'name':
    //                return !data ? '收货人姓名不能为空!' : '';
    //            case 'phone_number':
    //                if (!data.match(/^1/) || 11 != data.length) {
    //                    return '请填写正确手机号!';
    //                } else if (!data) {
    //                    return '手机号不能为空!';
    //                } else {
    //                    return '';
    //                }
    //            case 'text_1':
    //                return !data || data == '请选择' ? '请选择省份!' : '';
    //            case 'text_2':
    //                return !data || data == '请选择' ? '请选择城市!' : '';
    //            case 'text_3':
    //                return !data || data == '请选择' ? '请选择地区!' : '';
    //            case 'text_4':
    //                return !data ? '详细地址不能为空!' : '';
    //            default:
    //                return '';
    //        }
    //    });

    //    var elementList = searchData.getElementList();

    //    function setOneSelectOption() {
    //        var inData = {};
    //        if (sessionStorage.createOrderData) {
    //            var s_data = JSON.parse(sessionStorage.createOrderData);
    //            inData = {
    //                username: s_data.name || '',
    //                phone: s_data.phone_number || '',
    //                province: s_data.text_1 || '',
    //                city: s_data.text_2 || '',
    //                district: s_data.text_3 || '',
    //                address: s_data.text_4 || ''
    //            };
    //            setData(inData);
    //        } else {
    //            $.get('/mobile/GetDeliveryAddress', { userId: sessionStorage.userId, appId: sessionStorage.appId }, function (data) {
    //                inData = {
    //                    username: data[0] != undefined ? data[0].ReceiptUserName : '',
    //                    phone: data[0] != undefined ? data[0].ReceiptPhone : '',
    //                    province: data[0] != undefined ? data[0].Province : '',
    //                    city: data[0] != undefined ? data[0].City : '',
    //                    district: data[0] != undefined ? data[0].District : '',
    //                    address: data[0] != undefined ? data[0].ReceiptAddress : ''
    //                };
    //                setData(inData);
    //            }, 'json');
    //        }

    //        function setData(data) {
    //            for (var i in elementList) {
    //                switch (i) {
    //                    case 'name':
    //                        elementList[i][0].value = data.username;
    //                        break;
    //                    case 'phone_number':
    //                        elementList[i][0].value = data.phone;
    //                        break;
    //                    case 'text_1':
    //                        elementList[i].append(option.clone().prop('disabled', true).text(data.province).val(data.province).addClass('option_clone')
    //							.attr('selected', 'selected'));
    //                        break;
    //                    case 'text_2':
    //                        elementList[i].prop('disabled', true).append(option.clone().text(data.city).val(data.city).addClass('option_clone'));
    //                        break;
    //                    case 'text_3':
    //                        elementList[i].prop('disabled', true).append(option.clone().text(data.district).val(data.district).addClass('option_clone'));
    //                        break;
    //                    case 'text_4':
    //                        elementList[i][0].value = data.address;
    //                        break;
    //                    default:
    //                        break;
    //                }
    //            }
    //        }

    //    }


    var sessionData = JSON.parse(sessionStorage.commodityUpInfo);
    var postData = {};

    postData.orderSDTO = {
        AppId: sessionStorage.appId || getQueryString('appId'),
        City: "",               //城市
        District: "",           //地区
        Price: "",              //总价格
        Province: "",           //省
        ReceiptAddress: "",     //收货人地址
        ReceiptPhone: "",       //收货人手机号
        ReceiptUserName: "",    //收货人姓名
        ShoppingCartItemSDTO: [ //商品信息
			{
			CommodityNumber: "", //商品数量
			Id: sessionStorage.commodityId_2,         //商品ID
			Intensity: sessionData.Intensity,  //折扣
			Name: sessionData.name,       //商品名称
			Pic: sessionData.pic,        //商品图片
			Price: sessionData.oldPrice,      //商品单价
			SizeAndColorId: (sessionData.color ? sessionData.color : '') + ',' + (sessionData.size ? sessionData.size : ''), //尺寸颜色
			UserId: sessionStorage.userId || getQueryString('userId') || "00000000-0000-0000-0000-000000000000",     //用户Id
			ShopCartItemId: "00000000-0000-0000-0000-000000000000",
			RealPrice: sessionData.price,
			DiscountPrice: sessionStorage.DiscountPrice





}
		],
        UserId: '',             //用户Id
        Details: "",             //订单备注
        SrcTagId: "00000000-0000-0000-0000-000000000000"
    };

    function upData() {
        ajaxLoading(2, '');
        var getData = searchData.getAjaxData();
        var commodityUpInfo = JSON.parse(sessionStorage.commodityUpInfo);
        postData.orderSDTO.City = $(".scity").html();
        postData.orderSDTO.District = $(".sdistrict").html();
        postData.orderSDTO.Price = getData.price_2;
        postData.orderSDTO.Province = $(".sprovince").html();
        postData.orderSDTO.ReceiptAddress = $(".spadress").html();
        postData.orderSDTO.ReceiptPhone = $("#hiddphone").val();
        postData.orderSDTO.ReceiptUserName = $(".spusername").html();
        postData.orderSDTO.RecipientsZipCode = $(".spzipcode").html();
        postData.orderSDTO.ShoppingCartItemSDTO[0].CommodityNumber = getData.number;
        postData.orderSDTO.ShoppingCartItemSDTO[0].Price = commodityUpInfo.oldPrice; //传入原价 * 数量
        postData.orderSDTO.ShoppingCartItemSDTO[0].UserId = sessionStorage.userId || getQueryString('userId') || sessionStorage.loginUser;
        postData.orderSDTO.Details = $("#txtdetails").val();
        postData.orderSDTO.UserId = sessionStorage.userId || getQueryString('userId') || sessionStorage.loginUser;
        if (sessionStorage.CPSId) {
            postData.orderSDTO.CPSId = sessionStorage.CPSId;

        }

        if (sessionStorage.SrcType) {
            postData.orderSDTO.SrcType = sessionStorage.SrcType;
        }

        if (sessionStorage.SrcTagId) {
            postData.orderSDTO.SrcTagId = sessionStorage.SrcTagId;
        }
        postData.orderSDTO.ShareId = getShareId();
      
        if (sessionStorage.IsHYL) {
            if (sessionStorage.SrcTagId) {
                postData.orderSDTO.SrcTagId = sessionStorage.SrcTagId;
            }
            if (postData.orderSDTO.SrcTagId == "" || postData.orderSDTO.SrcTagId == null || postData.orderSDTO.SrcTagId == undefined || postData.orderSDTO.SrcTagId == "null") {
                postData.orderSDTO.SrcTagId = "00000000-0000-0000-0000-000000000000";
            }
            getDataAjax2({
                url: '/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/SavePrizeCommodityOrder',
                data: JSON.stringify(postData),
                callback: function (data) {
                    var psource = sessionStorage.source == "share" ? "&source=share" : "";
                    if (data.Message == 'Success') {
                        sessionStorage.orderInfo = JSON.stringify(data);
                        commodityUpInfo.priceAll = getData.price_2;
                        commodityUpInfo.Details = getData.text_5;
                        sessionStorage.commodityUpInfo = JSON.stringify(commodityUpInfo);
                        sessionStorage.createOrderData = JSON.stringify(getData);

                        setTimeout(function () {
                            //						ajaxLoading(2, '');
                            window.location.href = '/Mobile/PaymentHYL?appId=' + sessionStorage.appId + psource;
                        }, 1000);
                    } else {
                        ajaxLoading(2, '');
                        alert(data.Message);
                    }
                }
            });
        }
        else {
            if (postData.orderSDTO.SrcTagId == "" || postData.orderSDTO.SrcTagId == null || postData.orderSDTO.SrcTagId == undefined || postData.orderSDTO.SrcTagId == "null") {
                postData.orderSDTO.SrcTagId = "00000000-0000-0000-0000-000000000000";
            }
            getDataAjax2({
                url: '/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/SaveCommodityOrder',
                data: JSON.stringify(postData),
                callback: function (data) {
                    var psource = sessionStorage.source == "share" ? "&source=share" : "";
                    if (data.Message == 'Success') {
                        sessionStorage.orderInfo = JSON.stringify(data);
                        commodityUpInfo.priceAll = new Number((getData.price_2 * 1) + (data.Freight * 1)).toFixed(2);
                        commodityUpInfo.Details = getData.text_5;
                        sessionStorage.commodityUpInfo = JSON.stringify(commodityUpInfo);
                        sessionStorage.createOrderData = JSON.stringify(getData);

                        setTimeout(function () {
                            //						ajaxLoading(2, '');
                            window.location.href = '/Mobile/Payment?appId=' + sessionStorage.appId+psource;
                        }, 1000);
                    } else {
                        ajaxLoading(2, '');
                        alert(data.Message);
                    }
                    setTimeout(function () {
                        $("#ajaxLoadBlind").remove();
                    }, 2000);
                },
                beforeSend: function () {
                    ajaxLoading('22', '');
                },
                error: function () {
                    $("#ajaxLoadBlind").remove();
                }
            });
        }
    }


    $('.box_2').on("click",function () {
        if ($(".devk").html() != null && $(".devk").html() != "") {
            return false;
        }
        //判断地址 是否因为 异常原因加载失败了
        if ($(".spadress").html() == "" || $(".spusername").html() == "" || $("#hiddphone").val() == "") {
            return false;
        }
        //        var error = searchData.getErrorList();

        //        if (error.length) {
        //            error[0].element.focus();
        //            alert(error[0].error);
        //            //			upData();
        //        } else {
        //			if (!getQueryString('userId') || !sessionStorage.loginUser || !sessionStorage.userId) {
        //				$('#addCommodity').show();
        //			} else {
        //				upData();
        //			}

        var data = {
            appId: sessionStorage.appId || getQueryString('appId'),
            commodityId: sessionStorage.commodityId_2 || getQueryString('commodityId'),
            userId: sessionStorage.userId
        };
        //获取数据.
        getDataAjax({
            url: '/Mobile/GetCommodityDetails',
            data: data,
            callback: function (data) {
                var span_number = $('.span_number').text();
                if (data.Stock <= 0 || span_number > data.Stock) {
                    alert('库存不足.不能购买');
                } else if (data.State == 1) {
                    alert('商品已下架.不能购买');
                } else if (data.State == 3) {
                    alert('商品已删除.不能购买');
                } else {
                    upData();
                }
            },
            beforeSend: function () {
            }
        });
        //        }


        //		$.post('/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/SaveCommodityOrder', JSON.stringify(postData), function () {
        //
        //		});
        //		window.location.href = '/Mobile/Payment';
    });
}

function setSessionStorageItemHeightAndImgHeight() {
    var imgs = document.createElement('img');
    imgs.src = '/Content/Mobile/1.png?' + new Date().getTime();
    imgs.onload = function () {
        var parent_item = $('#parent_item').clone().show();
        $('body').append(parent_item);
        sessionStorage.img_height = Math.round(parent_item.width() / this.width * this.height);
        sessionStorage.item_height = parent_item.height() + 3;
        parent_item.remove();
    };
}

//获取订单列表 ajax
function getOrder(obj) {

    var data = {
        userId: obj.userId,
        pageIndex: obj.pageIndex,
        pageSize: obj.pageSize
    };
    return getDataAjax({
        url: '/Mobile/GetOrder',
        //		data: '{"appId":"' + obj.appId + '","pageIndex":' + obj.pageIndex + ',"pageSize":' + obj.pageSize + '}',
        data: data,
        beforeSend: obj.beforeSend,
        callback: obj.callback
    });
}


//商品列表中设置各个商品相关属性
function setOrderItemData(obj) {
    var clone_item = obj.element;
    var clone_orderItem = obj.ordeItem;
    var data = obj.data;
    //if (window.itemIdList) {
    //    if (window.itemIdList[data.Id]) {
    //        return '';
    //    }
    //    window.itemIdList[data.Id] = 'on'
    //} else {
    //    window.itemIdList = {};
    //    window.itemIdList[data.Id] = 'on'
    //}
    clone_item.css({
        //		height: obj.height
    }).attr('data-id', data.CommodityOrderId);
    //	clone_item.find('a')[0].href += '?id=' + data.Id;
    //clone_item.find('img').parent().css({'height': Math.round(sessionStorage.img_height) + 'px','overflow': 'hidden'}).
    //    find('img').css('height', Math.round(sessionStorage.img_height) + 'px').attr('src', data.Pic)
    //    .addClass('LazyLoad')[0].onerror = function () {
    //    console.log(self.css({background: 'none'}).attr('src', '/Content/Mobile/b_2.png'));
    //};
    clone_item.find('a').attr('href', 'MyOrderDetail?orderId=' + data.CommodityOrderId + '&userId=' + sessionStorage.userId + '&appId=' + data.AppId);


    if (data.ShoppingCartItemSDTO && data.ShoppingCartItemSDTO.length) {
        var html = "";
        for (var i = 0; i < data.ShoppingCartItemSDTO.length; i++) {

            var cartItem = data.ShoppingCartItemSDTO[i];

            if (i == 0) {
                html += "<li>";
            }
            html += "<div style=\"margin-top:5px;\"><img src='" + cartItem.Pic + "' alt=\"\" width=\"60\" height=\"60\">";
            html += " <div class=\"title\">" + cartItem.Name + "</div></div><div style=\"clear:both;\"></div>";
            if (i == data.ShoppingCartItemSDTO.length - 1) {
                if (data.State == 0) {
                    html += "<span  class=\"con\">待付款</span>";
                    // clone_item.find('.con').text("待付款");
                }
                else if (data.State == 1) {
                    html += "<span  class=\"con\">待发货</span>";
                    //clone_item.find('.con').text("待发货");
                }
                else if (data.State == 2) {
                    html += "<span  class=\"con\">已发货</span>";
                    //clone_item.find('.con').text("已发货");
                }
                else if (data.State == 3) {
                    html += "<span  class=\"con\">交易成功</span>";
                    //clone_item.find('.con').text("交易成功");
                }
                else if (data.State == 4) {
                    html += "<span  class=\"con\">交易失败</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 5) {
                    html += "<span  class=\"con\">交易失败</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 6) {
                    html += "<span  class=\"con\">交易关闭</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 7) {
                    html += "<span  class=\"con\">已退款</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 8) {
                    html += "<span  class=\"con\">退款中</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 9) {
                    html += "<span  class=\"con\">退款中</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 10) {
                    html += "<span  class=\"con\">退款中</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 11) {
                    html += "<span  class=\"con\">待发货</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 12) {
                    html += "<span  class=\"con\">金和处理退款中</span>";
                    //clone_item.find('.con').text("交易失败");





















































                }
                html += "</li>";
            }




        }
        clone_item.find('.list').prepend(html)
    }


    return clone_item;
}

function OrderList() {
    $(function () {
        //new TouchMoveEvent();
        ////		newTouchMoveEvent();
        //navEvent();

        //sessionStorage.userId = getQueryString('userId') || '';

        //getNavList();

        if (getQueryString('type') == "shuaxin") {
            sessionStorage.orderList = false;
        }
        if (sessionStorage.orderList) {

            if (JSON.parse(sessionStorage.orderList).length) {
                (function () {
                    var items = $('#orderItems');
                    var item = $('#order');
                    var orderItem = $('#orderItem');
                    var clone_item;
                    var clone_orderItem;
                    var data = JSON.parse(sessionStorage.orderList);

                    for (var i = 0; i < data.length; i++) {
                        clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                        clone_orderItem = orderItem.clone().removeClass('noDisplay').attr('id', '');

                        items.append(setOrderItemData({ element: clone_item, data: data[i], ordeItem: clone_orderItem }));
                    }
                    showLazyLoadImg();
                    if (data.length < 10)
                        $('#footer_loading').find('span').text('没有更多商品');
                })();
            } else {

                getAjaxData();
            }
        } else {

            getAjaxData();
        }


        //getAjaxData();

        function getAjaxData() {
            getOrder({
                userId: sessionStorage.userId,
                pageIndex: sessionStorage.page,
                pageSize: 10,
                callback: function (data) {
                    if (data.length > 0) {
                        $('#noOrder').hide();
                        sessionStorage.orderList = JSON.stringify(data);
                        var items = $('#orderItems');
                        var item = $('#order');
                        var orderItem = $('#orderItem');
                        var clone_item;
                        var clone_orderItem;

                        for (var i = 0; i < data.length; i++) {
                            clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                            clone_orderItem = orderItem.clone().removeClass('noDisplay').attr('id', '');
                            items.append(setOrderItemData({ element: clone_item, data: data[i], ordeItem: clone_orderItem }));
                        }
                        //					items.append(div);
                        showLazyLoadImg();
                        if (data.length < 10)
                            $('#footer_loading').find('span').text('没有更多商品');
                    }
                    else {
                        var h = ($(window).height() / 2) - 50;
                        $("#main").append("<div style=\"color: #d7d7d7;margin: auto;text-align: center;margin-top: " + h + "px;font-size: 25px;\">暂无订单<div>");

                        $('#noOrder').show();
                    }
                }
            });
        }
    });
}

function downEvent() {
    sessionStorage.page = 1;
    var body = document.getElementsByTagName('body')[0]; //缓存body元素
    var top_loading = document.getElementById('top_loading'); //头部加载文字盒子
    var footer_loading = document.getElementById('footer_loading'); //底部加载文字盒子
    var $body = $(body);
    var $top_loading = $(top_loading);
    var $footer_loading = $(footer_loading);
    var startPageX = 0; //横轴开始值
    var startPageY = 0; //纵轴开始值
    var startY = document.body.scrollTop; //记录滚动条距离
    var top_loading_start_size = 0;
    var moveSize = 0; //横轴移动量
    var moveSizeY = 0; //纵轴移动量
    var topLoadingLock = false;
    var type = {};

    body.addEventListener('touchstart', function (e) {
        var touch = e.changedTouches[0];
        startPageX = touch.screenX;
        startPageY = touch.screenY;
        startY = body.scrollTop;
        top_loading_start_size = parseInt($top_loading.css('margin-top'));
    });
    body.addEventListener('touchmove', function (e) {
        var touch = e.changedTouches[0];
        moveSize = touch.screenX - startPageX;
        moveSizeY = touch.screenY - startPageY;
        var top_move = top_loading_start_size + moveSizeY;
        if (startY == 0 && moveSizeY > 0) {
            if (top_move <= 0) {
                e.preventDefault();
                $top_loading.css({
                    marginTop: top_move
                })
            } else {
                $top_loading.css({
                    marginTop: 0
                }).find('span').text('松开可更新');
                topLoadingLock = true;
            }
        }
    });
    body.addEventListener('touchend', function (e) {
        if (topLoadingLock) {
            //window.location.reload();
            getOrder({
                userId: sessionStorage.userId,
                pageIndex: 1,
                pageSize: 10,
                callback: function (data) {
                    //sessionStorage.data = JSON.stringify(data);
                    sessionStorage.orderList = JSON.stringify(data);
                    var items = $('#orderItems').empty();
                    var item = $('#order');
                    var orderItem = $('#orderItem');
                    var clone_item;
                    var clone_orderItem;

                    for (var i = 0; i < data.length; i++) {
                        clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                        clone_orderItem = orderItem.clone().removeClass('noDisplay').attr('id', '');
                        items.append(setOrderItemData({ element: clone_item, data: data[i], ordeItem: clone_orderItem }));
                    }
                    $top_loading.find('span').text('已更新');
                    setTimeout(function () {
                        $top_loading.css('margin-top', '-' + $top_loading.css('height'))
								.find('span').text('下拉可刷新');
                    }, 1000);
                    if (data.length < 10)
                        $('#footer_loading').find('span').text('没有更多商品');
                }
            });
            sessionStorage.page = 1;
        } else {
            $top_loading.css('margin-top', '-' + $top_loading.css('height'));
        }
    });

    $footer_loading.on("click",function () {
        sessionStorage.page++;
        getOrder({
            userId: sessionStorage.userId,
            pageIndex: sessionStorage.page,
            pageSize: 10,
            callback: function (data) {
                //sessionStorage.data = JSON.stringify(data);
                sessionStorage.orderList = JSON.stringify(JSON.parse(sessionStorage.orderList).concat(data));
                var items = $('#orderItems');
                var item = $('#order');
                var orderItem = $('#orderItem');
                var clone_item;
                var clone_orderItem;

                for (var i = 0; i < data.length; i++) {
                    clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                    clone_orderItem = orderItem.clone().removeClass('noDisplay').attr('id', '');
                    items.append(setOrderItemData({ element: clone_item, data: data[i], ordeItem: clone_orderItem }));
                }
                //					items.append(div);
                showLazyLoadImg();
                if (data.length < 10) {

                    $('#footer_loading').find('span').text('没有更多商品');
                }
            }
        });

    });
}





function round2(number, fractionDigits) {
    with (Math) {
        return round(number * pow(10, fractionDigits)) / pow(10, fractionDigits);
    }
}

//var _hmt = _hmt || [];
////百度埋码
//(function () {
//    var htmpro = (("https:" == document.location.protocol) ? "https://" : "http://");
//    var hj = "", url = document.location.href;
//    if (url.indexOf(htmpro + "dev") >= 0) {
//        return;
//    }
//    else if (url.indexOf(htmpro + "test") >= 0) {
//        return;
//    }
//    else if (url.indexOf(htmpro + "pre") >= 0) {
//        return;
//    }
//    else if (url.indexOf("localhost") >= 0) {
//        return;
//    }
//    var hm = document.createElement("script");
//    hm.src = "//hm.baidu.com/hm.js?b2605c4e1512bf83f1abadd4ba614c70";
//    var s = document.getElementsByTagName("script")[0];
//    s.parentNode.insertBefore(hm, s);
//})();

//获取商品列表 ajax
function getCommodityZPH(obj) {

    csson($("#cdefault"));
    var data = {
        ActId: sessionStorage.actId,
        PageIndex: sessionStorage.page,
        PageSize: 20
    };
    if (sessionStorage.List && sessionStorage.List.length > 0) {
        var result = JSON.parse(sessionStorage.List);
        sessionStorage.List = '';
        return result;
    }
    return getDataAjax({
        url: '/SetMobile/CommodityListZPH',
        //		data: '{"appId":"' + obj.appId + '","pageIndex":' + obj.pageIndex + ',"pageSize":' + obj.pageSize + '}',
        data: data,
        beforeSend: obj.beforeSend,
        callback: obj.callback
    });
}

//商品列表页首次加载使用函数
//涉及到页面加载时获取第一分页信息.
//设置侧边栏分类信息.
//侧边栏相关事件.
function CommodityView() {

    $(function () {
        $(window).scroll(function () {
            backToTop();
            scrollEvent();
        });
        //回到顶部
        function backToTop() {

            var scrollTop = $(window).scrollTop();
            //回到顶部
            if (scrollTop > goTopAdvance && showGoTop == false) {
                goTop.addClass("e-backtop-fixed");
                showGoTop = true;
            } else if (scrollTop < goTopAdvance && showGoTop == true) {
                goTop.removeClass("e-backtop-fixed");
                showGoTop = false;
            }
        }
        //向下设置列表分页数据
        function setDownListItem() {

            if (Math.abs(parseInt($('#Commodity_nav').css('left'))) + 5 < Math.round($(window).width() * 0.6)) {
                return 0;
            }
            //window.ajax;
            if (!sessionStorage.want) {
                sessionStorage.page == 1 ? sessionStorage.page++ : '';

                window.ajax = getCommodityZPH({
                    actId: sessionStorage.actId,
                    pageSize: 10,
                    beforeSend: function () {

                    },
                    callback: function (data) {

                        var items = $('#items');
                        var item = $('#parent_item');
                        var height = item.height();
                        var img_height = item.width() / item.find('img').width();
                        var clone_item;
                        sessionStorage.commodityList =
								JSON.stringify(JSON.parse(sessionStorage.commodityList).concat(data));

                        setTimeout(function () {
                            if (data.length) {
                                for (var i = 0; i < data.length; i++) {
                                    clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                                    items.append(setItemData({
                                        element: clone_item,
                                        data: data[i],
                                        height: height,
                                        img_height: img_height
                                    }));
                                }

                                showLazyLoadImg();
                                sessionStorage.page++;
                            } else {
                                $('#footer_loading').find('span').text('没有更多商品');
                            }
                        }, 500);
                        scrollLoading = false;
                    }
                })


            } else if (sessionStorage.want) {
                sessionStorage.wantLock = true;
                getWantCommodityData(sessionStorage.want);
            }
        }
        //滚动逻辑函数
        function scrollEvent() {
            if (scrollLoading == false) {

                var scrollHeight = $(document).height() > $(window).height() ? $(document).height() - $(window).height() : 0;
                var scrollTop = $(window).scrollTop();
                var scrollBottom = scrollHeight - scrollTop;


                //滚动加载
                if (scrollBottom <= advance && maxHeight < scrollHeight) {
                    maxHeight = scrollHeight;
                    scrollLoading = true;


                    var bnext = false;
                    switch (sessionStorage.ComTypeSearch) {
                        case "2":
                            bnext = true;
                            sessionStorage.page++;
                            ComOrderList(1, sessionStorage.PriceState, 0);
                            break;
                        case "3":
                            bnext = true;
                            sessionStorage.page++;
                            ComOrderList(0, 0, 0);
                            break;
                        case "4":
                            bnext = true;
                            sessionStorage.page++;
                            ComOrderList(2, 0, 0);
                            break;
                    }
                    if (bnext == false) {
                        setDownListItem();
                    }
                }
            }
        }

        new TouchMoveEvent();
        //		newTouchMoveEvent();
        navEvent();

        if (sessionStorage.appId && sessionStorage.appId != "undefined" && sessionStorage.appId != "null") {
        }
        else {
            sessionStorage.appId = getQueryString('appId') || appId;
        }

        if (sessionStorage.userId && sessionStorage.userId != "undefined") {
        }
        else {
            sessionStorage.userId = getQueryString('user') || '';
        }

        getNavList();

        if (getQueryString("sortType") == "New" && sessionStorage.ComTypeSearch != 1) {
            sessionStorage.appId = getQueryString('appId');
            csson($("#cnewcom"));
            ajaxLoading('22', '')
            sessionStorage.page = 1;
            sessionStorage.ComTypeSearch = 4;
            ComOrderList(2, 0, 1);
        }
        else {

            getAjaxData();

        }
        function getAjaxData() {
            getCommodityZPH({
                actId: sessionStorage.actId,
                pageSize: 10,
                callback: function (data) {
                    if (data.length === 0) alert('此分类下没有商品');
                    sessionStorage.data = JSON.stringify(data);
                    sessionStorage.commodityList = JSON.stringify(data);
                    var items = $('#items');
                    var item = $('#parent_item');
                    var height = item.height();
                    var img_height = item.find('img').height();
                    var clone_item;

                    for (var i = 0; i < data.length; i++) {
                        clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                        items.append(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
                    }
                    //					items.append(div);
                    showLazyLoadImg();
                    if (data.length < 10) {
                        $('#footer_loading').find('span').text('没有更多商品');
                    }
                    if (getQueryString('promotionId') != '' && getQueryString('promotionId') != undefined) {
                        $('.toporder').addClass('noDisplay');
                    }
                    if (!hasLazyLoadSet) {
                        $("body").picLazyLoad();
                        hasLazyLoadSet = true;
                    }
                }
            });
        }

    });

    var touchLock = true;

    /**
    * 手势事件
    *
    */
    function newTouchMoveEvent() {
        var nav = document.getElementById('Commodity_nav'),
			$nav = $(nav),
			navStartLeft,
			navWidth = $nav.css({ position: "absolute", visibility: "hidden", display: "block" }).width(),
			navMoveAutoMinSize = 20,
			starSize;

        $nav.css({ position: "fixed", visibility: "inherit", display: "none", left: -navWidth });

        touch.config({
            tap: true,                  //tap类事件开关, 默认为true
            doubleTap: true,            //doubleTap事件开关， 默认为true
            hold: true,                 //hold事件开关, 默认为true
            holdTime: 650,              //hold时间长度
            swipe: true,                //swipe事件开关
            swipeTime: 300,             //触发swipe事件的最大时长
            swipeMinDistance: 18,       //swipe移动最小距离
            swipeFactor: 5,             //加速因子, 值越大变化速率越快
            drag: true,                 //drag事件开关
            pinch: true                //pinch类事件开关
        });

        touch.on('body', 'swipestart', function (ev) {
            starSize = ev.x;
            navStartLeft = parseInt(nav.style.left);
        });

        touch.on('body', 'touchmove', function (e) {
            //			e.preventDefault();
        });

        touch.on('body', 'swiping', function (ev) {
            if (Math.abs(ev.x) > navMoveAutoMinSize) {
                nav.style.left = navStartLeft + ev.x - navMoveAutoMinSize + 'px';
                nav.style.display = 'block';
                nav.style.zIndex = '1000';
            }
        });

        touch.on('body', 'swipeend', function (ev) {
            var left = Math.abs(parseInt(nav.style.left));

            if (starSize - ev.x < 0) {
                if (left > navWidth - navMoveAutoMinSize) {
                    nav.style.left = -navWidth + 'px';
                } else {
                    nav.style.left = '0px';
                }
            } else {
                if (left < navMoveAutoMinSize) {
                    nav.style.left = '0px';
                } else {
                    nav.style.left = -navWidth + 'px';
                }
            }
        });




        //获取样式表信息
        function getStyle(obj, styleName) {
            return document.defaultView.getComputedStyle(obj, false)[styleName];
        }
    }

    function TouchMoveEvent() {
        var body = document.getElementsByTagName('body')[0]; //缓存body元素
        var main = document.getElementById('main'); //缓存main元素
        var box = document.getElementById('box'); //缓存box元素
        var nav = document.getElementById('Commodity_nav'); //缓存nav元素
        var top_loading = document.getElementById('top_loading'); //头部加载文字盒子
        var footer_loading = document.getElementById('footer_loading'); //底部加载文字盒子
        var $main = $(main);
        var $body = $(body);
        var $nav = $(nav);
        var $box = $(box);
        var $top_loading = $(top_loading);
        var $footer_loading = $(footer_loading);
        var startPageX = 0; //横轴开始值
        var startPageY = 0; //纵轴开始值
        var top_loading_start_size = 0;
        var moveSize = 0; //横轴移动量
        var moveSizeY = 0; //纵轴移动量
        var moveYMax = 10; //纵轴阀值
        var moveXMax = 10; //横轴阀值
        var startY = document.body.scrollTop; //记录滚动条距离
        var startX = document.body.scrollLeft;
        var startNavLeft = 0;
        var boxLeft = 0; //盒子左边距
        var touchMoveMinSize = 20; //最小移动距离.当到达这个值时将自动移动到touchMoveMaxSize设置的值
        var touchMoveMaxSize = parseInt($nav.width());
        //		var touchMoveMaxSize = $nav.css({position: "absolute", visibility: "hidden", display: "block" }).width(); //最大移动距离.
        //		$nav.css({position: "fixed", visibility: "inherit", display: "block" });
        var lockY = true; //纵轴锁
        var lockX = false; //横走锁
        var lockInnerX = true; //纵轴内部锁.该值为真时才能横向移动
        var mainLock = true; //main元素横向锁
        var navScrollStart = 0; //nav开始值
        var navScrollTop = 0; //nav滚动值
        var navMove = 0; //nav移动量
        var footerLoadingLock = false;
        var topLoadingLock = false;
        var type = {};



        var testBox = $('<div id="testBox"></div>').css({
            position: 'fixed',
            top: 0,
            height: '200px',
            width: '300px',
            left: 0,
            zIndex: 999,
            backgroundColor: '#fff'
        }
		);

        $nav.css({
            //			left: '-' + touchMoveMaxSize + 'px',
            zIndex: 1000
        });

        body.addEventListener('touchstart', function (e) {
            var touch = e.changedTouches[0];
            startPageX = touch.screenX;
            startPageY = touch.screenY;
            boxLeft = parseInt($(box).css('left'));
            box.style.left = boxLeft + 'px';
            startNavLeft = parseInt($nav.css('left'));
            lockX = false;
            lockY = true;
            startY = body.scrollTop;
            top_loading_start_size = parseInt($top_loading.css('margin-top'));
            footerLoadingLock = false;
            topLoadingLock = false;
            //$nav.css({ height: ($(window).height() - 200) });

            if (parseInt($nav.css('left')) > -20) {
                lockY = false;
                //				e.preventDefault();
                lockX = true;
            }

            if ($(document).height() > $(window).height()) {
                type.h = $(document).height() - $(window).height();
            } else {
                type.h = $(window).height() - $(document).height();
            }

            touchMoveMaxSize = parseInt($nav.width());
        });
        body.addEventListener('touchmove', function (e) {
            var touch = e.changedTouches[0];
            moveSize = touch.screenX - startPageX;
            moveSizeY = touch.screenY - startPageY;

            //判断当前纵轴移动量是否超过阀值.则将横轴锁打开
            //				if (Math.abs(moveSizeY) > moveYMax && lockX && lockY) {
            //					lockX = false;
            //				}

            //判断当前横轴移动量是否超过阀值.则将纵轴锁打开
            if (Math.abs(moveSize) > moveXMax && Math.abs(moveSizeY) < moveYMax) {
                lockY = false;
                e.preventDefault();
                lockX = true;
            }

            //纵移动相关
            if (lockY) {
                if (startNavLeft == 0) {
                    $nav.css({
                        left: 0
                    });
                } else {
                    $nav.css({
                        left: -touchMoveMaxSize + 'px'
                    });
                }

                if (startY == 0 && moveSizeY > 0) {
                    var top_move = top_loading_start_size + moveSizeY;
                    if (top_move <= 0) {
                        e.preventDefault();
                        $top_loading.css({
                            marginTop: top_move
                        })
                    } else {
                        $top_loading.css({
                            marginTop: 0
                        }).find('span').text('松开可更新');
                        topLoadingLock = true;
                    }
                }

                if ($body.scrollTop() == type.h) {
                    if (moveSizeY < -10 && $footer_loading.find('span').text() != '正在获取信息...') {
                        //$footer_loading.find('span').text('松开可更新');
                        e.preventDefault();
                        footerLoadingLock = true;
                    }
                } else {
                    //						$footer_loading.find('span').text('获取更多信息');
                }
            }

            //横移动相关
            if (lockX) {
                if (Math.abs(moveSizeY) < 10) {
                    e.preventDefault();
                    $nav.show();
                    var ll = startNavLeft + moveSize;
                    if (ll < 0) {
                        $nav.css({
                            left: startNavLeft + moveSize + 'px'
                        })
                    }
                    body.scrollTop = startY;
                }
            }

        });
        body.addEventListener('touchend', function (e) {
            touchMoveMaxSize = parseInt($nav.width());
            type.moveEnd = new Date().getTime();
            var touch = e.changedTouches[0];
            var mo = startPageX - touch.screenX;
            //				moveSizeY = startPageY - touch.screenY;

            //				if (lockY) {
            //					var item = $('#main').find('.item_1').eq(0);
            var items = $('#items');


            //横移动相关  nav相关
            if (lockX) {
                var n_left = parseInt($nav.css('left'));

                if (mo > 0) {
                    if (Math.abs(n_left) > 20) {
                        $nav.css('left', -touchMoveMaxSize + 'px')
                    } else {
                        $nav.css('left', 0);
                    }
                } else {
                    if (Math.abs(n_left) > touchMoveMaxSize - 20) {
                        $nav.css('left', -touchMoveMaxSize + 'px')
                    } else {
                        $nav.css('left', 0)
                    }
                }
            }

            moveSizeY = 0;
            moveSize = 0;
        });

        //向上获取数据
        function setUpListItem() {
            if (sessionStorage.want) {
                $top_loading.find('span').text('正在获取信息...');
                $('#items').empty();
                sessionStorage.page = 1;
                sessionStorage.wantLock = true;
                getWantCommodityData(sessionStorage.want);

                setTimeout(function () {
                    $top_loading.css('margin-top', '-' + $top_loading.css('height'))
						.find('span').text('下拉可刷新');
                }, 1000);

            } else {
                sessionStorage.page = 1;
                getCommodityZPH({
                    actId: sessionStorage.actId,
                    pageIndex: 1,
                    pageSize: 10,
                    beforeSend: function () {
                        $top_loading.find('span').text('正在获取信息...');
                    },
                    callback: function (data) {
                        //					if (sessionStorage.data != JSON.stringify(data)) {
                        var items = $('#items').empty();
                        var item = $('#parent_item');
                        var height = item.height();
                        var img_height = item.find('img').height();
                        var clone_item;

                        window.itemIdList = {};

                        for (var i = 0; i < data.length; i++) {
                            clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                            items.append(setItemData({
                                element: clone_item,
                                data: data[i],
                                height: height,
                                img_height: img_height
                            }));
                        }

                        $top_loading.find('span').text('已更新');
                        sessionStorage.data = JSON.stringify(data);
                        sessionStorage.commodityList = JSON.stringify(data);
                        sessionStorage.page = 1;
                        setTimeout(function () {
                            $top_loading.css('margin-top', '-' + $top_loading.css('height'))
								.find('span').text('下拉可刷新');
                        }, 1000);
                        $footer_loading.show();

                    }
                });
            }
        }


        /**
        * nav触摸开始事件
        * 设置初始值
        */
        nav.addEventListener('touchstart', function (e) {
            var touch = e.changedTouches[0];
            navScrollStart = touch.pageY;
        });

        /**
        * nav触摸移动事件.
        * 将浏览器默认事件重置.
        * 并模拟内部滚动.
        */
        nav.addEventListener('touchmove', function (e) {
            e.preventDefault();
            var touch = e.changedTouches[0];
            navMove = touch.pageY - navScrollStart;

            nav.scrollTop = navScrollTop - navMove;
        });

        /**
        * nav触摸结束时.将当前高度记录.供下次移动时使用.
        * 并设置全局touchLock锁.
        */
        nav.addEventListener('touchend', function (e) {
            navScrollTop = nav.scrollTop;
            touchLock = !navMove;
            navMove = 0;
        });

        $box.on('click', '.item_1', function (e) {
            if (Math.abs(moveSizeY) < 10 && !lockX) {
                ajaxLoading('22', '');
                var self = $(this);
                var box_2 = $('#box_2');
                sessionStorage.commodityId_2 = self.data('id');

                setTimeout(function () {
                    $("#ajaxLoadBlind").remove();
                }, 1000);
                window.location.href = '/Mobile/CommodityDetail?commodityId=' +
                    self.data('id') + '&appId=' + self.attr('appid') + '&user=' + sessionStorage.userId + '&type=show&source=' + sessionStorage.source + '&SrcType=' + sessionStorage.SrcType + '&SrcTagId=' + sessionStorage.SrcTagId + '&share=' + getShareId();
            }
        });


    }

    /**
    * nav相关事件
    */
    function navEvent() {
        var parentUl;
        var parentUlClone = {};
        $('#Commodity_nav').on('click', '.nav_inner', function () {
            if (touchLock) {
                var self = $(this);
                parentUl = self.parents('ul');
                parentUlClone = parentUl.clone();
                var self_name = self.data('parent-name');
                var item_key = self.data('show-item');
                var one_li = '<li class="nav_one" data-category-id="{id}">{text}</li>';
                var two_li = '<li class="nav_two" data-category-id="{id}">{text}</li>';

                parentUl.empty();

                parentUl.append(one_li.replace('{text}', self_name + '<span>></span>' + self.text())
					.replace('{id}', self.data('category-id')));

                parentUlClone.find('.hidden').each(function () {
                    if ($(this).data('show-key') == item_key) {
                        parentUl.append(two_li.replace('{text}', $(this).text()).replace('{id}', $(this).data('category-id')));
                    }
                });
            }
        }).on('click', '.title', function () {
            window.itemIdList = {};
            if (parentUl) {
                var ul_parent = parentUl.parent();
                ul_parent.find('ul').remove();
                parentUlClone.find('li').removeClass('nav_focus');
                ul_parent.append(parentUlClone);
            } else {
                $(this).parent().find('li').removeClass('nav_focus');
            }
            var items = $('#items').empty();

            getCommodityZPH({
                actId: sessionStorage.actId,
                pageIndex: 1,
                pageSize: 10,
                callback: function (data) {
                    sessionStorage.data = JSON.stringify(data);
                    sessionStorage.categoryId = '';
                    sessionStorage.categoryLock = '';
                    sessionStorage.want = '';
                    sessionStorage.wantLock = '';
                    sessionStorage.commodityList = JSON.stringify(data);
                    sessionStorage.page = 1;
                    var item = $('#parent_item');
                    var height = item.height();
                    var img_height = item.find('img').height();
                    var clone_item;

                    for (var i = 0; i < data.length; i++) {
                        clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                        items.append(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
                    }

                    showLazyLoadImg();

                    if (data.length < 10)
                        $('#footer_loading').find('span').text('没有更多商品');
                    parentUl = '';
                }
            });


        }).on('click', 'li', function () {
            if (touchLock) {
                var self = $(this);
                var parent = self.parent();
                if (!self.hasClass('no_nav_focus')) {
                    parent.find('li').removeClass('nav_focus');
                    if (!self.hasClass('no_nav_focus')) {
                        self.addClass('nav_focus');
                    }

                    if (!self.hasClass('nav_inner')) {
                        sessionStorage.page = 1;
                        sessionStorage.ComTypeSearch = 0;
                        sessionStorage.categoryLock = '';
                        var psource = sessionStorage.source == "share" ? "&source=share" : "";
                        location.href = '/SetMobile/CommodityList?pageIndex=1&pageSize=10&fieldSort=0&order=1&setCategoryId=' + self.data('category-id')+psource;
                        //getCommodityByCategory(self.data('category-id'));
                    }
                }
            }
        }).on('keydown', 'input', function (e) {
            var self = $(this);
            if (e.keyCode == 13) {
                sessionStorage.page = 1;
                sessionStorage.categoryLock = '';
                getWantCommodityData(self.val());
                self[0].blur();
            }
        });


    }
}

var iosShareData = '{}';
function shareAndroid(content, title, imgUrl, url, sourceType) {
    try {
        var shareurl = "";
        var shareSquareUrl = "";

        url = url.toLowerCase();

        url = url.replace(/&userid=[^&]*/g, "");
        url = url.replace(/\?userid=[^&]*&/g, "\?");
        url = url.replace(/&sessionid=[^&]*/g, "");
        url = url.replace(/\?sessionid=[^&]*&/g, "\?");
        url = url.replace(/&srcappid=[^&]*/g, "");
        url = url.replace(/\?srcappid=[^&]*&/g, "\?");
        url = url.replace(/\?srctype=[^&]*&/g, "\?");
        url = url.replace(/&srctype=[^&]*/g, "");
        url = url.replace(/\?srctagid=[^&]*&/g, "\?");
        url = url.replace(/&srctagid=[^&]*/g, "");
        url = url.replace(/\?user=[^&]*&/g, "\?");
        url = url.replace(/&user=[^&]*/g, "");
        
        url = url.replace("&source=share&", "&").replace("?source=share&", "?").replace("&source=share", "").replace("?source=share", "").replace('?&', '?');

        var tmpIndex = url.indexOf("?");
        if (tmpIndex < 0) {
            shareurl = url + "?source=share";
            shareSquareUrl = url;
        } else if (tmpIndex == url.length - 1) {
            shareurl = url + "source=share";
            shareSquareUrl = url.substr(0, url.length - 1);
        } else {
            shareurl = url + "&source=share";
            shareSquareUrl = url;
        }

        //原应用Id.
        if (JsVilaDataNull(sessionStorage.srcAppId)) {
            var appIdUrl = "srcAppId=" + sessionStorage.srcAppId;
            shareurl += "&" + appIdUrl;
        }  

        iosShareData = '{\"shareurl\":\"' + shareurl + '",\"shareSquareUrl\":"' + shareSquareUrl + '",\"content\":"' + (content ? content : '') + '",\"title\":"' + (title ? title : '') + '",\"imgUrl\":"' + (imgUrl ? imgUrl : '') + '",\"sourceType\":"' + (sourceType ? sourceType : '') + '"}';
        window.appshare.getShareInfo(shareurl, shareSquareUrl, content, title, imgUrl, null, null, 18);
    } catch (e) {
    }
}
function shareIOS() {
    return iosShareData;
}

$(function () {
    $("#navBackground").live("click", function () {
        var navWidth = $("#Commodity_nav").css({ position: "absolute", visibility: "hidden", display: "block" }).width();
        $("#Commodity_nav").css({ position: "fixed", visibility: "inherit", display: "none", left: -navWidth });
        $("#navBackground").remove();
    });
});

function navBackground() {
    if (!$('#navBackground')[0]) {
        //ajaxLoading盒子ID对象
        //蒙版
        var blind = $('<div></div>');

        //蒙版相关css
        blind.css({
            'position': 'fixed',
            'z-index': '998',
            'opacity': 0,
            'backgroundColor': '#ccc',
            'height': '100%',
            'width': '100%',
            'top': 0,
            'left': 0
        });

        //蒙版ID值
        blind.attr('id', 'navBackground');
        $('body').append(blind);
    }
}

//读取cookies 
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))

        return unescape(arr[2]);
    else
        return null;
}
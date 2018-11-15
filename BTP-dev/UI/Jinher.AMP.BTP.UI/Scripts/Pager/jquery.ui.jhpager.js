/**
* @fileOverview 这个文件是分页控件的源文件. 
* @author 李玲 2011/8/18 
* @version 1.0.0 
*
* @depends：
*   jquery.extend.js
*   jquery.ui.core.js
*   jquery.ui.widget.js
*
*/
(function ($) {
    /** @class 分页控件
    * @name jhpager
    * @description 分页控件
    * @property {string} dataType 预期服务器返回的数据类型，仅当requestType为get或post，async为true时有效。
    *                             具体信息请参考jquery的ajax方法说明
    * @property {string} requestType 获取数据的方式 local-本地数据 get-以get方式获取数据 post-以post方式获取数据
    * @property {array/string} dataSource 如果是requestType为local，那么此处为数据源数组，另外两种方式，此处为url字符串
    * @property {boolean} async 当requestType为get或post时有效，设置分页请求是异步方式还是同步方式
    * @property {number} totalCount 总记录数
    * @property {number} currentPage 当前页
    * @property {number} rowNum 每页显示的记录数
    * @property {json} requestData 发送请求时提交的参数
    */
    $.widget("ui.jhpager",
    /** @lends jhpager.prototype */
    {
    //默认属性
    options: {
        async: false,    //当requestType为get或post时有效，设置分页请求是异步方式还是同步方式
        totalCount: "",       //是否可以添加图片，默认为false
        currentPage: 1,
        rowNum: 20,       //是否在图片下显示标题信息
        dataType: 'json',
        requestType: 'get',   //获取数据的方式 local-本地数据 get-以get方式获取数据 post-以post方式获取数据
        dataSource: [], //如果是requestType为local，那么此处为数据源数组，另外两种方式，此处为url字符串 
        requestData: null,   //发送请求时提交的数据     /*content: 用requestData传递回调函数 date:20140513 by:邓贞才*/
        /**  
        * @name jhpager#beforeRequest  
        * @event  
        * @param {event} e  
        * @param {number} pager 点击的页码
        * @description 请求前事件
        */
        beforeRequest: null,
        /**
        * @name jhpager#requestSuccess  
        * @event  
        * @param {event} e  
        * @param response 请求成功后的返回值，如果requestType为local，那么是根据分页信息返回的相应记录（json对象数组）
        * @description 请求成功事件
        */
        requestSuccess: null,
        /**
        * @name jhpager#requestFailure  
        * @event  
        * @param {event} e  
        * @param response 请求失败后的返回值
        * @description 请求失败事件
        */
        requestFailure: null
    },

    //控件的初始化方法
    _create: function () {
        var self = this,
            element = this.element,
            options = this.options;
        element.addClass('ui-jhpager');
        self._createLinks();
        return this;
    },
    //创建分页按钮
    _createLinks: function (currentPage) {
        var self = this,
            element = self.element,
            options = self.options;
        if (!options.totalCount && options.totalCount == "" && options.requestType == 'local') {
            options.totalCount = options.dataSource.length;
        }
        element.html('<div style="clear:both;width:0;height:0;"></div>');
        var container = self.container = $("<div></div>").prependTo(element);
        var totalrecordsNumContainer = self.totalrecordsNumContainer = $('<span style="float:left; margin-top:5px;">' + '总记录数0条' + '</span>').appendTo(container),
        firstPageButton = self.firstPageButton = $('<span></span>').appendTo(container)
                .addClass('ui-jhpager-button ui-jhpager-firstPageButton')
                .text($.jhpager.firstPage)
                .attr('title', $.jhpager.firstPage)
                .click(function () {
                    if ($(this).hasClass("ui-jhpager-firstPageButton-disabled")) { return false; }
                    self._getPage(1);
                })
                .hover(
                    function () {
                        firstPageButton.addClass('ui-jhpager-firstPageButton-hover');
                    },
                    function () {
                        firstPageButton.removeClass('ui-jhpager-firstPageButton-hover');
                    }
                )
                .mousedown(function () {
                    firstPageButton.addClass('ui-jhpager-firstPageButton-mousedown');
                })
                .mouseup(function () {
                    firstPageButton.removeClass('ui-jhpager-firstPageButton-mousedown');
                }),
            prevPageButton = self.prevPageButton = $('<span></span>').appendTo(container)
                .addClass('ui-jhpager-button ui-jhpager-prevPageButton')
                .text($.jhpager.prevPage)
                .attr('title', $.jhpager.prevPage)
                .click(function () {
                    if ($(this).hasClass("ui-jhpager-prevPageButton-disabled")) { return false; }
                    self._getPage("prev");
                })
                .hover(
                    function () {
                        prevPageButton.addClass('ui-jhpager-prevPageButton-hover');
                    },
                    function () {
                        prevPageButton.removeClass('ui-jhpager-prevPageButton-hover');
                    }
                )
                .mousedown(function () {
                    prevPageButton.addClass('ui-jhpager-prevPageButton-mousedown');
                })
                .mouseup(function () {
                    prevPageButton.removeClass('ui-jhpager-prevPageButton-mousedown');
                }),
            pagerContainer = self.pagerContainer = $('<span class="ui-jhpager-pager">' + currentPage + '/' + (options.totalCount != "" ? options.totalCount : 1) + '</span>').appendTo(container),
            nextPageButton = self.nextPageButton = $('<span></span>').appendTo(container)
                .addClass('ui-jhpager-button ui-jhpager-nextPageButton')
                .text($.jhpager.nextPage)
                .attr('title', $.jhpager.nextPage)
                .click(function () {
                    if ($(this).hasClass("ui-jhpager-nextPageButton-disabled")) { return false; }
                    self._getPage("next");
                })
                .hover(
                    function () {
                        nextPageButton.addClass('ui-jhpager-nextPageButton-hover');
                    },
                    function () {
                        nextPageButton.removeClass('ui-jhpager-nextPageButton-hover');
                    }
                )
                .mousedown(function () {
                    nextPageButton.addClass('ui-jhpager-nextPageButton-mousedown');
                })
                .mouseup(function () {
                    nextPageButton.removeClass('ui-jhpager-nextPageButton-mousedown');
                }),
            lastPageButton = self.lastPageButton = $('<span></span>').appendTo(container)
                .addClass('ui-jhpager-button ui-jhpager-lastPageButton')
                .text($.jhpager.lastPage)
                .attr('title', $.jhpager.lastPage)
                .click(function () {
                    if ($(this).hasClass("ui-jhpager-lastPageButton-disabled")) { return false; }
                    self._getPage("last");
                })
                .hover(
                    function () {
                        lastPageButton.addClass('ui-jhpager-lastPageButton-hover');
                    },
                    function () {
                        lastPageButton.removeClass('ui-jhpager-lastPageButton-hover');
                    }
                )
                .mousedown(function () {
                    lastPageButton.addClass('ui-jhpager-lastPageButton-mousedown');
                })
                .mouseup(function () {
                    lastPageButton.removeClass('ui-jhpager-lastPageButton-mousedown');
                }),

            skipContainer = self.skipContainer = $('<span class="ui-jhpager-skip">' + $.format($.jhpager.skipToPage, '<input type="text" class="ui-jhpager-skip-input"/>') + '</span>').appendTo(container),
            skipButton = self.skipButton = $('<button class="ui-jhpager-skip-button">' + $.jhpager.skip + '</button>').insertAfter(skipContainer).button()
                .click(function () {
                    var pagerInput = $('input:text', skipContainer);
                    var pager = pagerInput.val();
                    if (!pager) { return; }
                    pagerInput.val('');
                    self._getPage(pager);
                });

        this.refresh(options.currentPage, options.totalCount);
        $('.ui-jhpager-skip-input').keydown(function (event, v, d) {
            var k = event.keyCode;
            var keyCode = $.ui.keyCode;
            switch (k) {
                case 13:
                    skipButton.trigger("click");
                    event.preventDefault();
                    break;
                case keyCode.UP:
                case keyCode.DOWN:
                case keyCode.BACKSPACE: //退格键
                case keyCode.DELETE: //删除键
                case keyCode.TAB:
                case 37:
                case 39:
                    break;
                default:
                    if ((47 < k && k < 58) || (95 < k && k < 106)) {
                        return true;
                    } else {
                        return false;
                    }
                    break;
            }
        });
    },
    //获取页数
    _getTotalPage: function () {
        return Math.ceil(this.options.totalCount / this.options.rowNum);
    },

    /**
    * 根据当前页、总记录数刷新分页按钮状态
    */
    refresh: function (currentPage, totalCount) {
        var self = this,
            options = self.options,
            element = self.element;
        options.currentPage = currentPage;
        options.totalCount = totalCount ? totalCount : 1;
        var totalPage = self._getTotalPage();
        var width = 0;
        self.pagerContainer.text(currentPage + '/' + totalPage);

        self.totalrecordsNumContainer.text('总记录数' + totalCount + '条')

        if (totalPage <= 1) {
            self.skipContainer.hide();
            self.skipButton.hide();
        } else {
            self.skipContainer.show();
            self.skipButton.show();
            width = self.skipContainer.width() + self.skipButton.width();
        }
        if (currentPage > 1 && totalPage > 1) {
            self.prevPageButton.removeClass('ui-jhpager-prevPageButton-disabled');
            self.firstPageButton.removeClass('ui-jhpager-firstPageButton-disabled');
        } else {
            self.prevPageButton.addClass('ui-jhpager-prevPageButton-disabled');
            self.firstPageButton.addClass('ui-jhpager-firstPageButton-disabled');
        }
        if (currentPage < totalPage && totalPage > 1) {
            self.nextPageButton.removeClass('ui-jhpager-nextPageButton-disabled');
            self.lastPageButton.removeClass('ui-jhpager-lastPageButton-disabled');
        } else {
            self.nextPageButton.addClass('ui-jhpager-nextPageButton-disabled');
            self.lastPageButton.addClass('ui-jhpager-lastPageButton-disabled');
        }

        width = width + self.firstPageButton.outerWidth() * 4 + self.pagerContainer.outerWidth() + 50 + self.totalrecordsNumContainer.outerWidth();
        var margin = (element.width() - width) / 2;
        self.container.css({ "width": width, "margin-left": margin });
    },

    _getPage: function (param) {
        var self = this,
            options = self.options,
            currentPage = parseInt(options.currentPage),
            totalPage = self._getTotalPage(),
            pager;
        if (param == 'prev' && currentPage > 1) {
            if (currentPage > 1) {
                pager = currentPage - 1;
            } else {
                return;
            }
        } else if (param == 'next') {
            if (totalPage > currentPage) {
                pager = currentPage + 1;
            } else {
                return;
            }
        } else if (param == 'last') {
            pager = totalPage;
        } else {
            param = parseInt(param);
            if (param > totalPage) {
                pager = totalPage;
            } else if (param < 1) {
                pager = 1;
            } else if (param) {
                pager = param;
            }
        }
        self._trigger('beforeRequest', null, pager);
        if (options.requestType == 'local') {
            self._getLocalData(pager);
            //默认滚动到页面的顶部。
            $(window.top).scrollTop(0);
        }
        else {
            self._getRemoteData(pager);
        }
       
    },
    _getLocalData: function (pager) {
        var self = this,
            options = self.options;
        if (typeof options.dataSource == 'object') {
            var dataSource = options.dataSource;
            var totalCount = dataSource.length;
            var rowNum = options.rowNum;
            var pageLastNum = pager * rowNum;
            var lastNum;
            if (pageLastNum > totalCount) {
                lastNum = totalCount;
            } else {
                lastNum = pageLastNum;
            }
            self.refresh(pager, totalCount);
            var response = dataSource.slice((pager - 1) * rowNum, lastNum);
            self._trigger('requestSuccess', null, response);
        } else {
            alert('数据源设置有误，应为json数组');
        }
    },
    _getRemoteData: function (pager) {
        var self = this,
            options = self.options;
        var data = {};
        if (options.requestData != null 
        && jQuery.isFunction(options.requestData)) {
            data = options.requestData();
        }
        if (options.async) {
            $.ajax({
                url: options.dataSource + '?currentPage=' + pager,
                dataType: options.dataType,
                type: options.requestType,
                data: data,
                success: function (response) {
                    self.refresh(pager, options.totalCount);
                    self._trigger('requestSuccess', null, response);
                    //默认滚动到页面的顶部。
                    $(window.top).scrollTop(0);
                },
                error: function (response) {
                    self._trigger('requestFailure', null, response);
                }
            });
        } else {
            var href = options.dataSource + '?currentPage=' + pager;
            if (data.length > 0) {
                var param = jQuery.param(data);
                href += '&' + param;
            }
            document.location.href = href;
        }
    }

});
$.extend($.ui.jhpager, {
    version: "1.0.0"
});

})(jQuery);

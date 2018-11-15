(function ($) {
    $.extend($.ui.jhtablebox,
    /** @lends jhtablebox.prototype */
    {
    //创建Tab页签形式的窗口的标题栏
    _createTabTitleBar: function (self, title, titleId) {
        $.extend(self, this);
        var options = self.options;
        if (self.options.minWidth < 536) {
            self.options.minWidth = 536;
        }

        //窗口标题部分对应的html
        var html = '<div id="div_tabs_titlesdiv_grri" class="ui-tabs-title">'
                 + '<div class="ui-tabs-title-pic titleclass '
                 + (options.titleIcon ? options.titleIcon : '') + '"'
                 + (options.titleIconPath ? 'style="background-image:url(' + options.titleIconPath + ');"' : '')
                 + '></div>'
                 + '<div class="ui-tabs-title-name">' + options.title + '</div>'
                 + '</div>',
        //创建窗口的标题栏
            uiDialogTitlebar = (self.uiDialogTitlebar = $('<div></div>'))
				    .addClass(
					    'ui-tabs-header ' +
					    'ui-helper-clearfix'
				    )
				    .prependTo(self.uiDialog)
                    .html(html),
        //添加标题栏上的按钮
            uiDialogButtonContainer = $('<div class="ui-jhtablebox-title-button-area"></div>').appendTo(uiDialogTitlebar).before('<div class="ui-tabs-right"></div>'),
            containerWidth = self._createTitleButtons(uiDialogButtonContainer);
        uiDialogButtonContainer.width(containerWidth);

        //添加Tab页签区域
        var uiDialogTitleTabs = $('<div></div>').addClass('ui-jhtablebox-tabs')
                .appendTo(uiDialogTitlebar);
        var uiDialogTitleTabsContainer = $('<div></div>').addClass('ui-jhtablebox-tabs-container').appendTo(uiDialogTitleTabs);
        //向前按钮
        var prevButton = self.prevButton = $('<div class="ui-jhtablebox-tabs-prev"></div>').appendTo(uiDialogTitleTabsContainer)
                .hover(
                    function () { prevButton.addClass('ui-jhtablebox-tabs-prev-hover'); },
                    function () { prevButton.removeClass('ui-jhtablebox-tabs-prev-hover'); }
                )
                .mousedown(function () {
                    prevButton.addClass('ui-jhtablebox-tabs-prev-click');
                })
                .mouseup(function () {
                    prevButton.removeClass('ui-jhtablebox-tabs-prev-click');
                })
                .click(function () {
                    if (self._prevButtonDisabled()) return false;
                    self._changePosition("prev");
                });
        //向后按钮
        var nextButton = self.nextButton = $('<div class="ui-jhtablebox-tabs-next"></div>').appendTo(uiDialogTitleTabsContainer)
                .hover(
                    function () { nextButton.addClass('ui-jhtablebox-tabs-next-hover'); },
                    function () { nextButton.removeClass('ui-jhtablebox-tabs-next-hover'); }
                ).mousedown(function () {
                    nextButton.addClass('ui-jhtablebox-tabs-next-click');
                })
                .mouseup(function () {
                    nextButton.removeClass('ui-jhtablebox-tabs-next-click');
                })
                .click(function () {
                    if (self._nextButtonDisabled()) return false;
                    self._changePosition("next");
                });
        //内容区域
        var uiDialogTitleTabsContent = self.uiDialogTitleTabsContent = $('<div></div>').addClass('ui-jhtablebox-tabs-content').appendTo(uiDialogTitleTabsContainer);
        //根据设置，在Tab页签区域添加Tab页签
        self._createTabs(uiDialogTitleTabsContent);
        self._changeButtonState(parseInt(self.tabUl.css('margin-left')));
        return self;
    },

    //添加Tab页签
    _createTabs: function (parent) {
        var self = this,
            tabs = self.options.tabs;
        var tabUl = self.tabUl = $('<ul></ul>').appendTo(parent);
        var num = 0;
        $.each(tabs, function (i, tabData) {
            self.createTab(tabData);
            num++;
        });
        if (!self.selectedTab) {
            self.selectTab(0);
        }
        var ulWidth = 100 * num;
        tabUl.width(ulWidth);
    },

    /**
    * 创建Tab页签
    * @param Tab对象 {tabIcon:'',tabName:'',currentClass:'',url:''}
    */
    createTab: function (tabData) {
        var self = this,
            tabUl = this.tabUl;

        var html = '<div class="ui-jhtablebox-tab-li-icon ';
        if (!tabData.tabIcon && !tabData.tabIconPath) {
            html += 'ui-jhtablebox-tab-li-icon-default"';
        } else {
            html += (tabData.tabIcon ? tabData.tabIcon : '') + '" ' +
                    (tabData.tabIconPath ? 'style="background:url(' + tabData.tabIconPath + ')  no-repeat 0 2px"' : '');
        }
        html += '></div>'
                     + '<div class="ui-jhtablebox-tab-li-text" title="' + tabData.tabName + '">' + tabData.tabName + '</div>';
        var tabLi = $('<li id="jhtablebox_tabs_' + tabData.tabId + '"></li>').appendTo(tabUl)
                    .addClass('ui-jhtablebox-tab-li')
                    .data("tabData", tabData)
                    .html(html)
                    .hover(
                        function () {
                            if (!$(this).hasClass('ul-li-bg-current')) {
                                $(this).addClass('ul-li-bg-hover');
                            }
                        },
                        function () {
                            $(this).removeClass('ul-li-bg-hover');
                        })
                    .click(function () {
                        self._selectTab(tabLi, tabData);
                    });
        if (tabData.selected) {
            self._selectTab(tabLi, tabData);
        }
    },
    _selectTab: function (tabLi, tabData) {
        var self = this;
        var preSelectedTab = self.selectedTab;
        var selectTab = { "tabLi": tabLi, "tabData": tabData };
        /**  
        * @name jhtablebox#beforeSelect 
        * @event  
        * @param {event} e  
        * @param preSelectedTab 前一个选中的Tab {"tabLi": tabLi, "tabData": tabData} tabLi-tab页签html对象；tabData-tab对象
        * @param selectTab 要选中的Tab  {"tabLi": tabLi, "tabData": tabData}
        * @description 选中前事件				   
        */
        if (!self._trigger('beforeSelect', null, { preSelectedTab: preSelectedTab, selectTab: selectTab })) {
            return false;
        }
        if (preSelectedTab) {
            var preSelectedTabLi = preSelectedTab.tabLi,
                preSelectedTabData = preSelectedTab.tabData;
            preSelectedTabLi.removeClass('ul-li-bg-current');    
                    
            var currentClass = preSelectedTabData.currentClass ? preSelectedTabData.currentClass : "ui-jhtablebox-tab-li-icon-default-on";
            $('.ui-jhtablebox-tab-li-icon', preSelectedTabLi).removeClass(currentClass);
            if (preSelectedTabData.currentIconPath) {
                $('.ui-jhtablebox-tab-li-icon', preSelectedTabLi).attr("style", "background:url(" + preSelectedTabData.tabIconPath + ") no-repeat 0 2px");
            }

            $('#' + preSelectedTabData.tabId + '_frame').hide();
        }

        var url = tabData.customUrl ? tabData.customUrl : tabData.url;
        //在切换页签时动态生成页面的url
        if (this.options.changeUrlWhenSelect) {
            url = this.options.changeUrlWhenSelect.call(this, url, selectTab);
        }
        var frame = $('#' + tabData.tabId + '_frame');
        if (frame.length > 0) {
            frame.show();
            if (self.options.loadEverytime || frame.attr('src') != url) {
                frame.attr('src', url);
            }
        } else {
            $('<iframe frameborder="0" id="' + tabData.tabId + '_frame" name="' + tabData.tabId + '_frame" src="' + url + '" width="100%" height="100%"></iframe>').appendTo(self.frame);
        }

        //高亮显示当前菜单
        tabLi.removeClass("ul-li-bg-common")
                .removeClass("ul-li-bg-hover")
                .addClass("ul-li-bg-current");
        var currentClass = tabData.currentClass ? tabData.currentClass : "ui-jhtablebox-tab-li-icon-default-on";
        //当前标签中小图标
        $('.ui-jhtablebox-tab-li-icon', tabLi).addClass(currentClass);
        if (tabData.currentIconPath) {
            $('.ui-jhtablebox-tab-li-icon', tabLi).attr("style", "background:url(" + tabData.currentIconPath + ") no-repeat 0 2px");
        }


        self.selectedTab = selectTab;
        /**  
        * @name jhtablebox#select 
        * @event  
        * @param {event} e  
        * @param selectTab 要选中的Tab  {"tabLi": tabLi, "tabData": tabData} tabLi-tab页签html对象；tabData-tab对象
        * @description 选中前事件				   
        */
        self._trigger('select', null, selectTab);
    },
    /**
    * 选中指定索引的Tab页签
    */
    selectTab: function (index) {
        var tabLi = $('li:eq(' + index + ')', this.tabUl);
        this._selectTabByLi(tabLi);
    },

    selectTabById: function (id, url) {
        var tabLi = $('#jhtablebox_tabs_' + id);
        this._selectTabByLi(tabLi);
        if (url) {
            $("#" + tabLi.data("tabData").tabId + "_frame").attr("src", url);
        }
    },

    _selectTabByLi: function (tabLi) {
        var self = this;
        if (tabLi.length == 0) {
            return;
        }
        self._selectTab(tabLi, tabLi.data("tabData"));
        var uicontainerWidth = self.uiDialogTitleTabsContent.width();
        var tabliLeft = tabLi.position().left;
        var newMarginLeft = marginLeft = parseInt(self.tabUl.css('margin-left'));
        if (tabliLeft > uicontainerWidth) {
            if (self.tabUl.width() + marginLeft - (tabliLeft - uicontainerWidth + 100) < uicontainerWidth) {
                newMarginLeft = uicontainerWidth - self.tabUl.width();
            } else {
                newMarginLeft = marginLeft - (tabliLeft - uicontainerWidth + 100);
            }
        } else if ((tabliLeft + marginLeft) < 0) {
            newMarginLeft = -tabliLeft;
        }
        self.tabUl.css('margin-left', newMarginLeft);
        self._changeButtonState(newMarginLeft);
    },
    /**
    * 刷新tab页内容
    */
    refreshTab: function (index) {
        var self = this;
        var tabLi = $('li:eq(' + index + ')', this.tabUl);
        var tabData = tabLi.data("tabData");
        $("#" + tabData.tabId + "_frame").attr("src", tabData.url);
    },

    _prevButtonDisabled: function () {
        return this.prevButton.hasClass('ui-jhtablebox-tabs-prev-disabled') || this.prevButton.hasClass('ui-jhtablebox-tab-button-hide');
    },
    _nextButtonDisabled: function () {
        return this.nextButton.hasClass('ui-jhtablebox-tabs-next-disabled') || this.prevButton.hasClass('ui-jhtablebox-tab-button-hide');
    },
    _changePosition: function (direction) {
        var self = this;
        self.tabUl.stop(false, true);
        var marginLeft = parseInt(self.tabUl.css("margin-left"));

        if (direction == "prev") {
            //newMarginLeft = "+=" + newMarginLeft;
            marginLeft += 100;
        } else {
            marginLeft -= 100;
        }
        self._changeButtonState(marginLeft);
        self.tabUl.animate({ "margin-left": marginLeft }, 500);
    },
    //改变左右按钮的可用状态
    _changeButtonState: function (newMarginLeft) {
        var self = this;
        if (newMarginLeft >= 0) {
            self.prevButton.addClass('ui-jhtablebox-tabs-prev-disabled');
        } else if (self._prevButtonDisabled()) {
            self.prevButton.removeClass('ui-jhtablebox-tabs-prev-disabled');
        }
        if (self.tabUl.width() + newMarginLeft <= self.uiDialogTitleTabsContent.width()) {
            self.nextButton.addClass('ui-jhtablebox-tabs-next-disabled');
        } else if (self._nextButtonDisabled()) {
            self.nextButton.removeClass('ui-jhtablebox-tabs-next-disabled');
        }
    }
});
})(jQuery);
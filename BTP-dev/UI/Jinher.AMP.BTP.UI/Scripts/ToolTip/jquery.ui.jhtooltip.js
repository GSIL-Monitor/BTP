/**
* @fileOverview 这个文件是tooltip控件的源文件. 
* @author 李卫 2011/8/1  
* @version 1.0.0 
*
* Depends:
*	jquery.ui.core.js
*	jquery.ui.widget.js
*/
(function ($) {
    /** @class jhtooltip控件类
    * @name jhtooltip
    * @description jhtooltip控件类
    * @property {number} width 宽度   
    * @property {number} height 高度
    * @property {boolean} autoWidth 是否根据内容自适应宽度，默认为true。如果设置为false，那么tooltip的宽度为应用此控件的对象的宽度
    * @property {string} type 类型 "custom"-为浮动提示，蓝色边框，"normal"-浮动提示，橙色边框，"embed"-嵌入到页面上的提示，橙色边框，默认为"normal"
    * @property {number} zIndex z-Index
    * @property {boolean} autoClose  是否自动关闭，默认为true
    * @property {number} closeSpeed  关闭的延迟时间，单位秒
    * @property {boolean} canStopClose  是否可以中断关闭动作，默认为false，当鼠标移到提示框内时，中断关闭动作
    * @property {boolean} hasCloseButton  是否显示关闭按钮，默认为false
    * @property {string} message  要显示的消息
    * @property {boolean} hasArrow 是否显示箭头，默认为true
    * @property {string} arrowPosition 设置箭头在tooltip的上方/下方，"top"-上方，"bottom"-下方，"auto"-默认值，根据在容器内位置，自动设置箭头位置
    * @property {number} arrowPadding 是否显示箭头，默认为true
    * @property {string} arrowAlign 设置箭头居左/居右，"left"-居左，"right"-居右，"auto"-默认值，根据在容器内位置，自动设置箭头位置
    * @property {obj} container 所在的容器，用于判断弹出框显示位置，默认为应用此控件的上级元素
    */
    $.widget("ui.jhtooltip", {
        /** @lends jhtooltip.prototype */
        options: {
            width: null,
            height: null,
            autoWidth: true,
            type: "normal",
            zIndex: 1,
            autoClose: true,
            closeSpeed: 1,
            canStopClose: false,
            hasCloseButton: false,
            message: null,
            url: null,
            hasArrow: true,
            arrowPosition: "auto",
            arrowPadding: null,
            arrowAlign: "auto", //默认auto，根据在容器内位置，设置弹出框的位置和按钮位置
            container: null //所在的容器，用于判断弹出框显示位置，默认为应用此控件的上级元素
        },
        ///初始化控件
        _create: function () {
            var self = this;
            var o = self.options;
            var currentobject = self.element ? self.element : parent; //当前元素

            switch (o.type) {
                case "custom":
                    currentobject.hover(
                        function () {
                            currentobject.timeout = setTimeout(function () {
                                self.createContent();
                            }, 1000);
                        },
                        function () {
                            clearTimeout(currentobject.timeout);
                            if (o.autoClose) {
                                self.closeTooltipWin();
                            }
                        }
                    );
                    break;
                case "normal":
                case "embed":
                default:
                    self.createContent();
                    if (o.autoClose) {
                        self.closeTooltipWin();
                    }
                    break;
            }
            return this;
        },
        createContent: function () {
            if (this.options.url) {
                var message = this._getMessageRemote();
            } else {
                this._renderContent();
            }
        },

        _renderContent: function () {
            var self = this,
                o = self.options;
            if (o.zIndex > $.ui.dialog.maxZ) {
                $.ui.dialog.maxZ = parseInt(o.zIndex);
            }
            var parent, position;
            if (o.type == "embed") {
                parent = self.element;
                position = "relative";
            } else {
                parent = "body";
                position = "absolute";
            }

            var tooltipWin = self.tooltipWin = $('<div class="jhtooltipWin"><div class="tooltipcontent"><span class="jhtooltip-message">' + o.message + '</span></div></div>').appendTo(parent)
                        .css({ "z-index": $.ui.dialog.maxZ, position: position });
            if (o.type == "custom") {
                tooltipWin.addClass("jhtooltip-custom");
            }
            //设置宽度和高度
            if (o.width) {
                width = o.width;
                tooltipWin.css({ "width": width });
            } else if (o.autoWidth) {
                width = $(".jhtooltip-message", tooltipWin).width() + 22;
                if (o.hasCloseButton) {
                    width += 12;
                }
                tooltipWin.css({ "width": width });
                o.width = width;
            } else {
                o.width = tooltipWin.width();
            }
            var messageContainer = $(".jhtooltip-message", tooltipWin);
            if (o.height) {
                tooltipWin.css({ "height": o.height });
                var padding = (o.height - messageContainer.parent().outerHeight()) / 2;
                if (padding > 0) {
                    messageContainer.parent().css({ "padding-top": padding, "padding-bottom": padding });
                }
            }
            else {
                messageContainer.css({ "padding": "10px" });
                o.height = tooltipWin.height();
            }

            //创建箭头
            if (o.hasArrow) {
                self._createArrow();
            }
            if (o.hasCloseButton) {
                self._renderCloseButton();
            }
            if (o.type != "embed") {
                self._setPosition();
            }

            if (o.autoClose && o.canStopClose) {
                self._bindTooltipWinHover();
            }
        },

        _getMessageRemote: function () {
            var self = this;
            $.ajax({
                type: "GET",
                url: self.options.url,
                success: function (dataSource) {
                    self._renderContent();
                    $.each(dataSource, function (key, value) {
                        var obj = $("#Tooltip_" + key);
                        if (obj.length > 0 && obj[0].tagName == "IMG") {
                            obj.attr("src", rootPath ? (rootPath + value) : value);
                        } else {
                            obj.text(value);
                        }
                    });
                },
                error: function (msg) {
                    
                }
            });
        },

        //创建箭头
        _createArrow: function () {
            var self = this,
                element = self.element,
                options = self.options,
                tooltipWin = self.tooltipWin;
            //根据tooltip在容器内位置，计算箭头位置
            if (options.arrowPosition == "auto") {
                var container = options.container ? options.container : element.parent(),
                    offset = element.offset(),
                    top = offset.top,
                    height = element.height(),
                    tooltipHeight = tooltipWin.height();

                if (top > 40 && top + height + tooltipHeight > container.offset().top + container.height()) {
                    options.arrowPosition = "bottom";
                } else {
                    options.arrowPosition = "top";
                }
            }
            //如果初始化时设置了箭头位置，那么不进行自适应
            if (options.arrowPosition == "bottom") {
                tooltipWin.append('<div class="arrowheadparentdown"><div class="arrowheadsdown jhtooltip-arrow"></div></div>');
            } else {
                tooltipWin.prepend('<div class="arrowheadparent"><div class="arrowheadstop jhtooltip-arrow"></div></div>');
            }
            $(".arrowheadparent", self.tooltipWin).css("width", tooltipWin.width());
        },
        //设置tooltip位置
        _setPosition: function () {
            var self = this,
                element = self.element,
                options = self.options,
                tooltipWin = self.tooltipWin,
                container = options.container ? options.container : element.parent(),
                offset = element.offset(),
                top = offset.top,
                left = offset.left,
                height = element.outerHeight(),
                width = element.outerWidth(),
                tooltipHeight = tooltipWin.height(),
                tooltipWidth = tooltipWin.width(),
                arrowPadding = options.arrowPadding ? options.arrowPadding : tooltipWidth / 2;
            //计算箭头左右位置
            if (options.arrowAlign == "auto") {
                if (left + width / 2 + tooltipWidth - arrowPadding > container.offset().left + container.width()) {
                    options.arrowAlign = "right";
                } else {
                    options.arrowAlign = "left";
                }
            }
            //计算tooltip的left
            var showLeft;
            if (options.arrowAlign == "left") {
                showLeft = left + width / 2 - arrowPadding - 4;
                $(".jhtooltip-arrow", self.tooltipWin).css("margin-left", arrowPadding);
            } else {
                // showLeft = left + width / 2 - tooltipWidth + arrowPadding * 2 - 2;
                showLeft = left + width / 2 - tooltipWidth + arrowPadding - 2;
                $(".jhtooltip-arrow", self.tooltipWin).css("margin-right", arrowPadding);
            }
            //计算tooltip的top
            var showTop;
            if (options.arrowPosition == "bottom") {
                showTop = top - self.tooltipWin.height() - 4;
            } else {
                showTop = top + height + 4;
            }
            self.tooltipWin.css({ "left": showLeft, "top": showTop });
        },
        _bindTooltipWinHover: function () {
            var self = this;
            self.tooltipWin.hover(
                    function () {
                        self.tooltipWin.stop(true, false);
                    },
                    function () {
                        self.closeTooltipWin();
                    }
                );
        },
        closeTooltipWin: function () {
            var self = this;
            if (!self.tooltipWin) { return; }
            var closeSpeed = self.options.closeSpeed * 1000;
            self.tooltipWin.delay(closeSpeed).hide(0, function () {
                self.destroy();
            });
        },

        _renderCloseButton: function () {
            var self = this;
            var closeButton = $("<span class='ui-jhtooltip-close'></span>").prependTo(self.tooltipWin)
                .click(function () {
                    self.destroy();
                });
        },
        destroy: function () {
            if (this.tooltipWin) {
                this.tooltipWin.remove();
                this.tooltipWin = null;
                $.Widget.prototype.destroy.call(this);
            }
        }
    });

})(jQuery);
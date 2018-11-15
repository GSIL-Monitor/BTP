/**
 * @fileOverview 这个文件是弹出窗口控件的源文件.
 * @author 李玲 2011/7/28
 * @version 1.0.0
 *
 * @depends：
 *	jquery.ui.core.js
 *	jquery.ui.widget.js
 *  jquery.ui.button.js
 *	jquery.ui.draggable.js
 *	jquery.ui.mouse.js
 *	jquery.ui.position.js
 *	jquery.ui.resizable.js
 *  jquery.ui.dialog.js
 *
 */
(function ($) {
	var uiDialogClasses =
		'ui-jhtablebox ' +
			'ui-widget ';
	/** @class 弹出窗口类，继承jquery ui的link ui.dialog类，基本的属性、方法请参考<a target="_blank" href="http://jqueryui.com/demos/dialog/">ui.dialog的api文档</a>
	 * @name jhtablebox
	 * @description 弹出窗口类，继承jquery ui的ui.dialog类，基本的属性、方法请参考<a target="_blank" href="http://jqueryui.com/demos/dialog/">ui.dialog的api文档</a>
	 * @property {boolean} canMinimize 窗口是否可最小化
	 * @property {json} customButtons 用户自定义按钮
	 * @property {string} type 窗口类型。'Alert'-提示窗口，'Confirm'-确认窗口，'Content'-内容窗口,'Tab'-带页签的窗口,默认为'Content'
	 * @property {string} closedByHide 关闭时是销毁还是隐藏窗口，true隐藏，false销毁，默认为销毁
	 * @property {json} buttons 设置窗口中的按钮，如果已经设置了窗口类型为'Alert'或'Confirm'，那么此处的设置无效。格式：{Ok:function(){}}或{Ok:{text:'',click:function(){}}}
	 * @property {string} title 窗口标题
	 * @property {string} content 窗口内容
	 * @property {string} containment 弹出窗口所在容器，窗口只能在此容器范围内拖动，默认为document
	 * @property {boolean} loadEverytime 应用于Tab窗口，设置每个页签的内容是否每次切换页签时都加载，如果设为true，那么每次切换页签时都刷新窗口内容，否则当切换页签时不刷新窗口内容，默认为false

	 * @example
	 *  $("#dialog").jhtablebox({
    *       canMinimize: true,
    *       beforeclose: function () { alert("beforeclose"); },
    *       close: function () { alert("close"); },
    *       minimize: function () { alert("minimize"); }
    *  });
	 * @example
	 * win = $("<div></div>").jhtablebox({
    *    title: "窗口",       //窗口标题
    *    type: "Tab",          //窗口类型，Tab为多页签形式窗口
    *    titleIcon: "sub_network",   //窗口小图标，类名
    *    canMinimize: true,          //是否可以最小化   
    *    minimize: function () { alert("minimize"); }, //最小化的回调函数
    *    tabs: [     //页签定义，下面几个属性为控件需要的属性，其他用户的数据可以也作为属性保存到tabs对象中
    *       { 
    *           tabName: '标签1',                      //页签显示名
    *           tabIcon: 'tabicon1',                   //页签小图标，类名
    *           url: 'http://www.google.com.hk',       //内容页的url地址
    *           selected: true                         //选中页
    *           currentClass:'selected'                //选中页签的小图标，类名
    *        },
    *       { tabName: '标签2', url: '2' }
    *    ]
    * });
	 */
	$.widget("ui.jhtablebox", $.ui.dialog,
		/** @lends jhtablebox.prototype */
		{
			_create: function () {
				var self = this,
					element = self.element,
					options = self.options;
				//李玲 2011-11-2 删除代码，解决窗口内事件不能触发的bug
				//options.content = options.content || element.html();
				self.originalTitle = element.attr('title');
				// #5742 - .attr() might return a DOMElement
				if (typeof self.originalTitle !== 'string') {
					self.originalTitle = '';
				}
				options.title = options.title || self.originalTitle;

				//是否显示阴影
				if (options.hasShadow) {
					uiDialogClasses += 'ui-jhtablebox-shadow ';
				}

				//窗口的标题
				var title = options.title || '&#160;',
					titleId = $.ui.dialog.getTitleId(element),
				//窗口对象
					uiDialog = (self.uiDialog = $('<div></div>'))
						.appendTo(document.body)
						.hide()
						.addClass(uiDialogClasses + options.dialogClass)
						.css({
							zIndex: options.zIndex
						})
						.attr('tabIndex', -1).css('outline', 0).keydown(function (event) {
							if (options.closeOnEscape && event.keyCode &&
								event.keyCode === $.ui.keyCode.ESCAPE) {
								self.close(event);
								event.preventDefault();
							}
						})
						.attr({
							role: 'jhtablebox',
							'aria-labelledby': titleId
						})
						.mousedown(function (event) {
							self.moveToTop(false, event);
						});

				self._customCreateButtons();
				//根据是否生成按钮，使用不同的样式
				var uiDialogContentClass;
				if (options.buttons) {
					uiDialogContentClass = 'ui-jhtablebox-element ui-jhtablebox-button-content';
				} else {
					uiDialogContentClass = 'ui-jhtablebox-element ui-jhtablebox-content ui-corner-bottom';
				}
				//窗口的内容区域
				var uiDialogContent = element
					.removeAttr('title')
					.addClass(uiDialogContentClass)
					.prependTo(uiDialog);
				if (options.type == 'Tab') {
					self.frame = $('<div></div>').appendTo(uiDialogContent);
					//self.frame = $('<iframe class="frame" frameborder="0"></iframe>').appendTo(uiDialogContent);
					uiDialogContent.addClass('ui-tabs-content').disableSelection();
				} else if (options.content) {
					uiDialogContent.html('<div class="ui-tabs-content-inside">' + options.content + '</div>');
				} else if (options.url) {
					var ram = new Date().getTime();
					self.frame = $('<iframe class="frame"  frameborder="0" src="' + options.url + '" id="frame_' + ram + '" name="frame_' + ram + '"></frame>').appendTo(uiDialogContent);
				} else {
					self.elementFromDocument = true;
				}

				if (options.type == 'Alert' || options.type == 'Confirm') {
					uiDialogContent.find(".ui-tabs-content-inside").css({ "height": "70px", "width": "278px", "display": "table-cell", "vertical-align": "middle", "padding":"10px 0 0 0" });
				}

				//创建窗口的标题栏
				if (options.type == 'Tab') {
					self = $.ui.jhtablebox._createTabTitleBar(self, title, titleId);
				} else {
					self._createSingleTitleBar(title, titleId);
				}

				if (options.draggable && $.fn.draggable) {
					self._makeDraggable();
				}
				if (options.resizable && $.fn.resizable) {
					self._makeResizable();
				}

				self.uiDialog.find('.ui-dialog-buttonpane').addClass('ui-corner-bottom');
				self._isOpen = false;

				if ($.fn.bgiframe) {
					uiDialog.bgiframe();
				}
			},
			_makeResizable: function (handles) {
				handles = (handles === undefined ? this.options.resizable : handles);
				var self = this,
					options = self.options,
				// .ui-resizable has position: relative defined in the stylesheet
				// but dialogs have to use absolute or fixed positioning
					position = self.uiDialog.css('position'),
					resizeHandles = (typeof handles === 'string' ?
						handles :
						'n,e,s,w,se,sw,ne,nw'
						);

				function filteredUi(ui) {
					return {
						originalPosition: ui.originalPosition,
						originalSize: ui.originalSize,
						position: ui.position,
						size: ui.size
					};
				}

				self.uiDialog.resizable({
					cancel: '.ui-jhtablebox-content,.ui-tabs-header',
					containment: 'document',
					alsoResize: self.element,
					maxWidth: options.maxWidth,
					maxHeight: options.maxHeight,
					minWidth: options.minWidth,
					minHeight: self._minHeight(),
					handles: resizeHandles,
					start: function (event, ui) {
						self.resizeMask = $('<div style="width:100%; height:100%; filter:alpha(opacity=0); opacity:0; background-color:#000; position:absolute; top:0; left:0;z-index:' + ($.ui.dialog.maxZ + 1) + '"></div>').appendTo("body");
						$(this).addClass("ui-dialog-resizing");
						self._trigger('resizeStart', event, filteredUi(ui));
					},
					resize: function (event, ui) {
						if (self.uiDialogTitleTabsContent) {
							//if (self.uiDialogTitleTabsContent.width() > )
							if (self.uiDialogTitleTabsContent.width() > self.tabUl.width() - 10) {
								self.tabUl.css("margin-left", 0);
								self.prevButton.addClass('ui-jhtablebox-tab-button-hide');
								self.nextButton.addClass('ui-jhtablebox-tab-button-hide');
							} else {
								self.prevButton.removeClass('ui-jhtablebox-tab-button-hide');
								self.nextButton.removeClass('ui-jhtablebox-tab-button-hide');
							}
						}
						if (self.frame) {
							self.frame.width(self.element.width() - 1).height(self.element.height() - 1);
						}

						self._trigger('resize', event, filteredUi(ui));
					},
					stop: function (event, ui) {
						self.resizeMask.remove();
						$(this).removeClass("ui-dialog-resizing");
						options.height = $(this).height();
						options.width = $(this).width();
						self._trigger('resizeStop', event, filteredUi(ui));
						$.ui.dialog.overlay.resize();
					}
				})
					.css('position', position)
					.find('.ui-resizable-se').addClass('ui-icon ui-icon-grip-diagonal-se');
			},
			/**
			 * 打开窗口
			 */
			open: function () {
				if (this._isOpen) { return; }

				var self = this,
					options = self.options,
					uiDialog = self.uiDialog;

				self.overlay = options.modal ? new $.ui.dialog.overlay(self) : null;
				self._size();
				self._position(options.position);
				uiDialog.show(options.show);

				uiDialog.find('.ui-dialog-buttonpane button').button();

				self.moveToTop(true);

				// prevent tabbing out of modal dialogs
				if (options.modal) {
					uiDialog.bind('keypress.ui-dialog', function (event) {
						if (event.keyCode !== $.ui.keyCode.TAB) {
							return;
						}

						var tabbables = $(':tabbable', this),
							first = tabbables.filter(':first'),
							last = tabbables.filter(':last');

						if (event.target === last[0] && !event.shiftKey) {
							first.focus(1);
							return false;
						} else if (event.target === first[0] && event.shiftKey) {
							last.focus(1);
							return false;
						}
					});
				}

				// set focus to the first tabbable element in the content area or the first button
				// if there are no tabbable elements, set focus on the dialog itself
				if (options.focusCancelButton) {
					$(uiDialog.find('.ui-dialog-buttonpane button:last')).eq(0).focus();
				} else {
					$(uiDialog.find('.ui-dialog-buttonpane button:first')).eq(0).focus();
					//            $(self.element.find(':tabbable').get().concat(
					//            uiDialog.find('.ui-dialog-buttonpane :tabbable').get().concat(
					//            uiDialog.get()))).eq(0).focus();
				}
	            var objTop = ($(window.parent).height() - this.uiDialog[0].clientHeight) / 2 + $(window.parent.document.documentElement).scrollTop() + $(window.parent.document.body).scrollTop();
	            this.uiDialog[0].style.top = (objTop > 39 ? objTop - 30 : (objTop > 0 ? objTop : 20)) + "px";
				self._isOpen = true;
				self._trigger('open');
				if (self.uiDialogTitleTabsContent) {
					//if (self.uiDialogTitleTabsContent.width() > )
					if (self.uiDialogTitleTabsContent.width() > self.tabUl.width() - 10) {
						self.tabUl.css("margin-left", 0);
						self.prevButton.addClass('ui-jhtablebox-tab-button-hide');
						self.nextButton.addClass('ui-jhtablebox-tab-button-hide');
					} else {
						self.prevButton.removeClass('ui-jhtablebox-tab-button-hide');
						self.nextButton.removeClass('ui-jhtablebox-tab-button-hide');
					}
				}

				if (self.frame) {
					self.frame.width(self.element.width()).height(self.element.height());
				}
				return self;
			},
			//创建简单窗口的标题栏
			_createSingleTitleBar: function (title, titleId) {
				var self = this,
					options = self.options,
					uiDialogTitlebarContainer = $('<div><div class ="ui-jhsingletablebox-leftcorner"></div><div class ="ui-jhsingletablebox-rightcorner"></div></div>').prependTo(self.uiDialog),
					uiDialogTitlebar = (self.uiDialogTitlebar = $('<div></div>'))
						.addClass(
							'ui-jhsingletablebox-titlebar ' +
								'ui-helper-clearfix'
						)
						.appendTo(uiDialogTitlebarContainer),
					uiDialogTitle = self.uiDialogTitle = $('<span></span>')
						.addClass('ui-dialog-title')
						.attr('id', titleId)
						.html(title)
						.prependTo(uiDialogTitlebar);
				self._createTitleButtons(uiDialogTitlebar);
			},
			/**
			 * 修改窗口标题
			 * @param {html} title 标题
			 */
			changeTitle: function (title) {
				this.uiDialogTitle.html(title);
			},

			changeContent: function (src) {
				if (this.frame) {
					this.frame.attr("src", src);
				} else {
					this.element.html(src);
				}
			},
			//创建按钮
			_customCreateButtons: function () {
				var self = this,
					options = self.options;
				var confirmText = options.confirmButtonText || "确定";
				var cancelText = options.cancelButtonText || "取消";
				if (options.type == 'Alert') {               //提示窗口
					options.resizable = false;
					options.modal = true;
					options.buttons =
					{
						Ok: {
							text: confirmText,
							click: function (event) {
								if (self._trigger('confirm', event, self)) {
									self.close(event);
								}
							}
						}
					};
				} else if (options.type == 'Confirm') {       //确认窗口
					options.resizable = false;
					options.modal = true;
					options.buttons = {
						Ok: {
							text: confirmText,
							click: function (event) {
								if (self._trigger('confirm', event, self)) {
									self.close(event);
								}
							}
						},
						Cancel: {
							text: cancelText,
							click: function (event) {
								self._trigger('cancel', event, self);
								self.close(event);
							}
						}
					};
				}
				self._createButtons(options.buttons);
			},
			_createTabTitle: function () {

			},
			_makeDraggable: function () {
				var self = this,
					options = self.options,
					doc = $(document),
					heightBeforeDrag;

				function filteredUi(ui) {
					return {
						position: ui.position,
						offset: ui.offset
					};
				}
				var containment = 'document';
				if (options.containment) {
					containment = options.containment;
				}
				self.uiDialog.draggable({
					cancel: '.ui-dialog-content,.ui-jhtablebox-content,.ui-jhtablebox-button-content, .ui-dialog-titlebar-close, .ui-jhtablebox-mask',
					handle: '.ui-dialog-titlebar',
					//  containment: 'document',
					containment: containment,
					start: function (event, ui) {
						heightBeforeDrag = options.height === "auto" ? "auto" : $(this).height();
						$(this).height($(this).height()).addClass("ui-dialog-dragging");
						self._trigger('dragStart', event, filteredUi(ui));
					},
					drag: function (event, ui) {
						self._trigger('drag', event, filteredUi(ui));
					},
					stop: function (event, ui) {
						options.position = [ui.position.left - doc.scrollLeft(),
							ui.position.top - doc.scrollTop()];
						$(this).removeClass("ui-dialog-dragging").height(heightBeforeDrag);
						self._trigger('dragStop', event, filteredUi(ui));
						$.ui.dialog.overlay.resize();
					}
				});
			},

			//创建标题栏上的按钮
			_createTitleButtons: function (parent) {
				var self = this,
					options = self.options,
					buttonContainerWidth = 0;

				self._createButton(parent, '关闭', 'ui-tabbox-icon-close', self._closeFunction);
				buttonContainerWidth = buttonContainerWidth + 21;

				if (options.canMinimize) {
					self._createButton(parent, '最小化', 'ui-tabbox-icon-min', self.minimize);
					buttonContainerWidth += 21;
				}
				var customButtons = options.customButtons;
				if (customButtons) {
					$(customButtons).each(function (i, btn) {
						self._createButton(parent, btn.tipText, btn.className, btn.closeFunction);
						buttonContainerWidth += 21;
					});
				}
				return buttonContainerWidth;
			},
			_closeFunction: function (event) {
				var self = this,
					options = self.options;
				if (options.closedByHide) {
					self.hide();
				} else {
					self.close(event);
				}
			},
			_createButton: function (parent, tipText, className, clickFunction) {
				var self = this,
					options = self.options,
					button = $('<a href="#"><span class="ui-icon ui-tabbox-icon ' + className + '" title="' + tipText + '">' + tipText + '</span></a>')
						.addClass('ui-dialog-titlebar-button ')
						.attr('role', 'button')
						.hover(
						function () {
							button.addClass('ui-tabbox-icon ui-tabbox-icon-hover');
						},
						function () {
							button.removeClass('ui-tabbox-icon ui-tabbox-icon-hover');
						})
						.mousedown(function () {
							button.addClass('ui-tabbox-icon ui-tabbox-icon-mousedown');
							return false;
						})
						.mouseup(function () {
							button.removeClass('ui-tabbox-icon ui-tabbox-icon-mousedown');
						})
						.mouseout(function () {
							button.removeClass('ui-tabbox-icon ui-tabbox-icon-mousedown');
						})
						.click(function (event) {
							if ($.isFunction(clickFunction)) {
								clickFunction.call(self, event);
							}
							return false;
						})
						.appendTo(parent);
			},

			/**
			 * @description 最小化窗口
			 * @return {jhtablebox} jhsingletablebox对象
			 * @example
			 * $("#win").jhtablebox('minimize');
			 */
			minimize: function (event) {
				/**
				 * @name jhtablebox#minimize
				 * @event
				 * @param {event} e
				 * @param {jhtablebox} jhtablebox jhtablebox 对象
				 * @description 最小化时的回调函数，如果返回false，将取消最小化事件
				 */
				if (false === this._trigger('minimize', event, self)) {
					return;
				}
				this._hide();
				return self;
			},

			/**
			 * @description 隐藏窗口方法
			 * @return {jhtablebox} jhtablebox
			 * @example
			 * $("#win").jhtablebox('hide');
			 */
			hide: function (event) {
				var self = this,
					maxZ, thisZ;

				/**
				 * @name jhtablebox#beforeClose
				 * @event
				 * @param {event} e
				 * @description 隐藏窗口或关闭窗口前的回调函数，如果返回false，将取消隐藏或关闭事件
				 */
				if (false === self._trigger('beforeClose', event)) {
					return;
				}
				self.uiDialog.removeClass('ui-jhtablebox-top');
				this._hide();
				return self;
			},

			/**
			 * @description 关闭窗口方法
			 * @example
			 * $("#win").jhtablebox('close');
			 */
			close: function (event) {
				var self = this;
				if (false === self._trigger('beforeClose', event)) {
					return;
				}
				self.uiDialog.removeClass('ui-jhtablebox-top');
				self.destroy();

				/**
				 * @name jhtablebox#close
				 * @event
				 * @param {event} e
				 * @description 关闭窗口时的回调函数
				 */
				self._trigger('close', event);
				self._adjustOtherWindow();
			},

			_hide: function () {
				var self = this,
					maxZ, thisZ;

				if (self.overlay) {
					self.overlay.destroy();
				}

				self.uiDialog.unbind('keypress.ui-dialog');

				self._isOpen = false;

				if (self.options.hide) {
					self.uiDialog.hide(self.options.hide);
				} else {
					self.uiDialog.hide();
				}

				$.ui.dialog.overlay.resize();
				self._adjustOtherWindow();
			},

			_adjustOtherWindow: function () {
				var self = this;
				// adjust the maxZ to allow other modal dialogs to continue to work (see #4309)
				if (self.options.modal) {
					var maxZ = 0;
					var maxDialog;
					$('.ui-jhtablebox').each(function () {
						if (this !== self.uiDialog[0]) {
							thisZ = $(this).css('z-index');
							if (!isNaN(thisZ) && thisZ > maxZ) {
								maxZ = thisZ;
								maxDialog = $(this);
							}
						}
					});
					$.ui.dialog.maxZ = maxZ;
					if (maxDialog) {
						maxDialog.children("div:last").jhtablebox("moveToTop");
					}
				} else {
					var maxZ = 0;
					var maxDialog;
					$('.ui-jhtablebox:visible').each(function () {
						if (this !== self.uiDialog[0]) {
							thisZ = $(this).css('z-index');
							if (!isNaN(thisZ) && thisZ > maxZ) {
								maxZ = thisZ;
								maxDialog = $(this);
							}
						}
					});
					if (maxDialog) {
						maxDialog.children("div:last").jhtablebox("moveToTop");
					}
				}
			},

			/**
			 * @description 获取当前弹出窗口的z-index值
			 * @example
			 * $("#win").jhtablebox('zIndex');
			 */
			zIndex: function () {
				return this.uiDialog.css("z-index");
			},

			// the force parameter allows us to move modal dialogs to their correct
			// position on open
			moveToTop: function (force, event) {
				var self = this,
					options = self.options,
					saveScroll;

				if (self.uiDialog.hasClass('ui-jhtablebox-top')) {
					return;
				}

				if (options.type != "Alert" && options.type != "Confirm" && options.modal == false) {
					$('.ui-jhtablebox-top').each(function () {
						$(this).removeClass('ui-jhtablebox-top');
						var element = $(this).children('.ui-jhtablebox-element');
						element.jhtablebox('mask', element);
					});
				}

				if ((options.modal && !force) ||
					(!options.stack && !options.modal)) {
					return self._trigger('focus', event);
				}

				if (options.zIndex > $.ui.dialog.maxZ) {
					$.ui.dialog.maxZ = options.zIndex;
				}
				if (self.overlay) {
					//$.ui.dialog.maxZ += 1;
					$.ui.dialog.maxZ = parseInt($.ui.dialog.maxZ) + 1; //肖连胜修改，此处如果不转换，有可能当成字符串相加了
					self.overlay.$el.css('z-index', $.ui.dialog.overlay.maxZ = $.ui.dialog.maxZ);
					//self.overlay.$el.css('z-index', $.ui.dialog.overlay.maxZ = self.element.closest('.ui-jhtablebox').css('z-index') - 1);
				}

				//Save and then restore scroll since Opera 9.5+ resets when parent z-Index is changed.
				//  http://ui.jquery.com/bugs/ticket/3193
				saveScroll = { scrollTop: self.element.attr('scrollTop'), scrollLeft: self.element.attr('scrollLeft') };
				//$.ui.dialog.maxZ += 1;
				$.ui.dialog.maxZ = parseInt($.ui.dialog.maxZ) + 1;
				self.uiDialog.css('z-index', $.ui.dialog.maxZ).addClass('ui-jhtablebox-top');
				self.element.attr(saveScroll);
				self._trigger('focus', event);

				//        var mask = $('.ui-jhtablebox-mask');
				//        if (mask.length > 0) {
				//            mask.remove();
				//        }
				if (self.uiTableboxMask) {
					self.uiTableboxMask.remove();
					self.uiTableboxMask = null;
				}
				return self;
			},

			mask: function (parent) {
				if (this.uiTableboxMask) {
					return;
				}
				this.uiTableboxMask = $('<div class="ui-jhtablebox-mask"></div>').prependTo(parent).width(parent.width()).height(parent.height());
			},

			/**
			 * @description 销毁窗口
			 * @example
			 * $("#win").jhtablebox('destroy');
			 */
			destroy: function () {
				var self = this;

				if (self.overlay) {
					self.overlay.destroy();
				}
				self.uiDialog.hide();
				//        self.element
				//			.unbind('.jhtablebox')
				//			.removeData('jhtablebox')
				//			.removeClass('ui-dialog-content ui-widget-content')
				//            .hide().appendTo('body');

				self.element
					.unbind('.jhtablebox')
					.removeData('jhtablebox')
					.removeClass('ui-dialog-content ui-widget-content')
					.hide();
				//如果不是在页面元素上直接应用的jhtablebox，删除element
				if (!self.elementFromDocument) {
					self.element.remove();
				} else {
					self.element.appendTo('body');
				}
				self.uiDialog.remove();
				if (self.originalTitle) {
					self.element.attr('title', self.originalTitle);
				}

				return self;
			}
		});
	$.extend($.ui.jhtablebox, {
		version: "1.0.0"
	});
})(jQuery);
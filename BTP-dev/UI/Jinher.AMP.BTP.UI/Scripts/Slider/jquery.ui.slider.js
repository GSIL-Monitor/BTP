/*
 * jQuery UI Slider 1.8.14
 *
 * Copyright 2011, AUTHORS.txt (http://jqueryui.com/about)
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * http://docs.jquery.com/UI/Slider
 *
 * Depends:
 *	jquery.ui.core.js
 *	jquery.ui.mouse.js
 *	jquery.ui.widget.js
 *
* @fileOverview 这个文件是滑块控件的源文件. 
* @author 张弘 2011/9/23  
* @version 1.0.0 *
*/
(function ($, undefined) {
    /** @class 滑块控件类
    * @name slider
    * @description 滑块控件类
    * @property {boolean,string,int} 滑块移动的动画效果
    * @property {int} distance 最大值和最小值的间距  
    * @property {int} max 最大值  
    * @property {int} min 最小值  
    * @property {string} orientation 滑条方向
    * @property {boolean} range 是否显示两个滑块以表示范围
    * @property {int} step 滑块每移动一下表示的距离  
    * @property {int} value 一个滑块的值，若有多个滑块，则表示第一个滑块的值
    * @property {string,array} values 多个滑块的值  
    */
    // number of pages in a slider
    // (how many times can you page up/down to go through the whole range)
    var numPages = 5;

    $.widget("ui.slider", $.ui.mouse, {

        widgetEventPrefix: "slide",

        options: {
            animate: false,
            distance: 0,
            max: 100,
            min: 0,
            orientation: "horizontal",
            range: false,
            step: 1,
            value: 100,
            values: null,
            /**  
            * @name slider#start  
            * @event  
            * @param {event} e  
            * @param {slider} slider 
            * @description 开始时触发的事件
            */
            start: null,
            /**  
            * @name slider#slide  
            * @event  
            * @param {event} e  
            * @param {slider} slider 
            * @description 滑动鼠标时触发的事件
            */
            slide: null,
            /**  
            * @name slider#change  
            * @event  
            * @param {event} e  
            * @param {slider} slider 
            * @description 滑块值改变时触发的事件
            */
            change: null,
            /**  
            * @name slider#stop  
            * @event  
            * @param {event} e  
            * @param {slider} slider 
            * @description 停止滑动滑块时触发的事件
            */
            stop: null
        },

        _create: function () {
            var self = this,
			o = this.options,
			existingHandles = this.element.find(".ui-slider-handle"),
			handle = "<a hidefocus='true' class='ui-slider-handle' href='#'></a>",
			handleCount = (o.values && o.values.length) || 1,
			handles = [];

            this._keySliding = false;
            this._mouseSliding = false;
            this._animateOff = true;
            this._handleIndex = null;
            this._detectOrientation();
            this._mouseInit();

            this.element
			.addClass("ui-slider" +
				" ui-slider-" + this.orientation +
				(o.disabled ? " ui-slider-disabled" : ""));

            this.range = $([]);

            if (o.range) {
                if (o.range === true) {
                    if (!o.values) {
                        o.values = [this._valueMin(), this._valueMin()];
                    }
                    if (o.values.length && o.values.length !== 2) {
                        o.values = [o.values[0], o.values[0]];
                    }
                }

                this.range = $("<div></div>")
				.appendTo(this.element)
				.addClass("ui-slider-range" +
                // note: this isn't the most fittingly semantic framework class for this element,
                // but worked best visually with a variety of themes				
				((o.range === "min" || o.range === "max") ? " ui-slider-range-" + o.range : ""));
            }

            for (var i = existingHandles.length; i < handleCount; i += 1) {
                handles.push(handle);
            }

            this.handles = existingHandles.add($(handles.join("")).appendTo(self.element));

            this.handle = this.handles.eq(0);

            this.handles.add(this.range).filter("a")
			.click(function (event) {
			    event.preventDefault();
			})
            .hover(function () {
                if (!o.disabled) {
                    $(this).addClass("ui-slider-handle-hover");
                }
            }, function () {
                $(this).removeClass("ui-slider-handle-hover");
            })
			.focus(function () {

			})
			.blur(function () {

			});

            this.handles.each(function (i) {
                $(this).data("index.ui-slider-handle", i);
            });

            this.handles
			.keydown(function (event) {
			    var ret = true,
					index = $(this).data("index.ui-slider-handle"),
					allowed,
					curVal,
					newVal,
					step;

			    if (self.options.disabled) {
			        return;
			    }

			    switch (event.keyCode) {
			        case $.ui.keyCode.HOME:
			        case $.ui.keyCode.END:
			        case $.ui.keyCode.PAGE_UP:
			        case $.ui.keyCode.PAGE_DOWN:
			        case $.ui.keyCode.UP:
			        case $.ui.keyCode.RIGHT:
			        case $.ui.keyCode.DOWN:
			        case $.ui.keyCode.LEFT:
			            ret = false;
			            if (!self._keySliding) {
			                self._keySliding = true;
			                $(this).addClass("ui-slider-handle-hover");
			                allowed = self._start(event, index);
			                if (allowed === false) {
			                    return;
			                }
			            }
			            break;
			    }

			    step = self.options.step;
			    if (self.options.values && self.options.values.length) {
			        curVal = newVal = self.values(index);
			    } else {
			        curVal = newVal = self.value();
			    }

			    switch (event.keyCode) {
			        case $.ui.keyCode.HOME:
			            newVal = self._valueMin();
			            break;
			        case $.ui.keyCode.END:
			            newVal = self._valueMax();
			            break;
			        case $.ui.keyCode.PAGE_UP:
			            newVal = self._trimAlignValue(curVal + ((self._valueMax() - self._valueMin()) / numPages));
			            break;
			        case $.ui.keyCode.PAGE_DOWN:
			            newVal = self._trimAlignValue(curVal - ((self._valueMax() - self._valueMin()) / numPages));
			            break;
			        case $.ui.keyCode.UP:
			        case $.ui.keyCode.RIGHT:
			            if (curVal === self._valueMax()) {
			                return;
			            }
			            newVal = self._trimAlignValue(curVal + step);
			            break;
			        case $.ui.keyCode.DOWN:
			        case $.ui.keyCode.LEFT:
			            if (curVal === self._valueMin()) {
			                return;
			            }
			            newVal = self._trimAlignValue(curVal - step);
			            break;
			    }

			    self._slide(event, index, newVal);

			    return ret;

			})
			.keyup(function (event) {
			    var index = $(this).data("index.ui-slider-handle");

			    if (self._keySliding) {
			        self._keySliding = false;
			        self._stop(event, index);
			        self._change(event, index);
			        $(this).removeClass("ui-slider-handle-hover");
			    }
			});

            this.minbtn = $('<span class = "ui-slider-minbtn"></span>')
            .appendTo(this.element)
            .hover(function () {
                $(this).addClass("ui-slider-minbtn-hover");
            }, function () {
                $(this).removeClass("ui-slider-minbtn-hover");
            })
            .mousedown(function (event) {
                self.mouse = true;
                self.intmin = setInterval(function () {
                    var ret = false,
					index = 0,
					allowed,
					curVal,
					newVal,
					step;

                    if (!self._keySliding) {
                        self._keySliding = true;
                        $(self.handles).addClass("ui-slider-handle-hover");
                        allowed = self._start(event, index);
                        if (allowed === false) {
                            return;
                        }
                    }

                    step = self.options.step;
                    if (self.options.values && self.options.values.length) {
                        curVal = newVal = self.values(index);
                    } else {
                        curVal = newVal = self.value();
                    }

                    if (curVal === self._valueMin()) {
                        return;
                    }
                    newVal = self._trimAlignValue(curVal - step);

                    self._slide(event, index, newVal);
                }, 50);
            })
            .mouseup(function (event) {
                self.mouse = false;
                window.clearInterval(self.intmin);
                var index = 0;

                if (self._keySliding) {
                    self._keySliding = false;
                    self._stop(event, index);
                    self._change(event, index);
                    $(self.handles).removeClass("ui-slider-handle-hover");
                }
            })
            .mouseout(function (event) {
                window.clearInterval(self.intmin);
                var index = 0;

                if (self._keySliding) {
                    self._keySliding = false;
                    self._stop(event, index);
                    self._change(event, index);
                    $(self.handles).removeClass("ui-slider-handle-hover");
                }
            });

            this.maxbtn = $('<span class = "ui-slider-maxbtn"></span>')
            .appendTo(this.element)
            .hover(function () {
                $(this).addClass("ui-slider-maxbtn-hover");
            }, function () {
                $(this).removeClass("ui-slider-maxbtn-hover");
            })
            .mousedown(function (event) {
                self.mouse = true;
                self.intmax = setInterval(function () {
                    var ret = false,
					index = 0,
					allowed,
					curVal,
					newVal,
					step;

                    if (!self._keySliding) {
                        self._keySliding = true;
                        $(self.handles).addClass("ui-slider-handle-hover");
                        allowed = self._start(event, index);
                        if (allowed === false) {
                            return;
                        }
                    }

                    step = self.options.step;
                    if (self.options.values && self.options.values.length) {
                        curVal = newVal = self.values(index);
                    } else {
                        curVal = newVal = self.value();
                    }

                    if (curVal === self._valueMax()) {
                        return;
                    }
                    newVal = self._trimAlignValue(curVal + step);

                    self._slide(event, index, newVal);
                }, 50);
            })
            .mouseup(function (event) {
                self.mouse = false; ;
                window.clearInterval(self.intmax);
                var index = 0;

                if (self._keySliding) {
                    self._keySliding = false;
                    self._stop(event, index);
                    self._change(event, index);
                    $(self.handles).removeClass("ui-slider-handle-hover");
                }
            })
            .mouseout(function (event) {
                window.clearInterval(self.intmax);
                var index = 0;

                if (self._keySliding) {
                    self._keySliding = false;
                    self._stop(event, index);
                    self._change(event, index);
                    $(self.handles).removeClass("ui-slider-handle-hover");
                }
            });

            this._refreshValue();
            this._animateOff = false;
        },

        destroy: function () {
            this.handles.remove();
            this.range.remove();

            this.element
			.removeClass("ui-slider" +
				" ui-slider-horizontal" +
				" ui-slider-vertical" +
				" ui-slider-disabled" +
				" ui-widget" +
				" ui-widget-content" +
				" ui-corner-all")
			.removeData("slider")
			.unbind(".slider");

            this._mouseDestroy();

            return this;
        },

        _mouseCapture: function (event) {
            var o = this.options,
			position,
			normValue,
			distance,
			closestHandle,
			self,
			index,
			allowed,
			offset,
			mouseOverHandle;

            if (o.disabled) {
                return false;
            }

            this.elementSize = {
                width: this.element.outerWidth(),
                height: this.element.outerHeight()
            };
            this.elementOffset = this.element.offset();

            position = { x: event.pageX, y: event.pageY };
            normValue = this._normValueFromMouse(position);
            distance = this._valueMax() - this._valueMin() + 1;
            self = this;
            this.handles.each(function (i) {
                var thisDistance = Math.abs(normValue - self.values(i));
                if (distance > thisDistance) {
                    distance = thisDistance;
                    closestHandle = $(this);
                    index = i;
                }
            });

            // workaround for bug #3736 (if both handles of a range are at 0,
            // the first is always used as the one with least distance,
            // and moving it is obviously prevented by preventing negative ranges)
            if (o.range === true && this.values(1) === o.min) {
                index += 1;
                closestHandle = $(this.handles[index]);
            }

            allowed = this._start(event, index);
            if (allowed === false) {
                return false;
            }
            this._mouseSliding = true;

            self._handleIndex = index;

            closestHandle
			.addClass("ui-slider-handle-hover")
			.focus();

            offset = closestHandle.offset();
            mouseOverHandle = !$(event.target).parents().andSelf().is(".ui-slider-handle");
            this._clickOffset = mouseOverHandle ? { left: 0, top: 0} : {
                left: event.pageX - offset.left - (closestHandle.width() / 2),
                top: event.pageY - offset.top -
				(closestHandle.height() / 2) -
				(parseInt(closestHandle.css("borderTopWidth"), 10) || 0) -
				(parseInt(closestHandle.css("borderBottomWidth"), 10) || 0) +
				(parseInt(closestHandle.css("marginTop"), 10) || 0)
            };

            if (!this.handles.hasClass("ui-slider-handle-hover")) {
                this._slide(event, index, normValue);
            }
            this._animateOff = true;
            return true;
        },

        _mouseStart: function (event) {
            return true;
        },

        _mouseDrag: function (event) {
            if (this.mouse) {
                return;
            }
            var position = { x: event.pageX, y: event.pageY },
			normValue = this._normValueFromMouse(position);

            this._slide(event, this._handleIndex, normValue);

            return false;
        },

        _mouseStop: function (event) {
            this.handles.removeClass("ui-slider-handle-hover");
            this._mouseSliding = false;

            this._stop(event, this._handleIndex);
            this._change(event, this._handleIndex);

            this._handleIndex = null;
            this._clickOffset = null;
            this._animateOff = false;

            return false;
        },

        _detectOrientation: function () {
            this.orientation = (this.options.orientation === "vertical") ? "vertical" : "horizontal";
        },

        _normValueFromMouse: function (position) {
            var pixelTotal,
			pixelMouse,
			percentMouse,
			valueTotal,
			valueMouse;

            if (this.orientation === "horizontal") {
                pixelTotal = this.elementSize.width;
                pixelMouse = position.x - this.elementOffset.left - (this._clickOffset ? this._clickOffset.left : 0);
            } else {
                pixelTotal = this.elementSize.height;
                pixelMouse = position.y - this.elementOffset.top - (this._clickOffset ? this._clickOffset.top : 0);
            }

            percentMouse = (pixelMouse / pixelTotal);
            if (percentMouse > 1) {
                percentMouse = 1;
            }
            if (percentMouse < 0) {
                percentMouse = 0;
            }
            if (this.orientation === "vertical") {
                percentMouse = 1 - percentMouse;
            }

            valueTotal = this._valueMax() - this._valueMin();
            valueMouse = this._valueMin() + percentMouse * valueTotal;

            return this._trimAlignValue(valueMouse);
        },

        _start: function (event, index) {
            var uiHash = {
                handle: this.handles[index],
                value: this.value()
            };
            if (this.options.values && this.options.values.length) {
                uiHash.value = this.values(index);
                uiHash.values = this.values();
            }
            return this._trigger("start", event, uiHash);
        },

        _slide: function (event, index, newVal) {
            var otherVal,
			newValues,
			allowed;

            if (this.options.values && this.options.values.length) {
                otherVal = this.values(index ? 0 : 1);

                if ((this.options.values.length === 2 && this.options.range === true) &&
					((index === 0 && newVal > otherVal) || (index === 1 && newVal < otherVal))
				) {
                    newVal = otherVal;
                }

                if (newVal !== this.values(index)) {
                    newValues = this.values();
                    newValues[index] = newVal;
                    // A slide can be canceled by returning false from the slide callback
                    allowed = this._trigger("slide", event, {
                        handle: this.handles[index],
                        value: newVal,
                        values: newValues
                    });
                    otherVal = this.values(index ? 0 : 1);
                    if (allowed !== false) {
                        this.values(index, newVal, true);
                    }
                }
            } else {
                if (newVal !== this.value()) {
                    // A slide can be canceled by returning false from the slide callback
                    allowed = this._trigger("slide", event, {
                        handle: this.handles[index],
                        value: newVal
                    });
                    if (allowed !== false) {
                        this.value(newVal);
                    }
                }
            }
        },

        _stop: function (event, index) {
            var uiHash = {
                handle: this.handles[index],
                value: this.value()
            };
            if (this.options.values && this.options.values.length) {
                uiHash.value = this.values(index);
                uiHash.values = this.values();
            }

            this._trigger("stop", event, uiHash);
        },

        _change: function (event, index) {
            if (!this._keySliding && !this._mouseSliding) {
                var uiHash = {
                    handle: this.handles[index],
                    value: this.value()
                };
                if (this.options.values && this.options.values.length) {
                    uiHash.value = this.values(index);
                    uiHash.values = this.values();
                }

                this._trigger("change", event, uiHash);
            }
        },

        value: function (newValue) {
            if (arguments.length) {
                this.options.value = this._trimAlignValue(newValue);
                this._refreshValue();
                this._change(null, 0);
                return;
            }

            return this._value();
        },

        values: function (index, newValue) {
            var vals,
			newValues,
			i;

            if (arguments.length > 1) {
                this.options.values[index] = this._trimAlignValue(newValue);
                this._refreshValue();
                this._change(null, index);
                return;
            }

            if (arguments.length) {
                if ($.isArray(arguments[0])) {
                    vals = this.options.values;
                    newValues = arguments[0];
                    for (i = 0; i < vals.length; i += 1) {
                        vals[i] = this._trimAlignValue(newValues[i]);
                        this._change(null, i);
                    }
                    this._refreshValue();
                } else {
                    if (this.options.values && this.options.values.length) {
                        return this._values(index);
                    } else {
                        return this.value();
                    }
                }
            } else {
                return this._values();
            }
        },

        _setOption: function (key, value) {
            var i,
			valsLength = 0;

            if ($.isArray(this.options.values)) {
                valsLength = this.options.values.length;
            }

            $.Widget.prototype._setOption.apply(this, arguments);

            switch (key) {
                case "disabled":
                    if (value) {
                        this.handles.filter(".ui-slider-handle-hover").blur();
                        this.handles.removeClass("ui-slider-handle-hover");
                        this.handles.attr("disabled", "disabled");
                        this.element.addClass("ui-disabled");
                    } else {
                        this.handles.removeAttr("disabled");
                        this.element.removeClass("ui-disabled");
                    }
                    break;
                case "orientation":
                    this._detectOrientation();
                    this.element
					.removeClass("ui-slider-horizontal ui-slider-vertical")
					.addClass("ui-slider-" + this.orientation);
                    this._refreshValue();
                    break;
                case "value":
                    this._animateOff = true;
                    this._refreshValue();
                    this._change(null, 0);
                    this._animateOff = false;
                    break;
                case "values":
                    this._animateOff = true;
                    this._refreshValue();
                    for (i = 0; i < valsLength; i += 1) {
                        this._change(null, i);
                    }
                    this._animateOff = false;
                    break;
            }
        },

        //internal value getter
        // _value() returns value trimmed by min and max, aligned by step
        _value: function () {
            var val = this.options.value;
            val = this._trimAlignValue(val);

            return val;
        },

        //internal values getter
        // _values() returns array of values trimmed by min and max, aligned by step
        // _values( index ) returns single value trimmed by min and max, aligned by step
        _values: function (index) {
            var val,
			vals,
			i;

            if (arguments.length) {
                val = this.options.values[index];
                val = this._trimAlignValue(val);

                return val;
            } else {
                // .slice() creates a copy of the array
                // this copy gets trimmed by min and max and then returned
                vals = this.options.values.slice();
                for (i = 0; i < vals.length; i += 1) {
                    vals[i] = this._trimAlignValue(vals[i]);
                }

                return vals;
            }
        },

        // returns the step-aligned value that val is closest to, between (inclusive) min and max
        _trimAlignValue: function (val) {
            if (val <= this._valueMin()) {
                return this._valueMin();
            }
            if (val >= this._valueMax()) {
                return this._valueMax();
            }
            var step = (this.options.step > 0) ? this.options.step : 1,
			valModStep = (val - this._valueMin()) % step;
            alignValue = val - valModStep;

            if (Math.abs(valModStep) * 2 >= step) {
                alignValue += (valModStep > 0) ? step : (-step);
            }

            // Since JavaScript has problems with large floats, round
            // the final value to 5 digits after the decimal point (see #4124)
            return parseFloat(alignValue.toFixed(5));
        },

        _valueMin: function () {
            return this.options.min;
        },

        _valueMax: function () {
            return this.options.max;
        },

        _refreshValue: function () {
            var oRange = this.options.range,
			o = this.options,
			self = this,
			animate = (!this._animateOff) ? o.animate : false,
			valPercent,
			_set = {},
			lastValPercent,
			value,
			valueMin,
			valueMax;

            if (this.options.values && this.options.values.length) {
                this.handles.each(function (i, j) {
                    valPercent = (self.values(i) - self._valueMin()) / (self._valueMax() - self._valueMin()) * 100;
                    _set[self.orientation === "horizontal" ? "left" : "bottom"] = valPercent + "%";
                    $(this).stop(1, 1)[animate ? "animate" : "css"](_set, o.animate);
                    if (self.options.range === true) {
                        if (self.orientation === "horizontal") {
                            if (i === 0) {
                                self.range.stop(1, 1)[animate ? "animate" : "css"]({ left: valPercent + "%" }, o.animate);
                            }
                            if (i === 1) {
                                self.range[animate ? "animate" : "css"]({ width: (valPercent - lastValPercent) + "%" }, { queue: false, duration: o.animate });
                            }
                        } else {
                            if (i === 0) {
                                self.range.stop(1, 1)[animate ? "animate" : "css"]({ bottom: (valPercent) + "%" }, o.animate);
                            }
                            if (i === 1) {
                                self.range[animate ? "animate" : "css"]({ height: (valPercent - lastValPercent) + "%" }, { queue: false, duration: o.animate });
                            }
                        }
                    }
                    lastValPercent = valPercent;
                });
            } else {
                value = this.value();
                valueMin = this._valueMin();
                valueMax = this._valueMax();
                valPercent = (valueMax !== valueMin) ?
					(value - valueMin) / (valueMax - valueMin) * 100 :
					0;
                _set[self.orientation === "horizontal" ? "left" : "bottom"] = valPercent + "%";
                this.handle.stop(1, 1)[animate ? "animate" : "css"](_set, o.animate);

                if (oRange === "min" && this.orientation === "horizontal") {
                    this.range.stop(1, 1)[animate ? "animate" : "css"]({ width: valPercent + "%" }, o.animate);
                }
                if (oRange === "max" && this.orientation === "horizontal") {
                    this.range[animate ? "animate" : "css"]({ width: (100 - valPercent) + "%" }, { queue: false, duration: o.animate });
                }
                if (oRange === "min" && this.orientation === "vertical") {
                    this.range.stop(1, 1)[animate ? "animate" : "css"]({ height: valPercent + "%" }, o.animate);
                }
                if (oRange === "max" && this.orientation === "vertical") {
                    this.range[animate ? "animate" : "css"]({ height: (100 - valPercent) + "%" }, { queue: false, duration: o.animate });
                }
            }
        }

    });

    $.extend($.ui.slider, {
        version: "1.8.14"
    });

} (jQuery));

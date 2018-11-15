/**
 * 获取定义的区域内 input select textarea 或自定义的值
 *
 * 用法: setData 初始对象传入的值 为object {
 *				element: '#search',                 需要查询数据的标签
 *				find_class_name: '.search',         抽取数据的className.
 *				find_class_data_name: 'search'      html中data前缀.
 *			}
 *
 *	在设置的 element 标签范围内 find_class_name 定义的 class 元素将被添加至对象内部.
 *  可设置 html data 相关属性值
 *      在 find_class_data_name 的基础上添加:
 *      key:  将成为对象内部元素的下标值.
 *      find: 定义搜索该标签下设置的属性值.
 *      find-data: 为find查询到时抽取的值.
 *
 * 对外访问方法: getAllData ()                        返回对象内部的 all_data 的值.
 *
 *            setElementEvent (callback)           设置对象内部每个元素的相关事件.
 *                                                  callback(element, index) {
 *                                                      element 为当前元素.
 *                                                      index 为当前下标
 *                                                  }
 *
 *            againSetListData (callback)          重新定义data_list中设置的值.
 *                                                  callback(element, index) {
 *                                                      element 为当前元素.
 *                                                      index 为当前下标
 *                                                  }
 *                                                  该方法返回修改后的 data_list 的值
 *
 *            getAjaxData ()                       返回 jax_data 信息
 *
 *            verificationData (callback)          重新定义data_list中设置的值.并将错误返回至error_list中.
 *                                                  callback(element, index) {
 *                                                      element 为当前元素.
 *                                                      index 为当前下标
 *                                                  }
 *                                                  如果定义在againSetListData后则验证againSetListData重新定义后的值
 *
 *            getErrorList ()                      返回error_list 信息
 *
 *            其中的callback为该方法内部遍历时使用.
 *            element为当前元素,
 *            index为如标签定义了find_class_data_name + Key 则为该值如果没有定义则为该元素在查询范围内的index值.
 *
 *
 * 例子:
 *  html code:
		<div id="search">
			<div>
				<select class="search">
					<option value="1">1</option>
					<option value="2">2</option>
					<option value="3">3</option>
				</select>
				<input class="search" type="text" value="22"/>
				<input class="search" type="checkbox" checked="checked" id="i_checkbox"/>
				<input class="search" type="radio" id="i_radio"/>
				<textarea class="search"></textarea>
			</div>
			<div>
				<ul class="search" data-search-find=".focus_1" data-search-key="ul_li">
					<li class="" data-search-find-data="l_1">l_1</li>
					<li class="" data-search-find-data="l_2">l_2</li>
					<li class="focus_1" data-search-find-data="l_3">l_3</li>
				</ul>
			</div>

			<input type="button" data-search-key="button" class="search" value="提交"/>
		</div>

    javascript code:
        $(function (){
			var s = new GetElementData({
				element: '#search',
				find_class_name: '.search',
				find_class_data_name: 'search'
			});

			console.log(s.getAllData());    //返回对象内 all_data 的值

			s.setElementEvent(function (element, index) {
				if(index == 'button') {
					element.click(function () {
					//定义一个对象.保存 againSetListData 返回的data_list
						var external_data = {};
						s.againSetListData(function (data, index, setFunction) {
							switch (index) {
								case 'button':
									external_data = setFunction(index, 22222);
									break;
								case 'ul_li':
									external_data = setFunction(index, 3333);
									break;
							}
						});

						s.verificationData(function (data, index) {
							console.log(data);
						});
						console.log(external_data);
						//Object {0: "1", 1: "22", 2: "on", 3: "", 4: "", ul_li: 3333, button: 22222}

						//返回没有被 againSetListData 重新改写的 data_list 对象
						console.log(s.getAjaxData());
						//Object {0: "1", 1: "22", 2: "on", 3: "", 4: "", ul_li: "l_3", button: "提交"}

					});
				}
			});
		})
 *
 *
 * @param setData  {element: '', find_class_name: '', find_class_data_name: ''}
 * @constructor
 */
var GetElementData = function (setData) {
	var self = this;
	//定义元素主发范围
	var parent = setData.element;
	//范围内class名称
	var class_name = setData.find_class_name;
	//范围内html data 前缀名称
	var data_name = setData.find_class_data_name;
	//该对象下标为该元素设置的KEY值,如果标签没有设置该元素.则默认为该标签在该范围内的index值
	//内容为{element: element, value: value}
	var all_data = {};
	//该对象保存充all_data中抽取出element元素的列表.
	var element_list = {};
	//重定义的element_list.下标为all_data的下标,内容为相对应的all_data.element值.
	var format_element_list = {};
	//对象保存all_data中抽取出来的value值.下标为all_data的下标.value为all_data.value
	var data_list = {};
	//组成ajax_data 对应的对象.{key:value}的形式
	var ajax_data = {};
	//保存验证时返回的错误列表.
	var error_list = [];
	//验证回调函数.
	var verificationCallback = {};

	/**
	 * 元素遍历方法.执行一个回调函数.并将当前的 元素 与 index 传入
	 * @param element
	 * @param callback
	 */
	function eachElement(element, callback) {
		element.each(function (index) {
			callback($(this), index);
		});
	}

	/**
	 * 获取需要查找的元素.并将数据放入 element_list 中
	 */
	var getElement = function () {
		element_list = $(parent).find(class_name);
	}();

	/**
	 * 设置内部All_data属性
	 */
	function setInternalData() {
//		getElement();

		getAllElementData();
	}

	/**
	 * 获取元素数据
	 * @param element   元素对象
	 * @param index     each遍历值
	 */
	function getAElementData(element, index) {
		var key = element.data(data_name + 'Key') || index;
		all_data[key] = {};
		all_data[key].element = element;
		format_element_list[key] = element;

		//获取input select textarea 下的value值
		switch (element[0].tagName.toLocaleLowerCase()) {
			case 'input':
				switch (element.attr('type').toLocaleLowerCase()) {
					case 'checkbox':
						if(element.is(':checked')) {
							all_data[key].value = returnValue(element);
						} else {
							all_data[key].value = '';
						}
						break;
					case 'radio':
						if(element.is(':checked')) {
							all_data[key].value = returnValue(element);
						} else {
							all_data[key].value = '';
						}
						break;
					default :
						all_data[key].value = returnValue(element);
				}
				break;
			case 'select':
				all_data[key].value = returnValue(element);
				break;
			case 'textarea':
				all_data[key].value = returnValue(element);
				break;
			default :
				//获取需要搜索元素内部的值, find 的值为继续搜索的关键值. 只获取第一次find找到的元素
				var isFind = element.data(data_name + 'Find');
				if(isFind) {
					all_data[key].value = element.find(isFind).data(data_name + 'FindData');
				} else {
					all_data[key].value = element.text();
				}
		}


		function returnValue (element) {
			return element.val() || '';
		}
	}

	/**
	 * 获取全部元素数据
	 */
	function getAllElementData() {
		eachElement(element_list, function (data, index) {
			getAElementData(data, index);
		});
	}

	/**
	 * 遍历格式化后的 format_element_list 数据
	 * @param callback  回调接收两个参数,第一个为当前元素,第二个为key值
	 */
	function eachFormatElementList (callback) {
		for(var i in format_element_list) {
			callback(format_element_list[i], i);
		}
	}

	/**
	 * 将data_all中的key与value赋值至data_list中
	 */
	function setDataList () {
		setInternalData();
		eachAllDataList(function (data, index) {
			data_list[index] = data;
		});
	}

	/**
	 * 遍历data_list.
	 * @param callback  回调接收两个参数,第一个为当前元素,第二个为key值
	 */
	function eachDataList (callback) {
		setDataList();
		for(var i in data_list) {
			callback(data_list[i], i);
		}
	}

	function eachExternalDataList(callback) {
		self.getAjaxData();
		for(var i in data_list) {
			callback(data_list[i], i);
		}
	}

	/**
	 * 遍历 all_data 数据.
	 * @param callback
	 */
	function eachAllDataList(callback) {
		for(var i in all_data) {
			callback(all_data[i].value, i);
		}
	}

	/**
	 * 设置 ajax_data 的值供 ajax 调用
	 *
	 * @param new_data_list 新定义的data_list
	 */
	function setAjaxData () {
		eachDataList(function (data, index) {
			ajax_data[index] = data;
		});
	}

	/**
	 * 供外部重新设置 data_list 的值并将最新列表返回
	 * @param index   当前 index  or  key
	 * @param data    修改的数据
	 * @returns {{}}  将对象返回.
	 */
	function externalSetDataList (index, data) {
		data_list[index] = data;
		return data_list;
	}

	/**
	 * 将 all_data 提供外部访问
	 * @returns {{}} 将对象内部 all_data 返回
	 */
	self.getAllData = function () {
		setInternalData();
		return all_data;
	};

	/**
	 * 供外部设置内部元素相关事件.
	 * @param callback
	 */
	self.setElementEvent = function (callback) {
		eachFormatElementList(callback);
	};

	/**
	 * 供外部重新设置
	 * @param callback
	 */
	self.againSetListData = function (callback) {
		eachDataList(function (data, index) {
			callback(data, index, externalSetDataList);
		});
	};

	/**
	 * 供外部获取对象内ajax_data值
	 * @returns {{}}
	 */
	self.getAjaxData = function () {
		setAjaxData();
		return ajax_data;
	};

	/**
	 * 供外部验证方法.
	 * @param callback
	 */
	self.verificationData = function (callback) {
		var r_error = [];

		verificationCallback = callback;

		eachExternalDataList(function (data, index) {
			var return_str = callback(data, index);
			return_str ? r_error.push({error:return_str, element:all_data[index].element}) : '';
		});

		error_list = r_error;
	};

	self.getErrorList = function () {
		self.verificationData(verificationCallback);
		return error_list;
	};

    self.getParent = function () {
        return $(parent);
    };

	self.getElementList = function () {
		return format_element_list;
	}
};
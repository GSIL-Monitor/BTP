/**
 * 查询并获取某元素区域内元素与数据.
 * @param setInfo       传入一对象.设置默认信息.{search_parent: dom_object, data_name: 查询时关键下标属性}
 * @constructor
 */
function SearchElement(setInfo) {
	var self                = this;//定义自身
	var search_parent       = setInfo.search_parent;//查询该元素
	var html_data_name      = setInfo.data_name;//查询关键前置下标
	var element_list        = [];//保存元素供遍历时获取其中内容.保存为一对象{obj, key}
	var element_obj_list    = {};//将保存的元素列表内值提取出来.保存为一对象{key: {obj, objValue : {value, verificationValue}}}
	var element_data        = {};//将保存的元素列表内值提取出来.保存为一对象{key: value}
	var errorList           = [];//保存由verificationData方法返回的错误信息
	var addErrorList        = [];//保存element_obj_list中objValue内的verificationValue返回的错误信息

	/**
	 * 取元素范围内全部tag对象
	 * 将 关键下标 + -key 的内容取出.并作为将来的key值保存至element_list数组中
	 */
	function getElementList() {
		var elements = search_parent.getElementsByTagName('*');
		for (var i = 0; i < elements.length; i++) {
			var for_element = elements[i].getAttribute(html_data_name + '-key');
			if (for_element) {
				element_list.push({obj: elements[i], key: for_element});
			}
		}
	}

	/**
	 * 当new 该对象时初始化该对象.
	 */
	(function () {
		getElementList();
	}());

	/**
	 * 获取该元素内的数据.
	 * 如是input等带有value属性的则取value内的值.则将获取 关键下标 + '-value' 属性内的值
	 *
	 * 如果设置了 关键下标 + '-find' 属性,属性规则为(findTagName, findKey),
	 * 搜索该标签内 findTagName 标签,带有 关键下标 + '-findKey' 标签的 关键下标 + -value 值
	 *
	 * @param element
	 * @returns {*}
	 */
	function getElementValue(element) {
		switch (element.tagName.toLocaleLowerCase()) {
			case 'input':
				switch (element.type) {
					case 'checkbox':
						return element.checked ? element.value : '';
					case 'radio':
						return element.checked ? element.value : '';
					default :
						return element.value;
				}
			case 'textarea':
				return element.value;
			case 'select':
				return element.value;
			default :
				if (element.getAttribute(html_data_name + '-find')) { //是否带有 '-find'
					var a = element.getAttribute(html_data_name + '-find').split('-'),
						for_element = element.getElementsByTagName(a[0]),
						tmp_arr = [];
					for (var i = 0; i < for_element.length; i++) {
						//此处待优化.使用了递归.
						if(a[0] == 'input' && for_element[i].checked) {
							tmp_arr.push(for_element[i].value);
						} else if (for_element[i].getAttribute(html_data_name + '-' + a[1])) {
							tmp_arr.push(for_element[i].getAttribute(html_data_name + '-value'));
						}
					}
					return tmp_arr;
				} else if (element.getAttribute(html_data_name + '-value')) { //则获取自身的 '-value'
					return element.getAttribute(html_data_name + '-value');
				}
				//都没有则返回空
				return '';
		}
	}

	/**
	 * 将element_list内的数据抽取出来.并设置 element_data 与 element_obj_list 值
	 */
	function setData() {
		var key,
			value;
		for (var i = 0; i < element_list.length; i++) {
			key = element_list[i].key;
			value = {
				value: getElementValue(element_list[i].obj),
				verificationValue: verificationValue
			};

			element_data[key] = value;
			element_obj_list[key] = {obj: element_list[i].obj, valueObj: value};
		}
	}

	/**
	 * 遍历 element_obj_list 对象.
	 * 传入一回调函数,函数接收(index, {elementObj, valueObj})
	 * @param callback
	 */
	function eachElementObjList(callback) {
		for (var i in element_obj_list) {
			callback(i, element_obj_list[i]);
		}
	}

	/**
	 * 遍历 element_data 对象.
	 * 传入一回调函数,函数接收(index, element)
	 * 该回调函数的返回值将插入 errorList 数组中,成为将来的错误信息.
	 * @param callback
	 */
	function eachElementList(callback) {
		errorList = [];
		for (var i in element_data) {
			var data = callback(i, element_data[i]);
			data ? errorList.push(data) : '';
		}
	}

	/**
	 * 设置 element_obj_list 内的 valueObj 对象中 verificationValue 方法.
	 * 该方法为验证数据方法,回调函数的返回值将插入 addErrorList 数组内.成为将来的错误信息
	 * @param callback
	 */
	function verificationValue(callback) {
		var data = callback();
		data ? addErrorList.push(data) : '';
	}

	/**
	 * 供外部获取 element_data 对象
	 * @returns {{}}
	 */
	self.getObjData = function () {
		setData();
		return element_data;
	};

	/**
	 * 供外部获取 element_obj_list 对象
	 * @returns {{}}
	 */
	self.getElementObjList = function () {
		setData();
		return element_obj_list;
	};

	/**
	 * 供外设置某一元素事件.
	 * @param elementName
	 * @returns {*}
	 */
	self.setElementEvent = function (elementName) {
		setData();
		return element_obj_list[elementName].obj;
	};

	/**
	 * 遍历 element_obj_list 对象.回调函数接收(index, {elementObj, valueObj})
	 * @param callback
	 */
	self.eachSetElementEvent = function (callback) {
		setData();
		eachElementObjList(callback);
	};

	/**
	 * 遍历 element_data 对象.回调接收(index, element),该回调返回的值将插入错误列表
	 * @param callback
	 */
	self.verificationData = function (callback) {
		setData();
		eachElementList(callback);
	};

	/**
	 * 返回 错误列表
	 * @returns {Array}
	 */
	self.getErrorList = function () {
		return errorList.concat(addErrorList);
	};

	/**
	 * 清空 addErrorList 列表
	 */
	self.emptyAddErrorList = function () {
		addErrorList = [];
	};
}
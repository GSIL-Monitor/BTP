(function($){
/**
 * jqGrid Chinese Translation
 * LiLing
 * http://www.jh0101.com/
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = {
	defaults : {
		recordtext: "当前显示{0}-{1}条记录 总记录数{2}",
		emptyrecords: "没有记录",
		loadtext: "加载中...",
		pgtext : "第{0}页 共{1}页"
	},
	search : {
		caption: "查询",
		Find: "查找",
		Reset: "复位",
		odata : ['等于', '不等于', '小于', '小于等于','大于','大于等于', 'begins with','does not begin with','is in','is not in','ends with','does not end with','contains','does not contain'],
		groupOps: [	{ op: "AND", text: "all" },	{ op: "OR",  text: "any" }	],
		matchText: " match",
		rulesText: " rules"
	},
	edit : {
		addCaption: "添加新记录",
		editCaption: "编辑记录",
		bSubmit: "提交",
		bCancel: "取消",
		bClose: "关闭",
		saveData: "Data has been changed! Save changes?",
		bYes : "是",
		bNo : "否",
		bExit : "取消",
		msg: {
			required:"Field is required",
			number:"请输入有效的数字",
			minValue:"数据必须大于或等于 ",
			maxValue:"数据必须小于或等于",
			email: "不是有效的邮箱地址",
			integer: "Please, enter valid integer value",
			date: "请输入有效的日期",
			url: "is not a valid URL. Prefix required ('http://' or 'https://')",
			nodefined : " is not defined!",
			novalue : " return value is required!",
			customarray : "Custom function should return array!",
			customfcheck : "Custom function should be present in case of custom checking!"
			
		}
	},
	view : {
		caption: "查看记录",
		bClose: "关闭"
	},
	del : {
		caption: "删除",
		msg: "是否要删除选中的记录？",
		bSubmit: "删除",
		bCancel: "取消"
	},
	nav : {
		edittext: "",
		edittitle: "编辑选择的行",
		addtext:"",
		addtitle: "添加新行",
		deltext: "",
		deltitle: "删除选择的行",
		searchtext: "",
		searchtitle: "查找记录",
		refreshtext: "",
		refreshtitle: "刷新",
		alertcap: "警告",
		alerttext: "请选择一行",
		viewtext: "",
		viewtitle: "查看选择的行"
	},
	col : {
		caption: "选择列",
		bSubmit: "确定",
		bCancel: "取消"
	},
	errors : {
		errcap : "错误",
		nourl : "没有设置url地址",
		norecords: "No records to process",
		model : "Length of colNames <> colModel!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0.00'},
		currency : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0.00'},
		date : {
			dayNames:   [
				"Sun", "Mon", "Tue", "Wed", "Thr", "Fri", "Sat",
				"星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"
			],
			monthNames: [
				"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
				"一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"
			],
			AmPm : ["am","pm","AM","PM"],
			S: function (j) {return j < 11 || j > 13 ? ['st', 'nd', 'rd', 'th'][Math.min((j - 1) % 10, 3)] : 'th'},
			srcformat: 'Y-m-d',
			newformat: 'd/m/Y',
			masks : {
				ISO8601Long:"Y-m-d H:i:s",
				ISO8601Short:"Y-m-d",
				ShortDate: "n/j/Y",
				LongDate: "l, F d, Y",
				FullDateTime: "l, F d, Y g:i:s A",
				MonthDay: "F d",
				ShortTime: "g:i A",
				LongTime: "g:i:s A",
				SortableDateTime: "Y-m-d\\TH:i:s",
				UniversalSortableDateTime: "Y-m-d H:i:sO",
				YearMonth: "F, Y"
			},
			reformatAfterEdit : false
		},
		baseLinkUrl: '',
		showAction: '',
		target: '',
		checkbox : {disabled:true},
		idName : 'id'
	}
};
})(jQuery);

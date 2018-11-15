; (function ($) {
    $.jgrid.formatCol = function (pos, rowInd) {		    
		    var ral = ts.p.colModel[pos].align, result = "style=\"", clas = ts.p.colModel[pos].classes;
		    if (ral) result += "text-align:" + ral + ";";
		    if (ts.p.colModel[pos].hidden === true) result += "display:none;";
            //添加代码 李玲 2011-8-8 添加自动换行功能
            if(ts.p.colModel[pos].wordWrap) result += "word-wrap:break-word;line-height:22px;";
		    if (rowInd === 0) {
		        result += "width: " + grid.headers[pos].width + "px;"
		    }
		    return result + "\"" + (clas !== undefined ? (" class=\"" + clas + "\"") : "");
		};
})(jQuery);
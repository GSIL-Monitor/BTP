var scrollEvent = {
	id:"",
	getHashID:function(){
		var hash = (!window.location.hash) ? "#up" : window.location.hash;
		var id = hash.replace("#","");
		return id;
	},
	//滚动条在Y轴上的滚动距离
	getScrollTop:function(){
		var scrollTop = 0, bodyScrollTop = 0, documentScrollTop = 0;
		if(document.body){
			bodyScrollTop = document.body.scrollTop;
		}
		if(document.documentElement){
			documentScrollTop = document.documentElement.scrollTop;
		}
		scrollTop = (bodyScrollTop - documentScrollTop > 0) ? bodyScrollTop : documentScrollTop;
		return scrollTop;
	},
	//文档的总高度
	getScrollHeight:function(){
		var scrollHeight = 0, bodyScrollHeight = 0, documentScrollHeight = 0;
		if(document.body){
			bodyScrollHeight = document.body.scrollHeight;
		}
		if(document.documentElement){
			documentScrollHeight = document.documentElement.scrollHeight;
		}

		scrollHeight = (bodyScrollHeight - documentScrollHeight > 0) ? bodyScrollHeight : documentScrollHeight;
		return scrollHeight;
	},
	//浏览器视口的高度
	getWindowHeight:function(){
	　　var windowHeight = 0;
		if(document.compatMode == "CSS1Compat"){
			windowHeight = document.documentElement.clientHeight;
		}else{
			windowHeight = document.body.clientHeight;
		}
		return windowHeight;		
	},
	scroll:function(){
		window.onscroll = function(){
			var timer = null;
			if(this.getScrollTop() + this.getWindowHeight() == this.getScrollHeight()){
				if(!timer){
					var id = this.getHashID();
					timer = setTimeout(function(){
						var div = document.createElement("div");
						div.style.fontSize = "1.3rem";
						div.style.textAlign = "center";
						div.innerText = "加载中";
						document.getElementById(id).appendChild(div);
						timer = null;
					}.bind(this),500);
				}	
			}
		}.bind(this);
	}
}
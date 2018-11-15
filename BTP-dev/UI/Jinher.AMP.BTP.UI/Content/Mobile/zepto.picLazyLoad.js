/**
 * @fileOverview Zepto picLazyLoad Plugin
 * @author <a href="http;//www.uedcool.com">zhangyh</a>
 * @version 1.0
 */
/**
* 页面中所有带有“picLazyLoadImg”样式的图片将会使用懒加载
**/
;(function($){
    $.fn.picLazyLoad = function(settings){
        var $this = $(this),
            _winScrollTop = 0,
            _winHeight = $(window).height(),
            $all;

        settings = $.extend({
            class:"picLazyLoadImg",
            threshold: 0, // 提前高度加载
            placeholder: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyJpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNiAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6MzhENkYyMzdCMDJCMTFFNEI1RkJDOTI3QzM5NEU5OUEiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6MzhENkYyMzhCMDJCMTFFNEI1RkJDOTI3QzM5NEU5OUEiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDozOEQ2RjIzNUIwMkIxMUU0QjVGQkM5MjdDMzk0RTk5QSIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDozOEQ2RjIzNkIwMkIxMUU0QjVGQkM5MjdDMzk0RTk5QSIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pn6s4YEAAAAQSURBVHjaYv7//z8DQIABAAkLAwFHaU43AAAAAElFTkSuQmCC'
        }, settings||{});

        // 执行懒加载图片
        lazyLoadPic();

        // 滚动触发换图
        $(window).on('scroll',function(){
            _winScrollTop = $(window).scrollTop();
            lazyLoadPic();
        });

        // 懒加载图片
        function lazyLoadPic(){
            $all = $this.find("." + settings.class);
            $all.each(function(){
                var $self = $(this);
                // 如果是img
                if($self.is('img')){
                    if($self.attr('data-original')){
                        var _offsetTop = $self.offset().top;
                        if((_offsetTop - settings.threshold) <= (_winHeight + _winScrollTop)){
                            $self.attr('src',$self.attr('data-original'));
                            $self.removeAttr('data-original');
                        }
                    }
                // 如果是背景图
                }else{
                    if($self.attr('data-original')){
                        // 默认占位图片
                        if($self.css('background-image') == 'none'){
                            $self.css('background-image','url('+settings.placeholder+')');
                        }
                        var _offsetTop = $self.offset().top;
                        if((_offsetTop - settings.threshold) <= (_winHeight + _winScrollTop)){
                            $self.css('background-image','url('+$self.attr('data-original')+')');
                            $self.removeAttr('data-original');
                        }
                    }
                }
            });
        }
    }
})(Zepto);

$(function(){
    $("body").picLazyLoad();
});